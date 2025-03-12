using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.BackgroundJob
{
    /// <summary> </summary>
    internal class JobStoreDashboardMiddleware
    {
        #region Ctor

        /// <summary> </summary>
        public JobStoreDashboardMiddleware(
            RequestDelegate nextRequestDelegate,
            JobStorage storage,
            DashboardOptions options,
            RouteCollection routes,
            JobStoreOptions jobStoreOptions)
        {
            _nextRequestDelegate = nextRequestDelegate;
            _jobStorage = storage;
            _dashboardOptions = options;
            _routeCollection = routes;
            _jobStoreOptions = jobStoreOptions;
        }

        #endregion

        /// <summary> </summary>
        public async Task Invoke(HttpContext httpContext)
        {
            var dashboardContext =
                new AspNetCoreDashboardContext(_jobStorage, _dashboardOptions, httpContext);

            var findResult = _routeCollection.FindDispatcher(httpContext.Request.Path.Value);
            if (findResult == null)
            {
                await _nextRequestDelegate.Invoke(httpContext).ConfigureAwait(false);
                return;
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                var prop = new AuthenticationProperties
                {
                    RedirectUri = _jobStoreOptions.PathMatch
                };
                await httpContext.ChallengeAsync(prop).ConfigureAwait(false);
                return;
            }

            if (_dashboardOptions.Authorization.Any(filter => filter.Authorize(dashboardContext) == false))
            {
                var isAuthenticated = httpContext.User?.Identity?.IsAuthenticated;
                httpContext.Response.StatusCode = isAuthenticated == true
                    ? (int) HttpStatusCode.Forbidden
                    : (int) HttpStatusCode.Unauthorized;
                return;
            }

            dashboardContext.UriMatch = findResult.Item2;
            await findResult.Item1.Dispatch(dashboardContext).ConfigureAwait(false);
        }

        #region Private

        private readonly DashboardOptions _dashboardOptions;
        private readonly JobStorage _jobStorage;
        private readonly RequestDelegate _nextRequestDelegate;
        private readonly RouteCollection _routeCollection;
        private readonly JobStoreOptions _jobStoreOptions;

        #endregion
    }
}