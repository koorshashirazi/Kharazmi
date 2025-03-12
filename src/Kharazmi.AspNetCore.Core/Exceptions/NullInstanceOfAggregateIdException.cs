using System;

 namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class NullInstanceOfAggregateIdException : AggregateException
    {
        public NullInstanceOfAggregateIdException(string message) : base(message)
        {
        }
    }
}