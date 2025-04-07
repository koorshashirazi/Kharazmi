using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Events;
using Moq;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs
{
    [CollectionDefinition(nameof(DomainEventRebuilderSpecs), DisableParallelization = true)]
    public class DomainEventRebuilderSpecsCollection
    {
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Collection(nameof(DomainEventRebuilderSpecs))]
    public class DomainEventRebuilderSpecs
    {
        [Fact]
        public void RebuildDomainEvents()
        {
            //Arrange
            var aggregateId = Id.New<string>();
            var payload = """
                          {
                              "FullName": "Georges Dupont",
                              "Adresse": "45 av charles degaulle paris, france"
                          }
                          """;

            var events = new List<IDomainEvent>
            {
                new PersonCreatedEventTest("Georges Dupont", "45 av charles degaulle paris, france")
            };

            var eventType = DomainEventType.From<PersonCreatedEventTest>();
            var eventStoreItems = new List<EventStoreEntity>
            {
                new(aggregateId, 0, "aggregate_type", eventType.ToString(),
                    DateTimeOffset.UtcNow, payload)
            };

            var moqEventSerializer = new Mock<IEventSerializer>();

            moqEventSerializer
                .Setup(m => m.Deserialize(payload,eventType.ToType()))
                .Returns(events.FirstOrDefault());

            var sut = new DomainEventFactory(moqEventSerializer.Object);

            var result = sut.CreateFrom(eventStoreItems);

            Assert.Equal(events, result);
        }
    }
}