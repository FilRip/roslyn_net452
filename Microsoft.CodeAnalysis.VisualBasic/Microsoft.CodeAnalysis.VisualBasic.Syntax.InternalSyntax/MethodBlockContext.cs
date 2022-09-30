using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class MethodBlockContext : ExecutableStatementContext
	{
		internal MethodBlockContext(SyntaxKind contextKind, StatementSyntax statement, BlockContext prevContext)
			: base(contextKind, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			if (base.Statements.Count == 0)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = SyntaxExtensions.LastTriviaIfAny(base.BeginStatement);
				if (visualBasicSyntaxNode != null && visualBasicSyntaxNode.Kind == SyntaxKind.ColonTrivia)
				{
					node = Parser.ReportSyntaxError(node, ERRID.ERR_MethodBodyNotAtLineStart);
				}
			}
			SyntaxKind kind = node.Kind;
			if (kind == SyntaxKind.ExitPropertyStatement && base.BlockKind != SyntaxKind.GetAccessorBlock && base.BlockKind != SyntaxKind.SetAccessorBlock)
			{
				node = Parser.ReportSyntaxError(node, ERRID.ERR_ExitPropNot);
			}
			return base.ProcessSyntax(node);
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			EndBlockStatementSyntax endStmt2 = (EndBlockStatementSyntax)endStmt;
			VisualBasicSyntaxNode result;
			switch (base.BlockKind)
			{
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			{
				MethodStatementSyntax beginStmt4 = null;
				GetBeginEndStatements(ref beginStmt4, ref endStmt2);
				result = base.SyntaxFactory.MethodBlock(base.BlockKind, beginStmt4, BodyWithWeakChildren(), endStmt2);
				break;
			}
			case SyntaxKind.ConstructorBlock:
			{
				SubNewStatementSyntax beginStmt3 = null;
				GetBeginEndStatements(ref beginStmt3, ref endStmt2);
				result = base.SyntaxFactory.ConstructorBlock(beginStmt3, BodyWithWeakChildren(), endStmt2);
				break;
			}
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
			{
				AccessorStatementSyntax beginStmt2 = null;
				GetBeginEndStatements(ref beginStmt2, ref endStmt2);
				result = base.SyntaxFactory.AccessorBlock(base.BlockKind, beginStmt2, BodyWithWeakChildren(), endStmt2);
				break;
			}
			case SyntaxKind.OperatorBlock:
			{
				OperatorStatementSyntax beginStmt = null;
				GetBeginEndStatements(ref beginStmt, ref endStmt2);
				result = base.SyntaxFactory.OperatorBlock(beginStmt, BodyWithWeakChildren(), endStmt2);
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(base.BlockKind);
			}
			FreeStatements();
			return result;
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			if (!node.MatchesFactoryContext(this))
			{
				return LinkResult.NotUsed;
			}
			return base.TryLinkSyntax(node, ref newContext);
		}
	}
}
