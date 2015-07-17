using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Portkit.Core.Extensions
{
    /// <summary>
    /// Enumerable extensions class.
    /// </summary>
    public static class EnumerableEx
    {
        /// <summary>
        /// Casts the object using generic methods.
        /// </summary>
        public static IEnumerable CastSlow(this IEnumerable series, Type elementType)
        {
            var method = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(elementType);
            return method.Invoke(null, new object[] { series }) as IEnumerable;
        }

        /// <summary>
        /// Determines weather a sequence is null or has no elements.
        /// </summary>
        /// <returns>True if sequence is null or there are no elements, otherwise returns false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return data == null || !data.Any();
        }

        /// <summary>
        /// Shuffles the collection.
        /// </summary>
        /// <param name="collection">Original collection</param>
        /// <typeparam name="T">Items type.</typeparam>
        /// <returns>Collection with same items, but shuffled items.</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            return collection.OrderBy(_ => Guid.NewGuid());
        }

        /// <summary>
        /// Checks if a collection is ordered.
        /// </summary>
        /// <typeparam name="TItem">Type of items</typeparam>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <param name="sourceCollection">Source collection</param>
        /// <param name="keySelector">Condition of ordering.</param>
        /// <param name="ascending">If true, check if collection is sorted ascending, else descending.</param>
        /// <returns>True if ordered, otherwise false.</returns>
        public static bool IsOrdered<TItem, TKey>(this IEnumerable<TItem> sourceCollection, Func<TItem, TKey> keySelector, bool ascending = true)
        {
            if (sourceCollection == null)
            {
                throw new ArgumentNullException("sourceCollection");
            }
            var comparer = Comparer<TKey>.Default;
            using (var iterator = sourceCollection.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    return true;
                }
                var current = keySelector(iterator.Current);
                if (ascending)
                {
                    while (iterator.MoveNext())
                    {
                        var next = keySelector(iterator.Current);
                        if (comparer.Compare(current, next) > 0)
                        {
                            return false;
                        }
                        current = next;
                    }
                }
                else
                {
                    while (iterator.MoveNext())
                    {
                        var next = keySelector(iterator.Current);
                        if (comparer.Compare(current, next) < 0)
                        {
                            return false;
                        }
                        current = next;
                    }
                }
            }
            return true;
        }
    }
}
