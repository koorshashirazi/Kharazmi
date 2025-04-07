using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    public interface IValidationHandler<in TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request,
            CancellationToken cancellationToken = default);
    }

    public interface ICommandValidationHandler<in TCommand> : IValidationHandler<TCommand, Result>
        where TCommand : DomainCommand
    {
    }
}