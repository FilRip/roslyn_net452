using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal static class SegmentedGenericArraySortHelper<T> where T : IComparable<T>
    {
        public static void Sort(SegmentedArraySegment<T> keys, IComparer<T>? comparer)
        {
            try
            {
                if (comparer == null || comparer == Comparer<T>.Default)
                {
                    if (keys.Length <= 1)
                    {
                        return;
                    }
                    if (typeof(T) == typeof(double) || typeof(T) == typeof(float) || typeof(T) == typeof(float))
                    {
                        int num = SegmentedArraySortUtils.MoveNansToFront(keys, default(Span<byte>));
                        if (num == keys.Length)
                        {
                            return;
                        }
                        keys = keys.Slice(num);
                    }
                    IntroSort(keys, 2 * (SegmentedArraySortUtils.Log2((uint)keys.Length) + 1));
                }
                else
                {
                    SegmentedArraySortHelper<T>.IntrospectiveSort(keys, comparer!.Compare);
                }
            }
            catch (IndexOutOfRangeException)
            {
                ThrowHelper.ThrowArgumentException_BadComparer(comparer);
            }
            catch (Exception e)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IComparerFailed, e);
            }
        }

        public static int BinarySearch(SegmentedArray<T> array, int index, int length, T value, IComparer<T>? comparer)
        {
            try
            {
                if (comparer == null || comparer == Comparer<T>.Default)
                {
                    return BinarySearch(array, index, length, value);
                }
                return SegmentedArraySortHelper<T>.InternalBinarySearch(array, index, length, value, comparer);
            }
            catch (Exception e)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IComparerFailed, e);
                return 0;
            }
        }

        private static int BinarySearch(SegmentedArray<T> array, int index, int length, T value)
        {
            int num = index;
            int num2 = index + length - 1;
            while (num <= num2)
            {
                int num3 = num + (num2 - num >> 1);
                int num4 = ((array[num3] != null) ? array[num3].CompareTo(value) : ((value != null) ? (-1) : 0));
                if (num4 == 0)
                {
                    return num3;
                }
                if (num4 < 0)
                {
                    num = num3 + 1;
                }
                else
                {
                    num2 = num3 - 1;
                }
            }
            return ~num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapIfGreater(ref T i, ref T j)
        {
            if (i != null && GreaterThan(ref i, ref j))
            {
                Swap(ref i, ref j);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(ref T i, ref T j)
        {
            T val = i;
            i = j;
            j = val;
        }

        private static void IntroSort(SegmentedArraySegment<T> keys, int depthLimit)
        {
            int num = keys.Length;
            while (num > 1)
            {
                if (num <= 16)
                {
                    switch (num)
                    {
                        case 2:
                            SwapIfGreater(ref keys[0], ref keys[1]);
                            break;
                        case 3:
                            {
                                ref T j = ref keys[2];
                                ref T reference = ref keys[1];
                                ref T i = ref keys[0];
                                SwapIfGreater(ref i, ref reference);
                                SwapIfGreater(ref i, ref j);
                                SwapIfGreater(ref reference, ref j);
                                break;
                            }
                        default:
                            InsertionSort(keys.Slice(0, num));
                            break;
                    }
                    break;
                }
                if (depthLimit == 0)
                {
                    HeapSort(keys.Slice(0, num));
                    break;
                }
                depthLimit--;
                int num2 = PickPivotAndPartition(keys.Slice(0, num));
                IntroSort(keys.Slice(num2 + 1, num - (num2 + 1)), depthLimit);
                num = num2;
            }
        }

        private static int PickPivotAndPartition(SegmentedArraySegment<T> keys)
        {
            int num = 0;
            int index = keys.Length - 1;
            int index2 = keys.Length - 1 >> 1;
            SwapIfGreater(ref keys[num], ref keys[index2]);
            SwapIfGreater(ref keys[num], ref keys[index]);
            SwapIfGreater(ref keys[index2], ref keys[index]);
            int num2 = keys.Length - 2;
            T left = keys[index2];
            Swap(ref keys[index2], ref keys[num2]);
            int num3 = num;
            int num4 = num2;
            while (num3 < num4)
            {
                if (left == null)
                {
                    while (num3 < num2 && keys[++num3] == null)
                    {
                    }
                    while (num4 > num && keys[--num4] != null)
                    {
                    }
                }
                else
                {
                    while (num3 < num2 && GreaterThan(ref left, ref keys[++num3]))
                    {
                    }
                    while (num4 > num && LessThan(ref left, ref keys[--num4]))
                    {
                    }
                }
                if (num3 >= num4)
                {
                    break;
                }
                Swap(ref keys[num3], ref keys[num4]);
            }
            if (num3 != num2)
            {
                Swap(ref keys[num3], ref keys[num2]);
            }
            return num3;
        }

        private static void HeapSort(SegmentedArraySegment<T> keys)
        {
            int length = keys.Length;
            for (int num = length >> 1; num >= 1; num--)
            {
                DownHeap(keys, num, length, 0);
            }
            for (int num2 = length; num2 > 1; num2--)
            {
                Swap(ref keys[0], ref keys[num2 - 1]);
                DownHeap(keys, 1, num2 - 1, 0);
            }
        }

        private static void DownHeap(SegmentedArraySegment<T> keys, int i, int n, int lo)
        {
            T left = keys[lo + i - 1];
            while (i <= n >> 1)
            {
                int num = 2 * i;
                if (num < n && (keys[lo + num - 1] == null || LessThan(ref keys[lo + num - 1], ref keys[lo + num])))
                {
                    num++;
                }
                if (keys[lo + num - 1] == null || !LessThan(ref left, ref keys[lo + num - 1]))
                {
                    break;
                }
                keys[lo + i - 1] = keys[lo + num - 1];
                i = num;
            }
            keys[lo + i - 1] = left;
        }

        private static void InsertionSort(SegmentedArraySegment<T> keys)
        {
            for (int i = 0; i < keys.Length - 1; i++)
            {
                T left = keys[i + 1];
                int num = i;
                while (num >= 0 && (left == null || LessThan(ref left, ref keys[num])))
                {
                    keys[num + 1] = keys[num];
                    num--;
                }
                keys[num + 1] = left;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LessThan(ref T left, ref T right)
        {
            if (typeof(T) == typeof(byte))
            {
                if ((byte)(object)left >= (byte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(sbyte))
            {
                if ((sbyte)(object)left >= (sbyte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(ushort))
            {
                if ((ushort)(object)left >= (ushort)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(short))
            {
                if ((short)(object)left >= (short)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(uint))
            {
                if ((uint)(object)left >= (uint)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(int))
            {
                if ((int)(object)left >= (int)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(ulong))
            {
                if ((ulong)(object)left >= (ulong)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(long))
            {
                if ((long)(object)left >= (long)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(UIntPtr))
            {
                if ((UIntPtr)(object)left >= (nuint)(UIntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(IntPtr))
            {
                if ((IntPtr)(object)left >= (nint)(IntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(float))
            {
                if (!((float)(object)left < (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(double))
            {
                if (!((double)(object)left < (double)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(float))
            {
                if (!((float)(object)left < (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (left.CompareTo(right) >= 0)
            {
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GreaterThan(ref T left, ref T right)
        {
            if (typeof(T) == typeof(byte))
            {
                if ((byte)(object)left <= (byte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(sbyte))
            {
                if ((sbyte)(object)left <= (sbyte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(ushort))
            {
                if ((ushort)(object)left <= (ushort)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(short))
            {
                if ((short)(object)left <= (short)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(uint))
            {
                if ((uint)(object)left <= (uint)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(int))
            {
                if ((int)(object)left <= (int)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(ulong))
            {
                if ((ulong)(object)left <= (ulong)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(long))
            {
                if ((long)(object)left <= (long)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(UIntPtr))
            {
                if ((UIntPtr)(object)left <= (nuint)(UIntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(IntPtr))
            {
                if ((IntPtr)(object)left <= (nint)(IntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(float))
            {
                if (!((float)(object)left > (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(double))
            {
                if (!((double)(object)left > (double)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(T) == typeof(float))
            {
                if (!((float)(object)left > (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (left.CompareTo(right) <= 0)
            {
                return false;
            }
            return true;
        }
    }
    internal static class SegmentedGenericArraySortHelper<TKey, TValue> where TKey : IComparable<TKey>
    {
        public static void Sort(SegmentedArraySegment<TKey> keys, Span<TValue> values, IComparer<TKey>? comparer)
        {
            try
            {
                if (comparer == null || comparer == Comparer<TKey>.Default)
                {
                    if (keys.Length <= 1)
                    {
                        return;
                    }
                    if (typeof(TKey) == typeof(double) || typeof(TKey) == typeof(float) || typeof(TKey) == typeof(float))
                    {
                        int num = SegmentedArraySortUtils.MoveNansToFront(keys, values);
                        if (num == keys.Length)
                        {
                            return;
                        }
                        keys = keys.Slice(num);
                        values = values.Slice(num);
                    }
                    IntroSort(keys, values, 2 * (SegmentedArraySortUtils.Log2((uint)keys.Length) + 1));
                }
                else
                {
                    SegmentedArraySortHelper<TKey, TValue>.IntrospectiveSort(keys, values, comparer);
                }
            }
            catch (IndexOutOfRangeException)
            {
                ThrowHelper.ThrowArgumentException_BadComparer(comparer);
            }
            catch (Exception e)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IComparerFailed, e);
            }
        }

        private static void SwapIfGreaterWithValues(SegmentedArraySegment<TKey> keys, Span<TValue> values, int i, int j)
        {
            ref TKey reference = ref keys[i];
            if (reference != null && GreaterThan(ref reference, ref keys[j]))
            {
                TKey val = reference;
                keys[i] = keys[j];
                keys[j] = val;
                TValue val2 = values[i];
                values[i] = values[j];
                values[j] = val2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(SegmentedArraySegment<TKey> keys, Span<TValue> values, int i, int j)
        {
            TKey val = keys[i];
            keys[i] = keys[j];
            keys[j] = val;
            TValue val2 = values[i];
            values[i] = values[j];
            values[j] = val2;
        }

        private static void IntroSort(SegmentedArraySegment<TKey> keys, Span<TValue> values, int depthLimit)
        {
            int num = keys.Length;
            while (num > 1)
            {
                if (num <= 16)
                {
                    switch (num)
                    {
                        case 2:
                            SwapIfGreaterWithValues(keys, values, 0, 1);
                            break;
                        case 3:
                            SwapIfGreaterWithValues(keys, values, 0, 1);
                            SwapIfGreaterWithValues(keys, values, 0, 2);
                            SwapIfGreaterWithValues(keys, values, 1, 2);
                            break;
                        default:
                            InsertionSort(keys.Slice(0, num), values.Slice(0, num));
                            break;
                    }
                    break;
                }
                if (depthLimit == 0)
                {
                    HeapSort(keys.Slice(0, num), values.Slice(0, num));
                    break;
                }
                depthLimit--;
                int num2 = PickPivotAndPartition(keys.Slice(0, num), values.Slice(0, num));
                IntroSort(keys.Slice(num2 + 1, num - (num2 + 1)), values.Slice(num2 + 1, num - (num2 + 1)), depthLimit);
                num = num2;
            }
        }

        private static int PickPivotAndPartition(SegmentedArraySegment<TKey> keys, Span<TValue> values)
        {
            int num = keys.Length - 1;
            int num2 = num >> 1;
            SwapIfGreaterWithValues(keys, values, 0, num2);
            SwapIfGreaterWithValues(keys, values, 0, num);
            SwapIfGreaterWithValues(keys, values, num2, num);
            TKey left = keys[num2];
            Swap(keys, values, num2, num - 1);
            int num3 = 0;
            int num4 = num - 1;
            while (num3 < num4)
            {
                if (left == null)
                {
                    while (num3 < num - 1 && keys[++num3] == null)
                    {
                    }
                    while (num4 > 0 && keys[--num4] != null)
                    {
                    }
                }
                else
                {
                    while (GreaterThan(ref left, ref keys[++num3]))
                    {
                    }
                    while (LessThan(ref left, ref keys[--num4]))
                    {
                    }
                }
                if (num3 >= num4)
                {
                    break;
                }
                Swap(keys, values, num3, num4);
            }
            if (num3 != num - 1)
            {
                Swap(keys, values, num3, num - 1);
            }
            return num3;
        }

        private static void HeapSort(SegmentedArraySegment<TKey> keys, Span<TValue> values)
        {
            int length = keys.Length;
            for (int num = length >> 1; num >= 1; num--)
            {
                DownHeap(keys, values, num, length, 0);
            }
            for (int num2 = length; num2 > 1; num2--)
            {
                Swap(keys, values, 0, num2 - 1);
                DownHeap(keys, values, 1, num2 - 1, 0);
            }
        }

        private static void DownHeap(SegmentedArraySegment<TKey> keys, Span<TValue> values, int i, int n, int lo)
        {
            TKey left = keys[lo + i - 1];
            TValue val = values[lo + i - 1];
            while (i <= n >> 1)
            {
                int num = 2 * i;
                if (num < n && (keys[lo + num - 1] == null || LessThan(ref keys[lo + num - 1], ref keys[lo + num])))
                {
                    num++;
                }
                if (keys[lo + num - 1] == null || !LessThan(ref left, ref keys[lo + num - 1]))
                {
                    break;
                }
                keys[lo + i - 1] = keys[lo + num - 1];
                values[lo + i - 1] = values[lo + num - 1];
                i = num;
            }
            keys[lo + i - 1] = left;
            values[lo + i - 1] = val;
        }

        private static void InsertionSort(SegmentedArraySegment<TKey> keys, Span<TValue> values)
        {
            for (int i = 0; i < keys.Length - 1; i++)
            {
                TKey left = keys[i + 1];
                TValue val = values[i + 1];
                int num = i;
                while (num >= 0 && (left == null || LessThan(ref left, ref keys[num])))
                {
                    keys[num + 1] = keys[num];
                    values[num + 1] = values[num];
                    num--;
                }
                keys[num + 1] = left;
                values[num + 1] = val;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LessThan(ref TKey left, ref TKey right)
        {
            if (typeof(TKey) == typeof(byte))
            {
                if ((byte)(object)left >= (byte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(sbyte))
            {
                if ((sbyte)(object)left >= (sbyte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(ushort))
            {
                if ((ushort)(object)left >= (ushort)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(short))
            {
                if ((short)(object)left >= (short)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(uint))
            {
                if ((uint)(object)left >= (uint)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(int))
            {
                if ((int)(object)left >= (int)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(ulong))
            {
                if ((ulong)(object)left >= (ulong)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(long))
            {
                if ((long)(object)left >= (long)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(UIntPtr))
            {
                if ((UIntPtr)(object)left >= (nuint)(UIntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(IntPtr))
            {
                if ((IntPtr)(object)left >= (nint)(IntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(float))
            {
                if (!((float)(object)left < (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(double))
            {
                if (!((double)(object)left < (double)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(float))
            {
                if (!((float)(object)left < (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (left.CompareTo(right) >= 0)
            {
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GreaterThan(ref TKey left, ref TKey right)
        {
            if (typeof(TKey) == typeof(byte))
            {
                if ((byte)(object)left <= (byte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(sbyte))
            {
                if ((sbyte)(object)left <= (sbyte)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(ushort))
            {
                if ((ushort)(object)left <= (ushort)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(short))
            {
                if ((short)(object)left <= (short)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(uint))
            {
                if ((uint)(object)left <= (uint)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(int))
            {
                if ((int)(object)left <= (int)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(ulong))
            {
                if ((ulong)(object)left <= (ulong)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(long))
            {
                if ((long)(object)left <= (long)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(UIntPtr))
            {
                if ((UIntPtr)(object)left <= (nuint)(UIntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(IntPtr))
            {
                if ((IntPtr)(object)left <= (nint)(IntPtr)(object)right)
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(float))
            {
                if (!((float)(object)left > (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(double))
            {
                if (!((double)(object)left > (double)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (typeof(TKey) == typeof(float))
            {
                if (!((float)(object)left > (float)(object)right))
                {
                    return false;
                }
                return true;
            }
            if (left.CompareTo(right) <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
