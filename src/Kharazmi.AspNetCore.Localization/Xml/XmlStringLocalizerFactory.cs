﻿using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Localization.Xml
{
    public class XmlStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IServiceProvider _resolver;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly LocalizationOptions _options;

        private readonly ConcurrentDictionary<string, XmlMemoryCacheStringLocalizer> _localizerMemoryCache =
            new ConcurrentDictionary<string, XmlMemoryCacheStringLocalizer>();

        private readonly ConcurrentDictionary<string, XmlDistributedCacheStringLocalizer> _localizerDistributedCache =
            new ConcurrentDictionary<string, XmlDistributedCacheStringLocalizer>();

        public XmlStringLocalizerFactory(
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
           Ensure.IsNotNull(resourceSource, nameof(resourceSource));

            var resourceType = resourceSource.GetTypeInfo();
            var cultureInfo = CultureInfo.CurrentUICulture;

            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);

            var assembly = resourceType.Assembly;
            var cacheKey = GetCacheKey(resourceName, assembly, cultureInfo);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                if (_localizerMemoryCache.TryGetValue(cacheKey,
                    out XmlMemoryCacheStringLocalizer instance))
                {
                    return instance;
                }

                return _localizerMemoryCache.GetOrAdd(cacheKey,
                    new XmlMemoryCacheStringLocalizer(assembly, resourceName, _resolver, _memoryCache, _options));
            }

            if (_localizerDistributedCache.TryGetValue(cacheKey,
                out XmlDistributedCacheStringLocalizer instanse))
            {
                return instanse;
            }

            return _localizerDistributedCache.GetOrAdd(cacheKey,
                new XmlDistributedCacheStringLocalizer(assembly, resourceName, _resolver, _distributedCache, _options));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName.IsEmpty())
                throw new ArgumentNullException(nameof(baseName));

            if (location.IsEmpty())
                throw new ArgumentNullException(nameof(location));

            var cultureInfo = CultureInfo.CurrentUICulture;
//            var resourceName = $"{baseName}.json";

            var resourceName = Helpers.TrimPrefix(baseName, location + ".");
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var assembly = Helpers.GetAssemblyFromLocation(location);
            var cacheKey = GetCacheKey(resourceName, assembly, cultureInfo);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                if (_localizerMemoryCache.TryGetValue(cacheKey, out XmlMemoryCacheStringLocalizer instance))
                {
                    return instance;
                }

                return _localizerMemoryCache.GetOrAdd(cacheKey,
                    new XmlMemoryCacheStringLocalizer(assembly, resourceName, _resolver, _memoryCache, _options));
            }

            if (_localizerDistributedCache.TryGetValue(cacheKey, out XmlDistributedCacheStringLocalizer instanse))
            {
                return instanse;
            }

            return _localizerDistributedCache.GetOrAdd(cacheKey,
                new XmlDistributedCacheStringLocalizer(assembly, resourceName, _resolver, _distributedCache, _options));
        }


        protected virtual string GetCacheKey(string resourceName, Assembly assembly, CultureInfo cultureInfo)
        {
            return resourceName + ';' + assembly.FullName + ';' + cultureInfo.Name;
        }
    }
}