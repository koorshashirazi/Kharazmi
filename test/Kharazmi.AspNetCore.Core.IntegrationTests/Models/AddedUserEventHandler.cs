using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddedUserDomainEventHandler : DomainEventHandler<AddedUserDomainEvent>
    {
        private readonly ILogger<AddedUserDomainEventHandler> _logger;

        public AddedUserDomainEventHandler(ILogger<AddedUserDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public override Task<Result> HandleAsync(AddedUserDomainEvent domainEvent, CancellationToken toekn = default)
        {
            _logger.LogInformation("From AddUserHandler");

            return Task.FromResult(Result.Ok());
        }
    }
}