using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic
{
	[StandardModule]
	[Embedded]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[StandardModule]
	internal sealed class Strings
	{
		public static char ChrW(int CharCode)
		{
			if (CharCode < -32768 || CharCode > 65535)
			{
				throw new ArgumentException();
			}
			return Convert.ToChar(CharCode & 0xFFFF);
		}

		public static int AscW(string String)
		{
			if (String == null || String.Length == 0)
			{
				throw new ArgumentException();
			}
			return String[0];
		}
	}
}
