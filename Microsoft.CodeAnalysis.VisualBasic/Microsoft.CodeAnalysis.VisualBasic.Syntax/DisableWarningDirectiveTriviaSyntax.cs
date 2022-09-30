using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class DisableWarningDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal SyntaxNode _errorCodes;

		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken DisableKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax)base.Green)._disableKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken WarningKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax)base.Green)._warningKeyword, GetChildPosition(2), GetChildIndex(2));

		public SeparatedSyntaxList<IdentifierNameSyntax> ErrorCodes
		{
			get
			{
				SyntaxNode red = GetRed(ref _errorCodes, 3);
				return (red == null) ? default(SeparatedSyntaxList<IdentifierNameSyntax>) : new SeparatedSyntaxList<IdentifierNameSyntax>(red, GetChildIndex(3));
			}
		}

		internal DisableWarningDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal DisableWarningDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax disableKeyword, KeywordSyntax warningKeyword, SyntaxNode errorCodes)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax(kind, errors, annotations, hashToken, disableKeyword, warningKeyword, errorCodes?.Green), null, 0)
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

		public new DisableWarningDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, DisableKeyword, WarningKeyword, ErrorCodes);
		}

		public DisableWarningDirectiveTriviaSyntax WithDisableKeyword(SyntaxToken disableKeyword)
		{
			return Update(HashToken, disableKeyword, WarningKeyword, ErrorCodes);
		}

		public DisableWarningDirectiveTriviaSyntax WithWarningKeyword(SyntaxToken warningKeyword)
		{
			return Update(HashToken, DisableKeyword, warningKeyword, ErrorCodes);
		}

		public DisableWarningDirectiveTriviaSyntax WithErrorCodes(SeparatedSyntaxList<IdentifierNameSyntax> errorCodes)
		{
			return Update(HashToken, DisableKeyword, WarningKeyword, errorCodes);
		}

		public DisableWarningDirectiveTriviaSyntax AddErrorCodes(params IdentifierNameSyntax[] items)
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
			return visitor.VisitDisableWarningDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitDisableWarningDirectiveTrivia(this);
		}

		public DisableWarningDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken disableKeyword, SyntaxToken warningKeyword, SeparatedSyntaxList<IdentifierNameSyntax> errorCodes)
		{
			if (hashToken != HashToken || disableKeyword != DisableKeyword || warningKeyword != WarningKeyword || errorCodes != ErrorCodes)
			{
				DisableWarningDirectiveTriviaSyntax disableWarningDirectiveTriviaSyntax = SyntaxFactory.DisableWarningDirectiveTrivia(hashToken, disableKeyword, warningKeyword, errorCodes);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(disableWarningDirectiveTriviaSyntax, annotations);
				}
				return disableWarningDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
