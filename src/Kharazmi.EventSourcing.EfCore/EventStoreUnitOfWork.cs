using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.EventSourcing.EfCore
{
    public interface IEventStoreUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken token = default);
        Task CommitAsync(CancellationToken token = default);
    }

    public sealed class EventStoreUnitOfWork : IEventStoreUnitOfWork
    {
        private readonly IEventStoreDbContext _databaseContext;

        private bool _isDisposed;

        public EventStoreUnitOfWork(IEventStoreDbContext databaseContext)
        {
            _databaseContext = Ensure.ArgumentIsNotNull(databaseContext, nameof(databaseContext));
        }

        public Task BeginTransactionAsync(CancellationToken token = default)
        {
            return _databaseContext.BeginTransactionAsync(token);
        }

        public Task CommitAsync(CancellationToken token = default)
        {
            return _databaseContext.SaveChangesAsync(token);
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _databaseContext.Dispose();
            _isDisposed = true;
        }
    }
}