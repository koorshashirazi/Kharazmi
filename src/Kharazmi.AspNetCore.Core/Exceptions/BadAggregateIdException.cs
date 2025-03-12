using System;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class BadAggregateIdException : AggregateException
    {
        public BadAggregateIdException(string message) : base(message)
        {
        }
    }
}