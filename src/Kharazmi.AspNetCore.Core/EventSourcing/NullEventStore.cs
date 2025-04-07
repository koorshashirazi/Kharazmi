using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    internal sealed class NullEventStore : IEventStore
    {
        public Task<IReadOnlyCollection<IDomainEvent>> GetEventsAsync<TAggregate, TKey>(TKey aggregateId,
            CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            return Task.FromResult<IReadOnlyCollection<IDomainEvent>>([]);
        }

        public Task<IReadOnlyCollection<IDomainEvent>>
            GetEventsFromVersion<TAggregate, TKey>(TKey aggregateId, ulong fromVersion)
            where TAggregate : class, IAggregateRoot<TKey> where TKey : IEquatable<TKey>
        {
            return Task.FromResult<IReadOnlyCollection<IDomainEvent>>([]);
        }

        public Task SaveAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : class, IEventSourcing
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}