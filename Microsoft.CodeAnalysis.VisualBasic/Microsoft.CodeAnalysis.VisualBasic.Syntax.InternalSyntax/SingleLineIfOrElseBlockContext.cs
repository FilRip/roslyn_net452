using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class SingleLineIfOrElseBlockContext : ExecutableStatementContext
	{
		internal override bool IsSingleLine => true;

		protected bool TreatOtherAsStatementTerminator
		{
			get
			{
				BlockContext prevBlock = base.PrevBlock;
				while (true)
				{
					switch (prevBlock.BlockKind)
					{
					case SyntaxKind.SingleLineIfStatement:
					case SyntaxKind.SingleLineElseClause:
						break;
					case SyntaxKind.SingleLineSubLambdaExpression:
						return true;
					default:
						return false;
					}
					prevBlock = prevBlock.PrevBlock;
				}
			}
		}

		protected SingleLineIfOrElseBlockContext(SyntaxKind kind, StatementSyntax statement, BlockContext prevContext)
			: base(kind, statement, prevContext)
		{
		}

		protected BlockContext ProcessOtherAsStatementTerminator()
		{
			BlockContext blockContext = EndBlock(null);
			while (true)
			{
				switch (blockContext.BlockKind)
				{
				case SyntaxKind.SingleLineIfStatement:
				case SyntaxKind.SingleLineElseClause:
					break;
				case SyntaxKind.SingleLineSubLambdaExpression:
					return blockContext.PrevBlock;
				default:
					throw ExceptionUtilities.UnexpectedValue(blockContext.BlockKind);
				}
				blockContext = blockContext.EndBlock(null);
			}
		}
	}
}
