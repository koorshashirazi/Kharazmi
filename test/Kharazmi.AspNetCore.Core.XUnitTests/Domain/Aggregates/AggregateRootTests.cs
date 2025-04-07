using FluentAssertions;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Exceptions;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates;
[CollectionDefinition(nameof(AggregateRootTests), DisableParallelization = true)]
public class AggregateRootTestsCollection { }

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[Collection(nameof(AggregateRootTests))]
public class AggregateRootTests
{
    [Fact]
    public void Constructor_WithIdAndVersion_SetsProperties()
    {
        // Arrange & Act
        var aggregate = new TestAggregate(Guid.NewGuid(), 5);

        // Assert
        aggregate.Version.Should().Be(5);
    }

    [Fact]
    public void GetAggregateType_ReturnsCorrectType()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act
        var aggregateType = aggregate.GetAggregateType();

        // Assert
        aggregateType.ToString().Should().Be(AggregateType.From<TestAggregate>().ToString());
    }

    [Fact]
    public void GetAggregateId_ReturnsIdWrappedInValueObject()
    {
        // Arrange
        var id = Guid.NewGuid();
        var aggregate = new TestAggregate(id);

        // Act
        var aggregateId = aggregate.GetAggregateId();

        // Assert
        aggregateId.Value.Should().Be(id);
    }

    [Fact]
    public void IsCommitted_WhenNoEvents_ReturnsTrue()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act
        var isCommitted = aggregate.IsCommitted();

        // Assert
        isCommitted.Should().BeTrue();
    }

    [Fact]
    public void IsCommitted_AfterEmittingEvent_ReturnsFalse()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act
        aggregate.EmitTestEvent("Test");

        // Assert
        aggregate.IsCommitted().Should().BeFalse();
    }

    [Fact]
    public void Emit_WithValidEvent_IncreasesVersion()
    {
        // Arrange
        var aggregate = new TestAggregate();
        var initialVersion = aggregate.Version;

        // Act
        aggregate.EmitTestEvent("Test");

        // Assert
        aggregate.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void Emit_WithValidEvent_AddsToUncommittedEvents()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act
        aggregate.EmitTestEvent("Test");

        // Assert
        aggregate.GetUncommittedEvents().Count.Should().Be(1);
        var domainEvent = aggregate.GetUncommittedEvents().First();
        domainEvent.Should().BeOfType<TestEvent>();
        ((TestEvent)domainEvent).Data.Should().Be("Test");
    }

    [Fact]
    public void Emit_WithNullEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act & Assert
        Action act = () => aggregate.EmitNullEvent();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Emit_WithEvent_SetsEventMetadata()
    {
        // Arrange
        var id = Guid.NewGuid();
        var aggregate = new TestAggregate(id);

        // Act
        aggregate.EmitTestEvent("Test");

        // Assert
        var domainEvent = (TestEvent)aggregate.GetUncommittedEvents().First();
        domainEvent.EventMetadata.AggregateId.Should().Be(id.ToString());
        domainEvent.EventMetadata.AggregateType.ToString().Should().Be(AggregateType.From<TestAggregate>().ToString());
        domainEvent.EventMetadata.AggregateVersion.Should().Be(1);
        domainEvent.EventMetadata.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void MarkChangesAsCommitted_ClearsUncommittedEvents()
    {
        // Arrange
        var aggregate = new TestAggregate();
        aggregate.EmitTestEvent("Test");

        // Act
        aggregate.MarkChangesAsCommitted();

        // Assert
        aggregate.GetUncommittedEvents().Count.Should().Be(0);
    }

    [Fact]
    public void ValidateVersion_WithCorrectVersion_DoesNotThrow()
    {
        // Arrange
        var aggregate = new TestAggregate();
        aggregate.EmitTestEvent("Test");

        // Act & Assert
        Action act = () => aggregate.ValidateVersion();
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateVersion_WithIncorrectVersion_ThrowsConcurrencyException()
    {
        // Arrange
        var aggregate = new TestAggregate();
        aggregate.EmitTestEvent("Test");

        // Simulate external version change
        var privateField = typeof(TestAggregate).GetProperty("Version");
        privateField.SetValue(aggregate, 10ul);

        // Act & Assert
        Action act = () => aggregate.ValidateVersion();
        act.Should().Throw<ConcurrencyException>();
    }

    [Fact]
    public void ValidateState_WithDefaultId_ThrowsBadAggregateIdException()
    {
        // Arrange
        var aggregate = new TestAggregate();
        
        typeof(TestAggregate).GetProperty("Id")?.SetValue(aggregate, Guid.Empty);
        
        // Act & Assert
        Action act = () => aggregate.ValidateStatePublic();
        act.Should().Throw<BadAggregateIdException>();
    }

    [Fact]
    public void ValidateState_WithValidId_DoesNotThrow()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());

        // Act & Assert
        Action act = () => aggregate.ValidateStatePublic();
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyChanges_WithValidEvents_SetsVersion()
    {
        // Arrange
        var aggregate = new TestAggregate();
        var event1 = new TestEvent("Test1", new EventMetadata { AggregateVersion = 0 });
        var event2 = new TestEvent("Test2", new EventMetadata { AggregateVersion = 1 });

        // Act
        aggregate.ApplyChangesPublic(event1, event2);

        // Assert
        aggregate.Version.Should().Be(2);
    }

    [Fact]
    public void ApplyChanges_WithNullEvents_ThrowsArgumentNullException()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act & Assert
        Action act = () => aggregate.ApplyChangesPublic(null);
        act.Should().Throw<ArgumentNullException>();
    }

    // Helper classes for testing
    
}