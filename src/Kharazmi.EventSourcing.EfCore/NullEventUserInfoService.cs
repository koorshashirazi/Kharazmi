using Kharazmi.AspNetCore.Core.EventSourcing;

namespace Kharazmi.EventSourcing.EfCore
{
    public sealed class NullEventUserInfoService : IEventUserInfoService
    {
        public NullEventUserInfoService()
        {
            UserInfo = new UserInfo();
        }

        public UserInfo UserInfo { get; }
    }
}