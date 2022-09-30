namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CaseBlockContext : ExecutableStatementContext
	{
		internal CaseBlockContext(SyntaxKind contextKind, StatementSyntax statement, BlockContext prevContext)
			: base(contextKind, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			SyntaxKind kind = node.Kind;
			if (kind - 211 <= SyntaxKind.List)
			{
				if (base.BlockKind == SyntaxKind.CaseElseBlock)
				{
					node = Parser.ReportSyntaxError(node, ERRID.ERR_CaseAfterCaseElse);
				}
				return base.PrevBlock.ProcessSyntax(CreateBlockSyntax(null)).ProcessSyntax(node);
			}
			return base.ProcessSyntax(node);
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			SyntaxKind kind = node.Kind;
			if (kind - 211 <= SyntaxKind.List)
			{
				return UseSyntax(node, ref newContext);
			}
			return base.TryLinkSyntax(node, ref newContext);
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			VisualBasicSyntaxNode result = ((base.BlockKind != SyntaxKind.CaseBlock) ? base.SyntaxFactory.CaseElseBlock((CaseStatementSyntax)base.BeginStatement, Body()) : base.SyntaxFactory.CaseBlock((CaseStatementSyntax)base.BeginStatement, Body()));
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax endStmt)
		{
			VisualBasicSyntaxNode syntax = CreateBlockSyntax(null);
			base.PrevBlock.ProcessSyntax(syntax);
			return base.PrevBlock.EndBlock(endStmt);
		}
	}
}
