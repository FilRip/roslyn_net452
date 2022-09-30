using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace Roslyn.Utilities
{
    public static class StringExtensions
    {
        private static ImmutableArray<string> s_lazyNumerals;

        private static readonly Func<char, char> s_toLower = char.ToLower;

        private static readonly Func<char, char> s_toUpper = char.ToUpper;

        private const string AttributeSuffix = "Attribute";

        public static string GetNumeral(int number)
        {
            ImmutableArray<string> value = s_lazyNumerals;
            if (value.IsDefault)
            {
                value = ImmutableArray.Create<string>("0", "1", "2", "3", "4", "5", "6", "7", "8", "9");
                ImmutableInterlocked.InterlockedInitialize(ref s_lazyNumerals, value);
            }
            if (number >= value.Length)
            {
                return number.ToString();
            }
            return value[number];
        }

        public static string Join(this IEnumerable<string?> source, string separator)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (separator == null)
            {
                throw new ArgumentNullException("separator");
            }
            return string.Join(separator, source);
        }

        public static bool LooksLikeInterfaceName(this string name)
        {
            if (name.Length >= 3 && name[0] == 'I' && char.IsUpper(name[1]))
            {
                return char.IsLower(name[2]);
            }
            return false;
        }

        public static bool LooksLikeTypeParameterName(this string name)
        {
            if (name.Length >= 3 && name[0] == 'T' && char.IsUpper(name[1]))
            {
                return char.IsLower(name[2]);
            }
            return false;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("shortName")]
        public static string? ToPascalCase(this string? shortName, bool trimLeadingTypePrefix = true)
        {
            return shortName.ConvertCase(trimLeadingTypePrefix, s_toUpper);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("shortName")]
        public static string? ToCamelCase(this string? shortName, bool trimLeadingTypePrefix = true)
        {
            return shortName.ConvertCase(trimLeadingTypePrefix, s_toLower);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("shortName")]
        private static string? ConvertCase(this string? shortName, bool trimLeadingTypePrefix, Func<char, char> convert)
        {
            if (!RoslynString.IsNullOrEmpty(shortName))
            {
                if (trimLeadingTypePrefix && (shortName.LooksLikeInterfaceName() || shortName.LooksLikeTypeParameterName()))
                {
                    return convert(shortName![1]) + shortName!.Substring(2);
                }
                if (convert(shortName![0]) != shortName![0])
                {
                    return convert(shortName![0]) + shortName!.Substring(1);
                }
            }
            return shortName;
        }

        public static bool IsValidClrTypeName([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] this string? name)
        {
            if (!RoslynString.IsNullOrEmpty(name))
            {
                return name!.IndexOf('\0') == -1;
            }
            return false;
        }

        public static bool IsValidClrNamespaceName([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] this string? name)
        {
            if (RoslynString.IsNullOrEmpty(name))
            {
                return false;
            }
            char c = '.';
            for (int i = 0; i < name!.Length; i++)
            {
                char c2 = name![i];
                if (c2 == '\0' || (c2 == '.' && c == '.'))
                {
                    return false;
                }
                c = c2;
            }
            return c != '.';
        }

        public static string GetWithSingleAttributeSuffix(this string name, bool isCaseSensitive)
        {
            string text = name;
            while ((text = text.GetWithoutAttributeSuffix(isCaseSensitive)) != null)
            {
                name = text;
            }
            return name + "Attribute";
        }

        public static bool TryGetWithoutAttributeSuffix(this string name, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? result)
        {
            return name.TryGetWithoutAttributeSuffix(isCaseSensitive: true, out result);
        }

        public static string? GetWithoutAttributeSuffix(this string name, bool isCaseSensitive)
        {
            if (!name.TryGetWithoutAttributeSuffix(isCaseSensitive, out var result))
            {
                return null;
            }
            return result;
        }

        public static bool TryGetWithoutAttributeSuffix(this string name, bool isCaseSensitive, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? result)
        {
            if (name.HasAttributeSuffix(isCaseSensitive))
            {
                result = name.Substring(0, name.Length - "Attribute".Length);
                return true;
            }
            result = null;
            return false;
        }

        public static bool HasAttributeSuffix(this string name, bool isCaseSensitive)
        {
            StringComparison comparisonType = (isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            if (name.Length > "Attribute".Length)
            {
                return name.EndsWith("Attribute", comparisonType);
            }
            return false;
        }

        public static bool IsValidUnicodeString(this string str)
        {
            int num = 0;
            while (num < str.Length)
            {
                char c = str[num++];
                if (char.IsHighSurrogate(c))
                {
                    if (num >= str.Length || !char.IsLowSurrogate(str[num]))
                    {
                        return false;
                    }
                    num++;
                }
                else if (char.IsLowSurrogate(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static string Unquote(this string arg)
        {
            return arg.Unquote(out bool quoted);
        }

        public static string Unquote(this string arg, out bool quoted)
        {
            if (arg.Length > 1 && arg[0] == '"' && arg[arg.Length - 1] == '"')
            {
                quoted = true;
                return arg.Substring(1, arg.Length - 2);
            }
            quoted = false;
            return arg;
        }

        public static int IndexOfBalancedParenthesis(this string str, int openingOffset, char closing)
        {
            char c = str[openingOffset];
            int num = 1;
            for (int i = openingOffset + 1; i < str.Length; i++)
            {
                char c2 = str[i];
                if (c2 == c)
                {
                    num++;
                }
                else if (c2 == closing)
                {
                    num--;
                    if (num == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static char First(this string arg)
        {
            return arg[0];
        }

        public static char Last(this string arg)
        {
            return arg[arg.Length - 1];
        }

        public static bool All(this string arg, Predicate<char> predicate)
        {
            foreach (char obj in arg)
            {
                if (!predicate(obj))
                {
                    return false;
                }
            }
            return true;
        }

        public static int GetCaseInsensitivePrefixLength(this string string1, string string2)
        {
            int i;
            for (i = 0; i < string1.Length && i < string2.Length && char.ToUpper(string1[i]) == char.ToUpper(string2[i]); i++)
            {
            }
            return i;
        }

        public static int GetCaseSensitivePrefixLength(this string string1, string string2)
        {
            int i;
            for (i = 0; i < string1.Length && i < string2.Length && string1[i] == string2[i]; i++)
            {
            }
            return i;
        }
    }
}
