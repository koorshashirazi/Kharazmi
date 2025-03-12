using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Validation
{
    internal class ValidationCommandPipeline<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly IValidator<TCommand> _validator;
        private readonly ILogger<ValidationCommandPipeline<TCommand>> _logger;

        public ValidationCommandPipeline(
            ICommandHandler<TCommand> handler,
            IValidator<TCommand> validator,
            ILogger<ValidationCommandPipeline<TCommand>> logger)
        {
            _handler = handler;
            _validator = validator;
            _logger = logger;
        }

        public Task<Result> HandleAsync(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("\n==================== Validation Command ==================== ");

            var failures = _validator.Validate(command)?.ToList() ?? new List<ValidationFailure>();
            if (!failures.Any())
            {
                return _handler.HandleAsync(command, domainContext, cancellationToken);
            }

            var jsonCommand = JsonConvert.SerializeObject(command);
            var commandType = command.GetGenericTypeName();

            _logger.LogError(
                "\nValidation errors:\n\t Command Type: {0}\n\tCommand: {1}\n\tValidation ValidationMessages: {2}", commandType, jsonCommand, failures);

            throw DomainException.Empty().AddValidationMessages(failures);
        }
    }
}