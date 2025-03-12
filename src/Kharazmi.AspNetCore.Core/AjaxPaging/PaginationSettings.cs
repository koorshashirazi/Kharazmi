namespace Kharazmi.AspNetCore.Core.AjaxPaging
{
    public class PaginationSettings
    {
        public long TotalItems { get; set; } = 0;

        public int ItemsPerPage { get; set; } = 10;

        public long CurrentPage { get; set; } = 1;

        public long MaxPagerItems { get; set; } = 10;

        public bool ShowFirstLast { get; set; } = false;

        public bool ShowNumbered { get; set; } = true;

        public bool UseReverseIncrement { get; set; } = false;

        public bool SuppressEmptyNextPrev { get; set; } = false;

        public bool SuppressInActiveFirstLast { get; set; } = false;

        public bool RemoveNextPrevLinks { get; set; } = false;
    }
}