using System.Threading.Tasks;
using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.BackgroundJob
{
    internal class JobExceptionFilterAttribute : JobFilterAttribute, IServerFilter, IElectStateFilter
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is FailedState failedState)
            {
                Logger.WarnFormat(
                    "Job `{0}` has been failed due to an exception `{1}`",
                    context.BackgroundJob.Id,
                    failedState.Exception);
            }
        }

        public void OnPerforming(PerformingContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                if (filterContext.Exception.InnerException is DomainException domainException)
                {
                    Parallel.ForEach(domainException.ExceptionErrors,
                        error => Logger.Error(
                            $"Job Id: {filterContext.BackgroundJob.Id} , Error with category: {error.Category}, message: {error.Message}"));
                }
                else
                {
                    var errorJsonMessage = filterContext.Exception.AsJsonException();

                    Logger.Error($"Job Id: {filterContext.BackgroundJob.Id} , Error Message: {errorJsonMessage}");
                }

                filterContext.CancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}