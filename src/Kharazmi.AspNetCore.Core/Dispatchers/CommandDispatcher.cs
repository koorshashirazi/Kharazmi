using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    public interface IDomainCommandHandlerTypeFactory
    {
        Type GetHandlerType(Type commandType);
    }

    internal sealed class DomainCommandHandlerTypeFactory : IDomainCommandHandlerTypeFactory
    {
        // TODO: Limit cache size to avoid memory overflow
        private static readonly ConcurrentDictionary<Type, Type> TypeCache = new();

        public Type GetHandlerType(Type commandType)
        {
            return TypeCache.GetOrAdd(commandType, type => typeof(IDomainCommandHandler<>).MakeGenericType(type));
        }
    }

    public interface IDomainCommandDispatcher
    {
        Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken token = default)
            where TCommand : class, IDomainCommand;
    }

    internal sealed class DomainCommandDispatcher : IDomainCommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDomainCommandHandlerTypeFactory _handlerTypeFactory;

        public DomainCommandDispatcher(IServiceProvider serviceProvider,
            IDomainCommandHandlerTypeFactory handlerTypeFactory)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _handlerTypeFactory = handlerTypeFactory ?? throw new ArgumentNullException(nameof(handlerTypeFactory));
        }

        public Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken token = default)
            where TCommand : class, IDomainCommand
        {
            return ExceptionHandler.ExecuteResultAsync(async domainCommand =>
            {
                var commandHandler = _serviceProvider.GetRequiredService<IDomainCommandHandler<TCommand>>();
                return await commandHandler.HandleAsync(domainCommand, token).ConfigureAwait(false);
            }, state: command, nameof(command), onError: e => Task.FromResult(Result.Fail($"Unable to send the {nameof(command)}").WithException(e)));
        }
    }
}