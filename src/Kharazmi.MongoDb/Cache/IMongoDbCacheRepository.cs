using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace Kharazmi.MongoDb.Cache
{
    public interface IMongoDbCacheRepository : IRepository<CacheItem, MongoDbCacheOptions>
    {
        IFindFluent<CacheItem, CacheItem> GetCacheItemQuery(string key, bool refresh);
        void DeleteExpired(DateTimeOffset utcNow);
        Task DeleteExpiredAsync(DateTimeOffset utcNow, CancellationToken cancellationToken = default);
        byte[] GetCacheItem(string key, bool refresh);
        Task<byte[]> GetCacheItemAsync(string key, bool refresh, CancellationToken token = default);
        void Set(string key, byte[] value, DistributedCacheEntryOptions options = null);

        Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options = null,
            CancellationToken token = default);

        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken token = default);
    }
}