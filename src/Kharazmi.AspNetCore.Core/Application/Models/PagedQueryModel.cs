namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPagedQueryModel
    {
        /// <summary>
        /// 
        /// </summary>
        int Page { get; set; }
        int PageSize { get; set; }
        string SortExpression { get; set; }
    }

    public class PagedQueryModel : IPagedQueryModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortExpression { get; set; } = "Id_DESC";
    }
}