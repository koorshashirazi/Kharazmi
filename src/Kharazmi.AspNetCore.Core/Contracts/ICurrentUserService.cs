using Kharazmi.AspNetCore.Core.Dependency;

namespace Kharazmi.AspNetCore.Core.Contracts
{
    public interface ICurrentUserService : ITransientDependency
    {
        long Id { get; }
        string UserName { get; }
        string DisplayName { get; }
        string BrowserName { get; }
        string Ip { get; }
        bool IsAuthenticated { get; }
    }
}