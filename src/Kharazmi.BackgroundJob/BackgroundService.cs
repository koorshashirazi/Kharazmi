using System;
using System.Threading.Tasks;
using Hangfire;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Threading;
using MongoDB.Driver.Core.Misc;

namespace Kharazmi.BackgroundJob
{
    /// <summary> </summary>
    public class BackgroundService : IBackgroundService
    {
        private readonly IBackgroundJobClient _jobClient;

        /// <summary> </summary>
        public BackgroundService(IBackgroundJobClient jobClient)
        {
            _jobClient = Ensure.IsNotNull(jobClient, nameof(jobClient));
        }

        /// <summary> </summary>
        public Task<string> AddJobToQueueAsync<TJob, TArg>(TArg model, JobPriority priority = JobPriority.Normal)
            where TJob : IAsyncJob<TArg>
        {
            if (model == null) return Task.FromResult("");
            var jobId = _jobClient.Enqueue<TJob>(job => job.ExecuteAsync(model));
            return Task.FromResult(jobId);
        }

        /// <summary> </summary>
        public string AddJobToQueue<TJob, TArg>(TArg model, JobPriority priority = JobPriority.Normal)
            where TJob : IJob<TArg>
        {
            if (model == null) return "";
            var jobId = _jobClient.Enqueue<TJob>(job => job.Execute(model));
            return jobId;
        }

        /// <summary> </summary>
        public Task<bool> RequeueJobAsync(string jobId, string fromState)
        {
            return jobId.IsEmpty() ? Task.FromResult(false) : Task.Run(() => _jobClient.Requeue(fromState, fromState));
        }

        /// <summary> </summary>
        public bool RequeueJob(string jobId, string fromState)
        {
            return AsyncHelper.RunSync(() => RequeueJobAsync(jobId, fromState));
        }

        /// <summary> </summary>
        public Task<bool> RemoveJobFromQueueAsync(string jobId)
        {
            return jobId.IsEmpty() ? Task.FromResult(false) : Task.Run(() => _jobClient.Delete(jobId));
        }

        /// <summary> </summary>
        public bool RemoveJobFromQueue(string jobId)
        {
            return AsyncHelper.RunSync(() => RemoveJobFromQueueAsync(jobId));
        }

        /// <summary> </summary>
        public Task<string> ScheduleJobAsync<TJob, TArg>(TArg model, TimeSpan delay,
            JobPriority priority = JobPriority.Normal) where TJob : IAsyncJob<TArg>
        {
            if (model == null) return Task.FromResult("");
            var jobId = _jobClient.Schedule<TJob>(job => job.ExecuteAsync(model), delay);
            return Task.FromResult(jobId);
        }

        /// <summary> </summary>
        public string ScheduleJob<TJob, TArg>(TArg model, TimeSpan delay,
            JobPriority priority = JobPriority.Normal) where TJob : IJob<TArg>
        {
            if (model == null) return "";
            var jobId = _jobClient.Schedule<TJob>(job => job.Execute(model), delay);
            return jobId;
        }
    }
}