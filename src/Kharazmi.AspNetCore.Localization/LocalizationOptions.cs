namespace Kharazmi.AspNetCore.Localization
{
    public enum CacheOption
    {
        MemoryCache,
        DistributedCache
    }

    public class LocalizationOptions
    {
        /// <summary>
        /// Gets or sets the cache dependency.
        /// </summary>
        /// <value>The cache dependency.</value>
        public CacheOption CacheDependency { get; set; } = CacheOption.MemoryCache;

        /// <summary>
        /// The relative path under application root where resource files are located.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;
    }
}