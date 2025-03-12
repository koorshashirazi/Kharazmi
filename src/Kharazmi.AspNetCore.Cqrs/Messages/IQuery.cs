using MediatR;

 namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
