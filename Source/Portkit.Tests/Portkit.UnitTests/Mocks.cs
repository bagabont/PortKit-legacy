using System;
using System.Collections.Generic;
using System.Linq;

namespace Portkit.UnitTests
{
    internal interface ITestMock : IDisposable
    {

    }

    internal class TestMockOne : ITestMock
    {
        public string Id { get; set; }

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