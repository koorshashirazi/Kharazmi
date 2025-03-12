using System;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class EventStoreNullException : AggregateException
    {
        public EventStoreNullException(string message) : base(message)
        {
        }

        public static T ThrowIfIsNull<T>(T argument, string parameterName)
            where T : class => argument ?? throw new EventStoreNullException(parameterName);
    }
}