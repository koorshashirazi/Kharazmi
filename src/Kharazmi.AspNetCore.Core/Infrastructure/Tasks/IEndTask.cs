using System.Threading.Tasks;

 namespace Kharazmi.AspNetCore.Core.Infrastructure.Tasks
{
    public interface IEndTask 
    {
        Task ExecuteAsync();
    }
}