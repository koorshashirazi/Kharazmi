using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    internal sealed class LoggerCommandPipeline<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly ILogger<LoggerCommandPipeline<TCommand>> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="logger"></param>
        public LoggerCommandPipeline(ICommandHandler<TCommand> handler, ILogger<LoggerCommandPipeline<TCommand>> logger)
        {
            _handler = handler;
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Result> HandleAsync(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken)
        {
            var jsonCommand = JsonConvert.SerializeObject(command, Formatting.Indented);
            var jsonMessageContext = JsonConvert.SerializeObject(domainContext, Formatting.Indented);
            _logger.LogInformation($"\n=============== Executing Command ================\n{jsonCommand}\n");
            _logger.LogInformation($"\n=============== With Message Context ================\n{jsonMessageContext}\n");
            return _handler.HandleAsync(command, domainContext, cancellationToken);
        }
    }
}