using System.Threading.Tasks;

 namespace Kharazmi.BackgroundJob
{
    /// <summary>
    /// To create an async job
    /// </summary>
    public interface IAsyncJob<in TArg>
    {
        /// <summary>
        /// To execute a job
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ExecuteAsync(TArg model);
    }
}