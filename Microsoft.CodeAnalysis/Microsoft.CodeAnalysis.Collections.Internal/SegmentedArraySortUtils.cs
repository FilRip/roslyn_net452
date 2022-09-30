using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal static class SegmentedArraySortUtils
    {
        private static ReadOnlySpan<byte> Log2DeBruijn => new byte[32]
        {
            0, 9, 1, 10, 13, 21, 2, 29, 11, 14,
            16, 18, 22, 25, 3, 30, 8, 12, 20, 28,
            15, 17, 24, 7, 19, 27, 23, 6, 26, 5,
            4, 31
        };

        public static int MoveNansToFront<TKey, TValue>(SegmentedArraySegment<TKey> keys, Span<TValue> values) where TKey : notnull
        {
            int num = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                if ((typeof(TKey) == typeof(double) && double.IsNaN((double)(object)keys[i])) || (typeof(TKey) == typeof(float) && float.IsNaN((float)(object)keys[i])) || (typeof(TKey) == typeof(float) && float.IsNaN((float)(object)keys[i])))
                {
                    TKey val = keys[num];
                    keys[num] = keys[i];
                    keys[i] = val;
                    if ((uint)i < (uint)values.Length)
                    {
                        TValue val2 = values[num];
                        values[num] = values[i];
                        values[i] = val2;
                    }
                    num++;
                }
            }
            return num;
        }

        public static int Log2(uint value)
        {
            return Log2SoftwareFallback(value);
        }

        private static int Log2SoftwareFallback(uint value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(Log2DeBruijn), (IntPtr)(int)(value * 130329821 >> 27));
        }
    }
}
