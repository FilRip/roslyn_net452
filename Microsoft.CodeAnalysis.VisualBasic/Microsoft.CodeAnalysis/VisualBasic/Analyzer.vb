Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class Analyzer
		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Sub AnalyzeMethodBody(ByVal method As MethodSymbol, ByVal body As BoundBlock, ByVal diagnostics As DiagnosticBag)
			Dim instance As DiagnosticBag = diagnostics
			If (method.IsImplicitlyDeclared AndAlso method.AssociatedSymbol IsNot Nothing AndAlso method.AssociatedSymbol.IsMyGroupCollectionProperty) Then
				instance = DiagnosticBag.GetInstance()
			End If
			FlowAnalysisPass.Analyze(method, body, instance)
			ForLoopVerification.VerifyForLoops(body, instance)
			If (instance <> diagnostics) Then
				DirectCast(method.AssociatedSymbol, SynthesizedMyGroupCollectionPropertySymbol).RelocateDiagnostics(instance, diagnostics)
				instance.Free()
			End If
		End Sub
	End Class
End Namespace