using System;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class MetaDataException : AggregateException
    {
        public MetaDataException(string message) : base(message)
        {
        }
        
        public MetaDataException(string message, Exception e) : base(message, e)
        {
        }
    }
}