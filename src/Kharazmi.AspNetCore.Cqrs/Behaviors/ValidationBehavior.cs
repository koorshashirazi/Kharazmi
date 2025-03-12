using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Cqrs.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

//using Kharazmi.AspNetCore.Core.Extensions;
//using Kharazmi.AspNetCore.Core.Functional;
//using Kharazmi.AspNetCore.Cqrs.Commands;
//using MediatR;
//using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Cqrs.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : DomainCommand
        where TResponse : Result
    {
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest>[] _validators;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validators"></param>
        /// <param name="logger"></param>
        public ValidationBehavior(
            IValidator<TRequest>[] validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = Ensure.IsNotNullWithDetails(validators, nameof(validators));
            _logger = Ensure.IsNotNullWithDetails(logger, nameof(logger));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogDebug("----- Validating command {CommandType}", typeName);

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Count == 0) return await next().ConfigureAwait(false);

            _logger.LogWarning(
                "Validation errors - {CommandType} - Command: {@Command} - Messages: {@ValidationErrors}", typeName,
                request, failures);

            throw new ValidationException("Validation exception", failures);
        }
    }
}