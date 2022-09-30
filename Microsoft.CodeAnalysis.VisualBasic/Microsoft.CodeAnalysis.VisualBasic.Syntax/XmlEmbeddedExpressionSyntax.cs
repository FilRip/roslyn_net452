using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlEmbeddedExpressionSyntax : XmlNodeSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken LessThanPercentEqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)base.Green)._lessThanPercentEqualsToken, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		public SyntaxToken PercentGreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)base.Green)._percentGreaterThanToken, GetChildPosition(2), GetChildIndex(2));

		internal XmlEmbeddedExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlEmbeddedExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanPercentEqualsToken, ExpressionSyntax expression, PunctuationSyntax percentGreaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax(kind, errors, annotations, lessThanPercentEqualsToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, percentGreaterThanToken), null, 0)
		{
		}

		public XmlEmbeddedExpressionSyntax WithLessThanPercentEqualsToken(SyntaxToken lessThanPercentEqualsToken)
		{
			return Update(lessThanPercentEqualsToken, Expression, PercentGreaterThanToken);
		}

		public XmlEmbeddedExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(LessThanPercentEqualsToken, expression, PercentGreaterThanToken);
		}

		public XmlEmbeddedExpressionSyntax WithPercentGreaterThanToken(SyntaxToken percentGreaterThanToken)
		{
			return Update(LessThanPercentEqualsToken, Expression, percentGreaterThanToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlEmbeddedExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlEmbeddedExpression(this);
		}

		public XmlEmbeddedExpressionSyntax Update(SyntaxToken lessThanPercentEqualsToken, ExpressionSyntax expression, SyntaxToken percentGreaterThanToken)
		{
			if (lessThanPercentEqualsToken != LessThanPercentEqualsToken || expression != Expression || percentGreaterThanToken != PercentGreaterThanToken)
			{
				XmlEmbeddedExpressionSyntax xmlEmbeddedExpressionSyntax = SyntaxFactory.XmlEmbeddedExpression(lessThanPercentEqualsToken, expression, percentGreaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlEmbeddedExpressionSyntax, annotations);
				}
				return xmlEmbeddedExpressionSyntax;
			}
			return this;
		}
	}
}
