using System;
using System.Linq;
using System.Linq.Expressions;

namespace Kharazmi.AspNetCore.Localization.EFCore
{
    /// <summary>
    /// Queryable Extensions
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Where Linq By condition
        /// </summary>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
            Expression<Func<T, bool>> predicate)
        {
          Ensure.IsNotNull(query, nameof(query));

            return condition
                ? query.Where(predicate)
                : query;
        }
        
        /// <summary>
        /// Take by condition
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <param name="condition"></param>
        /// <param name="limit"></param>
        /// <param name="orderByDescending"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> TakeIf<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> orderBy, bool condition, int limit, bool orderByDescending = true)
        {
            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return condition
                ? query.Take(limit)
                : query;
        }

        
        /// <summary>
        /// Pagination of a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderByDescending"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<T> PageBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> orderBy, int page, int pageSize, bool orderByDescending = true)
        {
            const int defaultPageNumber = 1;
            query.CheckArgumentIsNull(nameof(query));

            // Check if the page number is greater then zero - otherwise use default page number
            if (page <= 0)
                page = defaultPageNumber;

            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}