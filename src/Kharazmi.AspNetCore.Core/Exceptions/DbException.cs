using System;

 namespace Kharazmi.AspNetCore.Core.Exceptions
{
    [Serializable]
    public class DbException : FrameworkException
    {
        public DbException(string message, Exception innerException) 
        : base(message, innerException)
        {
        }
    }
}