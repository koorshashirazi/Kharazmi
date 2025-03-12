using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Security
{
    /// <summary>
    /// 
    /// </summary>
    public static class SecurityBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SecurityBuilder AddSecurityFrameWork(this IServiceCollection services)
        {
            return new SecurityBuilder(services);
        }
    }
}