using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay
{
	[StandardModule]
	internal sealed class ObjectDisplay
	{
		private const char s_nullChar = '\0';

		private const char s_back = '\b';

		private const char s_Cr = '\r';

		private const char s_formFeed = '\f';

		private const char s_Lf = '\n';

		private const char s_tab = '\t';

		private const char s_verticalTab = '\v';

		internal static string NullLiteral => "Nothing";

		public static string FormatPrimitive(object obj, ObjectDisplayOptions options)
		{
			if (obj == null)
			{
				return NullLiteral;
			}
			Type type = obj.GetType();
			if (type.GetTypeInfo().IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if ((object)type == typeof(int))
			{
				return FormatLiteral((int)obj, options);
			}
			if ((object)type == typeof(string))
			{
				return FormatLiteral((string)obj, options);
			}
			if ((object)type == typeof(bool))
			{
				return FormatLiteral((bool)obj);
			}
			if ((object)type == typeof(char))
			{
				return FormatLiteral((char)obj, options);
			}
			if ((object)type == typeof(byte))
			{
				return FormatLiteral((byte)obj, options);
			}
			if ((object)type == typeof(short))
			{
				return FormatLiteral((short)obj, options);
			}
			if ((object)type == typeof(long))
			{
				return FormatLiteral((long)obj, options);
			}
			if ((object)type == typeof(double))
			{
				return FormatLiteral((double)obj, options);
			}
			if ((object)type == typeof(ulong))
			{
				return FormatLiteral((ulong)obj, options);
			}
			if ((object)type == typeof(uint))
			{
				return FormatLiteral((uint)obj, options);
			}
			if ((object)type == typeof(ushort))
			{
				return FormatLiteral((ushort)obj, options);
			}
			if ((object)type == typeof(sbyte))
			{
				return FormatLiteral((sbyte)obj, options);
			}
			if ((object)type == typeof(float))
			{
				return FormatLiteral((float)obj, options);
			}
			if ((object)type == typeof(decimal))
			{
				return FormatLiteral((decimal)obj, options);
			}
			if ((object)type == typeof(DateTime))
			{
				return FormatLiteral((DateTime)obj);
			}
			return null;
		}

		internal static string FormatLiteral(bool value)
		{
			if (!value)
			{
				return "False";
			}
			return "True";
		}

		internal static string FormatLiteral(string value, ObjectDisplayOptions options)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			foreach (int item in TokenizeString(value, options))
			{
				builder.Append(Strings.ChrW(item & 0xFFFF));
			}
			return instance.ToStringAndFree();
		}

		internal static string FormatLiteral(char c, ObjectDisplayOptions options)
		{
			if (IsPrintable(c) || !options.IncludesOption(ObjectDisplayOptions.EscapeNonPrintableCharacters))
			{
				return options.IncludesOption(ObjectDisplayOptions.UseQuotes) ? ("\"" + EscapeQuote(c) + "\"c") : c.ToString();
			}
			string wellKnownCharacterName = GetWellKnownCharacterName(c);
			if (wellKnownCharacterName != null)
			{
				return wellKnownCharacterName;
			}
			int num = c;
			return (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers) ? ("ChrW(&H" + num.ToString("X")) : ("ChrW(" + num)) + ")";
		}

		private static string EscapeQuote(char c)
		{
			if (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(c), "\"", TextCompare: false) != 0)
			{
				return Microsoft.VisualBasic.CompilerServices.Conversions.ToString(c);
			}
			return "\"\"";
		}

		internal static string FormatLiteral(sbyte value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				string text;
				if (value < 0)
				{
					int num = value;
					text = num.ToString("X8");
				}
				else
				{
					text = value.ToString("X2");
				}
				return "&H" + text;
			}
			return value.ToString(GetFormatCulture(cultureInfo));
		}

		internal static string FormatLiteral(byte value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				return "&H" + value.ToString("X2");
			}
			return value.ToString(GetFormatCulture(cultureInfo));
		}

		internal static string FormatLiteral(short value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				builder.Append("&H");
				string value2;
				if (value < 0)
				{
					int num = value;
					value2 = num.ToString("X8");
				}
				else
				{
					value2 = value.ToString("X4");
				}
				builder.Append(value2);
			}
			else
			{
				builder.Append(value.ToString(GetFormatCulture(cultureInfo)));
			}
			if (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				builder.Append('S');
			}
			return instance.ToStringAndFree();
		}

		internal static string FormatLiteral(ushort value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				builder.Append("&H");
				builder.Append(value.ToString("X4"));
			}
			else
			{
				builder.Append(value.ToString(GetFormatCulture(cultureInfo)));
			}
			if (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				builder.Append("US");
			}
			return instance.ToStringAndFree();
		}

		internal static string FormatLiteral(int value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				builder.Append("&H");
				builder.Append(value.ToString("X8"));
			}
			else
			{
				builder.Append(value.ToString(GetFormatCulture(cultureInfo)));
			}
			if (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				builder.Append('I');
			}
			return instance.ToStringAndFree();
		}

		internal static string FormatLiteral(uint value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				builder.Append("&H");
				builder.Append(value.ToString("X8"));
			}
			else
			{
				builder.Append(value.ToString(GetFormatCulture(cultureInfo)));
			}
			if (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				builder.Append("UI");
			}
			return instance.ToStringAndFree();
		}

		internal static string FormatLiteral(long value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				builder.Append("&H");
				builder.Append(value.ToString("X16"));
			}
			else
			{
				builder.Append(value.ToString(GetFormatCulture(cultureInfo)));
			}
			if (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				builder.Append('L');
			}
			return instance.ToStringAndFree();
		}

		internal static string FormatLiteral(ulong value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			if (options.IncludesOption(ObjectDisplayOptions.UseHexadecimalNumbers))
			{
				builder.Append("&H");
				builder.Append(value.ToString("X16"));
			}
			else
			{
				builder.Append(value.ToString(GetFormatCulture(cultureInfo)));
			}
			if (options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				builder.Append("UL");
			}
			return instance.ToStringAndFree();
		}

		internal static string FormatLiteral(double value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			string text = value.ToString("R", GetFormatCulture(cultureInfo));
			if (!options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				return text;
			}
			return text + "R";
		}

		internal static string FormatLiteral(float value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			string text = value.ToString("R", GetFormatCulture(cultureInfo));
			if (!options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				return text;
			}
			return text + "F";
		}

		internal static string FormatLiteral(decimal value, ObjectDisplayOptions options, CultureInfo cultureInfo = null)
		{
			string text = value.ToString(GetFormatCulture(cultureInfo));
			if (!options.IncludesOption(ObjectDisplayOptions.IncludeTypeSuffix))
			{
				return text;
			}
			return text + "D";
		}

		internal static string FormatLiteral(DateTime value)
		{
			return value.ToString("#M/d/yyyy hh:mm:ss tt#", CultureInfo.InvariantCulture);
		}

		private static int Character(char c)
		{
			return 0xD0000 | c;
		}

		private static int Identifier(char c)
		{
			return 0xF0000 | c;
		}

		private static int Number(char c)
		{
			return 0xC0000 | c;
		}

		private static int Punctuation(char c)
		{
			return 0x150000 | c;
		}

		private static int Operator(char c)
		{
			return 0x120000 | c;
		}

		private static int Space()
		{
			return 1441824;
		}

		private static int Quotes()
		{
			return 852002;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_33_TokenizeString))]
		internal static IEnumerable<int> TokenizeString(string str, ObjectDisplayOptions options)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_33_TokenizeString(-2)
			{
				_0024P_str = str,
				_0024P_options = options
			};
		}

		internal static bool IsPrintable(char c)
		{
			return IsPrintable(CharUnicodeInfo.GetUnicodeCategory(c));
		}

		private static bool IsPrintable(UnicodeCategory category)
		{
			if ((uint)(category - 12) <= 2u || category == UnicodeCategory.Surrogate || category == UnicodeCategory.OtherNotAssigned)
			{
				return false;
			}
			return true;
		}

		internal static string GetWellKnownCharacterName(char c)
		{
			return c switch
			{
				'\0' => "vbNullChar", 
				'\b' => "vbBack", 
				'\r' => "vbCr", 
				'\f' => "vbFormFeed", 
				'\n' => "vbLf", 
				'\t' => "vbTab", 
				'\v' => "vbVerticalTab", 
				_ => null, 
			};
		}

		private static CultureInfo GetFormatCulture(CultureInfo cultureInfo)
		{
			return cultureInfo ?? CultureInfo.InvariantCulture;
		}

		[Conditional("DEBUG")]
		private static void ValidateOptions(ObjectDisplayOptions options)
		{
		}
	}
}
