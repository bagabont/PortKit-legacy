using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.Core.Extensions;

namespace Portkit.UnitTests.Core
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StringExTests
    {
        [TestMethod]
        public void GetSha1Test()
        {
            const string data = "abc";
            var expected = data.GetSha1();
            var actual = data.GetSha1();
            Assert.IsTrue(expected == actual);
        }
    }
}
