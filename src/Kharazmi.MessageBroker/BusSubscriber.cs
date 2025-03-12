using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration.Consume;
using RawRabbit.Configuration.Exchange;
using RawRabbit.Configuration.Queue;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Operations.Subscribe.Context;
using RawRabbit.Pipe;
using Policy = Polly.Policy;

namespace Kharazmi.MessageBroker
{
    /// <summary>
    /// 
    /// </summary>
    public class BusSubscriber : IBusSubscriber
    {
        private readonly IBusClient _busClient;

        private readonly int _retries;
        private readonly int _retryInterval;
        private readonly ILogger<BusSubscriber> _logger;
        private readonly RabbitMqOptions _options;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        public BusSubscriber(IApplicationBuilder app)
        {
            _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            _logger = _serviceProvider.GetService<ILogger<BusSubscriber>>();
            _busClient = _serviceProvider.GetService<IBusClient>();
            _options = _serviceProvider.GetService<RabbitMqOptions>();
            _retries = _options.Retries >= 0 ? _options.Retries : 3;
            _retryInterval = _options.RetryInterval > 0 ? _options.RetryInterval : 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        public IBusSubscriber SubscribeCommand<TCommand>(Func<TCommand, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            _busClient.SubscribeAsync<TCommand, DomainContext>(async (command, domainContext) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainDispatcher>();

                return await TryHandleAsync(command, domainContext,
                    async () => await dispatcher.SendCommandAsync(command, domainContext, cancellationToken).ConfigureAwait(false), onError,
                    cancellationToken).ConfigureAwait(false);
            }, UseThrottledConsume, token: cancellationToken);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageConfiguration"></param>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        public IBusSubscriber SubscribeCommandFrom<TCommand>(
            MessageConfiguration messageConfiguration = null,
            Func<TCommand, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            SubscribeFrom<TCommand>(messageConfiguration, async (command, domainContext) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainDispatcher>();

                return await TryHandleAsync(command, domainContext,
                    async () => await dispatcher.SendCommandAsync(command, domainContext, cancellationToken).ConfigureAwait(false),
                    onError, cancellationToken).ConfigureAwait(false);
            }, cancellationToken);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public IBusSubscriber SubscribeEvent<TEvent>(
            Func<TEvent, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default)
            where TEvent : class, IDomainEvent
        {
            if (typeof(TEvent).IsAbstract)
                throw new RabbitMqException(typeof(TEvent).FullName + " must be non abstract class");
            
            _busClient.SubscribeAsync<TEvent, DomainContext>(async (@event, domainContext) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainDispatcher>();

                return await TryHandleAsync(@event, domainContext,
                    async () => await dispatcher.RaiseEventAsync(@event, domainContext, cancellationToken).ConfigureAwait(false), onError,
                    cancellationToken).ConfigureAwait(false);
            }, UseThrottledConsume, token: cancellationToken);

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageConfiguration"></param>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public IBusSubscriber SubscribeEventFrom<TEvent>(
            MessageConfiguration messageConfiguration = null,
            Func<TEvent, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default)
            where TEvent : class, IDomainEvent
        {
            if (typeof(TEvent).IsAbstract)
                throw new RabbitMqException(typeof(TEvent).FullName + " must be non abstract class");
            
            SubscribeFrom<TEvent>(messageConfiguration, async (@event, domainContext) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainDispatcher>();

                return await TryHandleAsync(@event, domainContext,
                    async () => await dispatcher.RaiseEventAsync(@event, domainContext, cancellationToken).ConfigureAwait(false),
                    onError, cancellationToken).ConfigureAwait(false);
            }, cancellationToken);

            return this;
        }

        private void SubscribeFrom<TMessage>(
            MessageConfiguration messageConfiguration,
            Func<TMessage, DomainContext, Task<Acknowledgement>> subscribeMethod,
            CancellationToken cancellationToken = default)
        {
            var exchangeName = typeof(TMessage).GetOrCreateExchangeName(_options);
            var exchangeType = typeof(TMessage).GetExchangeType();
            var queueName = typeof(TMessage).GetCreateQueueName(_options);
            var routingKey = typeof(TMessage).GetCreateRoutingKey(_options);
            var queueNameConfig = "";
            var queueNameBindExchange = "";

            var exchangeConfig = messageConfiguration?.ExchangeConfiguration;
            var queueConfig = messageConfiguration?.QueueConfiguration;

            Action<IConsumeConfigurationBuilder> consumeConfigBuilder = consumeConfig => consumeConfig
                .OnExchange(exchangeName)
                .FromQueue(queueName)
                .WithRoutingKey(routingKey);

            Action<IExchangeDeclarationBuilder> exchange = exchangeBuilder => exchangeBuilder
                .WithName(exchangeName)
                .WithType(exchangeType);

            Action<IQueueDeclarationBuilder> queue = queueBuilder => queueBuilder
                .WithName(queueName);


            if (exchangeConfig != null)
            {
                consumeConfigBuilder = builder =>
                {
                    if (exchangeConfig.ExchangeName.IsNotEmpty())
                        exchangeName = exchangeConfig.ExchangeName;

                    if (Enum.GetName(typeof(ExchangeType), exchangeConfig.ExchangeType).IsNotEmpty())
                        exchangeType = exchangeConfig.ExchangeType;

                    if (exchangeConfig.RoutingKey.IsNotEmpty())
                        routingKey = exchangeConfig.RoutingKey;

                    if (exchangeConfig.QueueName.IsNotEmpty())
                        queueNameBindExchange = exchangeConfig.QueueName;

                    builder.OnExchange(exchangeName)
                        .FromQueue(queueName)
                        .WithRoutingKey(routingKey)
                        .WithPrefetchCount(exchangeConfig.PrefetchCount)
                        .WithAutoAck(exchangeConfig.AutoAck)
                        .WithNoLocal(exchangeConfig.NoLocal);

                    if (exchangeConfig.ConsumerTag.IsNotEmpty())
                        builder.WithConsumerTag(exchangeConfig.ConsumerTag);

                    if (exchangeConfig.Arguments == null || exchangeConfig.Arguments.Count <= 0) return;
                    foreach (var argument in exchangeConfig.Arguments)
                    {
                        builder.WithArgument(argument.Key, argument.Value);
                    }
                };

                exchange = exchangeBuilder =>
                {
                    exchangeBuilder
                        .WithName(exchangeName)
                        .WithDurability(exchangeConfig.Durability)
                        .WithAutoDelete(exchangeConfig.AutoDelete);

                    if (Enum.GetName(typeof(ExchangeType), exchangeConfig.ExchangeType).IsNotEmpty())
                        exchangeBuilder.WithType(exchangeConfig.ExchangeType);

                    if (exchangeConfig.Arguments == null || exchangeConfig.Arguments.Count <= 0) return;
                    foreach (var argument in exchangeConfig.Arguments)
                    {
                        exchangeBuilder.WithArgument(argument.Key, argument.Value);
                    }
                };
            }

            if (queueConfig != null)
            {
                queue = queueBuilder =>
                {
                    if (queueConfig.Name.IsNotEmpty())
                        queueNameConfig = queueConfig.Name;

                    queueBuilder.WithName(queueName)
                        .WithDurability(queueConfig.Durability)
                        .WithAutoDelete(queueConfig.AutoDelete);

                    if (queueConfig.NameSuffix.IsNotEmpty())
                        queueBuilder.WithNameSuffix(queueConfig.NameSuffix);

                    if (queueConfig.Arguments == null || queueConfig.Arguments.Count <= 0) return;
                    foreach (var argument in queueConfig.Arguments)
                    {
                        queueBuilder.WithArgument(argument.Key, argument.Value);
                    }
                };
            }

            if (queueNameConfig.IsNotEmpty() &&
                queueNameBindExchange.IsNotEmpty() &&
                !queueNameConfig.Equals(queueNameBindExchange))
            {
                throw DomainException.For(
                    $"Invalid Queue AggregateType: {nameof(queueName)}. Queue name is not assailable to the exchange [{exchangeName}]");
            }

            _busClient.SubscribeAsync(subscribeMethod, ctx =>
                ctx.UseSubscribeConfiguration(c => c
                    .Consume(consumeConfigBuilder)
                    .FromDeclaredQueue(queue)
                    .OnDeclaredExchange(exchange)), cancellationToken);
        }

        private async Task<Acknowledgement> TryHandleAsync<TMessage>(
            TMessage message,
            DomainContext domainContext,
            Func<Task> handle,
            Func<TMessage, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default)
        {
            if (message == null)
            {
                try
                {
                    message = Activator.CreateInstance<TMessage>();
                }
                catch
                {
                    throw DomainException.For("Unable to handle message, message is empty");
                }
            }

            if (handle == null)
                throw DomainException.For("Unable to handle message");

            if (domainContext == null)
                domainContext = DomainContext.Empty;

            if (_options.WithRequeuing)
            {
                return await TryHandleWithRequeuingAsync(message, domainContext, handle, onError, cancellationToken).ConfigureAwait(false);
            }

            var currentRetry = 0;
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(_retries, i => TimeSpan.FromSeconds(_retryInterval));

            var attribute = message.GetType().GetCustomAttribute<EventAttribute>();
            var messageName = attribute != null ? attribute.Name : message.GetType().Name;

            return await retryPolicy.ExecuteAsync<Acknowledgement>(async () =>
            {
                try
                {
                    var retryMessage = currentRetry == 0
                        ? string.Empty
                        : $"Retry: {currentRetry}'.";

                    var preLogMessage = $"Handling a message: '{messageName}' " +
                                        $"with domain id: '{domainContext.Id}'. {retryMessage}";

                    _logger.LogInformation(preLogMessage);

                    cancellationToken.ThrowIfCancellationRequested();

                    await handle.Invoke().ConfigureAwait(false);

                    var postLogMessage = $"Handled a message: '{messageName}' " +
                                         $"with domain id: '{domainContext.Id}'. {retryMessage}";
                    _logger.LogInformation(postLogMessage);

                    return new Ack();
                }
                catch (Exception exception)
                {
                    currentRetry++;
                    _logger.LogError(exception, exception.Message);

                    switch (exception)
                    {
                        case MessageBusException messageBusException when onError != null:
                        {
                            var rejectedEvent = onError(message, messageBusException);
                            await _busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(domainContext),
                                token: cancellationToken).ConfigureAwait(false);
                            _logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}' " +
                                                   $"for the message: '{messageName}' with domain id: '{domainContext.Id}'.");

                            return new Ack();
                        }
                        case DomainException _:
                            _logger.LogError($"Unable to handle a message: '{messageName}' " +
                                             $"with domain id: '{domainContext.Id}', " +
                                             $"retry {domainContext.Retries}...");

                            return new Ack();
                        default:
                            throw DomainException.For($"Unable to handle a message: '{messageName}' " +
                                                      $"with domain id: '{domainContext.Id}', " +
                                                      $"retry {currentRetry - 1}/{_retries}...");
                    }
                }
            }).ConfigureAwait(false);
        }

        private async Task<Acknowledgement> TryHandleWithRequeuingAsync<TMessage>(
            TMessage message,
            DomainContext domainContext,
            Func<Task> handle,
            Func<TMessage, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default)
        {
            var messageName = message.GetType().Name;
            var retryMessage = domainContext.Retries == 0
                ? string.Empty
                : $"Retry: {domainContext.Retries}'.";
            _logger.LogInformation($"Handling a message: '{messageName}' " +
                                   $"with domain id: '{domainContext.Id}'. {retryMessage}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await handle.Invoke().ConfigureAwait(false);

                _logger.LogInformation($"Handled a message: '{messageName}' " +
                                       $"with domain id: '{domainContext.Id}'. {retryMessage}");

                return new Ack();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                switch (exception)
                {
                    case MessageBusException messageBusException when onError != null:
                    {
                        var rejectedEvent = onError(message, messageBusException);
                        await _busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(domainContext),
                            token: cancellationToken).ConfigureAwait(false);
                        _logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}' " +
                                               $"for the message: '{messageName}' with domain id: '{domainContext.Id}'.");

                        return new Ack();
                    }
                    case DomainException _:
                        _logger.LogError($"Unable to handle a message: '{messageName}' " +
                                         $"with domain id: '{domainContext.Id}', " +
                                         $"retry {domainContext.Retries}...");
                        return new Ack();
                }

                if (domainContext.Retries >= _retries)
                {
                    var rejectedEvent = onError(message, exception.ToDomainException());

                    await _busClient.PublishAsync(rejectedEvent, ctx =>
                        ctx.UseMessageContext(domainContext), token: cancellationToken).ConfigureAwait(false);

                    throw DomainException.For($"Unable to handle a message: '{messageName}' " +
                                                 $"with domain id: '{domainContext.Id}' " +
                                                 $"after {domainContext.Retries} retries.", exception);
                }

                _logger.LogInformation($"Unable to handle a message: '{messageName}' " +
                                       $"with domain id: '{domainContext.Id}', " +
                                       $"retry {domainContext.Retries}/{_retries}...");

                return Retry.In(TimeSpan.FromSeconds(_retryInterval));
            }
        }

        private static void UseThrottledConsume(ISubscribeContext context)
        {
            context.UseThrottledConsume(async (func, token) =>
            {
                var semaphore = new SemaphoreSlim(1, 1);
                await semaphore.WaitAsync(token).ConfigureAwait(false);
                try
                {
                    func?.Invoke();
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
    }
}