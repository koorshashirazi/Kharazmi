using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.MessageBroker
{
    /// <summary>
    /// 
    /// </summary>
    public class BusHandler : IBusHandler
    {
        private Func<Task> _handle;
        private Func<Task> _onSuccess;
        private Func<Task> _always;
        private Func<Exception, Task> _onError;
        private Func<MessageBusException, Task> _onCustomError;
        private bool _rethrowException;
        private bool _rethrowCustomException;

        /// <summary> </summary>
        public BusHandler()
        {
            _always = () => Task.CompletedTask;
        }

        /// <summary> </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public IBusHandler Handle(Func<Task> handle)
        {
            _handle = handle;
            return this;
        }

        /// <summary></summary>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public IBusHandler OnSuccess(Func<Task> onSuccess)
        {
            _onSuccess = onSuccess;
            return this;
        }

        /// <summary> </summary>
        /// <param name="always"></param>
        /// <returns></returns>
        public IBusHandler Always(Func<Task> always)
        {
            _always = always;
            return this;
        }

        /// <summary> </summary>
        /// <param name="onError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        public IBusHandler OnError(Func<Exception, Task> onError, bool rethrow = false)
        {
            _onError = onError;
            _rethrowException = rethrow;
            return this;
        }

        /// <summary></summary>
        /// <param name="onCustomError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        public IBusHandler OnCustomError(Func<MessageBusException, Task> onCustomError, bool rethrow = false)
        {
            _onCustomError = onCustomError;
            _rethrowCustomException = rethrow;
            return this;
        }

        /// <summary> </summary>
        /// <returns></returns>
        public async Task ExecuteAsync()
        {
            var isFailure = false;

            try
            {
                await _handle().ConfigureAwait(false);
            }
            catch (MessageBusException customException)
            {
                isFailure = true;
                if (_onCustomError != null) await _onCustomError.Invoke(customException).ConfigureAwait(false);
                if (_rethrowCustomException)
                {
                    throw;
                }
            }
            catch (Exception exception)
            {
                isFailure = true;
                if (_onError != null) await _onError.Invoke(exception).ConfigureAwait(false);
                if (_rethrowException)
                {
                    throw;
                }
            }
            finally
            {
                if (!isFailure)
                {
                    if (_onSuccess != null) await _onSuccess.Invoke().ConfigureAwait(false);
                }

                if (_always != null) await _always.Invoke().ConfigureAwait(false);
            }
        }
    }
}