using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.Threading
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper for synchronous execution of asynchronous methods
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// Checks if the given method is an asynchronous method
        /// </summary>
        /// <param name="method">The method to check</param>
        /// <returns>true if the method is asynchronous, false otherwise</returns>
        public static bool IsAsync(this MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            var returnType = method.ReturnType;
            return returnType == typeof(Task) ||
                   returnType == typeof(ValueTask) ||
                   (returnType.IsGenericType &&
                    (returnType.GetGenericTypeDefinition() == typeof(Task<>) ||
                     returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)));
        }


        /// <summary>
        /// Attempt to execute an asynchronous function synchronously with cancellation ability
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="func">The asynchronous function to execute</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>The result of the asynchronous function</returns>
        /// <exception cref="OperationCanceledException">Occurs when the operation is canceled</exception>
        public static TResult? TryRunSync<TResult>(Func<Task<TResult?>> func,
            CancellationToken cancellationToken)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return ExecuteWithCulturePreservation(func, cancellationToken);
        }

        /// <summary>
        /// Attempts to synchronously execute an asynchronous function that accepts a cancellation token
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="func">The asynchronous function to execute</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>The result of the asynchronous function</returns>
        /// <exception cref="OperationCanceledException">Occurs when the operation is canceled</exception>
        public static TResult? TryRunSync<TResult>(Func<CancellationToken, Task<TResult?>> func,
            CancellationToken cancellationToken)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return ExecuteWithCulturePreservation(() => func(cancellationToken), cancellationToken);
        }

        /// <summary>
        /// Execute an asynchronous function synchronously
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="func">The asynchronous function to execute</param>
        /// <returns>The result of the asynchronous function</returns>
        public static TResult? RunSync<TResult>(Func<Task<TResult?>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return ExecuteWithCulturePreservation(func, CancellationToken.None);
        }

        /// <summary>
        /// Execute an asynchronous void function synchronously
        /// </summary>
        /// <param name="func">The asynchronous function to run</param>
        public static void RunSync(Func<Task> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            ExecuteWithCulturePreservation(func, CancellationToken.None);
        }

        /// <summary>
        /// Execute an operation while preserving the current thread's cultural settings
        /// </summary>
        private static TResult? ExecuteWithCulturePreservation<TResult>(Func<Task<TResult?>> func,
            CancellationToken cancellationToken)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var currentUiCulture = CultureInfo.CurrentUICulture;

            return Task.Run(() =>
            {
                return ExceptionHandler.ExecuteAsAsync(async () =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    return await func().ConfigureAwait(false);
                });
            }, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Execute an action by keeping the cultural settings of the current string (void version)
        /// </summary>
        private static void ExecuteWithCulturePreservation(Func<Task> func, CancellationToken cancellationToken)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var currentUiCulture = CultureInfo.CurrentUICulture;

            Task.Run(() =>
            {
                return ExceptionHandler.ExecuteAsync(async () =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    await func().ConfigureAwait(false);
                });
            }, cancellationToken).GetAwaiter().GetResult();
        }
    }
}