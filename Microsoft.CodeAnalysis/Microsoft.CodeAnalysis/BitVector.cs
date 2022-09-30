using System;
using System.Collections.Generic;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct BitVector : IEquatable<BitVector>
    {
        private const ulong ZeroWord = 0uL;

        private const int Log2BitsPerWord = 6;

        public const int BitsPerWord = 64;

        private static readonly BitVector s_nullValue = default(BitVector);

        private static readonly BitVector s_emptyValue = new BitVector(0uL, s_emptyArray, 0);

        private ulong _bits0;

        private ulong[] _bits;

        private int _capacity;

        private static ulong[] s_emptyArray => new ulong[0];

        public int Capacity => _capacity;

        public bool IsNull => _bits == null;

        public static BitVector Null => s_nullValue;

        public static BitVector Empty => s_emptyValue;

        public bool this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                if (index >= _capacity)
                {
                    return false;
                }
                int num = (index >> 6) - 1;
                return IsTrue((num < 0) ? _bits0 : _bits[num], index);
            }
            set
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                if (index >= _capacity)
                {
                    EnsureCapacity(index + 1);
                }
                int num = (index >> 6) - 1;
                int num2 = index & 0x3F;
                ulong num3 = (ulong)(1L << num2);
                if (num < 0)
                {
                    if (value)
                    {
                        _bits0 |= num3;
                    }
                    else
                    {
                        _bits0 &= ~num3;
                    }
                }
                else if (value)
                {
                    _bits[num] |= num3;
                }
                else
                {
                    _bits[num] &= ~num3;
                }
            }
        }

        private BitVector(ulong bits0, ulong[] bits, int capacity)
        {
            WordsForCapacity(capacity);
            _bits0 = bits0;
            _bits = bits;
            _capacity = capacity;
        }

        public bool Equals(BitVector other)
        {
            if (_capacity == other._capacity && _bits0 == other._bits0)
            {
                return MemoryExtensions.AsSpan(_bits).SequenceEqual(MemoryExtensions.AsSpan(other._bits));
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is BitVector other)
            {
                return Equals(other);
            }
            return false;
        }

        public static bool operator ==(BitVector left, BitVector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BitVector left, BitVector right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            int currentKey = _bits0.GetHashCode();
            if (_bits != null)
            {
                for (int i = 0; i < _bits.Length; i++)
                {
                    currentKey = Hash.Combine(_bits[i].GetHashCode(), currentKey);
                }
            }
            return Hash.Combine(_capacity, currentKey);
        }

        private static int WordsForCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                return 0;
            }
            return capacity - 1 >> 6;
        }

        [Conditional("DEBUG_BITARRAY")]
        private void Check()
        {
        }

        public void EnsureCapacity(int newCapacity)
        {
            if (newCapacity > _capacity)
            {
                int num = WordsForCapacity(newCapacity);
                if (num > _bits.Length)
                {
                    Array.Resize(ref _bits, num);
                }
                _capacity = newCapacity;
            }
        }

        public IEnumerable<ulong> Words()
        {
            if (_capacity > 0)
            {
                yield return _bits0;
            }
            int i = 0;
            ulong[] bits = _bits;
            for (int j = ((bits != null) ? bits.Length : 0); i < j; i++)
            {
                yield return _bits[i];
            }
        }

        public IEnumerable<int> TrueBits()
        {
            if (_bits0 != 0L)
            {
                for (int bit2 = 0; bit2 < 64; bit2++)
                {
                    ulong num = (ulong)(1L << bit2);
                    if ((_bits0 & num) != 0L)
                    {
                        if (bit2 >= _capacity)
                        {
                            yield break;
                        }
                        yield return bit2;
                    }
                }
            }
            for (int bit2 = 0; bit2 < _bits.Length; bit2++)
            {
                ulong w = _bits[bit2];
                if (w == 0L)
                {
                    continue;
                }
                for (int b = 0; b < 64; b++)
                {
                    ulong num2 = (ulong)(1L << b);
                    if ((w & num2) != 0L)
                    {
                        int num3 = (bit2 + 1 << 6) | b;
                        if (num3 >= _capacity)
                        {
                            yield break;
                        }
                        yield return num3;
                    }
                }
            }
        }

        public static BitVector Create(int capacity)
        {
            int num = WordsForCapacity(capacity);
            ulong[] bits = ((num == 0) ? s_emptyArray : new ulong[num]);
            return new BitVector(0uL, bits, capacity);
        }

        public static BitVector AllSet(int capacity)
        {
            if (capacity == 0)
            {
                return Empty;
            }
            int num = WordsForCapacity(capacity);
            ulong[] array = ((num == 0) ? s_emptyArray : new ulong[num]);
            int num2 = num - 1;
            ulong bits = ulong.MaxValue;
            for (int i = 0; i < num2; i++)
            {
                array[i] = ulong.MaxValue;
            }
            int num3 = capacity & 0x3F;
            if (num3 > 0)
            {
                ulong num4 = (ulong)(~(-1L << num3));
                if (num2 < 0)
                {
                    bits = num4;
                }
                else
                {
                    array[num2] = num4;
                }
            }
            else if (num > 0)
            {
                array[num2] = ulong.MaxValue;
            }
            return new BitVector(bits, array, capacity);
        }

        public BitVector Clone()
        {
            return new BitVector(bits: (_bits != null && _bits.Length != 0) ? ((ulong[])_bits.Clone()) : s_emptyArray, bits0: _bits0, capacity: _capacity);
        }

        public void Invert()
        {
            _bits0 = ~_bits0;
            if (_bits != null)
            {
                for (int i = 0; i < _bits.Length; i++)
                {
                    _bits[i] = ~_bits[i];
                }
            }
        }

        public bool IntersectWith(in BitVector other)
        {
            bool result = false;
            int num = other._bits.Length;
            ulong[] bits = _bits;
            int num2 = bits.Length;
            if (num > num2)
            {
                num = num2;
            }
            ulong bits2 = _bits0;
            ulong num3 = bits2 & other._bits0;
            if (num3 != bits2)
            {
                _bits0 = num3;
                result = true;
            }
            for (int i = 0; i < num; i++)
            {
                ulong num4 = bits[i];
                ulong num5 = num4 & other._bits[i];
                if (num5 != num4)
                {
                    bits[i] = num5;
                    result = true;
                }
            }
            for (int j = num; j < num2; j++)
            {
                if (bits[j] != 0L)
                {
                    bits[j] = 0uL;
                    result = true;
                }
            }
            return result;
        }

        public bool UnionWith(in BitVector other)
        {
            bool result = false;
            if (other._capacity > _capacity)
            {
                EnsureCapacity(other._capacity);
            }
            ulong bits = _bits0;
            _bits0 |= other._bits0;
            if (bits != _bits0)
            {
                result = true;
            }
            for (int i = 0; i < other._bits.Length; i++)
            {
                bits = _bits[i];
                _bits[i] |= other._bits[i];
                if (_bits[i] != bits)
                {
                    result = true;
                }
            }
            return result;
        }

        public void Clear()
        {
            _bits0 = 0uL;
            if (_bits != null)
            {
                Array.Clear(_bits, 0, _bits.Length);
            }
        }

        public static bool IsTrue(ulong word, int index)
        {
            int num = index & 0x3F;
            ulong num2 = (ulong)(1L << num);
            return (word & num2) != 0;
        }

        public static int WordsRequired(int capacity)
        {
            if (capacity <= 0)
            {
                return 0;
            }
            return WordsForCapacity(capacity) + 1;
        }

        internal string GetDebuggerDisplay()
        {
            char[] array = new char[_capacity];
            for (int i = 0; i < _capacity; i++)
            {
                array[_capacity - i - 1] = (this[i] ? '1' : '0');
            }
            return new string(array);
        }
    }
}
