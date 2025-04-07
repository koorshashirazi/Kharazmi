using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.Events
{
    public class SpeechCreatedEvent : DomainEvent
    {
        public string Id { get; }
        public string Title { get; }
        public string Url { get; }
        public string Description { get; }
        public string Type { get; }

        public SpeechCreatedEvent(string id, string title, string url,
            string description, string type): base(DomainEventType.From<SpeechCreatedEvent>())
        {
            Id = id;
            Title = title;
            Url = url;
            Description = description;
            Type = type;
        }
    }
}