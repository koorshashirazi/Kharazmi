namespace Kharazmi.AspNetCore.Core.Domain
{
    public interface IDomainQuery
    {
        DomainQueryType QueryType { get; }
        DomainQueryId QueryId { get; }
    }
}