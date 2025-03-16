using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.EfCore.XUnitTests.EnumConverter
{
    public sealed class DbContextTest(DbContextOptions<DbContextTest> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>()
                .HasKey(x => x.Id);

            // From number to ExpirationType
            modelBuilder.Entity<Employee>()
                .Property(x => x.ExpirationType)
                .HasConversion(x => x.Value, x => ExpirationType.FromValue(x));
        }
    }
}