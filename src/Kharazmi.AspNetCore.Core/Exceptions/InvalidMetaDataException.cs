using System;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class InvalidMetaDataException : AggregateException
    {
        public InvalidMetaDataException(string message) : base(message)
        {
        }
    }
}