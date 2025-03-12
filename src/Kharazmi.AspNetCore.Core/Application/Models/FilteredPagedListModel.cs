namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TFilteredPagedQueryModel"></typeparam>
    public class FilteredPagedListModel<TModel, TFilteredPagedQueryModel>
        where TFilteredPagedQueryModel : IFilteredPagedQueryModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        public FilteredPagedListModel(TFilteredPagedQueryModel query)
        {
            Query = query;
        }

        /// <summary>
        /// 
        /// </summary>
        public TFilteredPagedQueryModel Query { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PagedQueryResult<TModel> Result { get; set; }
    }
}