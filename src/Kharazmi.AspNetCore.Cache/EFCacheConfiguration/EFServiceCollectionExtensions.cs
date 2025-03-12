using System;
using CacheManager.Core;
using Kharazmi.AspNetCore.Cache.Ef;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Cache.EFCacheConfiguration
{
    public enum EfCacheStoreType
    {
        InMemory,
        Redis
    }

    public class EfCacheInMemoryOptions
    {
        public string InstanceName { get; set; } = "MemoryCache1";
        public ExpirationMode ExpirationMode { get; set; } = ExpirationMode.Absolute;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(10);
    }

    public class EfCacheRedisOptions
    {
        public string ConfigurationKey { get; set; } = "redis";
        public ExpirationMode ExpirationMode { get; set; } = ExpirationMode.Absolute;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(10);
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        public int MaxRetries { get; set; } = 100;
        public int RetryTimeout { get; set; } = 50;
    }

    /// <summary>
    /// ServiceCollection Extensions
    /// </summary>
    public static class EFServiceCollectionExtensions
    {
        /// <summary>
        /// A collection of service descriptors.
        /// </summary>
        public static IServiceCollection ServiceCollection { get; set; }

        /// <summary>
        /// Registers the required services of the EFSecondLevelCache.Core.
        /// </summary>
        public static EfCacheBuilder AddEfSecondLevelCache(this IServiceCollection services, EfCacheStoreType storeType)
        {
            services.AddSingleton<IEFCacheKeyHashProvider, EFCacheKeyHashProvider>();
            services.AddSingleton<IEFCacheKeyProvider, EFCacheKeyProvider>();
            services.AddSingleton<IEFCacheServiceProvider, EFCacheServiceProvider>();

            ServiceCollection = services;
            return new EfCacheBuilder(services, storeType);
        }
    }

    public class EfCacheBuilder
    {
        private readonly EfCacheStoreType _storeType;

        public EfCacheBuilder(IServiceCollection services, EfCacheStoreType storeType)
        {
            _storeType = storeType;
            Services = services;
        }

        public IServiceCollection Services { get; }

        public EfCacheBuilder UseInMemory(Action<EfCacheInMemoryOptions> options)
        {
            if (options == null) return this;
            Services.Configure(options);

            var config = Services.BuildServiceProvider().GetService<IOptions<EfCacheInMemoryOptions>>().Value;

            switch (_storeType)
            {
                case EfCacheStoreType.InMemory:
                    Services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
                    Services.AddSingleton(new CacheManagerConfiguration().Builder
                            .WithJsonSerializer()
                            .WithMicrosoftMemoryCacheHandle(config.InstanceName)
                            .WithExpiration(config.ExpirationMode, config.Timeout)
                            .Build());
                    break;
                case EfCacheStoreType.Redis:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        public EfCacheBuilder UseRedis(Action<EfCacheRedisOptions> options)
        {
            if (options == null) return this;
            Services.Configure(options);

            var redisOptions = Services.BuildServiceProvider().GetService<IOptions<EfCacheRedisOptions>>().Value;
            if (redisOptions == null) return this;

            switch (_storeType)
            {
                case EfCacheStoreType.InMemory:
                    break;
                case EfCacheStoreType.Redis:

                    var jss = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    var redisConfigurationKey = redisOptions.ConfigurationKey;
                    Services.AddSingleton(new CacheManagerConfiguration().Builder
                            .WithJsonSerializer(serializationSettings: jss, deserializationSettings: jss)
                            .WithUpdateMode(CacheUpdateMode.Up)
                            .WithRedisConfiguration(redisConfigurationKey, config =>
                            {
                                config.WithAllowAdmin()
                                    .WithDatabase(0)
                                    .WithEndpoint(redisOptions.Host, redisOptions.Port);
                            })
                            .WithMaxRetries(redisOptions.MaxRetries)
                            .WithRetryTimeout(redisOptions.RetryTimeout)
                            .WithRedisCacheHandle(redisConfigurationKey)
                            .WithExpiration(redisOptions.ExpirationMode, redisOptions.Timeout)
                            .Build());
                    Services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }
    }
}