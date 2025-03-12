using Kharazmi.AspNetCore.Core.EventSourcing;

namespace Kharazmi.EventSourcing.EfCore
{
    internal class NullEventUserClaimInfoService : IEventUserClaimInfoService
    {
        public NullEventUserClaimInfoService()
        {
            UserClaimInfo = new UserClaimInfo();
        }

        public UserClaimInfo UserClaimInfo { get; }
    }
}