using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditLoggingBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        IServiceCollection Services { get; }
    }
}