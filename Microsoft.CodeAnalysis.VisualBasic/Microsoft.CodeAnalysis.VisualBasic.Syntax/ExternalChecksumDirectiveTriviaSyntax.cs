using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ExternalChecksumDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken ExternalChecksumKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._externalChecksumKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._openParenToken, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken ExternalSource => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._externalSource, GetChildPosition(3), GetChildIndex(3));

		public SyntaxToken FirstCommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._firstCommaToken, GetChildPosition(4), GetChildIndex(4));

		public SyntaxToken Guid => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._guid, GetChildPosition(5), GetChildIndex(5));

		public SyntaxToken SecondCommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._secondCommaToken, GetChildPosition(6), GetChildIndex(6));

		public SyntaxToken Checksum => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._checksum, GetChildPosition(7), GetChildIndex(7));

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)base.Green)._closeParenToken, GetChildPosition(8), GetChildIndex(8));

		internal ExternalChecksumDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ExternalChecksumDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax externalChecksumKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax firstCommaToken, StringLiteralTokenSyntax guid, PunctuationSyntax secondCommaToken, StringLiteralTokenSyntax checksum, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(kind, errors, annotations, hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken), null, 0)
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

		public new ExternalChecksumDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, ExternalChecksumKeyword, OpenParenToken, ExternalSource, FirstCommaToken, Guid, SecondCommaToken, Checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithExternalChecksumKeyword(SyntaxToken externalChecksumKeyword)
		{
			return Update(HashToken, externalChecksumKeyword, OpenParenToken, ExternalSource, FirstCommaToken, Guid, SecondCommaToken, Checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(HashToken, ExternalChecksumKeyword, openParenToken, ExternalSource, FirstCommaToken, Guid, SecondCommaToken, Checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithExternalSource(SyntaxToken externalSource)
		{
			return Update(HashToken, ExternalChecksumKeyword, OpenParenToken, externalSource, FirstCommaToken, Guid, SecondCommaToken, Checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithFirstCommaToken(SyntaxToken firstCommaToken)
		{
			return Update(HashToken, ExternalChecksumKeyword, OpenParenToken, ExternalSource, firstCommaToken, Guid, SecondCommaToken, Checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithGuid(SyntaxToken guid)
		{
			return Update(HashToken, ExternalChecksumKeyword, OpenParenToken, ExternalSource, FirstCommaToken, guid, SecondCommaToken, Checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithSecondCommaToken(SyntaxToken secondCommaToken)
		{
			return Update(HashToken, ExternalChecksumKeyword, OpenParenToken, ExternalSource, FirstCommaToken, Guid, secondCommaToken, Checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithChecksum(SyntaxToken checksum)
		{
			return Update(HashToken, ExternalChecksumKeyword, OpenParenToken, ExternalSource, FirstCommaToken, Guid, SecondCommaToken, checksum, CloseParenToken);
		}

		public ExternalChecksumDirectiveTriviaSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(HashToken, ExternalChecksumKeyword, OpenParenToken, ExternalSource, FirstCommaToken, Guid, SecondCommaToken, Checksum, closeParenToken);
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
			return visitor.VisitExternalChecksumDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitExternalChecksumDirectiveTrivia(this);
		}

		public ExternalChecksumDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken externalChecksumKeyword, SyntaxToken openParenToken, SyntaxToken externalSource, SyntaxToken firstCommaToken, SyntaxToken guid, SyntaxToken secondCommaToken, SyntaxToken checksum, SyntaxToken closeParenToken)
		{
			if (hashToken != HashToken || externalChecksumKeyword != ExternalChecksumKeyword || openParenToken != OpenParenToken || externalSource != ExternalSource || firstCommaToken != FirstCommaToken || guid != Guid || secondCommaToken != SecondCommaToken || checksum != Checksum || closeParenToken != CloseParenToken)
			{
				ExternalChecksumDirectiveTriviaSyntax externalChecksumDirectiveTriviaSyntax = SyntaxFactory.ExternalChecksumDirectiveTrivia(hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(externalChecksumDirectiveTriviaSyntax, annotations);
				}
				return externalChecksumDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
