using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;

namespace Kharazmi.EventSourcing.EfCore.Behaviors
{
    public class TransactionDomainCommandPipeline<TCommand> : DomainCommandHandler<TCommand>, IPipelineHandler
        where TCommand : class, IDomainCommand
    {
        private readonly IEventStoreUnitOfWork _uow;
        private readonly IDomainCommandHandler<TCommand> _nextHandler;


        public TransactionDomainCommandPipeline(IEventStoreUnitOfWork uow, IDomainCommandHandler<TCommand> nextHandler)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            Ensure.ArgumentIsNotNull(nextHandler, nameof(nextHandler));
            _nextHandler = nextHandler;
        }

        public override async Task<Result> HandleAsync(TCommand command, CancellationToken token = default)
        {
            var result = await _nextHandler.HandleAsync(command, token).ConfigureAwait(false);

            if (result.Failed)
            {
                return result;
            }

            await _uow.CommitAsync(token).ConfigureAwait(false);

            return result;
        }
    }
}