﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portkit.Core.Caching;

namespace Portkit.UnitTests.Core
{
    [TestClass, ExcludeFromCodeCoverage]
    public class PortableCacheTests
    {
        [TestMethod]
        public void CacheInitializationTest()
        {
            var cache = new PortableCache();
            Assert.IsNotNull(cache);
        }

        [TestMethod]
        public void ConcurrencyAddTest()
        {
            var array = new byte[] { 1, 2, 3 };
            var cache = new PortableCache();
            const int count = 10000;
            bool hasThrown = false;
            var task = new Task[2];
            task[0] = Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        var ms = new MemoryStream(100);
                        ms.Write(array, 0, 3);
                        cache.Add(i.ToString(CultureInfo.InvariantCulture), ms);
                    }
                }
                catch (Exception)
                {
                    hasThrown = true;
                }
            });
            task[1] = Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        var ms = new MemoryStream(200);
                        ms.Write(array, 0, 3);
                        cache.Add(i.ToString(CultureInfo.InvariantCulture), ms);
                    }
                }
                catch (Exception)
                {
                    hasThrown = true;
                }
            });
            Task.WaitAll(task);
            Assert.IsFalse(hasThrown, "Concurrency issue has occurred when adding item to cache.");
        }

        [TestMethod]
        public void ConcurrencyRemoveTest()
        {
            bool hasThrown = false;
            var array = new byte[] { 1, 2, 3 };
            var cache = new PortableCache();
            const int count = 10000;
            //Fill cache
            for (int i = 0; i < count; i++)
            {
                var ms = new MemoryStream(100);
                ms.Write(array, 0, 3);
                cache.Add(i.ToString(CultureInfo.InvariantCulture), ms);
            }
            Assert.IsTrue(cache.Count == count);
            var task = new Task[2];
            task[0] = Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        cache.Remove(i.ToString(CultureInfo.InvariantCulture));
                    }
                }
                catch (Exception)
                {
                    hasThrown = true;
                }
            });
            task[1] = Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        cache.Remove(i.ToString(CultureInfo.InvariantCulture));
                    }
                }
                catch (Exception)
                {
                    hasThrown = true;
                }
            });
            Task.WaitAll(task);
            Assert.IsFalse(hasThrown, "Concurrency issue has occurred when removing item from cache.");
        }

        [TestMethod]
        public void ConcurrencyClearTest()
        {
            bool hasThrown = false;
            var array = new byte[] { 1, 2, 3 };
            var cache = new PortableCache();
            const int count = 10000;
            //Fill cache
            for (int i = 0; i < count; i++)
            {
                var ms = new MemoryStream(100);
                ms.Write(array, 0, 3);
                cache.Add(i.ToString(), ms);
            }
            Assert.IsTrue(cache.Count == count);
            var task = new Task[2];
            task[0] = Task.Run(() =>
            {
                try
                {
                    cache.Clear();
                }
                catch (Exception)
                {
                    hasThrown = true;
                }
            });
            task[1] = Task.Run(() =>
            {
                try
                {
                    cache.Clear();
                }
                catch (Exception)
                {
                    hasThrown = true;
                }
            });
            Task.WaitAll(task);
            Assert.IsFalse(hasThrown, "Concurrency issue has occurred when clearing item from cache.");
        }

        [TestMethod]
        public void AddNewStreamObjectTest()
        {
            var array = new byte[] { 1, 2, 3 };
            var cache = new PortableCache();
            const string key = "stream";
            var ms = new MemoryStream(100);
            ms.Write(array, 0, 3);
            cache.Add(key, ms);
            Assert.IsTrue(cache.Count == 1);
            byte[] actual = cache.Get<string, MemoryStream>(key).ToArray();
            Assert.IsTrue(actual.Length == array.Length && actual[0] == array[0] && actual[2] == array[2]);
        }

        [TestMethod]
        public void ReplaceExistingItemOnAddTest()
        {
            var array = new byte[] { 1, 2, 3 };
            const string key = "stream";
            var cache = new PortableCache();
            cache.Add(key, array);
            cache.Add(key, array);

            Assert.IsTrue(cache.Count == 1);
        }

        [TestMethod]
        public void RemoveExistingObjectReturnsTrueTest()
        {
            const string key = "stream";
            var array = new byte[] { 1, 2, 3 };
            var cache = new PortableCache();
            cache.Add(key, array);
            Assert.IsTrue(cache.Remove(key));
            Assert.IsFalse(cache.Contains(key) && cache.Count > 0, "Object not removed");
        }

        [TestMethod]
        public void RemoveNonExistingObjectReturnsFalseTest()
        {
            var array = new byte[] { 1, 2, 3 };
            const string key = "stream";
            var cache = new PortableCache();
            cache.Add(key, array);
            Assert.IsFalse(cache.Remove("SOMETHING"));
            Assert.IsTrue(cache.Contains(key));
        }

        [TestMethod]
        public void ContainsObjectReturnsTrueTest()
        {
            var cache = new PortableCache();
            cache.Add("person", new object());
            Assert.IsTrue(cache.Contains("person"));
        }

        [TestMethod]
        public void ContainsObjectReturnsFalseTest()
        {
            var cache = new PortableCache();
            //Check on empty cache
            Assert.IsFalse(cache.Contains("person"));
            cache.Add("person", new object());
            //Check if cache is not empty
            Assert.IsFalse(cache.Contains("animal"));
        }

        [TestMethod]
        public void ClearCacheTest()
        {
            var cache = new PortableCache();
            cache.Add(1, new object());
            cache.Add(2, new object());
            cache.Add(3, new object());

            Assert.IsTrue(cache.Count == 3, "Cache is not filled correctly.");
            int clearedItems = cache.Clear();
            Assert.IsTrue(clearedItems == 3);
            Assert.IsTrue(cache.Count == 0, "Cache was not cleared. Items count: {0}", cache.Count);
        }

        [TestMethod]
        public void ClearFiresCacheItemRemovedEveryTimeTest()
        {
            int cacheItemRemovedCount = 0;
            var cache = new PortableCache();

            // Wire item removed event.
            cache.CacheItemRemoved += (s, e) =>
            {
                cacheItemRemovedCount++;
            };

            cache.Add(1, new object());
            cache.Add(2, new object());
            cache.Add(3, new object());

            var addedItemsCount = cache.Count;
            var removedItemsCount = cache.Clear();

            Assert.IsTrue(cacheItemRemovedCount == addedItemsCount);
            Assert.IsTrue(removedItemsCount == addedItemsCount);
            Assert.IsTrue(cache.Count == 0);
        }

        [TestMethod]
        public void GetObjectItemTest()
        {
            var array = new byte[] { 1, 2, 3 };
            const string key = "stream";
            var cache = new PortableCache();
            cache.Add(key, array);
            byte[] actual = cache.Get<string, byte[]>(key);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Length == array.Length && actual[0] == array[0] && actual[2] == array[2]);
        }

        [TestMethod]
        public void GetNonExistingObjectReturnsNullTest()
        {
            var cache = new PortableCache();
            object actual = cache.Get<string, object>(Guid.NewGuid().ToString());
            Assert.IsNull(actual);
            var person = new object();
            cache.Add(Guid.NewGuid().ToString(), person);
            actual = cache.Get<string, object>(Guid.NewGuid().ToString());
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void ItemRemovedFiresEventTests()
        {
            var array = new byte[] { 1, 2, 3 };
            const string key = "stream";
            var waitHandler = new AutoResetEvent(false);
            var cache = new PortableCache();
            cache.Add(key, array);
            CacheItem removedItem = null;
            cache.CacheItemRemoved += (s, e) =>
            {
                removedItem = e.Value;
                waitHandler.Set();
            };
            cache.Remove(key);
            waitHandler.WaitOne();
            Assert.IsNotNull(removedItem);
        }

        [TestMethod]
        public void ItemAddedFiresEventTests()
        {
            var array = new byte[] { 1, 2, 3 };
            const string key = "stream";
            var waitHandler = new AutoResetEvent(false);
            var cache = new PortableCache();
            CacheItem addedItem = null;
            cache.CacheItemAdded += (s, e) =>
            {
                addedItem = e.Value;
                waitHandler.Set();
            };
            cache.Add(key, array);

            waitHandler.WaitOne();
            Assert.IsNotNull(addedItem);
        }

        [TestMethod]
        public void PurgeNormalPrioritiesTest()
        {
            var cache = new PortableCache();

            const string normalPriorityKey = "1";
            const string highPriorityKey = "100";

            cache.Add(normalPriorityKey, new object(), CacheItemPriority.Normal);
            cache.Add(highPriorityKey, new object(), CacheItemPriority.High);

            Assert.IsTrue(cache.Count == 2);

            // Clean items with normal priorities.
            cache.PurgeNormalPriorities();

            Assert.IsTrue(cache.Count == 1);
            Assert.IsTrue(cache.Contains(highPriorityKey));
            Assert.IsFalse(cache.Contains(normalPriorityKey));
        }

        [TestMethod]
        public async Task AbsoluteExpirationTest()
        {
            const int key = 1;
            var cache = new PortableCache();
            const int expirationPeriod = 100;

            var offset = DateTimeOffset.UtcNow.AddMilliseconds(expirationPeriod);
            var expPolicy = new CacheItemPolicy(CacheItemPriority.Normal, offset, CacheItemPolicy.NoSliding);

            // Add item with expiration policy.
            cache.Add(key, new object(), expPolicy);

            Assert.IsNotNull(await cache.GetWithDelay<int, object>(key, expirationPeriod / 3));
            Assert.IsNotNull(await cache.GetWithDelay<int, object>(key, expirationPeriod / 3));

            // Item must be expired already.
            Assert.IsNull(await cache.GetWithDelay<int, object>(key, expirationPeriod / 3));
        }

        [TestMethod]
        public async Task SlidingExpirationTest()
        {
            const int key = 1;
            var cache = new PortableCache();

            // Set absolute offset, to test if it gets overridden. 
            var offset = DateTimeOffset.UtcNow.AddMilliseconds(500);

            // Set slider expiration.
            var slider = TimeSpan.FromMilliseconds(200);

            var expPolicy = new CacheItemPolicy(CacheItemPriority.Normal, offset, slider);

            // Add item with expiration policy.
            cache.Add(key, new object(), expPolicy);
            Assert.IsTrue(cache.Count > 0, "Item is not added.");

            // Simulate cache activity.
            Assert.IsNotNull(await cache.GetWithDelay<int, object>(key, 50));
            Assert.IsNotNull(await cache.GetWithDelay<int, object>(key, 50));
            Assert.IsNotNull(await cache.GetWithDelay<int, object>(key, 50));
            Assert.IsNotNull(await cache.GetWithDelay<int, object>(key, 50));
            Assert.IsNotNull(await cache.GetWithDelay<int, object>(key, 50));

            // Let slider expire.
            Assert.IsNull(await cache.GetWithDelay<int, object>(key, (int)slider.TotalMilliseconds));
        }

        [TestMethod]
        public async Task PurgeExpiredItemsAutomaticallyTest()
        {
            const int key = 1;
            const int expirationPeriod = 50;
            var cache = new PortableCache();

            var offset = DateTimeOffset.UtcNow.AddMilliseconds(expirationPeriod);
            var expPolicy = new CacheItemPolicy(CacheItemPriority.Normal, offset, CacheItemPolicy.NoSliding);

            // Add item with expiration policy.
            cache.Add(key, new object(), expPolicy);

            // Check if purging affects item that has not expired yet.
            var purgedItems = cache.PurgeExpiredItems();
            Assert.IsTrue(purgedItems == 0);

            // Let item expire and get removed.
            await Task.Delay(expirationPeriod * 2);

            // Try to manually purge item.
            purgedItems = cache.PurgeExpiredItems();
            Assert.IsTrue(purgedItems == -1, "Purged items count: {0}", purgedItems);

            // Check if cache is empty.
            Assert.IsTrue(cache.Count == 0);
        }

        [TestMethod]
        public async Task PurgeExpiredItemsManuallyTest()
        {
            const int key = 1;
            const int expirationPeriod = 50;
            var cache = new PortableCache();

            var offset = DateTimeOffset.UtcNow.AddMilliseconds(expirationPeriod);
            var expPolicy = new CacheItemPolicy(CacheItemPriority.Normal, offset, CacheItemPolicy.NoSliding);

            // Import item with expiration policy. Do not use add, since it will trigger the auto purge.
            cache.Import(new CacheItem(key, new object(), expPolicy));

            // Check if purging affects item that has not expired yet.
            var purgedItems = cache.PurgeExpiredItems();
            Assert.IsTrue(purgedItems == 0);

            // Let item expire.
            await Task.Delay(expirationPeriod * 2);
            purgedItems = cache.PurgeExpiredItems();

            // Check if item was purged and the cache is empty.
            Assert.IsTrue(purgedItems == 1, "Purged items count: {0}", purgedItems);
            Assert.IsTrue(cache.Count == 0);
        }
    }
}