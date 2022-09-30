using System;
using System.Runtime.InteropServices;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct TypeLayout : IEquatable<TypeLayout>
    {
        private readonly byte _kind;

        private readonly short _alignment;

        private readonly int _size;

        public LayoutKind Kind
        {
            get
            {
                if (_kind != 0)
                {
                    return (LayoutKind)(_kind - 1);
                }
                return LayoutKind.Auto;
            }
        }

        public short Alignment => _alignment;

        public int Size => _size;

        public TypeLayout(LayoutKind kind, int size, byte alignment)
        {
            _kind = (byte)(kind + 1);
            _size = size;
            _alignment = alignment;
        }

        public bool Equals(TypeLayout other)
        {
            if (_size == other._size && _alignment == other._alignment)
            {
                return _kind == other._kind;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TypeLayout)
            {
                return Equals((TypeLayout)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Hash.Combine(Size, Alignment), _kind);
        }
    }
}
