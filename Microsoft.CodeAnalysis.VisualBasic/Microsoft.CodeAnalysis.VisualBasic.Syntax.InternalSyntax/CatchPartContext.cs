namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CatchPartContext : ExecutableStatementContext
	{
		internal CatchPartContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.CatchBlock, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			SyntaxKind kind = node.Kind;
			if (kind == SyntaxKind.CatchStatement || kind == SyntaxKind.FinallyStatement)
			{
				return base.PrevBlock.ProcessSyntax(CreateBlockSyntax(null)).ProcessSyntax(node);
			}
			return base.ProcessSyntax(node);
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
			CatchBlockSyntax result = base.SyntaxFactory.CatchBlock((CatchStatementSyntax)base.BeginStatement, Body());
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax statement)
		{
			return base.PrevBlock.ProcessSyntax(CreateBlockSyntax(null)).EndBlock(statement);
		}
	}
}
