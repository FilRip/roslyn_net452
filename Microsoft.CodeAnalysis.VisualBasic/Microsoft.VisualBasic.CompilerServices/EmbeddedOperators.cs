using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualBasic.CompilerServices
{
	[Embedded]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class EmbeddedOperators
	{
		private EmbeddedOperators()
		{
		}

		public static int CompareString(string Left, string Right, bool TextCompare)
		{
			if ((object)Left == Right)
			{
				return 0;
			}
			if (Left == null)
			{
				if (Right.Length == 0)
				{
					return 0;
				}
				return -1;
			}
			if (Right == null)
			{
				if (Left.Length == 0)
				{
					return 0;
				}
				return 1;
			}
			int num;
			if (TextCompare)
			{
				CompareOptions options = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
				num = Conversions.GetCultureInfo().CompareInfo.Compare(Left, Right, options);
			}
			else
			{
				num = string.CompareOrdinal(Left, Right);
			}
			if (num == 0)
			{
				return 0;
			}
			if (num > 0)
			{
				return 1;
			}
			return -1;
		}
	}
}
