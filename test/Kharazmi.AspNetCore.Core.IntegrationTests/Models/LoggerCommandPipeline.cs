using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Models
{
    internal sealed class LoggerDomainCommandPipeline<TCommand> : DomainCommandHandler<TCommand>
        where TCommand : class, IDomainCommand
    {
        private readonly IDomainCommandHandler<TCommand> _handler;
        private readonly ILogger<LoggerDomainCommandPipeline<TCommand>> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="logger"></param>
        public LoggerDomainCommandPipeline(IDomainCommandHandler<TCommand> handler,
            ILogger<LoggerDomainCommandPipeline<TCommand>> logger)
        {
            _handler = handler;
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public override Task<Result> HandleAsync(TCommand command, CancellationToken token = default)
        {
            var jsonCommand = JsonConvert.SerializeObject(command, Formatting.Indented);

            _logger.LogInformation($"\n=============== Executing Command ================\n{jsonCommand}\n");
            return _handler.HandleAsync(command, token);
        }
    }
}