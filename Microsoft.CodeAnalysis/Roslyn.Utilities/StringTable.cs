using System;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Roslyn.Utilities
{
    public class StringTable
    {
        private struct Entry
        {
            public int HashCode;

            public string Text;
        }

        private const int LocalSizeBits = 11;

        private const int LocalSize = 2048;

        private const int LocalSizeMask = 2047;

        private const int SharedSizeBits = 16;

        private const int SharedSize = 65536;

        private const int SharedSizeMask = 65535;

        private const int SharedBucketBits = 4;

        private const int SharedBucketSize = 16;

        private const int SharedBucketSizeMask = 15;

        private readonly Entry[] _localTable = new Entry[2048];

        private static readonly Entry[] s_sharedTable = new Entry[65536];

        private int _localRandom = Environment.TickCount;

        private static int s_sharedRandom = Environment.TickCount;

        private readonly ObjectPool<StringTable>? _pool;

        private static readonly ObjectPool<StringTable> s_staticPool = CreatePool();

        internal StringTable()
            : this(null)
        {
        }

        private StringTable(ObjectPool<StringTable>? pool)
        {
            _pool = pool;
        }

        private static ObjectPool<StringTable> CreatePool()
        {
            return new ObjectPool<StringTable>((ObjectPool<StringTable> pool) => new StringTable(pool), Environment.ProcessorCount * 2);
        }

        public static StringTable GetInstance()
        {
            return s_staticPool.Allocate();
        }

        public void Free()
        {
            _pool?.Free(this);
        }

        public string Add(char[] chars, int start, int len)
        {
            Span<char> span = MemoryExtensions.AsSpan(chars, start, len);
            int fNVHashCode = Hash.GetFNVHashCode(chars, start, len);
            Entry[] localTable = _localTable;
            int num = LocalIdxFromHash(fNVHashCode);
            if (localTable[num].Text != null && localTable[num].HashCode == fNVHashCode)
            {
                string text = localTable[num].Text;
                if (TextEquals(text, span))
                {
                    return text;
                }
            }
            string text2 = FindSharedEntry(chars, start, len, fNVHashCode);
            if (text2 != null)
            {
                localTable[num].HashCode = fNVHashCode;
                localTable[num].Text = text2;
                return text2;
            }
            return AddItem(chars, start, len, fNVHashCode);
        }

        public string Add(string chars, int start, int len)
        {
            int fNVHashCode = Hash.GetFNVHashCode(chars, start, len);
            Entry[] localTable = _localTable;
            int num = LocalIdxFromHash(fNVHashCode);
            if (localTable[num].Text != null && localTable[num].HashCode == fNVHashCode)
            {
                string text = localTable[num].Text;
                if (TextEquals(text, chars, start, len))
                {
                    return text;
                }
            }
            string text2 = FindSharedEntry(chars, start, len, fNVHashCode);
            if (text2 != null)
            {
                localTable[num].HashCode = fNVHashCode;
                localTable[num].Text = text2;
                return text2;
            }
            return AddItem(chars, start, len, fNVHashCode);
        }

        public string Add(char chars)
        {
            int fNVHashCode = Hash.GetFNVHashCode(chars);
            Entry[] localTable = _localTable;
            int num = LocalIdxFromHash(fNVHashCode);
            string text = localTable[num].Text;
            if (text != null)
            {
                string text2 = localTable[num].Text;
                if (text.Length == 1 && text[0] == chars)
                {
                    return text2;
                }
            }
            string text3 = FindSharedEntry(chars, fNVHashCode);
            if (text3 != null)
            {
                localTable[num].HashCode = fNVHashCode;
                localTable[num].Text = text3;
                return text3;
            }
            return AddItem(chars, fNVHashCode);
        }

        public string Add(StringBuilder chars)
        {
            int fNVHashCode = Hash.GetFNVHashCode(chars);
            Entry[] localTable = _localTable;
            int num = LocalIdxFromHash(fNVHashCode);
            if (localTable[num].Text != null && localTable[num].HashCode == fNVHashCode)
            {
                string text = localTable[num].Text;
                if (TextEquals(text, chars))
                {
                    return text;
                }
            }
            string text2 = FindSharedEntry(chars, fNVHashCode);
            if (text2 != null)
            {
                localTable[num].HashCode = fNVHashCode;
                localTable[num].Text = text2;
                return text2;
            }
            return AddItem(chars, fNVHashCode);
        }

        public string Add(string chars)
        {
            int fNVHashCode = Hash.GetFNVHashCode(chars);
            Entry[] localTable = _localTable;
            int num = LocalIdxFromHash(fNVHashCode);
            if (localTable[num].Text != null && localTable[num].HashCode == fNVHashCode)
            {
                string text = localTable[num].Text;
                if (text == chars)
                {
                    return text;
                }
            }
            string text2 = FindSharedEntry(chars, fNVHashCode);
            if (text2 != null)
            {
                localTable[num].HashCode = fNVHashCode;
                localTable[num].Text = text2;
                return text2;
            }
            AddCore(chars, fNVHashCode);
            return chars;
        }

        private static string? FindSharedEntry(char[] chars, int start, int len, int hashCode)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            string text = null;
            for (int i = 1; i < 17; i++)
            {
                text = array[num].Text;
                int hashCode2 = array[num].HashCode;
                if (text == null || (hashCode2 == hashCode && TextEquals(text, MemoryExtensions.AsSpan(chars, start, len))))
                {
                    break;
                }
                text = null;
                num = (num + i) & 0xFFFF;
            }
            return text;
        }

        private static string? FindSharedEntry(string chars, int start, int len, int hashCode)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            string text = null;
            for (int i = 1; i < 17; i++)
            {
                text = array[num].Text;
                int hashCode2 = array[num].HashCode;
                if (text == null || (hashCode2 == hashCode && TextEquals(text, chars, start, len)))
                {
                    break;
                }
                text = null;
                num = (num + i) & 0xFFFF;
            }
            return text;
        }

        private static string? FindSharedEntryASCII(int hashCode, ReadOnlySpan<byte> asciiChars)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            string text = null;
            for (int i = 1; i < 17; i++)
            {
                text = array[num].Text;
                int hashCode2 = array[num].HashCode;
                if (text == null || (hashCode2 == hashCode && TextEqualsASCII(text, asciiChars)))
                {
                    break;
                }
                text = null;
                num = (num + i) & 0xFFFF;
            }
            return text;
        }

        private static string? FindSharedEntry(char chars, int hashCode)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            string text = null;
            for (int i = 1; i < 17; i++)
            {
                text = array[num].Text;
                if (text == null || (text.Length == 1 && text[0] == chars))
                {
                    break;
                }
                text = null;
                num = (num + i) & 0xFFFF;
            }
            return text;
        }

        private static string? FindSharedEntry(StringBuilder chars, int hashCode)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            string text = null;
            for (int i = 1; i < 17; i++)
            {
                text = array[num].Text;
                int hashCode2 = array[num].HashCode;
                if (text == null || (hashCode2 == hashCode && TextEquals(text, chars)))
                {
                    break;
                }
                text = null;
                num = (num + i) & 0xFFFF;
            }
            return text;
        }

        private static string? FindSharedEntry(string chars, int hashCode)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            string text = null;
            for (int i = 1; i < 17; i++)
            {
                text = array[num].Text;
                int hashCode2 = array[num].HashCode;
                if (text == null || (hashCode2 == hashCode && text == chars))
                {
                    break;
                }
                text = null;
                num = (num + i) & 0xFFFF;
            }
            return text;
        }

        private string AddItem(char[] chars, int start, int len, int hashCode)
        {
            string text = new string(chars, start, len);
            AddCore(text, hashCode);
            return text;
        }

        private string AddItem(string chars, int start, int len, int hashCode)
        {
            string text = chars.Substring(start, len);
            AddCore(text, hashCode);
            return text;
        }

        private string AddItem(char chars, int hashCode)
        {
            string text = new string(chars, 1);
            AddCore(text, hashCode);
            return text;
        }

        private string AddItem(StringBuilder chars, int hashCode)
        {
            string text = chars.ToString();
            AddCore(text, hashCode);
            return text;
        }

        private void AddCore(string chars, int hashCode)
        {
            AddSharedEntry(hashCode, chars);
            Entry[] localTable = _localTable;
            int num = LocalIdxFromHash(hashCode);
            localTable[num].HashCode = hashCode;
            localTable[num].Text = chars;
        }

        private void AddSharedEntry(int hashCode, string text)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            int num2 = num;
            int num3 = 1;
            while (true)
            {
                if (num3 < 17)
                {
                    if (array[num2].Text == null)
                    {
                        num = num2;
                        break;
                    }
                    num2 = (num2 + num3) & 0xFFFF;
                    num3++;
                    continue;
                }
                int num4 = LocalNextRandom() & 0xF;
                num = (num + (num4 * num4 + num4) / 2) & 0xFFFF;
                break;
            }
            array[num].HashCode = hashCode;
            Volatile.Write(ref array[num].Text, text);
        }

        internal static string AddShared(StringBuilder chars)
        {
            int fNVHashCode = Hash.GetFNVHashCode(chars);
            string text = FindSharedEntry(chars, fNVHashCode);
            if (text != null)
            {
                return text;
            }
            return AddSharedSlow(fNVHashCode, chars);
        }

        private static string AddSharedSlow(int hashCode, StringBuilder builder)
        {
            string text = builder.ToString();
            AddSharedSlow(hashCode, text);
            return text;
        }

        internal static string AddSharedUTF8(ReadOnlySpan<byte> bytes)
        {
            int fNVHashCode = Hash.GetFNVHashCode(bytes, out bool isAscii);
            if (isAscii)
            {
                string text = FindSharedEntryASCII(fNVHashCode, bytes);
                if (text != null)
                {
                    return text;
                }
            }
            return AddSharedSlow(fNVHashCode, bytes, isAscii);
        }

        private static string AddSharedSlow(int hashCode, ReadOnlySpan<byte> utf8Bytes, bool isAscii)
        {
            string text;

            unsafe
            {
                fixed (byte* bytes = &utf8Bytes.GetPinnableReference())
                {
                    //text = Encoding.UTF8.GetString(bytes, utf8Bytes.Length);
                    // FilRip : Replacement of GetString with pointer
                    text = System.Reflection.Metadata.MetadataStringDecoder.DefaultUTF8.GetString(bytes, utf8Bytes.Length);
                }
            }

            // Don't add non-ascii strings to table. The hashCode we have here is not correct and we won't find them again.
            // Non-ascii in UTF8-encoded parts of metadata (the only use of this at the moment) is assumed to be rare in 
            // practice. If that turns out to be wrong, we could decode to pooled memory and rehash here.
            if (isAscii)
            {
                AddSharedSlow(hashCode, text);
            }

            return text;
        }

        private static void AddSharedSlow(int hashCode, string text)
        {
            Entry[] array = s_sharedTable;
            int num = SharedIdxFromHash(hashCode);
            int num2 = num;
            int num3 = 1;
            while (true)
            {
                if (num3 < 17)
                {
                    if (array[num2].Text == null)
                    {
                        num = num2;
                        break;
                    }
                    num2 = (num2 + num3) & 0xFFFF;
                    num3++;
                    continue;
                }
                int num4 = SharedNextRandom() & 0xF;
                num = (num + (num4 * num4 + num4) / 2) & 0xFFFF;
                break;
            }
            array[num].HashCode = hashCode;
            Volatile.Write(ref array[num].Text, text);
        }

        private static int LocalIdxFromHash(int hash)
        {
            return hash & 0x7FF;
        }

        private static int SharedIdxFromHash(int hash)
        {
            return (hash ^ (hash >> 11)) & 0xFFFF;
        }

        private int LocalNextRandom()
        {
            return _localRandom++;
        }

        private static int SharedNextRandom()
        {
            return Interlocked.Increment(ref s_sharedRandom);
        }

        internal static bool TextEquals(string array, string text, int start, int length)
        {
            if (array.Length != length)
            {
                return false;
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != text[start + i])
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool TextEquals(string array, StringBuilder text)
        {
            if (array.Length != text.Length)
            {
                return false;
            }
            for (int num = array.Length - 1; num >= 0; num--)
            {
                if (array[num] != text[num])
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool TextEqualsASCII(string text, ReadOnlySpan<byte> ascii)
        {
            if (ascii.Length != text.Length)
            {
                return false;
            }
            for (int i = 0; i < ascii.Length; i++)
            {
                if (ascii[i] != text[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool TextEquals(string array, ReadOnlySpan<char> text)
        {
            return MemoryExtensions.Equals(text, MemoryExtensions.AsSpan(array), StringComparison.Ordinal);
        }
    }
}
