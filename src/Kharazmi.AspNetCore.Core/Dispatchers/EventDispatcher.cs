using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task<Result> RaiseAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class, IDomainEvent;

        /// <summary></summary>
        /// <param name="event"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task<Result> RaiseAsync<TEvent>(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent;
    }

    internal class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceFactory;

        public EventDispatcher(IServiceProvider serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }


        public async Task<Result> RaiseAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class, IDomainEvent
        {
            foreach (var eventHandler in _serviceFactory.GetServices<IEventHandler<TEvent>>())
            {
                var result = await eventHandler
                    .HandleAsync(@event, DomainContext.Empty, cancellationToken).ConfigureAwait(false);

                if (result.Failed )
                    return result;
            }

            return Result.Ok();
        }

        public async Task<Result> RaiseAsync<TEvent>(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent
        {
            if (@event is DomainNotificationDomainEvent notification)
            {
                var domainNotificationHandler = _serviceFactory.GetRequiredService<IDomainNotificationHandler>();
                return await domainNotificationHandler.HandleAsync(notification, domainContext, cancellationToken).ConfigureAwait(false);
            }

            foreach (var eventHandler in _serviceFactory.GetServices<IEventHandler<TEvent>>())
            {
                var result = await eventHandler
                    .HandleAsync(@event, domainContext ?? DomainContext.Empty, cancellationToken).ConfigureAwait(false);

                if (result.Failed)
                {
                    return result;
                }
            }

            return Result.Ok();
        }
    }
}