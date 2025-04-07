using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    public interface IDomainCommandHandler
    {
        Task<Result> HandleAsync(IDomainCommand domainCommand,
            CancellationToken token = default);
    }

    public interface IDomainCommandHandler<in TCommand> : IDomainCommandHandler
        where TCommand : IDomainCommand
    {
        Task<Result> HandleAsync(TCommand command, CancellationToken token = default);
    }

    public abstract class DomainCommandHandler<TCommand> : IDomainCommandHandler<TCommand>
        where TCommand : class, IDomainCommand
    {
        public abstract Task<Result> HandleAsync(TCommand command,
            CancellationToken token = default);

        public Task<Result> HandleAsync(IDomainCommand domainCommand, CancellationToken token = default)
            => HandleAsync((TCommand)domainCommand, token);


        protected virtual IEnumerable<IDomainEvent> UseUncommittedEventMapper(
            (IEventSourcing? aggregate, object? state) arg)
        {
            if (arg.aggregate is null)
            {
                yield break;
            }

            foreach (var uncommittedEvent in arg.aggregate.GetUncommittedEvents())
            {
                yield return uncommittedEvent;
            }
        }
    }
}