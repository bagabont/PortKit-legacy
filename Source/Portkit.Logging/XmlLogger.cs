using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Portkit.Logging
{
    public class XmlLogger
    {
        private static readonly object SyncRoot = new object();
        private static XmlLogger _log;
        private readonly XElement _session;

        public List<IRemoteErrorTracker> Trackers { get; private set; }

        public static XmlLogger Log
        {
            get
            {
                if (_log == null)
                {
                    lock (SyncRoot)
                    {
                        if (_log == null)
                        {
                            _log = new XmlLogger();
                        }
                    }
                }
                return _log;
            }
        }

        private XmlLogger()
        {
            _session = new XElement("session");
            _session.SetAttributeValue("timestamp", DateTime.Now.ToString("O"));
            Trackers = new List<IRemoteErrorTracker>();
        }

        public void SetSessionAttribute(string name, object value)
        {
            _session.SetAttributeValue(name, value);
        }

        public void Info(string message, [CallerMemberName]string caller = null)
        {
            const string type = "information";
            XElement logEntry = CreateLogEntry(type, caller);
            logEntry.Add(new XElement(type, message));

            _session.Add(logEntry);
        }

        public void Warning(string message, [CallerMemberName]string caller = null)
        {
            const string type = "warning";
            XElement logEntry = CreateLogEntry(type, caller);
            logEntry.Add(new XElement(type, message));

            _session.Add(logEntry);
        }

        public void Error(string message, [CallerMemberName]string caller = null)
        {
            const string type = "error";
            XElement logEntry = CreateLogEntry(type, caller);
            logEntry.Add(new XElement(type, message));

            _session.Add(logEntry);
        }

        public async Task ReportExceptionAsync(Exception exception,
            [CallerMemberName]string caller = null,
            [CallerFilePath]string file = null,
            [CallerLineNumber]int lineNumber = 0)
        {
            if (!Trackers.Any())
            {
                throw new InvalidOperationException("No remote error trackers found. " +
                                                    "Please, either configure at least one error tracker " +
                                                    "or use LogException method instead.");
            }

            LogException(exception, caller, file, lineNumber);
            foreach (var errorTracker in Trackers)
            {
                await errorTracker.ReportAsync(exception, caller, file, lineNumber);
            }
        }

        public void LogException(Exception exception,
            [CallerMemberName]string caller = null,
            [CallerFilePath]string file = null,
            [CallerLineNumber]int lineNumber = 0)
        {
            const string type = "exception";
            XElement logEntry = CreateLogEntry(type, caller);

            Exception error = exception;
            while (error != null)
            {
                XElement exceptionNode = new XElement("exception");
                exceptionNode.SetAttributeValue("type", error.GetType());
                exceptionNode.SetAttributeValue("file", file);
                exceptionNode.SetAttributeValue("line", lineNumber);
                exceptionNode.Add(new XElement("message", error.Message));
                exceptionNode.Add(new XElement("stacktrace", error.StackTrace));

                // Add exception to log entry
                logEntry.Add(exceptionNode);

                // Set inner exception
                error = error.InnerException;
            }

            _session.Add(logEntry);
        }

        private XElement CreateLogEntry(string type, string caller)
        {
            XElement logEntry = new XElement("log");
            logEntry.SetAttributeValue("timestamp", DateTime.Now.ToString("O"));
            logEntry.SetAttributeValue("caller", caller);
            logEntry.SetAttributeValue("type", type);
            return logEntry;
        }

        public string GetLogContent()
        {
            return _session.ToString();
        }
    }
}
