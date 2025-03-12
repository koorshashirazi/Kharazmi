using System;
using System.Data;

namespace Kharazmi.AspNetCore.Core.Transaction
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class TransactionalAttribute : Attribute
    {
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;
        public TimeSpan? Timeout { get; set; }
    }
}