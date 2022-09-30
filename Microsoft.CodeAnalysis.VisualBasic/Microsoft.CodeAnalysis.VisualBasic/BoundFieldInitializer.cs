using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundFieldInitializer : BoundFieldOrPropertyInitializer
	{
		private readonly ImmutableArray<FieldSymbol> _InitializedFields;

		public ImmutableArray<FieldSymbol> InitializedFields => _InitializedFields;

		public BoundFieldInitializer(SyntaxNode syntax, ImmutableArray<FieldSymbol> initializedFields, BoundExpression memberAccessExpressionOpt, BoundExpression initialValue, bool hasErrors = false)
			: base(BoundKind.FieldInitializer, syntax, memberAccessExpressionOpt, initialValue, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(memberAccessExpressionOpt) || BoundNodeExtensions.NonNullAndHasErrors(initialValue))
		{
			_InitializedFields = initializedFields;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitFieldInitializer(this);
		}

		public BoundFieldInitializer Update(ImmutableArray<FieldSymbol> initializedFields, BoundExpression memberAccessExpressionOpt, BoundExpression initialValue)
		{
			if (initializedFields != InitializedFields || memberAccessExpressionOpt != base.MemberAccessExpressionOpt || initialValue != base.InitialValue)
			{
				BoundFieldInitializer boundFieldInitializer = new BoundFieldInitializer(base.Syntax, initializedFields, memberAccessExpressionOpt, initialValue, base.HasErrors);
				boundFieldInitializer.CopyAttributes(this);
				return boundFieldInitializer;
			}
			return this;
		}
	}
}
