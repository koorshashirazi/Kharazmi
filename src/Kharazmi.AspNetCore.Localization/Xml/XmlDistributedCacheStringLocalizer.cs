using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Kharazmi.AspNetCore.Localization.Xml.IO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Localization.Xml
{
    public class XmlDistributedCacheStringLocalizer : BaseStringLocalization
    {        
        private readonly IDistributedCache _cache;

        private readonly IServiceProvider _resolver;
        private readonly Assembly _resourceAssembly;
        private readonly LocalizationOptions _options;
        private string _searchedLocation;

        public XmlDistributedCacheStringLocalizer(
            Assembly resourceAssembly,
            string resourceName,
            IServiceProvider resolver,
            IDistributedCache cache, 
            LocalizationOptions options) : base("", resourceName)
        {
            _resolver = resolver ?? throw new ArgumentException(nameof(resolver));
            _cache = cache ?? throw new ArgumentException(nameof(cache));
            _options = options;
            _resourceAssembly = resourceAssembly;
            BuildResourcesCache();
        }

        private void BuildResourcesCache()
        {
            _searchedLocation = GetComputedPath(Culture.Name);
            var records = XmlReader.Read<List<LocalizationEntity>>(_searchedLocation);
            var loggerFactory = _resolver.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<XmlDistributedCacheStringLocalizer>();
            logger.LogInformation($"Read XMl File from : {_searchedLocation}");
            Parallel.ForEach(records, (record) => { _cache.SetString(record.Resource, record.Value); });
            logger.LogInformation($"Cache records from XML file");
            logger.LogInformation($"Cache Provider: {nameof(IDistributedCache)}");
        }

        protected override string GetComputedKey(string name)
        {
            return _cache.GetString(base.GetComputedKey(name)) ?? name;
        }

        protected override string GetComputedPath(string culture)
        {
            var resourcesPath = _resourceAssembly.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, ResourceName, culture);
            return computedPath;
        }
    }
}