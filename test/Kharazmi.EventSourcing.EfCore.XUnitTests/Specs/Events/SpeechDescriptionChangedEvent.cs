using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Events
{
    public class SpeechDescriptionChangedEvent : DomainEvent
    {
        public string AggregateId { get; }
        public string Description { get; }

        public SpeechDescriptionChangedEvent(string aggregateId, string description)
            : base(DomainEventType.From<SpeechDescriptionChangedEvent>())
        {
            AggregateId = aggregateId;
            Description = description;
        }
    }
}