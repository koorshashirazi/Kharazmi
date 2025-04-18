using Kharazmi.AspNetCore.Core.Domain.Aggregates;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests
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