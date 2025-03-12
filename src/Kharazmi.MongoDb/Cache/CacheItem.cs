using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Kharazmi.MongoDb.Cache
{
    public class CacheItem : MongoEntity
    {
        [BsonElement("value")] public byte[] Value { get; set; }

        [BsonElement("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }

        [BsonElement("absolute_expiration")] public DateTimeOffset? AbsoluteExpiration { get; set; }

        [BsonElement("sliding_expiration")] public double? SlidingExpirationInSeconds { get; set; }

        public CacheItem()
        {
        }


        public CacheItem WithExpiresAt(DateTimeOffset? expiresAt)
        {
            ExpiresAt = expiresAt;
            return this;
        }

        [BsonIgnore]
        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return Id;
                yield return ExpiresAt;
                yield return AbsoluteExpiration;
                
            }
        }
    }
}