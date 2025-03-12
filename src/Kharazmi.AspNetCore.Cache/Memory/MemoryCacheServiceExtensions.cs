using Kharazmi.AspNetCore.Core.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Cache.Memory
{
    /// <summary>
    /// Memory Cache Service Extensions
    /// </summary>
    internal static class MemoryCacheServiceExtensions
    {
        /// <summary>
        /// Adds ICacheService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddMemoryCacheService(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<ICacheService, MemoryCacheService>();
            return services;
        }
    }
}