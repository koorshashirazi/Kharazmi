using Kharazmi.AspNetCore.Core.EventSourcing;

namespace Kharazmi.EventSourcing.EfCore
{
    public sealed class NullEventRequestInfoService : IEventRequestInfoService
    {
        public NullEventRequestInfoService()
        {
            RequestInfo = new RequestInfo();
        }

        public RequestInfo RequestInfo { get; }
    }
}