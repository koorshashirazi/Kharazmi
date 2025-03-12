using System;
using System.Data;

namespace Kharazmi.AspNetCore.Core.Transaction
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TransactionOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }
    }
}