using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest.Events;
using Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest.SpeechAggregate;
using Moq;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.EventSourcingTest
{
    [CollectionDefinition(nameof(EventSourcingUnitTest), DisableParallelization = true)]
    public class EventSourcingUnitTestCollection
    {
    }
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Collection(nameof(EventSourcingUnitTest))]
    public class EventSourcingUnitTest
    {
        [Fact]
        public void GetUncommittedEvents_WhenNoEvents_ReturnsEmptyCollection()
        {
            // Arrange
            var aggregate = new TestAggregate();

            // Act
            var events = aggregate.GetUncommittedEvents();

            // Assert
            events.Should().BeEmpty();
        }

        [Fact]
        public void GetUncommittedEvents_AfterEmittingEvents_ReturnsAllEvents()
        {
            // Arrange
            var aggregate = new TestAggregate();
            aggregate.EmitTestEvent("Event1");
            aggregate.EmitTestEvent("Event2");

            // Act
            var events = aggregate.GetUncommittedEvents();

            // Assert
            events.Count.Should().Be(2);
            events.Select(e => ((TestEvent)e).Data).Should().BeEquivalentTo("Event1", "Event2");
        }

        [Fact]
        public void MarkChangesAsCommitted_ClearsAllUncommittedEvents()
        {
            // Arrange
            var aggregate = new TestAggregate();
            aggregate.EmitTestEvent("Event1");
            aggregate.EmitTestEvent("Event2");

            // Act
            aggregate.MarkChangesAsCommitted();

            // Assert
            aggregate.GetUncommittedEvents().Should().BeEmpty();
        }

        [Fact]
        public void ValidateVersion_WithCorrectVersion_DoesNotThrow()
        {
            // Arrange
            var aggregate = new TestAggregate();
            aggregate.EmitTestEvent("Event1");

            // Act & Assert
            Action act = () => aggregate.ValidateVersion();
            act.Should().NotThrow();
        }

        [Fact]
        public void ValidateVersion_WithIncorrectVersion_ThrowsConcurrencyException()
        {
            // Arrange
            var aggregate = new TestAggregate();
            aggregate.EmitTestEvent("Event1");

            // Manually change the version to simulate concurrency issue
            typeof(TestAggregate).GetProperty("Version").SetValue(aggregate, 10ul);

            // Act & Assert
            Action act = () => aggregate.ValidateVersion();
            act.Should().Throw<ConcurrencyException>()
                .WithMessage($"Invalid version specified : expectedVersion = 1 but  originalVersion = 10.");
        }

        [Fact]
        public async Task ApplyFromHistoryAsync_WithNullEventStore_ThrowsArgumentNullException()
        {
            // Arrange
            var aggregate = new TestAggregate();

            // Act & Assert
            Func<Task> act = async () => await aggregate.ApplyFromHistoryAsync(null).ConfigureAwait(false);
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'eventStore')").ConfigureAwait(false);
        }

        [Fact]
        public async Task ApplyFromHistoryAsync_WithValidEventStore_AppliesEvents()
        {
            // Arrange
            var aggregate = new TestAggregate(Guid.NewGuid());
            var mockEventStore = new Mock<IEventStore>();

            var events = new List<IDomainEvent>
            {
                new TestEvent("HistoricalEvent1") { EventMetadata = new EventMetadata { AggregateVersion = 0 } },
                new TestEvent("HistoricalEvent2") { EventMetadata = new EventMetadata { AggregateVersion = 1 } }
            };

            mockEventStore
                .Setup(es => es.GetEventsAsync<TestAggregate, Guid>(aggregate.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            // Act
            await aggregate.ApplyFromHistoryAsync(mockEventStore.Object).ConfigureAwait(false);

            // Assert
            aggregate.Version.Should().Be(2);
            aggregate.Data.Should().Be("HistoricalEvent2"); // Last event applied
            mockEventStore.Verify(
                es => es.GetEventsAsync<TestAggregate, Guid>(aggregate.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CommitAsync_WithNullEventStore_ThrowsArgumentNullException()
        {
            // Arrange
            var aggregate = new TestAggregate(Guid.NewGuid());

            // Act & Assert
            Func<Task> act = async () => await aggregate.CommitAsync(null).ConfigureAwait(false);
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'eventStore')");
        }

        [Fact]
        public async Task CommitAsync_WithValidEventStore_SavesEvents()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aggregate = new TestAggregate(id);
            aggregate.EmitTestEvent("NewEvent");

            var mockEventStore = new Mock<IEventStore>();
            mockEventStore
                .Setup(es => es.SaveAsync(It.IsAny<TestAggregate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await aggregate.CommitAsync(mockEventStore.Object);

            // Assert
            mockEventStore.Verify(
                es => es.SaveAsync(It.Is<AggregateRoot<TestAggregate, Guid>>(a => a.Id.Equals(id)), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CommitAsync_WithInvalidState_ThrowsBadAggregateIdException()
        {
            // Arrange
            var aggregate = new TestAggregate();
            var mockEventStore = new Mock<IEventStore>();

            typeof(TestAggregate).GetProperty("Id")?.SetValue(aggregate, Guid.Empty);

            // Act & Assert
            Func<Task> act = async () => await aggregate.CommitAsync(mockEventStore.Object).ConfigureAwait(false);
            await act.Should().ThrowAsync<BadAggregateIdException>().ConfigureAwait(false);
        }

        [Fact]
        public void ValidateVersionWithInvalidExpectedVersionShouldRaiseConcurrencyException()
        {
            //Arrange
            const ulong currentVersion = 1;
            var id = Id.New<string>();
            var source = new EventSourcingAggregateTest(id, currentVersion);

            typeof(EventSourcingAggregateTest).GetProperty("Version")?.SetValue(source, 10ul);

            //Act
            //Assert
            Assert.Throws<ConcurrencyException>(() => source.ValidateVersion());
        }

        [Fact]
        public void ValidateVersionWithValidExpectedVersionThenExpectedVersionShouldBeEqualsToAggregateVersion()
        {
            //Arrange
            const ulong expectedVersion = 2;
            var id = Id.New<string>();
            var source = new EventSourcingAggregateTest(id, 2);

            //Act
            source.ValidateVersion();

            //Assert
            Assert.Equal(expectedVersion, source.Version);
        }

        [Fact]
        public void ApplyEventShouldPopulateAggregatePropertiesWithEventProperties()
        {
            //Arrange
            var id = Id.New<string>();

            var evt = new SpeechCreatedEvent(id,
                "SpeechCreatedEvent Title ",
                "http://url-evt.com",
                "SpeechCreatedEvent description must be very long as a description than people can understand without efforts",
                "Conferences");

            //Act
            var aggregate = new Speech(id,
                new Title("SpeechCreatedEvent Title "),
                new UrlValue("http://url-evt.com"),
                new Description(
                    "SpeechCreatedEvent description must be very long as a description than people can understand without efforts"),
                new SpeechType("Conferences"));

            //Assert
            Assert.Equal(evt.EventMetadata.AggregateId, aggregate.Id);
            Assert.Equal(evt.Title, aggregate.Title.Value);
            Assert.Equal(evt.Url, aggregate.Url.Value);
            Assert.Equal(evt.Description, aggregate.Description.Value);
            Assert.Equal(evt.Type, aggregate.Type.Value.ToString());

            Assert.IsAssignableFrom<IDomainEvent>(evt);
        }

        [Fact]
        public void GetUncommittedEventsOfNewAggregateShouldReturnListOfIDomainEvent()
        {
            //Arrange
            const ulong expectedVersion = 1;
            var id = Id.New<string>();
            IEventSourcing aggregate = new EventSourcingAggregateTest(id, expectedVersion);

            //Act
            var result = aggregate.GetUncommittedEvents();

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.IsAssignableFrom<IEnumerable<IDomainEvent>>(result);
        }

        [Fact]
        public void AddDomainEventWithInvalidVesrionShouldRaiseConncurrencyException()
        {
            //Arrange
            const ulong currentVersion = 1;
            var id = Id.New<string>();
            var sut = new EventSourcingAggregateTest(id, currentVersion);
            const int value = 10;
            typeof(EventSourcingAggregateTest).GetProperty("Version")?.SetValue(sut, 10ul);

            //Act
            sut.IncrementValue(value);


            //Assert
            Assert.Throws<ConcurrencyException>(() => sut.ValidateVersion());
        }

        [Fact]
        public void AddDomainEventWithValidVersionThenVersionOfEventShouldBeEqualsToCurrentVerSionOfAggregate()
        {
            //Arrange
            const ulong expectedVersion = 1;
            var id = Id.New<string>();
            var sut = new EventSourcingAggregateTest(id, expectedVersion);
            const int value = 10;

            //Act
            sut.IncrementValue(value);
            SubEventIncremented domainEvent = (SubEventIncremented)sut.GetUncommittedEvents().Single();

            //Assert
            Assert.Equal(sut.Version, domainEvent.EventMetadata.AggregateVersion);
        }

        [Fact]
        public void AddDomainEventWithValidVersionThenEventShouldBeAppliedToAggregate()
        {
            //Arrange
            const ulong expectedVersion = 1;
            var id = Id.New<string>();
            var sut = new EventSourcingAggregateTest(id, expectedVersion);
            const int value = 10;

            //Act
            sut.IncrementValue(value);
            SubEventIncremented domainEvent = (SubEventIncremented)sut.GetUncommittedEvents().Single();

            //Assert
            sut.Id.Should().Be(domainEvent.EventMetadata.AggregateId);
            Assert.Equal(sut.Value, domainEvent.Value);
        }


        [Fact]
        public void ClearUncommittedEventsThenUncommittedEventsShouldBeEmpty()
        {
            //Arrange
            const ulong expectedVersion = 1;
            var sut = new EventSourcingAggregateTest(Id.New<string>(), expectedVersion);

            //Act
            sut.IncrementValue(10);
            sut.MarkChangesAsCommitted();
            var uncommittedEvents = sut.GetUncommittedEvents();

            //Assert
            Assert.NotNull(uncommittedEvents);
            Assert.Empty(uncommittedEvents);
        }
    }

    public class TestAggregate : AggregateRoot<TestAggregate, Guid>
    {
        public string Data { get; private set; }

        public TestAggregate() : this(Guid.NewGuid())
        {
        }

        public TestAggregate(Guid id, ulong version = 0) : base(id, version)
        {
            RegisterApplier<TestEvent>(Apply);
        }

        private void Apply(TestEvent evt)
        {
            Data = evt.Data;
        }

        public void EmitTestEvent(string data)
        {
            Emit(new TestEvent(data));
        }
    }

    public class TestEvent : DomainEvent
    {
        public string Data { get; }

        public TestEvent(string data) : base(DomainEventType.From<TestEvent>())
        {
            Data = data;
        }

        public TestEvent(DomainEventId eventId, DomainEventType eventType,
            EventMetadata eventMetadata) : base(eventId, eventType, eventMetadata)
        {
        }
    }
}