using Kharazmi.AspNetCore.Core.Bus;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    public class PipelineBuilder
    {
        public BusBuilder Builder { get; }
        public IServiceCollection Services { get;  }

        public PipelineBuilder(BusBuilder builder)
        {
            Builder = builder;
            Services = builder.Services;
        }
    }
}