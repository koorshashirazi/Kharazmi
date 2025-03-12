using System;

namespace Kharazmi.AspNetCore.Core.Cryptography
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EncryptedAttribute : Attribute
    { }
}