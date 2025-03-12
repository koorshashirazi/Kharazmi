using Kharazmi.AspNetCore.Cache.Memory;
using Kharazmi.AspNetCore.Core;

namespace Kharazmi.AspNetCore.Cache
{
    public static class CoreFrameworkBuilderExtensions
    {
        /// <summary>
        /// Register the ICacheService
        /// </summary>
        public static CoreFrameworkBuilder WithMemoryCache(this CoreFrameworkBuilder builder)
        {
            builder.Services.AddMemoryCacheService();
            return builder;
        }
    }
}