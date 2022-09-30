using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MyBaseExpressionSyntax : InstanceExpressionSyntax
	{
		public new SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax)base.Green)._keyword, base.Position, 0);

		internal MyBaseExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MyBaseExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax(kind, errors, annotations, keyword), null, 0)
		{
		}

		internal override SyntaxToken GetKeywordCore()
		{
			return Keyword;
		}

		internal override InstanceExpressionSyntax WithKeywordCore(SyntaxToken keyword)
		{
			return WithKeyword(keyword);
		}

		public new MyBaseExpressionSyntax WithKeyword(SyntaxToken keyword)
		{
			return Update(keyword);
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
			return visitor.VisitMyBaseExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMyBaseExpression(this);
		}

		public MyBaseExpressionSyntax Update(SyntaxToken keyword)
		{
			if (keyword != Keyword)
			{
				MyBaseExpressionSyntax myBaseExpressionSyntax = SyntaxFactory.MyBaseExpression(keyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(myBaseExpressionSyntax, annotations);
				}
				return myBaseExpressionSyntax;
			}
			return this;
		}
	}
}
