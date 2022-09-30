using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SingleLineIfBlockContext : SingleLineIfOrElseBlockContext
	{
		private SingleLineElseClauseSyntax _optionalElseClause;

		private bool _haveElseClause;

		internal SingleLineIfBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.SingleLineIfStatement, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.IfStatement:
			{
				IfStatementSyntax ifStatementSyntax = (IfStatementSyntax)node;
				if (ifStatementSyntax.ThenKeyword != null && !SyntaxFacts.IsTerminator(base.Parser.CurrentToken.Kind))
				{
					return new SingleLineIfBlockContext(ifStatementSyntax, this);
				}
				break;
			}
			case SyntaxKind.ElseIfStatement:
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_ExpectedEOS));
				return EndBlock(null);
			case SyntaxKind.ElseStatement:
				if (_haveElseClause)
				{
					throw ExceptionUtilities.Unreachable;
				}
				_haveElseClause = true;
				return new SingleLineElseContext(SyntaxKind.SingleLineElseClause, (StatementSyntax)node, this);
			case SyntaxKind.SingleLineElseClause:
				_optionalElseClause = (SingleLineElseClauseSyntax)node;
				return this;
			case SyntaxKind.CatchStatement:
			case SyntaxKind.FinallyStatement:
				Add(Parser.ReportSyntaxError(node, (node.Kind == SyntaxKind.CatchStatement) ? ERRID.ERR_CatchNoMatchingTry : ERRID.ERR_FinallyNoMatchingTry));
				return EndBlock(null);
			}
			return base.ProcessSyntax(node);
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			return CreateIfBlockSyntax();
		}

		private SingleLineIfStatementSyntax CreateIfBlockSyntax()
		{
			IfStatementSyntax ifStatementSyntax = (IfStatementSyntax)base.BeginStatement;
			SingleLineIfStatementSyntax result = base.SyntaxFactory.SingleLineIfStatement(ifStatementSyntax.IfKeyword, ifStatementSyntax.Condition, ifStatementSyntax.ThenKeyword, Body(), _optionalElseClause);
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax statement)
		{
			SingleLineIfStatementSyntax syntax = CreateIfBlockSyntax();
			return base.PrevBlock.ProcessSyntax(syntax);
		}

		internal override BlockContext ProcessStatementTerminator(BlockContext lambdaContext)
		{
			SyntaxToken currentToken = base.Parser.CurrentToken;
			switch (currentToken.Kind)
			{
			case SyntaxKind.StatementTerminatorToken:
			case SyntaxKind.EndOfFileToken:
				return EndBlock(null).ProcessStatementTerminator(lambdaContext);
			case SyntaxKind.ColonToken:
				base.Parser.ConsumeColonInSingleLineExpression();
				return this;
			default:
				throw ExceptionUtilities.UnexpectedValue(currentToken.Kind);
			}
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
				base.Parser.ConsumedStatementTerminator(allowLeadingMultilineTrivia: false);
				return this;
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
	}
}
