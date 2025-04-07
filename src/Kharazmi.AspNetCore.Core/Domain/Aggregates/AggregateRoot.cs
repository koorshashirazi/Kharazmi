using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    /// <summary> </summary>
    public interface IAggregateRoot : IEntity
    {
        /// <summary> </summary>
        ulong Version { get; }

        AggregateType GetAggregateType();
    }

    public interface IAggregateRoot<TKey> : IEntity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        IId<TKey> GetAggregateId();
    }


    /// <summary> </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TAggregate"></typeparam>
    public abstract class AggregateRoot<TAggregate, TKey> : Entity<TKey>, IAggregateRoot<TKey>, IEventSourcing
        where TAggregate : AggregateRoot<TAggregate, TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        internal readonly HashSet<IDomainEvent> UncommittedEvents = [];
        private readonly ConcurrentDictionary<Type, Action<IDomainEvent>> _appliers = [];
        private readonly ulong _currentVersion;

        protected AggregateRoot() : this(Core.ValueObjects.Id.Default<TKey>())
        {
        }

        /// <summary> </summary>
        protected AggregateRoot(TKey id, ulong version = 0) : base(id)
        {
            Version = version;
            _currentVersion = version;
        }

        public AggregateType GetAggregateType() => AggregateType.From<TAggregate>();


        [JsonInclude, JsonProperty] public ulong Version { get; protected set; }

        public IId<TKey> GetAggregateId() => new Id<TKey>(Id);

        public virtual bool IsCommitted() => UncommittedEvents.Count == 0;


        public virtual IReadOnlyCollection<IDomainEvent> GetUncommittedEvents()
        {
            return UncommittedEvents.AsEnumerable().AsReadOnly();
        }

        public virtual void MarkChangesAsCommitted()
        {
            UncommittedEvents.Clear();
        }

        /// <summary></summary>
        /// <exception cref="ConcurrencyException"></exception>
        public virtual void ValidateVersion()
        {
            var expectedVersion = _currentVersion + UnCommitedEventsCount();
            if (Version != expectedVersion)
            {
                throw new ConcurrencyException(
                    $"Invalid version specified : expectedVersion = {expectedVersion} but  originalVersion = {Version}.");
            }
        }

        private ulong UnCommitedEventsCount() => (ulong)GetUncommittedEvents().Count;

        /// <summary></summary>
        protected void RegisterApplier<TDomainEvent>(Action<TDomainEvent> handler)
            where TDomainEvent : class, IDomainEvent
        {
            _appliers.TryAdd(typeof(TDomainEvent), x => handler((TDomainEvent)x));
        }


        /// <summary> </summary>
        protected void Emit<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : class, IDomainEvent
        {
            if (domainEvent is null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            ApplyChange(domainEvent, true);
        }

        private void ApplyChange(IDomainEvent domainEvent, bool isNew)
        {
            if (domainEvent is null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            SetEventMetadata(domainEvent);

            if (isNew)
            {
                domainEvent.EventMetadata.SetAggregateVersion(Version);
            }

            var evtType = domainEvent.EventType.ToType();
            if (!_appliers.TryGetValue(evtType, out var handler))
            {
                throw new InvalidOperationException($"Apply method not found for event type {domainEvent.EventType}");
            }

            handler(domainEvent);

            if (isNew)
            {
                Version += 1;
                domainEvent.EventMetadata.SetAggregateVersion(Version);
                UncommittedEvents.Add(domainEvent);
            }
            else
            {
                Version = domainEvent.EventMetadata.AggregateVersion + 1;
            }
        }

        /// <summary> </summary>
        /// <param name="domainEvents"></param>
        /// <exception cref="EventStoreNullException"></exception>
        public void ApplyChanges(params IDomainEvent[] domainEvents)
        {
            if (domainEvents is null)
            {
                throw new ArgumentNullException(nameof(domainEvents));
            }

            foreach (var domainEvent in domainEvents)
            {
                ApplyChange(domainEvent, false);
            }
        }

        protected internal virtual void ValidateState()
        {
            if (Equals(Id, default(TKey)))
                throw new BadAggregateIdException($"Invalid id. {Id}");
        }


        /// <summary></summary>
        /// <param name="eventStore"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="EventStoreNullException"></exception>
        public virtual async Task ApplyFromHistoryAsync(IEventStore eventStore,
            CancellationToken cancellationToken = default)
        {
            if (eventStore is null)
            {
                throw new ArgumentNullException(nameof(eventStore));
            }

            var domainEvents = await eventStore.GetEventsAsync<TAggregate, TKey>(Id, cancellationToken)
                .ConfigureAwait(false);

            ApplyChanges([.. domainEvents]);
        }

        /// <summary> </summary>
        /// <param name="eventStore"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="EventStoreNullException"></exception>
        public virtual async Task CommitAsync(IEventStore eventStore, CancellationToken cancellationToken = default)
        {
            if (eventStore is null)
            {
                throw new ArgumentNullException(nameof(eventStore));
            }

            ValidateState();

            await eventStore.SaveAsync(this, cancellationToken).ConfigureAwait(false);
        }

        private void SetEventMetadata<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : class, IDomainEvent
        {
            domainEvent.EventMetadata
                .SetAggregateId($"{Id}")
                .SetAggregateType(GetAggregateType())
                .SetTimestamp(DateTimeOffset.UtcNow);
        }
    }
}