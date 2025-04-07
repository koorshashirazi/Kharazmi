using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    public interface IDomainQueryHandler<in TQuery, TResult>
        where TQuery : IDomainQuery
    {
        Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken token = default);
    }

    public interface IStreamDomainQueryHandler<in TQuery, out TResult>
        where TQuery : IDomainQuery
    {
        IAsyncEnumerable<TResult> HandleStreamAsync(TQuery query, CancellationToken token = default);
    }

    public abstract class DomainQueryHandler<TQuery, TResult> : IDomainQueryHandler<TQuery, TResult>
        where TQuery : class, IDomainQuery
    {
        public abstract Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken token = default);
    }

    public abstract class StreamQueryHandler<TQuery, TResult> : IStreamDomainQueryHandler<TQuery, TResult>
        where TQuery : class, IDomainQuery
    {
        public abstract IAsyncEnumerable<TResult> HandleStreamAsync(TQuery query, CancellationToken token = default);
    }
}