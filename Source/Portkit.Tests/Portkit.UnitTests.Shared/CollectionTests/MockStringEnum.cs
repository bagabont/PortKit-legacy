using Portkit.Utils.Collections;

namespace Portkit.UnitTests.CollectionTests
{
    public class MockStringEnum : TypeSafeEnum<string>
    {
        public static MockStringEnum Null = new MockStringEnum(null);

        public MockStringEnum(string value) : base(value)
        {
        }
    }
}