using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Portkit.Core.Caching
{
    /// <summary>
    /// Represents a storage class that allows caching items.
    /// </summary>
    public class PortableCache
    {
        #region Fields

        private readonly IDictionary<object, CacheItem> _cache;

        private static readonly object _syncLock = new Object();

        private const int MONITOR_WAIT_DEFAULT = 1000;
        private const int MONITOR_WAIT_TO_UPDATE_SLIDING = 500;

        private Timer _purgeExpiredTimer;

        #endregion

        #region Events

        /// <summary>
        /// Event that occurs when a new item is added to the cache.
        /// </summary>
        public event EventHandler<DataEventArgs<CacheItem>> CacheItemAdded;

        /// <summary>
        /// Fires the <see cref="CacheItemAdded"/> event.
        /// </summary>
        /// <param name="item">Cache item that is added.</param>
        protected virtual void OnCacheItemAdded(CacheItem item)
        {
            var handler = CacheItemAdded;
            if (handler != null)
            {
                handler(this, new DataEventArgs<CacheItem>(item));
            }
        }

        /// <summary>
        /// Event that occurs when an item is removed from the cache.
        /// </summary>
        public event EventHandler<DataEventArgs<CacheItem>> CacheItemRemoved;

        /// <summary>
        /// Fires the <see cref="CacheItemRemoved"/> event.
        /// </summary>
        /// <param name="item">Cache item that was removed.</param>
        protected virtual void OnCacheItemRemoved(CacheItem item)
        {
            var handler = CacheItemRemoved;
            if (handler != null)
            {
                handler(this, new DataEventArgs<CacheItem>(item));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of items stored in the cache.
        /// </summary>
        public int Count
        {
            get
            {
                if (!Monitor.TryEnter(_syncLock, MONITOR_WAIT_DEFAULT))
                {
                    return -1;
                }

                try
                {
                    return _cache.Count;
                }
                finally
                {
                    Monitor.Exit(_syncLock);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="PortableCache"/> class.
        /// </summary>
        public PortableCache()
        {
            _cache = new Dictionary<object, CacheItem>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <typeparam name="TKey">Item's key type.</typeparam>
        /// <typeparam name="TValue">Item's value type.</typeparam>
        /// <param name="key">Item's key.</param>
        /// <param name="value">Item's value.</param>
        public void Add<TKey, TValue>(TKey key, TValue value) where TValue : class
        {
            Add(key, value, CacheItemPolicy.Default);
        }

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <typeparam name="TKey">Item's key type.</typeparam>
        /// <typeparam name="TValue">Item's value type.</typeparam>
        /// <param name="key">Item's key.</param>
        /// <param name="value">Item's value.</param>
        /// <param name="priority">Item's caching priority.</param>
        public void Add<TKey, TValue>(TKey key, TValue value, CacheItemPriority priority) where TValue : class
        {
            Add(key, value, new CacheItemPolicy(priority, CacheItemPolicy.InfiniteOffset, CacheItemPolicy.NoSliding));
        }

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <typeparam name="TKey">Item's key type.</typeparam>
        /// <typeparam name="TValue">Item's value type.</typeparam>
        /// <param name="key">Item's key.</param>
        /// <param name="value">Item's value.</param>
        /// <param name="policy">Item's caching policy.</param>
        public void Add<TKey, TValue>(TKey key, TValue value, CacheItemPolicy policy) where TValue : class
        {
            if (!Monitor.TryEnter(_syncLock, MONITOR_WAIT_DEFAULT))
            {
                return;
            }
            try
            {
                if (policy == null)
                {
                    policy = CacheItemPolicy.Default;
                }
                var item = new CacheItem(key, value, policy);
                var cachedItem = GetItem(key);
                if (cachedItem == null)
                {
                    _cache.Add(key, item);
                }
                else
                {
                    _cache[key] = item;
                }
                if (policy.SlidingExpiration != CacheItemPolicy.NoSliding ||
                    policy.UtcExpirationOffset != CacheItemPolicy.InfiniteOffset)
                {
                    var offset = _cache.Values.Min(i => i.AbsoluteUtcExpiration) - DateTime.UtcNow;
                    if (offset > TimeSpan.Zero && offset < TimeSpan.MaxValue)
                    {
                        if (_purgeExpiredTimer == null)
                        {
                            _purgeExpiredTimer = new Timer(_ => PurgeExpiredItems(), null, offset, offset);
                        }
                        else
                        {
                            _purgeExpiredTimer.Change(offset, offset);
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(_syncLock);
            }
            OnCacheItemAdded(_cache[key]);
        }

        /// <summary>
        /// Gets all keys of a give type from the cache.
        /// </summary>
        /// <typeparam name="TKey">Item's key type.</typeparam>
        /// <returns>All keys, of a given type, stored in the cache.</returns>
        public IEnumerable<TKey> Keys<TKey>()
        {
            if (!Monitor.TryEnter(_syncLock, MONITOR_WAIT_DEFAULT))
            {
                return Enumerable.Empty<TKey>();
            }

            try
            {
                return _cache.Keys.Where(k => k.GetType() == typeof(TKey)).Cast<TKey>().ToList();
            }
            finally
            {
                Monitor.Exit(_syncLock);
            }
        }

        /// <summary>
        /// Gets all keys from the cache.
        /// </summary>
        /// <returns>All keys stored in the cache.</returns>
        public IEnumerable<object> Keys()
        {
            if (!Monitor.TryEnter(_syncLock, MONITOR_WAIT_DEFAULT))
            {
                return Enumerable.Empty<object>();
            }
            try
            {
                return _cache.Keys.ToList();
            }
            finally
            {
                Monitor.Exit(_syncLock);
            }
        }

        /// <summary>
        /// Checks if the cache contains an item.
        /// </summary>
        /// <param name="key">Item's key.</param>
        /// <returns>True if the item is in the cache, otherwise false.</returns>
        public bool Contains(string key)
        {
            var result = GetItem(key);
            return result != null && result.Value != null;
        }

        /// <summary>
        /// Gets item's value from the cache.
        /// </summary>
        /// <typeparam name="TKey">Item's key type.</typeparam>
        /// <typeparam name="TValue">Item's value type.</typeparam>
        /// <param name="key">Item's key.</param>
        /// <returns>Value of the cache item.</returns>
        public TValue Get<TKey, TValue>(TKey key) where TValue : class
        {
            try
            {
                var item = GetItem(key);
                if (item == null)
                {
                    return null;
                }
                if (item.SlidingExpiration != CacheItemPolicy.NoSliding)
                {
                    if (Monitor.TryEnter(_syncLock, MONITOR_WAIT_TO_UPDATE_SLIDING))
                    {
                        try
                        {
                            item.UpdateExpirationPolicy(DateTime.UtcNow);
                        }
                        finally
                        {
                            Monitor.Exit(_syncLock);
                        }
                    }
                }
                return (TValue)item.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Imports an item into the cache, without firing the <see cref="CacheItemAdded"/> event.
        /// </summary>
        /// <param name="item">Item to be imported.</param>
        public void Import(CacheItem item)
        {
            lock (_syncLock)
            {
                CacheItem cachedItem;
                if (_cache.TryGetValue(item.Key, out cachedItem))
                {
                    _cache[item.Key] = item;
                }
                else
                {
                    _cache.Add(item.Key, item);
                }
            }
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <typeparam name="TKey">Item's key type.</typeparam>
        /// <param name="key">Key under which the item is stored.</param>
        /// <returns>True if item was removed, otherwise false.</returns>
        public bool Remove<TKey>(TKey key)
        {
            CacheItem cachedItem;
            lock (_syncLock)
            {
                if (_cache.TryGetValue(key, out cachedItem))
                {
                    _cache.Remove(key);
                }
            }
            if (cachedItem != null)
            {
                OnCacheItemRemoved(cachedItem);
            }
            return cachedItem != null;
        }

        /// <summary>
        /// Manually removes all expired items from the cache.
        /// </summary>
        /// <returns>Number of expired items that have been removed.</returns>
        public int PurgeExpiredItems()
        {
            if (!Monitor.TryEnter(_syncLock, MONITOR_WAIT_DEFAULT))
            {
                return -1;
            }
            try
            {
                if (_cache.Count > 0)
                {
                    return _cache.Reverse()
                       .Where(i => i.Value != null && i.Value.HasExpired)
                       .Select(kv => Remove(kv.Key))
                       .Count();
                }
                // If cache is empty, dispose purge timer.
                if (_purgeExpiredTimer != null)
                {
                    _purgeExpiredTimer.Dispose();
                }
                return -1;
            }
            finally
            {
                Monitor.Exit(_syncLock);
            }
        }

        /// <summary>
        /// Removes all items with normal priority from the cache.
        /// </summary>
        /// <returns></returns>
        public int PurgeNormalPriorities()
        {
            if (Monitor.TryEnter(_syncLock, MONITOR_WAIT_DEFAULT))
            {
                try
                {
                    var keysToRemove = (from cacheItem in _cache
                                        where cacheItem.Value.Priority == CacheItemPriority.Normal
                                        select cacheItem.Key).ToList();

                    // Remove items.
                    return keysToRemove.Count(Remove);
                }
                finally
                {
                    Monitor.Exit(_syncLock);
                }
            }
            return -1;
        }

        /// <summary>
        /// Clears the cache. <see cref="CacheItemRemoved"/> event will be fired for each one of the removed items.
        /// </summary>
        /// <returns>Number of items that have been removed.</returns>
        public int Clear()
        {
            if (!Monitor.TryEnter(_syncLock, MONITOR_WAIT_DEFAULT))
            {
                return -1;
            }
            try
            {
                return _cache.Reverse().Select(kv => Remove(kv.Key)).Count();
            }
            finally
            {
                Monitor.Exit(_syncLock);
            }
        }

        /// <summary>
        /// When overridden imports all cache items and purges the expired.
        /// </summary>
        public virtual void Load()
        {
            PurgeExpiredItems();
        }

        private CacheItem GetItem<TKey>(TKey key)
        {
            if (Equals(key, null))
            {
                throw new ArgumentNullException("key");
            }
            lock (_syncLock)
            {
                CacheItem item;
                if (_cache.TryGetValue(key, out item) && !item.HasExpired)
                {
                    return item;
                }
            }
            Remove(key);
            return null;
        }

        #endregion
    }
}
