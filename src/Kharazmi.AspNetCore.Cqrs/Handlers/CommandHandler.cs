using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Cqrs.Messages;
using MediatR;

namespace Kharazmi.AspNetCore.Cqrs.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result> where TCommand : DomainCommand
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : DomainCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<Result> Handle(TCommand command, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static Result Ok() => Result.Ok();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected static Result Fail(string message) => Result.Fail(message);
    }
}