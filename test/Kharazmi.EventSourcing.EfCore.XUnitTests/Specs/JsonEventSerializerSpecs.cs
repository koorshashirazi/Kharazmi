using System.Globalization;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Events;
using Moq;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs
{
    [CollectionDefinition(nameof(JsonEventSerializerSpecs), DisableParallelization = true)]
    public class JsonEventSerializerSpecsCollection
    {
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Collection(nameof(JsonEventSerializerSpecs))]
    public class JsonEventSerializerSpecs
    {
        [Fact(DisplayName = "Deserialize Event Stream should return an event")]
        public void DeserializeEventStreamShouldReturnAnEvent()
        {
            //Arrange
            var mediaFileCreatedEvent = new MediaFileCreatedEvent("aggregate-123",
                Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851"), "https://example.com/mediafile.jpg");
            mediaFileCreatedEvent.EventMetadata
                .SetSourceId(new SourceId("MediaService"))
                .SetTimestamp(DateTimeOffset.Parse("2025-03-10T12:00:00Z", CultureInfo.InvariantCulture))
                .SetEventId(new DomainEventId("MediaFileCreated"))
                .SetEventType(new DomainEventType("event-456"));

            const string mediaFileCreatedEventJson = """
                                                     {
                                                       "File": "https://example.com/mediafile.jpg",
                                                       "AggregateId": "aggregate-123",
                                                       "MediaFileId": "d290f1ee-6c54-4b01-90e6-d701748f0851",
                                                       "EventMetadata": {
                                                         "Source": "MediaService",
                                                         "Timestamp": "2025-03-10T12:00:00Z"
                                                       },
                                                       "EventType": {
                                                         "Value": "MediaFileCreated"
                                                       },
                                                       "EventId": {
                                                         "Value": "event-456"
                                                       }
                                                     }
                                                     """;
            Mock<IJsonSerializer> moqJsonProvider = new Mock<IJsonSerializer>();
            moqJsonProvider
                .Setup(m => m.Deserialize<MediaFileCreatedEvent>(mediaFileCreatedEventJson))
                .Returns(mediaFileCreatedEvent);

            //Act
            IEventSerializer sut = new JsonEventSerializer(moqJsonProvider.Object);

            var result = sut.Deserialize<MediaFileCreatedEvent>(mediaFileCreatedEventJson);
            //Assert
            Assert.Equal(mediaFileCreatedEvent, result);
        }

        [Fact(DisplayName = "Serialize event stream should return a string")]
        public void SerializeEventStreamShouldReturnAString()
        {
            //Arrange
            var mediaFileCreatedEvent = new MediaFileCreatedEvent("aggregate-123",
                Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851"), "https://example.com/mediafile.jpg");
            mediaFileCreatedEvent.EventMetadata
                .SetSourceId(new SourceId("MediaService"))
                .SetTimestamp(DateTimeOffset.Parse("2025-03-10T12:00:00Z", CultureInfo.InvariantCulture))
                .SetEventId(new DomainEventId("MediaFileCreated"))
                .SetEventType(new DomainEventType("event-456"));

            const string mediaFileCreatedEventJson = """
                                                     {
                                                       "File": "https://example.com/mediafile.jpg",
                                                       "AggregateId": "aggregate-123",
                                                       "MediaFileId": "d290f1ee-6c54-4b01-90e6-d701748f0851",
                                                       "EventMetadata": {
                                                         "Source": "MediaService",
                                                         "Timestamp": "2025-03-10T12:00:00Z"
                                                       },
                                                       "EventType": {
                                                         "Value": "MediaFileCreated"
                                                       },
                                                       "EventId": {
                                                         "Value": "event-456"
                                                       }
                                                     }
                                                     """;
            
            Mock<IJsonSerializer> moqJsonProvider = new Mock<IJsonSerializer>();
            moqJsonProvider.Setup(m => m.Serialize(mediaFileCreatedEvent)).Returns(mediaFileCreatedEventJson);

            //Act
            IEventSerializer sut = new JsonEventSerializer(moqJsonProvider.Object);

            var result = sut.Serialize(mediaFileCreatedEvent);
            //Assert
            Assert.Equal(mediaFileCreatedEventJson, result);
        }
    }
}