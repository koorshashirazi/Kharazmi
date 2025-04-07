using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates;

public class TestAggregate : AggregateRoot<TestAggregate, Guid>
{
    public string Data { get; private set; }

    public TestAggregate() : this(ValueObjects.Id.New<Guid>())
    {
    }

    public TestAggregate(Guid id, ulong version = 0) : base(id, version)
    {
        RegisterApplier<TestEvent>(Apply);
    }

    private void Apply(TestEvent evt)
    {
        Data = evt.Data;
    }

    public void EmitTestEvent(string data)
    {
        Emit(new TestEvent(data, new EventMetadata()));
    }

    public void EmitNullEvent()
    {
        Emit<TestEvent>(null);
    }

    public void ValidateStatePublic()
    {
        ValidateState();
    }

    public void ApplyChangesPublic(params IDomainEvent[] events)
    {
        ApplyChanges(events);
    }
}