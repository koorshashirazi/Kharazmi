using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Functional;

public static class ExceptionHandler
{
    private static bool IsNonCritical(Exception ex) => ex
        is not OutOfMemoryException
        and not StackOverflowException
        and not ThreadAbortException
        and not AccessViolationException
        and not SecurityException;


    public static void Execute(Action action,
        Action<Exception>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName ??= Path.GetFileNameWithoutExtension(resource);
            HandleException(ex, onError, resourceName);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static Result Execute(Func<Result> action,
        Func<Exception, Result>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName ??= Path.GetFileNameWithoutExtension(resource);
            return HandleException(ex, onError, resourceName);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static T? ExecuteAs<T>(Func<T?> action,
        Func<Exception, T?>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName ??= Path.GetFileNameWithoutExtension(resource);
            return HandleException(ex, onError, resourceName);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }


    public static Result<T> ExecuteAs<T>(Func<Result<T>> action,
        Func<Exception, Result<T>>? onError = null, Action? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return action.Invoke();
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName ??=Path.GetFileNameWithoutExtension(resource);
            return HandleException(ex, onError, resourceName);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }

    public static async Task ExecuteAsync(Func<Task> action,
        Action<Exception>? onError = null, Func<Task>? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            await action.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName ??= Path.GetFileNameWithoutExtension(resource);
            HandleException(ex, onError, resourceName);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    public static async Task<Result> ExecuteAsync(Func<Task<Result>> action,
        Func<Exception, Result>? onError = null, Func<Task>? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return await action.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName ??= Path.GetFileNameWithoutExtension(resource);
            return HandleException(ex, onError, resourceName);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }


    public static async Task<Result<T>> ExecuteAsAsync<T>(Func<Task<Result<T>>> action,
        Func<Exception, Result<T>>? onError = null,  Func<Task>? onCompleted = null,
        string? resourceName = null, [CallerFilePath] string resource = "")
    {
        try
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return await action.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsNonCritical(ex))
        {
            resourceName ??=Path.GetFileNameWithoutExtension(resource);
            return HandleException(ex, onError, resourceName);
        }
        finally
        {
            if (onCompleted is not null)
            {
                await onCompleted.Invoke().ConfigureAwait(false);
            }
        }
    }

    private static Result HandleException(Exception ex, Func<Exception, Result>? onError,
        string? resourceName)
    {
        Debug.WriteLine(resourceName is null
            ? $"Exception caught: {ex.Message}"
            : $"{resourceName}: Exception caught: {ex.Message}");

        if (onError is null)
        {
            return Result.Fail(ResultMessages.ExceptionMessage, resourceName)
                .AddException(ex);
        }

        return onError.Invoke(ex);
    }

    private static T? HandleException<T>(Exception ex, Func<Exception, T?>? onError, string? resourceName)
    {
        Debug.WriteLine(resourceName is null
            ? $"Exception caught: {ex.Message}"
            : $"{resourceName}: Exception caught: {ex.Message}");


        return onError is null ? default : onError.Invoke(ex);
    }

    private static Result<T> HandleException<T>(Exception ex, Func<Exception, Result<T>>? onError,
        string? resourceName)
    {
        Debug.WriteLine(resourceName is null
            ? $"Exception caught: {ex.Message}"
            : $"{resourceName}: Exception caught: {ex.Message}");


        if (onError is null)
        {
            return Result.Fail<T>(ResultMessages.ExceptionMessage, resourceName)
                .AddException(ex);
        }

        return onError.Invoke(ex);
    }


    private static void HandleException(Exception ex, Action<Exception>? onError, string? resourceName)
    {
        Debug.WriteLine(resourceName is null
            ? $"Exception caught: {ex.Message}"
            : $"{resourceName}: Exception caught: {ex.Message}");

        onError?.Invoke(ex);
    }
}