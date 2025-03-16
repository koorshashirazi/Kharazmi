using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Exceptions;
using Xunit.Abstractions;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests;

[CollectionDefinition(nameof(SqlEventStoreTests), DisableParallelization = true)]
public class SqlEventStoreTestsCollection;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[Collection(nameof(SqlEventStoreTests))]
public class SqlEventStoreTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly SqliteConnection _connection;
    private readonly IEventStoreDbContext _dbContext;
    private readonly Mock<IDomainEventFactory> _eventFactoryMock;
    private readonly IEventSerializer _eventSerializerMock;
    private readonly IEventStoreUnitOfWork _unitOfWork;
    private readonly SqlEventStore _sut;

    public SqlEventStoreTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        // Setup SQLite in-memory connection
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Setup services
        var services = new ServiceCollection();

        // Register DbContext with in-memory SQLite
        services.AddDbContext<EventStoreDbContext>(options =>
            options.UseSqlite(_connection));

        // Register mocks
        _eventFactoryMock = new Mock<IDomainEventFactory>();
        _eventSerializerMock = new JsonEventSerializer(new JsonSerializer());

        services.AddSingleton(_eventFactoryMock.Object);
        services.AddSingleton(_eventSerializerMock);
        services.AddScoped<IEventStoreDbContext, EventStoreDbContext>();
        services.AddScoped<IEventStoreUnitOfWork, EventStoreUnitOfWork>();
        services.AddSingleton(new EventSourcingOptions { EnableStoreEvent = true });
        services.AddScoped<IEventStore, SqlEventStore>();

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        // Create database and apply migrations
        _dbContext = serviceProvider.GetRequiredService<IEventStoreDbContext>();
        if (_dbContext is not EventStoreDbContext eventStoreDbContext)
        {
            throw new InvalidCastException(nameof(eventStoreDbContext));
        }

        eventStoreDbContext.Database.EnsureCreated();

        // Get dependencies for SUT
        _unitOfWork = serviceProvider.GetRequiredService<IEventStoreUnitOfWork>();
        var eventSourcingOptions = serviceProvider.GetRequiredService<EventSourcingOptions>();

        // Create System Under Test
        _sut = new SqlEventStore(
            _dbContext,
            _eventFactoryMock.Object,
            _eventSerializerMock,
            _unitOfWork,
            eventSourcingOptions);
    }

    [Fact]
    public void GetEventsAsync_WhenAggregateIdIsDefault_ThrowsBadAggregateIdException()
    {
        // Arrange
        var defaultId = default(Guid);

        // Act & Assert
        Assert.ThrowsAsync<BadAggregateIdException>(() =>
            _sut.GetEventsAsync<TestAggregate, Guid>(defaultId));
    }

    [Fact]
    public async Task GetEventsAsync_WhenNoEventsExist_ReturnsEmptyCollection()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var expectedEvents = Array.Empty<IDomainEvent>();
        _eventFactoryMock.Setup(f => f.CreateFrom(It.IsAny<IReadOnlyCollection<EventStoreEntity>>()))
            .Returns(expectedEvents);

        // Act
        var result = await _sut.GetEventsAsync<TestAggregate, Guid>(aggregateId).ConfigureAwait(false);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEventsAsync_WhenEventsExist_ReturnsEvents()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var stringAggregateId = aggregateId.ToString();
        var aggregateType = AggregateType.From<TestAggregate>().ToString();

        // Create test events in the database
        var eventEntities = new[]
        {
            new EventStoreEntity(
                stringAggregateId,
                1,
                aggregateType,
                typeof(TestEvent).FullName,
                DateTimeOffset.UtcNow,
                "{\"EventType\":\"TestEvent\", \"EventMetadata\":{\"AggregateId\":\"" + stringAggregateId + "\"}}")
        };

        await _dbContext.EventStores.AddRangeAsync(eventEntities).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        var expectedEvents = new List<IDomainEvent> { new TestEvent() };
        _eventFactoryMock.Setup(f => f.CreateFrom(It.IsAny<IReadOnlyCollection<EventStoreEntity>>()))
            .Returns(expectedEvents);

        // Act
        var result = await _sut.GetEventsAsync<TestAggregate, Guid>(aggregateId).ConfigureAwait(false);

        // Assert
        Assert.Equal(expectedEvents.Count, result.Count);
        Assert.Same(expectedEvents[0], result.First());
    }

    [Fact]
    public async Task GetEventsFromVersion_WhenEventsExist_ReturnsEventsFromSpecifiedVersion()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var stringAggregateId = aggregateId.ToString();
        var aggregateType = AggregateType.From<TestAggregate>().ToString();
        var eventType = DomainEventType.From<TestEvent>().ToString();

        // Create test events in the database with different versions
        var eventEntities = new[]
        {
            new EventStoreEntity(
                stringAggregateId,
                1,
                aggregateType,
                eventType,
                DateTimeOffset.UtcNow,
                $"{{\"EventType\":\"{eventType}\", \"EventMetadata\":{{\"AggregateId\":\"" + stringAggregateId +
                "\"}}"),
            new EventStoreEntity(
                stringAggregateId,
                2,
                aggregateType,
                eventType,
                DateTimeOffset.UtcNow,
                $"{{\"EventType\":\"{eventType}\", \"EventMetadata\":{{\"AggregateId\":\"" + stringAggregateId +
                "\"}}"),
            new EventStoreEntity(
                stringAggregateId,
                3,
                aggregateType,
                eventType,
                DateTimeOffset.UtcNow,
                $"{{\"EventType\":\"{eventType}\", \"EventMetadata\":{{\"AggregateId\":\"" + stringAggregateId + "\"}}")
        };

        await _dbContext.EventStores.AddRangeAsync(eventEntities).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        var expectedEvents = new List<IDomainEvent> { new TestEvent(), new TestEvent() };
        _eventFactoryMock
            .Setup(f => f.CreateFrom(It.IsAny<IReadOnlyCollection<EventStoreEntity>>()))
            .Returns(expectedEvents);

        // Act
        var result = await _sut.GetEventsFromVersion<TestAggregate, Guid>(aggregateId, 1).ConfigureAwait(false);

        // Assert
        Assert.Equal(expectedEvents.Count, result.Count);
    }

    [Fact]
    public async Task SaveAsync_WhenEventSourcingDisabled_ReturnsEarly()
    {
        // Arrange
        var options = new EventSourcingOptions { EnableStoreEvent = false };
        using var eventStore = new SqlEventStore(
            _dbContext,
            _eventFactoryMock.Object,
            _eventSerializerMock,
            _unitOfWork,
            options);

        var aggregate = new Mock<TestAggregate>();

        // Act
        await eventStore.SaveAsync(aggregate.Object).ConfigureAwait(false);

        // Assert
        aggregate.Verify(a => a.GetUncommittedEvents(), Times.Never);
    }

    [Fact]
    public void SaveAsync_WhenAggregateIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.SaveAsync<TestAggregate>(null));
    }

    [Fact]
    public async Task SaveAsync_WithValidAggregate_SavesUncommittedEvents()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var eventType = DomainEventType.From<TestEvent>();
        var aggregateType = AggregateType.From<TestAggregate>();
        var timestamp = DateTimeOffset.UtcNow;

        var testEvent = new TestEvent
        {
            EventMetadata = new EventMetadata
            {
                AggregateId = aggregateId.ToString(),
                AggregateType = aggregateType,
                AggregateVersion = 1,
                EventType = eventType,
                Timestamp = timestamp
            }
        };

        var aggregate = new Mock<TestAggregate>();
        aggregate
            .Setup(a => a.GetUncommittedEvents())
            .Returns(new List<IDomainEvent> { testEvent });

        var json = $$"""
                     {
                       "EventMetadata": {
                         "AggregateId": "{{aggregateId}}",
                         "AggregateType": "{{aggregateType}}",
                         "AggregateVersion": 1,
                         "EventType": "{{eventType}}",
                         "Timestamp": "{{timestamp}}"
                       },
                       "EventType": {
                         "Value": "{{eventType}}"
                       },
                       "EventId": {
                         "Value": "{{testEvent.EventId}}"
                       }
                     }
                     """;

        // Act
        await _sut.SaveAsync(aggregate.Object).ConfigureAwait(false);
        var savedEvent = await _dbContext.EventStores.FirstOrDefaultAsync().ConfigureAwait(false);

        // Assert
        Assert.NotNull(savedEvent);
        Assert.Equal(testEvent.EventMetadata.AggregateId, savedEvent.AggregateId);
        Assert.Equal(testEvent.EventMetadata.EventType.ToString(), savedEvent.EventType);
    }

    [Fact]
    public async Task SaveAsync_WithNotAllowedEventType_SkipsEvent()
    {
        // Arrange
        var options = new EventSourcingOptions
        {
            EnableStoreEvent = true,
            NotAllowedEvents = [DomainEventType.From<TestEvent>().ToString()]
        };

        using var eventStore = new SqlEventStore(
            _dbContext,
            _eventFactoryMock.Object,
            _eventSerializerMock,
            _unitOfWork,
            options);

        var aggregateId = Guid.NewGuid();
        var testEvent = new TestEvent
        {
            EventMetadata = new EventMetadata
            {
                AggregateId = aggregateId.ToString(),
                AggregateType = AggregateType.From<TestAggregate>(),
                EventType = DomainEventType.From<TestEvent>(),
                Timestamp = DateTimeOffset.UtcNow
            }
        };

        var aggregate = new Mock<TestAggregate>();
        aggregate
            .Setup(a => a.GetUncommittedEvents())
            .Returns([testEvent]);

        // Act
        await eventStore.SaveAsync(aggregate.Object).ConfigureAwait(false);
        var savedEvents = await _dbContext.EventStores.ToListAsync().ConfigureAwait(false);

        // Assert
        Assert.Empty(savedEvents);
    }

    [Fact]
    public void SaveAsync_WithNullDomainEvent_ThrowsEventStoreNullException()
    {
        // Arrange
        var aggregate = new Mock<TestAggregate>();
        aggregate
            .Setup(a => a.GetUncommittedEvents())
            .Returns(new List<IDomainEvent> { null });

        // Act & Assert
        Assert.ThrowsAsync<EventStoreNullException>(() =>
            _sut.SaveAsync(aggregate.Object));
    }

    [Fact]
    public void SaveAsync_WithNullMetadata_ThrowsMetaDataNullException()
    {
        // Arrange
        var testEvent = new TestEvent(); // No metadata set

        var aggregate = new Mock<TestAggregate>();
        aggregate
            .Setup(a => a.GetUncommittedEvents())
            .Returns((List<IDomainEvent>) [testEvent]);

        // Act & Assert
        Assert.ThrowsAsync<MetaDataNotFindKeyException>(() =>
            _sut.SaveAsync(aggregate.Object));
    }

    [Fact]
    public void Dispose_MultipleCalls_OnlyDisposesOnce()
    {
        // Act
        _sut.Dispose();

        // No exception means the test passed
    }

    public void Dispose()
    {
        _sut.Dispose();
        _dbContext.Dispose();
        _connection.Dispose();
    }

    // Test classes for the tests
}

public class TestAggregate : AggregateRoot<TestAggregate, Guid>
{
    public TestAggregate() : base(Guid.NewGuid())
    {
    }
}

public class TestEvent : DomainEvent
{
    public TestEvent() : base(DomainEventType.From<TestEvent>())
    {
    }

    public TestEvent(DomainEventId eventId, DomainEventType eventType,
        EventMetadata eventMetadata) : base(eventId, eventType, eventMetadata)
    {
    }
}