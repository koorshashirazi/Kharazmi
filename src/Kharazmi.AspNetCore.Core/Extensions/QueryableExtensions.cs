using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Kharazmi.AspNetCore.Core.Application.Models;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Core.Extensions
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
            Ensure.ArgumentIsNotNull(query, nameof(query));

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="model"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, IFilteredPagedQueryModel model)
        {
            Ensure.ArgumentIsNotNull(query, nameof(query));
            Ensure.ArgumentIsNotNull(model, nameof(model));

            return query.ApplyFiltering(model.Filter);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, IPagedQueryModel model)
        {
            Ensure.ArgumentIsNotNull(query, nameof(query));
            Ensure.ArgumentIsNotNull(model, nameof(model));

            return query.ApplySorting(model.SortExpression);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, IPagedQueryModel model)
        {
            Ensure.ArgumentIsNotNull(query, nameof(query));
            Ensure.ArgumentIsNotNull(model, nameof(model));

            if (model.Page < 1) model.Page = 1;

            if (model.PageSize < 1) model.PageSize = 10;

            return query.ApplyPaging(model.Page, model.PageSize);
        }

        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, Filter filter)
        {
            Ensure.ArgumentIsNotNull(query, nameof(query));

            if (filter?.Logic == null) return query;

            var filters = filter.List();

            // Get all filter values as array (needed by the Where method of Dynamic Linq)
            var values = filters.Select(f => f.Value).ToArray();

            // Create a predicate expression e.g. Field1 = @0 And Field2 > @1
            var predicate = filter.ToExpression(filters);

            // Use the Where method of Dynamic Linq to filter the data
            query = query.Where(predicate, values);

            return query;
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortExpression)
        {
            Ensure.ArgumentIsNotNull(query, nameof(query));
            Ensure.ArgumentIsNotEmpty(sortExpression, nameof(sortExpression));

            return query.OrderBy(x=> sortExpression.Replace('_', ' '));
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            Ensure.ArgumentIsNotNull(query, nameof(query));

            var skip = (page - 1) * pageSize;
            var take = pageSize;

            return query.Skip(skip).Take(take);
        }
    }
}