using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ElseIfStatementSyntax : StatementSyntax
	{
		internal ExpressionSyntax _condition;

		public SyntaxToken ElseIfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax)base.Green)._elseIfKeyword, base.Position, 0);

		public ExpressionSyntax Condition => GetRed(ref _condition, 1);

		public SyntaxToken ThenKeyword
		{
			get
			{
				KeywordSyntax thenKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax)base.Green)._thenKeyword;
				return (thenKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, thenKeyword, GetChildPosition(2), GetChildIndex(2));
			}
		}

		internal ElseIfStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ElseIfStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax elseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax(kind, errors, annotations, elseIfKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, thenKeyword), null, 0)
		{
		}

		public ElseIfStatementSyntax WithElseIfKeyword(SyntaxToken elseIfKeyword)
		{
			return Update(elseIfKeyword, Condition, ThenKeyword);
		}

		public ElseIfStatementSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(ElseIfKeyword, condition, ThenKeyword);
		}

		public ElseIfStatementSyntax WithThenKeyword(SyntaxToken thenKeyword)
		{
			return Update(ElseIfKeyword, Condition, thenKeyword);
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
			return visitor.VisitElseIfStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitElseIfStatement(this);
		}

		public ElseIfStatementSyntax Update(SyntaxToken elseIfKeyword, ExpressionSyntax condition, SyntaxToken thenKeyword)
		{
			if (elseIfKeyword != ElseIfKeyword || condition != Condition || thenKeyword != ThenKeyword)
			{
				ElseIfStatementSyntax elseIfStatementSyntax = SyntaxFactory.ElseIfStatement(elseIfKeyword, condition, thenKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(elseIfStatementSyntax, annotations);
				}
				return elseIfStatementSyntax;
			}
			return this;
		}
	}
}
