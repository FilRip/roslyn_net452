Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class FinallyBlockBinder
		Inherits ExitableStatementBinder
		Public Sub New(ByVal enclosing As Binder)
			MyBase.New(enclosing, SyntaxKind.None, SyntaxKind.None)
		End Sub

		Public Overrides Function GetExitLabel(ByVal exitSyntaxKind As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			labelSymbol = If(exitSyntaxKind <> SyntaxKind.ExitTryStatement, MyBase.GetExitLabel(exitSyntaxKind), MyBase.ContainingBinder.ContainingBinder.GetExitLabel(exitSyntaxKind))
			Return labelSymbol
		End Function
	End Class
End Namespace