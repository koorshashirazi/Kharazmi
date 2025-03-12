using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Localization.EFCore
{
    public class EfStringLocalizerFactory<TContext> : IStringLocalizerFactory
        where TContext : DbContext
    {
        private readonly IServiceProvider _resolver;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly LocalizationOptions _options;

        private readonly ConcurrentDictionary<string, EfMemoryCacheStringLocalizer<TContext>> _localizerMemoryCache =
            new ConcurrentDictionary<string, EfMemoryCacheStringLocalizer<TContext>>();

        private readonly ConcurrentDictionary<string, EfDistributedCacheStringLocalizer<TContext>>
            _localizerDistributedCache =
                new ConcurrentDictionary<string, EfDistributedCacheStringLocalizer<TContext>>();

        public EfStringLocalizerFactory(
            IServiceProvider resolver,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            IOptions<LocalizationOptions> option)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _options = option.Value;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var resourceType = resourceSource.GetTypeInfo();
            var cultureInfo = CultureInfo.CurrentUICulture;
            var resourceName = resourceSource.GetTypeName();
            var assembly = resourceType.Assembly;
            var cacheKey = GetCacheKey(resourceName, assembly, cultureInfo);


            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                if (_localizerMemoryCache.TryGetValue(cacheKey, out var instance))
                {
                    return instance;
                }

                return _localizerMemoryCache.GetOrAdd(cacheKey,
                    new EfMemoryCacheStringLocalizer<TContext>(resourceName, _resolver, _memoryCache));
            }

            if (_localizerDistributedCache.TryGetValue(cacheKey, out var instanse))
            {
                return instanse;
            }

            return _localizerDistributedCache.GetOrAdd(cacheKey,
                new EfDistributedCacheStringLocalizer<TContext>(resourceName, _resolver, _distributedCache));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var cultureInfo = CultureInfo.CurrentUICulture;
            var resourceName = Helpers.TrimPrefix(baseName, location + ".");
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            
            var assembly = Helpers.GetAssemblyFromLocation(location);
            var cacheKey = GetCacheKey(resourceName, assembly, cultureInfo);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                if (_localizerMemoryCache.TryGetValue(cacheKey, out var instance))
                {
                    return instance;
                }

                return _localizerMemoryCache.GetOrAdd(cacheKey,
                    new EfMemoryCacheStringLocalizer<TContext>(resourceName, _resolver, _memoryCache));
            }

            if (_localizerDistributedCache.TryGetValue(cacheKey, out var instanse))
            {
                return instanse;
            }

            return _localizerDistributedCache.GetOrAdd(cacheKey,
                new EfDistributedCacheStringLocalizer<TContext>(resourceName, _resolver, _distributedCache));
        }

        protected virtual string GetCacheKey(string resourceName, Assembly assembly, CultureInfo cultureInfo)
        {
            return resourceName + ';' + assembly.FullName + ';' + cultureInfo.Name;
        }
    }
}