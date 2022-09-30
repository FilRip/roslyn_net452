using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;

#nullable enable

namespace Roslyn.Utilities
{
    public static class Hash
    {
        internal const int FnvOffsetBias = -2128831035;

        internal const int FnvPrime = 16777619;

        public static int Combine(int newKey, int currentKey)
        {
            return currentKey * -1521134295 + newKey;
        }

        public static int Combine(bool newKeyPart, int currentKey)
        {
            return Combine(currentKey, newKeyPart ? 1 : 0);
        }

        public static int Combine<T>(T newKeyPart, int currentKey) where T : class?
        {
            int num = currentKey * -1521134295;
            if (newKeyPart != null)
            {
                return num + newKeyPart.GetHashCode();
            }
            return num;
        }

        public static int CombineValues<T>(IEnumerable<T>? values, int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
            {
                return 0;
            }
            int num = 0;
            int num2 = 0;
            foreach (T item in values!)
            {
                if (num2++ < maxItemsToHash)
                {
                    if (item != null)
                    {
                        num = Combine(item.GetHashCode(), num);
                    }
                    continue;
                }
                return num;
            }
            return num;
        }

        public static int CombineValues<T>(T[]? values, int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
            {
                return 0;
            }
            int num = Math.Min(maxItemsToHash, values!.Length);
            int num2 = 0;
            for (int i = 0; i < num; i++)
            {
                T val = values[i];
                if (val != null)
                {
                    num2 = Combine(val.GetHashCode(), num2);
                }
            }
            return num2;
        }

        public static int CombineValues<T>(ImmutableArray<T> values, int maxItemsToHash = int.MaxValue)
        {
            if (values.IsDefaultOrEmpty)
            {
                return 0;
            }
            int num = 0;
            int num2 = 0;
            ImmutableArray<T>.Enumerator enumerator = values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (num2++ >= maxItemsToHash)
                {
                    break;
                }
                if (current != null)
                {
                    num = Combine(current.GetHashCode(), num);
                }
            }
            return num;
        }

        public static int CombineValues(IEnumerable<string?>? values, StringComparer stringComparer, int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
            {
                return 0;
            }
            int num = 0;
            int num2 = 0;
            foreach (string item in values!)
            {
                if (num2++ < maxItemsToHash)
                {
                    if (item != null)
                    {
                        num = Combine(stringComparer.GetHashCode(item), num);
                    }
                    continue;
                }
                return num;
            }
            return num;
        }

        public static int GetFNVHashCode(byte[] data)
        {
            int num = -2128831035;
            for (int i = 0; i < data.Length; i++)
            {
                num = (num ^ data[i]) * 16777619;
            }
            return num;
        }

        public static int GetFNVHashCode(ReadOnlySpan<byte> data, out bool isAscii)
        {
            int num = -2128831035;
            byte b = 0;
            for (int i = 0; i < data.Length; i++)
            {
                byte b2 = data[i];
                b = (byte)(b | b2);
                num = (num ^ b2) * 16777619;
            }
            isAscii = (b & 0x80) == 0;
            return num;
        }

        public static int GetFNVHashCode(ImmutableArray<byte> data)
        {
            int num = -2128831035;
            for (int i = 0; i < data.Length; i++)
            {
                num = (num ^ data[i]) * 16777619;
            }
            return num;
        }

        public static int GetFNVHashCode(ReadOnlySpan<char> data)
        {
            int num = -2128831035;
            for (int i = 0; i < data.Length; i++)
            {
                num = (num ^ data[i]) * 16777619;
            }
            return num;
        }

        public static int GetFNVHashCode(string text, int start, int length)
        {
            return GetFNVHashCode(MemoryExtensions.AsSpan(text, start, length));
        }

        public static int GetCaseInsensitiveFNVHashCode(string text)
        {
            return GetCaseInsensitiveFNVHashCode(MemoryExtensions.AsSpan(text, 0, text.Length));
        }

        public static int GetCaseInsensitiveFNVHashCode(ReadOnlySpan<char> data)
        {
            int num = -2128831035;
            for (int i = 0; i < data.Length; i++)
            {
                num = (num ^ CaseInsensitiveComparison.ToLower(data[i])) * 16777619;
            }
            return num;
        }

        public static int GetFNVHashCode(string text, int start)
        {
            return GetFNVHashCode(text, start, text.Length - start);
        }

        public static int GetFNVHashCode(string text)
        {
            return CombineFNVHash(-2128831035, text);
        }

        public static int GetFNVHashCode(StringBuilder text)
        {
            int num = -2128831035;
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                num = (num ^ text[i]) * 16777619;
            }
            return num;
        }

        public static int GetFNVHashCode(char[] text, int start, int length)
        {
            int num = -2128831035;
            int num2 = start + length;
            for (int i = start; i < num2; i++)
            {
                num = (num ^ text[i]) * 16777619;
            }
            return num;
        }

        public static int GetFNVHashCode(char ch)
        {
            return CombineFNVHash(-2128831035, ch);
        }

        public static int CombineFNVHash(int hashCode, string text)
        {
            foreach (char c in text)
            {
                hashCode = (hashCode ^ c) * 16777619;
            }
            return hashCode;
        }

        public static int CombineFNVHash(int hashCode, char ch)
        {
            return (hashCode ^ ch) * 16777619;
        }
    }
}
