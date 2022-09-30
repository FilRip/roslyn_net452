using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class OptionStrictEnumBounds
	{
		internal static bool IsValid(this OptionStrict value)
		{
			if (value >= OptionStrict.Off)
			{
				return value <= OptionStrict.On;
			}
			return false;
		}
	}
}
