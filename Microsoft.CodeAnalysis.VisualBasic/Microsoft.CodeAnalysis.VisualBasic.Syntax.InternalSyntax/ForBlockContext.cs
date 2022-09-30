using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ForBlockContext : ExecutableStatementContext
	{
		private static readonly NextStatementSyntax s_emptyNextStatement;

		static ForBlockContext()
		{
			s_emptyNextStatement = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.NextStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.NextKeyword), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode>));
		}

		internal ForBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base((statement.Kind == SyntaxKind.ForStatement) ? SyntaxKind.ForBlock : SyntaxKind.ForEachBlock, statement, prevContext)
		{
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			StatementSyntax beginStmt = null;
			NextStatementSyntax endStmt2 = (NextStatementSyntax)endStmt;
			GetBeginEndStatements(ref beginStmt, ref endStmt2);
			if (endStmt == s_emptyNextStatement)
			{
				endStmt2 = null;
			}
			VisualBasicSyntaxNode result = ((base.BlockKind != SyntaxKind.ForBlock) ? ((ForOrForEachBlockSyntax)base.SyntaxFactory.ForEachBlock((ForEachStatementSyntax)beginStmt, Body(), endStmt2)) : ((ForOrForEachBlockSyntax)base.SyntaxFactory.ForBlock((ForStatementSyntax)beginStmt, Body(), endStmt2)));
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax endStmt)
		{
			BlockContext blockContext = this;
			VisualBasicSyntaxNode syntax = blockContext.CreateBlockSyntax(endStmt);
			blockContext = blockContext.PrevBlock;
			blockContext = blockContext.ProcessSyntax(syntax);
			if (endStmt != null)
			{
				int count = ((NextStatementSyntax)endStmt).ControlVariables.Count;
				for (int i = 2; i <= count; i++)
				{
					if (blockContext == null)
					{
						break;
					}
					if (blockContext.BlockKind != SyntaxKind.ForBlock && blockContext.BlockKind != SyntaxKind.ForEachBlock)
					{
						break;
					}
					syntax = blockContext.CreateBlockSyntax(s_emptyNextStatement);
					blockContext = blockContext.PrevBlock;
					blockContext = blockContext.ProcessSyntax(syntax);
				}
			}
			return blockContext;
		}
	}
}
