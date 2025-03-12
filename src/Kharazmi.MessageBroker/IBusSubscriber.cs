using System;
using System.Threading;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.MessageBroker
{
    /// <summary> </summary>
    public interface IBusSubscriber
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        IBusSubscriber SubscribeCommand<TCommand>(
            Func<TCommand, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageConfiguration"></param>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        IBusSubscriber SubscribeCommandFrom<TCommand>(
            MessageConfiguration messageConfiguration = null,
            Func<TCommand, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        IBusSubscriber SubscribeEvent<TEvent>(
            Func<TEvent, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default) where TEvent :class, IDomainEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageConfiguration"></param>
        /// <param name="onError"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        IBusSubscriber SubscribeEventFrom<TEvent>(
            MessageConfiguration messageConfiguration = null,
            Func<TEvent, FrameworkException, RejectEvent> onError = null,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent;
    }
}