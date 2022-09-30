using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ExitPointsWalker : AbstractRegionControlFlowPass
	{
		private ArrayBuilder<StatementSyntax> _branchesOutOf;

		private ArrayBuilder<LabelSymbol> _labelsInside;

		internal static IEnumerable<StatementSyntax> Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
		{
			ExitPointsWalker exitPointsWalker = new ExitPointsWalker(info, region);
			try
			{
				return exitPointsWalker.Analyze() ? ((IEnumerable<StatementSyntax>)exitPointsWalker._branchesOutOf.ToImmutable()) : SpecializedCollections.EmptyEnumerable<StatementSyntax>();
			}
			finally
			{
				exitPointsWalker.Free();
			}
		}

		private new bool Analyze()
		{
			return Scan();
		}

		private ExitPointsWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region)
		{
			_branchesOutOf = ArrayBuilder<StatementSyntax>.GetInstance();
			_labelsInside = ArrayBuilder<LabelSymbol>.GetInstance();
		}

		protected override void Free()
		{
			_branchesOutOf.Free();
			_branchesOutOf = null;
			_labelsInside.Free();
			_labelsInside = null;
			base.Free();
		}

		public override BoundNode VisitLabelStatement(BoundLabelStatement node)
		{
			_ = node.Syntax;
			if (base.IsInside)
			{
				_labelsInside.Add(node.Label);
			}
			return base.VisitLabelStatement(node);
		}

		public override BoundNode VisitDoLoopStatement(BoundDoLoopStatement node)
		{
			if (base.IsInside)
			{
				_labelsInside.Add(node.ExitLabel);
				_labelsInside.Add(node.ContinueLabel);
			}
			return base.VisitDoLoopStatement(node);
		}

		public override BoundNode VisitForToStatement(BoundForToStatement node)
		{
			if (base.IsInside)
			{
				_labelsInside.Add(node.ExitLabel);
				_labelsInside.Add(node.ContinueLabel);
			}
			return base.VisitForToStatement(node);
		}

		public override BoundNode VisitForEachStatement(BoundForEachStatement node)
		{
			if (base.IsInside)
			{
				_labelsInside.Add(node.ExitLabel);
				_labelsInside.Add(node.ContinueLabel);
			}
			return base.VisitForEachStatement(node);
		}

		public override BoundNode VisitWhileStatement(BoundWhileStatement node)
		{
			if (base.IsInside)
			{
				_labelsInside.Add(node.ExitLabel);
				_labelsInside.Add(node.ContinueLabel);
			}
			return base.VisitWhileStatement(node);
		}

		public override BoundNode VisitSelectStatement(BoundSelectStatement node)
		{
			if (base.IsInside)
			{
				_labelsInside.Add(node.ExitLabel);
			}
			return base.VisitSelectStatement(node);
		}

		protected override void LeaveRegion()
		{
			ImmutableArray<PendingBranch>.Enumerator enumerator = base.PendingBranches.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PendingBranch current = enumerator.Current;
				if (!IsInsideRegion(current.Branch.Syntax.Span))
				{
					continue;
				}
				switch (current.Branch.Kind)
				{
				case BoundKind.GotoStatement:
					if (_labelsInside.Contains((current.Branch as BoundGotoStatement).Label))
					{
						continue;
					}
					break;
				case BoundKind.ExitStatement:
					if (_labelsInside.Contains((current.Branch as BoundExitStatement).Label))
					{
						continue;
					}
					break;
				case BoundKind.ContinueStatement:
					if (_labelsInside.Contains((current.Branch as BoundContinueStatement).Label))
					{
						continue;
					}
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(current.Branch.Kind);
				case BoundKind.ReturnStatement:
				case BoundKind.YieldStatement:
					break;
				}
				_branchesOutOf.Add((StatementSyntax)current.Branch.Syntax);
			}
			base.LeaveRegion();
		}
	}
}
