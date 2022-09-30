using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class AbstractRegionDataFlowPass : DataFlowPass
	{
		protected override bool SuppressRedimOperandRvalueOnPreserve => false;

		protected override bool IgnoreOutSemantics => false;

		protected override bool EnableBreakingFlowAnalysisFeatures => true;

		internal AbstractRegionDataFlowPass(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, HashSet<Symbol> initiallyAssignedVariables = null, bool trackUnassignments = false, bool trackStructsWithIntrinsicTypedFields = false)
			: base(info, region, suppressConstExpressionsSupport: false, initiallyAssignedVariables, trackUnassignments, trackStructsWithIntrinsicTypedFields)
		{
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			MakeSlots(node.LambdaSymbol.Parameters);
			return base.VisitLambda(node);
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

		public override BoundNode VisitParameter(BoundParameter node)
		{
			if (node.ParameterSymbol.ContainingSymbol.IsQueryLambdaMethod)
			{
				return null;
			}
			return base.VisitParameter(node);
		}

		protected override LocalSymbol CreateLocalSymbolForVariables(ImmutableArray<BoundLocalDeclaration> declarations)
		{
			if (declarations.Length == 1)
			{
				return declarations[0].LocalSymbol;
			}
			LocalSymbol[] array = new LocalSymbol[declarations.Length - 1 + 1];
			int num = declarations.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = declarations[i].LocalSymbol;
			}
			return AmbiguousLocalsPseudoSymbol.Create(array.AsImmutableOrNull());
		}
	}
}
