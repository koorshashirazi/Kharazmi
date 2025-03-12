using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Kharazmi.AspNetCore.Localization.Xml.IO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Localization.Xml
{
    public class XmlLocalizationService : ILocalizerService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger _logger;
        

        private readonly LocalizationOptions _options;

        public XmlLocalizationService(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            IOptions<LocalizationOptions> options,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<XmlLocalizationService>();
            _options = options.Value;

            if (_options.CacheDependency == CacheOption.MemoryCache)
            {
                _memoryCache = memoryCache ?? throw new ArgumentException(nameof(memoryCache));
            }
            else
            {
                _distributedCache = distributedCache ?? throw new ArgumentException(nameof(distributedCache));
            }
        }

        public void Delete(string resourceKey, string cultureName, Type resourceSource)
        {
//            var resourceType = resourceSource.GetTypeInfo();
//            var resourceName = $"{resourceType.FullName}.xml";
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resourceName);
//            var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, cultureName,
//                resourceName, name);

            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            
            _logger.LogInformation($"Read file from {computedPath}");
            
            var records = XmlReader.Read<List<LocalizationEntity>>(computedPath);
            
            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);
            
            var entity = records.FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                                                  && a.Resource == computedKey);

            if (entity != null)
            {
                records.Remove(records.FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                                         && a.Resource == computedKey));
                XmlReader.Write(records, computedKey);
                
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
//            var resourceType = resourceSource.GetTypeInfo();
//            var resourceName = $"{resourceType.FullName}.xml";
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resourceName);

            var resourceName = resourceSource.GetTypeName();
            resourceName = Helpers.TryFixInnerClassPath(resourceName);
            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            
            _logger.LogInformation($"Read file from {computedPath}");
            
            var records = XmlReader.Read<List<LocalizationEntity>>(computedPath);

            var isSuccess = new List<string>();
            
            foreach (var resourceKey in resourceKeys)
            {
//                var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, cultureName,
//                    resourceName, item);

                var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);

                var entity = records.FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                                              && a.Resource == computedKey);

                if (entity != null)
                {
                    records.Remove(records.FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                                             && a.Resource == computedKey));
                    isSuccess.Add(computedKey);
                    _logger.LogInformation($"Removed: {entity}");

                }
                else
                {
                    _logger.LogInformation($"No record was found with this key : {resourceKey}");
                }
            }

            XmlReader.Write(records, computedPath);

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
//            var resourceType = resourceSource.GetTypeInfo();
//            var resourceName = $"{resourceType.FullName}.xml";
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resourceName);

            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            _logger.LogInformation($"Read file from {computedPath}");
            return File.ReadAllText(computedPath);
        }

        public string ExportXml(string cultureName, Type resourceSource)
        {
//            var resourceType = resourceSource.GetTypeInfo();
//            var resourceName = $"{resourceType.FullName}.xml";
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resourceName);
            
            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            _logger.LogInformation($"Read file from {computedPath}");
            var records = XmlReader.Read<List<LocalizationEntity>>(computedPath);

            return JsonConvert.SerializeObject(records);
        }

        public void Insert(string resourceKey, string value, string cultureName, Type resourceSource)
        {
//            var resource =string.IsNullOrEmpty(resourceName) ? nameof(LocalizationResourceNames.SharedResource) : resourceName;
//            string computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, cultureName, resource,
//                resourceKey);
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resource);

            var resourceName = resourceSource.GetTypeName();
            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            _logger.LogInformation($"Read file from {computedPath}");
            var records = XmlReader.Read<List<LocalizationEntity>>(computedPath);

            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);
            var entity = records.FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                                                         && a.Resource ==
                                                                                         computedKey);

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

                XmlReader.Write(records, computedKey);
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
//            var resourceType = resourceSource.GetTypeInfo();
//            var resourceName = $"{resourceType.FullName}.xml";
//
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resourceName);

            var resourceName = resourceSource.GetTypeName();
            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            _logger.LogInformation($"Read file from {computedPath}");
            var records = XmlReader.Read<List<LocalizationEntity>>(computedPath);

            var isSuccess = new List<KeyValuePair<string, string>>();
            
            foreach (var item in keyValue)
            {
//                string computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, cultureName,
//                    resourceName, item.Key);

                var computedKey = Helpers.GetComputedCacheKey(resourceName, item.Key, cultureName);
                
                var entity = records.FirstOrDefault(a =>
                    a.Key == item.Key && a.CultureName == cultureName
                                      && a.Resource == computedKey);

                if (entity == null)
                {
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
                else
                {
                    _logger.LogInformation($"No record was found with this key : {item.Key}");
                }
            }

            XmlReader.Write(records, computedPath);

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
//            var resourceType = resourceSource.GetTypeInfo();
//            var resourceName = $"{resourceType.FullName}.xml";
//
//            string computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, cultureName,
//                resourceName, name);
//
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resourceName);

            var resourceName = resourceSource.GetTypeName();
            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            _logger.LogInformation($"Read file from {computedPath}");
            var records = XmlReader.Read<List<LocalizationEntity>>(computedPath);

            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);
            
            var entity = records.FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                                                  && a.Resource == computedKey);

            if (entity != null)
            {
                records.Where(a => a.Key == resourceKey && a.CultureName == cultureName
                                                 && a.Resource == computedKey).ToList()
                    .ForEach(x => x.Value = value);

                XmlReader.Write(records, computedKey);
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
            else
            {
                _logger.LogInformation($"No record was found with this key : {resourceKey}");
            }
        }

        public void Update(IEnumerable<KeyValuePair<string, string>> resourceKeys, string cultureName, Type resourceSource)
        {
//            var resourceType = resourceSource.GetTypeInfo();
//            var resourceName = $"{resourceType.FullName}.xml";
//
//            string computedPath = string.Format(XmlConfiguration.FileName(), cultureName, resourceName);

            var resourceName = resourceSource.GetTypeName();
            var computedPath = resourceSource.GetResourcesPath(_options.ResourcesPath);
            _logger.LogInformation($"Read file from {computedPath}");

            var records = XmlReader.Read<List<LocalizationEntity>>(computedPath);

            var isSuccess = new List<KeyValuePair<string, string>>();
            
            foreach (var item in resourceKeys)
            {
//                string computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, cultureName,
//                    resourceName, item.Key);

                var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
                    CultureInfo.CurrentUICulture.Name, resourceName, item.Key);
                
                var entity = records.FirstOrDefault(a =>
                    a.Key == item.Key && a.CultureName == cultureName
                                      && a.Resource == computedKey);

                if (entity != null)
                {
                    records.Where(a => a.Key == item.Key &&
                                       a.CultureName == cultureName &&
                                       a.Resource == computedKey).ToList().ForEach(x =>
                    {
                        x.Value = item.Value;
                        isSuccess.Add(new KeyValuePair<string, string>(computedKey, item.Value));
                    });

                    _logger.LogInformation($"Update: {entity}");
                }
                else
                {
                    _logger.LogInformation($"No record was found with this key : {item.Key}");
                }
            }

            XmlReader.Write(records, computedPath);

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
    }
}