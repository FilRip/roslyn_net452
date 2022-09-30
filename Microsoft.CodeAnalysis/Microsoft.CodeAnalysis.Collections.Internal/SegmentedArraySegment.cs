namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal readonly struct SegmentedArraySegment<T>
    {
        public SegmentedArray<T> Array { get; }

        public int Start { get; }

        public int Length { get; }

        public ref T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Length)
                {
                    ThrowHelper.ThrowIndexOutOfRangeException();
                }
                return ref Array[index + Start];
            }
        }

        public SegmentedArraySegment(SegmentedArray<T> array, int start, int length)
        {
            Array = array;
            Start = start;
            Length = length;
        }

        public SegmentedArraySegment<T> Slice(int start)
        {
            if ((uint)start >= (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            return new SegmentedArraySegment<T>(Array, Start + start, Length - start);
        }

        public SegmentedArraySegment<T> Slice(int start, int length)
        {
            if ((ulong)((uint)start + (long)(uint)length) > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            return new SegmentedArraySegment<T>(Array, Start + start, length);
        }
    }
}
