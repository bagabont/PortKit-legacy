using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace Portkit.Logging.EventMonitor
{
    internal static class ListEx
    {
        public static ReadOnlyCollection<T> AsReadOnly<T>(this List<T> list)
        {
            Contract.Ensures(Contract.Result<ReadOnlyCollection<T>>() != null);
            return new ReadOnlyCollection<T>(list);
        }
    }
}