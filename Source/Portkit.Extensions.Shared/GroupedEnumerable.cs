using System;
using System.Collections;
using System.Collections.Generic;

namespace Portkit.Extensions
{
    /// <summary>
    /// An enumerable data structure that contains grouped values by key.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class GroupedEnumerable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>
    {
        private readonly Dictionary<TKey, ICollection<TValue>> _items = new Dictionary<TKey, ICollection<TValue>>();

        /// <summary>
        /// Gets the collection of values stored under the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public ICollection<TValue> this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (!_items.ContainsKey(key))
                {
                    _items[key] = new List<TValue>();
                }

                return _items[key];
            }
        }

        /// <summary>
        /// Gets the collection of keys.
        /// </summary>
        public ICollection<TKey> Keys => _items.Keys;

        /// <summary>
        /// Gets the collection of collections of values.
        /// </summary>
        public ICollection<ICollection<TValue>> Values => _items.Values;

        /// <summary>
        /// Adds the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this[key].Add(value);
        }

        /// <summary>
        /// Removes the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if such a value existed and was removed; otherwise <c>false</c>.</returns>
        public bool Remove(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }


            if (!_items.ContainsKey(key))
            {
                return false;
            }
            return _items[key].Remove(value);
        }

        /// <summary>
        /// Removes all values for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>True</c> if any such values existed; otherwise <c>false</c>.</returns>
        public bool RemoveAll(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _items.Remove(key);
        }

        /// <summary>
        /// Removes all values.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Determines whether the multimap contains any values for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>True</c> if the multimap has one or more values for the specified key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _items.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the multimap contains the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if the multimap contains such a value; otherwise, <c>false</c>.</returns>
        public bool ContainsValue(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return _items.ContainsKey(key) && _items[key].Contains(value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a the multimap.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the multimap.</returns>
        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}