using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates;

public class TestEvent : DomainEvent
{
    public string Data { get; }

    public TestEvent(string data, EventMetadata metadata) : base(DomainEventType.From<TestEvent>())
    {
        Data = data;
        EventMetadata = metadata;
    }
}