using System;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class MetaDataNullException : AggregateException
    {
        public MetaDataNullException(string message) : base(message)
        {
        }

        public static T ThrowIfIsNull<T>(T argument, string parameterName)
           where T : class => argument ?? throw new MetaDataNullException(parameterName);
    }
}