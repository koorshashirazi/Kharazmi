using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.Events
{
    public class MediaFileCreatedEvent : DomainEvent
    {
        public string File { get; }
        public string AggregateId { get; }
        public Guid MediaFileId { get; }

        public MediaFileCreatedEvent(string aggregateId, Guid mediaFileId, string file)
            : base(DomainEventType.From<MediaFileCreatedEvent>())
        {
            AggregateId = aggregateId;
            MediaFileId = mediaFileId;
            File = file;
        }
    }
}