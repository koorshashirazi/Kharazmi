using System;
using System.Collections.Concurrent;
using Kharazmi.AspNetCore.Core.Dependency;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public interface IAggregateFactory
    {
        TAggregate GetOrCreate<TAggregate, TKey>(params object[] primitiveArguments)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>;
    }

    public class AggregateFactory(InstanceCreator instanceCreator) : IAggregateFactory
    {
        private readonly InstanceCreator _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        private readonly ConcurrentDictionary<Type, IAggregateRoot> _aggregateRoots = [];

        public TAggregate GetOrCreate<TAggregate, TKey>(params object[] primitiveArguments)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            return (TAggregate)_aggregateRoots.GetOrAdd(typeof(TAggregate),
                _instanceCreator.CreateInstance<TAggregate>());
        }
    }
}