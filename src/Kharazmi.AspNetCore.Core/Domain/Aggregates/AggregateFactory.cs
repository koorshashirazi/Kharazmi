using System;
using Kharazmi.AspNetCore.Core.Dependency;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public interface IAggregateFactory
    {
        TAggregate? Create<TAggregate, TKey>(params object[] primitiveArguments)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>;
    }

    public class AggregateFactory : IAggregateFactory
    {
        public TAggregate Create<TAggregate, TKey>(params object[] primitiveArguments)
            where TAggregate : class, IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            return InstanceCreator.CreateInstance<TAggregate>();
        }
    }
}