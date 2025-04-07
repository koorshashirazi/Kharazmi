using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    public interface IPipelineCommandHandler<in TCommand> : IPipelineHandler where TCommand : DomainCommand
    {
    }
}