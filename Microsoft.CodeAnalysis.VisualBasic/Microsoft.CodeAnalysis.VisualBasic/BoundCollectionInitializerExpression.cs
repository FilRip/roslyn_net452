using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundCollectionInitializerExpression : BoundObjectInitializerExpressionBase
	{
		public BoundCollectionInitializerExpression(SyntaxNode syntax, BoundWithLValueExpressionPlaceholder placeholderOpt, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.CollectionInitializerExpression, syntax, placeholderOpt, initializers, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(placeholderOpt) || BoundNodeExtensions.NonNullAndHasErrors(initializers))
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitCollectionInitializerExpression(this);
		}

		public BoundCollectionInitializerExpression Update(BoundWithLValueExpressionPlaceholder placeholderOpt, ImmutableArray<BoundExpression> initializers, TypeSymbol type)
		{
			if (placeholderOpt != base.PlaceholderOpt || initializers != base.Initializers || (object)type != base.Type)
			{
				BoundCollectionInitializerExpression boundCollectionInitializerExpression = new BoundCollectionInitializerExpression(base.Syntax, placeholderOpt, initializers, type, base.HasErrors);
				boundCollectionInitializerExpression.CopyAttributes(this);
				return boundCollectionInitializerExpression;
			}
			return this;
		}
	}
}
