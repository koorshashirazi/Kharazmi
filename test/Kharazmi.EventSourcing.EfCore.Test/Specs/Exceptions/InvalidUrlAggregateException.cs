using System;

 namespace Kharazmi.EventSourcing.EfCore.Test.Specs.Exceptions
{
    public class InvalidUrlAggregateException : AggregateException
    {
        public InvalidUrlAggregateException(string message) : base(message)
        {
        }
    }
}