using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class StatementBlockContext : ExecutableStatementContext
	{
		internal StatementBlockContext(SyntaxKind kind, StatementSyntax statement, BlockContext prevContext)
			: base(kind, statement, prevContext)
		{
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax statement)
		{
			EndBlockStatementSyntax endStmt = (EndBlockStatementSyntax)statement;
			VisualBasicSyntaxNode result;
			switch (base.BlockKind)
			{
			case SyntaxKind.WhileBlock:
			{
				WhileStatementSyntax beginStmt4 = null;
				GetBeginEndStatements(ref beginStmt4, ref endStmt);
				result = base.SyntaxFactory.WhileBlock(beginStmt4, Body(), endStmt);
				break;
			}
			case SyntaxKind.WithBlock:
			{
				WithStatementSyntax beginStmt3 = null;
				GetBeginEndStatements(ref beginStmt3, ref endStmt);
				result = base.SyntaxFactory.WithBlock(beginStmt3, Body(), endStmt);
				break;
			}
			case SyntaxKind.SyncLockBlock:
			{
				SyncLockStatementSyntax beginStmt2 = null;
				GetBeginEndStatements(ref beginStmt2, ref endStmt);
				result = base.SyntaxFactory.SyncLockBlock(beginStmt2, Body(), endStmt);
				break;
			}
			case SyntaxKind.UsingBlock:
			{
				UsingStatementSyntax beginStmt = null;
				GetBeginEndStatements(ref beginStmt, ref endStmt);
				result = base.SyntaxFactory.UsingBlock(beginStmt, Body(), endStmt);
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(base.BlockKind);
			}
			FreeStatements();
			return result;
		}
	}
}
