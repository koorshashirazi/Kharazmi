using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.EFCore.AuditLogging
{
    public class DefaultAuditEventLoggerSink<TDbContext, TAuditLog> : IAuditEventLoggerSink
        where TDbContext : DbContext, IUnitOfWork<TDbContext>, IAuditLoggingDbContext<TAuditLog>
        where TAuditLog : AuditLog, new()

    {
        private readonly IAuditLoggingRepository<TDbContext, TAuditLog> _auditLoggingRepository;

        public DefaultAuditEventLoggerSink(
            IAuditLoggingRepository<TDbContext, TAuditLog> auditLoggingRepository)
        {
            _auditLoggingRepository = auditLoggingRepository;
            _auditLoggingRepository.CheckArgumentIsNull(nameof(_auditLoggingRepository));
        }

        public virtual async Task PersistAsync(AuditEvent auditEvent)
        {
            if (auditEvent == null)
                throw new ArgumentNullException(nameof(auditEvent));
            await _auditLoggingRepository.SaveAsync(auditEvent.MapToEntity<TAuditLog>()).ConfigureAwait(false);
        }
    }
}