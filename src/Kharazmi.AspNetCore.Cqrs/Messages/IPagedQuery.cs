using Kharazmi.AspNetCore.Core.Application.Models;

 namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public interface IPagedQuery<out TReadModel> : IQuery<IPagedQueryResult<TReadModel>>
    {
        /// <summary>
        /// 
        /// </summary>
        int Page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string SortExpression { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class PagedQuery<TReadModel> : IPagedQuery<TReadModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SortExpression { get; set; }
    }
}