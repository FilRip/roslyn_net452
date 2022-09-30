using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class UnaryExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _operand;

		public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)base.Green)._operatorToken, base.Position, 0);

		public ExpressionSyntax Operand => GetRed(ref _operand, 1);

		internal UnaryExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal UnaryExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken operatorToken, ExpressionSyntax operand)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(kind, errors, annotations, operatorToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)operand.Green), null, 0)
		{
		}

		public UnaryExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
		{
			return Update(Kind(), operatorToken, Operand);
		}

		public UnaryExpressionSyntax WithOperand(ExpressionSyntax operand)
		{
			return Update(Kind(), OperatorToken, operand);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _operand;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Operand;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitUnaryExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitUnaryExpression(this);
		}

		public UnaryExpressionSyntax Update(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			if (kind != Kind() || operatorToken != OperatorToken || operand != Operand)
			{
				UnaryExpressionSyntax unaryExpressionSyntax = SyntaxFactory.UnaryExpression(kind, operatorToken, operand);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(unaryExpressionSyntax, annotations);
				}
				return unaryExpressionSyntax;
			}
			return this;
		}
	}
}
