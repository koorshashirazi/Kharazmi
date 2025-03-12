using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Cqrs.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        IHandler Handle(Func<Task> handle);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        IHandler OnSuccess(Func<Task> onSuccess);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        IHandler OnError(Func<Exception, Task> onError, bool rethrow = false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onCustomError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        IHandler OnCustomError(Func<FrameworkException, Task> onCustomError, bool rethrow = false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="always"></param>
        /// <returns></returns>
        IHandler Always(Func<Task> always);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class Handler : IHandler
    {
        private Func<Task> _handle;
        private Func<Task> _onSuccess;
        private Func<Task> _always;
        private Func<Exception, Task> _onError;
        private Func<FrameworkException, Task> _onCustomError;
        private bool _rethrowException;
        private bool _rethrowCustomException;

        /// <summary>
        /// 
        /// </summary>
        public Handler()
        {
            _always = () => Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public IHandler Handle(Func<Task> handle)
        {
            _handle = handle;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public IHandler OnSuccess(Func<Task> onSuccess)
        {
            _onSuccess = onSuccess;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="always"></param>
        /// <returns></returns>
        public IHandler Always(Func<Task> always)
        {
            _always = always;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        public IHandler OnError(Func<Exception, Task> onError, bool rethrow = false)
        {
            _onError = onError;
            _rethrowException = rethrow;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onCustomError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        public IHandler OnCustomError(Func<FrameworkException, Task> onCustomError, bool rethrow = false)
        {
            _onCustomError = onCustomError;
            _rethrowCustomException = rethrow;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteAsync()
        {
            bool isFailure = false;

            try
            {
                await _handle().ConfigureAwait(false);
            }
            catch (FrameworkException customException)
            {
                isFailure = true;
                await (_onCustomError?.Invoke(customException)).ConfigureAwait(false);
                if (_rethrowCustomException) 
                {
                    throw;
                }
            }
            catch (Exception exception)
            {
                isFailure = true;
                await (_onError?.Invoke(exception)).ConfigureAwait(false);
                if (_rethrowException) 
                {
                    throw;
                }
            }
            finally
            {
                if (!isFailure)
                {
                    await (_onSuccess?.Invoke()).ConfigureAwait(false);
                }
                await (_always?.Invoke()).ConfigureAwait(false);
            }
        }
    }
}