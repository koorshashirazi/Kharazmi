using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.HandlerRetry;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    internal sealed class TransientFaultDomainEventPipeline<TEvent> : DomainEventHandler<TEvent>, IPipelineHandler
        where TEvent : class, IDomainEvent
    {
        private readonly ILogger<TransientFaultDomainEventPipeline<TEvent>> _logger;
        private readonly IDomainEventHandler<TEvent> _handler;
        private readonly IHandlerRetryStrategy _retryStrategy;
        private readonly IDomainContextService _domainContextService;

        public TransientFaultDomainEventPipeline(
            IDomainEventHandler<TEvent> handler,
            IHandlerRetryStrategy retryStrategy,
            ILogger<TransientFaultDomainEventPipeline<TEvent>> logger, IDomainContextService domainContextService)
        {
            _logger = logger;
            _domainContextService = domainContextService;
            _handler = Ensure.ArgumentIsNotNull(handler, nameof(handler));
            _retryStrategy = Ensure.ArgumentIsNotNull(retryStrategy, nameof(retryStrategy));
        }


        public override Task<Result> HandleAsync(TEvent domainEvent, CancellationToken token = default)
        {
            return ExceptionHandler.ExecuteResultAsync(async () =>
            {
                if (token.IsCancellationRequested) return Result.Fail("Cancellation is requested");

                var label = domainEvent.EventType.ToString();
                var stopwatch = Stopwatch.StartNew();

                return await HandleResultAsync(domainEvent, token, label, 0, stopwatch.Elapsed.TotalSeconds)
                    .ConfigureAwait(false);
            }, onError: async ex =>
            {
                var label = domainEvent.EventType.ToString();
                var stopwatch = Stopwatch.StartNew();
                var currentRetryCount = 0;

                while (true)
                {
                    var currentTime = stopwatch.Elapsed;
                    var currentException = ex;
                    _logger.LogDebug(currentException.Message);

                    var retry = _retryStrategy.ShouldBeRetied(currentException, currentTime, currentRetryCount);

                    if (!retry.ShouldBeRetried)
                    {
                        ex.AsDomainException();
                        return Result.Fail(
                            $"Exception with message '{currentException.Message} 'is transient, retrying action '{label}' after {retry.RetryAfter.TotalSeconds} seconds for retry count {currentRetryCount}");
                    }

                    await _domainContextService
                        .UpdateAsync(context => context.UpdateRetrying(currentRetryCount))
                        .ConfigureAwait(false);

                    currentRetryCount++;

                    var result = await HandleResultAsync(domainEvent, token, label, currentRetryCount,
                            stopwatch.Elapsed.TotalSeconds)
                        .ConfigureAwait(false);

                    if (retry.RetryAfter != TimeSpan.Zero)
                    {
                        _logger.LogDebug(
                            "Exception with message '{Message} 'is transient, retrying action '{Label}' after {RetryAfter} seconds for retry count {CurrentRetryCount}",
                            currentException.Message,
                            label,
                            retry.RetryAfter.TotalSeconds,
                            currentRetryCount);

                        await Task.Delay(retry.RetryAfter, token).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "Exception {0} with message '{1}' is transient, retrying action '{2}' NOW for retry count {3}",
                            currentException.AsJsonException(),
                            currentException.Message,
                            label,
                            currentRetryCount);
                    }

                    if (result.Failed) continue;

                    return result;
                }
            });
        }

        private async Task<Result> HandleResultAsync(TEvent domainEvent, CancellationToken token, string label,
            int currentRetryCount, double totalSeconds)
        {
            var result = await _handler
                .HandleAsync(domainEvent, token)
                .ConfigureAwait(false);

            _logger.LogDebug(
                "Finished execution of '{Label}' after {CurrentRetryCount} retries and {TotalSeconds} seconds",
                label, currentRetryCount, totalSeconds);

            return result;
        }
    }
}