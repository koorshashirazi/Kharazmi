namespace Kharazmi.AspNetCore.Core.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRetryConfig
    {
        /// <summary>
        /// 
        /// </summary>
        int Attempt { get; set; }
        /// <summary>
        /// In MileSecond
        /// </summary>
        int Min { get; set; }
        /// <summary>
        /// In MileSecond
        /// </summary>
        int Max { get; set; }
    }
}