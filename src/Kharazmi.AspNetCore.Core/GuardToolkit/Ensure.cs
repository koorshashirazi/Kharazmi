using System;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.GuardToolkit
{
    /// <summary>
    /// Provides argument validation methods.
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Ensures the given argument is not null.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument value is null or an empty string.</exception>
        /// <returns>The argument value.</returns>
        public static TArgument ArgumentIsNotNull<TArgument>(TArgument argument, string parameterName)
            where TArgument : class
            => argument ?? throw new ArgumentNullException(parameterName);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="parameterName"></param>
        /// <typeparam name="TArgument"></typeparam>
        /// <returns></returns>
        public static TArgument IsNotNullWithDetails<TArgument>(TArgument argument, string parameterName)
            where TArgument : class
            => argument ?? throw new ArgumentNullException(parameterName)
                   .WithDetailsJsonException();

        /// <summary>
        /// Ensures the given argument is not null or an empty string.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument value is null or an empty string.</exception>
        /// <returns>The argument value.</returns>
        public static string IsNotEmpty(string argument, string parameterName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException($"The parameter '{parameterName}' cannot be null or an empty string.");
            }

            return argument;
        }
        
    }
}