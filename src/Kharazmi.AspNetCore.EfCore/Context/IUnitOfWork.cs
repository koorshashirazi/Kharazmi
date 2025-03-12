using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Dependency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kharazmi.AspNetCore.EFCore.Context
{
    // TODO:
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        DatabaseFacade Database { get; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWork : IDisposable, IScopedDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <typeparam name="TEntity"></typeparam>
        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <typeparam name="TEntity"></typeparam>
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        void MarkAsCreated<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        void MarkAsUnChanged<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        void DisconnectEntity<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        void DisconnectEntities();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object GetShadowPropertyValue(object entity, string propertyName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        /// <typeparam name="TEntity"></typeparam>
        void TrackGraph<TEntity>(TEntity entity, Action<EntityEntryGraphNode> callback) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
     
        //int ExecuteSqlInterpolatedCommand(FormattableString query);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteSqlRawCommand(string query, params object[] parameters);
        //Task<int> ExecuteSqlInterpolatedCommandAsync(FormattableString query);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlRawCommandAsync(string query, params object[] parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        void UseTransaction(DbTransaction transaction);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        void UseConnectionString(string connectionString);
        /// <summary>
        /// 
        /// </summary>
        bool HasTransaction { get; }
        /// <summary>
        /// 
        /// </summary>
        DbConnection Connection { get; }
        /// <summary>
        /// 
        /// </summary>
        IDbContextTransaction Transaction { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// 
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 
        /// </summary>
        void RollbackTransaction();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hookName"></param>
        void IgnoreHook(string hookName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string EntityHash<TEntity>(TEntity entity) where TEntity : class;
    }
}