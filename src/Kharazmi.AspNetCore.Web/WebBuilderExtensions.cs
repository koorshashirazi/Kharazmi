using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static WebBuilder AddWebApp(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return new WebBuilder(services);
        }
        
       
    }
}