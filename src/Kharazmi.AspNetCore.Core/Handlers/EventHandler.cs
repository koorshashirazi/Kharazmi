using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Domain.Queries;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Threading;
using Kharazmi.AspNetCore.Core.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    public interface IEventHandler<in TEvent> where TEvent : class, IDomainEvent
    {
        Task<Result> HandleAsync(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default);
    }

    public abstract class EventHandler<TEvent, TResult> : IEventHandler<TEvent>
        where TEvent : class, IDomainEvent
        where TResult : Result
    {
        public abstract Task<Result> HandleAsync(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class EventHandler<TEvent> : EventHandler<TEvent, Result> where TEvent : DomainEvent
    {
        protected readonly IDomainDispatcher DomainDispatcher;
        protected IDomainNotificationHandler DomainNotifier { get; private set; }
        protected DomainContext DomainContext { get; private set; }
        protected TEvent Event { get; private set; }

        private readonly AsyncLock _asyncLock = new AsyncLock();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public EventHandler(IServiceProvider serviceProvider)
        {
            DomainDispatcher = serviceProvider.GetService<IDomainDispatcher>();
            DomainNotifier = serviceProvider.GetService<IDomainNotificationHandler>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<Result> HandleAsync(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Event = @event;
                DomainContext = domainContext;
                return TryHandleAsync(cancellationToken);
            }
            catch (Exception e)
            {
                OnExecuteFailedAsync();
                e.AsDomainException();
                return Task.FromResult(Result.Fail(""));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<Result> TryHandleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Result Fail(string message) => Result.Fail(message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="failures"></param>
        /// <returns></returns>
        protected Result Fail(string message, IEnumerable<ValidationFailure> failures) =>
            Result.Fail(message).WithValidationMessages(failures);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Result Ok() => Result.Ok();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        protected virtual Task<Result> RaiseEventAsync<TEvent>(TEvent @event,
            CancellationToken cancellationToken = default) where TEvent : class, IDomainEvent
        {
            return DomainDispatcher.RaiseEventAsync(@event, DomainContext, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TMessageCommand"></typeparam>
        /// <returns></returns>
        protected virtual Task<Result> SendCommandAsync<TMessageCommand>(TMessageCommand command,
            CancellationToken cancellationToken = default) where TMessageCommand : Command
        {
            return DomainDispatcher.SendCommandAsync(command, DomainContext, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TQuery"></typeparam>
        /// <returns></returns>
        protected virtual Task<TQuery> QueryAsync<TQuery>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TQuery>
        {
            return DomainDispatcher.QueryResultAsync(query, DomainContext, cancellationToken);
        }


        protected virtual Task<bool> BeforeCommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<bool> OnCommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected virtual Task AfterCommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnExecuteFailedAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
        {
            var isNotValid = await DomainNotifier.IsNotValidAsync().ConfigureAwait(false);

            if (isNotValid)
                return await GetResultAsync().ConfigureAwait(false);

            using (await _asyncLock.WaitAsync(cancellationToken).ConfigureAwait(false))
            {
                if (!await BeforeCommitAsync(cancellationToken).ConfigureAwait(false))
                    return await GetResultAsync().ConfigureAwait(false);

                if (!await OnCommitAsync(cancellationToken).ConfigureAwait(false))
                    return await GetResultAsync().ConfigureAwait(false);
                await AfterCommitAsync(cancellationToken).ConfigureAwait(false);
            }

            return await GetResultAsync().ConfigureAwait(false);
        }

        protected virtual Task AddDomainResultAsync(Result result)
        {
            if (result == null)
                return Task.CompletedTask;

            return !result.Failed
                ? Task.CompletedTask
                : DomainNotifier.AddNotificationAsync(DomainNotificationDomainEvent.From(result));
        }

        protected virtual Task AddDomainErrorAsync(MessageModel messageModel)
        {
            return DomainNotifier.AddNotificationAsync(DomainNotificationDomainEvent.From(messageModel));
        }

        protected virtual Task AddDomainValidationAsync(ValidationFailure validationFailure)
        {
            return DomainNotifier.AddNotificationAsync(DomainNotificationDomainEvent.From(validationFailure));
        }

        protected virtual async Task ExecuteFailAsync()
        {
            await OnExecuteFailedAsync().ConfigureAwait(false);

            var domainException = DomainException.From(await GetResultAsync().ConfigureAwait(false));

            ClearDomainNotifaction();

            throw domainException;
        }

        protected virtual void ClearDomainNotifaction()
        {
            DomainNotifier.Clear();
        }

        protected virtual Task<Result> GetResultAsync(MessageModel? messageModel = null)
            => ToResultAsync(messageModel);

        protected virtual async Task<Result> ToResultAsync(
            MessageModel? errorDescriber)
        {
            var domainNotification = await DomainNotifier.GetNotificationsAsync().ConfigureAwait(false);

            var failures = domainNotification.Where(x => x.EventTypes == EventTypes.Failure)
                .SelectMany(x => x.Failures)
                .ToList();

            var messages = domainNotification.Where(x => x.EventTypes != EventTypes.Failure)
                .SelectMany(x => x.Messages)
                .ToList();

            if (failures.Count != 0 || messages.Count != 0 || errorDescriber != null)
                return Result.Fail(errorDescriber)
                    .WithMessageType(Event.EventType.Value)
                    .WithValidationMessages(failures)
                    .WithMessages(messages);

            return Result.Ok();
        }
    }
}