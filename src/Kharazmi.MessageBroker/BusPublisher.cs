using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Extensions;
using RabbitMQ.Client;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace Kharazmi.MessageBroker
{
    /// <summary> </summary>
    public class BusPublisher : IBusPublisher
    {
        private readonly IBusClient _busClient;
        private readonly RabbitMqOptions _options;


        /// <summary> </summary>
        public BusPublisher(IBusClient busClient, RabbitMqOptions options)
        {
            _busClient = busClient;
            _options = options;
        }

        /// <summary>
        /// Publish a command to rabbit message broken
        /// </summary>
        /// <param name="command"></param>
        /// <param name="eventBusContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        public Task SendAsync<TCommand>(TCommand command, DomainContext eventBusContext,
            CancellationToken cancellationToken = default)
            where TCommand : IDomainCommand
            => _busClient.PublishAsync(command, context => context.UseMessageContext(eventBusContext),
                token: cancellationToken);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="domainContext"></param>
        /// <param name="exchangeConfiguration"></param>
        /// <param name="properties"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        public Task SendToAsync<TCommand>(TCommand command,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TCommand : IDomainCommand
        {
            return PublishMessageAsync(command, domainContext, exchangeConfiguration, properties, cancellationToken);
        }

        /// <summary>
        /// Publish a event to rabbit message broken
        /// </summary>
        /// <param name="event"></param>
        /// <param name="eventBusContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public Task PublishAsync<TEvent>(TEvent @event, DomainContext eventBusContext,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent
        {
            if (typeof(TEvent).IsAbstract)
                throw new RabbitMqException(typeof(TEvent).FullName + " must be non abstract class");
            
            return _busClient.PublishAsync(@event, context => context.UseMessageContext(eventBusContext),
                token: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="domainContext"></param>
        /// <param name="exchangeConfiguration"></param>
        /// <param name="properties"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public Task PublishToAsync<TEvent>(TEvent @event,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent
        {
            if (typeof(TEvent).IsAbstract)
                throw new RabbitMqException(typeof(TEvent).FullName + " must be non abstract class");
            
            return PublishMessageAsync(@event, domainContext, exchangeConfiguration, properties, cancellationToken);
        }


        private Task PublishMessageAsync<TMessage>(
            TMessage message,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default)
        {
            return _busClient.PublishAsync(message, context =>
            {
                var exchangeName = typeof(TMessage).GetOrCreateExchangeName(_options);
                var routingKey = typeof(TMessage).GetCreateRoutingKey(_options);
                var exchangeType = typeof(TMessage).GetExchangeType();
                var isDurability = typeof(TMessage).IsDurability();
                var isAutoDelete = typeof(TMessage).IsAutoDelete();
                Dictionary<string, string> arguments = null;

                if (exchangeConfiguration != null)
                {
                    if (exchangeConfiguration.ExchangeName.IsNotEmpty())
                        exchangeName = exchangeConfiguration.ExchangeName;

                    if (Enum.GetName(typeof(ExchangeType), exchangeConfiguration.ExchangeType).IsNotEmpty())
                        exchangeType = exchangeConfiguration.ExchangeType;

                    isDurability = exchangeConfiguration.Durability;
                    isAutoDelete = exchangeConfiguration.AutoDelete;

                    if (exchangeConfiguration.Arguments != null && exchangeConfiguration.Arguments.Count >= 0)
                        arguments = exchangeConfiguration.Arguments;
                }

                context.UseMessageContext(domainContext).UsePublishConfiguration(publisherBuilder =>
                {
                    publisherBuilder.OnDeclaredExchange(exchangeBuilder =>
                    {
                        exchangeBuilder.WithName(exchangeName).WithType(exchangeType).WithDurability(isDurability)
                            .WithAutoDelete(isAutoDelete);

                        if (arguments == null || arguments.Count <= 0) return;

                        foreach (var argument in arguments)
                            exchangeBuilder.WithArgument(argument.Key, argument.Value);
                    }).OnExchange(exchangeName).WithRoutingKey(routingKey).WithProperties(properties);
                });
            }, token: cancellationToken);
        }
    }
}