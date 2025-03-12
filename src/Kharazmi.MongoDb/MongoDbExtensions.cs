using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.MongoDb.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.MongoDb
{
    public static class MongoDbExtensions
    {
        public static IMongoDbBuilder AddMongoDb<TOptions>(
            this IServiceCollection services,
            TOptions options) where TOptions : class, IMongoDbOptions<TOptions>
        {
            Ensure.ArgumentIsNotNull(options, nameof(options));
            services.AddSingleton(typeof(IMongoDbOptions<TOptions>), options);
            services.AddScoped(typeof(IMongoDbContext<TOptions>), typeof(MongoDbContext<TOptions>));
            services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));

            return new MongoDbBuilder(services);
        }

        /// <summary>
        /// Adds MongoDb distributed caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IMongoDbBuilder AddMongoDbDistributeCache(
            this IServiceCollection services, 
            MongoDbCacheOptions options)
        {
            return new MongoDbBuilder(services).WithDistributeCache(options);
        }
    }
}