using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Queries;
using Kharazmi.AspNetCore.Core.Handlers;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, DomainContext domainContext,
            CancellationToken cancellationToken = default);
    }

    internal class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceFactory;

        public QueryDispatcher(IServiceProvider serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IQueryHandler<,>)
                .MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceFactory.GetService(handlerType);

            return await handler.HandleAsync((dynamic) query, domainContext, cancellationToken);
        }
    }
}