using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddedUserDomainEvent : DomainEvent
    {
        public AddedUserDomainEvent(string id, string name)
            : base(DomainEventType.From<AddedUserDomainEvent>())
        {
            Id = id ?? Guid.NewGuid().ToString("N");
            Name = name;
        }

        public string Id { get;  }
        public string Name { get;  }
        
    }
}