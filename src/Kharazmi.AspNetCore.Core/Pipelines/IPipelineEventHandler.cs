using Kharazmi.AspNetCore.Core.Domain.Events;

 namespace Kharazmi.AspNetCore.Core.Pipelines
{
    public interface IPipelineEventHandler<in TEvent>: IPipelineHandler where TEvent : class, IDomainEvent
    {
        
    }
}