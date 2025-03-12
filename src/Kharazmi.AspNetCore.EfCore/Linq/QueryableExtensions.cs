using System.Linq;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Application.Models;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Filter = Kharazmi.AspNetCore.Core.Application.Models.Filter;

namespace Kharazmi.AspNetCore.EFCore.Linq
{
    public static class QueryableExtensions
    {
        public static Task<PagedQueryResult<T>> ToPagedQueryResultAsync<T>(this IQueryable<T> query,
            IPagedQueryModel model)
        {
            var filter = model is IFilteredPagedQueryModel filteredQuery ? filteredQuery.Filter : null;

            return query.ToPagedQueryResultAsync(model.Page, model.PageSize, model.SortExpression, filter);
        }

        public static Task<PagedQueryResult<T>> ToPagedQueryResultAsync<T>(this IQueryable<T> query,
            int page,
            int pageSize,
            string sortExpression)
        {
            return query.ToPagedQueryResultAsync(page, pageSize, sortExpression, null);
        }

        public static async Task<PagedQueryResult<T>> ToPagedQueryResultAsync<T>(this IQueryable<T> query,
            int page,
            int pageSize,
            string sortExpression,
            Filter filter)
        {
            query = query.ApplyFiltering(filter);

            var totalCount = await query.LongCountAsync().ConfigureAwait(false);

            query = query.ApplySorting(sortExpression);
            query = query.ApplyPaging(page, pageSize);

            return new PagedQueryResult<T>
            {
                Items = await query.ToListAsync().ConfigureAwait(false),
                TotalCount = totalCount
            };
        }
    }
}