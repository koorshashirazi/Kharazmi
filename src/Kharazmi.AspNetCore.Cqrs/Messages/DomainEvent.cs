using System;
using MediatR;

namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DomainEvent : INotification
    {
        /// <summary></summary>
        protected DomainEvent()
        {
            EventId = Guid.NewGuid().ToString("N");
            CreateAt = DateTime.Now;
            Action = GetType().Name;
        }

        /// <summary> </summary>
        public DateTime CreateAt { get; protected set; }

        /// <summary> </summary>
        public string Action { get; protected set; }

        /// <summary> </summary>
        public string AggregateId { get; protected set; }

        /// <summary></summary>
        public string EventId { get; protected set; }

        /// <summary></summary>
        public string Source { get; protected set; }

        /// <summary></summary>
        public string Reason { get; protected set; }

        /// <summary></summary>
        public bool IsEssential { get; protected set; }

        /// <summary></summary>
        public int Version { get; protected set; }
    }
}