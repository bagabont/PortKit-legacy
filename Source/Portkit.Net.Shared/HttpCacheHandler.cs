using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Portkit.Cache;

namespace Portkit.Net
{
    public sealed class HttpCacheHandler : HttpClientHandler
    {
        private readonly ICache<string, HttpResponseMessage> _cache;

        public HttpCacheHandler(ICache<string, HttpResponseMessage> cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            _cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            bool isCacheDisabled = request.Method != HttpMethod.Get ||
                (request.Headers.CacheControl != null &&
                request.Headers.CacheControl.NoCache);

            if (isCacheDisabled)
            {
                return await base.SendAsync(request, ct);
            }
            var cacheKey = request.RequestUri.OriginalString.GetSha1();
            var cachedResponse = await ConfigureHttpRequest(request, cacheKey);
            var serverResponse = await base.SendAsync(request, ct);

            switch (serverResponse.StatusCode)
            {
                case HttpStatusCode.NotModified:
                    return await HandleNotModifiedResponse(request, ct, cachedResponse, serverResponse);

                case HttpStatusCode.OK:
                    return await HandleOkResponse(cachedResponse, serverResponse, cacheKey);

                default:
                    return serverResponse;
            }
        }

        private async Task<HttpResponseMessage> ConfigureHttpRequest(HttpRequestMessage request, string cacheKey)
        {
            // Load cached response and set web request headers
            var cachedResponse = await _cache.GetAsync(cacheKey);
            if (cachedResponse != null)
            {
                request.Headers.IfModifiedSince = cachedResponse.Headers.Date.GetValueOrDefault();
                request.Headers.IfNoneMatch.Add(cachedResponse.Headers.ETag);
            }
            return cachedResponse;
        }

        private async Task<HttpResponseMessage> HandleOkResponse(HttpResponseMessage cachedResponse, HttpResponseMessage serverResponse, string cacheKey)
        {
            bool notCached = cachedResponse == null;

            // Check basic caching allowance.
            bool canCacheResponse = notCached &&
                                    (serverResponse.Headers.CacheControl == null ||
                                     !serverResponse.Headers.CacheControl.NoCache);
            if (canCacheResponse)
            {
                await _cache.PutAsync(cacheKey, serverResponse);
            }
            return serverResponse;
        }

        private async Task<HttpResponseMessage> HandleNotModifiedResponse(HttpRequestMessage request,
            CancellationToken ct, HttpResponseMessage cachedResponse, HttpResponseMessage serverResponse)
        {
            // Read content from cache.
            if (cachedResponse == null)
            {
                return await base.SendAsync(request, ct);
            }
            var content = cachedResponse.Content;
            if (content == null)
            {
                return await base.SendAsync(request, ct);
            }
            serverResponse.Content = content;

            // Change response status code, since
            // it already has content.
            serverResponse.StatusCode = HttpStatusCode.OK;
            serverResponse.ReasonPhrase = "OK";
            return serverResponse;
        }
    }
}