using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class DataFlowsOutWalker : AbstractRegionDataFlowPass
	{
		private readonly ImmutableArray<ISymbol> _dataFlowsIn;

		private readonly HashSet<Symbol> _originalUnassigned;

		private readonly HashSet<Symbol> _dataFlowsOut;

		private DataFlowsOutWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, HashSet<Symbol> unassignedVariables, HashSet<Symbol> originalUnassigned, ImmutableArray<ISymbol> dataFlowsIn)
			: base(info, region, unassignedVariables, trackUnassignments: true, trackStructsWithIntrinsicTypedFields: true)
		{
			_dataFlowsOut = new HashSet<Symbol>();
			_dataFlowsIn = dataFlowsIn;
			_originalUnassigned = originalUnassigned;
		}

		internal static HashSet<Symbol> Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, HashSet<Symbol> unassignedVariables, ImmutableArray<ISymbol> dataFlowsIn)
		{
			HashSet<Symbol> hashSet = new HashSet<Symbol>();
			foreach (Symbol unassignedVariable in unassignedVariables)
			{
				if (unassignedVariable.Kind != SymbolKind.Local || !((LocalSymbol)unassignedVariable).IsStatic)
				{
					hashSet.Add(unassignedVariable);
				}
			}
			DataFlowsOutWalker dataFlowsOutWalker = new DataFlowsOutWalker(info, region, hashSet, unassignedVariables, dataFlowsIn);
			try
			{
				bool num = dataFlowsOutWalker.Analyze();
				HashSet<Symbol> dataFlowsOut = dataFlowsOutWalker._dataFlowsOut;
				return num ? dataFlowsOut : new HashSet<Symbol>();
			}
			finally
			{
				dataFlowsOutWalker.Free();
			}
		}

		protected override void EnterRegion()
		{
			ImmutableArray<ISymbol>.Enumerator enumerator = _dataFlowsIn.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol symbol = (Symbol)enumerator.Current;
				int orCreateSlot = GetOrCreateSlot(symbol);
				if (!State.IsAssigned(orCreateSlot) && symbol.Kind != SymbolKind.RangeVariable && (symbol.Kind != SymbolKind.Local || !((LocalSymbol)symbol).IsStatic))
				{
					_dataFlowsOut.Add(symbol);
				}
			}
			base.EnterRegion();
		}

		protected override void Assign(BoundNode node, BoundExpression value, bool assigned = true)
		{
			if (base.IsInside)
			{
				if (assigned)
				{
					GetNodeSymbol(node);
				}
				assigned = false;
				if (State.Reachable)
				{
					switch (node.Kind)
					{
					case BoundKind.MeReference:
					case BoundKind.Parameter:
					{
						BoundExpression node2 = (BoundExpression)node;
						SlotCollection slotCollection2 = MakeSlotsForExpression(node2);
						if (slotCollection2.Count <= 0)
						{
							break;
						}
						int num2 = slotCollection2[0];
						if (num2 >= 2)
						{
							ParameterSymbol parameterSymbol = (ParameterSymbol)variableBySlot[num2].Symbol;
							if ((object)parameterSymbol != null && parameterSymbol.IsByRef)
							{
								_dataFlowsOut.Add(parameterSymbol);
							}
						}
						break;
					}
					case BoundKind.Local:
					{
						BoundLocal boundLocal = (BoundLocal)node;
						LocalSymbol localSymbol = boundLocal.LocalSymbol;
						if (!localSymbol.IsStatic || !WasUsedBeforeAssignment(localSymbol))
						{
							break;
						}
						SlotCollection slotCollection = MakeSlotsForExpression(boundLocal);
						if (slotCollection.Count > 0)
						{
							int num = slotCollection[0];
							if (num >= 2)
							{
								_dataFlowsOut.Add(variableBySlot[num].Symbol);
							}
						}
						break;
					}
					}
				}
			}
			base.Assign(node, value, assigned);
		}

		protected override void ReportUnassigned(Symbol local, SyntaxNode node, ReadWriteContext rwContext, int slot = -1, BoundFieldAccess boundFieldAccess = null)
		{
			if (!_dataFlowsOut.Contains(local) && local.Kind != SymbolKind.RangeVariable && !base.IsInside)
			{
				if (local.Kind == SymbolKind.Field)
				{
					Symbol nodeSymbol = GetNodeSymbol(boundFieldAccess);
					if ((object)nodeSymbol != null)
					{
						_dataFlowsOut.Add(nodeSymbol);
					}
				}
				else
				{
					_dataFlowsOut.Add(local);
				}
			}
			base.ReportUnassigned(local, node, rwContext, slot, boundFieldAccess);
		}

		protected override void ReportUnassignedByRefParameter(ParameterSymbol parameter)
		{
			_dataFlowsOut.Add(parameter);
			base.ReportUnassignedByRefParameter(parameter);
		}

		protected override void NoteWrite(Symbol variable, BoundExpression value)
		{
			if (State.Reachable)
			{
				bool flag = variable.Kind == SymbolKind.Parameter && ((ParameterSymbol)variable).IsByRef;
				bool flag2 = variable.Kind == SymbolKind.Local && ((LocalSymbol)variable).IsStatic;
				if (base.IsInside && (flag || (flag2 && WasUsedBeforeAssignment(variable))))
				{
					_dataFlowsOut.Add(variable);
				}
			}
			base.NoteWrite(variable, value);
		}

		private bool WasUsedBeforeAssignment(Symbol sym)
		{
			return _originalUnassigned.Contains(sym);
		}

		internal override void AssignLocalOnDeclaration(LocalSymbol local, BoundLocalDeclaration node)
		{
			if (local.IsStatic)
			{
				Assign(node, node.InitializerOpt);
			}
			else
			{
				base.AssignLocalOnDeclaration(local, node);
			}
		}
	}
}
