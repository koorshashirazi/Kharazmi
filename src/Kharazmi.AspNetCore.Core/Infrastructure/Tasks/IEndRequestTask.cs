using System.Threading.Tasks;

 namespace Kharazmi.AspNetCore.Core.Infrastructure.Tasks
{
    public interface IEndRequestTask 
    {
        Task ExecuteAsync();
    }
}