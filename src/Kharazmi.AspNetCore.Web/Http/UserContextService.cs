using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using NextGen.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary> </summary>
    public class UserContextService : IUserContextService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary> </summary>
        public UserContextService(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            HttpContextAccessor = Ensure.ArgumentIsNotNull(httpContextAccessor, nameof(httpContextAccessor));
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommonHttpClientFactory HttpClientFactory =>
            _serviceProvider.GetService<ICommonHttpClientFactory>();

        /// <summary> </summary>
        public IHttpContextAccessor HttpContextAccessor { get; }

        /// <summary> </summary>
        public HttpContext HttpContext => HttpContextAccessor?.HttpContext;

        /// <summary> </summary>
        public HttpRequest Request => HttpContext?.Request;

        /// <summary> </summary>
        public ClaimsPrincipal CurrentUser => HttpContext?.User;

        /// <summary> </summary>
        public IIdentity CurrentIdentity => CurrentUser?.Identity;

        /// <summary> </summary>
        public bool IsAuthenticated => CurrentUser?.Identity?.IsAuthenticated ?? false;

        /// <summary> </summary>
        public string UserDisplayName => CurrentIdentity?.FindUserDisplayName();

        /// <summary> </summary>
        public string UserId => CurrentIdentity?.FindUserId();

        /// <summary> </summary>
        public string UserName => CurrentIdentity?.FindUserName();

        /// <summary> </summary>
        public string Email => CurrentIdentity?.FindUserEmail();

        /// <summary> </summary>
        public string UserBrowserName => HttpContext?.FindUserAgent();

        /// <summary> </summary>
        public string UserIp => HttpContext?.FindUserIP();

        /// <summary> </summary>
        public string TraceId => HttpContext?.TraceIdentifier;

        /// <summary> </summary>
        public string ConnectionId => HttpContext?.Connection.Id;

        /// <summary> </summary>
        public string RequestPath => HttpContext?.Request.Path.ToString();

        /// <summary> </summary>
        public IReadOnlyCollection<string> Roles => CurrentIdentity?.GetUserRoles();

        /// <summary> </summary>
        public IReadOnlyCollection<string> Permissions => CurrentIdentity?.GetUserPermissions();

        /// <summary> </summary>
        public IReadOnlyList<Claim> Claims => CurrentUser?.Claims.ToList();

        /// <summary> </summary>
        public bool IsAdmin(string adminRoleName, string adminUserName, Func<Claim, bool> predicate) =>
            Claims.Any(predicate) && IsInRole(adminRoleName) && UserName.Equals(adminUserName);

        /// <summary> </summary>
        public bool IsInRole(string role) =>
            CurrentUser?.IsInRole(role) ?? false;


        /// <summary> </summary>
        public bool HasClaim(Predicate<Claim> predicate) =>
            CurrentUser?.HasClaim(predicate) ?? false;


        /// <summary> </summary>
        public Claim FindClaim(string claimType) =>
            CurrentUser?.FindFirst(claimType);


        /// <summary> </summary>
        public CultureInfo GetCurrentCulture =>
            HttpContext?.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture ??
            System.Threading.Thread.CurrentThread.CurrentCulture;

        /// <summary> </summary>
        public CultureInfo GetCurrentUiCulture =>
            HttpContext?.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture ??
            System.Threading.Thread.CurrentThread.CurrentUICulture;

        /// <summary> </summary>
        public string CultureName => GetCurrentCulture?.Name ?? "";

        /// <summary> </summary>

        public string UiCultureName => GetCurrentUiCulture?.Name ?? "";

        /// <summary> </summary>
        public async Task<string> CurrentAccessToken()
        {
            return IsAuthenticated
                ? await HttpContext.GetTokenAsync("access_token").ConfigureAwait(false) ?? string.Empty
                : string.Empty;
        }

        /// <summary> </summary>
        public async Task<string> CurrentRefreshToken()
        {
            return IsAuthenticated
                ? await HttpContext.GetTokenAsync("refresh_token").ConfigureAwait(false) ?? string.Empty
                : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<Result> RevokeAccessToken(DiscoveryTokenOptions options)
        {
            if (HttpClientFactory == null)
                return Result.Fail(
                    $"The {nameof(ICommonHttpClientFactory)} service has not been registered yet. Use WebFramework with method [WithHttpClientFactory]");

            Ensure.ArgumentIsNotNull(options, nameof(options));

            var accessToken = await CurrentAccessToken().ConfigureAwait(false);
            if (accessToken.IsEmpty())
                return Result.Fail("RevokeAccessTokenFailError");

            var client = HttpClientFactory.GetOrCreate(options.AuthorityUrl);

            var discovery = await HttpClientFactory.GetDiscoveryResponseAsync(options.AuthorityUrl)
                .ConfigureAwait(false);

            if (discovery.IsError)
            {
                return Result.Fail(FriendlyResultMessage.With("RevokeAccessTokenFailError"))
                    .WithInternalMessages(InternalResultMessage.With(nameof(UserContextService), discovery.Error));
            }

            var response = await client.RevokeTokenAsync(
                new TokenRevocationRequest
                {
                    Address = discovery.RevocationEndpoint,
                    ClientId = options.ClientId,
                    ClientSecret = options.ClientSecret,
                    TokenTypeHint = OidcConstants.TokenResponse.AccessToken,
                    Token = accessToken
                }).ConfigureAwait(false);

            if (response.IsError)
            {
                return Result
                    .Fail(FriendlyResultMessage.With("RevokeAccessTokenFailError"))
                    .WithInternalMessages(InternalResultMessage.With(nameof(UserContextService), response.Error));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<Result> RevokeRefreshToken(DiscoveryTokenOptions options)
        {
            if (HttpClientFactory == null)
                return Result.Fail(
                    $"The {nameof(ICommonHttpClientFactory)} service has not been registered yet. Use WebFramework with method [WithHttpClientFactory]");

            var refreshToken = await CurrentRefreshToken().ConfigureAwait(false);

            if (refreshToken.IsEmpty())
                return Result.Fail("RevokeRefreshTokenFailError");

            var discovery = await HttpClientFactory.GetDiscoveryResponseAsync(options.AuthorityUrl)
                .ConfigureAwait(false);

            if (discovery.IsError)
            {
                return Result
                    .Fail(FriendlyResultMessage.With("RevokeAccessTokenFailError"))
                    .WithInternalMessages(InternalResultMessage.With(nameof(UserContextService), discovery.Error));
            }

            var client = HttpClientFactory.GetOrCreate(options.AuthorityUrl);

            var response = await client.RevokeTokenAsync(
                new TokenRevocationRequest
                {
                    Address = discovery.RevocationEndpoint,
                    ClientId = options.ClientId,
                    ClientSecret = options.ClientSecret,
                    TokenTypeHint = OidcConstants.TokenResponse.RefreshToken,
                    Token = refreshToken
                }).ConfigureAwait(false);

            if (response.IsError)
            {
                return Result
                    .Fail(FriendlyResultMessage.With("RevokeAccessTokenFailError"))
                    .WithInternalMessages(InternalResultMessage.With(nameof(UserContextService), response.Error));
            }

            return Result.Ok();
        }
    }
}