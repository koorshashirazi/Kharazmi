using System;

 namespace Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest.Exceptions
{
    public class InvalidUrlAggregateException : AggregateException
    {
        public InvalidUrlAggregateException(string message) : base(message)
        {
        }
    }
}