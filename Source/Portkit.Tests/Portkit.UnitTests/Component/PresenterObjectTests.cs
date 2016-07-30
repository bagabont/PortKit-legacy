using System.Diagnostics.CodeAnalysis;
using Moq;
using System.ComponentModel;
using NUnit.Framework;
using Portkit.ComponentModel;

namespace Portkit.UnitTests.Component
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class PresenterObjectTests
    {
        const string property = "TEST";
        private Mock<IThreadDispatcher> _dispatcherMock;

        [TestFixtureSetUp]
        public void Setup()
        {
            _dispatcherMock = new Mock<IThreadDispatcher>();
            BindableObject.RegisterDispatcher(_dispatcherMock.Object);
        }

        [Test]
        public void PropertyChangedRaisesTest()
        {
            var mock = new Mock<BindableObject>();
            mock.Object.PropertyChanged += (s, e) =>
                Assert.AreEqual(e.PropertyName, property);

            mock.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs(property));
        }

        [Test]
        public void ThreadDispatcherHasNoAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(false);
            Assert.IsFalse(_dispatcherMock.Object.HasThreadAccess);
        }

        [Test]
        public void ThreadDispatcherHasAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(true);
            Assert.IsTrue(_dispatcherMock.Object.HasThreadAccess);
        }

        [Test]
        public void PresenterDispatcherHasAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(true);

            var presenterMock = new Mock<BindableObject>();
            presenterMock.Object.PropertyChanged += (s, e) =>
                Assert.AreEqual(e.PropertyName, property);

            presenterMock.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs(property));
        }

        [Test]
        public void PresenterDispatcherHasNoAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(false);

            var presenterMock = new Mock<BindableObject>();
            presenterMock.Object.PropertyChanged += (s, e) =>
                Assert.AreEqual(e.PropertyName, property);

            presenterMock.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs(property));
        }
    }
}
