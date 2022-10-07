Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class Rewriter
		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Function LowerBodyOrInitializer(ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal body As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal previousSubmissionFields As SynthesizedSubmissionFields, ByVal compilationState As TypeCompilationState, ByVal instrumentForDynamicAnalysis As Boolean, <Out> ByRef dynamicAnalysisSpans As ImmutableArray(Of SourceSpan), ByVal debugDocumentProvider As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef lazyVariableSlotAllocator As VariableSlotAllocator, ByVal lambdaDebugInfoBuilder As ArrayBuilder(Of LambdaDebugInfo), ByVal closureDebugInfoBuilder As ArrayBuilder(Of ClosureDebugInfo), ByRef delegateRelaxationIdDispenser As Integer, <Out> ByRef stateMachineTypeOpt As StateMachineTypeSymbol, ByVal allowOmissionOfConditionalCalls As Boolean, ByVal isBodySynthesized As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim flag As Boolean
			Dim dynamicAnalysisInjector As Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector
			Dim symbols As ISet(Of Symbol) = Nothing
			Dim boundNodes As HashSet(Of BoundNode) = Nothing
			Dim rewritingFlag As LocalRewriter.RewritingFlags = If(allowOmissionOfConditionalCalls, LocalRewriter.RewritingFlags.AllowOmissionOfConditionalCalls, LocalRewriter.RewritingFlags.[Default])
			Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(diagnostics)
			dynamicAnalysisSpans = ImmutableArray(Of SourceSpan).Empty
			Try
				If (instrumentForDynamicAnalysis) Then
					dynamicAnalysisInjector = Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector.TryCreate(method, body, New SyntheticBoundNodeFactory(method, method, body.Syntax, compilationState, diagnostics), diagnostics, debugDocumentProvider, Instrumenter.NoOp)
				Else
					dynamicAnalysisInjector = Nothing
				End If
				Dim dynamicAnalysisInjector1 As Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector = dynamicAnalysisInjector
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = LocalRewriter.Rewrite(body, method, compilationState, previousSubmissionFields, instance, boundNodes, flag, symbols, rewritingFlag, If(dynamicAnalysisInjector1 IsNot Nothing, New DebugInfoInjector(dynamicAnalysisInjector1), DebugInfoInjector.Singleton), Nothing)
				If (dynamicAnalysisInjector1 IsNot Nothing) Then
					dynamicAnalysisSpans = dynamicAnalysisInjector1.DynamicAnalysisSpans
				End If
				If (boundBlock1.HasErrors OrElse instance.HasAnyErrors()) Then
					diagnostics.AddRangeAndFree(instance)
					boundBlock = boundBlock1
				Else
					If (lazyVariableSlotAllocator Is Nothing) Then
						lazyVariableSlotAllocator = compilationState.ModuleBuilderOpt.TryCreateVariableSlotAllocator(method, method, diagnostics.DiagnosticBag)
					End If
					Dim boundBlock2 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = boundBlock1
					If (flag) Then
						boundBlock2 = LambdaRewriter.Rewrite(boundBlock1, method, methodOrdinal, lambdaDebugInfoBuilder, closureDebugInfoBuilder, delegateRelaxationIdDispenser, lazyVariableSlotAllocator, compilationState, If(symbols, SpecializedCollections.EmptySet(Of Symbol)()), instance, boundNodes)
					End If
					If (boundBlock2.HasErrors OrElse instance.HasAnyErrors()) Then
						diagnostics.AddRangeAndFree(instance)
						boundBlock = boundBlock2
					Else
						Dim boundBlock3 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Rewriter.RewriteIteratorAndAsync(boundBlock2, method, methodOrdinal, compilationState, instance, lazyVariableSlotAllocator, stateMachineTypeOpt)
						diagnostics.AddRangeAndFree(instance)
						boundBlock = boundBlock3
					End If
				End If
			Catch cancelledByStackGuardException1 As BoundTreeVisitor.CancelledByStackGuardException
				ProjectData.SetProjectError(cancelledByStackGuardException1)
				Dim cancelledByStackGuardException As BoundTreeVisitor.CancelledByStackGuardException = cancelledByStackGuardException1
				diagnostics.AddRangeAndFree(instance)
				cancelledByStackGuardException.AddAnError(diagnostics)
				boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(body.Syntax, body.StatementListSyntax, body.Locals, body.Statements, True)
				ProjectData.ClearProjectError()
			End Try
			Return boundBlock
		End Function

		Friend Shared Function RewriteIteratorAndAsync(ByVal bodyWithoutLambdas As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal slotAllocatorOpt As VariableSlotAllocator, <Out> ByRef stateMachineTypeOpt As Microsoft.CodeAnalysis.VisualBasic.StateMachineTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim iteratorStateMachine As Microsoft.CodeAnalysis.VisualBasic.IteratorStateMachine = Nothing
			Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = IteratorRewriter.Rewrite(bodyWithoutLambdas, method, methodOrdinal, slotAllocatorOpt, compilationState, diagnostics, iteratorStateMachine)
			If (Not boundBlock1.HasErrors) Then
				Dim asyncStateMachine As Microsoft.CodeAnalysis.VisualBasic.AsyncStateMachine = Nothing
				Dim boundBlock2 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = AsyncRewriter.Rewrite(boundBlock1, method, methodOrdinal, slotAllocatorOpt, compilationState, diagnostics, asyncStateMachine)
				Dim stateMachineTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineTypeSymbol = iteratorStateMachine
				If (stateMachineTypeSymbol Is Nothing) Then
					stateMachineTypeSymbol = asyncStateMachine
				End If
				stateMachineTypeOpt = stateMachineTypeSymbol
				boundBlock = boundBlock2
			Else
				boundBlock = boundBlock1
			End If
			Return boundBlock
		End Function
	End Class
End Namespace