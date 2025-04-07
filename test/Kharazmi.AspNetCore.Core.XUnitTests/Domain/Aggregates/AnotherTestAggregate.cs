using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates;

public class AnotherTestAggregate : AggregateRoot<AnotherTestAggregate, int>
{
    public AnotherTestAggregate() : base(ValueObjects.Id.New<int>())
    {
    }

    public AnotherTestAggregate(int id) : base(id)
    {
    }
}