using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Cqrs
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICqrsBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CqrsBuilder : ICqrsBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public CqrsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// 
        /// </summary>
        public IServiceCollection Services { get; }
    }
}