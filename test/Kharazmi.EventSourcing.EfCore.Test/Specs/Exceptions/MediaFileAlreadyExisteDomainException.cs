using Kharazmi.AspNetCore.Core.Exceptions;

 namespace Kharazmi.EventSourcing.EfCore.Test.Specs.Exceptions
{
    public class MediaFileAlreadyExisteDomainException : FrameworkException
    {
        public MediaFileAlreadyExisteDomainException(string message) : base(message)
        {
        }
    }
}