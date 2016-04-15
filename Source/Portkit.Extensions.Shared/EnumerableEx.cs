using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Portkit.Extensions
{
    /// <summary>
    /// Enumerable extensions class.
    /// </summary>
    public static class EnumerableEx
    {
        /// <summary>
        /// Computes the weighted average of a collection.
        /// </summary>
        public static double WeightedAverage<T>(this IList<T> collection, Func<T, double> value, Func<T, double> weight)
        {
            double weightedValueSum = collection.Sum(x => value(x) * weight(x));
            double weightSum = collection.Sum(weight);
            return weightedValueSum / weightSum;
        }

#if WINDOWS_UWP

        /// <summary>
        /// Casts the object using generic methods.
        /// </summary>
        public static IEnumerable CastSlow(this IEnumerable series, Type elementType)
        {
            var method = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(elementType);
            return method.Invoke(null, new object[] { series }) as IEnumerable;
        }
#endif

        /// <summary>
        /// Check if a sequence is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">Type of sequence elements.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }
            /* If this is a list, use the Count property. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = source as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return source.Any();
        }

        /// <summary>
        /// Checks if a collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>
        ///     <c>true</c> if the collection is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            if (source == null)
            {
                return true;
            }
            return source.Count < 1;
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
        /// <param name="collection">Source collection</param>
        /// <param name="keySelector">Condition of ordering.</param>
        /// <param name="ascending">If true, check if collection is sorted ascending, else descending.</param>
        /// <returns>True if ordered, otherwise false.</returns>
        public static bool IsOrdered<TItem, TKey>(this IEnumerable<TItem> collection, Func<TItem, TKey> keySelector, bool ascending = true)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            var comparer = Comparer<TKey>.Default;
            using (var iterator = collection.GetEnumerator())
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
