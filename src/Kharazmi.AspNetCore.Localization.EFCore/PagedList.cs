using System.Collections.Generic;
using System.Linq;

namespace Kharazmi.AspNetCore.Localization.EFCore
{
    public interface IPageList<T> : IList<T>
        where T : class
    {
        int TotalCount { get; set; }
        int TotalPages { get; set; }
        int PageIndex { get; set; }
        int PageSize { get; set; }
        bool HasPrevious { get; }
        bool HasNext { get; }
    }

    public class PagedList<T> : List<T>, IPageList<T>
        where T : class
    {
        public PagedList()
        {
        }

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount)
        {
            BuildPaging(source, pageIndex, pageSize, totalCount);
        }
      

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => PageIndex > 0;
        public bool HasNext => PageIndex + 1 < TotalCount;

        private void BuildPaging(IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount)
        {
            Ensure.IsNotNull(source, nameof(source));

            PageIndex = pageIndex < 0 ? PageIndex : pageIndex;
            PageSize = pageSize > 0 ? pageSize : PageSize;
            TotalCount = totalCount ?? source.Count();

            if (pageSize > 0)
            {
                TotalPages = TotalCount / pageSize;
                if (TotalCount % pageSize > 0)
                    TotalPages++;
            }
            
            PageSize = pageSize;
            PageIndex = pageIndex;
            source = totalCount == null ? source.Skip(pageIndex * pageSize).Take(pageSize) : source;
            AddRange(source);

        }
    }
}