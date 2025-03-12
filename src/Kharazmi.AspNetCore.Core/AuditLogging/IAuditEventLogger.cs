using System;
using System.Threading.Tasks;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditEventLogger : IAuditEventLogger<AuditLoggerOptions>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAuditLoggerOptions"></typeparam>
    public interface IAuditEventLogger<out TAuditLoggerOptions> where TAuditLoggerOptions : AuditLoggerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="loggerOptions"></param>
        /// <returns></returns>
        Task LogEventAsync(AuditEvent auditEvent, Action<TAuditLoggerOptions> loggerOptions = null);
    }
}