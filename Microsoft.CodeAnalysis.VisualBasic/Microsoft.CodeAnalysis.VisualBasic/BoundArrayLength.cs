using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundArrayLength : BoundExpression
	{
		private readonly BoundExpression _Expression;

		public BoundExpression Expression => _Expression;

		public BoundArrayLength(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ArrayLength, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitArrayLength(this);
		}

		public BoundArrayLength Update(BoundExpression expression, TypeSymbol type)
		{
			if (expression != Expression || (object)type != base.Type)
			{
				BoundArrayLength boundArrayLength = new BoundArrayLength(base.Syntax, expression, type, base.HasErrors);
				boundArrayLength.CopyAttributes(this);
				return boundArrayLength;
			}
			return this;
		}
	}
}
