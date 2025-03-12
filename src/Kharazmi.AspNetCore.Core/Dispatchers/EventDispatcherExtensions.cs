using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Application.Events;
using Kharazmi.AspNetCore.Core.Application.Models;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventDispatcherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="busManager"></param>
        /// <param name="events"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RaiseEventAsync(
            this IEventDispatcher busManager,
            IEnumerable<IDomainEvent> events,
            DomainContext domainContext = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = events.Select(async @event => await
                busManager.RaiseAsync(@event, domainContext, cancellationToken).ConfigureAwait(false));
            return Task.WhenAll(tasks);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="busManager"></param>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static Task RaiseCreatingEventAsync<TModel, TKey>(
            this IEventDispatcher busManager,
            IEnumerable<TModel> models,
            CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return RaiseAsync(busManager, typeof(CreatingEvent<TModel, TKey>), models, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="busManager"></param>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static Task RaiseCreatedEventAsync<TModel, TKey>(
            this IEventDispatcher busManager,
            IEnumerable<TModel> models,
            CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return RaiseAsync(busManager, typeof(CreatedEvent<TModel, TKey>), models, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="busManager"></param>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static Task RaiseEditingEventAsync<TModel, TKey>(
            this IEventDispatcher busManager,
            IEnumerable<ModifiedModel<TModel>> models,
            CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return RaiseAsync(busManager, typeof(EditingEvent<TModel, TKey>), models, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="busManager"></param>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static Task RaiseEditedEventAsync<TModel, TKey>(
            this IEventDispatcher busManager,
            IEnumerable<ModifiedModel<TModel>> models,
            CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return RaiseAsync(busManager, typeof(EditedEvent<TModel, TKey>), models, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="busManager"></param>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static Task RaiseDeletingEventAsync<TModel, TKey>(
            this IEventDispatcher busManager,
            IEnumerable<TModel> models,
            CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return RaiseAsync(busManager, typeof(DeletingEvent<TModel, TKey>), models, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="busManager"></param>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static Task RaiseDeletedEventAsync<TModel, TKey>(
            this IEventDispatcher busManager,
            IEnumerable<TModel> models,
            CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return RaiseAsync(busManager, typeof(DeletedEvent<TModel, TKey>), models, cancellationToken);
        }

        private static Task<Result> RaiseAsync(
            IEventDispatcher busManager,
            Type eventType,
            object model,
            CancellationToken cancellationToken = default)
        {
            if (!(Activator.CreateInstance(eventType, model) is IDomainEvent @event))
                return Task.FromResult(Result.Fail("Can't create a instance of event"));

            return busManager.RaiseAsync(@event, cancellationToken);
        }
    }
}