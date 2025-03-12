using System;
using System.Threading.Tasks;

namespace Kharazmi.AspNetCore.Core.Infrastructure.Tasks
{
    public interface IErrorTask
    {
        Task ExecuteAsync(Exception exception);
    }
}