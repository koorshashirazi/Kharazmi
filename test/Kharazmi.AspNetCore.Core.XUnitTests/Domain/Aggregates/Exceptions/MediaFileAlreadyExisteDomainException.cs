using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.Exceptions
{
    public class MediaFileAlreadyExisteDomainException : FrameworkException
    {
        public MediaFileAlreadyExisteDomainException(string message) : base(message)
        {
        }
    }
}