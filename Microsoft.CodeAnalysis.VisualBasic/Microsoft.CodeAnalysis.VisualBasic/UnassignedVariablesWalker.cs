using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class UnassignedVariablesWalker : DataFlowPass
	{
		private readonly HashSet<Symbol> _result;

		protected override bool SuppressRedimOperandRvalueOnPreserve => false;

		protected override bool IgnoreOutSemantics => false;

		protected override bool EnableBreakingFlowAnalysisFeatures => true;

		private UnassignedVariablesWalker(FlowAnalysisInfo info)
			: base(info, suppressConstExpressionsSupport: false, trackStructsWithIntrinsicTypedFields: true)
		{
			_result = new HashSet<Symbol>();
		}

		internal static HashSet<Symbol> Analyze(FlowAnalysisInfo info)
		{
			UnassignedVariablesWalker unassignedVariablesWalker = new UnassignedVariablesWalker(info);
			try
			{
				return unassignedVariablesWalker.Analyze() ? unassignedVariablesWalker._result : new HashSet<Symbol>();
			}
			finally
			{
				unassignedVariablesWalker.Free();
			}
		}

		protected override void ReportUnassigned(Symbol local, SyntaxNode node, ReadWriteContext rwContext, int slot = -1, BoundFieldAccess boundFieldAccess = null)
		{
			if (local.Kind == SymbolKind.Field)
			{
				Symbol nodeSymbol = GetNodeSymbol(boundFieldAccess);
				if ((object)nodeSymbol != null)
				{
					_result.Add(nodeSymbol);
				}
			}
			else
			{
				_result.Add(local);
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
