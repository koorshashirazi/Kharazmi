using System;

 namespace Kharazmi.MongoDb.Cache
{
    public class MongoDbCacheOptions : MongoDbOptions<MongoDbCacheOptions>
    {
        /// <summary>
        /// If set true, the table will be removed and rebuilt.
        /// </summary>
        public bool CreateDropCollection { get; set; }

        public TimeSpan? ExpiredScanInterval { get; set; }
    }
}