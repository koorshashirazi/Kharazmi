using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Model : Model<int>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Model<TKey> : ITrackable where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        public TKey Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TrackingState TrackingState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<string> ModifiedProperties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsNew() =>
            EqualityComparer<TKey>.Default.Equals(Id, default) || TrackingState == TrackingState.Added;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsModified() => !IsNew() && TrackingState == TrackingState.Modified;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsUnchanged() => !IsNew() && TrackingState == TrackingState.Unchanged;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsDeleted() => !IsNew() && TrackingState == TrackingState.Deleted;
    }
}