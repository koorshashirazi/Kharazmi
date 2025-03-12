using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;

namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    /// <summary>
    /// Used to indicate that a something is considered personal data.
    /// </summary>
    public class IsPersonalDataAttribute : Attribute
    {
    }

    public interface IEntity
    {
        /// <summary></summary>
        object GetObjectId();
    }
    
    /// <summary>
    /// Interface base entity
    /// </summary>
    public interface IEntity<out TKey>: IEntity
        where TKey : IEquatable<TKey>
    {
        /// <summary></summary>
        TKey Id { get; }
    }

    /// <summary>
    /// Base entity
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Entity<TKey> : ValueObject, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected Entity()
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        protected Entity(TKey id)
        {
            if (Equals(id, default(TKey)))
                throw new ArgumentException("The Id cannot be the type's default value.", nameof(id));

            Id = id;
        }

        /// <summary>Gets or sets the primary key for this role.</summary>
        public TKey Id { get; protected set; }

        public object GetObjectId() => Id;

        /// <summary>
        /// 
        /// </summary>
        protected virtual object This => this;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsTransient()
        {
            if (EqualityComparer<TKey>.Default.Equals(Id, default)) return true;

            //Workaround for EF Core since it sets int/long to min value when attaching to dbContext
            if (typeof(TKey) == typeof(int)) return Convert.ToInt32(Id) <= 0;
            if (typeof(TKey) == typeof(long)) return Convert.ToInt64(Id) <= 0;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Id; }
        }

    }
}