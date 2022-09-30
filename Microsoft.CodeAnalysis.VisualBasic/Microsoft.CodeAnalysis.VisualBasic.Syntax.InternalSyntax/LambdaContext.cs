using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LambdaContext : MethodBlockContext
	{
		internal override bool IsLambda => true;

		internal override bool IsSingleLine => false;

		internal LambdaContext(StatementSyntax statement, BlockContext prevContext)
			: base((statement.Kind == SyntaxKind.FunctionLambdaHeader) ? SyntaxKind.MultiLineFunctionLambdaExpression : SyntaxKind.MultiLineSubLambdaExpression, statement, prevContext)
		{
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			LambdaHeaderSyntax beginStmt = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = Body();
			EndBlockStatementSyntax endStmt2 = (EndBlockStatementSyntax)endStmt;
			GetBeginEndStatements(ref beginStmt, ref endStmt2);
			MultiLineLambdaExpressionSyntax result = base.SyntaxFactory.MultiLineLambdaExpression(base.BlockKind, beginStmt, syntaxList, endStmt2);
			FreeStatements();
			return result;
		}

		internal override BlockContext EndBlock(StatementSyntax endStmt)
		{
			return base.PrevBlock;
		}
	}
}
