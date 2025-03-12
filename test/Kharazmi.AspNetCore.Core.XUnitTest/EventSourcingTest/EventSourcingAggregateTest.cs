using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.AspNetCore.Core.XUnitTest.EventSourcingTest
{
    public class EventSourcingAggregateTest : AggregateRoot<EventSourcingAggregateTest, string>
    {
        private EventSourcingAggregateTest()
        {
        }

        public EventSourcingAggregateTest(string id, ulong version = 0) : base(id, version)
        {
            RegisterApplier<SubEventIncremented>(Apply);
        }

        public int Value { get; private set; }

        public void IncrementValue(int value)
        {
            Emit(new SubEventIncremented(value));
        }

        public void Apply(SubEventIncremented subEvent)
        {
            Value = subEvent.Value;
        }
    }
}