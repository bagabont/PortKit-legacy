using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.ComponentModel.Presenter;

namespace Portkit.UnitTests.Component
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void RelayCommandExecuteTest()
        {
            bool hasExecuted = false;
            var command = new RelayCommand(s =>
            {
                hasExecuted = true;
            });
            command.Execute(null);

            Assert.IsTrue(hasExecuted);
        }

        [TestMethod]
        public void RelayCommandCanExecuteTest()
        {
            bool hasExecuted = false;
            var command = new RelayCommand(() => { hasExecuted = true; }, _ => false);
            command.Execute(null);

            Assert.IsFalse(command.CanExecute(null));
            Assert.IsFalse(hasExecuted);
        }

        [TestMethod]
        public void RelayCommandRaiseCanExecuteChanged()
        {
            bool canExecute = false;

            bool hasExecuted = false;
            var command = new RelayCommand(
                () =>
                {
                    hasExecuted = true;
                },
                _ => canExecute);
            command.Execute(null);

            Assert.IsFalse(command.CanExecute(null));
            Assert.IsFalse(hasExecuted);

            canExecute = true;

            Assert.IsTrue(command.CanExecute(null));
            command.Execute(null);
            Assert.IsTrue(hasExecuted);
        }
    }
}
