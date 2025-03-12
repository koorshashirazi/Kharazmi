using System;
using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.AjaxPaging
{
    public interface IBuildPaginationLinks
    {
        List<PaginationLink> BuildPaginationLinks(
            PaginationSettings paginationSettings,
            Func<long, string> generateUrl,
            string firstPageText,
            string firstPageTitle,
            string previousPageText,
            string previousPageTitle,
            string nextPageText,
            string nextPageTitle,
            string lastPageText,
            string lastPageTitle,
            string spacerText = "...");
    }
}