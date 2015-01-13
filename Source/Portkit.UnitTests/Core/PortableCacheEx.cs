using System.Threading.Tasks;
using Portkit.Core.Caching;

namespace Portkit.UnitTests.Core
{
    internal static class PortableCacheEx
    {
        internal static async Task<TValue> GetWithDelay<TKey, TValue>(this PortableCache cache, TKey key, int millisecondsDelay) where TValue : class
        {
            await Task.Delay(millisecondsDelay);
            return cache.Get<TKey, TValue>(key);
        }
    }
}