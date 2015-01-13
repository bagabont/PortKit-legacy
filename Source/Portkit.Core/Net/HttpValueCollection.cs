﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portkit.Core.Net
{
    /// <summary>
    /// Represents a collection of <see cref="HttpValue"/> objects.
    /// </summary>
    public class HttpValueCollection : List<HttpValue>
    {
        #region Parameters

        /// <summary>
        /// Gets a parameter value from the key.
        /// </summary>
        /// <param name="key">Key of the parameter.</param>
        public string this[string key]
        {
            get
            {
                return this.First(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Value;
            }
            set
            {
                this.First(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Value = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance on the <see cref="HttpValueCollection"/> class.
        /// </summary>
        public HttpValueCollection()
        {
        }

        /// <summary>
        /// Creates new instance on the <see cref="HttpValueCollection"/> class.
        /// </summary>
        public HttpValueCollection(string query)
            : this(query, true)
        {
        }

        /// <summary>
        /// Creates new instance on the <see cref="HttpValueCollection"/> class.
        /// </summary>
        public HttpValueCollection(string query, bool urlencoded)
        {
            if (!string.IsNullOrEmpty(query))
            {
                FillFromString(query, urlencoded);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new http key - value pair to the collection.
        /// </summary>
        public void Add(string key, string value)
        {
            Add(new HttpValue(key, value));
        }

        /// <summary>
        /// Checks if the collection contains a key.
        /// </summary>
        /// <param name="key">Key to check for.</param>
        /// <returns>True if the key is contained, otherwise false.</returns>
        public bool ContainsKey(string key)
        {
            return this.Any(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets all values of the provided key.
        /// </summary>
        /// <param name="key">Key of the values.</param>
        /// <returns>Array of values.</returns>
        public string[] GetValues(string key)
        {
            return
                this.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Value)
                    .ToArray();
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="key">Key of the item.</param>
        public void Remove(string key)
        {
            var items = this.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
            foreach (var item in items)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Converts the value collection to an escaped query string.
        /// </summary>
        /// <returns>Query string.</returns>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// Converts the value collection to query string.
        /// </summary>
        /// <param name="urlencoded">If set to true, escapes the data string.</param>
        /// <returns>Query string.</returns>
        public virtual string ToString(bool urlencoded)
        {
            return ToString(urlencoded, null);
        }

        /// <summary>
        /// Converts the value collection to query string.
        /// </summary>
        /// <param name="urlencoded">If set to true, escapes the data string.</param>
        /// <param name="excludeKeys">Keys to exclude from the query.</param>
        /// <returns>Query string.</returns>
        public virtual string ToString(bool urlencoded, IDictionary excludeKeys)
        {
            if (Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var item in this)
            {
                var key = item.Key;
                if ((excludeKeys == null) || !excludeKeys.Contains(key))
                {
                    var value = item.Value;
                    if (urlencoded)
                    {
                        key = Uri.EscapeDataString(key);
                    }
                    if (sb.Length > 0)
                    {
                        sb.Append('&');
                    }
                    sb.Append((key != null) ? (key + "=") : string.Empty);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (urlencoded)
                        {
                            value = Uri.EscapeDataString(value);
                        }
                        sb.Append(value);
                    }
                }
            }
            return sb.ToString();
        }

        private void FillFromString(string query, bool urlencoded)
        {
            var num = (query != null) ? query.Length : 0;
            for (var i = 0; i < num; i++)
            {
                var startIndex = i;
                var num4 = -1;
                while (i < num)
                {
                    var ch = query[i];
                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }
                    i++;
                }
                string str = null;
                string str2;
                if (num4 >= 0)
                {
                    str = query.Substring(startIndex, num4 - startIndex);
                    str2 = query.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    str2 = query.Substring(startIndex, i - startIndex);
                }

                if (urlencoded)
                {
                    Add(Uri.UnescapeDataString(str), Uri.UnescapeDataString(str2));
                }
                else
                {
                    Add(str, str2);
                }
                if ((i == (num - 1)) && (query[i] == '&'))
                {
                    Add(null, string.Empty);
                }
            }
        }

        #endregion
    }
}