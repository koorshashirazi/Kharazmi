using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs
{
    [CollectionDefinition(nameof(DatabaseContextSpecs), DisableParallelization = true)]
    public class DatabaseContextSpecsCollection
    {
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Collection(nameof(DatabaseContextSpecs))]
    public class DatabaseContextSpecs
    {
        [Fact(DisplayName = "Verify that DatabaseContext Shoud Be  instanciable and disposable and DbSet is not null")]
        public void Verify_that_DatabaseContext_Shoud_Be_instanciable_and_disposable_and_DbSet_is_not_null()
        {
            //Arrange
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            
            var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();
            optionsBuilder.UseSqlite(connection);
            
            var context = new EventStoreDbContext(optionsBuilder.Options);

            //Act
            context.Database.EnsureCreated();
            context.Dispose();

            //Assert
            Assert.NotNull(context.EventStores);
        }
    }
}