using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.Core.XUnitTest.EnumConverter
{
    public class DbContextTest : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public DbContextTest(DbContextOptions<DbContextTest> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>()
                .HasKey(x => x.Id);
            
            // From number to ExpirationType
            modelBuilder.Entity<Employee>()
                .Property(x => x.ExpirationType)
                .HasConversion(x => x.Value,
                    x => ExpirationType.FromValue(x));
        }
    }
}