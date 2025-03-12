using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Application.Models;
using Kharazmi.AspNetCore.Core.Application.Services;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Mapping;
using Kharazmi.AspNetCore.Core.Transaction;
using Kharazmi.AspNetCore.Core.Validation;
using Kharazmi.AspNetCore.EFCore.Context;
using Kharazmi.AspNetCore.EFCore.Context.Extensions;
using Kharazmi.AspNetCore.EFCore.Linq;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.EFCore.Application
{
    public abstract class CrudService<TEntity, TKey, TModel> :
        CrudService<TEntity, TKey, TModel, TModel>,
        ICrudService<TKey, TModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected CrudService(IUnitOfWork uow, IEventDispatcher busManager) : base(uow, busManager)
        {
        }
    }

    public abstract class CrudService<TEntity, TKey, TReadModel, TModel> :
        CrudService<TEntity, TKey, TReadModel, TModel, FilteredPagedQueryModel>,
        ICrudService<TKey, TReadModel, TModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected CrudService(IUnitOfWork uow, IEventDispatcher busManager) : base(uow, busManager)
        {
        }
    }

    public abstract class CrudService<TEntity, TKey, TReadModel, TModel,
        TFilteredPagedQueryModel> : ApplicationService,
        ICrudService<TKey, TReadModel, TModel, TFilteredPagedQueryModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel
        where TKey : IEquatable<TKey>
    {
        protected readonly DbSet<TEntity> EntitySet;
        protected readonly IEventDispatcher EventBusManager;
        protected readonly IUnitOfWork UnitOfWork;

        protected CrudService(IUnitOfWork uow, IEventDispatcher busManager)
        {
            UnitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
            EventBusManager = busManager ?? throw new ArgumentNullException(nameof(busManager));
            EntitySet = UnitOfWork.Set<TEntity>();
        }

        [SkipValidation]
        public async Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel model)
        {
            var result = await BuildReadQuery(model).ToPagedQueryResultAsync(model).ConfigureAwait(false);

            await AfterReadAsync(result).ConfigureAwait(false);

            return result;
        }

        public async Task<Maybe<TModel>> FindAsync(TKey id)
        {
            var models = await FindAsync(BuildEqualityExpressionForId(id)).ConfigureAwait(false);

            return models.SingleOrDefault();
        }

        public Task<IReadOnlyList<TModel>> FindListAsync(IEnumerable<TKey> ids)
        {
            return FindAsync(entity => ids.Contains(entity.Id));
        }

        public Task<IReadOnlyList<TModel>> FindListAsync()
        {
            return FindAsync(_ => true);
        }

        [SkipValidation]
        public async Task<IPagedQueryResult<TModel>> FindPagedListAsync(PagedQueryModel model)
        {
            var pagedList = await BuildFindQuery().ToPagedQueryResultAsync(model).ConfigureAwait(false);

            var result = new PagedQueryResult<TModel>
            {
                Items = pagedList.Items.MapReadOnlyList(MapToModel),
                TotalCount = pagedList.TotalCount
            };

            await AfterFindAsync(result.Items).ConfigureAwait(false);

            return result;
        }

        [Transactional]
        public Task<Result> CreateAsync(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return CreateAsync(new[] {model});
        }

        [Transactional]
        public async Task<Result> CreateAsync(IEnumerable<TModel> models)
        {
            var modelList = models.ToList();

            var result = await BeforeCreateAsync(modelList).ConfigureAwait(false);
            if (result.Failed) return result;

            var entityList = modelList.MapReadOnlyList<TModel, TEntity>(MapToEntity);

            await AfterMappingAsync(modelList, entityList).ConfigureAwait(false);

            await EventBusManager.RaiseCreatingEventAsync<TModel, TKey>(modelList).ConfigureAwait(false);

            EntitySet.AddRange(entityList);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            UnitOfWork.MarkUnchanged(entityList);

            MapToModel(entityList, modelList);

            result = await AfterCreateAsync(modelList).ConfigureAwait(false);
            if (result.Failed) return result;

            await EventBusManager.RaiseCreatedEventAsync<TModel, TKey>(modelList).ConfigureAwait(false);

            return result;
        }

        [Transactional]
        public Task<Result> EditAsync(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return EditAsync(new[] {model});
        }

        [Transactional]
        public async Task<Result> EditAsync(IEnumerable<TModel> models)
        {
            var modelList = models.ToList();

            var ids = modelList.Select(m => m.Id).ToList();
            var entityList = await BuildFindQuery().Where(e => ids.Contains(e.Id)).ToListAsync().ConfigureAwait(false);

            var modifiedList = BuildModifiedModel(modelList, entityList);

            var result = await BeforeEditAsync(modifiedList, entityList).ConfigureAwait(false);
            if (result.Failed) return result;

            MapToEntity(modelList, entityList);

            await AfterMappingAsync(modelList, entityList).ConfigureAwait(false);

            await EventBusManager.RaiseEditingEventAsync<TModel, TKey>(modifiedList).ConfigureAwait(false);

            UnitOfWork.TrackChanges(entityList);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            UnitOfWork.MarkUnchanged(entityList);

            MapToModel(entityList, modelList);

            result = await AfterEditAsync(modifiedList, entityList).ConfigureAwait(false);
            if (result.Failed) return result;

            await EventBusManager.RaiseEditedEventAsync<TModel, TKey>(modifiedList).ConfigureAwait(false);

            return result;
        }

        [Transactional]
        [SkipValidation]
        public Task<Result> DeleteAsync(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return DeleteAsync(new[] {model});
        }

        [Transactional]
        [SkipValidation]
        public virtual async Task<Result> DeleteAsync(IEnumerable<TModel> models)
        {
            var modelList = models.ToList();

            var result = await BeforeDeleteAsync(modelList).ConfigureAwait(false);
            if (result.Failed) return result;

            var entityList = modelList.MapReadOnlyList<TModel, TEntity>(MapToEntity);

            await EventBusManager.RaiseDeletingEventAsync<TModel, TKey>(modelList).ConfigureAwait(false);

            EntitySet.RemoveRange(entityList);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            result = await AfterDeleteAsync(modelList).ConfigureAwait(false);
            if (result.Failed) return result;

            await EventBusManager.RaiseDeletedEventAsync<TModel, TKey>(modelList, CancellationToken.None).ConfigureAwait(false);

            return result;
        }

        [Transactional]
        [SkipValidation]
        public async Task<Result> DeleteAsync(TKey id)
        {
            var model = await FindAsync(id).ConfigureAwait(false);
            if (model.HasValue) return await DeleteAsync(model.Value).ConfigureAwait(false);

            return Ok();
        }

        [Transactional]
        [SkipValidation]
        public async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await FindListAsync(ids).ConfigureAwait(false);
            if (models.Any()) return await DeleteAsync(models).ConfigureAwait(false);

            return Ok();
        }

        public Task<bool> ExistsAsync(TKey id)
        {
            return EntitySet.AnyAsync(BuildEqualityExpressionForId(id));
        }

        protected abstract IQueryable<TReadModel> BuildReadQuery(TFilteredPagedQueryModel model);

        protected async Task<IReadOnlyList<TModel>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entityList = await BuildFindQuery().Where(predicate).ToListAsync().ConfigureAwait(false);

            var modelList = entityList.MapReadOnlyList(MapToModel);

            await AfterFindAsync(modelList).ConfigureAwait(false);

            return modelList;
        }

        protected virtual IQueryable<TEntity> BuildFindQuery()
        {
            return EntitySet.AsNoTracking();
        }

        protected virtual Task AfterReadAsync(PagedQueryResult<TReadModel> result)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterFindAsync(IReadOnlyList<TModel> models)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterMappingAsync(IReadOnlyList<TModel> models, IReadOnlyList<TEntity> entities)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<Result> BeforeCreateAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterCreateAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> BeforeEditAsync(
            IReadOnlyList<ModifiedModel<TModel>> models, IReadOnlyList<TEntity> entities)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterEditAsync(
            IReadOnlyList<ModifiedModel<TModel>> models, IReadOnlyList<TEntity> entities)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> BeforeDeleteAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterDeleteAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected abstract void MapToEntity(TModel model, TEntity entity);

        protected abstract TModel MapToModel(TEntity entity);

        private IReadOnlyList<ModifiedModel<TModel>> BuildModifiedModel(IReadOnlyCollection<TModel> models,
            IReadOnlyCollection<TEntity> entities)
        {
            if (models.Count != entities.Count) throw new DbConcurrencyException();

            var modelList = entities.MapReadOnlyList(MapToModel);
            var modelDictionary = modelList.ToDictionary(e => e.Id);

            var result = models.Select(
                model => new ModifiedModel<TModel>
                    {NewValue = model, OriginalValue = modelDictionary[model.Id]}).ToList();

            return result;
        }

        private void MapToModel(IReadOnlyList<TEntity> entities, IEnumerable<TModel> models)
        {
            var i = 0;
            foreach (var model in models)
            {
                var entity = entities[i++];
                var m = MapToModel(entity);

                var properties = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite).ToList();
                foreach (var property in properties) property.SetValue(model, property.GetValue(m));
            }
        }

        private void MapToEntity(IEnumerable<TModel> models, IReadOnlyList<TEntity> entities)
        {
            var i = 0;
            foreach (var model in models)
            {
                var entity = entities[i++];
                MapToEntity(model, entity);
            }
        }

        private Expression<Func<TEntity, bool>> BuildEqualityExpressionForId(TKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, nameof(Entity<TKey>.Id)),
                Expression.Constant(id, typeof(TKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}