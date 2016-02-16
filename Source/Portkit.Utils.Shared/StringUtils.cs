using Portkit.Utils.Cryptography;
using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Portkit.Utils
{
    /// <summary>
    /// String extensions class.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Gets the SHA-1 hash of the string.
        /// </summary>
        /// <returns>SHA-1 hash value.</returns>
        public static string GetSha1(string source)
        {
            Sha1 shaGenerator = new Sha1();
            return shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a URI 
        /// with the string and ensures that the string does not require further escaping.        
        /// </summary>
        /// <returns>A System.Boolean value that is true if the string was well-formed; else false</returns>
        public static bool IsWellFormedUri(string source)
        {
            return !string.IsNullOrEmpty(source) && Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a URI 
        /// with the string and ensures that the string does not require further escaping.        
        /// </summary>
        /// <returns>A System.Boolean value that is true if the string was well-formed; else false</returns>
        public static bool IsWellFormedUri(string source, UriKind uriKind)
        {
            return Uri.IsWellFormedUriString(source, uriKind);
        }

        /// <summary>
        /// Escapes a string to extended ASCII.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <returns>Extended ASCII string.</returns>
        public static string ToExtendedAscii(string text)
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
        public static bool IsJson(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
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
        public static bool IsXml(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
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
        public static string Extract(string data, string from = null, string until = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
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

        /// <summary>
        /// Encrypts text string.
        /// </summary>
        /// <param name="plainText">Plain text to encrypt.</param>
        /// <param name="key">Encryption key.</param>
        /// <returns>Encrypted text string.</returns>
        public static string Encrypt(string plainText, string key)
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
        public static string Decrypt(string cipherText, string key)
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
