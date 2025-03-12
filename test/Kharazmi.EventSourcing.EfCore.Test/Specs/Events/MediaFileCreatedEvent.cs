﻿using System;
using Kharazmi.AspNetCore.Core.Domain.Events;

namespace Kharazmi.EventSourcing.EfCore.Test.Specs.Events
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