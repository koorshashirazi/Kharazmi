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
    internal sealed class TransientFaultDomainQueryPipeline<TQuery, TResult> : DomainQueryHandler<TQuery, TResult>, IPipelineHandler
        where TQuery : class,IDomainQuery
    {
        private readonly ILogger<TransientFaultDomainQueryPipeline<TQuery, TResult>> _logger;
        private readonly IDomainQueryHandler<TQuery, TResult> _handler;
        private readonly IHandlerRetryStrategy _retryStrategy;
        private readonly IDomainContextService _domainContextService;

        public TransientFaultDomainQueryPipeline(
            IDomainQueryHandler<TQuery, TResult> handler,
            IHandlerRetryStrategy retryStrategy,
            ILogger<TransientFaultDomainQueryPipeline<TQuery, TResult>> logger, IDomainContextService domainContextService)
        {
            _logger = logger;
            _domainContextService = domainContextService;
            _handler = Ensure.ArgumentIsNotNull(handler, nameof(handler));
            _retryStrategy = Ensure.ArgumentIsNotNull(retryStrategy, nameof(retryStrategy));
        }


        public override Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken token = default)
        {
          return ExceptionHandler.ExecuteResultAsAsync(async () =>
            {
                if (token.IsCancellationRequested) return Result.Fail<TResult>("Cancellation is requested");

                var label = query.QueryType.ToString();
                var stopwatch = Stopwatch.StartNew();

                return await HandleResultAsync(query, token, label, 0, stopwatch.Elapsed.TotalSeconds)
                    .ConfigureAwait(false);
            }, onError: async ex =>
            {
                var label = query.QueryType.ToString();
                var stopwatch = Stopwatch.StartNew();
                var currentRetryCount = 0;

                while (true)
                {
                    var currentTime = stopwatch.Elapsed;
                    _logger.LogDebug(ex.Message);

                    var retry = _retryStrategy.ShouldBeRetied(ex, currentTime, currentRetryCount);

                    if (!retry.ShouldBeRetried)
                    {
                        ex.AsDomainException();
                        return Result.Fail<TResult>(
                            $"Exception with message '{ex.Message} 'is transient, retrying action '{label}' after {retry.RetryAfter.TotalSeconds} seconds for retry count {currentRetryCount}");
                    }

                    await _domainContextService
                        .UpdateAsync(context => context.UpdateRetrying(currentRetryCount))
                        .ConfigureAwait(false);

                    currentRetryCount++;

                    var result = await HandleResultAsync(query, token, label, currentRetryCount, stopwatch.Elapsed.TotalSeconds)
                        .ConfigureAwait(false);

                    if (retry.RetryAfter != TimeSpan.Zero)
                    {
                        _logger.LogDebug(
                            "Exception with message '{Message} 'is transient, retrying action '{Label}' after {RetryAfter} seconds for retry count {CurrentRetryCount}",
                            ex.Message,
                            label,
                            retry.RetryAfter.TotalSeconds,
                            currentRetryCount);

                        await Task.Delay(retry.RetryAfter, token).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "Exception {0} with message '{1}' is transient, retrying action '{2}' NOW for retry count {3}",
                            ex.AsJsonException(),
                            ex.Message,
                            label,
                            currentRetryCount);
                    }

                    if (result.Failed) continue;

                    return result;
                }
            });
        }

        private async Task<Result<TResult>> HandleResultAsync(TQuery query, CancellationToken token, string label,
            int currentRetryCount, double totalSeconds)
        {
            var result = await _handler
                .HandleAsync(query, token)
                .ConfigureAwait(false);

            _logger.LogDebug(
                "Finished execution of '{Label}' after {CurrentRetryCount} retries and {TotalSeconds} seconds",
                label, currentRetryCount, totalSeconds);

            return result;
        }
    }
}