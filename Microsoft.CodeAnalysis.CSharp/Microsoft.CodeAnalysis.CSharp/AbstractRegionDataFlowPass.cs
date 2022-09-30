using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class AbstractRegionDataFlowPass : DefiniteAssignmentPass
    {
        internal AbstractRegionDataFlowPass(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion, HashSet<Symbol> initiallyAssignedVariables = null, HashSet<PrefixUnaryExpressionSyntax> unassignedVariableAddressOfSyntaxes = null, bool trackUnassignments = false)
            : base(compilation, member, node, firstInRegion, lastInRegion, initiallyAssignedVariables, unassignedVariableAddressOfSyntaxes, trackUnassignments)
        {
        }

        protected override ImmutableArray<AbstractFlowPass<LocalState, LocalFunctionState>.PendingBranch> Scan(ref bool badRegion)
        {
            MakeSlots(base.MethodParameters);
            if ((object)base.MethodThisParameter != null)
            {
                GetOrCreateSlot(base.MethodThisParameter);
            }
            return base.Scan(ref badRegion);
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            MakeSlots(node.Symbol.Parameters);
            return base.VisitLambda(node);
        }

        public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            MakeSlots(node.Symbol.Parameters);
            return base.VisitLocalFunctionStatement(node);
        }

        private void MakeSlots(ImmutableArray<ParameterSymbol> parameters)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                GetOrCreateSlot(current);
            }
        }
    }
}
