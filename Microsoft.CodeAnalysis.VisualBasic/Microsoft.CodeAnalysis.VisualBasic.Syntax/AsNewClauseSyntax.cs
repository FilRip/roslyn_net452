using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AsNewClauseSyntax : AsClauseSyntax
	{
		internal NewExpressionSyntax _newExpression;

		public new SyntaxToken AsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax)base.Green)._asKeyword, base.Position, 0);

		public NewExpressionSyntax NewExpression => GetRed(ref _newExpression, 1);

		internal AsNewClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AsNewClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, NewExpressionSyntax newExpression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax(kind, errors, annotations, asKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax)newExpression.Green), null, 0)
		{
		}

		internal override SyntaxToken GetAsKeywordCore()
		{
			return AsKeyword;
		}

		internal override AsClauseSyntax WithAsKeywordCore(SyntaxToken asKeyword)
		{
			return WithAsKeyword(asKeyword);
		}

		public new AsNewClauseSyntax WithAsKeyword(SyntaxToken asKeyword)
		{
			return Update(asKeyword, NewExpression);
		}

		public AsNewClauseSyntax WithNewExpression(NewExpressionSyntax newExpression)
		{
			return Update(AsKeyword, newExpression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _newExpression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return NewExpression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAsNewClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAsNewClause(this);
		}

		public AsNewClauseSyntax Update(SyntaxToken asKeyword, NewExpressionSyntax newExpression)
		{
			if (asKeyword != AsKeyword || newExpression != NewExpression)
			{
				AsNewClauseSyntax asNewClauseSyntax = SyntaxFactory.AsNewClause(asKeyword, newExpression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(asNewClauseSyntax, annotations);
				}
				return asNewClauseSyntax;
			}
			return this;
		}
	}
}
