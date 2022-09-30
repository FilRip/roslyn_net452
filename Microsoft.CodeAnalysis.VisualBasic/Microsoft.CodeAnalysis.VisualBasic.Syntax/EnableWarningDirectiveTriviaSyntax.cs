using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EnableWarningDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal SyntaxNode _errorCodes;

		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken EnableKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax)base.Green)._enableKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken WarningKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax)base.Green)._warningKeyword, GetChildPosition(2), GetChildIndex(2));

		public SeparatedSyntaxList<IdentifierNameSyntax> ErrorCodes
		{
			get
			{
				SyntaxNode red = GetRed(ref _errorCodes, 3);
				return (red == null) ? default(SeparatedSyntaxList<IdentifierNameSyntax>) : new SeparatedSyntaxList<IdentifierNameSyntax>(red, GetChildIndex(3));
			}
		}

		internal EnableWarningDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EnableWarningDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax enableKeyword, KeywordSyntax warningKeyword, SyntaxNode errorCodes)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax(kind, errors, annotations, hashToken, enableKeyword, warningKeyword, errorCodes?.Green), null, 0)
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

		public new EnableWarningDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, EnableKeyword, WarningKeyword, ErrorCodes);
		}

		public EnableWarningDirectiveTriviaSyntax WithEnableKeyword(SyntaxToken enableKeyword)
		{
			return Update(HashToken, enableKeyword, WarningKeyword, ErrorCodes);
		}

		public EnableWarningDirectiveTriviaSyntax WithWarningKeyword(SyntaxToken warningKeyword)
		{
			return Update(HashToken, EnableKeyword, warningKeyword, ErrorCodes);
		}

		public EnableWarningDirectiveTriviaSyntax WithErrorCodes(SeparatedSyntaxList<IdentifierNameSyntax> errorCodes)
		{
			return Update(HashToken, EnableKeyword, WarningKeyword, errorCodes);
		}

		public EnableWarningDirectiveTriviaSyntax AddErrorCodes(params IdentifierNameSyntax[] items)
		{
			return WithErrorCodes(ErrorCodes.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 3)
			{
				return _errorCodes;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 3)
			{
				return GetRed(ref _errorCodes, 3);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEnableWarningDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEnableWarningDirectiveTrivia(this);
		}

		public EnableWarningDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken enableKeyword, SyntaxToken warningKeyword, SeparatedSyntaxList<IdentifierNameSyntax> errorCodes)
		{
			if (hashToken != HashToken || enableKeyword != EnableKeyword || warningKeyword != WarningKeyword || errorCodes != ErrorCodes)
			{
				EnableWarningDirectiveTriviaSyntax enableWarningDirectiveTriviaSyntax = SyntaxFactory.EnableWarningDirectiveTrivia(hashToken, enableKeyword, warningKeyword, errorCodes);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(enableWarningDirectiveTriviaSyntax, annotations);
				}
				return enableWarningDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
