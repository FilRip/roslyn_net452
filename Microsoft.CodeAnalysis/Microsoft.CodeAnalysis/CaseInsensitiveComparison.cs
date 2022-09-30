using System;
using System.Globalization;
using System.Text;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class CaseInsensitiveComparison
    {
        private sealed class OneToOneUnicodeComparer : StringComparer
        {
            private static int CompareLowerUnicode(char c1, char c2)
            {
                if (c1 != c2)
                {
                    return ToLower(c1) - ToLower(c2);
                }
                return 0;
            }

            public override int Compare(string? str1, string? str2)
            {
                if ((object)str1 == str2)
                {
                    return 0;
                }
                if (str1 == null)
                {
                    return -1;
                }
                if (str2 == null)
                {
                    return 1;
                }
                int num = Math.Min(str1!.Length, str2!.Length);
                for (int i = 0; i < num; i++)
                {
                    int num2 = CompareLowerUnicode(str1![i], str2![i]);
                    if (num2 != 0)
                    {
                        return num2;
                    }
                }
                return str1!.Length - str2!.Length;
            }

            public int Compare(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
            {
                int num = Math.Min(str1.Length, str2.Length);
                for (int i = 0; i < num; i++)
                {
                    int num2 = CompareLowerUnicode(str1[i], str2[i]);
                    if (num2 != 0)
                    {
                        return num2;
                    }
                }
                return str1.Length - str2.Length;
            }

            private static bool AreEqualLowerUnicode(char c1, char c2)
            {
                if (c1 != c2)
                {
                    return ToLower(c1) == ToLower(c2);
                }
                return true;
            }

            public override bool Equals(string? str1, string? str2)
            {
                if ((object)str1 == str2)
                {
                    return true;
                }
                if (str1 == null || str2 == null)
                {
                    return false;
                }
                if (str1!.Length != str2!.Length)
                {
                    return false;
                }
                for (int i = 0; i < str1!.Length; i++)
                {
                    if (!AreEqualLowerUnicode(str1![i], str2![i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool Equals(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
            {
                if (str1.Length != str2.Length)
                {
                    return false;
                }
                for (int i = 0; i < str1.Length; i++)
                {
                    if (!AreEqualLowerUnicode(str1[i], str2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public static bool EndsWith(string value, string possibleEnd)
            {
                if ((object)value == possibleEnd)
                {
                    return true;
                }
                if (value == null || possibleEnd == null)
                {
                    return false;
                }
                int num = value.Length - 1;
                int num2 = possibleEnd.Length - 1;
                if (num < num2)
                {
                    return false;
                }
                while (num2 >= 0)
                {
                    if (!AreEqualLowerUnicode(value[num], possibleEnd[num2]))
                    {
                        return false;
                    }
                    num--;
                    num2--;
                }
                return true;
            }

            public static bool StartsWith(string value, string possibleStart)
            {
                if ((object)value == possibleStart)
                {
                    return true;
                }
                if (value == null || possibleStart == null)
                {
                    return false;
                }
                if (value.Length < possibleStart.Length)
                {
                    return false;
                }
                for (int i = 0; i < possibleStart.Length; i++)
                {
                    if (!AreEqualLowerUnicode(value[i], possibleStart[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public override int GetHashCode(string str)
            {
                int num = -2128831035;
                for (int i = 0; i < str.Length; i++)
                {
                    num = Hash.CombineFNVHash(num, ToLower(str[i]));
                }
                return num;
            }
        }

        private static readonly TextInfo s_unicodeCultureTextInfo = GetUnicodeCulture().TextInfo;

        private static readonly OneToOneUnicodeComparer s_comparer = new OneToOneUnicodeComparer();

        public static StringComparer Comparer => s_comparer;

        private static CultureInfo GetUnicodeCulture()
        {
            try
            {
                return new CultureInfo("en");
            }
            catch (ArgumentException)
            {
                return CultureInfo.InvariantCulture;
            }
        }

        public static char ToLower(char c)
        {
            if ((uint)(c - 65) <= 25u)
            {
                return (char)(c | 0x20u);
            }
            if (c < 'À')
            {
                return c;
            }
            return ToLowerNonAscii(c);
        }

        private static char ToLowerNonAscii(char c)
        {
            if (c == 'İ')
            {
                return 'i';
            }
            return s_unicodeCultureTextInfo.ToLower(c);
        }

        public static bool Equals(string left, string right)
        {
            return s_comparer.Equals(left, right);
        }

        public static bool Equals(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            return s_comparer.Equals(left, right);
        }

        public static bool EndsWith(string value, string possibleEnd)
        {
            return OneToOneUnicodeComparer.EndsWith(value, possibleEnd);
        }

        public static bool StartsWith(string value, string possibleStart)
        {
            return OneToOneUnicodeComparer.StartsWith(value, possibleStart);
        }

        public static int Compare(string left, string right)
        {
            return s_comparer.Compare(left, right);
        }

        public static int Compare(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            return s_comparer.Compare(left, right);
        }

        public static int GetHashCode(string value)
        {
            return s_comparer.GetHashCode(value);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
        public static string? ToLower(string? value)
        {
            if (value == null)
            {
                return null;
            }
            if (value!.Length == 0)
            {
                return value;
            }
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            StringBuilder builder = instance.Builder;
            builder.Append(value);
            ToLower(builder);
            return instance.ToStringAndFree();
        }

        public static void ToLower(StringBuilder builder)
        {
            if (builder != null)
            {
                for (int i = 0; i < builder.Length; i++)
                {
                    builder[i] = ToLower(builder[i]);
                }
            }
        }
    }
}
