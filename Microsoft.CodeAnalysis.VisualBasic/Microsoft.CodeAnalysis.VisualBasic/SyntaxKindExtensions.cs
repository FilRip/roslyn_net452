using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class SyntaxKindExtensions
	{
		public static bool Contains(this SyntaxKind[] kinds, SyntaxKind kind)
		{
			for (int i = 0; i < kinds.Length; i = checked(i + 1))
			{
				if (kinds[i] == kind)
				{
					return true;
				}
			}
			return false;
		}
	}
}
