using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundRValuePlaceholderBase : BoundValuePlaceholderBase
	{
		protected BoundRValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(kind, syntax, type, hasErrors)
		{
		}

		protected BoundRValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol type)
			: base(kind, syntax, type)
		{
		}
	}
}
