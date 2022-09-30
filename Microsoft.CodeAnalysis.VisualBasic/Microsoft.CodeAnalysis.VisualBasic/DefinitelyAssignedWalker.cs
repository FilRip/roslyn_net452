using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class DefinitelyAssignedWalker : AbstractRegionDataFlowPass
	{
		private readonly HashSet<Symbol> _definitelyAssignedOnEntry;

		private readonly HashSet<Symbol> _definitelyAssignedOnExit;

		private DefinitelyAssignedWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region)
		{
			_definitelyAssignedOnEntry = new HashSet<Symbol>();
			_definitelyAssignedOnExit = new HashSet<Symbol>();
		}

		internal static (HashSet<Symbol> entry, HashSet<Symbol> ex) Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
		{
			DefinitelyAssignedWalker definitelyAssignedWalker = new DefinitelyAssignedWalker(info, region);
			try
			{
				return definitelyAssignedWalker.Analyze() ? (definitelyAssignedWalker._definitelyAssignedOnEntry, definitelyAssignedWalker._definitelyAssignedOnExit) : (new HashSet<Symbol>(), new HashSet<Symbol>());
			}
			finally
			{
				definitelyAssignedWalker.Free();
			}
		}

		protected override void EnterRegion()
		{
			ProcessRegion(_definitelyAssignedOnEntry);
			base.EnterRegion();
		}

		protected override void LeaveRegion()
		{
			ProcessRegion(_definitelyAssignedOnExit);
			base.LeaveRegion();
		}

		private void ProcessRegion(HashSet<Symbol> definitelyAssigned)
		{
			definitelyAssigned.Clear();
			if (IsConditionalState)
			{
				ProcessState(definitelyAssigned, StateWhenTrue, StateWhenFalse);
			}
			else
			{
				ProcessState(definitelyAssigned, State, null);
			}
		}

		private void ProcessState(HashSet<Symbol> definitelyAssigned, LocalState state1, LocalState? state2opt)
		{
			foreach (int item in state1.Assigned.TrueBits())
			{
				if (item < variableBySlot.Length && (!state2opt.HasValue || state2opt.Value.IsAssigned(item)))
				{
					Symbol symbol = variableBySlot[item].Symbol;
					if ((object)symbol != null && symbol.Kind != SymbolKind.Field)
					{
						definitelyAssigned.Add(symbol);
					}
				}
			}
		}
	}
}
