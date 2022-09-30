namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct BoundNodeSummary
	{
		public readonly BoundNode LowestBoundNode;

		public readonly BoundNode HighestBoundNode;

		public readonly BoundNode LowestBoundNodeOfSyntacticParent;

		public BoundNodeSummary(BoundNode lowestBound, BoundNode highestBound, BoundNode lowestBoundOfSyntacticParent)
		{
			this = default(BoundNodeSummary);
			LowestBoundNode = lowestBound;
			HighestBoundNode = highestBound;
			LowestBoundNodeOfSyntacticParent = lowestBoundOfSyntacticParent;
		}
	}
}
