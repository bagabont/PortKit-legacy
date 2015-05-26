using System;
using System.Threading.Tasks;

namespace Portkit.Logging
{
    public interface IRemoteErrorTracker
    {
        Task ReportAsync(Exception exception, string caller, string file, int lineNumber);
    }
}
