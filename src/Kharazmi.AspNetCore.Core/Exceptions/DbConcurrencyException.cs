using System;

 namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DbConcurrencyException : DbException
    {
        /// <summary>
        /// 
        /// </summary>
        public DbConcurrencyException() : base(string.Empty, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public DbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}