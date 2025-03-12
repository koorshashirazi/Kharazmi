using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    /// <summary></summary>
    public sealed record UserInfoOptions
    {
        public UserInfoOptions() : this("sub", "name")
        {
        }

        /// <summary></summary>
        public UserInfoOptions(string userIdClaimType, string userNameClaimType)
        {
            UserIdClaimType = userIdClaimType;
            UserNameClaimType = userNameClaimType;
        }

        public string UserIdClaimType { get; set; }

        public string UserNameClaimType { get; set; }
    }

    /// <summary></summary>
    public sealed record UserInfo(bool IsAuthenticated, string? UserId, string? UserName)
    {
        public UserInfo() : this(false, null, null)
        {
        }

        public bool IsAuthenticated { get; } = IsAuthenticated;
        public string? UserId { get; } = UserId;
        public string? UserName { get; } = UserName;
    }

    /// <summary></summary>
    public sealed class UserClaimInfoOptions(string[] specifiedClaimTypes)
    {
        public UserClaimInfoOptions() : this([])
        {
        }

        public IReadOnlyCollection<string> SpecifiedClaimTypes { get; set; } = specifiedClaimTypes;
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed record UserClaim(string Type, string Value)
    {
        public string Type { get; } = Type;
        public string Value { get; } = Value;
    }

    /// <summary></summary>
    public sealed record UserClaimInfo
    {
        public UserClaimInfo() : this([])
        {
        }

        public UserClaimInfo(UserClaim[] specifiedUserClaim)
        {
            SpecifiedUserClaim = specifiedUserClaim;
        }

        public IReadOnlyCollection<UserClaim> SpecifiedUserClaim { get; }
    }

    /// <summary></summary>
    public sealed record RequestInfoOptions
    {
        public bool IncludeFormVariables { get; set; }
    }

    /// <summary></summary>
    public sealed record RequestInfo
    {
        public RequestInfo()
        {
        }

        public RequestInfo(string? traceIdentifier, string? uriValue, string? httpMethod, string? agent,
            string? remoteIpAddress, string? localIpAddress, Dictionary<string, string>? formVariables)
        {
            TraceIdentifier = traceIdentifier;
            UriValue = uriValue;
            HttpMethod = httpMethod;
            Agent = agent;
            RemoteIpAddress = remoteIpAddress;
            LocalIpAddress = localIpAddress;
            FormVariables = formVariables;
        }

        public string? TraceIdentifier { get; set; }
        public string? UriValue { get; set; }
        public string? HttpMethod { get; set; }
        public string? Agent { get; set; }
        public string? RemoteIpAddress { get; set; }
        public string? LocalIpAddress { get; set; }
        public IReadOnlyDictionary<string, string>? FormVariables { get; set; }
    }
}