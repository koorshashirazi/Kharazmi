using Kharazmi.AspNetCore.Core.Domain.Events;
using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.XUnitTests.EventSourcingTest
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