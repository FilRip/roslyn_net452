using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	public sealed class GeneratedExtensionSyntaxFacts
	{
		public static string GetText(this SyntaxKind kind)
		{
			return SyntaxFacts.GetText(kind);
		}
	}
}
