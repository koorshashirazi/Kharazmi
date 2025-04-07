using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Events
{
    public class EventStub : DomainEvent
    {
        private int Id { get; }

        public EventStub(int id)
            : base(DomainEventType.From<EventStub>())
        {
            Id = id;
        }
    }
}