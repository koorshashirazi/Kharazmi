using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Core.Exceptions;

public static class ExceptionHandler
{
    /// <summary>
    /// Determines if an exception is non-critical and can be handled.
    /// </summary>
    private static bool IsNonCritical(Exception ex) => ex
        is not OutOfMemoryException
        and not StackOverflowException
        and not ThreadAbortException
        and not AccessViolationException
        and not SecurityException;

    /// <summary>
    /// Gets a file name without extension from full path.
    /// </summary>
    private static string GetResourceName(string? resourceName, string resource) =>
        resourceName ?? Path.GetFileNameWithoutExtension(resource);

    /// <summary>
    /// Logs exception details.
    /// </summary>
    private static void LogException(string? resourceName, Exception ex)
    {
        var name = string.IsNullOrEmpty(resourceName) ? string.Empty : $"{resourceName}: ";
        Debug.WriteLine($"{name}Exception caught: {ex.Message}");
    }

    /// <summary>
    /// Creates a default Result failure from an exception.
    /// </summary>
    private static Result CreateFailResult(string resourceName, Exception ex) => Result
        .Fail(ResultMessages.ExceptionMessage)
        .WithInternalMessages(InternalResultMessage.With(resourceName, ex.Message))
        .WithException(ex);

    /// <summary>
    /// Creates a default generic Result failure from an exception.
    /// </summary>
    private static Result<T> CreateFailResult<T>(string resourceName, Exception ex) => Result
        .Fail<T>(ResultMessages.ExceptionMessage)
        .WithInternalMessages(InternalResultMessage.With(resourceName, ex.Message))
        .WithException(ex);

    #region Synchronous methods

    public static void Execute(Action action,
        Action<Exception>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);

            action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null)
            {
                throw;
            }

            onError.Invoke(ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static TResult? ExecuteAs<TResult>(Func<TResult?> action,
        Func<Exception, TResult?>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);

            return action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null) throw;

            return onError.Invoke(ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static TResult? ExecuteAs<TState, TResult>(
        Func<TState, TResult?> action,
        TState state,
        string? stateParamName = null,
        Func<Exception, TResult?>? onError = null,
        Action? onCompleted = null,
        string? resourceName = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            stateParamName ??= callerMemberName;
            Ensure.ArgumentIsNotNull(state, stateParamName);
            Ensure.ArgumentIsNotNull(action);
            return action.Invoke(state);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null) throw;

            return onError.Invoke(ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static Result ExecuteResult(Func<Result> action,
        Func<Exception, Result>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);

            return action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            return onError?.Invoke(ex) ?? CreateFailResult(resourceName, ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static Result ExecuteResult<TState>(
        Func<TState, Result> action,
        TState state,
        string? stateParamName = null,
        Func<Exception, Result>? onError = null, 
        Action? onCompleted = null,
        string? resourceName = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            stateParamName ??= callerMemberName;
            Ensure.ArgumentIsNotNull(state, stateParamName);
            Ensure.ArgumentIsNotNull(action);
            return action.Invoke(state);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            return onError?.Invoke(ex) ?? CreateFailResult(resourceName, ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static Result<TResult> ExecuteResultAs<TResult>(Func<Result<TResult>> action,
        Func<Exception, Result<TResult>>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);

            return action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            return onError?.Invoke(ex) ?? CreateFailResult<TResult>(resourceName, ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static Result<TResult> ExecuteResultAs<TState, TResult>(
        Func<TState, Result<TResult>> action,
        TState state,
        string? stateParamName = null,
        Func<Exception, Result<TResult>>? onError = null,
        Action? onCompleted = null,
        string? resourceName = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            stateParamName ??= callerMemberName;
            Ensure.ArgumentIsNotNull(state, stateParamName);
            Ensure.ArgumentIsNotNull(action);
            return action.Invoke(state);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            return onError?.Invoke(ex) ?? CreateFailResult<TResult>(resourceName, ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    #endregion

    #region Asynchronous methods

    public static async Task ExecuteAsync(Func<Task> action,
        Action<Exception>? onError = null, Func<Task>? onCompleted = null,
        string? resourceName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);

            await action.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null)
            {
                throw;
            }

            onError.Invoke(ex);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    public static async Task<TResult?> ExecuteAsAsync<TResult>(Func<Task<TResult?>> action,
        Func<Exception, TResult?>? onError = null,
        Func<Task>? onCompleted = null,
        string? resourceName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);
            return await action.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null) throw;

            return onError.Invoke(ex);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    public static async Task<TResult?> ExecuteAsAsync<TState, TResult>(
        Func<TState, Task<TResult?>> action,
        TState state,
        string? stateParamName = null,
        Func<Exception, TResult?>? onError = null,
        Func<Task>? onCompleted = null,
        string? resourceName = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            stateParamName ??= callerMemberName;
            Ensure.ArgumentIsNotNull(state, stateParamName);
            Ensure.ArgumentIsNotNull(action);
            return await action.Invoke(state).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null) throw;

            return onError.Invoke(ex);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    public static async Task<Result> ExecuteResultAsync(
        Func<Task<Result>> action,
        Func<Exception, Task<Result>>? onError = null,
        Func<Task>? onCompleted = null,
        string? resourceName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);

            return await action.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null)
            {
                return CreateFailResult(resourceName, ex);
            }

            return await onError.Invoke(ex).ConfigureAwait(false);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    public static async Task<Result> ExecuteResultAsync<TState>(
        Func<TState, Task<Result>> action,
        TState state,
        string? stateParamName = null,
        Func<Exception, Task<Result>>? onError = null,
        Func<Task>? onCompleted = null,
        string? resourceName = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            stateParamName ??= callerMemberName;
            Ensure.ArgumentIsNotNull(state, stateParamName);
            Ensure.ArgumentIsNotNull(action);
            return await action.Invoke(state).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null)
            {
                return CreateFailResult(resourceName, ex);
            }

            return await onError.Invoke(ex).ConfigureAwait(false);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    public static async Task<Result<TResult>> ExecuteResultAsAsync<TResult>(
        Func<Task<Result<TResult>>> action,
        Func<Exception, Task<Result<TResult>>>? onError = null,
        Func<Task>? onCompleted = null,
        string? resourceName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            Ensure.ArgumentIsNotNull(action);

            return await action.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null)
            {
                return CreateFailResult<TResult>(resourceName, ex);
            }

            return await onError.Invoke(ex).ConfigureAwait(false);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    public static async Task<Result<TResult>> ExecuteResultAsAsync<TState, TResult>(
        Func<TState, Task<Result<TResult>>> action,
        TState state,
        string? stateParamName = null,
        Func<Exception, Task<Result<TResult>>>? onError = null,
        Func<Task>? onCompleted = null,
        string? resourceName = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string resource = "")
    {
        try
        {
            stateParamName ??= callerMemberName;
            Ensure.ArgumentIsNotNull(state, stateParamName);
            Ensure.ArgumentIsNotNull(action);
            return await action.Invoke(state).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName = GetResourceName(resourceName, resource);
            LogException(resourceName, ex);

            if (onError is null)
            {
                return CreateFailResult<TResult>(resourceName, ex);
            }

            return await onError.Invoke(ex).ConfigureAwait(false);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    #endregion
}