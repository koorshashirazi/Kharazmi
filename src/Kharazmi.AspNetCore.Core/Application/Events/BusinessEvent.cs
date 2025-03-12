using System;

namespace Kharazmi.AspNetCore.Core.Application.Events
{
    public interface IBusinessEvent
    {
        string BusinessEventId { get; }
        string EventName { get; }
        DateTime CreateAt { get; }
        bool IsEssential { get;  }
    }

    public abstract class BusinessEvent : IBusinessEvent
    {
        protected BusinessEvent()
        {
            BusinessEventId = Guid.NewGuid().ToString("N");
            EventName = GetType().Name;
            CreateAt = DateTime.Now;
        }

        public string BusinessEventId { get;  }
        public string EventName { get; }
        public DateTime CreateAt { get; }
        public bool IsEssential { get; protected set; }
    }
}