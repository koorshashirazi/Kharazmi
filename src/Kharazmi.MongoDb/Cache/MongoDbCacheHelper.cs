using Kharazmi.AspNetCore.Core.Threading;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Kharazmi.MongoDb.Cache
{
    internal static class MongoDbCacheHelper
    {
        public static void Setup(IMongoDbContext<MongoDbCacheOptions> context, string collectionName)
        {
            CreateDropCollection(context, collectionName);
            CreateIndex(context, collectionName);
        }

        public static void CreateDropCollection(IMongoDbContext<MongoDbCacheOptions> context,
            string collectionName)
        {
            if (context.Exists(collectionName))
            {
                AsyncHelper.RunSync(async () => await context.DropCollectionAsync(collectionName).ConfigureAwait(false));
            }

            AsyncHelper.RunSync(async () =>
                await context.CreateCollectionAsync(collectionName, new CreateCollectionOptions()).ConfigureAwait(false));
        }

        public static void CreateIndex(IMongoDbContext<MongoDbCacheOptions> context,
            string collectionName)
        {
            var collection = context.GetCollection<CacheItem>(collectionName);
            collection.Indexes.CreateOne(new CreateIndexModel<CacheItem>(
                Builders<CacheItem>.IndexKeys.Descending(p => p.ExpiresAt), new CreateIndexOptions
                {
                    Background = true,
                    Name = "cache_item_expireAt_index"
                }));
        }

        public static void RegisterMapping()
        {
            var cp = new ConventionPack {new IgnoreExtraElementsConvention(true)};
            ConventionRegistry.Register("MongoDbDistributeCache", cp, t => true);

            BsonClassMap.RegisterClassMap<CacheItem>(config =>
            {
                config.AutoMap();
                config.MapIdMember(x => x.Id);
            });
        }
    }
}