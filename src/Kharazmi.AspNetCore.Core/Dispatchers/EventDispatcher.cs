using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task<Result> RaiseAsync<TEvent>(TEvent domainEvent, CancellationToken token = default)
            where TEvent : class, IDomainEvent;
    }

    internal class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceFactory;
        private static readonly ConcurrentDictionary<Type, IEnumerable<IDomainEventHandler>> EventHandlersTypes = new();

        public DomainEventDispatcher(IServiceProvider serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }


        // TODO: like CommandDispatcher
        public Task<Result> RaiseAsync<TEvent>(TEvent domainEvent, CancellationToken token = default)
            where TEvent : class, IDomainEvent
        {
            return ExceptionHandler.ExecuteResultAsync(async @event =>
                {
                    var eventHandlers = _serviceFactory.GetServices<IDomainEventHandler<TEvent>>();

                    foreach (var eventHandler in eventHandlers)
                    {
                        var result = await eventHandler.HandleAsync(@event, token).ConfigureAwait(false);
                        if (result.Failed)
                        {
                            return result;
                        }
                    }

                    return Result.Ok();
                }, state: domainEvent, nameof(domainEvent),
                onError: e =>
                    Task.FromResult(Result.Fail($"Unable to raise {DomainEventType.From<TEvent>()}").WithException(e)));
        }

        private static IEnumerable<IDomainEventHandler> GetEventHandlers(IServiceProvider sp, Type eventType)
        {
            return EventHandlersTypes.GetOrAdd(eventType, ValueFactory);

            IEnumerable<IDomainEventHandler> ValueFactory(Type arg)
            {
                var serviceType = typeof(IDomainEventHandler<>).MakeGenericType(arg);
                var handlers = sp.GetServices(serviceType);
                return handlers.OfType<IDomainEventHandler>();
            }
        }
    }
}