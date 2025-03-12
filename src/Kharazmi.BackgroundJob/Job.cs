namespace Kharazmi.BackgroundJob
{
    /// <summary>
    /// Base job class
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    public abstract class Job<TArg> : IJob<TArg>
    {
        /// <summary> Ctor </summary>
        protected Job()
        {
        }

        /// <summary> </summary>
        /// <param name="model"></param>
        public abstract void Execute(TArg model);
    }
}