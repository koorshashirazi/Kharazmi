namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TPagedQueryModel"></typeparam>
    public class PagedListModel<TModel, TPagedQueryModel> where TPagedQueryModel : IPagedQueryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public TPagedQueryModel Query { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IPagedQueryResult<TModel> Result { get; set; }
    }
}