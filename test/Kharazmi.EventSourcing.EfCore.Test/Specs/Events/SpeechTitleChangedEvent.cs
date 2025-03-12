﻿using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.EventSourcing.EfCore.Test.Specs.Events
{
    public class SpeechTitleChangedEvent : DomainEvent
    {
        public string AggregateId { get; }
        public string Title { get; }

        public SpeechTitleChangedEvent(string aggregateId, string title)
            : base(DomainEventType.From<SpeechTitleChangedEvent>())
        {
            AggregateId = aggregateId;
            Title = title;
        }
    }
}