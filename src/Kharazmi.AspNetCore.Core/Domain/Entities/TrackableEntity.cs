using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TrackableEntity : TrackableEntity<int>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class TrackableEntity<TKey> : Entity<TKey>, ITrackable where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        [NotMapped] public TrackingState TrackingState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped] public ICollection<string> ModifiedProperties { get; set; }
    }
}