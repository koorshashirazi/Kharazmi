using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddUserEventHandler : Handlers.EventHandler<AddUserDomainEvent>
    {

        public AddUserEventHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Task<Result> TryHandleAsync(CancellationToken cancellationToken = default)
        {
            return RaiseEventAsync(new AddedUserDomainEvent(Event.Id, Event.Name), cancellationToken);
        }
    }
}