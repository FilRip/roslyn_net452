using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SingleLineLambdaContext : MethodBlockContext
	{
		internal override bool IsLambda => true;

		internal override bool IsSingleLine => true;

		internal SingleLineLambdaContext(StatementSyntax statement, BlockContext prevContext)
			: base((statement.Kind == SyntaxKind.FunctionLambdaHeader) ? SyntaxKind.SingleLineFunctionLambdaExpression : SyntaxKind.SingleLineSubLambdaExpression, statement, prevContext)
		{
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = Body();
			VisualBasicSyntaxNode visualBasicSyntaxNode;
			bool flag;
			if (syntaxList.Count == 0)
			{
				visualBasicSyntaxNode = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingEmptyToken);
				flag = true;
			}
			else
			{
				visualBasicSyntaxNode = syntaxList[0];
				flag = !visualBasicSyntaxNode.ContainsDiagnostics && !IsSingleStatement(visualBasicSyntaxNode);
			}
			LambdaHeaderSyntax lambdaHeaderSyntax = (LambdaHeaderSyntax)base.BeginStatement;
			SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = base.SyntaxFactory.SingleLineLambdaExpression(base.BlockKind, lambdaHeaderSyntax, visualBasicSyntaxNode);
			if (flag)
			{
				singleLineLambdaExpressionSyntax = Parser.ReportSyntaxError(singleLineLambdaExpressionSyntax, ERRID.ERR_SubRequiresSingleStatement);
			}
			else if (lambdaHeaderSyntax.Kind == SyntaxKind.FunctionLambdaHeader && lambdaHeaderSyntax.Modifiers.Any(632))
			{
				singleLineLambdaExpressionSyntax = Parser.ReportSyntaxError(singleLineLambdaExpressionSyntax, ERRID.ERR_BadIteratorExpressionLambda);
			}
			FreeStatements();
			return singleLineLambdaExpressionSyntax;
		}

		internal override BlockContext EndBlock(StatementSyntax endStmt)
		{
			return base.PrevBlock;
		}

		internal override BlockContext ResyncAndProcessStatementTerminator(StatementSyntax statement, BlockContext lambdaContext)
		{
			return ProcessStatementTerminator(lambdaContext);
		}

		internal override BlockContext ProcessStatementTerminator(BlockContext lambdaContext)
		{
			switch (base.Parser.CurrentToken.Kind)
			{
			case SyntaxKind.ColonToken:
				if (_statements.Count > 0)
				{
					_statements[0] = Parser.ReportSyntaxError(_statements[0], ERRID.ERR_SubRequiresSingleStatement);
				}
				return BlockContextExtensions.EndLambda(this);
			default:
				return base.PrevBlock;
			}
		}

		private static bool IsSingleStatement(VisualBasicSyntaxNode statement)
		{
			switch (statement.Kind)
			{
			case SyntaxKind.EmptyStatement:
			case SyntaxKind.WhileBlock:
			case SyntaxKind.UsingBlock:
			case SyntaxKind.SyncLockBlock:
			case SyntaxKind.WithBlock:
			case SyntaxKind.MultiLineIfBlock:
			case SyntaxKind.TryBlock:
			case SyntaxKind.SelectBlock:
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
			case SyntaxKind.DoLoopWhileBlock:
			case SyntaxKind.DoLoopUntilBlock:
				return false;
			default:
				return true;
			}
		}
	}
}
