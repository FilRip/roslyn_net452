using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundPropertyInitializer : BoundFieldOrPropertyInitializer
	{
		private readonly ImmutableArray<PropertySymbol> _InitializedProperties;

		public ImmutableArray<PropertySymbol> InitializedProperties => _InitializedProperties;

		public BoundPropertyInitializer(SyntaxNode syntax, ImmutableArray<PropertySymbol> initializedProperties, BoundExpression memberAccessExpressionOpt, BoundExpression initialValue, bool hasErrors = false)
			: base(BoundKind.PropertyInitializer, syntax, memberAccessExpressionOpt, initialValue, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(memberAccessExpressionOpt) || BoundNodeExtensions.NonNullAndHasErrors(initialValue))
		{
			_InitializedProperties = initializedProperties;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitPropertyInitializer(this);
		}

		public BoundPropertyInitializer Update(ImmutableArray<PropertySymbol> initializedProperties, BoundExpression memberAccessExpressionOpt, BoundExpression initialValue)
		{
			if (initializedProperties != InitializedProperties || memberAccessExpressionOpt != base.MemberAccessExpressionOpt || initialValue != base.InitialValue)
			{
				BoundPropertyInitializer boundPropertyInitializer = new BoundPropertyInitializer(base.Syntax, initializedProperties, memberAccessExpressionOpt, initialValue, base.HasErrors);
				boundPropertyInitializer.CopyAttributes(this);
				return boundPropertyInitializer;
			}
			return this;
		}
	}
}
