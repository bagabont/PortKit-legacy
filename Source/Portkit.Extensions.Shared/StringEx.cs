using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Portkit.Extensions
{
    /// <summary>
    /// String extensions class.
    /// </summary>
    public static class StringEx
    {
        public static string GetSha1(this string source)
        {
            var sha1 = new Sha1();
            return sha1.ComputeHashString(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a URI 
        /// with the string and ensures that the string does not require further escaping.        
        /// </summary>
        /// <returns>A System.Boolean value that is true if the string was well-formed; else false</returns>
        public static bool IsWellFormedUri(this string source)
        {
            return IsWellFormedUri(source, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a URI 
        /// with the string and ensures that the string does not require further escaping.        
        /// </summary>
        /// <returns>A System.Boolean value that is true if the string was well-formed; else false</returns>
        public static bool IsWellFormedUri(this string source, UriKind uriKind)
        {
            return !string.IsNullOrWhiteSpace(source) && Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute);
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
        /// Checks weather a string is enclosed between '{}' or '[]' brackets. 
        /// </summary>
        /// <param name="input">Source string</param>
        /// <returns>True if string looks like JSON, otherwise false.</returns>
        public static bool IsJson(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}") ||
                   input.StartsWith("[") && input.EndsWith("]");
        }

        /// <summary>
        /// Checks weather a string looks like XML.
        /// </summary>
        /// <param name="input">Source string</param>
        /// <returns>True if string looks like XML, otherwise false.</returns>
        public static bool IsXml(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
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
        /// <param name="fromText">Optional string to search after</param>
        /// <param name="untilText">Optional string to search before</param>
        /// <param name="comparison">Optional comparison for the search</param>
        /// <returns>Substring based on the search</returns>
        public static string Extract(this string data, string fromText = null, string untilText = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var fromLength = (fromText ?? string.Empty).Length;
            var startIndex = !string.IsNullOrWhiteSpace(fromText) ? data.IndexOf(fromText, comparison) + fromLength : 0;
            if (startIndex < fromLength)
            {
                throw new ArgumentException("Failed to find an instance of the first anchor", nameof(fromText));
            }
            var endIndex = !string.IsNullOrWhiteSpace(untilText) ? data.IndexOf(untilText, startIndex, comparison) : data.Length;
            if (endIndex < 0)
            {
                throw new ArgumentException("Failed to find an instance of the last anchor", nameof(untilText));
            }
            return data.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Encrypts text string.
        /// </summary>
        /// <param name="plainText">Plain text to encrypt.</param>
        /// <param name="key">Encryption key.</param>
        /// <returns>Encrypted text string.</returns>
        public static string Encrypt(this string plainText, string key)
        {
            var keyHash = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var plainBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf8);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            var symetricKey = aes.CreateSymmetricKey(keyHash);
            var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, plainBuffer, null);
            var cipherText = CryptographicBuffer.EncodeToBase64String(buffEncrypted);
            return cipherText;
        }

        /// <summary>
        /// Decrypts text string.
        /// </summary>
        /// <param name="cipherText">Cipher text to decrypt.</param>
        /// <param name="key">Encrypted key, used to encrypt the text string.</param>
        /// <returns>Plain text string.</returns>
        public static string Decrypt(this string cipherText, string key)
        {
            var keyHash = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var cipherBuffer = CryptographicBuffer.DecodeFromBase64String(cipherText);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            var symetricKey = aes.CreateSymmetricKey(keyHash);
            var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, cipherBuffer, null);
            string plainText = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);
            return plainText;
        }
    }
}
