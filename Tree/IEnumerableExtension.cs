using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    /// <summary>
    /// <see>IEnumerable<T></see> extensions class
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// Find a collection of items indexes selected by predicate
        /// </summary>
        /// <typeparam name="T">Collection items type</typeparam>
        /// <param name="source">Collection</param>
        /// <param name="predicate">Predicate to select indexes</param>
        /// <returns>Collection of items indexes selected by predicate</returns>
        public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T element in source)
            {
                if (predicate(element)) yield return index;
                index++;
            }
        }
        /// <summary>
        /// Convert enumerable collection to a singleline string
        /// </summary>
        /// <typeparam name="T">Collection items type</typeparam>
        /// <param name="source">Collection</param>
        /// <returns>Singleline string representation of collection</returns>
        public static string AsString<T>(this IEnumerable<T> source)
        {
            return source.Select(item => item.ToString()).Aggregate((item1, item2) => $"{item1} {item2}");
        }
        /// <summary>
        /// Convert enumerable collection to an indented string
        /// </summary>
        /// <typeparam name="T">Collection items type</typeparam>
        /// <param name="source">Collection</param>
        /// <returns>Indented string representation of collection</returns>
        public static string AsIndentedString<T>(this IEnumerable<T> source, string delimiter = "\n")
        {
            return source.Select(item => item.ToString()).Aggregate((item1, item2) => $"{item1}{delimiter}{item2}");
        }
    }
}
