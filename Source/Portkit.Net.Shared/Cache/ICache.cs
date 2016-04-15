using System.Threading.Tasks;

namespace Portkit.Net.Cache
{
    public interface ICache<in TKey, TValue> where TValue : class
    {
        Task<TValue> GetAsync(TKey key);

        Task PutAsync(TKey key, TValue value);
    }
}