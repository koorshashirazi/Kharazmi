using System;
using Kharazmi.AspNetCore.Core.Functional;
using MediatR;

namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary></summary>
    public abstract class DomainCommand : IRequest <Result>
    {
        /// <summary></summary>
        protected DomainCommand()
        {
            CreateAt = DateTime.UtcNow;
            Action = GetType().Name;
        }

        /// <summary> </summary>
        public DateTime CreateAt { get; protected set; }

        /// <summary> </summary>
        public string Action { get; protected set; }

        /// <summary> </summary>
        public string AggregateId { get; protected set; }
    }
}