using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Cqrs.Messages;

namespace Kharazmi.AspNetCore.Cqrs
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theEvent"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        Task SaveAsync<TEvent>(TEvent theEvent, CancellationToken cancellationToken = default)
            where TEvent : DomainEvent;
    }
}