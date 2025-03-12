using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Events
{
    public class SpeechDescriptionChangedEvent : DomainEvent
    {
        public string Description { get; }

        public SpeechDescriptionChangedEvent( string description) 
            : base(DomainEventType.From<SpeechDescriptionChangedEvent>())
        {
            Description = description;
        }
    }
}