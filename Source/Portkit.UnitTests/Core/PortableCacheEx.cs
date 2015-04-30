using System.Threading.Tasks;
using Portkit.Core.Caching;

namespace Portkit.UnitTests.Core
{
    internal static class PortableCacheEx
    {
        internal static async Task<TValue> GetWithDelay<TKey, TValue>(this PortableMemoryCache memoryCache, TKey key, int millisecondsDelay) where TValue : class
        {
            await Task.Delay(millisecondsDelay);
            return memoryCache.Get<TKey, TValue>(key);
        }
    }
}