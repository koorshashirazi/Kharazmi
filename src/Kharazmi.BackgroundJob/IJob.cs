namespace Kharazmi.BackgroundJob
{
    /// <summary>
    /// To create a job
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    public interface IJob<in TArg>
    {
        /// <summary>
        /// To Execute a jab
        /// </summary>
        /// <param name="model"></param>
        void Execute(TArg model);
    }
}