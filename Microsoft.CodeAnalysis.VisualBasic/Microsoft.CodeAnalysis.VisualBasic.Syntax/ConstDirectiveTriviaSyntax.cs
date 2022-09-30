using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ConstDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal ExpressionSyntax _value;

		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken ConstKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax)base.Green)._constKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax)base.Green)._name, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax)base.Green)._equalsToken, GetChildPosition(3), GetChildIndex(3));

		public ExpressionSyntax Value => GetRed(ref _value, 4);

		internal ConstDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ConstDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax constKeyword, IdentifierTokenSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax value)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax(kind, errors, annotations, hashToken, constKeyword, name, equalsToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)value.Green), null, 0)
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

		public new ConstDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, ConstKeyword, Name, EqualsToken, Value);
		}

		public ConstDirectiveTriviaSyntax WithConstKeyword(SyntaxToken constKeyword)
		{
			return Update(HashToken, constKeyword, Name, EqualsToken, Value);
		}

		public ConstDirectiveTriviaSyntax WithName(SyntaxToken name)
		{
			return Update(HashToken, ConstKeyword, name, EqualsToken, Value);
		}

		public ConstDirectiveTriviaSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(HashToken, ConstKeyword, Name, equalsToken, Value);
		}

		public ConstDirectiveTriviaSyntax WithValue(ExpressionSyntax value)
		{
			return Update(HashToken, ConstKeyword, Name, EqualsToken, value);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 4)
			{
				return _value;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 4)
			{
				return Value;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitConstDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitConstDirectiveTrivia(this);
		}

		public ConstDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken constKeyword, SyntaxToken name, SyntaxToken equalsToken, ExpressionSyntax value)
		{
			if (hashToken != HashToken || constKeyword != ConstKeyword || name != Name || equalsToken != EqualsToken || value != Value)
			{
				ConstDirectiveTriviaSyntax constDirectiveTriviaSyntax = SyntaxFactory.ConstDirectiveTrivia(hashToken, constKeyword, name, equalsToken, value);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(constDirectiveTriviaSyntax, annotations);
				}
				return constDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
