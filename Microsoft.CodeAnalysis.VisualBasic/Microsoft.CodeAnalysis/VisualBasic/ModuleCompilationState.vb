Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ModuleCompilationState
		Inherits ModuleCompilationState(Of NamedTypeSymbol, MethodSymbol)
		Public Sub New()
			MyBase.New()
		End Sub
	End Class
End Namespace