using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public class Cache : Entity<string>
    {
        public Cache(string id): base(id)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Value { get; set; } = [];

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset ExpiresAtTime { get; set; } = DateTimeOffset.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        public long? SlidingExpirationInSeconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }
    }
}