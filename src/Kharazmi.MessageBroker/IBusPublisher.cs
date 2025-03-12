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
    public interface IBusPublisher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        Task SendAsync<TCommand>(TCommand command, DomainContext context,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand;

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
        Task SendToAsync<TCommand>(TCommand command,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TCommand : ICommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task PublishAsync<TEvent>(TEvent @event, DomainContext context,
            CancellationToken cancellationToken = default)
            where TEvent : class, IDomainEvent;

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
        Task PublishToAsync<TEvent>(TEvent @event,
            DomainContext domainContext,
            ExchangeConfiguration exchangeConfiguration = null,
            Action<IBasicProperties> properties = null,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent;
    }
}