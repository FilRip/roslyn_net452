using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundBadVariable : BoundExpression
	{
		private readonly BoundExpression _Expression;

		private readonly bool _IsLValue;

		public override LookupResultKind ResultKind => LookupResultKind.NotAValue;

		public BoundExpression Expression => _Expression;

		public override bool IsLValue => _IsLValue;

		public BoundBadVariable(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
			: this(syntax, expression, isLValue: true, type, hasErrors)
		{
		}

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundBadVariable MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(_Expression, isLValue: false, base.Type);
			}
			return this;
		}

		public BoundBadVariable(SyntaxNode syntax, BoundExpression expression, bool isLValue, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.BadVariable, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
			_IsLValue = isLValue;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitBadVariable(this);
		}

		public BoundBadVariable Update(BoundExpression expression, bool isLValue, TypeSymbol type)
		{
			if (expression != Expression || isLValue != IsLValue || (object)type != base.Type)
			{
				BoundBadVariable boundBadVariable = new BoundBadVariable(base.Syntax, expression, isLValue, type, base.HasErrors);
				boundBadVariable.CopyAttributes(this);
				return boundBadVariable;
			}
			return this;
		}
	}
}
