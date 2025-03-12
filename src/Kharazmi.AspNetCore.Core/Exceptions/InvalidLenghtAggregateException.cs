using System;

 namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class InvalidLenghtAggregateException : AggregateException
    {
        public InvalidLenghtAggregateException(string message) : base(message)
        {
        }
    }
}