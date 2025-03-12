using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary>
    /// CommonHttpClientFactory Extensions
    /// </summary>
    public static class CommonHttpClientFactoryExtensions
    {
        /// <summary>
        /// Adds ICommonHttpClientFactory to the IServiceCollection
        /// </summary>
        public static IServiceCollection AddCommonHttpClientFactory(this IServiceCollection services)
        {
            services.TryAddSingleton<ICommonHttpClientFactory, CommonHttpClientFactory>();
            return services;
        }
    }
}