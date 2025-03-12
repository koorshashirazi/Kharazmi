using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.EventSourcing.EfCore.Test
{
    public class EntityAsAggregateRoot : AggregateRoot<EntityAsAggregateRoot, string>
    {
        public EntityAsAggregateRoot()
        {
        }

        public EntityAsAggregateRoot(string id, ulong version) : base(id, version)
        {
        }
    }
}