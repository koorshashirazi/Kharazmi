namespace Kharazmi.AspNetCore.Core.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransientRetryDelay
    {
        /// <summary>
        /// 
        /// </summary>
        RetryDelay TransientRetryDelay { get; }
    }
}