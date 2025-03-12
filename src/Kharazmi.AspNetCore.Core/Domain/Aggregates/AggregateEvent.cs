using System;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public interface IAggregateEvent 
    {
    }

    public interface IAggregateEvent<TAggregate, TKey> : IAggregateEvent
        where TAggregate : IAggregateRoot<TKey> where TKey : IEquatable<TKey>
    {
    }

    public abstract class AggregateEvent<TAggregate, TKey> : IAggregateEvent<TAggregate, TKey>
        where TAggregate : IAggregateRoot<TKey> where TKey : IEquatable<TKey>
    {
        public override string ToString()
        {
            return $"{typeof(TAggregate).PrettyPrint()}/{GetType().PrettyPrint()}";
        }
    }
}