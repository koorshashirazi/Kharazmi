using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain
{
    public interface IDomainEvent
    {
        DomainEventType EventType { get; }
        DomainEventId EventId { get; }
        EventMetadata EventMetadata { get; }
    }

    /// <summary></summary>
    public abstract class DomainEvent : ValueObject, IDomainEvent
    {
        /// <summary></summary>
        protected DomainEvent(DomainEventType eventType)
        {
            EventType = eventType;
            var eventId = DomainEventId.New();
            EventId = eventId;
            EventMetadata = EventMetadata.Empty
                .SetEventId(eventId)
                .SetEventType(eventType)
                .SetTimestamp(DateTimeOffset.UtcNow)
                .SetEventVersion(1);
        }

        [Newtonsoft.Json.JsonConstructor, System.Text.Json.Serialization.JsonConstructor]
        protected DomainEvent(DomainEventId eventId, DomainEventType eventType, EventMetadata eventMetadata)
        {
            EventMetadata = eventMetadata ?? throw new ArgumentNullException(nameof(eventMetadata));
            EventType = eventType;
            EventId = eventId;
        }

        [JsonInclude, JsonProperty] public DomainEventType EventType { get; }
        [JsonInclude, JsonProperty] public DomainEventId EventId { get; }
        [JsonInclude, JsonProperty] public EventMetadata EventMetadata { get; set; }

        [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return EventType;
                yield return EventId;
            }
        }
    }
}