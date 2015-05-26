using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
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
        private XmlSchemaSet _schemas;

        [TestInitialize]
        [TestMethod]
        public void Initialize()
        {
            const string resourceName = "Portkit.UnitTests.Logging.LogSchema.xsd";

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                _schemas = new XmlSchemaSet();
                _schemas.Add("", XmlReader.Create(reader));
                _schemas.Compile();
            }
        }

        [TestMethod]
        public void LogNestedExceptionsTest()
        {
            XmlLogger.Log.SetSessionAttribute("one", "text");
            XmlLogger.Log.SetSessionAttribute("two", DateTime.Now);
            XmlLogger.Log.SetSessionAttribute("three", 555);
            try
            {
                throw new Exception("Outer error message.", new Exception("Internal error message."));
            }
            catch (Exception ex)
            {
                XmlLogger.Log.LogException(ex);
            }

            string log = XmlLogger.Log.GetLogContent();
            var xml = XDocument.Parse(log);
            xml.Validate(_schemas, (o, e) => Assert.Fail(e.Message), true);
        }

        [TestMethod]
        public void LogErrorTest()
        {
            XmlLogger.Log.Error("Example error new line.");
            string log = XmlLogger.Log.GetLogContent();
            XElement xml = XElement.Parse(log);
            var errorNodes = xml.Descendants("error");
            Assert.IsTrue(errorNodes.Any());
        }

        [TestMethod]
        public void LogHeterogeneousContentTest()
        {
            XmlLogger.Log.Information("Information message");
            XmlLogger.Log.Warning("Warning message");
            XmlLogger.Log.Error("Error message");
            XmlLogger.Log.LogException(new DivideByZeroException("Sample zero division."));
            try
            {
                throw new Exception("Outer error message.", new Exception("Internal error message."));
            }
            catch (Exception ex)
            {
                XmlLogger.Log.LogException(ex);
            }
            string content = XmlLogger.Log.GetLogContent();
        }
    }
}
