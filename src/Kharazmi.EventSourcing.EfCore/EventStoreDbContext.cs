using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.EventSourcing.EfCore
{
    /// <summary> </summary>
    public interface IEventStoreDbContext:  IDisposable
    {
        /// <summary></summary>
        DbSet<EventStoreEntity> EventStores { get; set; }

        Task BeginTransactionAsync(CancellationToken token = default);

        /// <summary> </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary> </summary>
    public class EventStoreDbContext : DbContext, IEventStoreDbContext
    {
        /// <summary></summary>
        public DbSet<EventStoreEntity> EventStores { get; set; }
        
        /// <summary></summary>
        /// <param name="options"></param>
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
        {
        }

        /// <summary></summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyStoredEventConfiguration();
            base.OnModelCreating(modelBuilder);
        }

        public Task BeginTransactionAsync(CancellationToken token = default)
        {
            return Database.BeginTransactionAsync(token);
        }
    }
}