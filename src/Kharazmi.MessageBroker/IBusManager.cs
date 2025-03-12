using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Domain.Events;
using RabbitMQ.Client;

namespace Kharazmi.MessageBroker
{
    /// <summary> </summary>
    public interface IBusManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        Task PublishCommandAsync<TCommand>(TCommand command, DomainContext domainContext = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="domainContext"></param>
        /// <param name="exchangeConfiguration"></param>
        /// <param name="properties"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        Task PublishCommandForAsync<TCommand>(
            TCommand command,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task PublishEventAsync<TEvent>(TEvent @event, DomainContext domainContext = null,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="domainContext"></param>
        /// <param name="exchangeConfiguration"></param>
        /// <param name="properties"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task PublishEventForAsync<TEvent>(
            TEvent @event,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent;
    }
}