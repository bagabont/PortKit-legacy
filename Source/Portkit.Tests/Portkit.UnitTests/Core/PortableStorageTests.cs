using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Portkit.ComponentModel;

namespace Portkit.UnitTests.Core
{
    [ExcludeFromCodeCoverage]
    public static class StaticTestFixture
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

    [TestFixture, ExcludeFromCodeCoverage]
    public class PortableStorageTests
    {
        [Test]
        public void StaticCallerMemberPropertyTest()
        {
            StaticTestFixture.TestCallerMember = "test1";
            var a = StaticTestFixture.TestCallerMember;
            Assert.IsTrue(a == "test1");
        }

        [Test]
        public void StaticKeyExpressionPropertyTest()
        {
            StaticTestFixture.TestKeyExpression = "test2";
            var a = StaticTestFixture.TestKeyExpression;
            Assert.IsTrue(a == "test2");
        }
    }
}
