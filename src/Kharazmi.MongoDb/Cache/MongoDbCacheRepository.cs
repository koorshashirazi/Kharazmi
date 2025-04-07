using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Threading;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace Kharazmi.MongoDb.Cache
{
    internal class MongoDbCacheRepository : MongoRepository<CacheItem, MongoDbCacheOptions>, IMongoDbCacheRepository
    {
        public MongoDbCacheRepository(
            IMongoDbContext<MongoDbCacheOptions> dbContext,
            MongoDbCacheOptions options) :
            base(dbContext)
        {
            if (options?.CreateDropCollection ?? false)
            {
                MongoDbCacheHelper.Setup(dbContext, CollectionName);
            }
        }


        public IFindFluent<CacheItem, CacheItem> GetCacheItemQuery(string key, bool refresh)
        {
            var query = GetFluent(x => x.Id.Equals(key));
            if (refresh)
                query = query.Project<CacheItem>(Builders<CacheItem>.Projection.Exclude(x => x.Value));
            return query;
        }


        public void DeleteExpired(DateTimeOffset utcNow)
        {
            DbSet.DeleteMany(Builders<CacheItem>.Filter.Lte(x => x.ExpiresAt, utcNow));
        }

        public Task DeleteExpiredAsync(DateTimeOffset utcNow, CancellationToken cancellationToken = default)
        {
            return DbSet.DeleteManyAsync(Builders<CacheItem>.Filter.Lte(x => x.ExpiresAt, utcNow), cancellationToken);
        }

        public byte[] GetCacheItem(string key, bool refresh)
        {
            return AsyncHelper.RunSync(() => GetCacheItemAsync(key, refresh, CancellationToken.None));
        }


        public async Task<byte[]> GetCacheItemAsync(string key, bool refresh, CancellationToken token = default)
        {
            var utcNow = DateTimeOffset.UtcNow;

            if (key == null)
                return null;

            var query = GetCacheItemQuery(key, refresh);
            var cacheItem = await query.SingleOrDefaultAsync(token).ConfigureAwait(false);
            if (cacheItem == null)
                return null;

            if (CheckIfExpired(utcNow, cacheItem))
            {
                await RemoveAsync(cacheItem.Id, token).ConfigureAwait(false);
                return null;
            }

            cacheItem = await UpdateExpiresAtIfRequiredAsync(utcNow, cacheItem, token).ConfigureAwait(false);

            return cacheItem?.Value;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options = null)
        {
            AsyncHelper.RunSync(() => SetAsync(key, value, options, CancellationToken.None));
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options = null,
            CancellationToken token = default)
        {
            var utcNow = DateTimeOffset.UtcNow;

            if (key == null)
                throw new MongoDbException($"Argument null reference {nameof(key)}");

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var absoluteExpiration = options?.AbsoluteExpiration;
            var slidingExpirationInSeconds = options?.SlidingExpiration?.TotalSeconds;

            if (options?.AbsoluteExpirationRelativeToNow != null)
                absoluteExpiration = utcNow.Add(options.AbsoluteExpirationRelativeToNow.Value);

            if (absoluteExpiration <= utcNow)
                throw new MongoDbException("The absolute expiration value must be in the future.");

            var expiresAt = GetExpiresAt(utcNow, slidingExpirationInSeconds, absoluteExpiration);
            var cacheItem = new CacheItem()
            {
                Id = key,
                Value = value,
                ExpiresAt = expiresAt,
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpirationInSeconds = slidingExpirationInSeconds
            };

            await DbSet.ReplaceOneAsync(x => x.Id.Equals(key), cacheItem, new ReplaceOptions
            {
                IsUpsert = true
            }, token).ConfigureAwait(false);
        }

        public void Remove(string key)
        {
            AsyncHelper.RunSync(() => RemoveAsync(key, CancellationToken.None));
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return DbSet.FindOneAndDeleteAsync(x => x.Id.Equals(key), cancellationToken: token);
        }


        private static bool CheckIfExpired(DateTimeOffset utcNow, CacheItem cacheItem) =>
            cacheItem?.ExpiresAt <= utcNow;

        private async Task<CacheItem> UpdateExpiresAtIfRequiredAsync(DateTimeOffset utcNow, CacheItem cacheItem,
            CancellationToken cancellationToken = default)
        {
            if (cacheItem.ExpiresAt == null)
                return cacheItem;

            var absoluteExpiration =
                GetExpiresAt(utcNow, cacheItem.SlidingExpirationInSeconds, cacheItem.AbsoluteExpiration);


            await FindAndUpdateAsync(x => x.Id.Equals(cacheItem.Id) && x.ExpiresAt != null,
                Builders<CacheItem>.Update.Set(x => x.ExpiresAt, absoluteExpiration), cancellationToken).ConfigureAwait(false);

            return cacheItem.WithExpiresAt(absoluteExpiration);
        }

        private static DateTimeOffset? GetExpiresAt(DateTimeOffset utcNow, double? slidingExpirationInSeconds,
            DateTimeOffset? absoluteExpiration)
        {
            switch (slidingExpirationInSeconds)
            {
                case null when absoluteExpiration == null:
                    return null;
                case null:
                    return absoluteExpiration;
                default:
                {
                    var seconds = slidingExpirationInSeconds.GetValueOrDefault();

                    return utcNow.AddSeconds(seconds) > absoluteExpiration
                        ? absoluteExpiration
                        : utcNow.AddSeconds(seconds);
                }
            }
        }
    }
}