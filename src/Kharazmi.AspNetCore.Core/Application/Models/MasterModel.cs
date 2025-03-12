using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MasterModel : MasterModel<int>
    {
    }

    public abstract class MasterModel<TKey> : ReadModel<TKey>, IHasRowVersion where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[] Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNew() => EqualityComparer<TKey>.Default.Equals(Id, default);
    }
}