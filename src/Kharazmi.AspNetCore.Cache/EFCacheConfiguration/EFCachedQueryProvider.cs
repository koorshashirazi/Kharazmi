﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Cache.Ef;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Kharazmi.AspNetCore.Cache.EFCacheConfiguration
{
    /// <summary>
    /// Defines methods to create and execute queries that are described by an System.Linq.IQueryable object.
    /// </summary>
    public class EfCachedQueryProvider : IAsyncQueryProvider
    {
        private readonly IEFCacheKeyProvider _cacheKeyProvider;
        private readonly IEFCacheServiceProvider _cacheServiceProvider;
        private readonly EFCacheDebugInfo _debugInfo;
        private readonly EFCachePolicy _cachePolicy;
        private readonly IQueryable _query;

        private static readonly MethodInfo _fromResultMethodInfo = typeof(Task).GetMethod("FromResult");
        private static readonly MethodInfo _materializeAsyncMethodInfo = typeof(EFMaterializer).GetMethod(nameof(EFMaterializer.MaterializeAsync));

        /// <summary>
        /// Defines methods to create and execute queries that are described by an System.Linq.IQueryable object.
        /// </summary>
        /// <param name="query">The input EF query.</param>
        /// <param name="cachePolicy">Defines the expiration mode of the cache item.</param>
        /// <param name="debugInfo">Stores the debug information of the caching process.</param>
        /// <param name="cacheKeyProvider">Gets an EF query and returns its hash to store in the cache.</param>
        /// <param name="cacheServiceProvider">The Cache Service Provider.</param>
        public EfCachedQueryProvider(
            IQueryable query,
            EFCachePolicy cachePolicy,
            EFCacheDebugInfo debugInfo,
            IEFCacheKeyProvider cacheKeyProvider,
            IEFCacheServiceProvider cacheServiceProvider)
        {
            _query = query;
            _cachePolicy = cachePolicy;
            _debugInfo = debugInfo;
            _cacheKeyProvider = cacheKeyProvider;
            _cacheServiceProvider = cacheServiceProvider;
            Materializer =
                new EFMaterializer(_query, _cachePolicy, _debugInfo, _cacheKeyProvider, _cacheServiceProvider);
        }

        /// <summary>
        /// Defines methods to create and execute queries that are described by an System.Linq.IQueryable object.
        /// </summary>
        public EFMaterializer Materializer { get; }

        /// <summary>
        /// Constructs an System.Linq.IQueryable of T object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements that is returned.</typeparam>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>An System.Linq.IQueryable of T that can evaluate the query represented by the specified expression tree.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)CreateQuery(expression);
        }

        /// <summary>
        /// Constructs an System.Linq.IQueryable object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>An System.Linq.IQueryable that can evaluate the query represented by the specified expression tree.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            var argumentType = expression.Type.GenericTypeArguments[0];
            var cachedQueryable = typeof(EFCachedQueryable<>).MakeGenericType(argumentType);
            var constructorArgs = new object[]
            {
                _query.Provider.CreateQuery(expression),
                _cachePolicy,
                _debugInfo,
                _cacheKeyProvider,
                _cacheServiceProvider
            };
            return (IQueryable)Activator.CreateInstance(cachedQueryable, constructorArgs);
        }

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>The value that results from executing the specified query.</returns>
        public object Execute(Expression expression)
        {
            return Materializer.Materialize(expression, () => _query.Provider.Execute(expression));
        }

        /// <summary>
        /// Executes the strongly-typed query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>The value that results from executing the specified query.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            return Materializer.Materialize(expression, () => _query.Provider.Execute<TResult>(expression));
        }

        /// <summary>
        /// Asynchronously executes the strongly-typed query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.  The task result contains the value that results from executing the specified query.</returns>
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var type = typeof(TResult);

            if (_query.Provider is EntityQueryProvider eqProvider)
            {
                if (isTaskOfT(type))
                {
                    var materializeAsyncMethod = _materializeAsyncMethodInfo.MakeGenericMethod(expression.Type);
                    return (TResult)materializeAsyncMethod.Invoke(Materializer, new object[]
                            {
                                expression,
                                new Func<TResult>(() => eqProvider.ExecuteAsync<TResult>(expression, cancellationToken))
                            });
                }

                return Materializer.Materialize(
                             expression,
                             () => eqProvider.ExecuteAsync<TResult>(expression, cancellationToken));
            }

            if (isTaskOfT(type))
            {
                var result = Execute(expression);
                var taskFromResultMethod = _fromResultMethodInfo.MakeGenericMethod(expression.Type);
                return (TResult)taskFromResultMethod.Invoke(null, new[] { result });
            }

            return Execute<TResult>(expression);
        }

        private static bool isTaskOfT(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }
    }
}