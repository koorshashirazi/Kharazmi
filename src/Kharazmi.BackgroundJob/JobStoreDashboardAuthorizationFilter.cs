using Hangfire.Dashboard;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.BackgroundJob
{
    internal class JobStoreDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var services = context.GetHttpContext().RequestServices;
            var authorization = services.GetRequiredService<IAuthorizationService>();
            var jobStoreOptions = services.GetRequiredService<JobStoreOptions>();
            if (jobStoreOptions.PolicyName.IsEmpty())
                return false;
            var user = context.GetHttpContext()?.User;
            if (user == null)
                return false;
            var allowed = AsyncHelper.RunSync(async () => await authorization.AuthorizeAsync(user, jobStoreOptions.PolicyName).ConfigureAwait(false));
            return allowed.Succeeded;
        }
    }
}