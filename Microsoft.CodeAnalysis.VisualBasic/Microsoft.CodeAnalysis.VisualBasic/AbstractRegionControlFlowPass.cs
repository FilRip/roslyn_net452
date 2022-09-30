namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class AbstractRegionControlFlowPass : ControlFlowPass
	{
		internal AbstractRegionControlFlowPass(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region, suppressConstantExpressionsSupport: false)
		{
		}

		protected override void Visit(BoundNode node, bool dontLeaveRegion)
		{
			VisitAlways(node, dontLeaveRegion);
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			SavedPending oldPending = SavePending();
			Symbol symbol = base.symbol;
			base.symbol = node.LambdaSymbol;
			LocalState self = State;
			SetState(ReachableState());
			VisitBlock(node.Body);
			base.symbol = symbol;
			RestorePending(oldPending);
			IntersectWith(ref self, ref State);
			SetState(self);
			return null;
		}

		public override BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			VisitRvalue(node.Expression);
			return null;
		}

		public override BoundNode VisitQueryExpression(BoundQueryExpression node)
		{
			VisitRvalue(node.LastOperator);
			return null;
		}

		public override BoundNode VisitQuerySource(BoundQuerySource node)
		{
			VisitRvalue(node.Expression);
			return null;
		}

		public override BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			VisitRvalue(node.Source);
			return null;
		}

		public override BoundNode VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node)
		{
			VisitRvalue(node.ConversionCall);
			return null;
		}

		public override BoundNode VisitQueryClause(BoundQueryClause node)
		{
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitAggregateClause(BoundAggregateClause node)
		{
			if (node.CapturedGroupOpt != null)
			{
				VisitRvalue(node.CapturedGroupOpt);
			}
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitOrdering(BoundOrdering node)
		{
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			VisitRvalue(node.Value);
			return null;
		}
	}
}
