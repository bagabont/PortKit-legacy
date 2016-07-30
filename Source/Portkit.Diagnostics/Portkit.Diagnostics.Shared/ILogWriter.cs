namespace Portkit.Diagnostics
{
    public interface ILogWriter
    {
        void Write(string message, LogLevel level);
    }
}