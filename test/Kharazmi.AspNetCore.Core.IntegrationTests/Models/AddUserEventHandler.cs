using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddUserDomainEventHandler : DomainEventHandler<AddUserDomainEvent>
    {
        private readonly IDomainDispatcher _domainDispatcher;

        public AddUserDomainEventHandler(IDomainDispatcher domainDispatcher)
        {
            _domainDispatcher = domainDispatcher;
        }

        public override Task<Result> HandleAsync(AddUserDomainEvent domainEvent, CancellationToken token = default)
        {
            return _domainDispatcher.RaiseAsync(domainEvent, token);
        }
    }
}