using System;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary></summary>
    public class DomainNotification : DomainEvent
    {
        /// <summary></summary>
        public Guid DomainNotificationId { get; }

        /// <summary></summary>
        public string Key { get; }

        /// <summary></summary>
        public string Value { get; }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="version"></param>
        /// <param name="eventTypes"></param>
        [JsonConstructor]
        public DomainNotification(string key, string value, int version = 1, EventTypes eventTypes = EventTypes.Error)
        {
            DomainNotificationId = Guid.NewGuid();
            Version = version;
            Key = key;
            Value = value;
        }
    }
}