using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest.Events
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