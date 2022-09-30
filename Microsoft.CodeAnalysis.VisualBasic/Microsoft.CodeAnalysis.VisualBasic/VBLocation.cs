using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class VBLocation : Location
	{
		internal virtual EmbeddedSymbolKind EmbeddedKind => EmbeddedSymbolKind.None;

		internal virtual TextSpan PossiblyEmbeddedOrMySourceSpan => SourceSpan;

		internal virtual SyntaxTree PossiblyEmbeddedOrMySourceTree => (VisualBasicSyntaxTree)SourceTree;
	}
}
