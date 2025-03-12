using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.Test.Specs
{
    [CollectionDefinition(nameof(UnitOfWorkSpecs), DisableParallelization = true)]
    public class UnitOfWorkSpecsCollection
    {
    }

    [Collection(nameof(UnitOfWorkSpecs))]
    public class UnitOfWorkSpecs
    {
        [Fact(DisplayName =
            "When saving IUnitOfWork.Commit Should Save Aggregate Root and DbContext.SaveChanges called only once")]
        public async Task Commit()
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();
            optionsBuilder.UseSqlite(new SqliteConnection("DataSource=:memory:"));

            var context = new Mock<EventStoreDbContext>(() => new EventStoreDbContext(optionsBuilder.Options));
            context.Setup(c => c.SaveChanges()).Returns(1).Verifiable();

            //Act
            EventStoreUnitOfWork eventStoreUnitOfWork = new(context.Object);
            await eventStoreUnitOfWork.CommitAsync().ConfigureAwait(false);

            //Assert
            context.Verify(m => m.SaveChangesAsync(default), Times.Once, "SaveChanges should be called only once");
        }

        [Fact(DisplayName = "When disposing unitOfWork.Dispose should be called only once")]
        public void Dispose()
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();
            optionsBuilder.UseSqlite(new SqliteConnection("DataSource=:memory:"));

            var context = new Mock<EventStoreDbContext>(() => new EventStoreDbContext(optionsBuilder.Options));
            context.Setup(c => c.Dispose()).Verifiable();

            //Act
            EventStoreUnitOfWork eventStoreUnitOfWork = new(context.Object);
            eventStoreUnitOfWork.Dispose();

            //Assert
            context.Verify(m => m.Dispose(), Times.Once, "Dispose must be called only once");
        }
    }
}