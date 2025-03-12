using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Common;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.Linq;

namespace Kharazmi.MongoDb
{
    public static class QueryExtensions
    {
        private const int DefaultPageIndex = 1;
        private const int DefaultPageSize = 10;

        /// <summary>
        /// Return a paginated MongoQueryable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IMongoQueryable<T> PageBy<T>(this IMongoQueryable<T> query, int pageIndex, int pageSize)
            where T : class

        {
            Ensure.IsNotNull(query, nameof(query));

            if (pageIndex <= 0)
                pageIndex = DefaultPageIndex;

            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Return a PagedList of MongoQueryable
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<PagedList<T>> PageByAsync<T>(this IMongoQueryable<T> source, int pageIndex, int pageSize,
            int? totalCount = null, CancellationToken cancellationToken = default) where T : class
        {
            AspNetCore.Core.GuardToolkit.Ensure.ArgumentIsNotNull(source, nameof(source));

            if (pageSize <= 0)
                pageSize = DefaultPageSize;

            var pageResult = new PagedList<T> {TotalCount = totalCount ?? await source.CountAsync(cancellationToken).ConfigureAwait(false)};

            source = totalCount == null ? source.Skip(pageIndex * pageSize).Take(pageSize) : source;
            pageResult.AddRange(source);

            if (pageSize > 0)
            {
                pageResult.TotalPages = pageResult.TotalCount / pageSize;
                if (pageResult.TotalCount % pageSize > 0)
                    pageResult.TotalPages++;
            }

            pageResult.PageSize = pageSize;
            pageResult.PageIndex = pageIndex;

            return pageResult;
        }
    }
}