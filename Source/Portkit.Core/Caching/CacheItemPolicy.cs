using System;

namespace Portkit.Core.Caching
{
    /// <summary>
    /// Represents a caching policy under which items are being stored in the cache.
    /// </summary>
    public sealed class CacheItemPolicy
    {
        /// <summary>
        /// Represents the minimum sliding update time delta period.
        /// </summary>
        public static readonly TimeSpan MinSlidingUpdateDelta = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// A constant used to specify an infinite expiration offset.
        /// </summary>
        public static readonly DateTimeOffset InfiniteOffset = DateTimeOffset.MaxValue;

        /// <summary>
        /// A constant used to specify a no sliding expiration period.
        /// </summary>
        public static readonly TimeSpan NoSliding = TimeSpan.Zero;

        private static readonly object SyncLock = new Object();

        private static volatile CacheItemPolicy _default;

        /// <summary>
        /// Gets the cache policy value.
        /// </summary>
        public static CacheItemPolicy Default
        {
            get
            {
                if (_default == null)
                {
                    lock (SyncLock)
                    {
                        if (_default == null)
                        {
                            _default = new CacheItemPolicy(CacheItemPriority.Normal, InfiniteOffset, NoSliding);
                        }
                    }
                    return _default;
                }
                return _default;
            }
        }

        /// <summary>
        /// Gets or sets the cache policy priority.
        /// </summary>
        public CacheItemPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the absolute UTC expiration time.
        /// </summary>
        public DateTimeOffset UtcExpirationOffset { get; set; }

        /// <summary>
        /// Gets or sets the time period for sliding expiration.
        /// </summary>
        public TimeSpan SlidingExpiration { get; set; }

        /// <summary>
        /// Creates new instance of the <see cref="CacheItemPolicy"/> class.
        /// </summary>
        /// <param name="priority">Storage priority.</param>
        /// <param name="utcExpirationOffset">Expiration offset in UTC time.</param>
        /// <param name="slidingExpiration">Sliding expiration period.</param>
        public CacheItemPolicy(CacheItemPriority priority, DateTimeOffset utcExpirationOffset, TimeSpan slidingExpiration)
        {
            Priority = priority;
            UtcExpirationOffset = utcExpirationOffset;
            SlidingExpiration = slidingExpiration;
        }
    }
}
