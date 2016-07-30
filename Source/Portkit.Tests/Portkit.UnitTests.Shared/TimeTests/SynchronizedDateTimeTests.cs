using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Portkit.Time;

namespace Portkit.UnitTests.TimeTests
{
    [TestClass]
    public class SynchronizedDateTimeTests
    {
        [TestMethod]
        public async Task ShouldProvideAccurateDateTimeTest()
        {
            await SynchronizedDateTime.SynchronizeAsync();

            var utcNow = DateTime.UtcNow;
            var networkUtcNow = SynchronizedDateTime.UtcNow;

            var offset = networkUtcNow - utcNow;
            var tolerance = TimeSpan.FromMinutes(1);

            Assert.IsTrue(offset.Duration() < tolerance,
              $"Offset does not fit in the allowed tolerance of {tolerance}. System UTC time: {utcNow}. Network UTC time: {networkUtcNow.ToString("G")}");
        }
    }
}
