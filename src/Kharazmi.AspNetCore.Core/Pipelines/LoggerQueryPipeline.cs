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
    internal sealed class LoggerDomainQueryPipeline<TQuery, TResult> : IDomainQueryHandler<TQuery, TResult>, IPipelineHandler
        where TQuery : IDomainQuery
    {
        private readonly IDomainQueryHandler<TQuery, TResult> _handler;
        private readonly ILogger<LoggerDomainQueryPipeline<TQuery, TResult>> _logger;

        public LoggerDomainQueryPipeline(
            IDomainQueryHandler<TQuery, TResult> handler,
            ILogger<LoggerDomainQueryPipeline<TQuery, TResult>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        private void Print(string jsonQuery)
        {
            _logger.LogDebug("Query: {JaonQuery}", jsonQuery);
        }

        public async Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (query is null) throw new ArgumentNullException(nameof(query));
            var typeName = query.GetType().GetGenericTypeName();

            var jsonQuery =
                $"\n=============== Executing Query ================\n{JsonConvert.SerializeObject(query, Formatting.Indented)}\n";
            Print(jsonQuery);
            Print(jsonQuery);

            var result = await _handler.HandleAsync(query, token).ConfigureAwait(false);

            Print($"\n=============== Executed {typeName} ================");

            Print($"\nResult failed: {result.Failed}");

            if (result.Failed)
            {
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
            }

            return result;
        }
    }
}