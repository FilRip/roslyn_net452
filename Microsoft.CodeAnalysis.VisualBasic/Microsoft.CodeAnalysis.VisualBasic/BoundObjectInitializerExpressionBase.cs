using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundObjectInitializerExpressionBase : BoundExpression
	{
		private readonly BoundWithLValueExpressionPlaceholder _PlaceholderOpt;

		private readonly ImmutableArray<BoundExpression> _Initializers;

		public BoundWithLValueExpressionPlaceholder PlaceholderOpt => _PlaceholderOpt;

		public ImmutableArray<BoundExpression> Initializers => _Initializers;

		protected BoundObjectInitializerExpressionBase(BoundKind kind, SyntaxNode syntax, BoundWithLValueExpressionPlaceholder placeholderOpt, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
			: base(kind, syntax, type, hasErrors)
		{
			_PlaceholderOpt = placeholderOpt;
			_Initializers = initializers;
		}
	}
}
