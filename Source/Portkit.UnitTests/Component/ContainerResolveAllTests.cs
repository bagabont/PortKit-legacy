using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.ComponentModel;
using Portkit.ComponentModel.Communication;

namespace Portkit.UnitTests.Component
{
    [TestClass]
    public class ContainerResolveAllTests
    {
        private readonly PortableContainer _container = new PortableContainer();

        [TestMethod]
        public void ResolveAllTest()
        {
            _container.Register<ITestMock, TestMockOne>();

            List<ITestMock> services = _container.ResolveAll<ITestMock>().ToList();

            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 1);
            Assert.IsInstanceOfType(services[0], typeof(TestMockOne));
        }
    }
}