using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Validation;

namespace Kharazmi.AspNetCore.Core.Domain.Events
{
    /// <summary> </summary>
    public class DomainNotificationDomainEvent : DomainEvent
    {
        private List<FriendlyResultMessage> _messages = [];
        private List<ValidationFailure> _failures;

        public DomainNotificationDomainEvent(string reason, EventTypes eventTypes = EventTypes.Error): base(DomainEventType.From<DomainNotificationDomainEvent>())
        {
            EventTypes = eventTypes;
            Reason = reason;
        }

        public string Reason { get; private set; }

        /// <summary> </summary>
        public IReadOnlyList<FriendlyResultMessage> Messages => _messages.AsReadOnly();

        /// <summary> </summary>
        public IReadOnlyList<ValidationFailure> Failures => _failures;

        /// <summary> </summary>
        public string MessageName { get; private set; }

        /// <summary> </summary>
        public EventTypes EventTypes { get; private set; }


        /// <summary> </summary>
        public static DomainNotificationDomainEvent For(string reason)
            => new DomainNotificationDomainEvent(reason);

        /// <summary> </summary>
        public static DomainNotificationDomainEvent From(FriendlyResultMessage message)
            => For(message.Description).WithMessageName(message.MessageType);

        /// <summary> </summary>
        public static DomainNotificationDomainEvent From(ValidationFailure failure)
            => For("").WithFailure(failure);

        /// <summary> </summary>
        public static DomainNotificationDomainEvent From(Result result)
            => For(result.FriendlyMessage.Description)
                .WithFailures(result.ValidationMessages)
                .WithMessages(result.Messages)
                .WithMessageName(result.FriendlyMessage.MessageType);

        /// <summary> </summary>
        public DomainNotificationDomainEvent WithMessageName(string value)
        {
            MessageName = value;
            return this;
        }

        /// <summary> </summary>
        public DomainNotificationDomainEvent WithEventType(EventTypes value)
        {
            EventTypes = value;
            return this;
        }

        

        /// <summary> </summary>
        public DomainNotificationDomainEvent WithMessages(params IReadOnlyCollection<FriendlyResultMessage> messages)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));
            _messages.AddRange(messages);
            return this;
        }

        /// <summary> </summary>
        public DomainNotificationDomainEvent WithFailure(ValidationFailure failure)
        {
            _failures.Add(failure);
            EventTypes = EventTypes.Failure;
            return this;
        }

        /// <summary> </summary>
        public DomainNotificationDomainEvent WithFailures(params IReadOnlyCollection<ValidationFailure> failures)
        {
            _failures.AddRange(failures);
            EventTypes = EventTypes.Failure;
            return this;
        }
    }
}