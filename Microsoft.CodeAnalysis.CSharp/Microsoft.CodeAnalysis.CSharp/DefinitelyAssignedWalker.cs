using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class DefinitelyAssignedWalker : AbstractRegionDataFlowPass
    {
        private readonly HashSet<Symbol> _definitelyAssignedOnEntry = new HashSet<Symbol>();

        private readonly HashSet<Symbol> _definitelyAssignedOnExit = new HashSet<Symbol>();

        private DefinitelyAssignedWalker(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion)
            : base(compilation, member, node, firstInRegion, lastInRegion)
        {
        }

        internal static (HashSet<Symbol> entry, HashSet<Symbol> exit) Analyze(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion)
        {
            DefinitelyAssignedWalker definitelyAssignedWalker = new DefinitelyAssignedWalker(compilation, member, node, firstInRegion, lastInRegion);
            try
            {
                bool badRegion = false;
                definitelyAssignedWalker.Analyze(ref badRegion, null);
                return badRegion ? (new HashSet<Symbol>(), new HashSet<Symbol>()) : (definitelyAssignedWalker._definitelyAssignedOnEntry, definitelyAssignedWalker._definitelyAssignedOnExit);
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
                if (item < variableBySlot.Count && (!state2opt.HasValue || state2opt.GetValueOrDefault().IsAssigned(item)))
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
