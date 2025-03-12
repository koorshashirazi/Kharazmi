using System;

namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    public interface IEntityTranslation
    {
        string Language { get; }
    }

    public interface IEntityTranslation<out TEntity, out TKey> : IEntityTranslation
        where TKey : IEquatable<TKey>
    {
        TEntity Entity { get; }
        TKey EntityId { get; }
    }
}