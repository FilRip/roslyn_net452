using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlEmbeddedExpression : BoundExpression
	{
		private readonly BoundExpression _Expression;

		public BoundExpression Expression => _Expression;

		public BoundXmlEmbeddedExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlEmbeddedExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlEmbeddedExpression(this);
		}

		public BoundXmlEmbeddedExpression Update(BoundExpression expression, TypeSymbol type)
		{
			if (expression != Expression || (object)type != base.Type)
			{
				BoundXmlEmbeddedExpression boundXmlEmbeddedExpression = new BoundXmlEmbeddedExpression(base.Syntax, expression, type, base.HasErrors);
				boundXmlEmbeddedExpression.CopyAttributes(this);
				return boundXmlEmbeddedExpression;
			}
			return this;
		}
	}
}
