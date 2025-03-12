namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFilteredPagedQueryModel : IPagedQueryModel
    {
        /// <summary>
        /// 
        /// </summary>
        Filter Filter { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public  class FilteredPagedQueryModel : PagedQueryModel, IFilteredPagedQueryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Filter Filter { get; set; }
    }
}