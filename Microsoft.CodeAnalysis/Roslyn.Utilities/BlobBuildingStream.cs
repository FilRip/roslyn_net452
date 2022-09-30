using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Roslyn.Utilities
{
    internal sealed class BlobBuildingStream : Stream
    {
        private static readonly ObjectPool<BlobBuildingStream> s_pool = new ObjectPool<BlobBuildingStream>(() => new BlobBuildingStream());

        private readonly BlobBuilder _builder;

        public const int ChunkSize = 32768;

        public override bool CanWrite => true;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override long Length => _builder.Count;

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public static BlobBuildingStream GetInstance()
        {
            return s_pool.Allocate();
        }

        private BlobBuildingStream()
        {
            _builder = new BlobBuilder(32768);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _builder.WriteBytes(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            _builder.WriteByte(value);
        }

        public void WriteInt32(int value)
        {
            _builder.WriteInt32(value);
        }

        public Blob ReserveBytes(int byteCount)
        {
            return _builder.ReserveBytes(byteCount);
        }

        public ImmutableArray<byte> ToImmutableArray()
        {
            return _builder.ToImmutableArray();
        }

        public void Free()
        {
            _builder.Clear();
            s_pool.Free(this);
        }

        public override void Flush()
        {
        }

        protected override void Dispose(bool disposing)
        {
            Free();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
