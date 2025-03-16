using System;
using Kharazmi.AspNetCore.Core.Domain.Commands;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddUserCommand : Command
    {
        public AddUserCommand(string id, string name)
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