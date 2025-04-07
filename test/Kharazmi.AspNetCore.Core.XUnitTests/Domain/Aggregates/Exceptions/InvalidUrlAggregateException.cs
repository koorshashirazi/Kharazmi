namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.Exceptions
{
    public class InvalidUrlAggregateException : AggregateException
    {
        public InvalidUrlAggregateException(string message) : base(message)
        {
        }
    }
}