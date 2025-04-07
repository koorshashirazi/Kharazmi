
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.SpeechAggregate;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs
{
    [CollectionDefinition(nameof(AggregateFactoryTest), DisableParallelization = true)]
    public class AggregateFactoryTestCollection { }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Collection(nameof(AggregateFactoryTest))]
    public class AggregateFactoryTest
    {
        [Fact(DisplayName = "CreateInstance Of AggregateRoot Should Return AN Empty Aggregate")]
        public void CreateInstanceOfAggregateRootShouldReturnEmptyAggregate()
        {
            //Arrange
            using var instanceCreator = new InstanceCreator();
            var sut = new AggregateFactory(instanceCreator);

            //Act
            var result = sut.GetOrCreate<Speech, string>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<Speech>(result);

            Assert.Equal(AspNetCore.Core.ValueObjects.Id.Default<string>(), result.Id);
        }
    }
}