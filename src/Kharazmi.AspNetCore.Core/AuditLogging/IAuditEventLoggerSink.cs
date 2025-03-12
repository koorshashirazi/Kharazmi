using System.Threading.Tasks;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditEventLoggerSink
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <returns></returns>
        Task PersistAsync(AuditEvent auditEvent);
    }
}