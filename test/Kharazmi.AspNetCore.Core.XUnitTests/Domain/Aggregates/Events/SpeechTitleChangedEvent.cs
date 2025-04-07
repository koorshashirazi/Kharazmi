using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.Events
{
    public class SpeechTitleChangedEvent : DomainEvent
    {
        public string Title { get; }

        public SpeechTitleChangedEvent(string title) : base(DomainEventType.From<SpeechTitleChangedEvent>())
        {
            Title = title;
        }
    }
}