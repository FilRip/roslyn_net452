using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ExternalSourceDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken ExternalSourceKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)base.Green)._externalSourceKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)base.Green)._openParenToken, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken ExternalSource => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)base.Green)._externalSource, GetChildPosition(3), GetChildIndex(3));

		public SyntaxToken CommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)base.Green)._commaToken, GetChildPosition(4), GetChildIndex(4));

		public SyntaxToken LineStart => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)base.Green)._lineStart, GetChildPosition(5), GetChildIndex(5));

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)base.Green)._closeParenToken, GetChildPosition(6), GetChildIndex(6));

		internal ExternalSourceDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax externalSourceKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax commaToken, IntegerLiteralTokenSyntax lineStart, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(kind, errors, annotations, hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken), null, 0)
		{
		}

		internal override SyntaxToken GetHashTokenCore()
		{
			return HashToken;
		}

		internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
		{
			return WithHashToken(hashToken);
		}

		public new ExternalSourceDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, ExternalSourceKeyword, OpenParenToken, ExternalSource, CommaToken, LineStart, CloseParenToken);
		}

		public ExternalSourceDirectiveTriviaSyntax WithExternalSourceKeyword(SyntaxToken externalSourceKeyword)
		{
			return Update(HashToken, externalSourceKeyword, OpenParenToken, ExternalSource, CommaToken, LineStart, CloseParenToken);
		}

		public ExternalSourceDirectiveTriviaSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(HashToken, ExternalSourceKeyword, openParenToken, ExternalSource, CommaToken, LineStart, CloseParenToken);
		}

		public ExternalSourceDirectiveTriviaSyntax WithExternalSource(SyntaxToken externalSource)
		{
			return Update(HashToken, ExternalSourceKeyword, OpenParenToken, externalSource, CommaToken, LineStart, CloseParenToken);
		}

		public ExternalSourceDirectiveTriviaSyntax WithCommaToken(SyntaxToken commaToken)
		{
			return Update(HashToken, ExternalSourceKeyword, OpenParenToken, ExternalSource, commaToken, LineStart, CloseParenToken);
		}

		public ExternalSourceDirectiveTriviaSyntax WithLineStart(SyntaxToken lineStart)
		{
			return Update(HashToken, ExternalSourceKeyword, OpenParenToken, ExternalSource, CommaToken, lineStart, CloseParenToken);
		}

		public ExternalSourceDirectiveTriviaSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(HashToken, ExternalSourceKeyword, OpenParenToken, ExternalSource, CommaToken, LineStart, closeParenToken);
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
			return visitor.VisitExternalSourceDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitExternalSourceDirectiveTrivia(this);
		}

		public ExternalSourceDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken externalSourceKeyword, SyntaxToken openParenToken, SyntaxToken externalSource, SyntaxToken commaToken, SyntaxToken lineStart, SyntaxToken closeParenToken)
		{
			if (hashToken != HashToken || externalSourceKeyword != ExternalSourceKeyword || openParenToken != OpenParenToken || externalSource != ExternalSource || commaToken != CommaToken || lineStart != LineStart || closeParenToken != CloseParenToken)
			{
				ExternalSourceDirectiveTriviaSyntax externalSourceDirectiveTriviaSyntax = SyntaxFactory.ExternalSourceDirectiveTrivia(hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(externalSourceDirectiveTriviaSyntax, annotations);
				}
				return externalSourceDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
