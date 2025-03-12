using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    internal class LoggerEventPipeline<TEvent> : IEventHandler<TEvent>, IPipelineHandler where TEvent : class, IDomainEvent
    {
        private readonly IEventHandler<TEvent> _handler;
        private readonly ILogger<LoggerEventPipeline<TEvent>> _logger;

        public LoggerEventPipeline(
            IEventHandler<TEvent> handler,
            ILogger<LoggerEventPipeline<TEvent>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task<Result> HandleAsync(TEvent @event, DomainContext domainContext,
            CancellationToken cancellationToken = default)
        {
            if(@event == null)
                return Result.Fail("Event is null");
            
            var eventType = @event.GetType().GetGenericTypeName();

            var jsonEvent =
                $"\n=============== Executing Event ================\n{JsonConvert.SerializeObject(@event, Formatting.Indented)}\n";
            var jsonMessageContext =
                $"\n=============== With Message Context ================\n{JsonConvert.SerializeObject(domainContext, Formatting.Indented)}\n";

            Print(jsonEvent, jsonMessageContext);

            var result = await _handler.HandleAsync(@event, domainContext, cancellationToken).ConfigureAwait(false);

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