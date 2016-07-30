using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Portkit.Time
{
    /// <summary>
    /// Represents a class, that provides accurate, system independent and network synchronized <see cref="DateTime"/>.
    /// </summary>
    public static class SynchronizedDateTime
    {
        /// <summary>
        /// Gets or sets the timeout per client. If a <see cref="ITimeSyncClient"/> 
        /// fails to acquire  accurate network time,  for this duration, then 
        /// the synchronization will fallback to the next available client. 
        /// </summary>
        public static TimeSpan TimeoutPerClient { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets the list of available synchronization clients. 
        /// Important: The position of each client determines the order in 
        /// which it will be used on an attempt to acquire network time. 
        /// If the attempt fails, the synchronization will fallback 
        /// to the client at the next position in this list.
        /// </summary>
        public static List<ITimeSyncClient> SynchronizationClients { get; } = new List<ITimeSyncClient>
        {
            new NtpTimeSyncClient(),
            new HttpTimeSyncClient()
        };

        /// <summary>
        /// Gets the difference between the network time and the system time.
        /// </summary>
        public static TimeSpan CorrectionOffset { get; private set; }

        /// <summary>
        /// Gets the current <see cref="DateTime"/> with the added correction offset.
        /// </summary>
        public static DateTime Now => DateTime.Now.Add(CorrectionOffset);

        /// <summary>
        /// Gets the current UTC <see cref="DateTime"/> with the added correction offset.
        /// </summary>
        public static DateTime UtcNow => DateTime.Now.Add(CorrectionOffset).ToUniversalTime();

        /// <summary>
        /// Attempts to synchronize the correction offset with a network acquired time. 
        /// If the synchronization fails, the method will fallback to the next available
        /// client in <see cref="SynchronizationClients"/> list.
        /// </summary>
        public static async Task SynchronizeAsync()
        {
            foreach (var timeSyncClient in SynchronizationClients)
            {
                try
                {
                    var accurateUtcTime = await timeSyncClient.GetNetworkUtcTimeAsync(TimeoutPerClient);
                    CorrectionOffset = accurateUtcTime.ToLocalTime() - DateTime.Now;
                    Debug.WriteLine($"Network time synchronized. Correction offset: {CorrectionOffset}");
                    return; // If synchronization succeeds, break the loop.
                }
                catch
                {
                    // If an error is caught, attempt to 
                    // synchronize time via the next available client
                    Debug.WriteLine("Synchronization failed. Using next fallback synchronization client.");
                }
            }
        }

        /// <summary>
        /// Resets the correction offset to 0.
        /// </summary>
        public static void ResetCorrectionOffset()
        {
            CorrectionOffset = TimeSpan.Zero;
        }
    }
}
