using Kharazmi.AspNetCore.Core.Domain.Commands;

 namespace Kharazmi.AspNetCore.Core.Pipelines
{
    public interface IPipelineCommandHandler<in TCommand> : IPipelineHandler where TCommand : Command
    {
    }
}