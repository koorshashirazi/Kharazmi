using System;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.Enumerations
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentNullException(string paramName)
            => throw new ArgumentNullException(paramName);

        public static void ThrowArgumentNullOrEmptyException(string paramName)
            => throw new ArgumentException("Argument cannot be null or empty.", paramName);

        public static void ThrowNameNotFoundException<TEnum, TValue>(string name)
            where TEnum : Enumeration<TEnum, TValue>
            where TValue : IEquatable<TValue>, IComparable<TValue>
            => throw new EnumConverterException($"No {typeof(TEnum).Name} with AggregateType \"{name}\" found.");

        public static void ThrowValueNotFoundException<TEnum, TValue>(TValue value)
            where TEnum : Enumeration<TEnum, TValue>
            where TValue : IEquatable<TValue>, IComparable<TValue>
            => throw new EnumConverterException($"No {typeof(TEnum).Name} with Value {value} found.");
    }
}