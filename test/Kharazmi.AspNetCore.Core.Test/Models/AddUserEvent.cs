using System;
using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.AspNetCore.Core.Test.Models
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