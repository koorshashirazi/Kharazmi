using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;
using Kharazmi.MessageBroker;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    [PipelineBehavior(typeof(LoggerCommandPipeline<>))]
    public class AddUserCommandHandler : CommandHandler<AddUserCommand>
    {
        private readonly IBusManager _busManager;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(
            IBusManager busManager,
            ILogger<AddUserCommandHandler> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _busManager = busManager;
            _logger = logger;
        }


        public override async Task<Result> TryHandleAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("From AddUserHandler");
         
            // Store command in db
            // ..
            // Tes message broker
            await _busManager.PublishEventAsync(new AddUserDomainEvent(Command.Id, Command.Name), DomainContext, cancellationToken).ConfigureAwait(false);
            
            // Test domain notification
            await RaiseEventAsync(DomainNotificationDomainEvent.For("test"), cancellationToken)
                .ConfigureAwait(false);
            
            // Test manuel command validation 
            var result = await ValidateCommandAsync(cancellationToken).ConfigureAwait(false);
            var f = await DomainNotifier.GetNotificationsAsync().ConfigureAwait(false);
            
            return Result.Ok(f);
        }
    }
}