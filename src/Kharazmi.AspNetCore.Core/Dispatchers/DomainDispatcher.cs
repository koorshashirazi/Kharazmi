using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainDispatcher:  IDomainEventDispatcher, IDomainCommandDispatcher, IDomainQueryDispatcher
    {
        Task<Result> RaiseAsync<TEvent>(TEvent domainEvent, CancellationToken token = default)
            where TEvent : class, IDomainEvent;
        
        Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken token = default)
            where TCommand : class, IDomainCommand;
        
        Task<Result<TResult>> QueryAsync<TQuery, TResult>(TQuery domainQuery, CancellationToken token = default)
            where TQuery : class, IDomainQuery;    
    
        IAsyncEnumerable<TResult> QueryStreamAsync<TQuery, TResult>(TQuery domainQuery, CancellationToken token = default)
            where TQuery : class, IDomainQuery;
        
    }

    internal class DomainDispatcher : IDomainDispatcher
    {
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IDomainQueryDispatcher _domainQueryDispatcher;

        public DomainDispatcher(
            IDomainCommandDispatcher domainCommandDispatcher,
            IDomainEventDispatcher domainEventDispatcher,
            IDomainQueryDispatcher domainQueryDispatcher)
        {
            _domainCommandDispatcher = domainCommandDispatcher;
            _domainEventDispatcher = domainEventDispatcher;
            _domainQueryDispatcher = domainQueryDispatcher;
        }


        public Task<Result<TResult>> QueryAsync<TQuery, TResult>(TQuery domainQuery, CancellationToken token = default)
            where TQuery : class, IDomainQuery =>
            _domainQueryDispatcher.QueryAsync<TQuery, TResult>(domainQuery, token);

        public IAsyncEnumerable<TResult> QueryStreamAsync<TQuery, TResult>(TQuery domainQuery,
            CancellationToken token = default) where TQuery : class, IDomainQuery
            => _domainQueryDispatcher.QueryStreamAsync<TQuery, TResult>(domainQuery, token);

        public Task<Result> RaiseAsync<TEvent>( TEvent domainEvent, CancellationToken token = default)
            where TEvent : class, IDomainEvent
            => _domainEventDispatcher.RaiseAsync(domainEvent, token);

        public Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken token = default)
            where TCommand : class, IDomainCommand
            => _domainCommandDispatcher.SendAsync(command, token);
    }
}