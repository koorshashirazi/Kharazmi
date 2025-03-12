using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using JsonReader = Kharazmi.AspNetCore.Localization.Json.Internal.JsonReader;

namespace Kharazmi.AspNetCore.Localization.Json
{
    public class JsonMemoryCacheStringLocalizer : BaseStringLocalization
    {
        private readonly IMemoryCache _cache;
        private readonly IServiceProvider _resolver;
        private readonly Assembly _resourceAssembly;
        private readonly LocalizationOptions _options;
        private string _searchedLocation;

        public JsonMemoryCacheStringLocalizer(
            Assembly resourceAssembly,
            string resourceName,
            IServiceProvider resolver,
            IMemoryCache cache,
            LocalizationOptions options) : base("", resourceName)
        {
            _cache = cache ?? throw new ArgumentException(nameof(cache));
            _resourceAssembly = resourceAssembly;
            _resolver = resolver;
            _options = options;

//            ReadResources(resourceName, resourceAssembly, cultureInfo, logger, false);
            BuildResourcesCache();
        }

        private void BuildResourcesCache()
        {
            _searchedLocation = GetComputedPath(Culture.Name);
            var records = JsonReader.Read<List<LocalizationEntity>>(_searchedLocation);
            
            var loggerFactory = _resolver.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<JsonDistributedCacheStringLocalizer>();
            logger.LogInformation($"Read Json File from : {_searchedLocation}");
            
            Parallel.ForEach(records, record => { _cache.Set(record.Resource, record.Value); });
            
            logger.LogInformation($"Cache records from Json file");
            logger.LogInformation($"Cache Provider: {nameof(IMemoryCache)}");
        }

        private void ReadResources(string resourceName, Assembly resourceAssembly, CultureInfo cultureInfo,
            ILogger<JsonMemoryCacheStringLocalizer> logger, bool isFallback)
        {
            var entities = Helpers.ReadResources(resourceName, resourceAssembly, cultureInfo, logger, false);
            Parallel.ForEach(entities, record => { _cache.Set(record.Resource, record.Value); });
        }

        protected override string GetComputedKey(string name)
        {
            var culture = Culture;
            string value = null;

            while (!culture.Equals(culture.Parent))
            {
                var key = base.GetComputedKey(name);
                var hasValue = _cache.TryGetValue(key, out value);

                if (hasValue)
                    break;

                culture = culture.Parent;
            }

            return value;
        }


        protected override string GetComputedPath(string culture)
        {
            var resourcesPath = _resourceAssembly.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, ResourceName, culture);
            return computedPath;
        }

        private IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            var resourceNames = includeParentCultures
                ? GetAllStringsFromCultureHierarchy(culture)
                : GetAllResourceStrings(culture);

            foreach (var name in resourceNames)
            {
                var value = GetComputedKey(name);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null,
                    searchedLocation: _searchedLocation);
            }
        }

        private IEnumerable<string> GetAllStringsFromCultureHierarchy(CultureInfo startingCulture)
        {
            var currentCulture = startingCulture;
            var resourceNames = new HashSet<string>();

            while (!currentCulture.Equals(currentCulture.Parent))
            {
                var cultureResourceNames = GetAllResourceStrings(currentCulture);

                if (cultureResourceNames != null)
                {
                    foreach (var resourceName in cultureResourceNames)
                    {
                        resourceNames.Add(resourceName);
                    }
                }

                currentCulture = currentCulture.Parent;
            }

            return resourceNames;
        }

        private IEnumerable<string> GetAllResourceStrings(CultureInfo culture)
        {
            if (_cache.TryGetValue(culture.Name, out IEnumerable<KeyValuePair<string, string>> resources))
            {
                foreach (var resource in resources)
                {
                    yield return resource.Key;
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}