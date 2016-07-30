using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Portkit.Time;

namespace Portkit.UnitTests.TimeTests
{
    [TestClass]
    public class HttpTimeSyncClientTests
    {
        [TestMethod]
        public async Task ShouldGetNetworkTimeViaHttpTest()
        {
            var httpTimeSync = new HttpTimeSyncClient();
            var networkUtcNow = await httpTimeSync.GetNetworkUtcTimeAsync();
            var utcNow = DateTime.UtcNow;
            var offset = networkUtcNow - utcNow;
            var tolerance = TimeSpan.FromMinutes(1);

            Assert.IsTrue(offset.Duration() < tolerance,
                $"Offset does not fit in the allowed tolerance of {tolerance}. System UTC time: {utcNow}. Network UTC time: {networkUtcNow.ToString("G")}");
        }
    }
}
