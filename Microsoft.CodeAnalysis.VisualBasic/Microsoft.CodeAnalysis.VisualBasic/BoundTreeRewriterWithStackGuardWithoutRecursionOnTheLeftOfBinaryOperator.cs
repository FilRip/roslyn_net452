using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator : BoundTreeRewriterWithStackGuard
	{
		protected BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator()
		{
		}

		protected BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator(int recursionDepth)
			: base(recursionDepth)
		{
		}

		public sealed override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
		{
			BoundExpression left = node.Left;
			if (left.Kind != BoundKind.BinaryOperator)
			{
				return base.VisitBinaryOperator(node);
			}
			ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
			instance.Push(node);
			BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)left;
			while (true)
			{
				instance.Push(boundBinaryOperator);
				left = boundBinaryOperator.Left;
				if (left.Kind != BoundKind.BinaryOperator)
				{
					break;
				}
				boundBinaryOperator = (BoundBinaryOperator)left;
			}
			BoundExpression boundExpression = (BoundExpression)Visit(left);
			do
			{
				boundBinaryOperator = instance.Pop();
				BoundExpression right = (BoundExpression)Visit(boundBinaryOperator.Right);
				TypeSymbol type = VisitType(boundBinaryOperator.Type);
				boundExpression = boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpression, right, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, type);
			}
			while (instance.Count > 0);
			instance.Free();
			return boundExpression;
		}
	}
}
