using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace Portkit.Net.Cache
{
    internal class HttpCachedResponse : IDisposable
    {
        private readonly Stream _stream;
        private readonly int _contentSize;
        private readonly long _contentOffset;
        private readonly BinaryReader _binaryReader;

        public string RequestUrl { get; set; }

        public string Etag { get; set; }

        public string LastModified { get; set; }

        public HttpCachedResponse(Stream stream)
        {
            _stream = stream;
            _stream.Seek(0, SeekOrigin.Begin);
            _binaryReader = new BinaryReader(stream);

            RequestUrl = _binaryReader.ReadString();
            Etag = _binaryReader.ReadString();
            LastModified = _binaryReader.ReadString();
            _contentSize = _binaryReader.ReadInt32();
            _contentOffset = _binaryReader.ReadInt64();
        }

        public async Task<byte[]> ReadContentAsync()
        {
            if (_contentSize == 0)
            {
                return null;
            }
            byte[] buffer = new byte[_contentSize];
            int offset = ((int)_contentOffset);
            _stream.Seek(offset, SeekOrigin.Begin);
            await _stream.ReadAsync(buffer, 0, buffer.Length);
            return buffer;
        }

        public static async Task<HttpCachedResponse> LoadResponseAsync(StorageFile file)
        {
            var stream = await file.OpenStreamForReadAsync();
            return new HttpCachedResponse(stream);
        }

        public static async Task CacheResponseAsync(HttpResponseMessage response, StorageFile file)
        {
            string requestUrl = response.RequestMessage.RequestUri.OriginalString;
            string etag = response.Headers.ETag != null ? response.Headers.ETag.ToString() : " ";
            string lastModified = response.Content.Headers.LastModified.HasValue ?
                response.Content.Headers.LastModified.Value.ToString("o") : " ";

            var responseContent = await response.Content.ReadAsByteArrayAsync();
            var stream = await file.OpenStreamForWriteAsync();
            using (var bw = new BinaryWriter(stream))
            {
                bw.Write(requestUrl);
                bw.Write(etag);
                bw.Write(lastModified);
                bw.Write(responseContent.Length);
                bw.Write(bw.BaseStream.Position + sizeof(long));
                bw.Write(responseContent);
            }
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
                _binaryReader?.Dispose();
            }
        }
        #endregion

    }
}
