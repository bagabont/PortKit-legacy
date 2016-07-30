using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Portkit.UnitTests.CollectionTests
{
    [TestClass]
    public class TypeSafeEnumTests
    {
        [TestMethod]
        public void EnumsWithNullValuesShouldBeEqual()
        {
            var e = new MockStringEnum(null);
            Assert.IsTrue(e.Equals(MockStringEnum.Null));
            Assert.IsTrue(e == MockStringEnum.Null);
        }

        [TestMethod]
        public void EnumWithValueAndEnumWithNullValueShouldBeDifferent()
        {
            var e = new MockStringEnum("test");

            Assert.IsFalse(e.Equals(MockStringEnum.Null));
            Assert.IsFalse(e == MockStringEnum.Null);
            Assert.IsTrue(e != MockStringEnum.Null);
        }

        [TestMethod]
        public void EnumsWithDifferentValuesShouldBeDifferent()
        {
            var e1 = new MockStringEnum("test1");
            var e2 = new MockStringEnum("test2");

            Assert.IsFalse(e1.Equals(e2));
            Assert.IsFalse(e1 == e2);
            Assert.IsFalse(e2 == e1);

            Assert.IsTrue(e1 != e2);
            Assert.IsTrue(e2 != e1);
        }

        [TestMethod]
        public void EnumShouldBeEqualToSameReference()
        {
            var e1 = new MockStringEnum("test");
            var e2 = e1;

            Assert.IsTrue(e1 == e2);
            Assert.IsTrue(e2 == e1);
            Assert.IsTrue(e1.Equals(e2));
            Assert.IsTrue(e2.Equals(e1));
            Assert.IsFalse(e1 != e2);
        }
    }
}
