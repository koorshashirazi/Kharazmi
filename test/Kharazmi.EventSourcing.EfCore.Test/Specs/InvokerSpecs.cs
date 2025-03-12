
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.EventSourcing.EfCore.Test.Specs.SpeechAggregate;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.Test.Specs
{
    [CollectionDefinition(nameof(InvokerSpecs), DisableParallelization = true)]
    public class InvokerSpecsCollection { }

    [Collection(nameof(InvokerSpecs))]
    public class InvokerSpecs
    {
        [Fact(DisplayName = "CreateInstance Of AggregateRoot Should Return AN Empty Aggregate")]
        public void CreateInstanceOfAggregateRootShouldReturnEmptyAggregate()
        {
            //Arrange
            var sut = new AggregateFactory();

            //Act
            var result = sut.Create<Speech, string>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<Speech>(result);

            Assert.Equal(AspNetCore.Core.ValueObjects.Id.Default<string>(), result.Id);
        }
    }
}