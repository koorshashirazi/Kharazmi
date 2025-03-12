using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Cqrs.Messages;

namespace Kharazmi.AspNetCore.Cqrs.Handlers
{
    /// <summary></summary>
    public interface IDomainNotificationHandler : IEventHandler<DomainNotification>
    {
        /// <summary></summary>
        List<DomainNotification> GetNotifications();

        /// <summary></summary>
        bool HasNotifications();

        /// <summary></summary>
        void Dispose();
    }

    /// <summary></summary>
    public class DomainNotificationHandler : IDomainNotificationHandler
    {
        private List<DomainNotification> _notifications;

        /// <summary></summary>
        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }

        /// <summary></summary>
        public Task Handle(DomainNotification message, CancellationToken cancellationToken)
        {
            message.CheckArgumentIsNull(nameof(message));
            _notifications.Add(message);
            return Task.CompletedTask;
        }

        /// <summary></summary>
        public virtual List<DomainNotification> GetNotifications()
        {
            return _notifications;
        }

        /// <summary></summary>
        public virtual bool HasNotifications()
        {
            return GetNotifications().Any();
        }

        /// <summary></summary>
        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
        }
    }
}