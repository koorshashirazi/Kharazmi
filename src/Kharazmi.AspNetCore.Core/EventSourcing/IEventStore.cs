using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    public interface IEventStore : IDisposable
    {
        Task<IReadOnlyCollection<IDomainEvent>> GetEventsAsync<TAggregate, TKey>(TKey aggregateId,
            CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>;

        Task<IReadOnlyCollection<IDomainEvent>> GetEventsFromVersion<TAggregate, TKey>(TKey aggregateId, ulong fromVersion)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>;
        Task SaveAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : class, IEventSourcing;
    }
}