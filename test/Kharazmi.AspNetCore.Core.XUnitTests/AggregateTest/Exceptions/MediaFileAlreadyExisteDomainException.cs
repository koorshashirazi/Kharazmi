using Kharazmi.AspNetCore.Core.Exceptions;

 namespace Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest.Exceptions
{
    public class MediaFileAlreadyExisteDomainException : FrameworkException
    {
        public MediaFileAlreadyExisteDomainException(string message) : base(message)
        {
        }
    }
}