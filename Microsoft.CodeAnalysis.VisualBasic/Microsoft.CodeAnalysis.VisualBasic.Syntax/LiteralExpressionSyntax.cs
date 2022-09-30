using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class LiteralExpressionSyntax : ExpressionSyntax
	{
		public SyntaxToken Token => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)base.Green)._token, base.Position, 0);

		internal LiteralExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal LiteralExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken token)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(kind, errors, annotations, token), null, 0)
		{
		}

		public LiteralExpressionSyntax WithToken(SyntaxToken token)
		{
			return Update(Kind(), token);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitLiteralExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitLiteralExpression(this);
		}

		public LiteralExpressionSyntax Update(SyntaxKind kind, SyntaxToken token)
		{
			if (kind != Kind() || token != Token)
			{
				LiteralExpressionSyntax literalExpressionSyntax = SyntaxFactory.LiteralExpression(kind, token);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(literalExpressionSyntax, annotations);
				}
				return literalExpressionSyntax;
			}
			return this;
		}
	}
}
