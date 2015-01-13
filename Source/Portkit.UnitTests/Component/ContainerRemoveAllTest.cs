using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.ComponentModel.Communication;

namespace Portkit.UnitTests.Component
{
    [TestClass]
    public class ContainerRemoveAllTests
    {
        private readonly PortableContainer _container = new PortableContainer();

        [TestMethod]
        public void RemovesAllTest()
        {
            _container.Register<ITestMock, TestMockOne>();

            var service1 = _container.Resolve<ITestMock>();
            Assert.IsNotNull(service1);

            _container.RemoveAll<ITestMock>();
            try
            {
                _container.Resolve<ITestMock>();
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Assert.Fail();
        }
    }
}