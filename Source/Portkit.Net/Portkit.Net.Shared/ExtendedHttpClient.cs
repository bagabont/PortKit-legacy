using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Portkit.Net.Cache;
using Portkit.Net.Shared;

namespace Portkit.Net
{
    public class ExtendedHttpClient : HttpClient
    {
        private readonly ICache<string, HttpResponseMessage> _cache;

        public ExtendedHttpClient(ICache<string, HttpResponseMessage> cache)
        {
            _cache = cache;
        }

        public ExtendedHttpClient() :
            this(new HttpCache(ApplicationData.Current.LocalCacheFolder))
        {
        }

        public IObservable<HttpResponseMessage> GetCacheFirst(string requestUrl)
        {
            return GetCacheFirst(new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl)));
        }

        public IObservable<HttpResponseMessage> GetCacheFirst(Uri requestUri)
        {
            return GetCacheFirst(new HttpRequestMessage(HttpMethod.Get, requestUri));
        }

        public IObservable<HttpResponseMessage> GetCacheFirst(HttpRequestMessage request)
        {
            return Observable.Create<HttpResponseMessage>(observer => CreateRequestAsync(observer, request));
        }

        private async Task CreateRequestAsync(IObserver<HttpResponseMessage> observer, HttpRequestMessage request)
        {
            try
            {
                var cacheKey = KeyUtil.ComputeHash(request.RequestUri.OriginalString);
                var cachedResponse = await _cache.GetAsync(cacheKey);
                if (cachedResponse != null)
                {
                    request.Headers.IfModifiedSince = cachedResponse.Headers.Date.GetValueOrDefault();
                    request.Headers.IfNoneMatch.Add(cachedResponse.Headers.ETag);
                    observer.OnNext(cachedResponse);
                }
                var serverResponse = await SendAsync(request);

                // Cache the response
                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    await Task.Run(() => _cache.PutAsync(cacheKey, serverResponse));
                }

                if (serverResponse.StatusCode != HttpStatusCode.NotModified)
                {
                    observer.OnNext(serverResponse);
                    observer.OnCompleted();
                }
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                observer.OnCompleted();
            }
            finally
            {
                request?.Dispose();
            }
        }
    }
}