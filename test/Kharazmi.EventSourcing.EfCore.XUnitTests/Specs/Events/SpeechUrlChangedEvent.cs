using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Events
{
    public class SpeechUrlChangedEvent : DomainEvent
    {
        public string AggregateId { get; }
        public string Url { get; }

        public SpeechUrlChangedEvent(string aggregateId, string url)
            : base(DomainEventType.From<SpeechUrlChangedEvent>())
        {
            AggregateId = aggregateId;
            Url = url;
        }
    }
}