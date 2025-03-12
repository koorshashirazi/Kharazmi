using System;
using System.Threading.Tasks;

namespace Kharazmi.BackgroundJob
{
    /// <summary>
    /// Job service
    /// </summary>
    public interface IBackgroundService
    {
        /// <summary>
        /// Add job to queue 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="priority"></param>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <returns>return job id</returns>
        Task<string> AddJobToQueueAsync<TJob, TArg>(TArg model, JobPriority priority = JobPriority.Normal)
            where TJob : IAsyncJob<TArg>;

        /// <summary>
        /// Add job to queue 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="priority"></param>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <returns>Return jon id</returns>
        string AddJobToQueue<TJob, TArg>(TArg model, JobPriority priority = JobPriority.Normal)
            where TJob : IJob<TArg>;

        /// <summary>
        /// Try to execute an unsuccessful job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="fromState"></param>
        /// <returns></returns>
        Task<bool> RequeueJobAsync(string jobId, string fromState);

        /// <summary>
        /// Try to execute an unsuccessful job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="fromState"></param>
        /// <returns></returns>
        bool RequeueJob(string jobId, string fromState);

        /// <summary>
        /// Remove job from queue 
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns>If success return true, otherwise false</returns>
        Task<bool> RemoveJobFromQueueAsync(string jobId);

        /// <summary>
        /// Remove job from queue
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns>If success return true, otherwise false</returns>
        bool RemoveJobFromQueue(string jobId);

        /// <summary>
        /// Schedule Job
        /// </summary>
        /// <param name="model"></param>
        /// <param name="priority"></param>
        /// <param name="delay"></param>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <returns>Return job id</returns>
        Task<string> ScheduleJobAsync<TJob, TArg>(TArg model, TimeSpan delay, JobPriority priority = JobPriority.Normal)
            where TJob : IAsyncJob<TArg>;

        /// <summary>
        /// Schedule Job
        /// </summary>
        /// <param name="model"></param>
        /// <param name="priority"></param>
        /// <param name="delay"></param>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <returns>Return job id</returns>
        string ScheduleJob<TJob, TArg>(TArg model, TimeSpan delay, JobPriority priority = JobPriority.Normal)
            where TJob : IJob<TArg>;
    }
}