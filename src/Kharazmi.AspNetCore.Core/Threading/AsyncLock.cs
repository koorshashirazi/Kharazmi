using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kharazmi.AspNetCore.Core.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AsyncLock : IDisposable
    {
        private sealed class AsyncLockReleaser : IDisposable
        {
            private readonly AsyncLock _asyncLock;

            public AsyncLockReleaser(AsyncLock asyncLock)
            {
                _asyncLock = asyncLock;
            }

            public void Dispose()
            {
                _asyncLock.Release();
            }
        }

        private readonly Task<IDisposable> _completed;
        private readonly AsyncLockReleaser _releaser;
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCount"></param>
        public AsyncLock(int initialCount = 1)
        {
            _semaphore = new SemaphoreSlim(initialCount);
            _releaser = new AsyncLockReleaser(this);
            _completed = Task.FromResult<IDisposable>(_releaser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IDisposable> WaitAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.Run(() => null as IDisposable, cancellationToken);
            }

            // ReSharper disable once MethodSupportsCancellation
            var task = _semaphore.WaitAsync();

            if (task.Status == TaskStatus.RanToCompletion)
            {
                return _completed;
            }

            if (!cancellationToken.CanBeCanceled)
            {
                return task.ContinueWith(
                    (_, s) => (IDisposable) s,
                    _releaser,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }

            var taskCompletionSource = new TaskCompletionSource<IDisposable>();
            var registration = cancellationToken.Register(
                () =>
                {
                    if (taskCompletionSource.TrySetCanceled())
                    {
                        task.ContinueWith(
                            (_, s) => ((SemaphoreSlim) s).Release(),
                            _semaphore,
                            CancellationToken.None,
                            TaskContinuationOptions.ExecuteSynchronously,
                            TaskScheduler.Default);
                    }
                });

            task.ContinueWith(
                _ =>
                {
                    if (taskCompletionSource.TrySetResult(_releaser))
                    {
                        registration.Dispose();
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Release()
        {
            _semaphore.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IDisposable Wait(CancellationToken cancellationToken)
        {
            _semaphore.Wait(cancellationToken);
            return _releaser;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}