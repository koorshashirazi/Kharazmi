using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Domain.Queries;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        Task<Result> SendCommandAsync<TCommand>(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken = default) where TCommand : ICommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task<Result> RaiseEventAsync<TEvent>(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        Task<TResult> QueryResultAsync<TResult>(IQuery<TResult> query, DomainContext domainContext,
            CancellationToken cancellationToken = default);
    }

    internal class DomainDispatcher : IDomainDispatcher
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public DomainDispatcher(
            ICommandDispatcher commandDispatcher,
            IEventDispatcher eventDispatcher,
            IQueryDispatcher queryDispatcher,
            EventSourcingOptions eventSourcingOptions,
            IEventStore eventStore)
        {
            _commandDispatcher = commandDispatcher;
            _eventDispatcher = eventDispatcher;
            _queryDispatcher = queryDispatcher;
        }


        public Task<Result> SendCommandAsync<TCommand>(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            return _commandDispatcher.SendAsync(command, domainContext, cancellationToken);
        }

        public async Task<Result> RaiseEventAsync<TEvent>(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default)
            where TEvent : class, IDomainEvent
        {
            return await _eventDispatcher.RaiseAsync(@event, domainContext, cancellationToken).ConfigureAwait(false);
        }

        public Task<TResult> QueryResultAsync<TResult>(IQuery<TResult> query, DomainContext domainContext,
            CancellationToken cancellationToken = default)
            => _queryDispatcher.QueryAsync(query, domainContext, cancellationToken);
    }
}