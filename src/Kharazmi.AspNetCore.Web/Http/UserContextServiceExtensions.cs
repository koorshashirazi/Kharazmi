using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary>
    /// 
    /// </summary>
    internal static class UserContextServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserContextService(this IServiceCollection services)
        {
            services.AddScoped<IUserContextService, UserContextService>();
            return services;
        }
    }
}