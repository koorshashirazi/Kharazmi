using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainQueryDispatcher
    {
        Task<Result<TResult>> QueryAsync<TQuery, TResult>(TQuery domainQuery, CancellationToken token = default)
            where TQuery : class, IDomainQuery;

        IAsyncEnumerable<TResult> QueryStreamAsync<TQuery, TResult>(TQuery domainQuery,
            CancellationToken token = default)
            where TQuery : class, IDomainQuery;
    }

    internal class DomainQueryDispatcher : IDomainQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainQueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task<Result<TResult>> QueryAsync<TQuery, TResult>(TQuery domainQuery,
            CancellationToken token = default) where TQuery : class, IDomainQuery
        {
            return ExceptionHandler.ExecuteResultAsAsync<TQuery, TResult>(async query =>
            {
                if (query == null) throw new ArgumentNullException(nameof(query));
                var handler = _serviceProvider.GetRequiredService<IDomainQueryHandler<TQuery, TResult>>();
                return await handler.HandleAsync(query, token).ConfigureAwait(false);
            }, state: domainQuery, nameof(domainQuery),
                onError: e => Task.FromResult(Result
                .Fail($"Unable to handle the query {DomainQueryType.From<TQuery>()}")
                .WithException(e)
                .MapToFail<TResult>()));
        }

        public IAsyncEnumerable<TResult> QueryStreamAsync<TQuery, TResult>(TQuery domainQuery,
            CancellationToken token = default) where TQuery : class, IDomainQuery
        {
            if (domainQuery == null) throw new ArgumentNullException(nameof(domainQuery));
            var handler = _serviceProvider.GetRequiredService<IStreamDomainQueryHandler<TQuery, TResult>>();
            return handler.HandleStreamAsync(domainQuery, token);
        }
    }
}