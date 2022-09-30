using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class EntryPointsWalker : AbstractRegionControlFlowPass
	{
		private readonly HashSet<LabelStatementSyntax> _entryPoints;

		internal static IEnumerable<LabelStatementSyntax> Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, ref bool? succeeded)
		{
			EntryPointsWalker entryPointsWalker = new EntryPointsWalker(info, region);
			try
			{
				succeeded = entryPointsWalker.Analyze();
				return succeeded.GetValueOrDefault() ? entryPointsWalker._entryPoints : SpecializedCollections.EmptyEnumerable<LabelStatementSyntax>();
			}
			finally
			{
				entryPointsWalker.Free();
			}
		}

		private new bool Analyze()
		{
			return Scan();
		}

		private EntryPointsWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region)
		{
			_entryPoints = new HashSet<LabelStatementSyntax>();
		}

		protected override void Free()
		{
			base.Free();
		}

		protected override void NoteBranch(PendingBranch pending, BoundStatement stmt, BoundLabelStatement labelStmt)
		{
			if (stmt.Syntax != null && labelStmt.Syntax != null && IsInsideRegion(labelStmt.Syntax.Span) && !IsInsideRegion(stmt.Syntax.Span))
			{
				switch (stmt.Kind)
				{
				case BoundKind.GotoStatement:
					_entryPoints.Add((LabelStatementSyntax)labelStmt.Syntax);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(stmt.Kind);
				case BoundKind.ReturnStatement:
					break;
				}
			}
		}
	}
}
