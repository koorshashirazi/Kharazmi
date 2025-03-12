using Kharazmi.AspNetCore.Core.Application.Models;

namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public interface IFilteredPagedQuery<out TReadModel> : IPagedQuery<TReadModel>
    {
        /// <summary>
        /// 
        /// </summary>
        Filter Filter { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class FilteredPagedQuery<TReadModel> : PagedQuery<TReadModel>, IFilteredPagedQuery<TReadModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public Filter Filter { get; set; }
    }
}