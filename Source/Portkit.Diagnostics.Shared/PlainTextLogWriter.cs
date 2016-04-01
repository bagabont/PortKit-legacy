using System;
using System.Diagnostics;
using System.Text;

namespace Portkit.Diagnostics
{
    public class PlainTextLogWriter : ILogWriter
    {
        private readonly StringBuilder _log;

        public LogLevel Level { get; set; }

        public PlainTextLogWriter(LogLevel level)
        {
            Level = level;
            _log = new StringBuilder();
        }

        public void Write(string message, LogLevel level)
        {
            Debug.WriteLine($"{level}: {message}");

            // Restrict log level
            if (level > Level)
            {
                return;
            }
            var time = DateTime.Now.ToString("O");
            _log.AppendLine($"{time} {level}: {message}");
        }

        public string GetLog()
        {
            return _log.ToString();
        }
    }
}