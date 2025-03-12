using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Application.Services;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Configuration
{
    public interface IKeyValueService : IApplicationService
    {
        Task SaveValueAsync(string key, string value);
        Task<Maybe<string>> FindValueAsync(string key);
    }
}