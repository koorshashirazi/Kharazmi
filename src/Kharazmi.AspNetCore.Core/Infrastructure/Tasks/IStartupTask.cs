using Microsoft.Extensions.DependencyInjection;

 namespace Kharazmi.AspNetCore.Core.Infrastructure.Tasks
{
    public interface IStartupTask
    {
        void ConfigureServices(IServiceCollection services);
        void Execute();
    }
}