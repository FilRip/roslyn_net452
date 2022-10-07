Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class FlowAnalysisPass
		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Sub Analyze(ByVal method As MethodSymbol, ByVal block As BoundBlock, ByVal diagnostics As DiagnosticBag)
			FlowAnalysisPass.Analyze(method.DeclaringCompilation, method, block, diagnostics)
		End Sub

		Private Shared Sub Analyze(ByVal compilation As VisualBasicCompilation, ByVal method As MethodSymbol, ByVal block As BoundBlock, ByVal diagnostics As DiagnosticBag)
			ControlFlowPass.Analyze(New FlowAnalysisInfo(compilation, method, block), diagnostics, True)
			DataFlowPass.Analyze(New FlowAnalysisInfo(compilation, method, block), diagnostics, True)
		End Sub
	End Class
End Namespace