using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.Test.Specs
{
    [CollectionDefinition(nameof(DatabaseContextSpecs), DisableParallelization = true)]
    public class DatabaseContextSpecsCollection
    {
    }

    [Collection(nameof(DatabaseContextSpecs))]
    public class DatabaseContextSpecs
    {
        [Fact(DisplayName = "Verify that DatabaseContext Shoud Be  instanciable and disposable and DbSet is not null")]
        public void Verify_that_DatabaseContext_Shoud_Be_instanciable_and_disposable_and_DbSet_is_not_null()
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();

            optionsBuilder.UseSqlite(new SqliteConnection("DataSource=:memory:"));
            var context = new EventStoreDbContext(optionsBuilder.Options);

            //Act
            context.Database.EnsureCreated();
            context.Dispose();

            //Assert
            Assert.NotNull(context.EventStores);
        }
    }
}