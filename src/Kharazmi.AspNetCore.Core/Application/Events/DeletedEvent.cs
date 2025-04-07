using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kharazmi.AspNetCore.Core.Application.Models;

namespace Kharazmi.AspNetCore.Core.Application.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class DeletedEvent<TModel, TKey> : BusinessEvent
        where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        public DeletedEvent(ReadOnlyCollection<TModel> models)
        {
            Models = models;
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<TModel> Models { get; }
    }
}