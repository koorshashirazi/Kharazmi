using Kharazmi.AspNetCore.Core.Exceptions;

 namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Exceptions
{
    public class MediaFileAlreadyExisteDomainException : FrameworkException
    {
        public MediaFileAlreadyExisteDomainException(string message) : base(message)
        {
        }
    }
}