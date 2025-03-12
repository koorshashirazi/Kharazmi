using System;

namespace Kharazmi.AspNetCore.Core.Exceptions;

/// <summary>
/// Exception thrown when there is an error creating an instance.
/// </summary>
public sealed class InstanceException : Exception
{
    public InstanceException()
    {
    }

    public InstanceException(Type type) : base($"Failed to create an instance of type {type}")
    {
    }

    public InstanceException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}