using FluentAssertions;
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using NSubstitute;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates;

public class AggregateFactoryTests
{
    [Fact]
    public void GetOrCreate_ShouldCreateAndReturnNewAggregate_WhenAggregateDoesNotExist()
    {
        // Arrange 
        var expectedAggregate = new TestAggregate();

        var instanceCreator = Substitute.For<IInstanceCreator>();
        instanceCreator.CreateInstance<TestAggregate>().Returns(expectedAggregate);

        var aggregateFactory = new AggregateFactory(instanceCreator);

        // Act 
        var actualAggregate = aggregateFactory.GetOrCreate<TestAggregate, Guid>();

        // Assert 
        actualAggregate.Should().Be(expectedAggregate);
        instanceCreator.Received(1).CreateInstance<TestAggregate>();
    }

    [Fact]
    public void AggregateFactory_Constructor_ShouldThrowArgumentNullException_WhenInstanceCreatorIsNull()
    {
        // Arrange, Act, Assert 
        Assert.Throws<ArgumentNullException>(() => new AggregateFactory(null));
    }

    [Fact]
    public void GetOrCreate_ShouldReturnDifferentInstancesForDifferentAggregateTypes()
    {
        // Arrange 
        var expectedAggregate1 = new TestAggregate();
        var expectedAggregate2 = new AnotherTestAggregate();

        var instanceCreator = Substitute.For<IInstanceCreator>();

        instanceCreator.CreateInstance<TestAggregate>().Returns(expectedAggregate1);
        instanceCreator.CreateInstance<AnotherTestAggregate>().Returns(expectedAggregate2);

        var aggregateFactory = new AggregateFactory(instanceCreator);

        // Act
        var actualAggregate1 = aggregateFactory.GetOrCreate<TestAggregate, Guid>();
        var actualAggregate2 = aggregateFactory.GetOrCreate<AnotherTestAggregate, int>();

        // Assert 
        actualAggregate1.Should().Be(expectedAggregate1);
        actualAggregate2.Should().Be(expectedAggregate2);
        actualAggregate1.Should().NotBe(actualAggregate2);
    }


    [Fact]
    public void GetOrCreate_ShouldWorkCorrectlyAfterMultipleCallsForTheSameType()
    {
        // Arrange 
        var expected = new TestAggregate();

        var instanceCreator = Substitute.For<IInstanceCreator>();
        instanceCreator.CreateInstance<TestAggregate>(null).Returns(expected);

        var aggregateFactory = new AggregateFactory(instanceCreator);

        // Act 
        var actualAggregate1 = aggregateFactory.GetOrCreate<TestAggregate, Guid>();
        var actualAggregate2 = aggregateFactory.GetOrCreate<TestAggregate, Guid>();

        // Assert 
        actualAggregate1.Should().Be(expected);
        actualAggregate2.Should().Be(expected);
        actualAggregate1.Should().Be(actualAggregate2);
        instanceCreator.Received(1).CreateInstance<TestAggregate>(); // Only created once. 
    }

    [Fact]
    public void CreateInstance_ShouldBeCalledOnce()
    {
        // Arrange
        var instanceCreator = new InstanceCreator();
        var wrapper = new InstanceCreatorWrapper(instanceCreator);
        var aggregateFactory = new AggregateFactory(wrapper);

// Act
        var actualAggregate1 = aggregateFactory.GetOrCreate<TestAggregate, Guid>();
        var actualAggregate2 = aggregateFactory.GetOrCreate<TestAggregate, Guid>();

// Assert
        actualAggregate1.Should().Be(actualAggregate2);
        wrapper.CreateInstanceCallCount.Should().Be(1); // Ensure CreateInstance called only once
    }


    [Fact]
    public void NSub_Test()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.Add(1, 2).Returns(3);
        calculator.Add(1, 2).Should().Be(3);
    }

    [Fact]
    public void NSub_Test_2()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.Add(1, 2);
        calculator.Received().Add(1, 2);
        calculator.DidNotReceive().Add(5, 7);
    }
}

public interface ICalculator
{
    int Add(int a, int b);
    string Mode { get; set; }
    event EventHandler PoweringUp;
}

public class InstanceCreatorWrapper : IInstanceCreator
{
    private readonly InstanceCreator _instanceCreator;
    public int CreateInstanceCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }

    public InstanceCreatorWrapper(InstanceCreator instanceCreator)
    {
        _instanceCreator = instanceCreator;
    }

    public T CreateInstance<T>() where T : new()
    {
        CreateInstanceCallCount++;
        return _instanceCreator.CreateInstance<T>();
    }

    public object CreateInstance(Type concreteType, Action<InstanceCreatorOptions>? options = null,
        params object[] primitiveArguments)
    {
        CreateInstanceCallCount++;
        return _instanceCreator.CreateInstance(concreteType, options, primitiveArguments);
    }

    public T CreateInstance<T>(Action<InstanceCreatorOptions>? options = null, params object[] primitiveArguments)
    {
        CreateInstanceCallCount++;
        return _instanceCreator.CreateInstance<T>(options, primitiveArguments);
    }

    public void Dispose()
    {
        DisposeCallCount++;
        _instanceCreator.Dispose();
    }
}