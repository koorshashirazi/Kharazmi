using System;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="domainContext"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        Task<Result> SendAsync<TCommand>(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken = default) where TCommand : ICommand;
    }

    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<Result> SendAsync<TCommand>(TCommand command, DomainContext domainContext,
            CancellationToken cancellationToken = default) where TCommand : ICommand
            => _serviceProvider.GetService<ICommandHandler<TCommand>>()
                .HandleAsync(command, domainContext ?? DomainContext.Empty, cancellationToken);
    }
}