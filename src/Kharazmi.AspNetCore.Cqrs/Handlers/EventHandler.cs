using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Cqrs.Messages;
using MediatR;

namespace Kharazmi.AspNetCore.Cqrs.Handlers
{
    /// <summary></summary>
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : DomainEvent
    {
    }
    
    /// <summary></summary>
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : DomainEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task Handle(TEvent notification, CancellationToken cancellationToken);
    }
}