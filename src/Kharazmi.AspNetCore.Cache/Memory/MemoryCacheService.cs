using System;
using Kharazmi.AspNetCore.Core.Caching;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.Extensions.Caching.Memory;

namespace Kharazmi.AspNetCore.Cache.Memory
{

  

    /// <summary>
    /// Encapsulates IMemoryCache functionality.
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        ///  Encapsulates IMemoryCache functionality.
        /// </summary>
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = Ensure.ArgumentIsNotNull(memoryCache, nameof(memoryCache));
        }

        /// <summary>
        /// Gets the key's value from the cache.
        /// </summary>
        public T Get<T>(string cacheKey)
        {
            return _memoryCache.Get<T>(cacheKey);
        }

        /// <summary>
        /// Tries to get the key's value from the cache.
        /// </summary>
        public bool TryGetValue<T>(string cacheKey, out T result)
        {
            return _memoryCache.TryGetValue(cacheKey, out result);
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// It will use the factory method to get the value and then inserts it.
        /// </summary>
        public void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration)
        {
            _memoryCache.Set(cacheKey, factory(), absoluteExpiration);
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        public void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration)
        {
            _memoryCache.Set(cacheKey, value, absoluteExpiration);
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        public void Add<T>(string cacheKey, T value)
        {
            _memoryCache.Set(cacheKey, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(string cacheKey, T value, MemoryCacheEntryOptions options)
        {
            _memoryCache.Set(cacheKey, value, options);
        }

        /// <summary>
        /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
        /// Otherwise it will use the factory method to get the value and then inserts it.
        /// </summary>
        public T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration)
        {
            // locks get and set internally
            if (_memoryCache.TryGetValue<T>(cacheKey, out var result))
            {
                return result;
            }

            lock (TypeLock<T>.Lock)
            {
                if (_memoryCache.TryGetValue(cacheKey, out result))
                {
                    return result;
                }

                result = factory();
                _memoryCache.Set(cacheKey, result, absoluteExpiration);

                return result;
            }
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        private static class TypeLock<T>
        {
            public static object Lock { get; } = new object();
        }
    }
}