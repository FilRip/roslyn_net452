namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class IfPartContext : ExecutableStatementContext
	{
		internal IfPartContext(SyntaxKind kind, StatementSyntax statement, BlockContext prevContext)
			: base(kind, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			SyntaxKind kind = node.Kind;
			if (kind - 183 <= SyntaxKind.List && base.BlockKind == SyntaxKind.ElseIfBlock)
			{
				return base.PrevBlock.ProcessSyntax(CreateBlockSyntax(null)).ProcessSyntax(node);
			}
			return base.ProcessSyntax(node);
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			SyntaxKind kind = node.Kind;
			if (kind - 183 <= SyntaxKind.List)
			{
				return UseSyntax(node, ref newContext);
			}
			return base.TryLinkSyntax(node, ref newContext);
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax statement)
		{
			VisualBasicSyntaxNode result = ((base.BeginStatement.Kind != SyntaxKind.ElseStatement) ? ((VisualBasicSyntaxNode)base.SyntaxFactory.ElseIfBlock((ElseIfStatementSyntax)base.BeginStatement, Body())) : ((VisualBasicSyntaxNode)base.SyntaxFactory.ElseBlock((ElseStatementSyntax)base.BeginStatement, Body())));
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax statement)
		{
			VisualBasicSyntaxNode syntax = CreateBlockSyntax(null);
			return base.PrevBlock.ProcessSyntax(syntax).EndBlock(statement);
		}

		internal override BlockContext ResyncAndProcessStatementTerminator(StatementSyntax statement, BlockContext lambdaContext)
		{
			if (statement.Kind == SyntaxKind.ElseStatement && !SyntaxFacts.IsTerminator(base.Parser.CurrentToken.Kind))
			{
				base.Parser.ConsumedStatementTerminator(allowLeadingMultilineTrivia: false);
				return this;
			}
			return base.ResyncAndProcessStatementTerminator(statement, lambdaContext);
		}
	}
}
