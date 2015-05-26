using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.Logging;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Portkit.UnitTests.Logging
{
    [TestClass]
    public class XmlLoggerTests
    {
        [TestMethod]
        public void LogExceptionTest()
        {
            try
            {
                throw new NotImplementedException("TEST");
            }
            catch (Exception ex)
            {
                XmlLogger.Log.LogException(ex, "Trace not implemented exception.");
            }
            string log = XmlLogger.Log.GetLogContent();
            XElement xml = XElement.Parse(log);
            var exceptionNode = xml.Descendants("exception");
            Assert.IsTrue(exceptionNode.Any());
        }

        [TestMethod]
        public void LogErrorTest()
        {
            XmlLogger.Log.Error("Example error\n new line.");
            string log = XmlLogger.Log.GetLogContent();
            XElement xml = XElement.Parse(log);
            var errorNodes = xml.Descendants("error");
            Assert.IsTrue(errorNodes.Any());
        }
    }
}
