using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonReader = Kharazmi.AspNetCore.Localization.Json.Internal.JsonReader;

namespace Kharazmi.AspNetCore.Localization.Json
{
    public class JsonDistributedCacheStringLocalizer : BaseStringLocalization
    {
        private readonly IDistributedCache _cache;

        private readonly Assembly _resourceAssembly;
        private readonly IServiceProvider _resolver;
        private string _searchedLocation;
        private readonly LocalizationOptions _options;
        public JsonDistributedCacheStringLocalizer(
            Assembly resourceAssembly,
            string resourceName,
            IServiceProvider resolver,
            IDistributedCache cache,
            LocalizationOptions options ) : base("", resourceName)
        {
            _resourceAssembly = resourceAssembly;
            _options = options;
            _resolver = resolver;
            _cache = Ensure.IsNotNullWithDetails(cache, nameof(cache));
            BuildResourcesCache();
//            ReadResources(resourceName, resourceAssembly, cultureInfo, logger, false);
        }

        private void BuildResourcesCache()
        {
            _searchedLocation = GetComputedPath(Culture.Name);
            var records = JsonReader.Read<List<LocalizationEntity>>(_searchedLocation);
            var loggerFactory = _resolver.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<JsonDistributedCacheStringLocalizer>();
            logger.LogInformation($"Read Json File from : {_searchedLocation}");
            Parallel.ForEach(records, record => { _cache.SetString(record.Resource, record.Value); });
            logger.LogInformation($"Cache records from Json file");
            logger.LogInformation($"Cache Provider: {nameof(IDistributedCache)}");
        }
        
        private void ReadResources(string resourceName, Assembly resourceAssembly, CultureInfo cultureInfo,
            ILogger<JsonDistributedCacheStringLocalizer> logger, bool isFallback)
        {
            Assembly satelliteAssembly;
            try
            {
                satelliteAssembly = resourceAssembly.GetSatelliteAssembly(cultureInfo);
            }
            catch (FileNotFoundException x)
            {
                logger.LogInformation(
                    $"Could not find satellite assembly for {(isFallback ? "fallback " : "")}'{cultureInfo.Name}': {x.Message}");
                return;
            }

            var stream = satelliteAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                logger.LogInformation(
                    $"Resource '{resourceName}' not found for {(isFallback ? "fallback " : "")}'{cultureInfo.Name}'.");
                return;
            }

            string json;
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }

            var entities = JsonConvert.DeserializeObject<List<LocalizationEntity>>(json);
            Parallel.ForEach(entities, record => { _cache.SetString(record.Resource, record.Value); });
        }

        protected override string GetComputedKey(string name)
        {
            var culture = Culture;
            string key = null;

            while (!culture.Equals(culture.Parent))
            {
                 key = base.GetComputedKey(name);

                if (key.IsNotEmpty())
                    break;

                culture = culture.Parent;
            }

            return _cache.GetString(base.GetComputedKey(key)) ?? name;
        }
        
        protected override string GetComputedPath(string culture)
        {
            var resourcesPath = _resourceAssembly.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, ResourceName, culture);
            return computedPath;
        }
    }
}