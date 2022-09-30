using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class LocationExtensions
	{
		public static EmbeddedSymbolKind EmbeddedKind(this Location location)
		{
			if (location is VBLocation vBLocation)
			{
				return vBLocation.EmbeddedKind;
			}
			return EmbeddedSymbolKind.None;
		}

		public static TextSpan PossiblyEmbeddedOrMySourceSpan(this Location location)
		{
			if (location is VBLocation vBLocation)
			{
				return vBLocation.PossiblyEmbeddedOrMySourceSpan;
			}
			return location.SourceSpan;
		}

		public static SyntaxTree PossiblyEmbeddedOrMySourceTree(this Location location)
		{
			if (location is VBLocation vBLocation)
			{
				return vBLocation.PossiblyEmbeddedOrMySourceTree;
			}
			return (VisualBasicSyntaxTree)location.SourceTree;
		}
	}
}
