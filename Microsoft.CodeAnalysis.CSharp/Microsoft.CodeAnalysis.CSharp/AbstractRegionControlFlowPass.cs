using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class AbstractRegionControlFlowPass : ControlFlowPass
    {
        internal AbstractRegionControlFlowPass(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion)
            : base(compilation, member, node, firstInRegion, lastInRegion)
        {
        }

        public override BoundNode Visit(BoundNode node)
        {
            VisitAlways(node);
            return null;
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            SavedPending oldPending = SavePending();
            LocalState self = State;
            State = TopState();
            SavedPending oldPending2 = SavePending();
            VisitAlways(node.Body);
            RestorePending(oldPending2);
            ImmutableArray<PendingBranch> immutableArray = RemoveReturns();
            RestorePending(oldPending);
            Join(ref self, ref State);
            ImmutableArray<PendingBranch>.Enumerator enumerator = immutableArray.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PendingBranch current = enumerator.Current;
                State = current.State;
                Join(ref self, ref State);
            }
            State = self;
            return null;
        }
    }
}
