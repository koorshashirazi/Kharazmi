using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Localization.Json
{
    public class JsonCacheStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IServiceProvider _resolver;
        private readonly LocalizationOptions _options;

        private readonly ConcurrentDictionary<string, JsonMemoryCacheStringLocalizer> _localizerMemoryCache =
            new ConcurrentDictionary<string, JsonMemoryCacheStringLocalizer>();

        private readonly ConcurrentDictionary<string, JsonDistributedCacheStringLocalizer> _localizerDistributedCache =
            new ConcurrentDictionary<string, JsonDistributedCacheStringLocalizer>();

        public JsonCacheStringLocalizerFactory(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            IOptions<LocalizationOptions> option,
            IServiceProvider resolver)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _resolver = resolver;
            _options = option.Value;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            Ensure.IsNotNull(resourceSource,nameof(resourceSource));

            var resourceType = resourceSource.GetTypeInfo();
            var cultureInfo = CultureInfo.CurrentUICulture;
//            var resourceName = $"{resourceType.FullName}.json";

            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);

            var assembly = resourceType.Assembly;
            var cacheKey = GetCacheKey(resourceName, assembly, cultureInfo);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                return _localizerMemoryCache.TryGetValue(cacheKey,
                    out var instance)
                    ? instance
                    : _localizerMemoryCache.GetOrAdd(cacheKey,
                        new JsonMemoryCacheStringLocalizer(assembly, resourceName, _resolver, _memoryCache, _options));
            }

            return _localizerDistributedCache.TryGetValue(cacheKey,
                out var instance2)
                ? instance2
                : _localizerDistributedCache.GetOrAdd(cacheKey,
                    new JsonDistributedCacheStringLocalizer(assembly, resourceName, _resolver, _distributedCache,
                        _options));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName.IsEmpty())
                throw new ArgumentNullException(nameof(baseName));

            if (location.IsEmpty())
                throw new ArgumentNullException(nameof(location));

            var cultureInfo = CultureInfo.CurrentUICulture;

            var resourceName = Helpers.TrimPrefix(baseName, location + ".");
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var assembly = Helpers.GetAssemblyFromLocation(location);
            var cacheKey = GetCacheKey(resourceName, assembly, cultureInfo);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                if (_localizerMemoryCache.TryGetValue(cacheKey, out var instance))
                    return instance;

                return _localizerMemoryCache.GetOrAdd(cacheKey,
                    new JsonMemoryCacheStringLocalizer(assembly, resourceName, _resolver, _memoryCache, _options));
            }

            if (_localizerDistributedCache.TryGetValue(cacheKey, out var instance2))
                return instance2;

            return _localizerDistributedCache.GetOrAdd(cacheKey,
                new JsonDistributedCacheStringLocalizer(assembly, resourceName, _resolver, _distributedCache,
                    _options));
        }

        protected virtual string GetCacheKey(string resourceName, Assembly assembly, CultureInfo cultureInfo)
        {
            return resourceName + ';' + assembly.FullName + ';' + cultureInfo.Name;
        }
    }
}