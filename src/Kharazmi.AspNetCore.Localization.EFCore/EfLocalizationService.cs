using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Kharazmi.AspNetCore.Core.Linq;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Localization.EFCore
{
    public interface IEfLocalizerService : ILocalizerService
    {
        LocalizationEntity Find(Expression<Func<LocalizationEntity, bool>> predicate);
        LocalizationEntity Find(Type resourceSource, string resourceKey, string cultureName);
        DataSourceResult Get(DataSourceRequest request, Expression<Func<LocalizationEntity, bool>> predicate = null);
        PagedList<LocalizationEntity> GetPaged(
            int page = 1,
            int pageSize = 10,
            Expression<Func<LocalizationEntity, bool>> predicate = null,
            Expression<Func<LocalizationEntity, string>> orderBy = null);
    }

    public class EfLocalizationService<TContext> : IEfLocalizerService
        where TContext : DbContext
    {
        private readonly ILogger _logger;

        private readonly IServiceProvider _resolver;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly LocalizationOptions _options;

        public EfLocalizationService(
            ILoggerFactory loggerFactory,
            IServiceProvider resolver,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            IOptions<LocalizationOptions> options)
        {
            _resolver = resolver ?? throw new ArgumentException(nameof(resolver));
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<EfLocalizationService<TContext>>();

            switch (_options.CacheDependency)
            {
                case CacheOption.MemoryCache:
                    _memoryCache = memoryCache ?? throw new ArgumentException(nameof(memoryCache));
                    _logger.LogInformation($"Selected Cache option : MemoryCache");
                    break;
                case CacheOption.DistributedCache:
                    _distributedCache = distributedCache ?? throw new ArgumentException(nameof(distributedCache));
                    _logger.LogInformation($"Selected Cache option : DistributedCache");
                    break;
                default:
                    break;
            }
        }

        public void Delete(string resourceKey, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;

//            var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
//                CultureInfo.CurrentUICulture.AggregateType, resourceName, resourceKey);

            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);
            LocalizationEntity entity = null;
            var isSuccess = _resolver.RunScopedService<bool, TContext>(context =>
            {
                entity = context.Set<LocalizationEntity>()
                    .FirstOrDefault(a => a.Key == resourceKey &&
                                         a.CultureName == cultureName &&
                                         a.Resource == computedKey);

                if (entity != null)
                {
                    context.Set<LocalizationEntity>().Remove(entity);
                    try
                    {
                        context.SaveChanges();
                        _logger.LogInformation($"Removed: {entity}");
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Remove failed , key {resourceKey}");
                        _logger.LogDebug(e.Message);
                        return false;
                    }
                }

                _logger.LogInformation($"No record was found with this key : {resourceKey}");
                return false;
            });

            if (isSuccess)
            {
                if (_options.CacheDependency == CacheOption.MemoryCache)
                    _memoryCache.Remove(computedKey);
                else
                    _distributedCache.Remove(computedKey);

                _logger.LogInformation($"Remove Entity: \n[{entity}] \nwith key [{resourceKey}] from Cache");
            }
            else
            {
                _logger.LogError($"Remove failed : with key [{resourceKey}]");
            }
        }

        public void Delete(IEnumerable<string> resourceKeys, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;

            var isSuccess = new List<string>();
            _resolver.RunScopedService<TContext>(context =>
            {
                foreach (var resourceKey in resourceKeys)
                {
//                    var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
//                        CultureInfo.CurrentUICulture.AggregateType, resourceName, item);

                    var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);

                    var entity = context.Set<LocalizationEntity>()
                        .FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                                  && a.Resource == computedKey);

                    if (entity != null)
                    {
                        context.Set<LocalizationEntity>().Remove(entity);
                        isSuccess.Add(computedKey);
                        _logger.LogInformation($"Removing for: {entity}");
                    }
                    else
                    {
                        _logger.LogInformation($"No record was found with this key : {resourceKey}");
                    }
                }

                try
                {
                    context.SaveChanges();
                    _logger.LogInformation($"Removed all records from the database");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Delete failed");
                    _logger.LogDebug(e.Message);
                }
            });

            switch (_options.CacheDependency)
            {
                case CacheOption.MemoryCache:
                {
                    foreach (var item in isSuccess)
                    {
                        _memoryCache.Remove(item);
                        _logger.LogInformation($"Removed from cache with key : {item}");
                    }

                    break;
                }
                case CacheOption.DistributedCache:
                {
                    foreach (var item in isSuccess)
                    {
                        _distributedCache.Remove(item);
                        _logger.LogInformation($"Removed from cache with key : {item}");
                    }

                    break;
                }
                default:
                    _logger.LogInformation("Invalid CacheDependency");
                    break;
            }
        }

        public string ExportJson(string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;

            var computedKey = string.Format(DefaultConfiguration.LocalizationPathTemplate,
                CultureInfo.CurrentUICulture.Name, resourceName);

            var records = new List<LocalizationEntity>();
            _resolver.RunScopedService<TContext>(context =>
            {
                records = context.Set<LocalizationEntity>().Where(e => e.Resource.Contains(computedKey))
                    .ToList();
            });

            return Newtonsoft.Json.JsonConvert.SerializeObject(records);
        }

        public string ExportXml(string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;
            var computedKey = string.Format(DefaultConfiguration.LocalizationPathTemplate,
                CultureInfo.CurrentUICulture.Name, resourceName);

            var records = new List<LocalizationEntity>();
            _resolver.RunScopedService<TContext>(context =>
            {
                records = context.Set<LocalizationEntity>().Where(e => e.Resource.Contains(computedKey))
                    .ToList();
            });
            var xmlSerializer = new XmlSerializer(typeof(List<LocalizationEntity>));
            var memoryStream = new MemoryStream();
            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            xmlSerializer.Serialize(xmlTextWriter, records);

            var output = Encoding.UTF8.GetString(memoryStream.ToArray());
            var _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (output.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
            {
                output = output.Remove(0, _byteOrderMarkUtf8.Length);
            }

            return output;
        }

        public void Insert(string resourceKey, string value, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;

//            var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
//                CultureInfo.CurrentUICulture.AggregateType, resourceName, resourceKey);

            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);

            LocalizationEntity entity = null;

            var isSuccess = _resolver.RunScopedService<bool, TContext>(context =>
            {
                entity = context.Set<LocalizationEntity>()
                    .FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                              && a.Resource == computedKey);

                if (entity == null)
                {
                    entity = new LocalizationEntity
                    {
                        Key = resourceKey,
                        Value = value,
                        CultureName = cultureName,
                        Resource = computedKey,
                    };
                    context.Set<LocalizationEntity>().Add(entity);
                    try
                    {
                        context.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Insert failed, Key : [{resourceKey}]");
                        _logger.LogDebug(e.Message);
                        return false;
                    }
                }

                Update(resourceKey, value, cultureName, resourceSource);

                return false;
            });

            if (isSuccess)
            {
                if (_options.CacheDependency == CacheOption.MemoryCache)
                {
                    _memoryCache.Set(computedKey, value);
                    _logger.LogInformation($"Add To Memory Cache : {entity}");
                }
                else
                {
                    _distributedCache.SetString(computedKey, value);
                    _logger.LogInformation($"Add To Distributed Cache : {entity}");
                }
            }
            else
            {
                _logger.LogInformation($"Insert failed : with key [{resourceKey}]");
            }
        }

        public void Insert(IEnumerable<KeyValuePair<string, string>> keyValue, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;
            var isSuccess = new List<KeyValuePair<string, string>>();
            _resolver.RunScopedService<TContext>(context =>
            {
                foreach (var item in keyValue)
                {
//                    var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
//                        CultureInfo.CurrentUICulture.AggregateType, resourceName, item.Key);

                    var computedKey = Helpers.GetComputedCacheKey(resourceName, item.Key, cultureName);

                    var entity = context.Set<LocalizationEntity>()
                        .FirstOrDefault(a => a.Key == item.Key && a.CultureName == cultureName
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
                        context.Set<LocalizationEntity>().Add(entity);
                        isSuccess.Add(new KeyValuePair<string, string>(computedKey, item.Value));
                        _logger.LogInformation($"Inserting {entity}, Key {item.Key}");
                    }
                }

                try
                {
                    context.SaveChanges();
                    _logger.LogInformation($"Insert Succeeded");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Insert Failed");
                }
            });

            switch (_options.CacheDependency)
            {
                case CacheOption.MemoryCache:
                {
                    foreach (var item in isSuccess)
                    {
                        _memoryCache.Set(item.Key, item.Value);
                        _logger.LogInformation($"Added to cache, Key: {item}");
                    }

                    break;
                }
                case CacheOption.DistributedCache:
                    foreach (var item in isSuccess)
                    {
                        _distributedCache.SetString(item.Key, item.Value);
                        _logger.LogInformation($"Added to cache, Key: {item}");
                    }

                    break;
                default:
                    _logger.LogError($"Invalid cache option");
                    break;
            }
        }

        public void Update(string resourceKey, string value, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;

//            var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
//                CultureInfo.CurrentUICulture.AggregateType, resourceName, name);

            var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);

            var isSuccess = _resolver.RunScopedService<bool, TContext>(context =>
            {
                var entity = context.Set<LocalizationEntity>()
                    .FirstOrDefault(a => a.Key == resourceKey && a.CultureName == cultureName
                                                              && a.Resource == computedKey);

                if (entity != null)
                {
                    entity.Value = value;

                    context.Set<LocalizationEntity>().Update(entity);
                    try
                    {
                        context.SaveChanges();
                        _logger.LogInformation($"Update succeeded with key : {resourceKey}");
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Update failed");
                        _logger.LogDebug(e.Message);
                        return false;
                    }
                }

                _logger.LogInformation($"No record was found with this key : {resourceKey}");
                return false;
            });

            if (isSuccess)
            {
                if (_options.CacheDependency == CacheOption.MemoryCache)
                {
                    _memoryCache.Set(computedKey, value);
                }
                else
                {
                    _distributedCache.SetString(computedKey, value);
                }

                _logger.LogInformation($"Update cache succeeded with key : {resourceKey}");
            }
            else
            {
                _logger.LogError($"Update failed with key : {resourceKey}");
            }
        }

        public void Update(IEnumerable<KeyValuePair<string, string>> keyValue, string cultureName, Type resourceSource)
        {
            var resourceName = resourceSource.GetTypeInfo().FullName;

            var isSuccess = new List<KeyValuePair<string, string>>();
            _resolver.RunScopedService<TContext>(context =>
            {
                foreach (var item in keyValue)
                {
//                    var computedKey = string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate,
//                        CultureInfo.CurrentUICulture.AggregateType, resourceName, item.Key);


                    var computedKey = Helpers.GetComputedCacheKey(resourceName, item.Key, cultureName);

                    var entity = context.Set<LocalizationEntity>()
                        .FirstOrDefault(a => a.Key == item.Key && a.CultureName == cultureName
                                                               && a.Resource == computedKey);

                    if (entity == null)
                    {
                        _logger.LogWarning($"No record was found with this key : {item}");
                        continue;
                    }

                    entity.Value = item.Value;
                    context.Set<LocalizationEntity>().Update(entity);
                    isSuccess.Add(new KeyValuePair<string, string>(computedKey, item.Value));
                    _logger.LogInformation($"Updating {entity}, Key {item.Key}");
                }

                try
                {
                    context.SaveChanges();
                    _logger.LogInformation("Update succeeded");
                }
                catch (Exception e)
                {
                    _logger.LogError("Update failed");
                    _logger.LogDebug(e.Message);
                }
            });

            switch (_options.CacheDependency)
            {
                case CacheOption.MemoryCache:
                {
                    foreach (var item in isSuccess)
                    {
                        _memoryCache.Set(item.Key, item.Value);
                        _logger.LogInformation($"Added to cache, Key: {item}");
                    }

                    break;
                }
                case CacheOption.DistributedCache:
                    foreach (var item in isSuccess)
                    {
                        _distributedCache.SetString(item.Key, item.Value);
                        _logger.LogInformation($"Added to cache, Key: {item}");
                    }

                    break;
                default:
                    _logger.LogInformation("Invalid cache option");
                    break;
            }
        }

        public LocalizationEntity Find(Expression<Func<LocalizationEntity, bool>> predicate)
        {
            return _resolver.RunScopedService<LocalizationEntity, TContext>(context =>
            {
                var query = context.Set<LocalizationEntity>().AsNoTracking().FirstOrDefault(predicate);
                return query;
            });
        }

        public LocalizationEntity Find(Type resourceSource, string resourceKey, string cultureName)
        {
            return _resolver.RunScopedService<LocalizationEntity, TContext>(context =>
            {
                var resourceName = resourceSource.GetTypeInfo().FullName;
                var computedKey = Helpers.GetComputedCacheKey(resourceName, resourceKey, cultureName);
                
                var query = context.Set<LocalizationEntity>().AsNoTracking().FirstOrDefault(x=> x.Resource.Equals(computedKey));
                return query;
            });
        }

        public DataSourceResult Get(DataSourceRequest request,
            Expression<Func<LocalizationEntity, bool>> predicate = null)
        {
            return _resolver.RunScopedService<DataSourceResult, TContext>(context =>
            {
                var query = context.Set<LocalizationEntity>().AsNoTracking();
                if (predicate != null)
                    query = query.Where(predicate);

                return query.ToDataSourceResult(request);
            });
        }

        public PagedList<LocalizationEntity> GetPaged(
            int page = 1, int pageSize = 10,
            Expression<Func<LocalizationEntity, bool>> predicate = null,
            Expression<Func<LocalizationEntity, string>> orderBy = null)
        {
            return _resolver.RunScopedService<PagedList<LocalizationEntity>, TContext>(context =>
            {
                var pagedList = new PagedList<LocalizationEntity>();

                var query = context.Set<LocalizationEntity>().AsNoTracking();

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                if (orderBy != null)
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    orderBy = entity => entity.Key;
                }

                var entities = query.PageBy(orderBy, page, pageSize).ToList();

                var countQuery = context.Set<LocalizationEntity>().AsNoTracking();

                if (predicate != null)
                {
                    countQuery = countQuery.Where(predicate);
                }

                pagedList.AddRange(entities);
                pagedList.PageSize = pageSize;
                pagedList.TotalCount = countQuery.Count();


                return pagedList;
            });
        }
    }
}