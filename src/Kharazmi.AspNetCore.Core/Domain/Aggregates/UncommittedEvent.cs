namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public interface IUncommittedEvent
    {
        IAggregateEvent AggregateEvent { get; }
        EventMetadata EventMetadata { get; }
    }

    public class UncommittedEvent : IUncommittedEvent
    {
        public IAggregateEvent AggregateEvent { get; }
        public EventMetadata EventMetadata { get; }

        public UncommittedEvent(IAggregateEvent aggregateEvent, EventMetadata eventMetadata)
        {
            AggregateEvent = aggregateEvent;
            EventMetadata = eventMetadata;
        }
    }
}