using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Portkit.Extensions
{
    public static class ObservableCollectionEx
    {
        /// <summary>
        /// Sort an <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">Collection to sort.</param>
        /// <param name="keySelector">Key selector function.</param>
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector)
        {
            if (collection == null)
            {
                return;
            }
            var comparer = Comparer<TKey>.Default;
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                for (var j = 1; j <= i; j++)
                {
                    var o1 = collection[j - 1];
                    var o2 = collection[j];
                    if (comparer.Compare(keySelector(o1), keySelector(o2)) > 0)
                    {
                        collection.Remove(o1);
                        collection.Insert(j, o1);
                    }
                }
            }
        }
    }
}
