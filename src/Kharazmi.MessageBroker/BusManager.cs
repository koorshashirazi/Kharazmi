using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Functional;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Kharazmi.MessageBroker
{
    /// <summary> </summary>
    public class BusManager : IBusManager
    {
        private readonly IBusPublisher _busPublisher;
        private readonly ILogger<BusManager> _logger;

        /// <summary> </summary>
        public BusManager(
            IBusPublisher busPublisher,
            ILogger<BusManager> logger)
        {
            _busPublisher = busPublisher;
            _logger = logger;
        }

        /// <summary> </summary>
        public Task PublishCommandAsync<TCommand>(TCommand command, DomainContext domainContext = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            return TryHandleAsync(command, domainContext, () =>
                _busPublisher.SendAsync(command, domainContext, cancellationToken), cancellationToken);
        }

        /// <summary> </summary>
        public Task PublishCommandForAsync<TCommand>(
            TCommand command,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            return TryHandleAsync(command, domainContext, () =>
                    _busPublisher.SendToAsync(command, domainContext, exchangeConfiguration, properties,
                        cancellationToken),
                cancellationToken);
        }

        /// <summary> </summary>
        public Task PublishEventAsync<TEvent>(TEvent @event, DomainContext domainContext = null,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent
        {
            if (typeof(TEvent).IsAbstract)
                throw new RabbitMqException(typeof(TEvent).FullName + " must be non abstract class");
            
            return TryHandleAsync(@event, domainContext, () =>
                _busPublisher.PublishAsync(@event, domainContext, cancellationToken), cancellationToken);
        }

        /// <summary> </summary>
        public Task PublishEventForAsync<TEvent>(TEvent @event, DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent
        {
            if (typeof(TEvent).IsAbstract)
                throw new RabbitMqException(typeof(TEvent).FullName + " must be non abstract class");
            
            return TryHandleAsync(@event, domainContext, () =>
                _busPublisher.PublishToAsync(@event, domainContext, exchangeConfiguration, properties,
                    cancellationToken), cancellationToken);
        }

        private async Task<Result> TryHandleAsync<TMessage>(
            TMessage message,
            DomainContext domainContext,
            Func<Task> handle,
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


            var attribute = message.GetType().GetCustomAttribute<EventAttribute>();
            var messageName = attribute != null ? attribute.Name : message.GetType().Name;

            try
            {
                var preLogMessage = $"Handling a message: '{messageName}' " +
                                    $"with domain id: '{domainContext.Id}'";

                _logger.LogInformation(preLogMessage);

                cancellationToken.ThrowIfCancellationRequested();

                await handle.Invoke().ConfigureAwait(false);

                var postLogMessage = $"Handled a message: '{messageName}' " +
                                     $"with domain id: '{domainContext.Id}'";
                _logger.LogInformation(postLogMessage);

                return Result.Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                switch (exception)
                {
                    case DomainException _:
                        _logger.LogError($"Unable to handle a message: '{messageName}' " +
                                         $"with domain id: '{domainContext.Id}', " +
                                         $"retry {domainContext.Retries}...");

                        return Result.Fail("Unable to handle message");
                    default:
                        throw DomainException.For(
                            $"Unable to handle a message: '{messageName}' with domain id: '{domainContext.Id}'");
                }
            }
        }
    }
}