using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddUserDomainCommand : DomainCommand
    {
        public AddUserDomainCommand(string id, string name)
        {
            Id = id ?? Guid.NewGuid().ToString("N");
            Name = name;
        }

        public string Id { get;  }
        public string Name { get;  }
        public override bool IsValid()
        {
            return true;
        }
    }
}