using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Portkit.Time;

namespace Portkit.UnitTests.TimeTests
{
    [TestClass]
    public class NtpTimeSyncClientTests
    {
        [TestMethod]
        public async Task ShouldGetNetworkTimeViaNtpTest()
        {
            var ntpTimeSync = new NtpTimeSyncClient();
            var networkUtcNow = await ntpTimeSync.GetNetworkUtcTimeAsync();
            var utcNow = DateTime.UtcNow;
            var offset = networkUtcNow - utcNow;
            var tolerance = TimeSpan.FromMinutes(1);

            Assert.IsTrue(offset.Duration() < tolerance,
                $"Offset does not fit in the allowed tolerance of {tolerance}. System UTC time: {utcNow}. Network UTC time: {networkUtcNow.ToString("G")}");
        }
    }
}
