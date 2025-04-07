using System.Runtime.CompilerServices;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Dispatchers;

public class TestCommand : DomainCommand
{
    public override bool IsValid()
    {
        return true;
    }
}
public class TestFailedCommand : DomainCommand
{
    public string FailedMessage { get; }

    public TestFailedCommand(string failedMessage)
    {
        FailedMessage = failedMessage;
    }
    public override bool IsValid()
    {
        return true;
    }
}

public class AnotherTestCommand : DomainCommand
{
    public override bool IsValid()
    {
        return true;
    }
}

public class TestCommandHandler : IDomainCommandHandler<TestCommand>
{
    public Task<Result> HandleAsync(TestCommand command, CancellationToken token = default)
    {
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> HandleAsync(IDomainCommand domainCommand, CancellationToken token = default)
    {
        return Task.FromResult(Result.Ok());
    }
}
public class FailedTestCommandHandler : IDomainCommandHandler<TestFailedCommand>
{
    public Task<Result> HandleAsync(TestFailedCommand domainCommand, CancellationToken token = default)
    {
        return Task.FromResult(Result.Fail(domainCommand.FailedMessage));
    }

    public Task<Result> HandleAsync(IDomainCommand domainCommand, CancellationToken token = default)
    {
        return HandleAsync((TestFailedCommand)domainCommand, token);
    }
}


public static class TestExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this List<T> items,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        foreach (var item in items)
        {
            await Task.Delay(200, token).ConfigureAwait(false);
            yield return item;
        }
    }
}

public record TestQuery : IDomainQuery
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }

    public DomainQueryType QueryType => DomainQueryType.From<TestQuery>();

    public DomainQueryId QueryId => DomainQueryId.New();
}

public record TestResult
{
    public string Value { get; set; } = string.Empty;
}