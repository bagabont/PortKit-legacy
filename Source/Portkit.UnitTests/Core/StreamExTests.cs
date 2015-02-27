using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.Core.Extensions;

namespace Portkit.UnitTests.Core
{
    [TestClass, ExcludeFromCodeCoverage]
    public class StreamExTests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), TestMethod]
        public void CopyToDisposedStreamThrowsExceptionTest()
        {
            using (var sourceStream = new MemoryStream())
            {
                using (var destinationStream = new MemoryStream())
                {
                    using (destinationStream)
                    {
                        destinationStream.Position = 0;
                    }
                    var threw = false;
                    try
                    {
                        StreamEx.CopyTo(sourceStream, destinationStream);
                    }
                    catch (ObjectDisposedException)
                    {
                        threw = true;
                    }
                    Assert.IsTrue(threw);
                }
            }
        }

        [TestMethod]
        public void CopyFromDisposedStreamThrowsExceptionTest()
        {
            var sourceStream = new MemoryStream();
            var destinationStream = new MemoryStream();
            using (sourceStream)
            {
                sourceStream.Position = 0;
            }
            var threw = false;
            try
            {
                StreamEx.CopyTo(sourceStream, destinationStream);
            }
            catch (ObjectDisposedException)
            {
                threw = true;
            }
            Assert.IsTrue(threw);
        }

        [TestMethod]
        public void CopyToNullStreamThrowsExceptionTest()
        {
            var sourceStream = new MemoryStream();
            MemoryStream destinationStream = null;
            var threw = false;
            try
            {
                // destinationStream must be null
                StreamEx.CopyTo(sourceStream, destinationStream);
            }
            catch (ArgumentNullException)
            {
                threw = true;
            }
            Assert.IsTrue(threw);
        }

        [TestMethod]
        public void CopySourceStreamToDestinationTest()
        {
            byte[] actualArray;
            byte[] exptectedArray;

            using (var sourceStream = new MemoryStream())
            {
                using (var sw = new StreamWriter(sourceStream))
                {
                    sw.Write("Hello World!");
                    sw.Flush();
                    exptectedArray = sourceStream.ToArray();
                    using (var destinationStream = new MemoryStream())
                    {
                        sourceStream.Position = 0;
                        StreamEx.CopyTo(sourceStream, destinationStream);
                        actualArray = destinationStream.ToArray();
                    }
                }
            }
            Assert.IsTrue(actualArray.Length == exptectedArray.Length);
            for (var i = 0; i < exptectedArray.Length; i++)
            {
                Assert.IsTrue(actualArray[i] == exptectedArray[i]);
            }
        }

        [TestMethod]
        public void TryResetPositionTest()
        {
            using (var ms = new MemoryStream(new byte[] { 1, 2, 3, 4 }))
            {
                ms.Seek(3, SeekOrigin.Begin);
                Assert.IsTrue(ms.Position == 3);

                var success = ms.TryResetPosition();
                Assert.IsTrue(success && ms.Position == 0);
            }
        }

        [TestMethod]
        public void TryResetPositionFailsTest()
        {
            using (var ms = new UnseekableStream())
            {
                var success = ms.TryResetPosition();
                Assert.IsTrue(success == false);
            }
        }
    }
}
