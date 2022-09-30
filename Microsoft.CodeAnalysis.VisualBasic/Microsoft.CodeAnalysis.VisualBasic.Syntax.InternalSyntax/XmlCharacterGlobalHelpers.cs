using System;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class XmlCharacterGlobalHelpers
	{
		internal static bool isNameChar(char ch)
		{
			return XmlCharType.IsNameCharXml4e(ch);
		}

		internal static bool isStartNameChar(char ch)
		{
			return XmlCharType.IsStartNameCharXml4e(ch);
		}

		internal static bool isValidUtf16(char wh)
		{
			if (!XmlCharType.InRange(wh, ' ', '\ufffd'))
			{
				return XmlCharType.IsCharData(wh);
			}
			return true;
		}

		internal static Scanner.XmlCharResult HexToUTF16(StringBuilder pwcText)
		{
			uint pulCode = default(uint);
			if (TryHexToUnicode(pwcText, ref pulCode) && ValidateXmlChar(pulCode))
			{
				return UnicodeToUTF16(pulCode);
			}
			return default(Scanner.XmlCharResult);
		}

		internal static bool TryHexToUnicode(StringBuilder pwcText, ref uint pulCode)
		{
			uint num = 0u;
			int num2 = pwcText.Length - 1;
			for (int i = 0; i <= num2; i++)
			{
				char c = pwcText[i];
				if (XmlCharType.InRange(c, '0', '9'))
				{
					num = num * 16 + c - 48;
				}
				else if (XmlCharType.InRange(c, 'a', 'f'))
				{
					num = num * 16 + 10 + c - 97;
				}
				else
				{
					if (!XmlCharType.InRange(c, 'A', 'F'))
					{
						return false;
					}
					num = num * 16 + 10 + c - 65;
				}
				if ((long)num > 1114111L)
				{
					return false;
				}
			}
			pulCode = num;
			return true;
		}

		internal static Scanner.XmlCharResult DecToUTF16(StringBuilder pwcText)
		{
			ushort pulCode = default(ushort);
			if (TryDecToUnicode(pwcText, ref pulCode) && ValidateXmlChar(pulCode))
			{
				return UnicodeToUTF16(pulCode);
			}
			return default(Scanner.XmlCharResult);
		}

		internal static bool TryDecToUnicode(StringBuilder pwcText, ref ushort pulCode)
		{
			int num = 0;
			int num2 = pwcText.Length - 1;
			for (int i = 0; i <= num2; i++)
			{
				char c = pwcText[i];
				if (XmlCharType.InRange(c, '0', '9'))
				{
					num = num * 10 + c - 48;
					if (num > 1114111)
					{
						return false;
					}
					continue;
				}
				return false;
			}
			pulCode = (ushort)num;
			return true;
		}

		private static bool ValidateXmlChar(uint ulCode)
		{
			if (((long)ulCode < 55296L && ((long)ulCode > 31L || XmlCharType.IsWhiteSpace(Convert.ToChar(ulCode)))) || ((long)ulCode < 65534L && (long)ulCode > 57343L) || ((long)ulCode < 1114112L && (long)ulCode > 65535L))
			{
				return true;
			}
			return false;
		}

		private static Scanner.XmlCharResult UnicodeToUTF16(uint ulCode)
		{
			return ((long)ulCode <= 65535L) ? new Scanner.XmlCharResult(Convert.ToChar(ulCode)) : new Scanner.XmlCharResult(Convert.ToChar(55232 + (ulCode >> 10)), Convert.ToChar(0xDC00u | (ulCode & 0x3FFu)));
		}

		internal static int UTF16ToUnicode(Scanner.XmlCharResult ch)
		{
			return ch.Length switch
			{
				1 => Convert.ToInt32(ch.Char1), 
				2 => Convert.ToInt32(ch.Char1) - 55296 << 10 + (Convert.ToInt32(ch.Char2) - 56320) + 65536, 
				_ => 0, 
			};
		}
	}
}
