using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundArrayAccess : BoundExpression
	{
		private readonly BoundExpression _Expression;

		private readonly ImmutableArray<BoundExpression> _Indices;

		private readonly bool _IsLValue;

		public BoundExpression Expression => _Expression;

		public ImmutableArray<BoundExpression> Indices => _Indices;

		public override bool IsLValue => _IsLValue;

		public BoundArrayAccess(SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundExpression> indices, TypeSymbol type, bool hasErrors = false)
			: this(syntax, expression, indices, isLValue: true, type, hasErrors)
		{
		}

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundArrayAccess MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(_Expression, _Indices, isLValue: false, base.Type);
			}
			return this;
		}

		public BoundArrayAccess(SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundExpression> indices, bool isLValue, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ArrayAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression) || BoundNodeExtensions.NonNullAndHasErrors(indices))
		{
			_Expression = expression;
			_Indices = indices;
			_IsLValue = isLValue;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitArrayAccess(this);
		}

		public BoundArrayAccess Update(BoundExpression expression, ImmutableArray<BoundExpression> indices, bool isLValue, TypeSymbol type)
		{
			if (expression != Expression || indices != Indices || isLValue != IsLValue || (object)type != base.Type)
			{
				BoundArrayAccess boundArrayAccess = new BoundArrayAccess(base.Syntax, expression, indices, isLValue, type, base.HasErrors);
				boundArrayAccess.CopyAttributes(this);
				return boundArrayAccess;
			}
			return this;
		}
	}
}
