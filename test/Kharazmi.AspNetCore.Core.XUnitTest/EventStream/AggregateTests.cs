using System.Linq;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTest.EventStream
{
    [CollectionDefinition(nameof(AggregateTests), DisableParallelization = true)]
    public class AggregateTestsCollection { }

    [Collection(nameof(AggregateTests))]
    public class AggregateTests
    {
        [Fact]
        public void AggregateNameShouldEqualTheConcreteImplementationClassName()
        {
            var aggregate = new TestAggregate(Id.New<string>());
            aggregate.GetAggregateType().Should().Be(AggregateType.From<TestAggregate>());
        }

        [Fact]
        public void ApplyingAnEventShouldAddItToUncommittedEvents()
        {
            var aggregate = new TestAggregate(Id.New<string>());
            aggregate.ChangeState();
            var evt = aggregate.GetUncommittedEvents().Single();
            evt.Should().BeAssignableTo<TestAggregateStateUpdated>();
        }

        [Fact]
        public void MarkingEventsAsCommittedShouldClearUncommittedEvents()
        {
            var aggregate = new TestAggregate(Id.New<string>());
            aggregate.ChangeState();

            aggregate.GetUncommittedEvents().Count.Should().Be(1);

            aggregate.MarkChangesAsCommitted();
            aggregate.GetUncommittedEvents().Should().BeEmpty();
        }

        [Fact]
        public void ApplyFromCommandShouldDoValidation()
        {
            var aggregate = new TestAggregate(Id.New<string>());
            aggregate.ChangeState();

            aggregate.GetUncommittedEvents().Count.Should().Be(1);

            aggregate.Validation.Should().BeFalse();
            aggregate.StateIsChanged.Should().BeTrue();
        }

    }
}