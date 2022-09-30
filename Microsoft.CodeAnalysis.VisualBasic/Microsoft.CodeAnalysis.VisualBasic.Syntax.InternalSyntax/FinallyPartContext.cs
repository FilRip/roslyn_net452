namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class FinallyPartContext : ExecutableStatementContext
	{
		internal FinallyPartContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.FinallyBlock, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.CatchStatement:
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_CatchAfterFinally));
				return this;
			case SyntaxKind.FinallyStatement:
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_FinallyAfterFinally));
				return this;
			default:
				return base.ProcessSyntax(node);
			}
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			SyntaxKind kind = node.Kind;
			if (kind == SyntaxKind.CatchStatement || kind == SyntaxKind.FinallyStatement)
			{
				return UseSyntax(node, ref newContext);
			}
			return base.TryLinkSyntax(node, ref newContext);
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax statement)
		{
			FinallyBlockSyntax result = base.SyntaxFactory.FinallyBlock((FinallyStatementSyntax)base.BeginStatement, Body());
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax statement)
		{
			return base.PrevBlock.ProcessSyntax(CreateBlockSyntax(null)).EndBlock(statement);
		}
	}
}
