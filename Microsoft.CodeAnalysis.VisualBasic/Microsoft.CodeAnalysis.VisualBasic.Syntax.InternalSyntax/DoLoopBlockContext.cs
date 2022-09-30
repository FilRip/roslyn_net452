namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DoLoopBlockContext : ExecutableStatementContext
	{
		internal DoLoopBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base((((DoStatementSyntax)statement).WhileOrUntilClause == null) ? SyntaxKind.SimpleDoLoopBlock : SyntaxKind.DoWhileLoopBlock, statement, prevContext)
		{
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax statement)
		{
			DoStatementSyntax beginStmt = null;
			LoopStatementSyntax endStmt = (LoopStatementSyntax)statement;
			GetBeginEndStatements(ref beginStmt, ref endStmt);
			SyntaxKind syntaxKind = base.BlockKind;
			if (syntaxKind == SyntaxKind.DoWhileLoopBlock && endStmt.WhileOrUntilClause != null)
			{
				WhileOrUntilClauseSyntax whileOrUntilClause = endStmt.WhileOrUntilClause;
				KeywordSyntax whileOrUntilKeyword = Parser.ReportSyntaxError(whileOrUntilClause.WhileOrUntilKeyword, ERRID.ERR_LoopDoubleCondition);
				DiagnosticInfo[] diagnostics = whileOrUntilClause.GetDiagnostics();
				whileOrUntilClause = base.SyntaxFactory.WhileOrUntilClause(whileOrUntilClause.Kind, whileOrUntilKeyword, whileOrUntilClause.Condition);
				if (diagnostics != null)
				{
					whileOrUntilClause = (WhileOrUntilClauseSyntax)whileOrUntilClause.SetDiagnostics(diagnostics);
				}
				endStmt = base.SyntaxFactory.LoopStatement(endStmt.Kind, endStmt.LoopKeyword, whileOrUntilClause);
			}
			if (syntaxKind == SyntaxKind.SimpleDoLoopBlock && endStmt.WhileOrUntilClause != null)
			{
				syntaxKind = ((endStmt.Kind == SyntaxKind.LoopWhileStatement) ? SyntaxKind.DoLoopWhileBlock : SyntaxKind.DoLoopUntilBlock);
			}
			else if (beginStmt.WhileOrUntilClause != null)
			{
				syntaxKind = ((beginStmt.Kind == SyntaxKind.DoWhileStatement) ? SyntaxKind.DoWhileLoopBlock : SyntaxKind.DoUntilLoopBlock);
			}
			DoLoopBlockSyntax result = base.SyntaxFactory.DoLoopBlock(syntaxKind, beginStmt, Body(), endStmt);
			FreeStatements();
			return result;
		}

		internal override bool KindEndsBlock(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 773 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}
	}
}
