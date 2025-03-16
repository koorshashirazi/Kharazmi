using System;

 namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Exceptions
{
    public class InvalidUrlAggregateException : AggregateException
    {
        public InvalidUrlAggregateException(string message) : base(message)
        {
        }
    }
}