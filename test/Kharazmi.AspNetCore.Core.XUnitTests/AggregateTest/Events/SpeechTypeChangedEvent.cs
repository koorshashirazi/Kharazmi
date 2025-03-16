using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest.Events
{
    public class SpeechTypeChangedEvent : DomainEvent
    {
        public string AggregateId { get; }
        public string Type { get; }

        public SpeechTypeChangedEvent(string aggregateId, string type): base(DomainEventType.From<SpeechTypeChangedEvent>())
        {
            AggregateId = aggregateId;
            Type = type;
        }
    }
}