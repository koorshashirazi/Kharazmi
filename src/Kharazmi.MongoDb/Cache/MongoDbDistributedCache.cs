using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Threading;
using Microsoft.Extensions.Caching.Distributed;

namespace Kharazmi.MongoDb.Cache
{
    internal class MongoDbDistributedCache : IDistributedCache
    {
        private DateTimeOffset _lastScan = DateTimeOffset.UtcNow;
        private readonly DateTimeOffset _now = DateTimeOffset.Now;
        private TimeSpan _scanInterval;
        private readonly TimeSpan _defaultScanInterval = TimeSpan.FromMinutes(5);
        private readonly IMongoDbCacheRepository _cacheRepository;

        public MongoDbDistributedCache(
            IMongoDbCacheRepository cacheRepository,
            MongoDbCacheOptions options)
        {
            _cacheRepository = Ensure.ArgumentIsNotNull(cacheRepository, nameof(cacheRepository));
            SetScanInterval(options?.ExpiredScanInterval ?? _defaultScanInterval);
        }


        public byte[] Get(string key)
        {
            return AsyncHelper.RunSync(() => GetAsync(key));
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            var value = await _cacheRepository.GetCacheItemAsync(key, false, token).ConfigureAwait(false);
            await ScanAndDeleteExpiredAsync().ConfigureAwait(false);
            return value;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            AsyncHelper.RunSync(() => SetAsync(key, value, options, CancellationToken.None));
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            await _cacheRepository.SetAsync(key, value, options, token).ConfigureAwait(false);
            await ScanAndDeleteExpiredAsync().ConfigureAwait(false);
        }

        public void Refresh(string key)
        {
            AsyncHelper.RunSync(() => RefreshAsync(key, CancellationToken.None));
        }

        public async Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            await _cacheRepository.GetCacheItemAsync(key, true, token).ConfigureAwait(false);
            await ScanAndDeleteExpiredAsync().ConfigureAwait(false);
        }

        public void Remove(string key)
        {
            AsyncHelper.RunSync(() => RemoveAsync(key, CancellationToken.None));
        }

        public async Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            await _cacheRepository.RemoveAsync(key, token).ConfigureAwait(false);
            await ScanAndDeleteExpiredAsync().ConfigureAwait(false);
        }

        private async Task ScanAndDeleteExpiredAsync()
        {
            if (_lastScan.Add(_scanInterval) < _now)
            {
                _lastScan = _now;
                await _cacheRepository.DeleteExpiredAsync(_now, CancellationToken.None).ConfigureAwait(false);
            }
        }

        private void SetScanInterval(TimeSpan? scanInterval)
        {
            _scanInterval = scanInterval?.TotalSeconds > 0
                ? scanInterval.Value
                : _defaultScanInterval;
        }
    }
}