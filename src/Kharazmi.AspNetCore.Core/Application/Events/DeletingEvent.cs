using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kharazmi.AspNetCore.Core.Application.Models;

namespace Kharazmi.AspNetCore.Core.Application.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class DeletingEvent<TModel, TKey> : BusinessEvent
        where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        public DeletingEvent(IEnumerable<TModel> models)
        {
            Models = models.ToImmutableList();
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<TModel> Models { get; }
    }
}