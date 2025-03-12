using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary> </summary>
    public interface IUserContextService
    {
        /// <summary> </summary>
        IHttpContextAccessor HttpContextAccessor { get; }

        /// <summary> </summary>
        HttpContext HttpContext { get; }

        /// <summary> </summary>
        HttpRequest Request { get; }

        /// <summary> </summary>
        ClaimsPrincipal CurrentUser { get; }

        /// <summary> </summary>
        IIdentity CurrentIdentity { get; }

        /// <summary> </summary>
        bool IsAuthenticated { get; }

        /// <summary> </summary>
        string UserDisplayName { get; }

        /// <summary> </summary>
        string UserId { get; }

        /// <summary> </summary>
        string UserName { get; }

        /// <summary> </summary>
        string Email { get; }

        /// <summary> </summary>
        string UserBrowserName { get; }

        /// <summary> </summary>
        string UserIp { get; }

        /// <summary> </summary>
        string TraceId { get; }

        /// <summary> </summary>
        string ConnectionId { get; }

        /// <summary> </summary>
        string RequestPath { get; }

        /// <summary> </summary>
        IReadOnlyCollection<string> Roles { get; }

        /// <summary> </summary>
        IReadOnlyCollection<string> Permissions { get; }

        /// <summary> </summary>
        IReadOnlyList<Claim> Claims { get; }

        /// <summary> </summary>
        CultureInfo GetCurrentCulture { get; }

        /// <summary> </summary>
        CultureInfo GetCurrentUiCulture { get; }

        /// <summary> </summary>
        string CultureName { get; }

        /// <summary> </summary>
        string UiCultureName { get; }

        /// <summary> </summary>
        bool IsAdmin(string adminRoleName, string adminUserName, Func<Claim, bool> predicate);

        /// <summary> </summary>
        bool IsInRole(string role);

        /// <summary> </summary>
        bool HasClaim(Predicate<Claim> predicate);

        /// <summary> </summary>
        Claim FindClaim(string claimType);

        /// <summary> </summary>
        Task<string> CurrentAccessToken();

        /// <summary> </summary>
        Task<string> CurrentRefreshToken();
    }
}