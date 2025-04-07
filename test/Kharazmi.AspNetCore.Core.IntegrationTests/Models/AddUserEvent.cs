using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddUserDomainEvent : DomainEvent
    {
        public AddUserDomainEvent(string id, string name)
            : base(DomainEventType.From<AddUserDomainEvent>())
        {
            Id = id ?? Guid.NewGuid().ToString("N");
            Name = name;
        }

        

        public string Id { get;  }
        public string Name { get;  }
        
    }
}