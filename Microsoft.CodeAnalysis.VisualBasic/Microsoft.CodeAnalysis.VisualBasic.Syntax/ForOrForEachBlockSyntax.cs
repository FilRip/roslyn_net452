namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ForOrForEachBlockSyntax : ExecutableStatementSyntax
	{
		internal SyntaxNode _statements;

		internal NextStatementSyntax _nextStatement;

		public SyntaxList<StatementSyntax> Statements => GetStatementsCore();

		public NextStatementSyntax NextStatement => GetNextStatementCore();

		public abstract ForOrForEachStatementSyntax ForOrForEachStatement { get; }

		internal ForOrForEachBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxList<StatementSyntax> GetStatementsCore()
		{
			SyntaxNode redAtZero = GetRedAtZero(ref _statements);
			return new SyntaxList<StatementSyntax>(redAtZero);
		}

		public ForOrForEachBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return WithStatementsCore(statements);
		}

		internal abstract ForOrForEachBlockSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements);

		public ForOrForEachBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return AddStatementsCore(items);
		}

		internal abstract ForOrForEachBlockSyntax AddStatementsCore(params StatementSyntax[] items);

		internal virtual NextStatementSyntax GetNextStatementCore()
		{
			return GetRed(ref _nextStatement, 1);
		}

		public ForOrForEachBlockSyntax WithNextStatement(NextStatementSyntax nextStatement)
		{
			return WithNextStatementCore(nextStatement);
		}

		internal abstract ForOrForEachBlockSyntax WithNextStatementCore(NextStatementSyntax nextStatement);

		public ForOrForEachBlockSyntax AddNextStatementControlVariables(params ExpressionSyntax[] items)
		{
			return AddNextStatementControlVariablesCore(items);
		}

		internal abstract ForOrForEachBlockSyntax AddNextStatementControlVariablesCore(params ExpressionSyntax[] items);
	}
}
