using System;

namespace Kharazmi.AspNetCore.Core.Domain.Events
{
    public sealed record EventStoreEntity
    {
        /// <summary>
        ///  EF Constructor
        /// </summary>
        private EventStoreEntity()
        {
        }

        public EventStoreEntity(
            string aggregateId,
            ulong aggregateVersion,
            string aggregateType,
            string eventType,
            DateTimeOffset createAt,
            string payLoad)
        {
            AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            AggregateVersion = aggregateVersion;
            AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            CreateAt = createAt;
            PayLoad = payLoad ?? throw new ArgumentNullException(nameof(payLoad));
        }

        public EventStoreEntity(Guid id, DateTimeOffset createAt, string aggregateId, ulong aggregateVersion,
            string aggregateType, string eventType, string payLoad)
        {
            Id = id;
            CreateAt = createAt;
            AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            AggregateVersion = aggregateVersion;
            AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            CreateAt = createAt;
            PayLoad = payLoad ?? throw new ArgumentNullException(nameof(payLoad));
        }

        public Guid Id { get; }
        public DateTimeOffset CreateAt { get; }
        public string AggregateId { get; }
        public ulong AggregateVersion { get; }
        public string AggregateType { get; }
        public string EventType { get; }
        public string PayLoad { get; }
    }
}