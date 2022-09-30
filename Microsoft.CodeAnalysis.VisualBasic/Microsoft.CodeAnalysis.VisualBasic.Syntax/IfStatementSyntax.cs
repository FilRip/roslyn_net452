using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class IfStatementSyntax : StatementSyntax
	{
		internal ExpressionSyntax _condition;

		public SyntaxToken IfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)base.Green)._ifKeyword, base.Position, 0);

		public ExpressionSyntax Condition => GetRed(ref _condition, 1);

		public SyntaxToken ThenKeyword
		{
			get
			{
				KeywordSyntax thenKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)base.Green)._thenKeyword;
				return (thenKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, thenKeyword, GetChildPosition(2), GetChildIndex(2));
			}
		}

		internal IfStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal IfStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax(kind, errors, annotations, ifKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, thenKeyword), null, 0)
		{
		}

		public IfStatementSyntax WithIfKeyword(SyntaxToken ifKeyword)
		{
			return Update(ifKeyword, Condition, ThenKeyword);
		}

		public IfStatementSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(IfKeyword, condition, ThenKeyword);
		}

		public IfStatementSyntax WithThenKeyword(SyntaxToken thenKeyword)
		{
			return Update(IfKeyword, Condition, thenKeyword);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _condition;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Condition;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitIfStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitIfStatement(this);
		}

		public IfStatementSyntax Update(SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken thenKeyword)
		{
			if (ifKeyword != IfKeyword || condition != Condition || thenKeyword != ThenKeyword)
			{
				IfStatementSyntax ifStatementSyntax = SyntaxFactory.IfStatement(ifKeyword, condition, thenKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(ifStatementSyntax, annotations);
				}
				return ifStatementSyntax;
			}
			return this;
		}
	}
}
