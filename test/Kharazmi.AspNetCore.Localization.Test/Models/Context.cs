using Kharazmi.AspNetCore.Localization.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.Localization.Test.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyLocalizationRecordConfiguration();
        }
    }
}
