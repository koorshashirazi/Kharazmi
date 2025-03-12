using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.Test.Specs
{
    [CollectionDefinition(nameof(EventStoreSpecs), DisableParallelization = true)]
    public class EventStoreRepositorySpecsCollection
    {
    }

    [Collection(nameof(EventStoreSpecs))]
    public class EventStoreSpecs
    {
        [Fact(DisplayName = "AppendAsync should append an event on eventstore")]
        public async Task AppendAsyncShouldAppendAnEventOnEventStore()
        {
            //Arrange
            await using var connection = new SqliteConnection("DataSource=:memory:");
            // await using var connection = new SqliteConnection("Data Source=SharedInMemoryDb;Mode=Memory;Cache=Shared");
            await connection.OpenAsync().ConfigureAwait(false);

            var options = new DbContextOptionsBuilder<EventStoreDbContext>()
                .UseSqlite(connection)
                .Options;

            var storeDbContext = new EventStoreDbContext(options);
            await storeDbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);

            var eventSerializer = new JsonEventSerializer(new JsonSerializer());
            var uow = new EventStoreUnitOfWork(storeDbContext);

            var eventStore = new SqlEventStore(storeDbContext,
                new DomainEventFactory(eventSerializer), eventSerializer, uow, new EventSourcingOptions());

            var aggregateId = Id.New<string>();
            var aggregateTest = new PersonAggregateTest(aggregateId, "Dupont", "45 av charles degaulle paris, france");

            var domainEvent = aggregateTest.GetUncommittedEvents().First();
            var payload = eventSerializer.Serialize(domainEvent);

            var eventStoreEntity = new EventStoreEntity(aggregateTest.Id,
                aggregateTest.Version, aggregateTest.GetAggregateType(), domainEvent.EventType,
                domainEvent.EventMetadata.Timestamp, payload);

            //Act
            await aggregateTest.CommitAsync(eventStore).ConfigureAwait(false);
            var result = await storeDbContext.EventStores.SingleOrDefaultAsync().ConfigureAwait(false);
            eventStore.Dispose();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(result.Id, Guid.Empty);
            aggregateTest.IsCommitted().Should().BeTrue();
            aggregateTest.GetUncommittedEvents().Count.Should().Be(0);
            Assert.Equal(eventStoreEntity.AggregateId, result.AggregateId);
            Assert.Equal(eventStoreEntity.AggregateType, result.AggregateType);
            Assert.Equal(eventStoreEntity.EventType, result.EventType);
            Assert.Equal(eventStoreEntity.CreateAt, result.CreateAt);
            Assert.Equal(eventStoreEntity.PayLoad, result.PayLoad);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnDomainEvents()
        {
            //Arrange
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync().ConfigureAwait(false);

            var options = new DbContextOptionsBuilder<EventStoreDbContext>()
                .UseSqlite(connection)
                .Options;

            var storeDbContext = new EventStoreDbContext(options);
            await storeDbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);

            var eventSerializer = new JsonEventSerializer(new JsonSerializer());
            var uow = new EventStoreUnitOfWork(storeDbContext);

            IEventStore sqlEventStore = new SqlEventStore(storeDbContext,
                new DomainEventFactory(eventSerializer), eventSerializer, uow, new EventSourcingOptions());

            var aggregateId = Id.New<string>();
            var aggregateTest = new PersonAggregateTest(aggregateId, "Dupont", "45 av charles degaulle paris, france");
            await aggregateTest.CommitAsync(sqlEventStore).ConfigureAwait(false);

            //Act
            PersonAggregateTest personAggregateTest = new(aggregateId); 
            await personAggregateTest.ApplyFromHistoryAsync(sqlEventStore).ConfigureAwait(false);

            uow.Dispose();
            sqlEventStore.Dispose();

            // Assert
            Assert.NotNull(personAggregateTest);
            personAggregateTest.Should().BeEquivalentTo(aggregateTest);
        }
    }
}