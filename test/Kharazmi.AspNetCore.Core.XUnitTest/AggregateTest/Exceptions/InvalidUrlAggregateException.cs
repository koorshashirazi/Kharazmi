using System;

 namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Exceptions
{
    public class InvalidUrlAggregateException : AggregateException
    {
        public InvalidUrlAggregateException(string message) : base(message)
        {
        }
    }
}