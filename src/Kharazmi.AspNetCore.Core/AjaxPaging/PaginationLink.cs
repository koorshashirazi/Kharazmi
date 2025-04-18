﻿namespace Kharazmi.AspNetCore.Core.AjaxPaging
{
    public class PaginationLink
    {
        public bool Active { get; set; } = true;

        public bool IsCurrent { get; set; } = false;

        public long PageNumber { get; set; } = -1;

        public string Text { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public bool IsSpacer { get; set; } = false;
    }
}