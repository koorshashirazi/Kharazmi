using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    internal sealed class LoggerDomainCommandPipeline<TCommand> : DomainCommandHandler<TCommand>, IPipelineHandler
        where TCommand : DomainCommand
    {
        private readonly IDomainCommandHandler<TCommand> _handler;
        private readonly ILogger<LoggerDomainCommandPipeline<TCommand>> _logger;

        public LoggerDomainCommandPipeline(
            IDomainCommandHandler<TCommand> handler,
            ILogger<LoggerDomainCommandPipeline<TCommand>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        private void Print(string jsonCommand)
        {
            _logger.LogDebug("Command: {JsonCommand}", jsonCommand);
        }

        public override async Task<Result> HandleAsync(TCommand command, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (command == null) throw new ArgumentNullException(nameof(command));
            var commandType = command.GetType().GetGenericTypeName();

            var jsonCommand =
                $"\n=============== Executing Command {commandType} ================\n{JsonConvert.SerializeObject(command, Formatting.Indented)}\n";

            Print(jsonCommand);

            var result = await _handler.HandleAsync(command, token).ConfigureAwait(false);

            Print($"\n=============== Executed {commandType} ================");

            Print($"\nResult failed: {result.Failed}");

            if (!result.Failed) return result;

            Print($"\nError:");

            foreach (var error in result.Messages)
            {
                Print($"\n\t{error.MessageType}\n\t{error.Description}");
            }

            Print($"\nFailures:");
            foreach (var failure in result.ValidationMessages)
            {
                Print($"\n\t{failure.PropertyName}\n\t{failure.ErrorMessage}");
            }

            return result;
        }
    }
}