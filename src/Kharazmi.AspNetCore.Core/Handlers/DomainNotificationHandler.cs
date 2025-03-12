using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    public interface IDomainNotificationHandler
    {
        Task<Result> HandleAsync(DomainNotificationDomainEvent domainEvent, DomainContext domainContext,
            CancellationToken cancellationToken = default);

        Task AddNotificationAsync(DomainNotificationDomainEvent notificationDomain);
        Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetNotificationsAsync();
        Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetErrorNotificationsAsync();
        Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetFailureNotificationsAsync();
        Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetInfoNotificationsAsync();
        Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetSuccessNotificationsAsync();
        Task<bool> HasNotificationsAsync();
        Task<bool> IsNotValidAsync();
        void Remove(DomainNotificationDomainEvent domainEvent);
        void Remove(Func<DomainNotificationDomainEvent, bool> predicate);
        void RemoveRange(IEnumerable<DomainNotificationDomainEvent> domainNotificationEvents);
        void Clear();
    }

    
    public sealed class DomainNotificationHandler :  IDomainNotificationHandler
    {
        private readonly List<DomainNotificationDomainEvent> _notifications;
        private readonly object _lock = new object();

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotificationDomainEvent>();
        }

        public Task<Result> HandleAsync(DomainNotificationDomainEvent domainEvent, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromResult(Result.Fail("Cancellation token is Requested"));

            AddNotificationAsync(domainEvent);
            return Task.FromResult(Result.Ok());
        }


        public Task AddNotificationAsync(DomainNotificationDomainEvent notificationDomain)
        {
            lock (_lock)
            {
                _notifications.Add(notificationDomain);
                return Task.CompletedTask;
            }
        }

        public Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetNotificationsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_notifications.AsReadOnly());
            }
        }

        public Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetErrorNotificationsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_notifications.Where(x => x.EventTypes == EventTypes.Error).AsReadOnly());
            }
        }

        public Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetFailureNotificationsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_notifications.Where(x => x.EventTypes == EventTypes.Failure).AsReadOnly());
            }
        }

        public Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetInfoNotificationsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_notifications.Where(x => x.EventTypes == EventTypes.Information).AsReadOnly());
            }
        }

        public Task<ReadOnlyCollection<DomainNotificationDomainEvent>> GetSuccessNotificationsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_notifications.Where(x => x.EventTypes == EventTypes.Success).AsReadOnly());
            }
        }

        public Task<bool> HasNotificationsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_notifications.Any());
            }
        }

        public Task<bool> IsNotValidAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_notifications.Any(x =>
                    x.EventTypes == EventTypes.Error || x.EventTypes == EventTypes.Failure));
            }
        }

        public void Remove(DomainNotificationDomainEvent domainEvent)
        {
            lock (_lock)
            {
                _notifications.Remove(domainEvent);
            }
        }


        public void Remove(Func<DomainNotificationDomainEvent, bool> predicate)
        {
            var toRemoves = _notifications.Where(predicate);
            foreach (var @event in toRemoves)
            {
                Remove(@event);
            }
        }

        public void RemoveRange(IEnumerable<DomainNotificationDomainEvent> domainNotificationEvents)
        {
            foreach (var @event in domainNotificationEvents)
            {
                Remove(@event);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _notifications.Clear();
            }
        }
    }
}