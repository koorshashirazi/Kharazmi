using System;
using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    public interface IMultiLingualEntity<TTranslation, TKey>
        where TTranslation : Entity<TKey>, IEntityTranslation where TKey : IEquatable<TKey>
    {
        ICollection<TTranslation> Translations { get; }
    }
}