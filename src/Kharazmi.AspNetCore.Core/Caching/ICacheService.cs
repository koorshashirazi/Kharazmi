using System;
using Microsoft.Extensions.Caching.Memory;

namespace Kharazmi.AspNetCore.Core.Caching
{
    /// <summary>
    /// ICacheService encapsulates IMemoryCache functionality.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
        /// Otherwise it will use the factory method to get the value and then inserts it.
        /// </summary>
        T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Gets the key's value from the cache.
        /// </summary>
        T Get<T>(string cacheKey);

        /// <summary>
        /// Tries to get the key's value from the cache.
        /// </summary>
        bool TryGetValue<T>(string cacheKey, out T result);

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Adds a key-value to the cache.
        /// It will use the factory method to get the value and then inserts it.
        /// </summary>
        void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        void Add<T>(string cacheKey, T value, MemoryCacheEntryOptions options);
        
        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        void Add<T>(string cacheKey, T value);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        void Remove(string cacheKey);
    }
}