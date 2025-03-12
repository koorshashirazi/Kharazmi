using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Kharazmi.AspNetCore.Localization.Xml.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Localization.Xml
{
    public class XmlMemoryCacheStringLocalizer : BaseStringLocalization
    {
        private readonly IMemoryCache _cache;

        private readonly IServiceProvider _resolver;
        private readonly string _resourceName;
        private readonly Assembly _resourceAssembly;
        private readonly LocalizationOptions _options;
        private string _searchedLocation;
        
        public XmlMemoryCacheStringLocalizer(
            Assembly resourceAssembly,
            string resourceName,
            IServiceProvider resolver,
            IMemoryCache cache,
            LocalizationOptions options) : base("", resourceName)
        {
            _resolver = resolver ?? throw new ArgumentException(nameof(resolver));
            _resourceName = resourceName;
            _cache = cache ?? throw new ArgumentException(nameof(cache));
            _resourceAssembly = resourceAssembly;
            _options = options;
            BuildResourcesCache();
        }

        private void BuildResourcesCache()
        {
            _searchedLocation = GetComputedPath(Culture.Name);
            var records = XmlReader.Read<List<LocalizationEntity>>(_searchedLocation);
            var loggerFactory = _resolver.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<XmlDistributedCacheStringLocalizer>();
            logger.LogInformation($"Read XMl File from : {_searchedLocation}");
            Parallel.ForEach(records, record => { _cache.Set(record.Resource, record.Value); });
            logger.LogInformation($"Cache records from XML file");
            logger.LogInformation($"Cache Provider: {nameof(IMemoryCache)}");
        }

        protected override string GetComputedKey(string name)
        {
            return _cache.Get(base.GetComputedKey(name))?.ToString() ?? name;
        }

        protected override string GetComputedPath(string culture)
        {
            var resourcesPath = _resourceAssembly.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, ResourceName, culture);
            return computedPath;
        }
    }
}