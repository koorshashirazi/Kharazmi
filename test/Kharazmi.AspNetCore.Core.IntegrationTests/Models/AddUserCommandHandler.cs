using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;
using Kharazmi.MessageBroker;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    [PipelineBehavior(typeof(LoggerDomainCommandPipeline<>))]
    public class AddUserDomainCommandHandler : DomainCommandHandler<AddUserDomainCommand>
    {
        private readonly IBusManager _busManager;
        private readonly ILogger<AddUserDomainCommandHandler> _logger;
        private readonly IDomainContextService _contextService;
        private readonly IDomainDispatcher _domainDispatcher;
        private readonly ICommandValidationHandler<AddUserDomainCommand> _commandValidationHandler;
        private readonly IDomainNotificationHandler _notificationHandler;

        public AddUserDomainCommandHandler(
            IBusManager busManager,
            IDomainContextService contextService,
            IDomainDispatcher domainDispatcher,
            ICommandValidationHandler<AddUserDomainCommand> commandValidationHandler,
            IDomainNotificationHandler notificationHandler,
            ILogger<AddUserDomainCommandHandler> logger)
        {
            _busManager = busManager;
            _logger = logger;
            _contextService = contextService;
            _domainDispatcher = domainDispatcher;
            _commandValidationHandler = commandValidationHandler;
            _notificationHandler = notificationHandler;
        }


        public override async Task<Result> HandleAsync(AddUserDomainCommand command, CancellationToken token = default)
        {
            _logger.LogInformation("From AddUserHandler");
         
            // Store command in db
            // ..
            // Tes message broker
            var context = await _contextService.GetAsync();
            await _busManager.PublishEventAsync(new AddUserDomainEvent(command.Id, command.Name),context, token).ConfigureAwait(false);
            
            // Test domain notification
            await _domainDispatcher.RaiseAsync(DomainNotificationDomainEvent.For("test"), token)
                .ConfigureAwait(false);
            
            // Test manuel command validation 
            var result = await _commandValidationHandler.HandleAsync(command,token).ConfigureAwait(false);
            var f = await _notificationHandler.GetNotificationsAsync().ConfigureAwait(false);
            
            return Result.Ok(f);
        }
    }
}