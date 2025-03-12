using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Cqrs.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Cqrs.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : DomainCommand
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogDebug("----- Handling command {CommandName} ({@Command})", request.GetGenericTypeName(), request);

            var response = await next().ConfigureAwait(false);

            _logger.LogDebug("----- Command {CommandName} handled - response: {@Response}", request.GetGenericTypeName(), response);

            return response;
        }
    }
}
