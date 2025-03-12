using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.AspNetCore.Core.XUnitTest.EventStream
{
    internal class TestAggregate : AggregateRoot<TestAggregate, string>
    {
        public TestAggregate(string id) : base(id)
        {
            Validation = false;
            StateIsChanged = false;
            RegisterApplier<TestAggregateStateUpdated>(Apply);
        }


        public bool StateIsChanged { get; internal set; }
        public bool Validation { get; internal set; }


        public void ChangeState()
        {
            Emit(new TestAggregateStateUpdated(true));
        }    
     

        private void Apply(TestAggregateStateUpdated evt)
        {
            StateIsChanged = evt.IsChanged;
        }
    }
}