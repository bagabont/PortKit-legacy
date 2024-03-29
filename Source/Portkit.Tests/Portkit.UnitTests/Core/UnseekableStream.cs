﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Portkit.UnitTests.Core
{
    [ExcludeFromCodeCoverage]
    internal sealed class UnseekableStream : MemoryStream
    {
        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new Exception("Stream cannot be seeked.");
        }

        public override long Position
        {
            get { return base.Position; }
            set
            {
                throw new Exception();
            }
        }
    }
}
