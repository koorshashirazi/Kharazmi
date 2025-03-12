using System;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class MetaDataNotFindKeyException : AggregateException
    {
        public MetaDataNotFindKeyException(string message) : base(message)
        {
        }
    }
}