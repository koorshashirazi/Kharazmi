using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Common;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Threading;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.Linq;

namespace Kharazmi.MongoDb
{
    public class MongoRepository<TEntity, TOptions> : IRepository<TEntity, TOptions>
        where TEntity : MongoEntity
        where TOptions : IMongoDbOptions<TOptions>
    {
        private readonly IMongoDbContext<TOptions> _dbContext;
        public string CollectionName { get; }
        public IMongoCollection<TEntity> DbSet { get; }
        public IMongoDatabase Database { get; }

        public MongoRepository(IMongoDbContext<TOptions> dbContext)
        {
            _dbContext = Ensure.IsNotNull(dbContext, nameof(dbContext));

            var tableAttribute = typeof(TEntity).GetCustomAttribute<TableAttribute>();
            CollectionName = tableAttribute != null && tableAttribute.Name.IsNotEmpty()
                ? tableAttribute.Name
                : typeof(TEntity).Name;

            DbSet = _dbContext.GetCollection<TEntity>(CollectionName);
            Database = _dbContext.Database();
        }


        #region Queries

        public IMongoQueryable<TEntity> Table => DbSet.AsQueryable();


        public TEntity FindById(string id)
        {
            return AsyncHelper.RunSync(() => FindByIdAsync(id));
        }

        public Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Table.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        }

        public TEntity FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return AsyncHelper.RunSync(() => FindByAsync(predicate));
        }

        public Task<TEntity> FindByAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return Table.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public bool Any()
        {
            return AsyncHelper.RunSync(() => AnyAsync(CancellationToken.None));
        }

        public bool Any(Expression<Func<TEntity, bool>> predication)
        {
            return AsyncHelper.RunSync(() => AnyAsync(predication));
        }

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return Table.AnyAsync(cancellationToken);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predication,
            CancellationToken cancellationToken = default)
        {
            return Table.AnyAsync(predication, cancellationToken);
        }

        public int Count()
        {
            return AsyncHelper.RunSync(() => CountAsync(CancellationToken.None));
        }

        public int Count(Expression<Func<TEntity, bool>> predication)
        {
            return AsyncHelper.RunSync(() => CountAsync(predication));
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Table.CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predication,
            CancellationToken cancellationToken = default)
        {
            return Table.CountAsync(predication, cancellationToken);
        }

        public IList<TEntity> GetBy(FilterDefinition<TEntity> query)
        {
            return DbSet.Find(query).ToList();
        }

        public List<TEntity> GetBy(Expression<Func<TEntity, bool>> query)
        {
            return AsyncHelper.RunSync(() => GetByAsync(query));
        }

        public IMongoQueryable<TEntity> GetByQuery(Expression<Func<TEntity, bool>> query)
        {
            return Table.Where(query);
        }

        public IFindFluent<TEntity, TEntity> GetFluent(Expression<Func<TEntity, bool>> query)
        {
            return DbSet.Find(query);
        }

        public Task<List<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> query,
            CancellationToken cancellationToken = default)
        {
            return Table.Where(query).ToListAsync(cancellationToken);
        }

        public PagedList<TEntity> PageBy<TOrderKey>(Expression<Func<TEntity, TOrderKey>> orderBy,
            Expression<Func<TEntity, bool>> predication = null, int pageSize = 10, int page = 1, bool isAsc = true)
        {
            return AsyncHelper.RunSync(() => PageByAsync(orderBy, predication, pageSize, page, isAsc));
        }

        public Task<PagedList<TEntity>> PageByAsync<TOrderKey>(
            Expression<Func<TEntity, TOrderKey>> orderBy = null,
            Expression<Func<TEntity, bool>> predication = null,
            int pageSize = 10, int page = 1, bool isAsc = true, CancellationToken cancellationToken = default)
        {
            var filter = Table;
            if (predication != null)
                filter = filter.Where(predication);

            var query = filter;

            if (orderBy != null)
                query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

            return query.PageByAsync(page, pageSize, cancellationToken: cancellationToken);
        }

        #endregion


        #region Commands

        public void Insert(TEntity entity)
        {
            AsyncHelper.RunSync(() => InsertAsync(entity, CancellationToken.None));
        }

        public void InsertTransaction(TEntity entity)
        {
            AsyncHelper.RunSync(() => InsertTransactionAsync(entity, CancellationToken.None));
        }

        public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(entity, nameof(entity));
            return _dbContext.AddCommandAsync(GetInsertKey(entity),
                () => DbSet.InsertOneAsync(entity, cancellationToken: cancellationToken));
        }

        public Task InsertTransactionAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(entity, nameof(entity));
            return _dbContext.AddCommandAsync(GetInsertKey(entity), () =>
                DbSet.InsertOneAsync(_dbContext.Session, entity, cancellationToken: cancellationToken));
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
            AsyncHelper.RunSync(() => InsertAsync(entities, CancellationToken.None));
        }

        public void InsertTransaction(IEnumerable<TEntity> entities)
        {
            AsyncHelper.RunSync(() => InsertTransactionAsync(entities, CancellationToken.None));
        }

        public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var list = entities.ToList();

            if (list.Count <= 0) return Task.CompletedTask;

            var key = list.Average(x => x.GetHashCode());

            return _dbContext.AddCommandAsync($"{key}",
                () => DbSet.InsertManyAsync(list, cancellationToken: cancellationToken));
        }

        public Task InsertTransactionAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var list = entities.ToList();

            if (list.Count <= 0) return Task.CompletedTask;

            var key = list.Average(x => x.GetHashCode());

            return _dbContext.AddCommandAsync($"{key}", () =>
                DbSet.InsertManyAsync(_dbContext.Session, list, cancellationToken: cancellationToken));
        }

        public void Update(TEntity entity)
        {
            AsyncHelper.RunSync(() => UpdateAsync(entity, CancellationToken.None));
        }

        public void UpdateTransaction(TEntity entity)
        {
            AsyncHelper.RunSync(() => UpdateTransactionAsync(entity, CancellationToken.None));
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(entity, nameof(entity));
            return _dbContext.AddCommandAsync(GetUpdateKey(entity), () =>
                DbSet.ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity,
                    new ReplaceOptions {IsUpsert = false}, cancellationToken: cancellationToken));
        }

        public Task UpdateTransactionAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return _dbContext.AddCommandAsync(GetUpdateKey(entity), () =>
                DbSet.ReplaceOneAsync(_dbContext.Session, x => x.Id.Equals(entity.Id), entity,
                    new ReplaceOptions {IsUpsert = false}, cancellationToken: cancellationToken));
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            AsyncHelper.RunSync(() => UpdateAsync(entities, CancellationToken.None));
        }

        public void FindAndUpdate(Expression<Func<TEntity, bool>> findQuery, UpdateDefinition<TEntity> updateQuery)
        {
            AsyncHelper.RunSync(() => FindAndUpdateAsync(findQuery, updateQuery, CancellationToken.None));
        }

        public async Task FindAndUpdateAsync(Expression<Func<TEntity, bool>> findQuery,
            UpdateDefinition<TEntity> updateQuery,
            CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(findQuery, nameof(findQuery));
            var entity = await FindByAsync(findQuery, cancellationToken).ConfigureAwait(false);
            if (entity == null)
                return;
            await _dbContext.AddCommandAsync(GetUpdateKey(entity), () => DbSet.FindOneAndUpdateAsync(findQuery,
                updateQuery, new FindOneAndUpdateOptions<TEntity>
                {
                    IsUpsert = false
                }, cancellationToken)).ConfigureAwait(false);
        }

        public async Task FindAndUpdateTransactionAsync(Expression<Func<TEntity, bool>> findQuery,
            UpdateDefinition<TEntity> updateQuery, CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(findQuery, nameof(findQuery));
            var entity = await FindByAsync(findQuery, cancellationToken).ConfigureAwait(false);
            if (entity == null)
                return;
            await _dbContext.AddCommandAsync(GetUpdateKey(entity), () => 
                DbSet.FindOneAndUpdateAsync(_dbContext.Session,findQuery,
                    updateQuery, new FindOneAndUpdateOptions<TEntity>
                    {
                        IsUpsert = false
                    }, cancellationToken)).ConfigureAwait(false);
        }

        public void UpdateTransaction(IEnumerable<TEntity> entities)
        {
            AsyncHelper.RunSync(() => UpdateTransactionAsync(entities, CancellationToken.None));
        }

        public async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task UpdateTransactionAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await UpdateTransactionAsync(entity, cancellationToken).ConfigureAwait(false);
            }
        }

        public void Delete(TEntity entity)
        {
            AsyncHelper.RunSync(() => DeleteAsync(entity, CancellationToken.None));
        }

        public void DeleteTransaction(TEntity entity)
        {
            AsyncHelper.RunSync(() => DeleteTransactionAsync(entity, CancellationToken.None));
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(entity, nameof(entity));
            return _dbContext.AddCommandAsync(GetDeleteKey(entity), () =>
                DbSet.DeleteOneAsync(x => x.Id.Equals(entity.Id), cancellationToken: cancellationToken));
        }

        public Task DeleteTransactionAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(entity, nameof(entity));
            return _dbContext.AddCommandAsync(GetDeleteKey(entity), () => DbSet.DeleteOneAsync(_dbContext.Session,
                x => x.Id.Equals(entity.Id),
                cancellationToken: cancellationToken));
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            AsyncHelper.RunSync(() => DeleteAsync(entities, CancellationToken.None));
        }

        public void DeleteTransaction(IEnumerable<TEntity> entities)
        {
            AsyncHelper.RunSync(() => DeleteTransactionAsync(entities, CancellationToken.None));
        }

        public async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task DeleteTransactionAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await DeleteTransactionAsync(entity, cancellationToken).ConfigureAwait(false);
            }
        }

        public void FindAndDelete(Expression<Func<TEntity, bool>> findQuery)
        {
            AsyncHelper.RunSync(() => FindAndDeleteAsync(findQuery, CancellationToken.None));
        }

        public async Task FindAndDeleteAsync(Expression<Func<TEntity, bool>> findQuery,
            CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(findQuery, nameof(findQuery));
            var entity = await FindByAsync(findQuery, cancellationToken).ConfigureAwait(false);
            if (entity == null)
                return;
            await DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task FindAndDeleteTransactionAsync(Expression<Func<TEntity, bool>> findQuery,
            CancellationToken cancellationToken = default)
        {
            Ensure.IsNotNull(findQuery, nameof(findQuery));
            var entity = await FindByAsync(findQuery, cancellationToken).ConfigureAwait(false);
            if (entity == null)
                return;
            await DeleteTransactionAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        private static string GetInsertKey(TEntity entity) => $"Insert_{entity.GetHashCode()}";

        private static string GetDeleteKey(TEntity entity) =>
            entity == null ? $"Delete_{Guid.NewGuid():N}" : $"Delete_{entity.GetHashCode()}";

        private static string GetUpdateKey(TEntity entity) =>
            entity == null ? $"Update_{Guid.NewGuid():N}" : $"Update_{entity.GetHashCode()}";
    }
}