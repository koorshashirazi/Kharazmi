using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.MessageBroker
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBusHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        IBusHandler Handle(Func<Task> handle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        IBusHandler OnSuccess(Func<Task> onSuccess);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        IBusHandler OnError(Func<Exception, Task> onError, bool rethrow = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onCustomError"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        IBusHandler OnCustomError(Func<MessageBusException, Task> onCustomError, bool rethrow = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="always"></param>
        /// <returns></returns>
        IBusHandler Always(Func<Task> always);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();
    }
}