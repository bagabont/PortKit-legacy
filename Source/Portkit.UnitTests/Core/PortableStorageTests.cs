using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.Core;

namespace Portkit.UnitTests.Core
{
    public static class StaticTestClass
    {
        private static readonly PortableStorage Storage = new PortableStorage(new Dictionary<string, object>());

        public static string TestKeyExpression
        {
            get
            {
                return Storage.GetValue(() => TestKeyExpression);
            }
            set
            {
                Storage.SetValue(value, () => TestKeyExpression);
            }
        }

        public static string TestCallerMember
        {
            get
            {
                return Storage.GetValue(() => TestCallerMember);
            }
            set
            {
                Storage.SetValue(value, () => TestCallerMember);
            }
        }
    }

    [TestClass]
    public class PortableStorageTests
    {
        [TestMethod]
        public void StaticCallerMemberPropertyTest()
        {
            StaticTestClass.TestCallerMember = "test1";
            var a = StaticTestClass.TestCallerMember;
            Assert.IsTrue(a == "test1");
        }

        [TestMethod]
        public void StaticKeyExpressionPropertyTest()
        {
            StaticTestClass.TestKeyExpression = "test2";
            var a = StaticTestClass.TestKeyExpression;
            Assert.IsTrue(a == "test2");
        }
    }
}
