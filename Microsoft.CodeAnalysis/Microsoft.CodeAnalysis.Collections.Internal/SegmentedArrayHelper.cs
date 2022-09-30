using System;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal static class SegmentedArrayHelper
    {
        internal static class TestAccessor
        {
            public static int CalculateSegmentSize(int elementSize)
            {
                return SegmentedArrayHelper.CalculateSegmentSize(elementSize);
            }

            public static int CalculateSegmentShift(int elementSize)
            {
                return SegmentedArrayHelper.CalculateSegmentShift(elementSize);
            }

            public static int CalculateOffsetMask(int elementSize)
            {
                return SegmentedArrayHelper.CalculateOffsetMask(elementSize);
            }
        }

        private static class ReferenceTypeSegmentHelper
        {
            public static readonly int SegmentSize = CalculateSegmentSize(Unsafe.SizeOf<object>());

            public static readonly int SegmentShift = CalculateSegmentShift(SegmentSize);

            public static readonly int OffsetMask = CalculateOffsetMask(SegmentSize);
        }

        private static class ValueTypeSegmentHelper<T>
        {
            public static readonly int SegmentSize = CalculateSegmentSize(Unsafe.SizeOf<T>());

            public static readonly int SegmentShift = CalculateSegmentShift(SegmentSize);

            public static readonly int OffsetMask = CalculateOffsetMask(SegmentSize);
        }

        internal const int IntrosortSizeThreshold = 16;

        internal const MethodImplOptions FastPathMethodImplOptions = (MethodImplOptions)768;

        [MethodImpl((MethodImplOptions)768)]
        internal static int GetSegmentSize<T>()
        {
            if (Unsafe.SizeOf<T>() == Unsafe.SizeOf<object>())
            {
                return ReferenceTypeSegmentHelper.SegmentSize;
            }
            return ValueTypeSegmentHelper<T>.SegmentSize;
        }

        [MethodImpl((MethodImplOptions)768)]
        internal static int GetSegmentShift<T>()
        {
            if (Unsafe.SizeOf<T>() == Unsafe.SizeOf<object>())
            {
                return ReferenceTypeSegmentHelper.SegmentShift;
            }
            return ValueTypeSegmentHelper<T>.SegmentShift;
        }

        [MethodImpl((MethodImplOptions)768)]
        internal static int GetOffsetMask<T>()
        {
            if (Unsafe.SizeOf<T>() == Unsafe.SizeOf<object>())
            {
                return ReferenceTypeSegmentHelper.OffsetMask;
            }
            return ValueTypeSegmentHelper<T>.OffsetMask;
        }

        private static int CalculateSegmentSize(int elementSize)
        {
            int num = 2;
            while (ArraySize(elementSize, num << 1) < 85000)
            {
                num <<= 1;
            }
            return num;
            static int ArraySize(int elementSize, int segmentSize)
            {
                return 2 * IntPtr.Size + elementSize * segmentSize;
            }
        }

        private static int CalculateSegmentShift(int segmentSize)
        {
            int num = 0;
            while ((segmentSize >>= 1) != 0)
            {
                num++;
            }
            return num;
        }

        private static int CalculateOffsetMask(int segmentSize)
        {
            return segmentSize - 1;
        }
    }
}
