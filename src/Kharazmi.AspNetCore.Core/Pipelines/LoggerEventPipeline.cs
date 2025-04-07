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
    internal sealed class LoggerDomainEventPipeline<TEvent> : DomainEventHandler<TEvent>, IPipelineHandler
        where TEvent : class, IDomainEvent
    {
        private readonly IDomainEventHandler<TEvent> _handler;
        private readonly ILogger<LoggerDomainEventPipeline<TEvent>> _logger;

        public LoggerDomainEventPipeline(
            IDomainEventHandler<TEvent> handler,
            ILogger<LoggerDomainEventPipeline<TEvent>> logger)
        {
            _handler = handler;
            _logger = logger;
        }


        private void Print(string jsonEvent)
        {
            _logger.LogDebug("Event: {JsonEvent}", jsonEvent);
        }

        public override async Task<Result> HandleAsync(TEvent domainEvent, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

            var eventType = domainEvent.EventType.ToType().GetGenericTypeName();

            var jsonEvent =
                $"\n=============== Executing Event ================\n{JsonConvert.SerializeObject(domainEvent, Formatting.Indented)}\n";
            Print(jsonEvent);

            var result = await _handler.HandleAsync(domainEvent, token).ConfigureAwait(false);

            Print($"\n=============== Executed {eventType} ================");

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