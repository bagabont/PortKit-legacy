using System.Collections.Generic;
using System.Linq;
using Portkit.ComponentModel;

namespace Portkit.UnitTests.Component
{
    internal interface ITestMock : IStateDisposable
    {
    }

    internal class TestMockOne : ITestMock
    {
        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }
    }

    internal class TestMockTwo : ITestMock
    {
        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }
    }

    internal class MockEnumConstructor : IMockEnumConstructor
    {
        public MockEnumConstructor(IEnumerable<ITestMock> enumService)
        {
            SecondService = enumService.Skip(1).First();
        }

        public ITestMock SecondService { get; set; }
    }

    internal interface IMockEnumConstructor
    {
        ITestMock SecondService { get; set; }
    }

}