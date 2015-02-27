using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Portkit.UnitTests.Component
{
    [TestClass]
    public class ContainerResolveTests
    {
        private readonly PortableContainer _container = new PortableContainer();

        [TestMethod]
        public void RegisterClassOnlyTest()
        {
            _container.Register<TestMockOne>();

            var service = _container.Resolve<TestMockOne>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TestMockOne));
        }

        [TestMethod]
        public void ResolveInstanceEqualTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register<ITestMock, TestMockOne>(instance);

                Assert.AreSame(instance, container.Resolve<ITestMock>());
            }
        }

        [TestMethod]
        public void ResolveInstanceNotEqualTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();

                container.Register<ITestMock, TestMockOne>(new TestMockOne());

                var resolved = container.Resolve<ITestMock>();

                Assert.AreNotSame(instance, resolved);
            }
        }

        [TestMethod]
        public void ThrowExceptionIfNotRegisteredTest()
        {
            try
            {
                _container.Resolve<ITestMock>();
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Assert.Fail("Did not throw exception");
        }

        [TestMethod]
        public void ResolveRegisteredTest()
        {
            _container.Register<ITestMock, TestMockOne>();

            var service = _container.Resolve<ITestMock>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TestMockOne));
        }

        [TestMethod]
        public void ResolveFirstRegisteredTest()
        {
            _container.Register<ITestMock, TestMockOne>();
            _container.Register<ITestMock, TestMockTwo>();

            var service = _container.Resolve<ITestMock>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TestMockOne));
        }

        [TestMethod]
        public void EnumerableArgumentInConstructorTest()
        {
            _container.Register<ITestMock, TestMockOne>();
            _container.Register<ITestMock, TestMockTwo>();
            _container.Register<IMockEnumConstructor, MockEnumConstructor>();

            var asks = _container.Resolve<IMockEnumConstructor>();

            Assert.IsNotNull(asks);

            Assert.IsNotNull(asks.SecondService);
            Assert.IsInstanceOfType(asks.SecondService, typeof(TestMockTwo));
        }

        [TestMethod]
        public void ReturnCachedInstanceTest()
        {
            _container.Register<ITestMock, TestMockOne>();

            var service1 = _container.Resolve<ITestMock>();
            var service2 = _container.Resolve<ITestMock>();

            Assert.AreSame(service1, service2);
        }

        [TestMethod]
        public void ReturnDifferentInstanceWhenRegisteredAsTransientsTest()
        {
            _container.RegisterTransient<ITestMock, TestMockOne>();

            var service1 = _container.Resolve<ITestMock>();
            var service2 = _container.Resolve<ITestMock>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void ResolveInstanceDifferentImplementationTest()
        {
            _container.Register<ITestMock, TestMockOne>();
            _container.Register<ITestMock, TestMockTwo>();
            List<ITestMock> services = _container.ResolveAll<ITestMock>().ToList();


            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 2);

            Assert.IsInstanceOfType(services[0], typeof(TestMockOne));
            Assert.IsInstanceOfType(services[1], typeof(TestMockTwo));
        }

        [TestMethod]
        public void RemoveAllDisposesObjectsTest()
        {
            _container.Register<ITestMock, TestMockOne>();
            _container.Register<ITestMock, TestMockTwo>();

            List<ITestMock> services = _container.ResolveAll<ITestMock>().ToList();
            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 2);

            _container.RemoveAll<ITestMock>();
        }
    }
}