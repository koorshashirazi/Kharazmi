using System;
using System.Text;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToStringFormat(this Exception ex)
        {
            var builder = new StringBuilder();
            builder.AppendLine("An error occurred. ");

            var inner = ex;
            while (inner != null)
            {
                builder.Append("Error Message:");
                builder.AppendLine(inner.Message);
                builder.Append("Stack Trace:");
                builder.AppendLine(inner.StackTrace);
                inner = inner.InnerException;
            }

            return builder.ToString();
        }

       
    }
}