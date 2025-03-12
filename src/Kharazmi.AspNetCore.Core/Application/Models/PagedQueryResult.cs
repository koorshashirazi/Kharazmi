using System.Collections.Generic;

 namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IPagedQueryResult<out TModel>
    {
        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<TModel> Items { get; }
        /// <summary>
        /// 
        /// </summary>
        long TotalCount { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class PagedQueryResult<TModel> : IPagedQueryResult<TModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<TModel> Items { get; set; } = new List<TModel>();
        /// <summary>
        /// 
        /// </summary>
        public long TotalCount { get; set; }
    }
}