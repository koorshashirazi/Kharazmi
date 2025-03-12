using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.HandlerRetry;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Threading;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    internal class TransientFaultCommandPipeline<TCommand> : ICommandHandler<TCommand>,IPipelineHandler
        where TCommand : ICommand
    {
        private readonly ILogger<TransientFaultCommandPipeline<TCommand>> _logger;
        private readonly ICommandHandler<TCommand> _handler;
        private readonly IHandlerRetryStrategy _retryStrategy;
        private readonly IDomainContextService _domainContextService;
        public TransientFaultCommandPipeline(
            ICommandHandler<TCommand> handler,
            IHandlerRetryStrategy retryStrategy,
            ILogger<TransientFaultCommandPipeline<TCommand>> logger, 
            IDomainContextService domainContextService)
        {
            _logger = logger;
            _domainContextService = domainContextService;
            _handler = Ensure.ArgumentIsNotNull(handler, nameof(handler));
            _retryStrategy = Ensure.ArgumentIsNotNull(retryStrategy, nameof(retryStrategy));
        }

        public async Task<Result> HandleAsync(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            var label = $"{command.GetGenericTypeName()}";
            var stopwatch = Stopwatch.StartNew();
            var currentRetryCount = 0;

            while (true)
            {
                Exception currentException;
                Retry retry;

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = await _handler.HandleAsync(command, domainContext, cancellationToken)
                        .ConfigureAwait(false);
                    _logger?.LogInformation(
                        "Finished execution of '{0}' after {1} retries and {2:0.###} seconds",
                        label,
                        currentRetryCount,
                        stopwatch.Elapsed.TotalSeconds);

                    return result;
                }
                catch (Exception ex)
                {
                    var currentTime = stopwatch.Elapsed;
                    currentException = ex;
                    _logger?.LogDebug(currentException.Message);
                    
                    retry = _retryStrategy.ShouldBeRetied(currentException, currentTime, currentRetryCount);
                    
                    if (!retry.ShouldBeRetried)
                    {
                        ex.AsDomainException();
                    }
                }
                
                domainContext.UpdateRetrying(currentRetryCount);
                
                if (_domainContextService != null)
                    await _domainContextService.UpdateAsync<TCommand>(domainContext).ConfigureAwait(false);
                
                currentRetryCount++;

                if (retry.RetryAfter != TimeSpan.Zero)
                {
                    _logger?.LogInformation(
                        "Exception {0} with message '{1} 'is transient, retrying action '{2}' after {3:0.###} seconds for retry count {4}",
                        currentException.WithDetailsJsonException().Message,
                        currentException.Message,
                        label,
                        retry.RetryAfter.TotalSeconds,
                        currentRetryCount);
                    await Task.Delay(retry.RetryAfter, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    _logger?.LogInformation(
                        "Exception {0} with message '{1}' is transient, retrying action '{2}' NOW for retry count {3}",
                        currentException.WithDetailsJsonException().Message,
                        currentException.Message,
                        label,
                        currentRetryCount);
                }
            }
        }
    }
}