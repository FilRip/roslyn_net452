namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundNodeFinder : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
	{
		private readonly bool _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;

		private BoundNode _nodeToFind;

		public static bool ContainsNode(BoundNode findWhere, BoundNode findWhat, int recursionDepth, bool convertInsufficientExecutionStackExceptionToCancelledByStackGuardException)
		{
			if (findWhere == findWhat)
			{
				return true;
			}
			BoundNodeFinder boundNodeFinder = new BoundNodeFinder(findWhat, recursionDepth, convertInsufficientExecutionStackExceptionToCancelledByStackGuardException);
			boundNodeFinder.Visit(findWhere);
			return boundNodeFinder._nodeToFind == null;
		}

		private BoundNodeFinder(BoundNode _nodeToFind, int recursionDepth, bool convertInsufficientExecutionStackExceptionToCancelledByStackGuardException)
			: base(recursionDepth)
		{
			this._nodeToFind = _nodeToFind;
			_convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;
		}

		public override BoundNode Visit(BoundNode node)
		{
			if (_nodeToFind != null)
			{
				if (_nodeToFind == node)
				{
					_nodeToFind = null;
				}
				else
				{
					base.Visit(node);
				}
			}
			return null;
		}

		protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
		{
			return _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;
		}

		public override BoundNode VisitUnboundLambda(UnboundLambda node)
		{
			Visit(node.BindForErrorRecovery());
			return null;
		}
	}
}
