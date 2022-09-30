using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ControlFlowPass : AbstractFlowPass<ControlFlowPass.LocalState>
	{
		internal struct LocalState : AbstractLocalState
		{
			internal bool Alive;

			internal bool Reported;

			public LocalState(bool live, bool reported)
			{
				this = default(LocalState);
				Alive = live;
				Reported = reported;
			}

			public LocalState Clone()
			{
				return this;
			}

			LocalState AbstractLocalState.Clone()
			{
				//ILSpy generated this explicit interface implementation from .override directive in Clone
				return this.Clone();
			}
		}

		protected bool _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;

		protected override bool IntersectWith(ref LocalState self, ref LocalState other)
		{
			LocalState localState = self;
			self.Alive |= other.Alive;
			self.Reported &= other.Reported;
			return !self.Equals(localState);
		}

		protected override void UnionWith(ref LocalState self, ref LocalState other)
		{
			self.Alive &= other.Alive;
			self.Reported &= other.Reported;
		}

		protected override string Dump(LocalState state)
		{
			return "[alive: " + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(state.Alive) + "; reported: " + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(state.Reported) + "]";
		}

		internal ControlFlowPass(FlowAnalysisInfo info, bool suppressConstExpressionsSupport)
			: base(info, suppressConstExpressionsSupport)
		{
			_convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = false;
		}

		internal ControlFlowPass(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, bool suppressConstantExpressionsSupport)
			: base(info, region, suppressConstantExpressionsSupport, trackUnassignments: false)
		{
			_convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = false;
		}

		protected override LocalState ReachableState()
		{
			return new LocalState(live: true, reported: false);
		}

		protected override LocalState UnreachableState()
		{
			return new LocalState(live: false, State.Reported);
		}

		protected override void Visit(BoundNode node, bool dontLeaveRegion)
		{
			if (!(node is BoundExpression))
			{
				base.Visit(node, dontLeaveRegion);
			}
		}

		public static bool Analyze(FlowAnalysisInfo info, DiagnosticBag diagnostics, bool suppressConstantExpressionsSupport)
		{
			ControlFlowPass controlFlowPass = new ControlFlowPass(info, suppressConstantExpressionsSupport);
			if (diagnostics != null)
			{
				controlFlowPass._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = true;
			}
			try
			{
				controlFlowPass.Analyze();
				diagnostics?.AddRange(controlFlowPass.diagnostics);
				return controlFlowPass.State.Alive;
			}
			catch (CancelledByStackGuardException ex) when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				ProjectData.SetProjectError(ex);
				return diagnostics != null;
			}).Invoke())
			{
				ex.AddAnError(diagnostics);
				bool result = true;
				ProjectData.ClearProjectError();
				return result;
			}
			finally
			{
				controlFlowPass.Free();
			}
		}

		protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
		{
			return _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;
		}

		protected override void VisitStatement(BoundStatement statement)
		{
			BoundKind kind = statement.Kind;
			if (kind != BoundKind.NoOpStatement && kind != BoundKind.Block && kind != BoundKind.LabelStatement && !State.Alive && !State.Reported)
			{
				switch (statement.Kind)
				{
				case BoundKind.LocalDeclaration:
					if ((statement as BoundLocalDeclaration).InitializerOpt != null)
					{
						State.Reported = true;
					}
					break;
				case BoundKind.ReturnStatement:
					if (!(statement as BoundReturnStatement).IsEndOfMethodReturn())
					{
						State.Reported = true;
					}
					break;
				default:
					State.Reported = true;
					break;
				case BoundKind.DimStatement:
					break;
				}
			}
			base.VisitStatement(statement);
		}

		protected override void VisitTryBlock(BoundStatement tryBlock, BoundTryStatement node, ref LocalState tryState)
		{
			if (node.CatchBlocks.IsEmpty)
			{
				base.VisitTryBlock(tryBlock, node, ref tryState);
				return;
			}
			SavedPending oldPending = SavePending();
			base.VisitTryBlock(tryBlock, node, ref tryState);
			RestorePending(oldPending, mergeLabelsSeen: true);
		}

		protected override void VisitCatchBlock(BoundCatchBlock node, ref LocalState finallyState)
		{
			SavedPending oldPending = SavePending();
			base.VisitCatchBlock(node, ref finallyState);
			ImmutableArray<PendingBranch>.Enumerator enumerator = base.PendingBranches.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PendingBranch current = enumerator.Current;
				if (current.Branch.Kind == BoundKind.YieldStatement)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_BadYieldInTryHandler, current.Branch.Syntax.GetLocation());
				}
			}
			RestorePending(oldPending);
		}

		protected override void VisitFinallyBlock(BoundStatement finallyBlock, ref LocalState endState)
		{
			SavedPending oldPending = SavePending();
			SavedPending oldPending2 = SavePending();
			base.VisitFinallyBlock(finallyBlock, ref endState);
			RestorePending(oldPending2);
			ImmutableArray<PendingBranch>.Enumerator enumerator = base.PendingBranches.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PendingBranch current = enumerator.Current;
				SyntaxNode syntax = current.Branch.Syntax;
				ERRID code;
				SyntaxNodeOrToken syntaxNodeOrToken;
				if (current.Branch.Kind == BoundKind.YieldStatement)
				{
					code = ERRID.ERR_BadYieldInTryHandler;
					syntaxNodeOrToken = syntax;
				}
				else
				{
					code = ERRID.ERR_BranchOutOfFinally;
					syntaxNodeOrToken = ((VisualBasicExtensions.Kind(syntax) != SyntaxKind.GoToStatement) ? ((SyntaxNodeOrToken)syntax) : ((SyntaxNodeOrToken)((GoToStatementSyntax)syntax).Label));
				}
				DiagnosticBagExtensions.Add(diagnostics, code, syntaxNodeOrToken.GetLocation());
			}
			RestorePending(oldPending);
		}
	}
}
