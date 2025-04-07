using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    public interface IEventSourcing
    {
        IReadOnlyCollection<IDomainEvent> GetUncommittedEvents();

        void MarkChangesAsCommitted();

        void ValidateVersion();
      
        Task ApplyFromHistoryAsync(IEventStore eventStore, CancellationToken cancellationToken);

        Task CommitAsync(IEventStore eventStore, CancellationToken cancellationToken);
    }
}