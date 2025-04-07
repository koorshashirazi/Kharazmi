using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.XUnitTests.EventSourcing
{
    public class SubEventIncremented : DomainEvent
    {
        public int Value { get; }

        public SubEventIncremented(int value) : base(DomainEventType.From<SubEventIncremented>())
        {
            Value = value;
        }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return base.EqualityValues;
                yield return Value;
            }
        }
    }
}