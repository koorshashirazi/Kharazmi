using System;

namespace Kharazmi.AspNetCore.Core.Domain
{
    public interface IMessage
    {
        /// <summary></summary>
        DateTimeOffset OccurrendOn { get; }

        /// <summary></summary>
        string Action { get; }
       
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Message : IMessage
    {
        /// <summary> </summary>
        protected Message()
        {
            OccurrendOn = DateTimeOffset.UtcNow;
            Action = GetType().Name;
        }

        /// <summary> </summary>
        public DateTimeOffset OccurrendOn { get; protected set; }

       
        /// <summary> </summary>
        public string Action { get; protected set; }

    }
}