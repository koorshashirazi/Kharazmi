using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Bus
{
    /// <summary>
    /// 
    /// </summary>
    public class BusBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        public CoreFrameworkBuilder Builder { get; }
        /// <summary>
        /// 
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public BusBuilder(CoreFrameworkBuilder builder)
        {
            Services = builder.Services;
            Builder = builder;
        }
    }
}