using System;
using System.Text;

namespace Portkit.Core.Cryptography
{
    /// <summary>
    /// Represents basic SHA-1 class that provides hash computation. 
    /// </summary>
    public class Sha1
    {
        private const int MESSAGE_BLOCK_SIZE = 64;
        private readonly uint[] _hashBuffer;
        private ulong _count;
        private readonly byte[] _messageBuffer;
        private int _messageCount;
        private readonly uint[] _buffer;

        /// <summary>
        /// Creates a new instance of the Sha1 class and initializes default buffers and values.
        /// </summary>
        public Sha1()
        {
            _hashBuffer = new uint[5];
            _messageBuffer = new byte[MESSAGE_BLOCK_SIZE];
            _buffer = new uint[80];
            Reset();
        }

        /// <summary>
        /// Computes the SHA-1 hash string of the provided buffer from the beginning to the end.
        /// </summary>
        /// <param name="buffer">The byte array for which the SHA-1 will be computed.</param>
        /// <returns>SHA-1 hash of the byte array.</returns>
        public string ComputeHashString(byte[] buffer)
        {
            return ComputeHashString(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Computes a hash string.
        /// </summary>
        public string ComputeHashString(byte[] buffer, int count)
        {
            return ComputeHashString(buffer, 0, count);
        }

        /// <summary>
        /// Computes a hash string.
        /// </summary>
        public string ComputeHashString(byte[] buffer, int offset, int count)
        {
            byte[] data = ComputeHash(buffer, offset, count);
            return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Computes a hash byte array.
        /// </summary>
        public byte[] ComputeHash(byte[] buffer)
        {
            return ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Computes a hash byte array.
        /// </summary>
        public byte[] ComputeHash(byte[] buffer, int count)
        {
            return ComputeHash(buffer, 0, count);
        }

        /// <summary>
        /// Computes the SHA-1 hash.
        /// </summary>
        /// <param name="buffer">Source buffer.</param>
        /// <param name="offset">Bytes offset.</param>
        /// <param name="count">The number of bytes to hash.</param>
        /// <returns>SHA-1 byte array hash.</returns>
        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            int i;

            if (_messageCount != 0)
            {
                if (count < (MESSAGE_BLOCK_SIZE - _messageCount))
                {
                    Buffer.BlockCopy(buffer, offset, _messageBuffer, _messageCount, count);
                    _messageCount += count;
                    return GetSha1ByteArray();
                }
                i = (MESSAGE_BLOCK_SIZE - _messageCount);
                Buffer.BlockCopy(buffer, offset, _messageBuffer, _messageCount, i);
                ProcessMessage(_messageBuffer, 0);
                _messageCount = 0;
                offset += i;
                count -= i;
            }

            for (i = 0; i < count - count % MESSAGE_BLOCK_SIZE; i += MESSAGE_BLOCK_SIZE)
            {
                ProcessMessage(buffer, (uint)(offset + i));
            }

            if (count % MESSAGE_BLOCK_SIZE != 0)
            {
                Buffer.BlockCopy(buffer, count - count % MESSAGE_BLOCK_SIZE + offset, _messageBuffer, 0, count % MESSAGE_BLOCK_SIZE);
                _messageCount = count % MESSAGE_BLOCK_SIZE;
            }
            return GetSha1ByteArray();
        }

        /// <summary>
        /// Resets the SHA-1 generation hash buffer.
        /// </summary>
        public void Reset()
        {
            _count = 0;
            _messageCount = 0;
            _hashBuffer[0] = 0x67452301;
            _hashBuffer[1] = 0xefcdab89;
            _hashBuffer[2] = 0x98badcfe;
            _hashBuffer[3] = 0x10325476;
            _hashBuffer[4] = 0xC3D2E1F0;
        }

        private byte[] GetSha1ByteArray()
        {
            var result = new byte[20];
            DigestMessage(_messageBuffer, 0, _messageCount);
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    result[i * 4 + j] = (byte)(_hashBuffer[i] >> (8 * (3 - j)));
                }
            }
            return result;
        }

        private void ProcessMessage(byte[] buffer, uint offset)
        {
            uint a, b, c, d, e;
            _count += MESSAGE_BLOCK_SIZE;
            PopulateBuffer(_buffer, buffer, offset);

            a = _hashBuffer[0];
            b = _hashBuffer[1];
            c = _hashBuffer[2];
            d = _hashBuffer[3];
            e = _hashBuffer[4];

            int i = 0;
            while (i < 20)
            {
                e += ((a << 5) | (a >> 27)) + (((c ^ d) & b) ^ d) + 0x5A827999 + _buffer[i];
                b = (b << 30) | (b >> 2);

                d += ((e << 5) | (e >> 27)) + (((b ^ c) & a) ^ c) + 0x5A827999 + _buffer[i + 1];
                a = (a << 30) | (a >> 2);

                c += ((d << 5) | (d >> 27)) + (((a ^ b) & e) ^ b) + 0x5A827999 + _buffer[i + 2];
                e = (e << 30) | (e >> 2);

                b += ((c << 5) | (c >> 27)) + (((e ^ a) & d) ^ a) + 0x5A827999 + _buffer[i + 3];
                d = (d << 30) | (d >> 2);

                a += ((b << 5) | (b >> 27)) + (((d ^ e) & c) ^ e) + 0x5A827999 + _buffer[i + 4];
                c = (c << 30) | (c >> 2);
                i += 5;
            }

            while (i < 40)
            {
                e += ((a << 5) | (a >> 27)) + (b ^ c ^ d) + 0x6ED9EBA1 + _buffer[i];
                b = (b << 30) | (b >> 2);

                d += ((e << 5) | (e >> 27)) + (a ^ b ^ c) + 0x6ED9EBA1 + _buffer[i + 1];
                a = (a << 30) | (a >> 2);

                c += ((d << 5) | (d >> 27)) + (e ^ a ^ b) + 0x6ED9EBA1 + _buffer[i + 2];
                e = (e << 30) | (e >> 2);

                b += ((c << 5) | (c >> 27)) + (d ^ e ^ a) + 0x6ED9EBA1 + _buffer[i + 3];
                d = (d << 30) | (d >> 2);

                a += ((b << 5) | (b >> 27)) + (c ^ d ^ e) + 0x6ED9EBA1 + _buffer[i + 4];
                c = (c << 30) | (c >> 2);
                i += 5;
            }

            while (i < 60)
            {
                e += ((a << 5) | (a >> 27)) + ((b & c) | (b & d) | (c & d)) + 0x8F1BBCDC + _buffer[i];
                b = (b << 30) | (b >> 2);

                d += ((e << 5) | (e >> 27)) + ((a & b) | (a & c) | (b & c)) + 0x8F1BBCDC + _buffer[i + 1];
                a = (a << 30) | (a >> 2);

                c += ((d << 5) | (d >> 27)) + ((e & a) | (e & b) | (a & b)) + 0x8F1BBCDC + _buffer[i + 2];
                e = (e << 30) | (e >> 2);

                b += ((c << 5) | (c >> 27)) + ((d & e) | (d & a) | (e & a)) + 0x8F1BBCDC + _buffer[i + 3];
                d = (d << 30) | (d >> 2);

                a += ((b << 5) | (b >> 27)) + ((c & d) | (c & e) | (d & e)) + 0x8F1BBCDC + _buffer[i + 4];
                c = (c << 30) | (c >> 2);
                i += 5;
            }

            while (i < 80)
            {
                e += ((a << 5) | (a >> 27)) + (b ^ c ^ d) + 0xCA62C1D6 + _buffer[i];
                b = (b << 30) | (b >> 2);

                d += ((e << 5) | (e >> 27)) + (a ^ b ^ c) + 0xCA62C1D6 + _buffer[i + 1];
                a = (a << 30) | (a >> 2);

                c += ((d << 5) | (d >> 27)) + (e ^ a ^ b) + 0xCA62C1D6 + _buffer[i + 2];
                e = (e << 30) | (e >> 2);

                b += ((c << 5) | (c >> 27)) + (d ^ e ^ a) + 0xCA62C1D6 + _buffer[i + 3];
                d = (d << 30) | (d >> 2);

                a += ((b << 5) | (b >> 27)) + (c ^ d ^ e) + 0xCA62C1D6 + _buffer[i + 4];
                c = (c << 30) | (c >> 2);
                i += 5;
            }

            _hashBuffer[0] += a;
            _hashBuffer[1] += b;
            _hashBuffer[2] += c;
            _hashBuffer[3] += d;
            _hashBuffer[4] += e;
        }

        private void DigestMessage(byte[] message, int offset, int count)
        {
            ulong total = _count + (ulong)count;
            int paddingSize = (56 - (int)(total % MESSAGE_BLOCK_SIZE));

            if (paddingSize < 1)
                paddingSize += MESSAGE_BLOCK_SIZE;

            int length = count + paddingSize + 8;
            byte[] buff = (length == 64) ? _messageBuffer : new byte[length];

            for (int i = 0; i < count; i++)
            {
                buff[i] = message[i + offset];
            }

            buff[count] = 0x80;
            for (int i = count + 1; i < count + paddingSize; i++)
            {
                buff[i] = 0x00;
            }
            ulong size = total << 3;
            PadMessageLength(size, buff, count + paddingSize);
            ProcessMessage(buff, 0);

            if (length == 128)
            {
                ProcessMessage(buff, 64);
            }
        }

        private static void PopulateBuffer(uint[] buff, byte[] input, uint inputOffset)
        {
            buff[0] = (uint)((input[inputOffset + 0] << 24) | (input[inputOffset + 1] << 16) | (input[inputOffset + 2] << 8) | (input[inputOffset + 3]));
            buff[1] = (uint)((input[inputOffset + 4] << 24) | (input[inputOffset + 5] << 16) | (input[inputOffset + 6] << 8) | (input[inputOffset + 7]));
            buff[2] = (uint)((input[inputOffset + 8] << 24) | (input[inputOffset + 9] << 16) | (input[inputOffset + 10] << 8) | (input[inputOffset + 11]));
            buff[3] = (uint)((input[inputOffset + 12] << 24) | (input[inputOffset + 13] << 16) | (input[inputOffset + 14] << 8) | (input[inputOffset + 15]));
            buff[4] = (uint)((input[inputOffset + 16] << 24) | (input[inputOffset + 17] << 16) | (input[inputOffset + 18] << 8) | (input[inputOffset + 19]));
            buff[5] = (uint)((input[inputOffset + 20] << 24) | (input[inputOffset + 21] << 16) | (input[inputOffset + 22] << 8) | (input[inputOffset + 23]));
            buff[6] = (uint)((input[inputOffset + 24] << 24) | (input[inputOffset + 25] << 16) | (input[inputOffset + 26] << 8) | (input[inputOffset + 27]));
            buff[7] = (uint)((input[inputOffset + 28] << 24) | (input[inputOffset + 29] << 16) | (input[inputOffset + 30] << 8) | (input[inputOffset + 31]));
            buff[8] = (uint)((input[inputOffset + 32] << 24) | (input[inputOffset + 33] << 16) | (input[inputOffset + 34] << 8) | (input[inputOffset + 35]));
            buff[9] = (uint)((input[inputOffset + 36] << 24) | (input[inputOffset + 37] << 16) | (input[inputOffset + 38] << 8) | (input[inputOffset + 39]));
            buff[10] = (uint)((input[inputOffset + 40] << 24) | (input[inputOffset + 41] << 16) | (input[inputOffset + 42] << 8) | (input[inputOffset + 43]));
            buff[11] = (uint)((input[inputOffset + 44] << 24) | (input[inputOffset + 45] << 16) | (input[inputOffset + 46] << 8) | (input[inputOffset + 47]));
            buff[12] = (uint)((input[inputOffset + 48] << 24) | (input[inputOffset + 49] << 16) | (input[inputOffset + 50] << 8) | (input[inputOffset + 51]));
            buff[13] = (uint)((input[inputOffset + 52] << 24) | (input[inputOffset + 53] << 16) | (input[inputOffset + 54] << 8) | (input[inputOffset + 55]));
            buff[14] = (uint)((input[inputOffset + 56] << 24) | (input[inputOffset + 57] << 16) | (input[inputOffset + 58] << 8) | (input[inputOffset + 59]));
            buff[15] = (uint)((input[inputOffset + 60] << 24) | (input[inputOffset + 61] << 16) | (input[inputOffset + 62] << 8) | (input[inputOffset + 63]));

            uint val;
            int i = 16;
            while (i < 80)
            {
                val = buff[i - 3] ^ buff[i - 8] ^ buff[i - 14] ^ buff[i - 16];
                buff[i] = (val << 1) | (val >> 31);

                val = buff[i - 2] ^ buff[i - 7] ^ buff[i - 13] ^ buff[i - 15];
                buff[i + 1] = (val << 1) | (val >> 31);

                val = buff[i - 1] ^ buff[i - 6] ^ buff[i - 12] ^ buff[i - 14];
                buff[i + 2] = (val << 1) | (val >> 31);

                val = buff[i + 0] ^ buff[i - 5] ^ buff[i - 11] ^ buff[i - 13];
                buff[i + 3] = (val << 1) | (val >> 31);

                val = buff[i + 1] ^ buff[i - 4] ^ buff[i - 10] ^ buff[i - 12];
                buff[i + 4] = (val << 1) | (val >> 31);

                val = buff[i + 2] ^ buff[i - 3] ^ buff[i - 9] ^ buff[i - 11];
                buff[i + 5] = (val << 1) | (val >> 31);

                val = buff[i + 3] ^ buff[i - 2] ^ buff[i - 8] ^ buff[i - 10];
                buff[i + 6] = (val << 1) | (val >> 31);

                val = buff[i + 4] ^ buff[i - 1] ^ buff[i - 7] ^ buff[i - 9];
                buff[i + 7] = (val << 1) | (val >> 31);
                i += 8;
            }
        }

        private static void PadMessageLength(ulong length, byte[] buffer, int position)
        {
            buffer[position++] = (byte)(length >> 56);
            buffer[position++] = (byte)(length >> 48);
            buffer[position++] = (byte)(length >> 40);
            buffer[position++] = (byte)(length >> 32);
            buffer[position++] = (byte)(length >> 24);
            buffer[position++] = (byte)(length >> 16);
            buffer[position++] = (byte)(length >> 8);
            buffer[position] = (byte)(length);
        }
    }
}