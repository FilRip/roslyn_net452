namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EnumDeclarationBlockContext : DeclarationContext
	{
		internal EnumDeclarationBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.EnumBlock, statement, prevContext)
		{
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			EnumStatementSyntax beginStmt = null;
			EndBlockStatementSyntax endStmt2 = (EndBlockStatementSyntax)endStmt;
			GetBeginEndStatements(ref beginStmt, ref endStmt2);
			EnumBlockSyntax result = base.SyntaxFactory.EnumBlock(beginStmt, Body(), endStmt2);
			FreeStatements();
			return result;
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			SyntaxKind kind = node.Kind;
			if (kind == SyntaxKind.EnumMemberDeclaration)
			{
				Add(node);
			}
			else
			{
				if (!SyntaxNodeExtensions.IsExecutableStatementOrItsPart(node))
				{
					return EndBlock(null).ProcessSyntax(Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideEndsEnum));
				}
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideEnum));
			}
			return this;
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			switch (node.Kind)
			{
			case SyntaxKind.EnumMemberDeclaration:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.NamespaceBlock:
			case SyntaxKind.ModuleBlock:
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
			case SyntaxKind.EnumBlock:
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
	}
}
