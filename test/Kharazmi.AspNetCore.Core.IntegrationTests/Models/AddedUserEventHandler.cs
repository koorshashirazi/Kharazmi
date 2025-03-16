using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    public class AddedUserEventHandler : IEventHandler<AddedUserDomainEvent>
    {
        private readonly ILogger<AddedUserEventHandler> _logger;

        public AddedUserEventHandler(ILogger<AddedUserEventHandler> logger)
        {
            _logger = logger;
        }
        public Task<Result> HandleAsync(AddedUserDomainEvent domainEvent, DomainContext domainContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("From AddUserHandler");
            
            return Task.FromResult(Result.Ok());
        }
    }
}