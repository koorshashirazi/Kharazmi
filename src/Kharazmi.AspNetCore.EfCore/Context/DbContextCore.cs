using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.EFCore.Context.Extensions;
using Kharazmi.AspNetCore.EFCore.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using DbException = Kharazmi.AspNetCore.Core.Exceptions.DbException;

namespace Kharazmi.AspNetCore.EFCore.Context
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class DbContextCore : DbContext, IUnitOfWork
    {
        private IEnumerable<IHook> _hooks;
        private readonly List<string> _ignoredHookList = new List<string>();

        public bool ActiveChangeTracker { get; set; }

        protected DbContextCore(DbContextOptions options, IEnumerable<IHook> hooks) : base(options)
        {
            _hooks = hooks ?? throw new ArgumentNullException(nameof(hooks));
            if (!ActiveChangeTracker) return;
            ChangeTracker.StateChanged += StateChanged;
            ChangeTracker.Tracked += Tracked;

        }

        public DbConnection Connection => Database.GetDbConnection();
        public bool HasTransaction => Transaction != null;
        public IDbContextTransaction Transaction { get; private set; }

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (HasTransaction) return Transaction;

            return Transaction = Database.BeginTransaction(isolationLevel);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (Transaction != null) return null;

            return Transaction = await Database.BeginTransactionAsync(isolationLevel).ConfigureAwait(false);
        }

        public void CommitTransaction()
        {
            if (!HasTransaction) throw new NullReferenceException("Please call `BeginTransaction()` method first.");

            try
            {
                Transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (Transaction != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            if (!HasTransaction) throw new NullReferenceException("Please call `BeginTransaction()` method first.");

            try
            {
                Transaction.Rollback();
            }
            finally
            {
                if (Transaction != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
        }

        public void IgnoreHook(string hookName)
        {
            _ignoredHookList.Add(hookName);
        }

        public void UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
        }

        public void UseConnectionString(string connectionString)
        {
            Database.GetDbConnection().ConnectionString = connectionString;
        }

        public void TrackGraph<TEntity>(TEntity entity, Action<EntityEntryGraphNode> callback) where TEntity : class
        {
            ChangeTracker.TrackGraph(entity, callback);
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Set<TEntity>().AddRange(entities);
        }

        public T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
        {
            var value = Entry(entity).Property(propertyName).CurrentValue;
            return value != null
                ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                : default;
        }

        public object GetShadowPropertyValue(object entity, string propertyName)
        {
            return Entry(entity).Property(propertyName).CurrentValue;
        }

        public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) return;
            var dbEntityEntry = Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
                Attach(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public void MarkAsUnChanged<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) return;
            var dbEntityEntry = Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
                Attach(entity);
            dbEntityEntry.State = EntityState.Unchanged;
        }

        public void MarkAsCreated<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) return;
            var dbEntityEntry = Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
                Attach(entity);
            dbEntityEntry.State = EntityState.Added;
        }

        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) return;
            var dbEntityEntry = Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
                Attach(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public void DisconnectEntity<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) return;
            var dbEntityEntry = Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached) return;
            if (Set<TEntity>().Local.Contains(entity))
                Set<TEntity>().AsNoTracking();
            dbEntityEntry.State = EntityState.Detached;
        }

        public virtual void DisconnectEntities()
        {
            foreach (var entityEntry in ChangeTracker.Entries().ToArray())
            {
                if (entityEntry.Entity != null)
                {
                    entityEntry.State = EntityState.Detached;
                }
            }
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Set<TEntity>().RemoveRange(entities);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result;
            try
            {
                var entryList = this.FindChangedEntries();
                var names = entryList.FindEntityNames();

                BeforeSaveTriggers();

                ExecuteHooks<IPreActionHook>(entryList);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = await base.SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
                ChangeTracker.AutoDetectChangesEnabled = true;

                ExecuteHooks<IPostActionHook>(entryList);

                //for RowIntegrity scenarios
                await base.SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);

                OnSaveCompleted(new EntityChangeContext(names, entryList));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message, e);
            }
            catch (DbUpdateException e)
            {
                throw new DbException(e.Message, e);
            }

            return result;
        }

        public override int SaveChanges()
        {
            int result;
            try
            {
                var entryList = this.FindChangedEntries();
                var names = entryList.FindEntityNames();
                BeforeSaveTriggers();
                ExecuteHooks<IPreActionHook>(entryList);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = base.SaveChanges(true);
                ChangeTracker.AutoDetectChangesEnabled = true;

                ExecuteHooks<IPostActionHook>(entryList);

              
                base.SaveChanges(true);

                OnSaveCompleted(new EntityChangeContext(names, entryList));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message, e);
            }
            catch (DbUpdateException e)
            {
                throw new DbException(e.Message, e);
            }

            return result;
        }

        public int ExecuteSqlInterpolatedCommand(FormattableString query)
        {
            return Database.ExecuteSqlInterpolated(query);
        }
        public int ExecuteSqlRawCommand(string query, params object[] parameters)
        {
            return Database.ExecuteSqlRaw(query, parameters);
        }

        public Task<int> ExecuteSqlInterpolatedCommandAsync(FormattableString query)
        {
            return Database.ExecuteSqlInterpolatedAsync(query);
        }
        public Task<int> ExecuteSqlRawCommandAsync(string query, params object[] parameters)
        {
            return Database.ExecuteSqlRawAsync(query, parameters);
        }
        public string EntityHash<TEntity>(TEntity entity) where TEntity : class
        {
            var row = Entry(entity).ToDictionary(p => p.Metadata.Name != EFCore.Hash &&
                                                      !p.Metadata.ValueGenerated.HasFlag(ValueGenerated.OnUpdate) &&
                                                      !p.Metadata.IsShadowProperty());
            return EntityHash<TEntity>(row);
        }

        protected virtual string EntityHash<TEntity>(Dictionary<string, object> row) where TEntity : class
        {
            var json = JsonConvert.SerializeObject(row, Formatting.Indented);
            using var hashAlgorithm = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(json);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
        /// <summary>
        /// For RowIntegrity scenarios
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnSaveCompleted(EntityChangeContext context)
        {
        }

        protected virtual void ExecuteHooks<THook>(IEnumerable<EntityEntry> entryList) where THook : IHook
        {
            _hooks = this.GetService<IEnumerable<IHook>>().ToList();
            foreach (var entry in entryList)
            {
                var hooks = _hooks.OfType<THook>()
                    .Where(hook => !_ignoredHookList.Contains(hook.Name))
                    .Where(x => x.HookState == entry.State)
                    .OrderBy(hook => hook.Order);
                foreach (var hook in hooks)
                {
                    var metadata = new HookEntityMetadata(entry);
                    hook.Hook(entry.Entity, metadata, this);
                }
            }
        }

        protected virtual void BeforeSaveTriggers()
        {
        }

        protected virtual void StateChangedLogger(string message)
        {
            Console.WriteLine(message);
        }

        protected virtual void StateChanged(object sender, EntityStateChangedEventArgs e)
        {
            var idValue = GetKeyValue(e.Entry.Entity);
            StateChangedLogger(
                $"State of {e.Entry.Entity.GetType()} with Id={idValue} changed from {e.OldState} to {e.NewState}");
        }

        protected virtual void Tracked(object sender, EntityTrackedEventArgs e)
        {
            var idValue = GetKeyValue(e.Entry.Entity);
            StateChangedLogger($"Newly tracked {e.Entry.Entity.GetType()} with Id={idValue} as {e.Entry.State}");
        }

        private string GetKeyValue<T>(T entity)
        {
            if (entity == null) return string.Empty;
            var keyName = Model.FindEntityType(entity.GetType()).FindPrimaryKey().Properties
                .Select(x => x.Name).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(keyName)) return string.Empty;
            var value = entity.GetType().GetProperty(keyName)?.GetValue(entity, null);
            return value == null ? string.Empty : value.ToString();
        }
    }
}