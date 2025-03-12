using System;
using System.Collections.Generic;
using System.Linq;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> EnumNamedValues<T>() where T : Enum
        {
            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));

            foreach (int item in values)
                result.Add(item, Enum.GetName(typeof(T), item));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="other"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddRange<T>(this ICollection<T> initial, IEnumerable<T> other)
        {
            if (initial == null) throw new ArgumentNullException(nameof(initial));
            if (other is null) return;

            if (initial is List<T> list)
            {
                list.AddRange(other);
                return;
            }

            foreach (var item in other)
            {
                initial.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || !source.Any();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool EqualsAll<T>(this IList<T> a, IList<T> b)
        {
            if (a == null || b == null)
                return a == null && b == null;

            if (a.Count != b.Count)
                return false;

            var comparer = EqualityComparer<T>.Default;

            return !a.Where((t, i) => !comparer.Equals(t, b[i])).Any();
        }
    }
}