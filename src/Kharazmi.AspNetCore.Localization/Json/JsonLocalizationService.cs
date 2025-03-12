using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;
using JsonReader = Kharazmi.AspNetCore.Localization.Json.Internal.JsonReader;

namespace Kharazmi.AspNetCore.Localization.Json
{
    public class JsonLocalizationService : ILocalizerService
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly LocalizationOptions _options;

        public JsonLocalizationService(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILoggerFactory loggerFactory,
            IOptions<LocalizationOptions> options)
        {
            _logger = loggerFactory.CreateLogger<JsonLocalizationService>();
            _options = options.Value;

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                _memoryCache = Ensure.IsNotNullWithDetails(memoryCache, nameof(memoryCache));
            }
            else
            {
                _distributedCache = Ensure.IsNotNullWithDetails(distributedCache, nameof(distributedCache));
            }
        }

        public void Delete(string resourceKey, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);
            
            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);
            _logger.LogInformation($"Read file from {computedPath}");

            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);

            var entity = records.FirstOrDefault(a => a.Key == resourceKey &&
                                                     a.CultureName == cultureName &&
                                                     a.Resource == computedKey);

            if (entity != null)
            {
                records.Remove(records.FirstOrDefault(a => a.Key == resourceKey &&
                                                           a.CultureName == cultureName &&
                                                           a.Resource == computedKey));

//                Helpers.WriteToResources(resourceName, resourceType.Assembly, cultureInfo,
//                    _loggerFactory.CreateLogger<JsonLocalizationService>(), records);

                JsonReader.Write(records, computedPath);

                if (_options.CacheDependency == CacheOption.MemoryCache)
                {
                    _memoryCache.Remove(computedKey);
                }
                else
                {
                    _distributedCache.Remove(computedKey);
                }

                _logger.LogInformation($"Removed: {entity}");
            }
            else
            {
                _logger.LogInformation($"No record was found with this key : {resourceKey}");
            }
        }

        public void Delete(IEnumerable<string> resourceKeys, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);

            _logger.LogInformation($"Read file from {computedPath}");
            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);
//            var records = Helpers.ReadResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), false

            List<string> isSuccess = new List<string>();
            foreach (string resourceKey in resourceKeys)
            {
                var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);

                LocalizationEntity entity = records.FirstOrDefault(a => a.Key == resourceKey &&
                                                                        a.CultureName == cultureName &&
                                                                        a.Resource == computedKey);

                if (entity != null)
                {
                    records.Remove(records.FirstOrDefault(a => a.Key == resourceKey &&
                                                               a.CultureName == cultureName &&
                                                               a.Resource == computedKey));

                    isSuccess.Add(computedKey);
                    _logger.LogInformation($"Removed: {entity}");
                }
                else
                {
                    _logger.LogInformation($"No record was found with this key : {resourceKey}");
                }
            }


//            Helpers.WriteToResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), records);

            JsonReader.Write(records, computedPath);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                foreach (string item in isSuccess)
                {
                    _memoryCache.Remove(item);
                }
            }
            else
            {
                foreach (string item in isSuccess)
                {
                    _distributedCache.Remove(item);
                }
            }
        }

        public string ExportJson(string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);

//            var records = Helpers.ReadResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), false);

            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);
            _logger.LogInformation($"Read file from {computedPath}");
            var output = JsonConvert.SerializeObject(records);
            return output;
        }

        public string ExportXml(string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);

//
//            var records = Helpers.ReadResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), false);


            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);
            _logger.LogInformation($"Read file from {computedPath}");
            var xmlSerializer = new XmlSerializer(typeof(List<LocalizationEntity>));
            var memoryStream = new MemoryStream();
            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            xmlSerializer.Serialize(xmlTextWriter, records);

            var output = Encoding.UTF8.GetString(memoryStream.ToArray());
            var byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (output.StartsWith(byteOrderMarkUtf8, StringComparison.Ordinal))
            {
                output = output.Remove(0, byteOrderMarkUtf8.Length);
            }

            return output;
        }


        public void Insert(string resourceKey, string value, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);

            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);

            _logger.LogInformation($"Read file from {computedPath}");

            //            var records = Helpers.ReadResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), false);

            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);

            var entity = records.FirstOrDefault(a => a.Key == resourceKey &&
                                                     a.CultureName == cultureName &&
                                                     a.Resource == computedKey);

            if (entity == null)
            {
                entity = new LocalizationEntity
                {
                    Key = resourceKey,
                    Value = value,
                    CultureName = cultureName,
                    Resource = computedKey,
                };

                records.Add(entity);

//                JsonReader.Write(records, computedPath);
//                Helpers.WriteToResources(resourceName, resourceType.Assembly, cultureInfo,
//                    _loggerFactory.CreateLogger<JsonLocalizationService>(), records);

                JsonReader.Write(records, computedPath);

                if (_options.CacheDependency == CacheOption.MemoryCache)
                {
                    _memoryCache.Set(computedKey, value);
                }
                else
                {
                    _distributedCache.SetString(computedKey, value);
                }

                _logger.LogInformation($"Added: {entity}");
            }
            else
            {
                Update(resourceKey, value, cultureName, resourceSource);
            }
        }


        public void Insert(IEnumerable<KeyValuePair<string, string>> keyValue, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);

//            var records = Helpers.ReadResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), false);

            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);
            _logger.LogInformation($"Read file from {computedPath}");
            var isSuccess = new List<KeyValuePair<string, string>>();
            foreach (var item in keyValue)
            {
                var computedKey = Helpers.GetComputedCacheKey(resourceName, item.Key, cultureName);

                var entity = records.FirstOrDefault(a => a.Key == item.Key &&
                                                         a.CultureName == cultureName &&
                                                         a.Resource == computedKey);

                if (entity != null)
                {
                    _logger.LogInformation($"No record was found with this key : {item.Key}");
                    continue;
                }

                entity = new LocalizationEntity
                {
                    Key = item.Key,
                    Value = item.Value,
                    CultureName = cultureName,
                    Resource = computedKey,
                };

                records.Add(entity);
                isSuccess.Add(new KeyValuePair<string, string>(computedKey, item.Value));
                _logger.LogInformation($"Added: {entity}");
            }
//
//            Helpers.WriteToResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), records);


            JsonReader.Write(records, computedPath);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                foreach (KeyValuePair<string, string> item in isSuccess)
                {
                    _memoryCache.Set(item.Key, item.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> item in isSuccess)
                {
                    _distributedCache.SetString(item.Key, item.Value);
                }
            }
        }

        public void Update(string resourceKey, string value, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);

            _logger.LogInformation($"Read file from {computedPath}");
            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);
//            var records = Helpers.ReadResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), false);
            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);


            var entity = records.FirstOrDefault(a => a.Key == resourceKey &&
                                                     a.CultureName == cultureName &&
                                                     a.Resource == computedKey);

            if (entity == null)
            {
                _logger.LogInformation($"No record was found with this key : {resourceKey}");
                return;
            }

            records.Where(a => a.Key == resourceKey &&
                               a.CultureName == cultureName &&
                               a.Resource == computedKey).ToList().ForEach(x => x.Value = value);

//            Helpers.WriteToResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), records);

            JsonReader.Write(records, computedPath);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                _memoryCache.Set(computedKey, value);
            }
            else
            {
                _distributedCache.SetString(computedKey, value);
            }

            _logger.LogInformation($"Update: {entity}");
        }

        public void Update(IEnumerable<KeyValuePair<string, string>> resourceKeys, string cultureName,
            Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var resourcesPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            var computedPath = Helpers.GetComputedResourceFile(resourcesPath, resourceName, cultureName);

            var records = JsonReader.Read<List<LocalizationEntity>>(computedPath);
            _logger.LogInformation($"Read file from {computedPath}");
//
//            var records = Helpers.ReadResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), false);


            var isSuccess = new List<KeyValuePair<string, string>>();
            foreach (var item in resourceKeys)
            {
                var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
                    CultureInfo.CurrentUICulture.Name, resourceName, item.Key);

                var entity = records.FirstOrDefault(a => a.Key == item.Key &&
                                                         a.CultureName == cultureName &&
                                                         a.Resource == computedKey);

                if (entity == null)
                {
                    _logger.LogInformation($"No record was found with this key : {item.Key}");
                    continue;
                }

                records.Where(a => a.Key == item.Key &&
                                   a.CultureName == cultureName &&
                                   a.Resource == computedKey)
                    .ToList()
                    .ForEach(x => x.Value = item.Value);

                isSuccess.Add(new KeyValuePair<string, string>(computedKey, item.Value));
                _logger.LogInformation($"Update: {entity}");
            }
//
//            Helpers.WriteToResources(resourceName, resourceType.Assembly, cultureInfo,
//                _loggerFactory.CreateLogger<JsonLocalizationService>(), records);

            JsonReader.Write(records, computedPath);

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                foreach (KeyValuePair<string, string> item in isSuccess)
                {
                    _memoryCache.Set(item.Key, item.Value);
                }
            }
            else
            {
                foreach (var item in isSuccess)
                {
                    _distributedCache.SetString(item.Key, item.Value);
                }
            }
        }
    }
}