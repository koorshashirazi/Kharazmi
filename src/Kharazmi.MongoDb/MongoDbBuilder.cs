using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.MongoDb.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.MongoDb
{
    public interface IMongoDbBuilder
    {
        IServiceCollection Services { get; }

        IMongoDbBuilder WithDistributeCache(MongoDbCacheOptions options);
    }

    public class MongoDbBuilder : IMongoDbBuilder
    {
        public MongoDbBuilder(IServiceCollection services)
        {
            Services = Ensure.ArgumentIsNotNull(services, nameof(services));
        }

        public IServiceCollection Services { get; }

        public IMongoDbBuilder WithDistributeCache(MongoDbCacheOptions options)
        {
            Services.AddSingleton(options);
            Services.AddMongoDb(options);
            Services.AddScoped<IMongoDbCacheRepository, MongoDbCacheRepository>();
            Services.AddSingleton<IDistributedCache, MongoDbDistributedCache>();
            return this;
        }
    }
}