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

        public List<IRemoteErrorTracker> RemoteTrackers { get; private set; }

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
            RemoteTrackers = new List<IRemoteErrorTracker>();
        }

        public void SetSessionAttribute(string name, object value)
        {
            _session.SetAttributeValue(name, value);
        }

        public void Information(string message, [CallerMemberName]string caller = null)
        {
            XElement logEntry = CreateLogEntry("information", caller);
            logEntry.SetValue(message);

            _session.Add(logEntry);
        }

        public void Warning(string message, [CallerMemberName]string caller = null)
        {
            XElement logEntry = CreateLogEntry("warning", caller);
            logEntry.SetValue(message);

            _session.Add(logEntry);
        }

        public void Error(string message, [CallerMemberName]string caller = null)
        {
            XElement logEntry = CreateLogEntry("error", caller);
            logEntry.SetValue(message);

            _session.Add(logEntry);
        }

        public async Task ReportExceptionAsync(Exception exception,
            [CallerMemberName]string caller = null,
            [CallerFilePath]string file = null,
            [CallerLineNumber]int lineNumber = 0)
        {
            if (!RemoteTrackers.Any())
            {
                throw new InvalidOperationException("No remote error trackers found. " +
                                                    "Please, either configure at least one error tracker " +
                                                    "or use LogException method instead.");
            }

            LogException(exception, caller, file, lineNumber);
            foreach (var errorTracker in RemoteTrackers)
            {
                await errorTracker.ReportAsync(exception, caller, file, lineNumber);
            }
        }

        public void LogException(Exception exception,
            [CallerMemberName]string caller = null,
            [CallerFilePath]string file = null,
            [CallerLineNumber]int lineNumber = 0)
        {
            XElement logEntry = CreateLogEntry("exception", caller);
            logEntry.Add(new XElement("file", file));
            logEntry.Add(new XElement("line", lineNumber));

            Exception error = exception;
            XElement lastErrorXml = logEntry;

            while (error != null)
            {
                XElement exceptionNode = new XElement("exception");
                exceptionNode.Add(new XElement("type", error.GetType()));
                exceptionNode.Add(new XElement("message", error.Message));
                exceptionNode.Add(new XElement("stacktrace", error.StackTrace));

                lastErrorXml.Add(exceptionNode);
                lastErrorXml = exceptionNode;

                // Set inner exception
                error = error.InnerException;
            }

            _session.Add(logEntry);
        }

        private static XElement CreateLogEntry(string level, string caller)
        {
            XElement logEntry = new XElement("log");
            logEntry.SetAttributeValue("level", level);
            logEntry.SetAttributeValue("caller", caller);
            logEntry.SetAttributeValue("timestamp", DateTime.Now.ToString("O"));

            return logEntry;
        }

        public string GetLogContent()
        {
            return _session.ToString();
        }
    }
}
