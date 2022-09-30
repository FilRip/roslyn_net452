using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class StatementWalker : BoundTreeWalker
	{
		protected override BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
