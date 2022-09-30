using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class Rewriter
	{
		public static BoundBlock LowerBodyOrInitializer(MethodSymbol method, int methodOrdinal, BoundBlock body, SynthesizedSubmissionFields previousSubmissionFields, TypeCompilationState compilationState, bool instrumentForDynamicAnalysis, out ImmutableArray<SourceSpan> dynamicAnalysisSpans, DebugDocumentProvider debugDocumentProvider, BindingDiagnosticBag diagnostics, ref VariableSlotAllocator lazyVariableSlotAllocator, ArrayBuilder<LambdaDebugInfo> lambdaDebugInfoBuilder, ArrayBuilder<ClosureDebugInfo> closureDebugInfoBuilder, ref int delegateRelaxationIdDispenser, out StateMachineTypeSymbol stateMachineTypeOpt, bool allowOmissionOfConditionalCalls, bool isBodySynthesized)
		{
			ISet<Symbol> symbolsCapturedWithoutCopyCtor = null;
			HashSet<BoundNode> rewrittenNodes = null;
			LocalRewriter.RewritingFlags flags = (allowOmissionOfConditionalCalls ? LocalRewriter.RewritingFlags.AllowOmissionOfConditionalCalls : LocalRewriter.RewritingFlags.Default);
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(diagnostics);
			dynamicAnalysisSpans = ImmutableArray<SourceSpan>.Empty;
			try
			{
				DynamicAnalysisInjector dynamicAnalysisInjector = (instrumentForDynamicAnalysis ? DynamicAnalysisInjector.TryCreate(method, body, new SyntheticBoundNodeFactory(method, method, body.Syntax, compilationState, diagnostics), diagnostics, debugDocumentProvider, Instrumenter.NoOp) : null);
				bool hasLambdas;
				BoundBlock boundBlock = LocalRewriter.Rewrite(body, method, compilationState, previousSubmissionFields, instance, out rewrittenNodes, out hasLambdas, out symbolsCapturedWithoutCopyCtor, flags, (dynamicAnalysisInjector != null) ? new DebugInfoInjector(dynamicAnalysisInjector) : DebugInfoInjector.Singleton, null);
				if (dynamicAnalysisInjector != null)
				{
					dynamicAnalysisSpans = dynamicAnalysisInjector.DynamicAnalysisSpans;
				}
				if (boundBlock.HasErrors || instance.HasAnyErrors())
				{
					diagnostics.AddRangeAndFree(instance);
					return boundBlock;
				}
				if (lazyVariableSlotAllocator == null)
				{
					lazyVariableSlotAllocator = compilationState.ModuleBuilderOpt.TryCreateVariableSlotAllocator(method, method, diagnostics.DiagnosticBag);
				}
				BoundBlock boundBlock2 = boundBlock;
				if (hasLambdas)
				{
					boundBlock2 = LambdaRewriter.Rewrite(boundBlock, method, methodOrdinal, lambdaDebugInfoBuilder, closureDebugInfoBuilder, ref delegateRelaxationIdDispenser, lazyVariableSlotAllocator, compilationState, symbolsCapturedWithoutCopyCtor ?? SpecializedCollections.EmptySet<Symbol>(), instance, rewrittenNodes);
				}
				if (boundBlock2.HasErrors || instance.HasAnyErrors())
				{
					diagnostics.AddRangeAndFree(instance);
					return boundBlock2;
				}
				BoundBlock result = RewriteIteratorAndAsync(boundBlock2, method, methodOrdinal, compilationState, instance, lazyVariableSlotAllocator, out stateMachineTypeOpt);
				diagnostics.AddRangeAndFree(instance);
				return result;
			}
			catch (BoundTreeVisitor.CancelledByStackGuardException ex)
			{
				ProjectData.SetProjectError(ex);
				BoundTreeVisitor.CancelledByStackGuardException ex2 = ex;
				diagnostics.AddRangeAndFree(instance);
				ex2.AddAnError(diagnostics);
				BoundBlock result2 = new BoundBlock(body.Syntax, body.StatementListSyntax, body.Locals, body.Statements, hasErrors: true);
				ProjectData.ClearProjectError();
				return result2;
			}
		}

		internal static BoundBlock RewriteIteratorAndAsync(BoundBlock bodyWithoutLambdas, MethodSymbol method, int methodOrdinal, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, VariableSlotAllocator slotAllocatorOpt, out StateMachineTypeSymbol stateMachineTypeOpt)
		{
			IteratorStateMachine stateMachineType = null;
			BoundBlock boundBlock = IteratorRewriter.Rewrite(bodyWithoutLambdas, method, methodOrdinal, slotAllocatorOpt, compilationState, diagnostics, out stateMachineType);
			if (boundBlock.HasErrors)
			{
				return boundBlock;
			}
			AsyncStateMachine stateMachineType2 = null;
			BoundBlock result = AsyncRewriter.Rewrite(boundBlock, method, methodOrdinal, slotAllocatorOpt, compilationState, diagnostics, out stateMachineType2);
			stateMachineTypeOpt = (StateMachineTypeSymbol)(((object)stateMachineType) ?? ((object)stateMachineType2));
			return result;
		}
	}
}
