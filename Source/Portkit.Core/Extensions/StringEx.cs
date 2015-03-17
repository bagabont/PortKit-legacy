using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Portkit.Core.Cryptography;
using Portkit.Core.Net;

namespace Portkit.Core.Extensions
{
    /// <summary>
    /// String extensions class.
    /// </summary>
    public static class StringEx
    {
        /// <summary>
        /// Gets the SHA-1 hash of the string.
        /// </summary>
        /// <returns>SHA-1 hash value.</returns>
        public static string GetSha1(this string source)
        {
            var shaGenerator = new Sha1();
            return shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a URI 
        /// with the string and ensures that the string does not require further escaping.        
        /// </summary>
        /// <returns>A System.Boolean value that is true if the string was well-formed; else false</returns>
        public static bool IsWellFormedUri(this string source)
        {
            return !string.IsNullOrEmpty(source) && Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a URI 
        /// with the string and ensures that the string does not require further escaping.        
        /// </summary>
        /// <returns>A System.Boolean value that is true if the string was well-formed; else false</returns>
        public static bool IsWellFormedUri(this string source, UriKind uriKind)
        {
            return Uri.IsWellFormedUriString(source, uriKind);
        }

        /// <summary>
        /// Escapes a string to extended ASCII.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <returns>Extended ASCII string.</returns>
        public static string ToExtendedAscii(this string text)
        {
            var sb = new StringBuilder();
            foreach (var c in text)
            {
                var asciiCode = Convert.ToInt32(c);
                if (asciiCode > 127)
                {
                    //Escape character
                    sb.AppendFormat("&#{0};", asciiCode);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parses all query parameters from the URI to a dictionary.
        /// </summary>
        /// <param name="source">URI with query.</param>
        /// <returns>Mapped parameter name and value dictionary.</returns>
        public static IDictionary<string, string> ParseQueryString(this Uri source)
        {
            var parameters = HttpUtility.ParseQueryString(source.OriginalString);
            return parameters.IsNullOrEmpty() ? null : parameters.ToDictionary(p => p.Key, p => p.Value);
        }

        /// <summary>
        /// Checks weather a string is enclosed between '{}' or '[]' brackets. 
        /// </summary>
        /// <param name="input">Source string</param>
        /// <returns>True if string looks like JSON, otherwise false.</returns>
        public static bool IsJson(this string input)
        {
            if (String.IsNullOrEmpty(input) || String.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        /// <summary>
        /// Checks weather a string looks like XML.
        /// </summary>
        /// <param name="input">Source string</param>
        /// <returns>True if string looks like XML, otherwise false.</returns>
        public static bool IsXml(this string input)
        {
            if (String.IsNullOrEmpty(input) || String.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            input = input.Trim();
            return input.StartsWith("<") && input.EndsWith(">");
        }

        /// <summary>
        /// Takes a substring between two anchor strings (or the end of the string if that anchor is null)
        /// </summary>
        /// <param name="data">String to operate on</param>
        /// <param name="from">Optional string to search after</param>
        /// <param name="until">Optional string to search before</param>
        /// <param name="comparison">Optional comparison for the search</param>
        /// <returns>Substring based on the search</returns>
        public static string Extract(this string data, string from = null, string until = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var fromLength = (from ?? string.Empty).Length;
            var startIndex = !string.IsNullOrEmpty(from)
                ? data.IndexOf(from, comparison) + fromLength
                : 0;

            if (startIndex < fromLength)
            {
                throw new ArgumentException("from: Failed to find an instance of the first anchor");
            }

            var endIndex = !string.IsNullOrEmpty(until)
            ? data.IndexOf(until, startIndex, comparison)
            : data.Length;

            if (endIndex < 0)
            {
                throw new ArgumentException("until: Failed to find an instance of the last anchor");
            }

            var subString = data.Substring(startIndex, endIndex - startIndex);
            return subString;
        }
    }
}
