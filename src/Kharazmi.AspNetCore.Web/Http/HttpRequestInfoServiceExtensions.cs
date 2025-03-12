using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary>
    /// Http Request Info Extensions
    /// </summary>
    public static class HttpRequestInfoServiceExtensions
    {
        /// <summary>
        /// Adds IHttpContextAccessor, IActionContextAccessor, IUrlHelper and IHttpRequestInfoService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddHttpRequestInfoService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Allows injecting IUrlHelper as a dependency
            services.AddScoped(serviceProvider =>
            {
                var actionContext = serviceProvider.GetService<IActionContextAccessor>().ActionContext;
                var urlHelperFactory = serviceProvider.GetService<IUrlHelperFactory>();
                return urlHelperFactory?.GetUrlHelper(actionContext);
            });
            services.AddScoped<IHttpRequestInfoService, HttpRequestInfoService>();
            return services;
        }
    }
}