using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.GuardToolkit
{
    /// <summary>
    /// Provides assertion and validation methods for ensuring correct parameters and state.
    /// </summary>
    public static partial class Ensure
    {
        #region Constants

        private const string AgainstMessage = "Assertion evaluation failed with 'false'.";
        private const string ImplementsMessage = "Type '{0}' must implement type '{1}'.";
        private const string InheritsFromMessage = "Type '{0}' must inherit from type '{1}'.";
        private const string IsTypeOfMessage = "Type '{0}' must be of type '{1}'.";
        private const string IsEqualMessage = "Compared objects must be equal.";
        private const string IsPositiveMessage = "Argument '{0}' must be a positive value. Value: '{1}'.";
        private const string IsTrueMessage = "True expected for '{0}' but the condition was False.";
        private const string NotNegativeMessage = "Argument '{0}' cannot be a negative value. Value: '{1}'.";
        private const string IsSubclassOfMessage = "Type '{0}' must be a subclass of type '{1}'.";

        #endregion

        #region Null checks

        /// <summary>
        /// Ensures the given argument is not null.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="callerMemberName"></param>
        /// <returns>The argument value.</returns>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        [DebuggerStepThrough]
        public static TArgument ArgumentIsNotNull<TArgument>(TArgument? argument, string? parameterName = null,
            [CallerMemberName] string? callerMemberName = null)
        {
            parameterName ??= callerMemberName ?? nameof(parameterName);
            return argument ?? throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Ensures the given argument is not null with detailed exception information.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="callerMemberName"></param>
        /// <returns>The argument value.</returns>
        /// <exception cref="ArgumentNullException">If the argument is null, with enhanced details.</exception>
        [DebuggerStepThrough]
        public static TArgument IsNotNullWithDetails<TArgument>(TArgument? argument, string? parameterName = null,
            [CallerMemberName] string? callerMemberName = null)
            where TArgument : class
        {
            parameterName ??= callerMemberName ?? nameof(parameterName);
            return argument ?? throw new ArgumentNullException(parameterName);
        }

        #endregion

        #region String validations

        /// <summary>
        /// Ensures the given string argument is not null or empty or whitespace.
        /// </summary>
        /// <param name="argument">The string argument.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="callerMemberName"></param>
        /// <returns>The string argument.</returns>
        /// <exception cref="ArgumentException">If the string is null, empty or whitespace.</exception>
        [DebuggerStepThrough]
        public static string ArgumentIsNotEmpty(string? argument, string? parameterName = null,
            [CallerMemberName] string? callerMemberName = null)
        {
            parameterName ??= callerMemberName ?? nameof(parameterName);
            if (argument is null || string.IsNullOrWhiteSpace(argument) || string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException(
                    $"The parameter '{parameterName}' cannot be null or an empty string.",
                    parameterName);
            }

            return argument;
        }

        /// <summary>
        /// Ensures the given string has no whitespace.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="callerMemberName"></param>
        /// <returns>The string value.</returns>
        /// <exception cref="ArgumentException">If the string is null or whitespace.</exception>
        [DebuggerStepThrough]
        public static string ArgumentHasNotWhiteSpace(string value, string? parameterName = null,
            [CallerMemberName] string? callerMemberName = null)
        {
            parameterName ??= callerMemberName ?? nameof(parameterName);
            if (string.IsNullOrWhiteSpace(value))
                throw Error.ArgumentNullOrWhiteSpace(parameterName);
            return value;
        }

        /// <summary>
        /// Ensures the given string does not exceed the maximum length.
        /// </summary>
        /// <param name="arg">The string argument.</param>
        /// <param name="maxLength">The maximum allowed length.</param>
        /// <param name="parameterName">The argument name.</param>
        /// <param name="callerMemberName"></param>
        /// <exception cref="ArgumentException">If the string exceeds the maximum length.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotOutOfLength(string arg, int maxLength, string? parameterName = null,
            [CallerMemberName] string? callerMemberName = null)
        {
            parameterName ??= callerMemberName ?? nameof(parameterName);
            ArgumentIsNotNull(arg, parameterName);
            if (arg.Trim().Length > maxLength)
                throw Error.Argument(parameterName, "Argument '{0}' cannot be more than {1} characters long.",
                    parameterName, maxLength);
        }

        #endregion

        #region Collection validations

        /// <summary>
        /// Ensures the given collection is not empty.
        /// </summary>
        /// <typeparam name="T">The collection element type.</typeparam>
        /// <param name="arg">The collection.</param>
        /// <param name="parameterName">The argument name.</param>
        /// <param name="callerMemberName"></param>
        /// <exception cref="ArgumentException">If the collection is null or empty.</exception>
        [DebuggerStepThrough]
        public static void ArgumentIsNotEmpty<T>(ICollection<T> arg, string? parameterName = null,
            [CallerMemberName] string? callerMemberName = null)
        {
            parameterName ??= callerMemberName ?? nameof(parameterName);
            ArgumentIsNotNull(arg, parameterName);
            if (arg.Count == 0)
                throw Error.Argument(parameterName, "Collection cannot be empty and must have at least one item.");
        }

        #endregion

        #region Type validations

        /// <summary>
        /// Ensures the given type inherits from the specified base type.
        /// </summary>
        /// <typeparam name="TBase">The expected base type.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <exception cref="InvalidOperationException">If the type does not inherit from the specified base type.</exception>
        [DebuggerStepThrough]
        public static void InheritsFrom<TBase>(Type type)
        {
            ArgumentIsNotNull(type, nameof(type));
            InheritsFrom<TBase>(type, InheritsFromMessage.FormatInvariant(type.FullName, typeof(TBase).FullName));
        }

        /// <summary>
        /// Ensures the given type inherits from the specified base type with custom message.
        /// </summary>
        /// <typeparam name="TBase">The expected base type.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="message">The error message.</param>
        /// <exception cref="InvalidOperationException">If the type does not inherit from the specified base type.</exception>
        [DebuggerStepThrough]
        public static void InheritsFrom<TBase>(Type type, string message)
        {
            ArgumentIsNotNull(type, nameof(type));
            if (type.BaseType != typeof(TBase))
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Ensures the given type implements the specified interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="InvalidOperationException">If the type does not implement the interface.</exception>
        [DebuggerStepThrough]
        public static void Implements<TInterface>(Type type, string message = ImplementsMessage)
        {
            ArgumentIsNotNull(type, nameof(type));
            if (!typeof(TInterface).IsAssignableFrom(type))
                throw new InvalidOperationException(
                    message.FormatInvariant(type.FullName, typeof(TInterface).FullName));
        }

        /// <summary>
        /// Ensures the given type is a subclass of the specified base type.
        /// </summary>
        /// <typeparam name="TBase">The base type.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <exception cref="InvalidOperationException">If the type is not a subclass of the base type.</exception>
        [DebuggerStepThrough]
        public static void IsSubclassOf<TBase>(Type type)
        {
            ArgumentIsNotNull(type, nameof(type));
            var baseType = typeof(TBase);
            if (!baseType.IsSubClass(type))
                throw new InvalidOperationException(
                    IsSubclassOfMessage.FormatInvariant(type.FullName, baseType.FullName));
        }

        /// <summary>
        /// Ensures the given instance is of the specified type.
        /// </summary>
        /// <typeparam name="TType">The expected type.</typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <exception cref="InvalidOperationException">If the instance is not of the expected type.</exception>
        [DebuggerStepThrough]
        public static void IsTypeOf<TType>(object instance)
        {
            ArgumentIsNotNull(instance, nameof(instance));
            IsTypeOf<TType>(instance, IsTypeOfMessage.FormatInvariant(instance.GetType().Name, typeof(TType).FullName));
        }

        /// <summary>
        /// Ensures the given instance is of the specified type with custom message.
        /// </summary>
        /// <typeparam name="TType">The expected type.</typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <param name="message">The error message.</param>
        /// <exception cref="InvalidOperationException">If the instance is not of the expected type.</exception>
        [DebuggerStepThrough]
        public static void IsTypeOf<TType>(object instance, string message)
        {
            ArgumentIsNotNull(instance, nameof(instance));
            if (!(instance is TType))
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Ensures the given type has a default constructor.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <exception cref="InvalidOperationException">If the type doesn't have a default constructor.</exception>
        [DebuggerStepThrough]
        public static void HasDefaultConstructor<T>()
        {
            HasDefaultConstructor(typeof(T));
        }

        /// <summary>
        /// Ensures the given type has a default constructor.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <exception cref="InvalidOperationException">If the type doesn't have a default constructor.</exception>
        [DebuggerStepThrough]
        public static void HasDefaultConstructor(Type t)
        {
            ArgumentIsNotNull(t, nameof(t));
            if (!t.HasDefaultConstructor())
                throw Error.InvalidOperation("The type '{0}' must have a default parameterless constructor.",
                    t.FullName);
        }

        #endregion

        #region Comparison validations

        /// <summary>
        /// Ensures the given argument is within the specified range.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="arg">The argument value.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <param name="argName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the argument is outside the specified range.</exception>
        [DebuggerStepThrough]
        public static void ArgumentInRange<T>(T arg, T min, T max, string argName) where T : struct, IComparable<T>
        {
            if (arg.CompareTo(min) < 0 || arg.CompareTo(max) > 0)
                throw Error.ArgumentOutOfRange(argName, "The argument '{0}' must be between '{1}' and '{2}'.", argName,
                    min, max);
        }

        /// <summary>
        /// Ensures the given argument is not negative.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="arg">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the argument is negative.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotNegative<T>(T arg, string argName, string message = NotNegativeMessage)
            where T : struct, IComparable<T>
        {
            if (arg.CompareTo(default(T)) < 0)
                throw Error.ArgumentOutOfRange(argName, message.FormatInvariant(argName, arg));
        }

        /// <summary>
        /// Ensures the given argument is not zero.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="arg">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the argument is zero.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotZero<T>(T arg, string argName) where T : struct, IComparable<T>
        {
            if (arg.CompareTo(default(T)) == 0)
                throw Error.ArgumentOutOfRange(argName,
                    "Argument '{0}' must be greater or less than zero. Value: '{1}'.", argName, arg);
        }

        /// <summary>
        /// Ensures the given argument is positive.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="arg">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the argument is not positive.</exception>
        [DebuggerStepThrough]
        public static void ArgumentIsPositive<T>(T arg, string argName, string message = IsPositiveMessage)
            where T : struct, IComparable<T>
        {
            if (arg.CompareTo(default(T)) <= 0)
                throw Error.ArgumentOutOfRange(argName, message.FormatInvariant(argName, arg));
        }

        /// <summary>
        /// Ensures the given objects are equal.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="compare">The first object.</param>
        /// <param name="instance">The second object.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="Exception">If the objects are not equal.</exception>
        [DebuggerStepThrough]
        public static void IsEqual<TException>(object compare, object instance, string message = IsEqualMessage)
            where TException : Exception
        {
            ArgumentIsNotNull(compare, nameof(compare));
            ArgumentIsNotNull(instance, nameof(instance));

            if (!compare.Equals(instance))
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        #endregion

        #region Boolean validations

        /// <summary>
        /// Ensures the given assertion is false.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="assertion">The assertion to check.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="Exception">If the assertion is true.</exception>
        [DebuggerStepThrough]
        public static void Against<TException>(bool assertion, string message = AgainstMessage)
            where TException : Exception
        {
            if (assertion)
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Ensures the given assertion function returns false.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="assertion">The assertion function.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="Exception">If the assertion function returns true.</exception>
        [DebuggerStepThrough]
        public static void Against<TException>(Func<bool> assertion, string message = AgainstMessage)
            where TException : Exception
        {
            ArgumentIsNotNull(assertion, nameof(assertion));

            if (assertion())
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Ensures the given argument is true.
        /// </summary>
        /// <param name="arg">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="message">Optional error message.</param>
        /// <exception cref="ArgumentException">If the argument is false.</exception>
        [DebuggerStepThrough]
        public static void ArgumentIsTrue(bool arg, string argName, string message = IsTrueMessage)
        {
            if (!arg)
                throw Error.Argument(argName, message.FormatInvariant(argName));
        }

        #endregion

        #region Enum validations

        /// <summary>
        /// Ensures the given type is an enum type.
        /// </summary>
        /// <param name="arg">The type to check.</param>
        /// <param name="argName">The argument name.</param>
        /// <exception cref="ArgumentException">If the type is not an enum.</exception>
        [DebuggerStepThrough]
        public static void ArgumentIsEnumType(Type arg, string argName)
        {
            ArgumentIsNotNull(arg, argName);
            if (!arg.IsEnum)
                throw Error.Argument(argName, "Type '{0}' must be a valid Enum type.", arg.FullName);
        }

        /// <summary>
        /// Ensures the argument is a defined value of the given enum type.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <param name="arg">The value to check.</param>
        /// <param name="argName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not defined in the enum.</exception>
        [DebuggerStepThrough]
        public static void ArgumentIsEnumType(Type enumType, object arg, string argName)
        {
            ArgumentIsNotNull(enumType, nameof(enumType));
            ArgumentIsEnumType(enumType, nameof(enumType));
            ArgumentIsNotNull(arg, argName);

            if (!Enum.IsDefined(enumType, arg))
                throw Error.ArgumentOutOfRange(argName,
                    "The value of the argument '{0}' provided for the enumeration '{1}' is invalid.", argName,
                    enumType.FullName);
        }

        /// <summary>
        /// Ensures the argument is a defined value of the given enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="arg">The value to check.</param>
        /// <param name="argName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not defined in the enum.</exception>
        [DebuggerStepThrough]
        public static void ArgumentIsEnumType<TEnum>(object arg, string argName) where TEnum : struct, Enum
        {
            ArgumentIsNotNull(arg, argName);
            if (!Enum.IsDefined(typeof(TEnum), arg))
                throw new ArgumentOutOfRangeException(argName,
                    string.Format(CultureInfo.CurrentCulture,
                        "The value of the argument '{0}' provided for the enumeration '{1}' is invalid.", argName,
                        typeof(TEnum).FullName));
        }

        #endregion

        #region Special validations

        /// <summary>
        /// Ensures the given GUID is not empty.
        /// </summary>
        /// <param name="arg">The GUID to check.</param>
        /// <param name="argName">The argument name.</param>
        /// <exception cref="ArgumentException">If the GUID is empty.</exception>
        [DebuggerStepThrough]
        public static void ArgumentIsNotEmpty(Guid arg, string argName)
        {
            if (arg == Guid.Empty)
                throw Error.Argument(argName, "Argument '{0}' cannot be an empty guid.", argName);
        }

        /// <summary>
        /// Validates paging arguments.
        /// </summary>
        /// <param name="indexArg">The page index.</param>
        /// <param name="sizeArg">The page size.</param>
        /// <param name="indexArgName">The index argument name.</param>
        /// <param name="sizeArgName">The size argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the paging arguments are invalid.</exception>
        [DebuggerStepThrough]
        public static void PagingArgsValid(int indexArg, long sizeArg, string indexArgName, string sizeArgName)
        {
            ArgumentNotNegative(indexArg, indexArgName, "PageIndex cannot be below 0");
            if (indexArg > 0)
            {
                ArgumentIsPositive(sizeArg, sizeArgName,
                    "PageSize cannot be below 1 if a PageIndex greater 0 was provided.");
            }
            else
            {
                ArgumentNotNegative(sizeArg, sizeArgName);
            }
        }

        #endregion

        #region Helper methods for validation

        /// <summary>
        /// Checks if a string has consecutive characters.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="sequenceLength">The sequence length to check.</param>
        /// <returns>True if consecutive characters found, false otherwise.</returns>
        [DebuggerStepThrough]
        public static bool HasConsecutiveChars(string inputText, int sequenceLength = 3)
        {
            if (string.IsNullOrEmpty(inputText))
                return false;

            var charEnumerator = StringInfo.GetTextElementEnumerator(inputText);
            var currentElement = string.Empty;
            var count = 1;

            while (charEnumerator.MoveNext())
            {
                if (currentElement == charEnumerator.GetTextElement())
                {
                    if (++count >= sequenceLength)
                        return true;
                }
                else
                {
                    count = 1;
                    currentElement = charEnumerator.GetTextElement();
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a string is a valid email address.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <returns>True if valid email, false otherwise.</returns>
        [DebuggerStepThrough]
        public static bool IsEmailAddress(string inputText)
        {
            return !string.IsNullOrWhiteSpace(inputText) && new EmailAddressAttribute().IsValid(inputText);
        }

        #endregion
    }
}