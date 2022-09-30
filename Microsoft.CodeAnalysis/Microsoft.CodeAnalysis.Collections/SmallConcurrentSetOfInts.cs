using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.Collections
{
    internal class SmallConcurrentSetOfInts
    {
        private int _v1;

        private int _v2;

        private int _v3;

        private int _v4;

        private SmallConcurrentSetOfInts? _next;

        private const int unoccupied = int.MinValue;

        public SmallConcurrentSetOfInts()
        {
            _v1 = (_v2 = (_v3 = (_v4 = int.MinValue)));
        }

        private SmallConcurrentSetOfInts(int initialValue)
        {
            _v1 = initialValue;
            _v2 = (_v3 = (_v4 = int.MinValue));
        }

        public bool Contains(int i)
        {
            return Contains(this, i);
        }

        private static bool Contains(SmallConcurrentSetOfInts set, int i)
        {
            SmallConcurrentSetOfInts smallConcurrentSetOfInts = set;
            do
            {
                if (smallConcurrentSetOfInts._v1 == i || smallConcurrentSetOfInts._v2 == i || smallConcurrentSetOfInts._v3 == i || smallConcurrentSetOfInts._v4 == i)
                {
                    return true;
                }
                smallConcurrentSetOfInts = smallConcurrentSetOfInts._next;
            }
            while (smallConcurrentSetOfInts != null);
            return false;
        }

        public bool Add(int i)
        {
            return Add(this, i);
        }

        private static bool Add(SmallConcurrentSetOfInts set, int i)
        {
            bool added = false;
            while (true)
            {
                if (AddHelper(ref set._v1, i, ref added) || AddHelper(ref set._v2, i, ref added) || AddHelper(ref set._v3, i, ref added) || AddHelper(ref set._v4, i, ref added))
                {
                    return added;
                }
                SmallConcurrentSetOfInts smallConcurrentSetOfInts = set._next;
                if (smallConcurrentSetOfInts == null)
                {
                    SmallConcurrentSetOfInts value = new SmallConcurrentSetOfInts(i);
                    smallConcurrentSetOfInts = Interlocked.CompareExchange(ref set._next, value, null);
                    if (smallConcurrentSetOfInts == null)
                    {
                        break;
                    }
                }
                set = smallConcurrentSetOfInts;
            }
            return true;
        }

        private static bool AddHelper(ref int slot, int i, ref bool added)
        {
            int num = slot;
            if (num == int.MinValue)
            {
                num = Interlocked.CompareExchange(ref slot, i, int.MinValue);
                if (num == int.MinValue)
                {
                    added = true;
                    return true;
                }
            }
            return num == i;
        }
    }
}
