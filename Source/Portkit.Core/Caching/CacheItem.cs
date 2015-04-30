using System;

namespace Portkit.Core.Caching
{
    /// <summary>
    /// Represents a cache item, that can be stored in <see cref="PortableMemoryCache"/> class instance.
    /// </summary>
    public class CacheItem
    {
        #region Properties

        /// <summary>
        /// Gets the key under which the item is stored.
        /// </summary>
        public object Key { get; protected set; }

        /// <summary>
        /// Gets the value object that is stored.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the UTC time when the item was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets the UTC time when the item expires.
        /// </summary>
        public DateTime AbsoluteUtcExpiration { get; protected set; }

        /// <summary>
        /// Gets the time period for which an item must be accessed or otherwise it will expired.
        /// </summary>
        public TimeSpan SlidingExpiration { get; protected set; }

        /// <summary>
        /// Gets the item storage priority.
        /// </summary>
        public CacheItemPriority Priority { get; protected set; }

        /// <summary>
        /// Compares the <see cref="AbsoluteUtcExpiration"/> with the 
        /// current UTC time and checks if the item has expired.
        /// </summary>
        public bool HasExpired
        {
            get { return AbsoluteUtcExpiration < DateTime.UtcNow; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of the <see cref="CacheItem"/> class.
        /// </summary>
        /// <param name="key">Cache item key.</param>
        /// <param name="value">Cache item value.</param>
        public CacheItem(string key, object value)
            : this(key, value, CacheItemPolicy.Default)
        {
        }

        /// <summary>
        /// Creates new instance of the <see cref="CacheItem"/> class.
        /// </summary>
        /// <param name="key">Cache item key.</param>
        /// <param name="value">Cache item value.</param>
        /// <param name="policy">Item's caching policy.</param>
        public CacheItem(object key, object value, CacheItemPolicy policy)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Created = DateTime.UtcNow;
            Key = key;
            Value = value;
            Priority = policy.Priority;
            SlidingExpiration = policy.SlidingExpiration;

            if (SlidingExpiration == CacheItemPolicy.NoSliding)
            {
                AbsoluteUtcExpiration = policy.UtcExpirationOffset.UtcDateTime;
            }
            else
            {
                AbsoluteUtcExpiration = Created + SlidingExpiration;
            }
        }

        #endregion

        #region Methods

        internal void UpdateExpirationPolicy(DateTime utcNow)
        {
            if (SlidingExpiration == CacheItemPolicy.NoSliding)
            {
                return;
            }
            var expireDate = utcNow + SlidingExpiration;
            if (expireDate - AbsoluteUtcExpiration >= CacheItemPolicy.MinSlidingUpdateDelta || expireDate < AbsoluteUtcExpiration)
            {
                AbsoluteUtcExpiration = expireDate;
            }
        }

        #endregion
    }
}