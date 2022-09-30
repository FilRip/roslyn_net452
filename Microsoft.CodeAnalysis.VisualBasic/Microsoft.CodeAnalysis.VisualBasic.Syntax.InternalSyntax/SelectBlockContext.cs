using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SelectBlockContext : ExecutableStatementContext
	{
		private readonly SyntaxListBuilder<CaseBlockSyntax> _caseBlocks;

		internal SelectBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.SelectBlock, statement, prevContext)
		{
			_caseBlocks = _parser._pool.Allocate<CaseBlockSyntax>();
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.CaseStatement:
				return new CaseBlockContext(SyntaxKind.CaseBlock, (StatementSyntax)node, this);
			case SyntaxKind.CaseElseStatement:
				return new CaseBlockContext(SyntaxKind.CaseElseBlock, (StatementSyntax)node, this);
			case SyntaxKind.CaseBlock:
			case SyntaxKind.CaseElseBlock:
				_caseBlocks.Add((CaseBlockSyntax)node);
				return this;
			default:
			{
				node = Parser.ReportSyntaxError(node, ERRID.ERR_ExpectedCase);
				CaseStatementSyntax statement = base.SyntaxFactory.CaseStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.CaseKeyword), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CaseClauseSyntax>));
				return new CaseBlockContext(SyntaxKind.CaseBlock, statement, this).ProcessSyntax(node);
			}
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
			case SyntaxKind.CaseStatement:
			case SyntaxKind.CaseElseStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.CaseBlock:
				return UseSyntax(node, ref newContext) | LinkResult.SkipTerminator;
			default:
				return LinkResult.NotUsed;
			}
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			SelectStatementSyntax beginStmt = null;
			EndBlockStatementSyntax endStmt2 = (EndBlockStatementSyntax)endStmt;
			GetBeginEndStatements(ref beginStmt, ref endStmt2);
			SelectBlockSyntax result = base.SyntaxFactory.SelectBlock(beginStmt, _caseBlocks.ToList(), endStmt2);
			_parser._pool.Free(_caseBlocks);
			FreeStatements();
			return result;
		}
	}
}
