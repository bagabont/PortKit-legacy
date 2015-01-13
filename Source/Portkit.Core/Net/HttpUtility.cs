using System;

namespace Portkit.Core.Net
{
    /// <summary>
    /// Represents a class to parse query strings.
    /// </summary>
    public sealed class HttpUtility
    {
        /// <summary>
        /// Parses a query string to a <see cref="HttpValueCollection"/>.
        /// </summary>
        /// <param name="query">Query string to be parsed.</param>
        /// <returns>Collection that contains key-value parameters.</returns>
        /// <exception cref="ArgumentNullException">If query is null.</exception>
        public static HttpValueCollection ParseQueryString(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if ((query.Length > 0) && (query[0] == '?'))
            {
                query = query.Substring(1);
            }
            return new HttpValueCollection(query, true);
        }
    }
}