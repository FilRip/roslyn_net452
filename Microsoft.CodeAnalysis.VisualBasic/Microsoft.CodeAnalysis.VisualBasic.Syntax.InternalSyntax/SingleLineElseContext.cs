using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SingleLineElseContext : SingleLineIfOrElseBlockContext
	{
		private bool TreatElseAsStatementTerminator
		{
			get
			{
				BlockContext prevBlock = base.PrevBlock.PrevBlock;
				while (prevBlock.BlockKind != SyntaxKind.SingleLineIfStatement)
				{
					switch (prevBlock.BlockKind)
					{
					case SyntaxKind.SingleLineElseClause:
						break;
					case SyntaxKind.SingleLineSubLambdaExpression:
						return true;
					default:
						return false;
					}
					prevBlock = prevBlock.PrevBlock.PrevBlock;
				}
				return true;
			}
		}

		internal SingleLineElseContext(SyntaxKind kind, StatementSyntax statement, BlockContext prevContext)
			: base(kind, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.IfStatement:
				if (((IfStatementSyntax)node).ThenKeyword != null)
				{
					SyntaxFacts.IsTerminator(base.Parser.CurrentToken.Kind);
					return base.ProcessSyntax(node);
				}
				break;
			case SyntaxKind.ElseIfStatement:
				return EndBlock(null).ProcessSyntax(node);
			case SyntaxKind.CatchStatement:
			case SyntaxKind.FinallyStatement:
				Add(Parser.ReportSyntaxError(node, (node.Kind == SyntaxKind.CatchStatement) ? ERRID.ERR_CatchNoMatchingTry : ERRID.ERR_FinallyNoMatchingTry));
				return EndBlock(null);
			}
			return base.ProcessSyntax(node);
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			return CreateElseBlockSyntax();
		}

		private SingleLineElseClauseSyntax CreateElseBlockSyntax()
		{
			ElseStatementSyntax elseStatementSyntax = (ElseStatementSyntax)base.BeginStatement;
			SingleLineElseClauseSyntax result = base.SyntaxFactory.SingleLineElseClause(elseStatementSyntax.ElseKeyword, OptionalBody());
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax statement)
		{
			return base.PrevBlock.ProcessSyntax(CreateElseBlockSyntax()).EndBlock(null);
		}

		internal override BlockContext ProcessStatementTerminator(BlockContext lambdaContext)
		{
			SyntaxToken currentToken = base.Parser.CurrentToken;
			SyntaxKind kind = currentToken.Kind;
			if (kind != SyntaxKind.ColonToken)
			{
				if (kind - 677 > SyntaxKind.List)
				{
					throw ExceptionUtilities.UnexpectedValue(currentToken.Kind);
				}
			}
			else if (_statements.Count > 0)
			{
				base.Parser.ConsumeColonInSingleLineExpression();
				return this;
			}
			return EndBlock(null).ProcessStatementTerminator(lambdaContext);
		}

		internal override BlockContext ResyncAndProcessStatementTerminator(StatementSyntax statement, BlockContext lambdaContext)
		{
			switch (base.Parser.CurrentToken.Kind)
			{
			case SyntaxKind.ColonToken:
			case SyntaxKind.StatementTerminatorToken:
			case SyntaxKind.EndOfFileToken:
				return ProcessStatementTerminator(lambdaContext);
			case SyntaxKind.ElseKeyword:
				if (TreatElseAsStatementTerminator)
				{
					base.Parser.ConsumedStatementTerminator(allowLeadingMultilineTrivia: false);
					return ProcessElseAsStatementTerminator();
				}
				return base.ResyncAndProcessStatementTerminator(statement, lambdaContext);
			default:
				if (_statements.Count > 0)
				{
					if (base.TreatOtherAsStatementTerminator)
					{
						return ProcessOtherAsStatementTerminator();
					}
					return base.ResyncAndProcessStatementTerminator(statement, lambdaContext);
				}
				base.Parser.ConsumedStatementTerminator(allowLeadingMultilineTrivia: false);
				return this;
			}
		}

		private BlockContext ProcessElseAsStatementTerminator()
		{
			BlockContext blockContext = EndBlock(null);
			while (blockContext.BlockKind != SyntaxKind.SingleLineIfStatement)
			{
				switch (blockContext.BlockKind)
				{
				case SyntaxKind.SingleLineElseClause:
					break;
				case SyntaxKind.SingleLineSubLambdaExpression:
					return blockContext.PrevBlock;
				default:
					throw ExceptionUtilities.UnexpectedValue(blockContext.BlockKind);
				}
				blockContext = blockContext.EndBlock(null);
			}
			return blockContext;
		}
	}
}
