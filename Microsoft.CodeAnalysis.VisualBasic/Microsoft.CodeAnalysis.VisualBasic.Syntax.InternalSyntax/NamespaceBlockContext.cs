namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class NamespaceBlockContext : DeclarationContext
	{
		internal NamespaceBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.NamespaceBlock, statement, prevContext)
		{
		}

		internal NamespaceBlockContext(SyntaxKind kind, StatementSyntax statement, BlockContext prevContext)
			: base(kind, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.NamespaceStatement:
				return new NamespaceBlockContext((StatementSyntax)node, this);
			case SyntaxKind.ModuleStatement:
				return new TypeBlockContext(SyntaxKind.ModuleBlock, (StatementSyntax)node, this);
			case SyntaxKind.NamespaceBlock:
			case SyntaxKind.ModuleBlock:
				Add(node);
				return this;
			default:
				return base.ProcessSyntax(node);
			}
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			switch (node.Kind)
			{
			case SyntaxKind.NamespaceStatement:
			case SyntaxKind.ModuleStatement:
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.EventStatement:
			case SyntaxKind.OperatorStatement:
			case SyntaxKind.PropertyStatement:
			case SyntaxKind.FieldDeclaration:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.ModuleBlock:
				return UseSyntax(node, ref newContext, ((TypeBlockSyntax)node).EndBlockStatement.IsMissing);
			case SyntaxKind.NamespaceBlock:
				return UseSyntax(node, ref newContext, ((NamespaceBlockSyntax)node).EndNamespaceStatement.IsMissing);
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.OperatorBlock:
			case SyntaxKind.PropertyBlock:
			case SyntaxKind.EventBlock:
				newContext = this;
				return LinkResult.Crumble;
			default:
				return base.TryLinkSyntax(node, ref newContext);
			}
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			NamespaceStatementSyntax beginStmt = (NamespaceStatementSyntax)base.BeginStatement;
			EndBlockStatementSyntax endStmt2 = (EndBlockStatementSyntax)endStmt;
			GetBeginEndStatements(ref beginStmt, ref endStmt2);
			NamespaceBlockSyntax result = base.SyntaxFactory.NamespaceBlock(beginStmt, Body(), endStmt2);
			FreeStatements();
			return result;
		}
	}
}
