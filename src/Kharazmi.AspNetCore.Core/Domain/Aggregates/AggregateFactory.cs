using System;
using System.Collections.Concurrent;
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public interface IAggregateFactory
    {
        TAggregate GetOrCreate<TAggregate, TKey>(params object[] primitiveArguments)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>;
    }

    public class AggregateFactory(IInstanceCreator instanceCreator) : IAggregateFactory
    {
        private readonly IInstanceCreator _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        private readonly ConcurrentDictionary<string, IAggregateRoot> _aggregateRoots = [];

        public TAggregate GetOrCreate<TAggregate, TKey>(params object[] primitiveArguments)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            return (TAggregate)_aggregateRoots.GetOrAdd(typeof(TAggregate).GetTypeFullName(), _ => _instanceCreator.CreateInstance<TAggregate>());
        }
    }
}