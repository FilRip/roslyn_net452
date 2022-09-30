using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TryBlockContext : ExecutableStatementContext
	{
		private readonly SyntaxListBuilder<CatchBlockSyntax> _catchParts;

		private FinallyBlockSyntax _optionalFinallyPart;

		internal TryBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.TryBlock, statement, prevContext)
		{
			_catchParts = _parser._pool.Allocate<CatchBlockSyntax>();
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.CatchStatement:
				return new CatchPartContext((StatementSyntax)node, this);
			case SyntaxKind.FinallyStatement:
				return new FinallyPartContext((StatementSyntax)node, this);
			case SyntaxKind.CatchBlock:
				_catchParts.Add((CatchBlockSyntax)node);
				break;
			case SyntaxKind.FinallyBlock:
				_optionalFinallyPart = (FinallyBlockSyntax)node;
				break;
			default:
				return base.ProcessSyntax(node);
			}
			return this;
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			switch (node.Kind)
			{
			case SyntaxKind.CatchStatement:
			case SyntaxKind.FinallyStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.CatchBlock:
			case SyntaxKind.FinallyBlock:
				return UseSyntax(node, ref newContext) | LinkResult.SkipTerminator;
			default:
				return base.TryLinkSyntax(node, ref newContext);
			}
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			TryStatementSyntax tryStatementSyntax = (TryStatementSyntax)base.BeginStatement;
			if (endStmt == null)
			{
				tryStatementSyntax = Parser.ReportSyntaxError(tryStatementSyntax, ERRID.ERR_ExpectedEndTry);
				endStmt = base.SyntaxFactory.EndTryStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.TryKeyword));
			}
			TryBlockSyntax result = base.SyntaxFactory.TryBlock(tryStatementSyntax, Body(), _catchParts.ToList(), _optionalFinallyPart, (EndBlockStatementSyntax)endStmt);
			_parser._pool.Free(_catchParts);
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax statement)
		{
			VisualBasicSyntaxNode syntax = CreateBlockSyntax(statement);
			return base.PrevBlock.ProcessSyntax(syntax);
		}
	}
}
