using System;

 namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class InvalidDomainEventException : AggregateException
    {
        public InvalidDomainEventException(string message) : base(message)
        {
        }
    }
}