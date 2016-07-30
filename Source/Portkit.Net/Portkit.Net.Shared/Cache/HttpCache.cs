using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace Portkit.Net.Cache
{
    public class HttpCache : ICache<string, HttpResponseMessage>, IDisposable
    {
        private readonly StorageFolder _cacheFolder;
        private static readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1);

        /// <summary>
        /// Creates a new instance of the <see cref="HttpCache"/> class.
        /// </summary>
        /// <param name="cacheFolder">Storage folder for cache items.</param>
        public HttpCache(StorageFolder cacheFolder)
        {
            _cacheFolder = cacheFolder;
        }

        public async Task<HttpResponseMessage> GetAsync(string key)
        {
            await _asyncLock.WaitAsync();

            try
            {
                using (var cachedResponse = await InternalGetAsync(key))
                {
                    if (cachedResponse == null)
                    {
                        return null;
                    }
                    var response = new HttpResponseMessage(HttpStatusCode.OK);

                    DateTimeOffset date;
                    if (DateTimeOffset.TryParse(cachedResponse.LastModified, out date))
                    {
                        response.Headers.Date = date;
                    }
                    response.Headers.ETag = EntityTagHeaderValue.Parse(cachedResponse.Etag);
                    response.Content = new ByteArrayContent(await cachedResponse.ReadContentAsync());
                    return response;
                }
            }
            catch
            {
                // Ignore errors and return a null response
                return null;
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        public async Task PutAsync(string key, HttpResponseMessage value)
        {
            await _asyncLock.WaitAsync();

            try
            {
                var file = await _cacheFolder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
                await HttpCachedResponse.CacheResponseAsync(value, file);
            }
            catch
            {
                // Ignore
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        private async Task<HttpCachedResponse> InternalGetAsync(string key)
        {
            try
            {
                var file = await _cacheFolder.GetFileAsync(key);
                if (!await CheckIsEmptyAsync(file))
                {
                    return await HttpCachedResponse.LoadResponseAsync(file);
                }
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (IOException)
            {
                // Ignore FileNotFoundException since the IO API 
                // does not provide other way to check if the file exists.
            }
            catch
            {
                //Ignore
            }
            return null;
        }

        public async Task PruneAsync(TimeSpan age)
        {
            try
            {
                var files = await _cacheFolder.GetFilesAsync();
                foreach (var file in files)
                {
                    await CheckFileAge(file, age);
                }
            }
            catch
            {
                // Ignore
            }
        }

        private static async Task CheckFileAge(IStorageItem file, TimeSpan age)
        {
            try
            {
                var fileAge = DateTime.UtcNow - file.DateCreated.ToUniversalTime();
                if (fileAge > age || await CheckIsEmptyAsync(file))
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        private static async Task<bool> CheckIsEmptyAsync(IStorageItem item)
        {
            var props = await item.GetBasicPropertiesAsync();
            if (props == null)
            {
                throw new IOException("Failed to retrieve basic item properties.");
            }
            return props.Size <= 0;
        }


        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _asyncLock?.Dispose();
            }
        }

        #endregion
    }
}
