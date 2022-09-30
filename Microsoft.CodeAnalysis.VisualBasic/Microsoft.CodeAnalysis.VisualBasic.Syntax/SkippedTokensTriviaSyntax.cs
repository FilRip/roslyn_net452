using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SkippedTokensTriviaSyntax : StructuredTriviaSyntax, ISkippedTokensTriviaSyntax
	{
		public SyntaxTokenList Tokens
		{
			get
			{
				GreenNode tokens = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax)base.Green)._tokens;
				return (tokens == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, tokens, base.Position, 0);
			}
		}

		private SyntaxTokenList ISkippedTokensTriviaSyntax_Tokens => Tokens;

		internal SkippedTokensTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SkippedTokensTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode tokens)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax(kind, errors, annotations, tokens), null, 0)
		{
		}

		public SkippedTokensTriviaSyntax WithTokens(SyntaxTokenList tokens)
		{
			return Update(tokens);
		}

		public SkippedTokensTriviaSyntax AddTokens(params SyntaxToken[] items)
		{
			return WithTokens(Tokens.AddRange(items));
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
			return visitor.VisitSkippedTokensTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSkippedTokensTrivia(this);
		}

		public SkippedTokensTriviaSyntax Update(SyntaxTokenList tokens)
		{
			if (tokens != Tokens)
			{
				SkippedTokensTriviaSyntax skippedTokensTriviaSyntax = SyntaxFactory.SkippedTokensTrivia(tokens);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(skippedTokensTriviaSyntax, annotations);
				}
				return skippedTokensTriviaSyntax;
			}
			return this;
		}
	}
}
