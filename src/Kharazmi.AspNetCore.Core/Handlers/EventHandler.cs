using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    public interface IDomainEventHandler
    {
        Task<Result> HandleAsync(IDomainEvent domainEventEvent, CancellationToken token = default);
    }

    public interface IDomainEventHandler<in TEvent> : IDomainEventHandler
        where TEvent : IDomainEvent
    {
        Task<Result> HandleAsync(TEvent domainEvent, CancellationToken token = default);
    }

    /// <summary>_</summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler<TEvent>
        where TEvent : class, IDomainEvent
    {
        public abstract Task<Result> HandleAsync(TEvent domainEvent,
            CancellationToken token = default);

        public Task<Result> HandleAsync(IDomainEvent domainEventEvent,
            CancellationToken token = default) => HandleAsync((TEvent)domainEventEvent, token);

        protected IEnumerable<IDomainEvent> UseUncommittedEventMapper(
            (IEventSourcing? aggregate, object? state) arg)
        {
            if (arg.aggregate is null)
            {
                yield break;
            }

            foreach (var uncommittedEvent in arg.aggregate.GetUncommittedEvents())
            {
                yield return uncommittedEvent;
            }
        }
    }
}