using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class IfBlockContext : ExecutableStatementContext
	{
		private readonly SyntaxListBuilder<ElseIfBlockSyntax> _elseIfBlocks;

		private ElseBlockSyntax _optionalElseBlock;

		internal IfBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.MultiLineIfBlock, statement, prevContext)
		{
			_elseIfBlocks = _parser._pool.Allocate<ElseIfBlockSyntax>();
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.ElseIfStatement:
				return new IfPartContext(SyntaxKind.ElseIfBlock, (StatementSyntax)node, this);
			case SyntaxKind.ElseIfBlock:
				_elseIfBlocks.Add((ElseIfBlockSyntax)node);
				break;
			case SyntaxKind.ElseStatement:
				return new IfPartContext(SyntaxKind.ElseBlock, (StatementSyntax)node, this);
			case SyntaxKind.ElseBlock:
				_optionalElseBlock = (ElseBlockSyntax)node;
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
			case SyntaxKind.ElseIfStatement:
			case SyntaxKind.ElseStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.ElseIfBlock:
			case SyntaxKind.ElseBlock:
				return UseSyntax(node, ref newContext) | LinkResult.SkipTerminator;
			default:
				return base.TryLinkSyntax(node, ref newContext);
			}
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			StatementSyntax statementSyntax = base.BeginStatement;
			if (endStmt == null)
			{
				statementSyntax = Parser.ReportSyntaxError(statementSyntax, ERRID.ERR_ExpectedEndIf);
				endStmt = base.SyntaxFactory.EndIfStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.IfKeyword));
			}
			MultiLineIfBlockSyntax result = base.SyntaxFactory.MultiLineIfBlock((IfStatementSyntax)statementSyntax, Body(), _elseIfBlocks.ToList(), _optionalElseBlock, (EndBlockStatementSyntax)endStmt);
			_parser._pool.Free(_elseIfBlocks);
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
