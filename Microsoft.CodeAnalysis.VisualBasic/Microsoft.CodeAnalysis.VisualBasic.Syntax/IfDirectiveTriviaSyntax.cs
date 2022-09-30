using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class IfDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal ExpressionSyntax _condition;

		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken ElseKeyword
		{
			get
			{
				KeywordSyntax elseKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)base.Green)._elseKeyword;
				return (elseKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, elseKeyword, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken IfOrElseIfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)base.Green)._ifOrElseIfKeyword, GetChildPosition(2), GetChildIndex(2));

		public ExpressionSyntax Condition => GetRed(ref _condition, 3);

		public SyntaxToken ThenKeyword
		{
			get
			{
				KeywordSyntax thenKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)base.Green)._thenKeyword;
				return (thenKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, thenKeyword, GetChildPosition(4), GetChildIndex(4));
			}
		}

		internal IfDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal IfDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(kind, errors, annotations, hashToken, elseKeyword, ifOrElseIfKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, thenKeyword), null, 0)
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

		public new IfDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(Kind(), hashToken, ElseKeyword, IfOrElseIfKeyword, Condition, ThenKeyword);
		}

		public IfDirectiveTriviaSyntax WithElseKeyword(SyntaxToken elseKeyword)
		{
			return Update(Kind(), HashToken, elseKeyword, IfOrElseIfKeyword, Condition, ThenKeyword);
		}

		public IfDirectiveTriviaSyntax WithIfOrElseIfKeyword(SyntaxToken ifOrElseIfKeyword)
		{
			return Update(Kind(), HashToken, ElseKeyword, ifOrElseIfKeyword, Condition, ThenKeyword);
		}

		public IfDirectiveTriviaSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(Kind(), HashToken, ElseKeyword, IfOrElseIfKeyword, condition, ThenKeyword);
		}

		public IfDirectiveTriviaSyntax WithThenKeyword(SyntaxToken thenKeyword)
		{
			return Update(Kind(), HashToken, ElseKeyword, IfOrElseIfKeyword, Condition, thenKeyword);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 3)
			{
				return _condition;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 3)
			{
				return Condition;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitIfDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitIfDirectiveTrivia(this);
		}

		public IfDirectiveTriviaSyntax Update(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken elseKeyword, SyntaxToken ifOrElseIfKeyword, ExpressionSyntax condition, SyntaxToken thenKeyword)
		{
			if (kind != Kind() || hashToken != HashToken || elseKeyword != ElseKeyword || ifOrElseIfKeyword != IfOrElseIfKeyword || condition != Condition || thenKeyword != ThenKeyword)
			{
				IfDirectiveTriviaSyntax ifDirectiveTriviaSyntax = SyntaxFactory.IfDirectiveTrivia(kind, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(ifDirectiveTriviaSyntax, annotations);
				}
				return ifDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
