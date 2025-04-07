using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.Events
{
    public class SpeechUrlChangedEvent : DomainEvent
    {
        public string AggregateId { get; }
        public string Url { get; }

        public SpeechUrlChangedEvent(string aggregateId, string url): base(DomainEventType.From<SpeechUrlChangedEvent>())
        {
            AggregateId = aggregateId;
            Url = url;
        }
    }
}