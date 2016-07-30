using System;
using System.Threading.Tasks;

namespace Portkit.Time
{
    public interface ITimeSyncClient
    {
        Task<DateTime> GetNetworkUtcTimeAsync(TimeSpan timeout);
    }
}
