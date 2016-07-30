using System;
using System.Threading.Tasks;

namespace Portkit.Diagnostics
{
    /// <summary>
    /// Provides opportunity to send errors to an exception tracking service.
    /// </summary>
    public interface IExceptionTracker
    {
        /// <summary>
        /// Send exception
        /// </summary>
        /// <param name="exception">Error to be sent.</param>
        Task ReportAsync(Exception exception);
    }
}