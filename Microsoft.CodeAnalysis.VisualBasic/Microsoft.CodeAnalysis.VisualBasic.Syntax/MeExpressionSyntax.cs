using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MeExpressionSyntax : InstanceExpressionSyntax
	{
		public new SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax)base.Green)._keyword, base.Position, 0);

		internal MeExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MeExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax(kind, errors, annotations, keyword), null, 0)
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

		public new MeExpressionSyntax WithKeyword(SyntaxToken keyword)
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
			return visitor.VisitMeExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMeExpression(this);
		}

		public MeExpressionSyntax Update(SyntaxToken keyword)
		{
			if (keyword != Keyword)
			{
				MeExpressionSyntax meExpressionSyntax = SyntaxFactory.MeExpression(keyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(meExpressionSyntax, annotations);
				}
				return meExpressionSyntax;
			}
			return this;
		}
	}
}
