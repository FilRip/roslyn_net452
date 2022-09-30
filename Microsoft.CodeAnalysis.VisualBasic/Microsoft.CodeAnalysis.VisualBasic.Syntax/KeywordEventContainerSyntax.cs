using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class KeywordEventContainerSyntax : EventContainerSyntax
	{
		public SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax)base.Green)._keyword, base.Position, 0);

		internal KeywordEventContainerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal KeywordEventContainerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax(kind, errors, annotations, keyword), null, 0)
		{
		}

		public KeywordEventContainerSyntax WithKeyword(SyntaxToken keyword)
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
			return visitor.VisitKeywordEventContainer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitKeywordEventContainer(this);
		}

		public KeywordEventContainerSyntax Update(SyntaxToken keyword)
		{
			if (keyword != Keyword)
			{
				KeywordEventContainerSyntax keywordEventContainerSyntax = SyntaxFactory.KeywordEventContainer(keyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(keywordEventContainerSyntax, annotations);
				}
				return keywordEventContainerSyntax;
			}
			return this;
		}
	}
}
