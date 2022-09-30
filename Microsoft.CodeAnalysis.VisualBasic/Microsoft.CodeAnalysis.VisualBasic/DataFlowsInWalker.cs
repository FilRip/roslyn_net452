using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class DataFlowsInWalker : AbstractRegionDataFlowPass
	{
		private readonly HashSet<Symbol> _dataFlowsIn;

		private DataFlowsInWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, HashSet<Symbol> unassignedVariables)
			: base(info, region, unassignedVariables, trackUnassignments: false, trackStructsWithIntrinsicTypedFields: true)
		{
			_dataFlowsIn = new HashSet<Symbol>();
		}

		internal static HashSet<Symbol> Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, HashSet<Symbol> unassignedVariables, ref bool? succeeded, ref bool invalidRegionDetected)
		{
			HashSet<Symbol> hashSet = new HashSet<Symbol>();
			foreach (Symbol unassignedVariable in unassignedVariables)
			{
				if (unassignedVariable.Kind != SymbolKind.Local || !((LocalSymbol)unassignedVariable).IsStatic)
				{
					hashSet.Add(unassignedVariable);
				}
			}
			DataFlowsInWalker dataFlowsInWalker = new DataFlowsInWalker(info, region, hashSet);
			try
			{
				succeeded = dataFlowsInWalker.Analyze() && !dataFlowsInWalker.InvalidRegionDetected;
				invalidRegionDetected = dataFlowsInWalker.InvalidRegionDetected;
				return succeeded.GetValueOrDefault() ? dataFlowsInWalker._dataFlowsIn : new HashSet<Symbol>();
			}
			finally
			{
				dataFlowsInWalker.Free();
			}
		}

		private LocalState ResetState(LocalState state)
		{
			bool num = !state.Reachable;
			state = ReachableState();
			if (num)
			{
				state.Assign(0);
			}
			return state;
		}

		protected override void EnterRegion()
		{
			SetState(ResetState(State));
			_dataFlowsIn.Clear();
			base.EnterRegion();
		}

		protected override void NoteBranch(PendingBranch pending, BoundStatement stmt, BoundLabelStatement labelStmt)
		{
			if (stmt.Syntax != null && labelStmt.Syntax != null && !IsInsideRegion(stmt.Syntax.Span) && IsInsideRegion(labelStmt.Syntax.Span))
			{
				pending.State = ResetState(pending.State);
			}
			base.NoteBranch(pending, stmt, labelStmt);
		}

		public override BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			if (!node.WasCompilerGenerated && base.IsInside && !IsInsideRegion(node.RangeVariable.Syntax.Span))
			{
				_dataFlowsIn.Add(node.RangeVariable);
			}
			return null;
		}

		protected override void VisitAmbiguousLocalSymbol(AmbiguousLocalsPseudoSymbol ambiguous)
		{
			base.VisitAmbiguousLocalSymbol(ambiguous);
			if (base.IsInside)
			{
				LocalSymbol localSymbol = ambiguous.Locals[0];
				if (!State.IsAssigned(VariableSlot(localSymbol)))
				{
					SetInvalidRegion();
				}
			}
		}

		protected override void ReportUnassigned(Symbol local, SyntaxNode node, ReadWriteContext rwContext, int slot = -1, BoundFieldAccess boundFieldAccess = null)
		{
			if (IsInsideRegion(node.Span))
			{
				if (local.Kind == SymbolKind.Field)
				{
					Symbol nodeSymbol = GetNodeSymbol(boundFieldAccess);
					if ((object)nodeSymbol != null)
					{
						_dataFlowsIn.Add(nodeSymbol);
					}
				}
				else
				{
					_dataFlowsIn.Add(local);
				}
			}
			base.ReportUnassigned(local, node, rwContext, slot, boundFieldAccess);
		}

		internal override void AssignLocalOnDeclaration(LocalSymbol local, BoundLocalDeclaration node)
		{
			if (!local.IsStatic)
			{
				base.AssignLocalOnDeclaration(local, node);
			}
		}
	}
}
