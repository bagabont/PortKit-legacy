using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Portkit.ComponentModel;

namespace Portkit.UnitTests.Component
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class RelayCommandTests
    {
        [Test]
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

        [Test]
        public void RelayCommandCanExecuteTest()
        {
            bool hasExecuted = false;
            var command = new RelayCommand(() => { hasExecuted = true; }, _ => false);
            command.Execute(null);

            Assert.IsFalse(command.CanExecute(null));
            Assert.IsFalse(hasExecuted);
        }

        [Test]
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
