using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Cqrs.Messages;
using Kharazmi.AspNetCore.EFCore.Context;
using MediatR;

namespace Kharazmi.AspNetCore.EfCore.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : DomainCommand
    {
        private readonly IUnitOfWork _uow;
        private readonly IPipelineBehavior<TRequest, TResponse> _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        public TransactionBehavior(IUnitOfWork uow, IPipelineBehavior<TRequest, TResponse> next)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await _next.Handle(request, next, cancellationToken).ConfigureAwait(false);

            _uow.CommitTransaction();

            return response;
        }
    }
}