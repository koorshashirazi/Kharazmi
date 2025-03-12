using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Application.Events
{
    public interface IBusinessEventHandler<in TEvent> where TEvent : IBusinessEvent
    {
        Task<Result> HandleAsync(TEvent @event, 
            CancellationToken cancellationToken = default);
    }

    public abstract class BusinessEventHandler<TEvent, TResult> : IBusinessEventHandler<TEvent>
        where TEvent : IBusinessEvent
        where TResult : Result
    {
        public abstract Task<Result> HandleAsync(TEvent @event, 
            CancellationToken cancellationToken = default);
    }
}