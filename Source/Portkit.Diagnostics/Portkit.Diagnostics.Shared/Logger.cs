namespace Portkit.Diagnostics
{
    public class Logger : ILogger
    {
        private readonly ILogWriter _writer;

        public Logger(ILogWriter writer)
        {
            _writer = writer;
        }

        public void Debug(string message) => _writer.Write(message, LogLevel.Verbose);

        public void Info(string message) => _writer.Write(message, LogLevel.Information);

        public void Warn(string message) => _writer.Write(message, LogLevel.Warning);

        public void Error(string message) => _writer.Write(message, LogLevel.Error);

        public void Critical(string message) => _writer.Write(message, LogLevel.Critical);

        public string GetLog() => _writer.GetLog();
    }
}