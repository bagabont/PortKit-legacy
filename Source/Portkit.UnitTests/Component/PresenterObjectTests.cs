using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel;
using Portkit.ComponentModel.Presenter;
using Portkit.ComponentModel.Threading;

namespace Portkit.UnitTests.Component
{
    [TestClass, ExcludeFromCodeCoverage]
    public class PresenterObjectTests
    {
        private Mock<IThreadDispatcher> _dispatcherMock;

        [TestInitialize]
        public void Setup()
        {
            _dispatcherMock = new Mock<IThreadDispatcher>();
            PresenterObject.UIDispatcher = _dispatcherMock.Object;
        }

        [TestMethod]
        public void PropertyChangedRaisesTest()
        {
            var mock = new Mock<PresenterObject>();
            const string property = "TEST";
            mock.Object.PropertyChanged += (s, e) =>
                Assert.AreEqual(e.PropertyName, property);

            mock.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs(property));
        }

        [TestMethod]
        public void ThreadDispatcherHasNoAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(false);
            Assert.IsFalse(_dispatcherMock.Object.HasThreadAccess);
        }

        [TestMethod]
        public void ThreadDispatcherHasAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(true);
            Assert.IsTrue(_dispatcherMock.Object.HasThreadAccess);
        }

        [TestMethod]
        public void PresenterDispatcherHasAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(true);

            var presenterMock = new Mock<PresenterObject>();
            const string property = "TEST";
            presenterMock.Object.PropertyChanged += (s, e) =>
                Assert.AreEqual(e.PropertyName, property);

            presenterMock.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs(property));
        }

        [TestMethod]
        public void PresenterDispatcherHasNoAccessTest()
        {
            _dispatcherMock.SetupGet(m => m.HasThreadAccess).Returns(false);

            var presenterMock = new Mock<PresenterObject>();
            const string property = "TEST";
            presenterMock.Object.PropertyChanged += (s, e) =>
                Assert.AreEqual(e.PropertyName, property);

            presenterMock.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs(property));
        }
    }
}
