using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Cqrs.Messages;

namespace Kharazmi.AspNetCore.Cqrs
{
    internal  class NullEventStore : IEventStore
    {
        public Task SaveAsync<TEvent>(TEvent theEvent, CancellationToken cancellationToken = default) where TEvent : DomainEvent
        {
            return Task.CompletedTask;
        }
    }
}