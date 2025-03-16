using Kharazmi.AspNetCore.Core.Domain.Events;
using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.XUnitTests.EventStream
{
    internal class TestAggregateStateUpdated : DomainEvent
    {
        public bool IsChanged { get; }

        public TestAggregateStateUpdated(bool isChanged) : base(DomainEventType.From<TestAggregateStateUpdated>())
        {
            IsChanged = isChanged;
        }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return base.EqualityValues;
                yield return IsChanged;
            }
        }
    }
}