using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.Events
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