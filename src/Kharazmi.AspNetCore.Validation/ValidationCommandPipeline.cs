using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Validation
{
    internal class ValidationDomainCommandPipeline<TCommand> : DomainCommandHandler<TCommand> where TCommand : class, IDomainCommand
    {
        private readonly IDomainCommandHandler<TCommand> _handler;
        private readonly IValidator<TCommand> _validator;
        private readonly ILogger<ValidationDomainCommandPipeline<TCommand>> _logger;

        public ValidationDomainCommandPipeline(
            IDomainCommandHandler<TCommand> handler,
            IValidator<TCommand> validator,
            ILogger<ValidationDomainCommandPipeline<TCommand>> logger)
        {
            _handler = handler;
            _validator = validator;
            _logger = logger;
        }

     

        public override Task<Result> HandleAsync(TCommand command, CancellationToken token = default)
        {
            _logger.LogDebug("\n==================== Validation Command ==================== ");

            var failures = _validator.Validate(command)?.ToList() ?? new List<ValidationFailure>();
            if (failures.Count == 0)
            {
                return _handler.HandleAsync(command, token);
            }

            var jsonCommand = JsonConvert.SerializeObject(command);
            var commandType = command.GetGenericTypeName();

            _logger.LogError(
                "\nValidation errors:\n\t Command Type: {0}\n\tCommand: {1}\n\tValidation ValidationMessages: {2}", commandType, jsonCommand, failures);

            throw DomainException.Empty().AddValidationMessages(failures);
        }
    }
}