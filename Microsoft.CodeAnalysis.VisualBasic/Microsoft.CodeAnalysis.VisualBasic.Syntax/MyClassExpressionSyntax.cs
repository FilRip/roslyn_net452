using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MyClassExpressionSyntax : InstanceExpressionSyntax
	{
		public new SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax)base.Green)._keyword, base.Position, 0);

		internal MyClassExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MyClassExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax(kind, errors, annotations, keyword), null, 0)
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

		public new MyClassExpressionSyntax WithKeyword(SyntaxToken keyword)
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
			return visitor.VisitMyClassExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMyClassExpression(this);
		}

		public MyClassExpressionSyntax Update(SyntaxToken keyword)
		{
			if (keyword != Keyword)
			{
				MyClassExpressionSyntax myClassExpressionSyntax = SyntaxFactory.MyClassExpression(keyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(myClassExpressionSyntax, annotations);
				}
				return myClassExpressionSyntax;
			}
			return this;
		}
	}
}
