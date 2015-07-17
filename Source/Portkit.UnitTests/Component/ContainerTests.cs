using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.ComponentModel;

namespace Portkit.UnitTests.Component
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ContainerTests
    {
        [TestMethod]
        public void RegisterClassOnlyTest()
        {
            var container = new PortableContainer();
            container.Register<TestMockOne>();

            var service = container.Resolve<TestMockOne>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TestMockOne));
        }

        [TestMethod]
        public void ResolveComponentImplementationInstanceEqualTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register<ITestMock, TestMockOne>(instance);

                Assert.AreSame(instance, container.Resolve<ITestMock>());
            }
        }

        [TestMethod]
        public void ResolveComponentImplementationInstanceNotEqualTest()
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
        public void ResolveImplementationInstanceEqualTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register<ITestMock>(instance);

                Assert.AreSame(instance, container.Resolve<ITestMock>());
            }
        }

        [TestMethod]
        public void ResolveImplementationInstanceNotEqualTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register<ITestMock>(new TestMockOne());
                var resolved = container.Resolve<ITestMock>();
                Assert.AreNotSame(instance, resolved);
            }
        }

        [TestMethod]
        public void RegisterObjectInstanceWithoutExplicitComponentTypeTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register(instance);
                var resolved = container.Resolve<TestMockOne>();

                Assert.AreSame(instance, resolved);
            }
        }

        [TestMethod]
        public void RegisterObjectInstanceWithExplicitComponentTypeTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register<ITestMock>(instance);
                var resolved = container.Resolve<ITestMock>();

                Assert.AreSame(instance, resolved);
            }
        }

        [TestMethod]
        public void ThrowExceptionIfNotRegisteredTest()
        {
            try
            {
                var container = new PortableContainer();
                container.Resolve<ITestMock>();
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
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();

            var service = container.Resolve<ITestMock>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TestMockOne));
        }

        [TestMethod]
        public void ResolveFirstRegisteredTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();
            container.Register<ITestMock, TestMockTwo>();

            var service = container.Resolve<ITestMock>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TestMockOne));
        }

        [TestMethod]
        public void EnumerableArgumentInConstructorTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();
            container.Register<ITestMock, TestMockTwo>();
            container.Register<IMockEnumConstructor, MockEnumConstructor>();

            var asks = container.Resolve<IMockEnumConstructor>();

            Assert.IsNotNull(asks);

            Assert.IsNotNull(asks.SecondService);
            Assert.IsInstanceOfType(asks.SecondService, typeof(TestMockTwo));
        }

        [TestMethod]
        public void ReturnCachedInstanceTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();

            var service1 = container.Resolve<ITestMock>();
            var service2 = container.Resolve<ITestMock>();

            Assert.AreSame(service1, service2);
        }

        [TestMethod]
        public void ReturnDifferentInstanceWhenRegisteredAsTransientsTest()
        {
            var container = new PortableContainer();
            container.RegisterTransient<ITestMock, TestMockOne>();

            var service1 = container.Resolve<ITestMock>();
            var service2 = container.Resolve<ITestMock>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void ResolveInstanceDifferentImplementationTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();
            container.Register<ITestMock, TestMockTwo>();
            List<ITestMock> services = container.ResolveAll<ITestMock>().ToList();


            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 2);

            Assert.IsInstanceOfType(services[0], typeof(TestMockOne));
            Assert.IsInstanceOfType(services[1], typeof(TestMockTwo));
        }

        [TestMethod]
        public void RemoveAllDisposesObjectsTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();
            container.Register<ITestMock, TestMockTwo>();

            List<ITestMock> services = container.ResolveAll<ITestMock>().ToList();
            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 2);

            container.RemoveAll<ITestMock>();
        }

        [TestMethod]
        public void RemovesAllTest()
        {
            PortableContainer container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();

            var service1 = container.Resolve<ITestMock>();
            Assert.IsNotNull(service1);

            container.RemoveAll<ITestMock>();
            try
            {
                container.Resolve<ITestMock>();
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void ResolveAllTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();

            var services = container.ResolveAll<ITestMock>().ToList();

            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 1);
            Assert.IsInstanceOfType(services[0], typeof(TestMockOne));
        }
    }
}