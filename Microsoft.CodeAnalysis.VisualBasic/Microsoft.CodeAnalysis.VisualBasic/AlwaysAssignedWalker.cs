using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class AlwaysAssignedWalker : AbstractRegionDataFlowPass
	{
		private LocalState _endOfRegionState;

		private readonly HashSet<LabelSymbol> _labelsInside;

		private IEnumerable<Symbol> AlwaysAssigned
		{
			get
			{
				List<Symbol> list = new List<Symbol>();
				if (_endOfRegionState.Reachable)
				{
					foreach (int item in _endOfRegionState.Assigned.TrueBits())
					{
						if (item < variableBySlot.Length)
						{
							VariableIdentifier variableIdentifier = variableBySlot[item];
							if (variableIdentifier.Exists && variableIdentifier.Symbol.Kind != SymbolKind.Field)
							{
								list.Add(variableIdentifier.Symbol);
							}
						}
					}
					return list;
				}
				return list;
			}
		}

		private AlwaysAssignedWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region)
		{
			_labelsInside = new HashSet<LabelSymbol>();
		}

		internal static IEnumerable<Symbol> Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
		{
			AlwaysAssignedWalker alwaysAssignedWalker = new AlwaysAssignedWalker(info, region);
			try
			{
				return alwaysAssignedWalker.Analyze() ? alwaysAssignedWalker.AlwaysAssigned : SpecializedCollections.EmptyEnumerable<Symbol>();
			}
			finally
			{
				alwaysAssignedWalker.Free();
			}
		}

		protected override void EnterRegion()
		{
			SetState(ReachableState());
			base.EnterRegion();
		}

		protected override void LeaveRegion()
		{
			if (IsConditionalState)
			{
				_endOfRegionState = StateWhenTrue.Clone();
				IntersectWith(ref _endOfRegionState, ref StateWhenFalse);
			}
			else
			{
				_endOfRegionState = State.Clone();
			}
			ImmutableArray<PendingBranch>.Enumerator enumerator = base.PendingBranches.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PendingBranch current = enumerator.Current;
				if (IsInsideRegion(current.Branch.Syntax.Span) && !_labelsInside.Contains(current.Label))
				{
					IntersectWith(ref _endOfRegionState, ref current.State);
				}
			}
			base.LeaveRegion();
		}

		public override BoundNode VisitLabelStatement(BoundLabelStatement node)
		{
			if (node.Syntax != null && IsInsideRegion(node.Syntax.Span))
			{
				_labelsInside.Add(node.Label);
			}
			return base.VisitLabelStatement(node);
		}

		protected override void ResolveBranch(PendingBranch pending, LabelSymbol label, BoundLabelStatement target, ref bool labelStateChanged)
		{
			if (base.IsInside && pending.Branch != null && !IsInsideRegion(pending.Branch.Syntax.Span))
			{
				pending.State = (pending.State.Reachable ? ReachableState() : UnreachableState());
			}
			base.ResolveBranch(pending, label, target, ref labelStateChanged);
		}

		protected override void WriteArgument(BoundExpression arg, bool isOut)
		{
			if (isOut)
			{
				base.WriteArgument(arg, isOut);
			}
		}
	}
}
