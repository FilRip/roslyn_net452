using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTypeAsValueExpression : BoundExpression
	{
		private readonly BoundTypeExpression _Expression;

		public BoundTypeExpression Expression => _Expression;

		public BoundTypeAsValueExpression(SyntaxNode syntax, BoundTypeExpression expression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.TypeAsValueExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTypeAsValueExpression(this);
		}

		public BoundTypeAsValueExpression Update(BoundTypeExpression expression, TypeSymbol type)
		{
			if (expression != Expression || (object)type != base.Type)
			{
				BoundTypeAsValueExpression boundTypeAsValueExpression = new BoundTypeAsValueExpression(base.Syntax, expression, type, base.HasErrors);
				boundTypeAsValueExpression.CopyAttributes(this);
				return boundTypeAsValueExpression;
			}
			return this;
		}
	}
}
