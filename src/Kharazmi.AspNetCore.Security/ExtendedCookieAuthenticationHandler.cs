﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Kharazmi.AspNetCore.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtendedCookieAuthenticationHandler : SignInAuthenticationHandler<ExtendedCookieAuthenticationOptions>
    {
        private const string HeaderValueNoCache = "no-cache";
        private const string HeaderValueEpocDate = "Thu, 01 Jan 1970 00:00:00 GMT";
        private const string SessionIdClaim = "Microsoft.AspNetCore.Authentication.Cookies-SessionId";

        private bool _shouldRefresh;
        private bool _signInCalled;
        private bool _signOutCalled;

        private DateTimeOffset? _refreshIssuedUtc;
        private DateTimeOffset? _refreshExpiresUtc;
        private string _sessionKey;
        private Task<AuthenticateResult> _readCookieTask;
        private AuthenticationTicket _refreshTicket;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public ExtendedCookieAuthenticationHandler(
            IOptionsMonitor<ExtendedCookieAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// Added this to overwrite default SessionIdClaim value
        /// </summary>
        public virtual string SessionIdClaimType => Options.SessionIdClaim.IsEmpty() ? SessionIdClaim : Options.SessionIdClaim;

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new CookieAuthenticationEvents Events
        {
            get => (CookieAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Task InitializeHandlerAsync()
        {
            // Cookies needs to finish the response
            Context.Response.OnStarting(FinishResponseAsync);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new instance of the events instance.
        /// </summary>
        /// <returns>A new instance of the events instance.</returns>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new CookieAuthenticationEvents());

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = await EnsureCookieTicket().ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return result;
            }

            var context = new CookieValidatePrincipalContext(Context, Scheme, Options, result.Ticket);
            await Events.ValidatePrincipal(context).ConfigureAwait(false);

            if (context.Principal == null)
            {
                return AuthenticateResult.Fail("No principal.");
            }

            if (context.ShouldRenew)
            {
                RequestRefresh(result.Ticket, context.Principal);
            }

            return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual async Task FinishResponseAsync()
        {
            // Only renew if requested, and neither sign in or sign out was called
            if (!_shouldRefresh || _signInCalled || _signOutCalled)
            {
                return;
            }

            var ticket = _refreshTicket;
            if (ticket != null)
            {
                var properties = ticket.Properties;

                if (_refreshIssuedUtc.HasValue)
                {
                    properties.IssuedUtc = _refreshIssuedUtc;
                }

                if (_refreshExpiresUtc.HasValue)
                {
                    properties.ExpiresUtc = _refreshExpiresUtc;
                }

                if (Options.SessionStore != null && _sessionKey != null)
                {
                    await Options.SessionStore.RenewAsync(_sessionKey, ticket).ConfigureAwait(false);
                    var principal = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[] { new Claim(SessionIdClaimType, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
                            Scheme.Name));
                    ticket = new AuthenticationTicket(principal, null, Scheme.Name);
                }

                var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

                var cookieOptions = BuildCookieOptions();
                if (properties.IsPersistent && _refreshExpiresUtc.HasValue)
                {
                    cookieOptions.Expires = _refreshExpiresUtc.Value.ToUniversalTime();
                }

                Options.CookieManager.AppendResponseCookie(
                    Context,
                    Options.Cookie.Name,
                    cookieValue,
                    cookieOptions);

                await ApplyHeaders(shouldRedirectToReturnUrl: false, properties: properties).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            properties ??= new AuthenticationProperties();

            _signInCalled = true;

            // Process the request cookie to initialize members like _sessionKey.
            await EnsureCookieTicket().ConfigureAwait(false);
            var cookieOptions = BuildCookieOptions();

            var signInContext = new CookieSigningInContext(
                Context,
                Scheme,
                Options,
                user,
                properties,
                cookieOptions);

            DateTimeOffset issuedUtc;
            if (signInContext.Properties.IssuedUtc.HasValue)
            {
                issuedUtc = signInContext.Properties.IssuedUtc.Value;
            }
            else
            {
                issuedUtc = Clock.UtcNow;
                signInContext.Properties.IssuedUtc = issuedUtc;
            }

            if (!signInContext.Properties.ExpiresUtc.HasValue)
            {
                signInContext.Properties.ExpiresUtc = issuedUtc.Add(Options.ExpireTimeSpan);
            }

            await Events.SigningIn(signInContext).ConfigureAwait(false);

            if (signInContext.Properties.IsPersistent)
            {
                var expiresUtc = signInContext.Properties.ExpiresUtc ?? issuedUtc.Add(Options.ExpireTimeSpan);
                signInContext.CookieOptions.Expires = expiresUtc.ToUniversalTime();
            }

            var ticket = new AuthenticationTicket(signInContext.Principal, signInContext.Properties, signInContext.Scheme.Name);

            //
            if (Options.SessionStore != null)
            {
                if (_sessionKey != null)
                {
                    await Options.SessionStore.RemoveAsync(_sessionKey).ConfigureAwait(false);
                }
                _sessionKey = await Options.SessionStore.StoreAsync(ticket).ConfigureAwait(false);
                var principal = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[] { new Claim(SessionIdClaimType, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
                        Options.ClaimsIssuer));
                ticket = new AuthenticationTicket(principal, null, Scheme.Name);
            }

            var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

            Options.CookieManager.AppendResponseCookie(
                Context,
                Options.Cookie.Name,
                cookieValue,
                signInContext.CookieOptions);

            var signedInContext = new CookieSignedInContext(
                Context,
                Scheme,
                signInContext.Principal,
                signInContext.Properties,
                Options);

            await Events.SignedIn(signedInContext).ConfigureAwait(false);

            // Only redirect on the login path
            var shouldRedirect = Options.LoginPath.HasValue && OriginalPath == Options.LoginPath;
            await ApplyHeaders(shouldRedirect, signedInContext.Properties).ConfigureAwait(false);

            Logger.LogInformation($"AuthenticationSchemeSignedIn: {Scheme.Name} was successfully authenticated.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            properties ??= new AuthenticationProperties();

            _signOutCalled = true;

            // Process the request cookie to initialize members like _sessionKey.
            await EnsureCookieTicket().ConfigureAwait(false);
            var cookieOptions = BuildCookieOptions();
            if (Options.SessionStore != null && _sessionKey != null)
            {
                await Options.SessionStore.RemoveAsync(_sessionKey).ConfigureAwait(false);
            }

            var context = new CookieSigningOutContext(
                Context,
                Scheme,
                Options,
                properties,
                cookieOptions);

            await Events.SigningOut(context).ConfigureAwait(false);

            Options.CookieManager.DeleteCookie(
                Context,
                Options.Cookie.Name,
                context.CookieOptions);

            // Only redirect on the logout path
            var shouldRedirect = Options.LogoutPath.HasValue && OriginalPath == Options.LogoutPath;
            await ApplyHeaders(shouldRedirect, context.Properties).ConfigureAwait(false);

            Logger.LogInformation($"AuthenticationSchemeSignedOut: {Scheme.Name} was successfully authenticated.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            var returnUrl = properties.RedirectUri;
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = OriginalPathBase + OriginalPath + Request.QueryString;
            }
            var accessDeniedUri = Options.AccessDeniedPath + QueryString.Create(Options.ReturnUrlParameter, returnUrl);
            var redirectContext = new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, BuildRedirectUri(accessDeniedUri));
            await Events.RedirectToAccessDenied(redirectContext).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var redirectUri = properties.RedirectUri;
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
            }

            var loginUri = Options.LoginPath + QueryString.Create(Options.ReturnUrlParameter, redirectUri);
            var redirectContext = new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, BuildRedirectUri(loginUri));
            await Events.RedirectToLogin(redirectContext).ConfigureAwait(false);
        }

        #region Helpers Method

        private async Task ApplyHeaders(bool shouldRedirectToReturnUrl, AuthenticationProperties properties)
        {
            Response.Headers[HeaderNames.CacheControl] = HeaderValueNoCache;
            Response.Headers[HeaderNames.Pragma] = HeaderValueNoCache;
            Response.Headers[HeaderNames.Expires] = HeaderValueEpocDate;

            if (shouldRedirectToReturnUrl && Response.StatusCode == 200)
            {
                // set redirect uri in order:
                // 1. properties.RedirectUri
                // 2. query parameter ReturnUrlParameter
                //
                // Absolute uri is not allowed if it is from query string as query string is not
                // a trusted source.
                var redirectUri = properties.RedirectUri;
                if (string.IsNullOrEmpty(redirectUri))
                {
                    redirectUri = Request.Query[Options.ReturnUrlParameter];
                    if (string.IsNullOrEmpty(redirectUri) || !IsHostRelative(redirectUri))
                    {
                        redirectUri = null;
                    }
                }

                if (redirectUri != null)
                {
                    await Events.RedirectToReturnUrl(
                        new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, redirectUri)).ConfigureAwait(false);
                }
            }
        }

        private static bool IsHostRelative(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            if (path.Length == 1)
            {
                return path[0] == '/';
            }
            return path[0] == '/' && path[1] != '/' && path[1] != '\\';
        }

        private string GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }

        private Task<AuthenticateResult> EnsureCookieTicket()
        {
            // We only need to read the ticket once
            return _readCookieTask ??= ReadCookieTicket();
        }

        private void CheckForRefresh(AuthenticationTicket ticket)
        {
            var currentUtc = Clock.UtcNow;
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            var allowRefresh = ticket.Properties.AllowRefresh ?? true;
            if (issuedUtc != null && expiresUtc != null && Options.SlidingExpiration && allowRefresh)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

                if (timeRemaining < timeElapsed)
                {
                    RequestRefresh(ticket);
                }
            }
        }

        private void RequestRefresh(AuthenticationTicket ticket, ClaimsPrincipal replacedPrincipal = null)
        {
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (issuedUtc != null && expiresUtc != null)
            {
                _shouldRefresh = true;
                var currentUtc = Clock.UtcNow;
                _refreshIssuedUtc = currentUtc;
                var timeSpan = expiresUtc.Value.Subtract(issuedUtc.Value);
                _refreshExpiresUtc = currentUtc.Add(timeSpan);
                _refreshTicket = CloneTicket(ticket, replacedPrincipal);
            }
        }

        private AuthenticationTicket CloneTicket(AuthenticationTicket ticket, ClaimsPrincipal replacedPrincipal)
        {
            var principal = replacedPrincipal ?? ticket.Principal;
            var newPrincipal = new ClaimsPrincipal();
            foreach (var identity in principal.Identities)
            {
                newPrincipal.AddIdentity(identity.Clone());
            }

            var newProperties = new AuthenticationProperties();
            foreach (var item in ticket.Properties.Items)
            {
                newProperties.Items[item.Key] = item.Value;
            }

            return new AuthenticationTicket(newPrincipal, newProperties, ticket.AuthenticationScheme);
        }

        private async Task<AuthenticateResult> ReadCookieTicket()
        {
            var cookie = Options.CookieManager.GetRequestCookie(Context, Options.Cookie.Name);
            if (string.IsNullOrEmpty(cookie))
            {
                return AuthenticateResult.NoResult();
            }

            var ticket = Options.TicketDataFormat.Unprotect(cookie, GetTlsTokenBinding());
            if (ticket == null)
            {
                return AuthenticateResult.Fail("Unprotect ticket failed");
            }

            if (Options.SessionStore != null)
            {
                var claim = ticket.Principal.Claims.FirstOrDefault(c => c.Type.Equals(SessionIdClaimType));
                if (claim == null)
                {
                    return AuthenticateResult.Fail("SessionId missing");
                }
                _sessionKey = claim.Value;
                ticket = await Options.SessionStore.RetrieveAsync(_sessionKey).ConfigureAwait(false);
                if (ticket == null)
                {
                    return AuthenticateResult.Fail("Identity missing in session store");
                }
            }

            var currentUtc = Clock.UtcNow;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
            {
                if (Options.SessionStore != null)
                {
                    await Options.SessionStore.RemoveAsync(_sessionKey).ConfigureAwait(false);
                }
                return AuthenticateResult.Fail("Ticket expired");
            }

            CheckForRefresh(ticket);

            // Finally we have a valid ticket
            return AuthenticateResult.Success(ticket);
        }

        private CookieOptions BuildCookieOptions()
        {
            var cookieOptions = Options.Cookie.Build(Context);
            // ignore the 'Expires' value as this will be computed elsewhere
            cookieOptions.Expires = null;

            return cookieOptions;
        }

        #endregion Helpers Method
    }
}