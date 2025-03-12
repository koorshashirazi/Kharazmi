using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    internal class LoggerCommandPipeline<TCommand> : ICommandHandler<TCommand>, IPipelineHandler
        where TCommand : Command
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly ILogger<LoggerCommandPipeline<TCommand>> _logger;

        public LoggerCommandPipeline(
            ICommandHandler<TCommand> handler,
            ILogger<LoggerCommandPipeline<TCommand>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task<Result> HandleAsync(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            var commandType = command.GetType().GetGenericTypeName();

            var jsonCommand =
                $"\n=============== Executing Command {commandType} ================\n{JsonConvert.SerializeObject(command, Formatting.Indented)}\n";
            var jsonMessageContext =
                $"\n=============== With Message Context ================\n{JsonConvert.SerializeObject(domainContext, Formatting.Indented)}\n";

            Print(jsonCommand, jsonMessageContext);

            var result = await _handler.HandleAsync(command, domainContext, cancellationToken).ConfigureAwait(false);

            Print($"\n=============== Executed {commandType} ================");

            Print($"\nResult failed: {result.Failed}");

            if (result.Failed)
            {
                Print($"\nError:");

                foreach (var error in result.Messages)
                {
                    Print($"\n\t{error.Code}\n\t{error.Description}");
                }

                Print($"\nFailures:");
                foreach (var failure in result.ValidationMessages)
                {
                    Print($"\n\t{failure.PropertyName}\n\t{failure.ErrorMessage}");
                }
            }


            return result;
        }

        private void Print(string jsonCommand, string jsonMessageContext = "")
        {
            if (_logger == null) return;
            _logger.LogInformation(jsonCommand);
            if (jsonMessageContext.IsNotEmpty())
                _logger.LogInformation(jsonMessageContext);
        }
    }
}