using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundTupleExpression : BoundExpression
	{
		private readonly ImmutableArray<BoundExpression> _Arguments;

		public ImmutableArray<BoundExpression> Arguments => _Arguments;

		protected BoundTupleExpression(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
			: base(kind, syntax, type, hasErrors)
		{
			_Arguments = arguments;
		}
	}
}
