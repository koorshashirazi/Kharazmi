using System.Threading.Tasks;

 namespace Kharazmi.BackgroundJob
{
    /// <summary>
    /// Base async job class
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    public abstract class AsyncJob<TArg> : IAsyncJob<TArg>
    {
        /// <summary> Ctor </summary>
        protected AsyncJob()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(TArg model);
    }
}