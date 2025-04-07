using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Validation;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Validation
{
    internal class CommandValidationHandler<TRequest> : ICommandValidationHandler<TRequest>
        where TRequest : DomainCommand

    {
        private readonly IValidator<TRequest> _validator;
        private readonly ILogger<CommandValidationHandler<TRequest>> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="logger"></param>
        public CommandValidationHandler(
            IValidator<TRequest> validator,
            ILogger<CommandValidationHandler<TRequest>> logger)
        {
            _validator = Ensure.ArgumentIsNotNull(validator, nameof(validator));
            _logger = logger;
        }


        public Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation("----- Validating command {CommandType}", typeName);

            var failures = _validator.Validate(request)?.ToList();

            if (failures != null && !failures.Any()) Task.FromResult(Result.Ok());

            _logger.LogError(
                "Validation errors - {CommandType} - Command: {@Command} - Messages: {@ValidationErrors}", typeName,
                request, failures);

            return Task.FromResult(failures.ToResult());
        }
    }
}