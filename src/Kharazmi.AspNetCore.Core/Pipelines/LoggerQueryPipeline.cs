using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Queries;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    internal class LoggerQueryPipeline<TQuery, TResult> : IQueryHandler<TQuery, TResult>, IPipelineHandler
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly ILogger<LoggerQueryPipeline<TQuery, TResult>> _logger;

        public LoggerQueryPipeline(
            IQueryHandler<TQuery, TResult> handler,
            ILogger<LoggerQueryPipeline<TQuery, TResult>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task<Result<TResult>> HandleAsync(TQuery query, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            var eventType = query.GetType().GetGenericTypeName();

            var jsonQuery =
                $"\n=============== Executing Query ================\n{JsonConvert.SerializeObject(query, Formatting.Indented)}\n";
            var jsonMessageContext =
                $"\n=============== With Message Context ================\n{JsonConvert.SerializeObject(domainContext, Formatting.Indented)}\n";

            Print(jsonQuery, jsonMessageContext);

            var result = await _handler.HandleAsync(query, domainContext, cancellationToken).ConfigureAwait(false);

            Print($"\n=============== Executed {eventType} ================");

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