using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VariablesDeclaredWalker : AbstractRegionControlFlowPass
	{
		private readonly HashSet<Symbol> _variablesDeclared;

		internal static IEnumerable<Symbol> Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
		{
			VariablesDeclaredWalker variablesDeclaredWalker = new VariablesDeclaredWalker(info, region);
			try
			{
				return variablesDeclaredWalker.Analyze() ? variablesDeclaredWalker._variablesDeclared : SpecializedCollections.EmptyEnumerable<Symbol>();
			}
			finally
			{
				variablesDeclaredWalker.Free();
			}
		}

		private new bool Analyze()
		{
			return Scan();
		}

		private VariablesDeclaredWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region)
		{
			_variablesDeclared = new HashSet<Symbol>();
		}

		public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			if (base.IsInside)
			{
				_variablesDeclared.Add(node.LocalSymbol);
			}
			return base.VisitLocalDeclaration(node);
		}

		protected override void VisitForStatementVariableDeclaration(BoundForStatement node)
		{
			if (base.IsInside && (object)node.DeclaredOrInferredLocalOpt != null)
			{
				_variablesDeclared.Add(node.DeclaredOrInferredLocalOpt);
			}
			base.VisitForStatementVariableDeclaration(node);
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			if (base.IsInside)
			{
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.LambdaSymbol.Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					_variablesDeclared.Add(current);
				}
			}
			return base.VisitLambda(node);
		}

		public override BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			base.VisitQueryableSource(node);
			if (!node.WasCompilerGenerated && node.RangeVariables.Length > 0 && base.IsInside)
			{
				_variablesDeclared.Add(node.RangeVariables[0]);
			}
			return null;
		}

		public override BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			if (!node.WasCompilerGenerated && base.IsInside)
			{
				_variablesDeclared.Add(node.RangeVariable);
			}
			base.VisitRangeVariableAssignment(node);
			return null;
		}

		protected override void VisitCatchBlock(BoundCatchBlock catchBlock, ref LocalState finallyState)
		{
			if (IsInsideRegion(catchBlock.Syntax.Span) && (object)catchBlock.LocalOpt != null)
			{
				_variablesDeclared.Add(catchBlock.LocalOpt);
			}
			base.VisitCatchBlock(catchBlock, ref finallyState);
		}
	}
}
