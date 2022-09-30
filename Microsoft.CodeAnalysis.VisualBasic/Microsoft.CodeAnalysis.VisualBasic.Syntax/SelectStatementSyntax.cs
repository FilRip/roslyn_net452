using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SelectStatementSyntax : StatementSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken SelectKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax)base.Green)._selectKeyword, base.Position, 0);

		public SyntaxToken CaseKeyword
		{
			get
			{
				KeywordSyntax caseKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax)base.Green)._caseKeyword;
				return (caseKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, caseKeyword, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public ExpressionSyntax Expression => GetRed(ref _expression, 2);

		internal SelectStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SelectStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax selectKeyword, KeywordSyntax caseKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax(kind, errors, annotations, selectKeyword, caseKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public SelectStatementSyntax WithSelectKeyword(SyntaxToken selectKeyword)
		{
			return Update(selectKeyword, CaseKeyword, Expression);
		}

		public SelectStatementSyntax WithCaseKeyword(SyntaxToken caseKeyword)
		{
			return Update(SelectKeyword, caseKeyword, Expression);
		}

		public SelectStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(SelectKeyword, CaseKeyword, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSelectStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSelectStatement(this);
		}

		public SelectStatementSyntax Update(SyntaxToken selectKeyword, SyntaxToken caseKeyword, ExpressionSyntax expression)
		{
			if (selectKeyword != SelectKeyword || caseKeyword != CaseKeyword || expression != Expression)
			{
				SelectStatementSyntax selectStatementSyntax = SyntaxFactory.SelectStatement(selectKeyword, caseKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(selectStatementSyntax, annotations);
				}
				return selectStatementSyntax;
			}
			return this;
		}
	}
}
