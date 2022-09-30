namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundTreeRewriterWithStackGuard : BoundTreeRewriter
	{
		private int _recursionDepth;

		protected int RecursionDepth => _recursionDepth;

		protected BoundTreeRewriterWithStackGuard()
		{
		}

		protected BoundTreeRewriterWithStackGuard(int recursionDepth)
		{
			_recursionDepth = recursionDepth;
		}

		public override BoundNode Visit(BoundNode node)
		{
			if (node is BoundExpression node2)
			{
				return VisitExpressionWithStackGuard(ref _recursionDepth, node2);
			}
			return base.Visit(node);
		}

		protected BoundExpression VisitExpressionWithStackGuard(BoundExpression expression)
		{
			return VisitExpressionWithStackGuard(ref _recursionDepth, expression);
		}

		protected sealed override BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node)
		{
			return (BoundExpression)base.Visit(node);
		}
	}
}
