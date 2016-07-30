using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Portkit.Core.Extensions;

namespace Portkit.UnitTests.Core
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class CollectionExTests
    {
        [Test]
        public void IsNullTest()
        {
            IList<object> list = null;
            Assert.IsTrue(list.IsNullOrEmpty());
        }

        [Test]
        public void IsEmptyTest()
        {
            var list = new List<object>();
            Assert.IsTrue(list.IsNullOrEmpty());
        }

        [Test]
        public void IsNotEmptyTest()
        {
            var list = new List<object> { new object() };
            Assert.IsFalse(list.IsNullOrEmpty());
        }

        [Test]
        public void IsOrderedAscendingTest()
        {
            var data = new Dictionary<string, object>
            {
                {"A", new object()}, 
                {"B", new object()}, 
                {"C", new object()}
            };
            Assert.IsTrue(data.IsOrdered(c => c.Key));
        }

        [Test]
        public void IsNotOrderedTest()
        {
            var unorderedData = new Dictionary<string, object>
            {
                {"C", new object()}, 
                {"A", new object()}, 
                {"B", new object()}
            };
            Assert.IsFalse(unorderedData.IsOrdered(c => c.Key));
        }

        [Test]
        public void IsOrderedDescendingTest()
        {
            var descendingData = new Dictionary<string, object>
            {
                {"C", new object()}, 
                {"B", new object()}, 
                {"A", new object()}
            };
            Assert.IsTrue(descendingData.IsOrdered(c => c.Key, false));
        }
    }
}
