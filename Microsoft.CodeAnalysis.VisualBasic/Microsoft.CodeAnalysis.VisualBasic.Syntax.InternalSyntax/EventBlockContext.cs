namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EventBlockContext : DeclarationContext
	{
		internal EventBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.EventBlock, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.AddHandlerAccessorStatement:
				return new MethodBlockContext(SyntaxKind.AddHandlerAccessorBlock, (StatementSyntax)node, this);
			case SyntaxKind.RemoveHandlerAccessorStatement:
				return new MethodBlockContext(SyntaxKind.RemoveHandlerAccessorBlock, (StatementSyntax)node, this);
			case SyntaxKind.RaiseEventAccessorStatement:
				return new MethodBlockContext(SyntaxKind.RaiseEventAccessorBlock, (StatementSyntax)node, this);
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
				Add(node);
				return this;
			default:
				return EndBlock(null).ProcessSyntax(Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideEndsEvent));
			}
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			if (KindEndsBlock(node.Kind))
			{
				return UseSyntax(node, ref newContext);
			}
			switch (node.Kind)
			{
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
				return UseSyntax(node, ref newContext, ((AccessorBlockSyntax)node).End.IsMissing);
			default:
				return TryUseStatement(node, ref newContext);
			}
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax statement)
		{
			EventStatementSyntax beginStmt = null;
			EndBlockStatementSyntax endStmt = (EndBlockStatementSyntax)statement;
			GetBeginEndStatements(ref beginStmt, ref endStmt);
			EventBlockSyntax result = base.SyntaxFactory.EventBlock(beginStmt, Body<AccessorBlockSyntax>(), endStmt);
			FreeStatements();
			return result;
		}
	}
}
