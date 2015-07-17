using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Portkit.ComponentModel;

namespace Portkit.UnitTests.Component
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class ContainerTests
    {
        [Test]
        public void RegisterClassOnlyTest()
        {
            var container = new PortableContainer();
            container.Register<TestMockOne>();

            var service = container.Resolve<TestMockOne>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOf<TestMockOne>(service);
        }

        [Test]
        public void ResolveComponentImplementationInstanceEqualTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register<ITestMock, TestMockOne>(instance);

                Assert.AreSame(instance, container.Resolve<ITestMock>());
            }
        }

        [Test]
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

        [Test]
        public void ResolveImplementationInstanceEqualTest()
        {
            using (var container = new PortableContainer())
            {
                var instance = new TestMockOne();
                container.Register<ITestMock>(instance);

                Assert.AreSame(instance, container.Resolve<ITestMock>());
            }
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void ResolveRegisteredTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();

            var service = container.Resolve<ITestMock>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOf<TestMockOne>(service);
        }

        [Test]
        public void ResolveFirstRegisteredTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();
            container.Register<ITestMock, TestMockTwo>();

            var service = container.Resolve<ITestMock>();

            Assert.IsNotNull(service);
            Assert.IsInstanceOf<TestMockOne>(service);
        }

        [Test]
        public void EnumerableArgumentInConstructorTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();
            container.Register<ITestMock, TestMockTwo>();
            container.Register<IMockEnumConstructor, MockEnumConstructor>();

            var asks = container.Resolve<IMockEnumConstructor>();

            Assert.IsNotNull(asks);

            Assert.IsNotNull(asks.SecondService);
            Assert.IsInstanceOf<TestMockTwo>(asks.SecondService);
        }

        [Test]
        public void ReturnCachedInstanceTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();

            var service1 = container.Resolve<ITestMock>();
            var service2 = container.Resolve<ITestMock>();

            Assert.AreSame(service1, service2);
        }

        [Test]
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

        [Test]
        public void ResolveInstanceDifferentImplementationTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();
            container.Register<ITestMock, TestMockTwo>();
            List<ITestMock> services = container.ResolveAll<ITestMock>().ToList();


            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 2);

            Assert.IsInstanceOf<TestMockOne>(services[0]);
            Assert.IsInstanceOf<TestMockTwo>(services[1]);
        }

        [Test]
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

        [Test]
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

        [Test]
        public void ResolveAllTest()
        {
            var container = new PortableContainer();
            container.Register<ITestMock, TestMockOne>();

            var services = container.ResolveAll<ITestMock>().ToList();

            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count == 1);
            Assert.IsInstanceOf<TestMockOne>(services[0]);
        }
    }
}