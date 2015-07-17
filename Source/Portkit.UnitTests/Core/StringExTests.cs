using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Portkit.Core.Extensions;

namespace Portkit.UnitTests.Core
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class StringExTests
    {
        [Test]
        public void GetSha1Test()
        {
            const string data = "abc";
            var expected = data.GetSha1();
            var actual = data.GetSha1();
            Assert.IsTrue(expected == actual);
        }
    }
}
