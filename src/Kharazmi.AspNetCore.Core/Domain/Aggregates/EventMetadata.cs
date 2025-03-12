using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public sealed class MetadataKeys
    {
        private MetadataKeys(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static readonly MetadataKeys EventId = new MetadataKeys("event_id");
        public static readonly MetadataKeys BatchId = new MetadataKeys("batch_id");
        public static readonly MetadataKeys EventType = new MetadataKeys("event_Type");
        public static readonly MetadataKeys EventName = new MetadataKeys("event_name");
        public static readonly MetadataKeys EventTypeName = new MetadataKeys("event_type_name");
        public static readonly MetadataKeys EventVersion = new MetadataKeys("event_version");
        public static readonly MetadataKeys Timestamp = new MetadataKeys("timestamp");
        public static readonly MetadataKeys TimestampEpoch = new MetadataKeys("timestamp_epoch");
        public static readonly MetadataKeys AggregateVersion = new MetadataKeys("aggregate_version");
        public static readonly MetadataKeys AggregateType = new MetadataKeys("aggregate_type");
        public static readonly MetadataKeys AggregateId = new MetadataKeys("aggregate_id");
        public static readonly MetadataKeys SourceId = new MetadataKeys("source_id");
    }

    public class EventMetadata : MetadataContainer
    {
        [JsonIgnore]
        public SourceId SourceId
        {
            get => GetMetadataValue(MetadataKeys.SourceId.Value, v => new SourceId(v));
            set => TryAdd(MetadataKeys.SourceId.Value, value.Value);
        }

        [JsonIgnore]
        public DomainEventType EventType
        {
            get => GetMetadataValue(MetadataKeys.EventType.Value, value => new DomainEventType(value));
            set => TryAdd(MetadataKeys.EventType.Value, value.Value);
        }

        [JsonIgnore]
        public string EventName => TryGetValue(MetadataKeys.EventTypeName.Value, out var eventName)
            ? eventName
            : EventType.ToType().Name;

        [JsonIgnore]
        public string? EventTypeName => TryGetValue(MetadataKeys.EventTypeName.Value, out var eventTypeName)
            ? eventTypeName
            : EventType.ToType().AssemblyQualifiedName;


        [JsonIgnore]
        public int EventVersion
        {
            get => GetMetadataValue(MetadataKeys.EventVersion.Value, int.Parse);
            set => TryAdd(MetadataKeys.EventVersion.Value, value.ToString());
        }

        [JsonIgnore]
        public DateTimeOffset Timestamp
        {
            get => GetMetadataValue(MetadataKeys.Timestamp.Value, DateTimeOffset.Parse);
            set => TryAdd(MetadataKeys.Timestamp.Value, value.ToString("O"));
        }

        [JsonIgnore]
        public double TimestampEpoch => TryGetValue(MetadataKeys.TimestampEpoch.Value, out var timestampEpoch)
            ? double.Parse(timestampEpoch)
            : Timestamp.ToUnixTime();

        [JsonIgnore]
        public string BatchId
        {
            get => GetMetadataValue(MetadataKeys.BatchId.Value);
            set => TryAdd(MetadataKeys.BatchId.Value, value);
        }

        [JsonIgnore]
        public ulong AggregateVersion
        {
            get => GetMetadataValue(MetadataKeys.AggregateVersion.Value, ulong.Parse);
            set => TryAdd(MetadataKeys.AggregateVersion.Value, value.ToString(CultureInfo.InvariantCulture));
        }

        [JsonIgnore]
        public string AggregateId
        {
            get => GetMetadataValue(MetadataKeys.AggregateId.Value);
            set => TryAdd(MetadataKeys.AggregateId.Value, value);
        }

        [JsonIgnore]
        public DomainEventId DomainEventId
        {
            get => GetMetadataValue(MetadataKeys.EventId.Value, value => new DomainEventId(value));
            set => TryAdd(MetadataKeys.EventId.Value, value.Value);
        }

        [JsonIgnore]
        public AggregateType AggregateType
        {
            get => GetMetadataValue(MetadataKeys.AggregateType.Value, value=> new AggregateType(value));
            set => TryAdd(MetadataKeys.AggregateType.Value, value.Value);
        }
        
        public EventMetadata SetAggregateVersion(ulong value)
        {
            this[MetadataKeys.AggregateVersion.Value] = $"{value}";
            return this;
        }

        public EventMetadata SetTimestamp(DateTimeOffset value)
        {
            this[MetadataKeys.Timestamp.Value] = $"{value}";
            return this;
        }

        public EventMetadata SetAggregateId(string value)
        {
            this[MetadataKeys.AggregateId.Value] = value;
            return this;
        }

        public EventMetadata SetAggregateType(AggregateType value)
        {
            this[MetadataKeys.AggregateType.Value] = value;
            return this;
        }

        public EventMetadata SetBatchId(string value)
        {
            this[MetadataKeys.BatchId.Value] = value;
            return this;
        }

        public EventMetadata SetEventId(DomainEventId value)
        {
            this[MetadataKeys.EventId.Value] = value.Value;
            return this;
        }

        public EventMetadata SetEventType(DomainEventType eventType)
        {
            this[MetadataKeys.EventType.Value] = eventType.Value;
            return this;
        }

        public EventMetadata SetEventVersion(int value)
        {
            this[MetadataKeys.EventVersion.Value] = $"{value}";
            return this;
        }

        public EventMetadata SetSourceId(SourceId value)
        {
            this[MetadataKeys.SourceId.Value] = value.Value;
            return this;
        }

        public EventMetadata SetTimestampEpoch(double value)
        {
            this[MetadataKeys.TimestampEpoch.Value] = $"{value}";
            return this;
        }

        public EventMetadata() : this(new Dictionary<string, string>())
        {
        }

        public EventMetadata(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
               
        }

        public EventMetadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : this(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public EventMetadata(params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>) keyValuePairs)
        {
        }

        public static EventMetadata Empty { get; } = new EventMetadata();

        public static EventMetadata With(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public static EventMetadata With(params KeyValuePair<string, string>[] keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public static EventMetadata With(params KeyValuePair<MetadataKeys, string>[] keyValuePairs)
        {
            return new EventMetadata(keyValuePairs.Select(x => new KeyValuePair<string, string>(x.Key.Value, x.Value)));
        }

        public static EventMetadata With(MetadataKeys key, string value)
        {
            return With(new KeyValuePair<string, string>(key.Value, value));
        }

        public static EventMetadata With(IDictionary<string, string> keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }
        
        public EventMetadata CloneWith(MetadataKeys key, string value)
        {
            return CloneWith(new KeyValuePair<string, string>(key.Value, value));
        }
        
        public EventMetadata CloneWith(params KeyValuePair<string, string>[] keyValuePairs)
        {
            return CloneWith((IEnumerable<KeyValuePair<string, string>>) keyValuePairs);
        }

        public EventMetadata CloneWith(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var metadata = new EventMetadata(this);
            foreach (var kv in keyValuePairs)
            {
                if (metadata.ContainsKey(kv.Key))
                {
                    throw new ArgumentException($"Key '{kv.Key}' is already present!");
                }

                metadata[kv.Key] = kv.Value;
            }

            return metadata;
        }
    }
}