using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Portkit.Net.Shared
{
    internal static class KeyUtil
    {
        public static string ComputeHash(string value)
        {
            var buffer = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);
            var hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var hashBuffer = hashAlgorithm.HashData(buffer);

            return CryptographicBuffer.EncodeToBase64String(hashBuffer);
        }
    }
}
