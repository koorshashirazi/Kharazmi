using System;

namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    public interface IMergeable
    {
        Guid TrackerId { get; }
    }
}