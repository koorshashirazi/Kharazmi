using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;

namespace Kharazmi.EventSourcing.EfCore.Behaviors
{

    public class TransactionCommandPipeline<TCommand> : ICommandHandler<TCommand>, IPipelineHandler
        where TCommand : Command
    {
        private readonly IEventStoreUnitOfWork _uow;
        private readonly ICommandHandler<TCommand> _nextHandler;

       
        public TransactionCommandPipeline(IEventStoreUnitOfWork uow, ICommandHandler<TCommand> nextHandler)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            Ensure.ArgumentIsNotNull(nextHandler, nameof(nextHandler));
            _nextHandler = nextHandler;
        }


        public async Task<Result> HandleAsync(TCommand command, DomainContext domainContext, CancellationToken cancellationToken = default)
        {
            var result = await _nextHandler.HandleAsync(command, domainContext, cancellationToken).ConfigureAwait(false);

            if (result.Failed)
            {
                return result;
            }

            await _uow.CommitAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}