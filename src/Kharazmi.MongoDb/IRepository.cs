using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Kharazmi.MongoDb
{
    public interface IRepository<TEntity, TOptions>
        where TEntity : MongoEntity
        where TOptions : IMongoDbOptions<TOptions>
    {
        string CollectionName { get; }
        IMongoCollection<TEntity> DbSet { get; }

        IMongoDatabase Database { get; }

        #region Query

        /// <summary>
        /// Gets a table
        /// </summary>
        IMongoQueryable<TEntity> Table { get; }

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        TEntity FindById(string id);

        /// <summary>
        /// Get async entity by id 
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Entity</returns>
        Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// find entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Entity</returns>
        TEntity FindBy(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// fin async entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Entity</returns>
        Task<TEntity> FindByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether a list contains any elements
        /// </summary>
        /// <returns></returns>
        bool Any();

        /// <summary>
        /// Determines whether any element of a list satisfies a condition.
        /// </summary>
        /// <param name="predication"></param>
        /// <returns></returns>
        bool Any(Expression<Func<TEntity, bool>> predication);

        /// <summary>
        /// Async determines whether a list contains any elements
        /// </summary>
        /// <returns></returns>
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Async determines whether any element of a list satisfies a condition.
        /// </summary>
        /// <param name="predication"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predication, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the number of elements in the specified sequence.
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Returns the number of elements in the specified sequence that satisfies a condition.
        /// </summary>
        /// <param name="predication"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predication);

        /// <summary>
        /// Async returns the number of elements in the specified sequence
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync( CancellationToken cancellationToken = default);

        /// <summary>
        /// Async returns the number of elements in the specified sequence that satisfies a condition.
        /// </summary>
        /// <param name="predication"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predication, CancellationToken cancellationToken = default);


        /// <summary>
        /// Get collection by filter definitions
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList<TEntity> GetBy(FilterDefinition<TEntity> query);

        /// <summary>
        /// Get collection by filter expression
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<TEntity> GetBy(Expression<Func<TEntity, bool>> query);

        /// <summary>
        /// Get collection by filter expression
        /// </summary>
        /// <param name="query"></param>
        /// <returns>IMongoQueryable</returns>
        IMongoQueryable<TEntity> GetByQuery(Expression<Func<TEntity, bool>> query);

        /// <summary>
        /// Get collection by filter expression
        /// </summary>
        /// <param name="query"></param>
        /// <returns>IFindFluent</returns>
        IFindFluent<TEntity, TEntity> GetFluent(Expression<Func<TEntity, bool>> query);


        /// <summary>
        /// et collection by filter expression
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Return PagedList as paged
        /// </summary>
        /// <param name="predication"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize">default is 10</param>
        /// <param name="page">Default is 1</param>
        /// <param name="isAsc">Default is true</param>
        /// <typeparam name="TOrderKey"></typeparam>
        /// <returns></returns>
        PagedList<TEntity> PageBy<TOrderKey>(
            Expression<Func<TEntity, TOrderKey>> orderBy = null,
            Expression<Func<TEntity, bool>> predication = null,
            int pageSize = 10, int page = 1, bool isAsc = true);

        /// <summary>
        /// Return PagedList as paged
        /// </summary>
        /// <param name="predication"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize">default is 10</param>
        /// <param name="page">Default is 1</param>
        /// <param name="isAsc">Default is true</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TOrderKey"></typeparam>
        /// <returns></returns>
        Task<PagedList<TEntity>> PageByAsync<TOrderKey>(
            Expression<Func<TEntity, TOrderKey>> orderBy = null,
            Expression<Func<TEntity, bool>> predication = null,
            int pageSize = 10, int page = 1, bool isAsc = true, CancellationToken cancellationToken = default);

        #endregion

        #region Commands

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Insert entity with transaction session scope
        /// </summary>
        /// <param name="entity">Entity</param>
        void InsertTransaction(TEntity entity);

        /// <summary>
        /// Async Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken"></param>
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async Insert entity with transaction session scope
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken"></param>
        Task InsertTransactionAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Insert entities with transaction session scope
        /// </summary>
        /// <param name="entities">Entities</param>
        void InsertTransaction(IEnumerable<TEntity> entities);

        /// <summary>
        /// Async Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="cancellationToken"></param>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async Insert entities with transaction session scope
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="cancellationToken"></param>
        Task InsertTransactionAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Update entity  with transaction session scope
        /// </summary>
        /// <param name="entity">Entity</param>
        void UpdateTransaction(TEntity entity);

        /// <summary>
        /// Async Update entity 
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken"></param>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async Update entity with transaction session scope
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken"></param>
        Task UpdateTransactionAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update entities 
        /// </summary>
        /// <param name="entities">Entities</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Find one and update
        /// </summary>
        /// <param name="findQuery"></param>
        /// <param name="updateQuery"></param>
        void FindAndUpdate(Expression<Func<TEntity, bool>> findQuery, UpdateDefinition<TEntity> updateQuery);

        /// <summary>
        /// Find one and update
        /// </summary>
        /// <param name="findQuery"></param>
        /// <param name="updateQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task FindAndUpdateAsync(Expression<Func<TEntity, bool>> findQuery, UpdateDefinition<TEntity> updateQuery,
            CancellationToken cancellationToken = default);

        Task FindAndUpdateTransactionAsync(Expression<Func<TEntity, bool>> findQuery,
            UpdateDefinition<TEntity> updateQuery, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update entities with transaction session scope
        /// </summary>
        /// <param name="entities">Entities</param>
        void UpdateTransaction(IEnumerable<TEntity> entities);

        /// <summary>
        /// Async Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="cancellationToken"></param>
        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async Update entities with transaction session scope
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="cancellationToken"></param>
        Task UpdateTransactionAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete entity with transaction session scope
        /// </summary>
        /// <param name="entity">Entity</param>
        void DeleteTransaction(TEntity entity);

        /// <summary>
        /// Async Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken"></param>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async Delete entity with transaction session scope
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken"></param>
        Task DeleteTransactionAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete entities 
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entities with transaction session scope
        /// </summary>
        /// <param name="entities">Entities</param>
        void DeleteTransaction(IEnumerable<TEntity> entities);

        /// <summary>
        /// Async Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="cancellationToken"></param>
        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async Delete entities with transaction session scope
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="cancellationToken"></param>
        Task DeleteTransactionAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void FindAndDelete(Expression<Func<TEntity, bool>> findQuery);

        Task FindAndDeleteAsync(Expression<Func<TEntity, bool>> findQuery,
            CancellationToken cancellationToken = default);

        Task FindAndDeleteTransactionAsync(Expression<Func<TEntity, bool>> findQuery,
            CancellationToken cancellationToken = default);

        #endregion
    }
}