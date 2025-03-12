using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.Extensions.Primitives;

namespace Kharazmi.AspNetCore.Core.Extensions
{

    
    public static partial class Core
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static NameValueCollection AsNameValueCollection(this IEnumerable<KeyValuePair<string, StringValues>> collection)
        {
            var nv = new NameValueCollection();

            foreach (var field in collection)
            {
                nv.Add(field.Key, field.Value.First());
            }

            return nv;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(
            this IEnumerable<KeyValuePair<string, StringValues>> col)
        {
            if (col == null)
                return null;
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var keyValuePair in col)
                dictionary.Add(keyValuePair.Key, string.Join(", ", (string[]) keyValuePair.Value));
            return dictionary;
        }

        #region Multimap

        //public static Multimap<TKey, TValue> ToMultimap<TSource, TKey, TValue>(
        //    this IEnumerable<TSource> source,
        //    Func<TSource, TKey> keySelector,
        //    Func<TSource, TValue> valueSelector)
        //{
        //    Guard.ThrowIfIsNull(() => source);
        //    Guard.ThrowIfIsNull(() => keySelector);
        //    Guard.ThrowIfIsNull(() => valueSelector);

        //    var map = new Multimap<TKey, TValue>();

        //    foreach (var item in source)
        //        if (keySelector != null) map.Add(keySelector(item), valueSelector(item));

        //    return map;
        //}

        #endregion

        #region NameValueCollection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="other"></param>
        public static void AddRange(this NameValueCollection initial, NameValueCollection other)
        {
            Guard.ArgumentNotNull(initial, nameof(initial));
            if (other == null)
                return;

            foreach (var item in other.AllKeys)
                initial.Add(item, other[item]);
        }

        #endregion

        #region Nested classes

        private static class DefaultReadOnlyCollection<T>
        {
            private static ReadOnlyCollection<T> _defaultCollection;
            internal static ReadOnlyCollection<T> Empty => _defaultCollection ??= new ReadOnlyCollection<T>(new T[0]);
        }

        #endregion

        #region IEnumerable

        private class Status
        {
            public bool EndOfSequence;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <param name="condition"></param>
        /// <param name="limit"></param>
        /// <param name="orderByDescending"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> TakeIf<T, TKey>(this IEnumerable<T> query, Func<T, TKey> orderBy, bool condition,
            int limit, bool orderByDescending = true)
        {
            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return condition
                ? query.Take(limit)
                : query;
        }

        private static IEnumerable<T> TakeOnEnumerator<T>(IEnumerator<T> enumerator, int count, Status status)
        {
            while (--count > 0 && !enumerator.MoveNext() && (status.EndOfSequence = true))
                yield return enumerator.Current;
        }


        /// <summary>
        ///     Slices the iteration over an enumerable by the given chunk size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="chunkSize">SIze of chunk</param>
        /// <returns>The sliced enumerable</returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> items, int chunkSize = 100)
        {
            if (chunkSize < 1)
                throw new ArgumentException("Chunks should not be smaller than 1 element");
            var status = new Status {EndOfSequence = false};
            using var enumerator = items.GetEnumerator();
            while (!status.EndOfSequence)
                yield return TakeOnEnumerator(enumerator, chunkSize, status);
        }


        /// <summary>
        ///     Performs an action on each item while iterating through a list.
        ///     This is a handy shortcut for <c>foreach(item in list) { ... }</c>
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The list, which holds the objects.</param>
        /// <param name="action">The action delegate which is called on each item while iterating.</param>
        //[DebuggerStepThrough]
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var t in source)
                action(t);
        }

        /// <summary>
        ///     Performs an action on each item while iterating through a list.
        ///     This is a handy shortcut for <c>foreach(item in list) { ... }</c>
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The list, which holds the objects.</param>
        /// <param name="action">The action delegate which is called on each item while iterating.</param>
        //[DebuggerStepThrough]
        public static void Each<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var t in source)
                action?.Invoke(t, i++);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> CastValid<T>(this IEnumerable source)
        {
            return source.Cast<object>().Where(o => o is T).Cast<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> source)
        {
            if (source == null) return DefaultReadOnlyCollection<T>.Empty;
            var enumerable = source as T[] ?? source.ToArray();
            if (!enumerable.Any())
                return DefaultReadOnlyCollection<T>.Empty;

            return source switch
            {
                ReadOnlyCollection<T> readOnly => readOnly,
                List<T> list => list.AsReadOnly(),
                _ => new ReadOnlyCollection<T>(enumerable.ToArray())
            };
        }

        #endregion

        #region AsSerializable

        /// <summary>
        ///     Convenience API to allow an IEnumerable[T] (such as returned by Linq2Sql, NHibernate, EF etc.)
        ///     to be serialized by DataContractSerializer.
        /// </summary>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <param name="source">The original collection.</param>
        /// <returns>A serializable enumerable wrapper.</returns>
        public static IEnumerable<T> AsSerializable<T>(this IEnumerable<T> source) where T : class
        {
            return new IEnumerableWrapper<T>(source);
        }

        /// <summary>
        ///     This wrapper allows IEnumerable
        ///     <T />
        ///     to be serialized by DataContractSerializer.
        ///     It implements the minimal amount of surface needed for serialization.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        private class IEnumerableWrapper<T> : IEnumerable<T>
            where T : class
        {
            private readonly IEnumerable<T> _collection;

            // The DataContractSerilizer needs a default constructor to ensure the object can be
            // deserialized. We have a dummy one since we don't actually need deserialization.

            internal IEnumerableWrapper(IEnumerable<T> collection)
            {
                _collection = collection;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _collection).GetEnumerator();
            }
        }

        #endregion

        #region Hierarchy

        /// <summary>
        /// Flatten a Hierarchy model 
        /// </summary>
        /// <param name="getChildList"></param>
        /// <param name="node"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> FlattenHierarchy<T>(this T node, Func<T, IEnumerable<T>> getChildList)
        {
            yield return node;
            if (getChildList(node) == null) yield break;
            foreach (var child in getChildList(node))
            foreach (var childOrDescendant in FlattenHierarchy(child, getChildList))
                yield return childOrDescendant;
        }

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static bool ContainsAll<T, TKey>(this IEnumerable<T> list1, IEnumerable<T> list2, Func<T, TKey> key)
        {
            var containingList = new HashSet<TKey>(list1.Select(key));
            return list2.All(x => containingList.Contains(key(x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> list1, IEnumerable<T> list2) =>
            list1.ContainsAll(list2, item => item);
    }
}