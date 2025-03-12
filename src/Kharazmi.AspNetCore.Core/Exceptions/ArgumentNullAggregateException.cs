 using System;

  namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class ArgumentNullAggregateException : AggregateException
    {
        public ArgumentNullAggregateException(string message) : base(message)
        {
        }
    }
}