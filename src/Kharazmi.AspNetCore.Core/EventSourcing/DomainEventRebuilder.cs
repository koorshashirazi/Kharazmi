using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    public interface IDomainEventFactory
    {
        IReadOnlyCollection<IDomainEvent> CreateFrom(IReadOnlyCollection<EventStoreEntity> eventStoreEntities);
    }

    public class DomainEventFactory(IEventSerializer eventSerializer) : IDomainEventFactory
    {
        private readonly IEventSerializer _eventSerializer =
            Ensure.ArgumentIsNotNull(eventSerializer, nameof(eventSerializer));

        public IReadOnlyCollection<IDomainEvent> CreateFrom(IReadOnlyCollection<EventStoreEntity> eventStoreEntities)
        {
            Ensure.ArgumentIsNotNull(eventStoreEntities, nameof(eventStoreEntities));
            HashSet<IDomainEvent> domainEvents = [];

            foreach (var eventStoreEntity in eventStoreEntities)
            {
                var domainEventType = Type.GetType(eventStoreEntity.EventType) ??
                                      throw new InvalidCastException(
                                          $"Unable to get the type `{eventStoreEntity.EventType}`");

                var domainEvent = _eventSerializer.Deserialize(eventStoreEntity.PayLoad, domainEventType) ??
                                  throw new InvalidCastException(
                                      $"Unable to deserialize the {eventStoreEntity.EventType}");
                domainEvents.Add(domainEvent);
            }

            return domainEvents.AsReadOnly();
        }
    }
}