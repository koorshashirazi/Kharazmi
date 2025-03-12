using Kharazmi.AspNetCore.Core.Exceptions;

 namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Exceptions
{
    public class MediaFileAlreadyExisteDomainException : FrameworkException
    {
        public MediaFileAlreadyExisteDomainException(string message) : base(message)
        {
        }
    }
}