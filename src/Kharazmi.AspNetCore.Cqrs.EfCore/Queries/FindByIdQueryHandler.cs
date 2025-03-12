using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Kharazmi.AspNetCore.Cqrs.Handlers;
using Kharazmi.AspNetCore.EFCore.Context;

namespace Kharazmi.AspNetCore.EfCore.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TReadModel"></typeparam>
    public class FindByIdQueryHandler<TKey, TEntity, TReadModel> : IQueryHandler<FindByIdQuery<TKey, TEntity, TReadModel>, TReadModel>
        where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>
    {
        private readonly IUnitOfWork _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public FindByIdQueryHandler(IUnitOfWork context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<TReadModel> Handle(FindByIdQuery<TKey, TEntity, TReadModel> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
