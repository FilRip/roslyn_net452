using System;
using System.Buffers.Binary;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    internal ref struct LittleEndianReader
    {
        private ReadOnlySpan<byte> _span;

        public LittleEndianReader(ReadOnlySpan<byte> span)
        {
            _span = span;
        }

        internal uint ReadUInt32()
        {
            uint result = BinaryPrimitives.ReadUInt32LittleEndian(_span);
            _span = _span.Slice(4);
            return result;
        }

        internal byte ReadByte()
        {
            byte result = _span[0];
            _span = _span.Slice(1);
            return result;
        }

        internal ushort ReadUInt16()
        {
            ushort result = BinaryPrimitives.ReadUInt16LittleEndian(_span);
            _span = _span.Slice(2);
            return result;
        }

        internal ReadOnlySpan<byte> ReadBytes(int byteCount)
        {
            ReadOnlySpan<byte> result = _span.Slice(0, byteCount);
            _span = _span.Slice(byteCount);
            return result;
        }

        internal int ReadInt32()
        {
            int result = BinaryPrimitives.ReadInt32LittleEndian(_span);
            _span = _span.Slice(4);
            return result;
        }

        internal byte[] ReadReversed(int byteCount)
        {
            byte[] array = ReadBytes(byteCount).ToArray();
            array.ReverseContents();
            return array;
        }
    }
}
