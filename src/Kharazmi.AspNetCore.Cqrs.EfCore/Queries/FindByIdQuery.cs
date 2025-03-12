using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Kharazmi.AspNetCore.Cqrs.Messages;

namespace Kharazmi.AspNetCore.EfCore.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TReadModel"></typeparam>
    public class FindByIdQuery<TKey, TEntity, TReadModel> : IQuery<TReadModel> where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        public TKey Id { get; set; }
    }
}
