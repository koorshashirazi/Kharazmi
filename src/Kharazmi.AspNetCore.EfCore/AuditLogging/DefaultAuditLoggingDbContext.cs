using System.Linq;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.EFCore.Context;
using Kharazmi.AspNetCore.EFCore.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Kharazmi.AspNetCore.EFCore.AuditLogging
{
    public interface IAuditLoggingDbContext<TAuditLog> where TAuditLog : AuditLog
    {
        DbSet<TAuditLog> AuditLog { get; set; }
    }


    public class DefaultAuditLoggingDbContext : DbContextCore, IUnitOfWork<DefaultAuditLoggingDbContext>, IAuditLoggingDbContext<AuditLog>
    {
        public DefaultAuditLoggingDbContext(
            DbContextOptions<DefaultAuditLoggingDbContext> dbContextOptions)
            : base(dbContextOptions, Enumerable.Empty<IHook>())
        {
        }

        public DbSet<AuditLog> AuditLog { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var auditLoggerOptions = this.GetService<AuditLoggerOptions>();
            modelBuilder.ApplyAuditLogConfiguration(auditLoggerOptions?.TableOption);
        }
    }
}