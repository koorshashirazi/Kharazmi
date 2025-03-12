using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Cqrs.Messages;
using MediatR;
using DomainEvent = Kharazmi.AspNetCore.Cqrs.Messages.DomainEvent;

namespace Kharazmi.AspNetCore.Cqrs
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainPublisher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        Task<Result> SendCommandAsync<TCommand>(
            TCommand command, CancellationToken cancellationToken = default) where TCommand : DomainCommand;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        Task<TResult> QueryResultAsync<TResult>(
            IQuery<TResult> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task RaiseEventAsync<TEvent>(
            TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : DomainEvent;
    }

    internal class DomainPublisher : IDomainPublisher
    {
        private readonly IEventStore _eventStore;
        private readonly IMediator _mediator;
        private readonly EventSourcingOptions _eventSourcingOptions;


        public DomainPublisher(
            IMediator mediator,
            EventSourcingOptions eventSourcingOptions,
            IEventStore eventStore)
        {
            _mediator = mediator;
            _eventSourcingOptions = eventSourcingOptions;
            _eventStore = eventStore;
        }

        public Task<Result> SendCommandAsync<TCommand>(
            TCommand command, CancellationToken cancellationToken = default) where TCommand : DomainCommand
        {
            return _mediator.Send(command, cancellationToken);
        }

        public Task<TResult> QueryResultAsync<TResult>(
            IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(query, cancellationToken);
        }

        public async Task RaiseEventAsync<TEvent>(
            TEvent @event,  CancellationToken cancellationToken = default)
            where TEvent : DomainEvent
        {
            if (@event == null)
            {
                await Task.CompletedTask.ConfigureAwait(false);
                return;
            }


            if (_eventSourcingOptions.EnableStoreEvent && !(@event is DomainNotification) && CanRaiseEvent(@event))
            {
                await _eventStore.SaveAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            await _mediator.Publish(@event, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        protected virtual bool CanRaiseEvent(DomainEvent evt)
        {
            return CanRaiseEventType(evt.Action);
        }

        /// <summary>
        /// Indicates if the type of event will be persisted.
        /// </summary>
        /// <param name="evtType"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private bool CanRaiseEventType(string evtType)
        {
            return _eventSourcingOptions.NotAllowedEvents.FirstOrDefault(x => x.Equals(evtType)) != null;
        }
    }
}