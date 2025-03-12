using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class AuditLoggingBuilder : IAuditLoggingBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public AuditLoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// 
        /// </summary>
        public IServiceCollection Services { get; }
    }
}