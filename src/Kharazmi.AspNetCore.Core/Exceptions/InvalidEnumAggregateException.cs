using System;

 namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class InvalidEnumAggregateException : AggregateException
    {
        public InvalidEnumAggregateException(string message) : base(message)
        {
        }
    }
}