using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Portkit.Time
{
    public class HttpTimeSyncClient : ITimeSyncClient
    {
        public async Task<DateTime> GetNetworkUtcTimeAsync(TimeSpan timeout)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                var content = await http.GetStringAsync("http://www.timeapi.org/utc/now");
                var accurateUtcTime = DateTime.Parse(content).ToUniversalTime();
                return accurateUtcTime;
            }
        }

        public async Task<DateTime> GetNetworkUtcTimeAsync()
        {
            return await GetNetworkUtcTimeAsync(TimeSpan.FromSeconds(45));
        }
    }
}
