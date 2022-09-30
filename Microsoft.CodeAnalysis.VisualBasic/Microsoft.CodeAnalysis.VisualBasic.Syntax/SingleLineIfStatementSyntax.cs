using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SingleLineIfStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _condition;

		internal SyntaxNode _statements;

		internal SingleLineElseClauseSyntax _elseClause;

		public SyntaxToken IfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax)base.Green)._ifKeyword, base.Position, 0);

		public ExpressionSyntax Condition => GetRed(ref _condition, 1);

		public SyntaxToken ThenKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax)base.Green)._thenKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 3);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public SingleLineElseClauseSyntax ElseClause => GetRed(ref _elseClause, 4);

		internal SingleLineIfStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SingleLineIfStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, SyntaxNode statements, SingleLineElseClauseSyntax elseClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax(kind, errors, annotations, ifKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, thenKeyword, statements?.Green, (elseClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax)elseClause.Green) : null), null, 0)
		{
		}

		public SingleLineIfStatementSyntax WithIfKeyword(SyntaxToken ifKeyword)
		{
			return Update(ifKeyword, Condition, ThenKeyword, Statements, ElseClause);
		}

		public SingleLineIfStatementSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(IfKeyword, condition, ThenKeyword, Statements, ElseClause);
		}

		public SingleLineIfStatementSyntax WithThenKeyword(SyntaxToken thenKeyword)
		{
			return Update(IfKeyword, Condition, thenKeyword, Statements, ElseClause);
		}

		public SingleLineIfStatementSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(IfKeyword, Condition, ThenKeyword, statements, ElseClause);
		}

		public SingleLineIfStatementSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public SingleLineIfStatementSyntax WithElseClause(SingleLineElseClauseSyntax elseClause)
		{
			return Update(IfKeyword, Condition, ThenKeyword, Statements, elseClause);
		}

		public SingleLineIfStatementSyntax AddElseClauseStatements(params StatementSyntax[] items)
		{
			SingleLineElseClauseSyntax singleLineElseClauseSyntax = ((ElseClause != null) ? ElseClause : SyntaxFactory.SingleLineElseClause());
			return WithElseClause(singleLineElseClauseSyntax.AddStatements(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _condition, 
				3 => _statements, 
				4 => _elseClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => Condition, 
				3 => GetRed(ref _statements, 3), 
				4 => ElseClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSingleLineIfStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSingleLineIfStatement(this);
		}

		public SingleLineIfStatementSyntax Update(SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken thenKeyword, SyntaxList<StatementSyntax> statements, SingleLineElseClauseSyntax elseClause)
		{
			if (ifKeyword != IfKeyword || condition != Condition || thenKeyword != ThenKeyword || statements != Statements || elseClause != ElseClause)
			{
				SingleLineIfStatementSyntax singleLineIfStatementSyntax = SyntaxFactory.SingleLineIfStatement(ifKeyword, condition, thenKeyword, statements, elseClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(singleLineIfStatementSyntax, annotations);
				}
				return singleLineIfStatementSyntax;
			}
			return this;
		}
	}
}
