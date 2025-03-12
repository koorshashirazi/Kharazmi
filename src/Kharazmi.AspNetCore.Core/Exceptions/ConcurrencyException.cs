
using System;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    public class ConcurrencyException : AggregateException
    {
        public ConcurrencyException(string message) : base(message)
        {
        }
    }
}