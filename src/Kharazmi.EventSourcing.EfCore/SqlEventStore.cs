using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Linq;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.EventSourcing.EfCore
{
    /// <summary> </summary>
    internal sealed class SqlEventStore : IEventStore
    {
        private bool _isDisposed;

        private readonly IDomainEventFactory _eventFactory;
        private readonly IEventSerializer _eventSerializer;
        private readonly IEventStoreUnitOfWork _eventStoreUnitOfWork;
        private readonly EventSourcingOptions _eventSourcingOptions;
        private readonly DbSet<EventStoreEntity> _storedEvents;

        public SqlEventStore(
            IEventStoreDbContext context,
            IDomainEventFactory eventFactory,
            IEventSerializer eventSerializer,
            IEventStoreUnitOfWork eventStoreUnitOfWork,
            EventSourcingOptions eventSourcingOptions)
        {
            Ensure.ArgumentIsNotNull(context, nameof(context));
            _eventFactory = Ensure.ArgumentIsNotNull(eventFactory, nameof(eventFactory));
            _eventSerializer = Ensure.ArgumentIsNotNull(eventSerializer, nameof(eventSerializer));
            _eventStoreUnitOfWork = Ensure.ArgumentIsNotNull(eventStoreUnitOfWork, nameof(eventStoreUnitOfWork));
            _eventSourcingOptions = Ensure.ArgumentIsNotNull(eventSourcingOptions, nameof(eventSourcingOptions));
            _storedEvents = context.EventStores;
        }


        public async Task<IReadOnlyCollection<IDomainEvent>> GetEventsAsync<TAggregate, TKey>(TKey aggregateId,
            CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            if (Equals(aggregateId, default(TKey)))
            {
                throw new BadAggregateIdException(nameof(aggregateId));
            }

            var stringAggregateId = aggregateId.ConvertToString(formatProvider: CultureInfo.InvariantCulture);
            var aggregateType = AggregateType.From<TAggregate>().ToString();
            var eventEntities = await _storedEvents
                .AsNoTracking()
                .Where(e => e.AggregateId == stringAggregateId && e.AggregateType == aggregateType)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (eventEntities.Length == 0) return [];

            return _eventFactory.CreateFrom(eventEntities).AsReadOnly();
        }

        public async Task<IReadOnlyCollection<IDomainEvent>> GetEventsFromVersion<TAggregate, TKey>
            (TKey aggregateId, ulong fromVersion)
            where TAggregate : class, IAggregateRoot<TKey> where TKey : IEquatable<TKey>
        {
            var stringAggregateId = aggregateId.ConvertToString(formatProvider: CultureInfo.InvariantCulture);
            
            Expression<Func<EventStoreEntity, bool>> expression = e =>
                e.AggregateId == stringAggregateId && (long)e.AggregateVersion > (long)fromVersion;
            var evaluateExpr = expression.PartiallyEvaluate();
            
            var eventEntities = await _storedEvents
                .AsNoTracking()
                .Where(evaluateExpr)
                .OrderBy(e => (long)e.AggregateVersion)
                .ToArrayAsync()
                .ConfigureAwait(false);

            if (eventEntities.Length == 0) return [];

            return _eventFactory.CreateFrom(eventEntities).AsReadOnly();
        }

        public async Task SaveAsync<TAggregate>(TAggregate aggregate,
            CancellationToken cancellationToken = default)
            where TAggregate : class, IEventSourcing

        {
            if (!_eventSourcingOptions.EnableStoreEvent)
                return;

            Ensure.ArgumentIsNotNull(aggregate, nameof(aggregate));
            aggregate.ValidateVersion();

            await _eventStoreUnitOfWork.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            var events = aggregate.GetUncommittedEvents();
            foreach (var domainEvent in events)
            {
                EventStoreNullException.ThrowIfIsNull(domainEvent, nameof(domainEvent));
                MetaDataNullException.ThrowIfIsNull(domainEvent.EventMetadata, nameof(domainEvent.EventMetadata));

                if (_eventSourcingOptions.NotAllowedEvents
                    .Any(x => x.Equals(domainEvent.EventType.ToString(), StringComparison.Ordinal)))
                {
                    continue;
                }

                var payload = _eventSerializer.Serialize(domainEvent);
                if (string.IsNullOrEmpty(payload) || string.IsNullOrWhiteSpace(payload))
                {
                    throw new SerializationException($"Unable to serialize the domain event: {domainEvent.EventType}");
                }
                
                var metaData = domainEvent.EventMetadata;

                var eventEntities = new EventStoreEntity(
                    metaData.AggregateId,
                    metaData.AggregateVersion,
                    metaData.AggregateType.ToString(),
                    metaData.EventType.ToString(),
                    metaData.Timestamp,
                    payload);

                await _storedEvents.AddAsync(eventEntities, cancellationToken).ConfigureAwait(false);
            }

            await _eventStoreUnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
            aggregate.MarkChangesAsCommitted();
        }


        public void Dispose()
        {
            if (_isDisposed) return;
            _eventStoreUnitOfWork.Dispose();
            _isDisposed = true;
        }
    }
}