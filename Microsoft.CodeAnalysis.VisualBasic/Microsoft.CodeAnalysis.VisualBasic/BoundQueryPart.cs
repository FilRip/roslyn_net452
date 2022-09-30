using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundQueryPart : BoundExpression
	{
		protected abstract override ImmutableArray<BoundNode> Children { get; }

		protected BoundQueryPart(BoundKind kind, SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(kind, syntax, type, hasErrors)
		{
		}

		protected BoundQueryPart(BoundKind kind, SyntaxNode syntax, TypeSymbol type)
			: base(kind, syntax, type)
		{
		}
	}
}
