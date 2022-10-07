Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ExitableStatementBinder
		Inherits BlockBaseBinder
		Private ReadOnly _continueLabel As LabelSymbol

		Private ReadOnly _continueKind As SyntaxKind

		Private ReadOnly _exitLabel As LabelSymbol

		Private ReadOnly _exitKind As SyntaxKind

		Friend Overrides ReadOnly Property Locals As ImmutableArray(Of LocalSymbol)
			Get
				Return ImmutableArray(Of LocalSymbol).Empty
			End Get
		End Property

		Public Sub New(ByVal enclosing As Binder, ByVal continueKind As SyntaxKind, ByVal exitKind As SyntaxKind)
			MyBase.New(enclosing)
			Me._continueKind = continueKind
			If (continueKind <> SyntaxKind.None) Then
				Me._continueLabel = New GeneratedLabelSymbol("continue")
			End If
			Me._exitKind = exitKind
			If (exitKind <> SyntaxKind.None) Then
				Me._exitLabel = New GeneratedLabelSymbol("exit")
			End If
		End Sub

		Public Overrides Function GetContinueLabel(ByVal continueSyntaxKind As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			labelSymbol = If(Me._continueKind <> continueSyntaxKind, MyBase.ContainingBinder.GetContinueLabel(continueSyntaxKind), Me._continueLabel)
			Return labelSymbol
		End Function

		Public Overrides Function GetExitLabel(ByVal exitSyntaxKind As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			labelSymbol = If(Me._exitKind <> exitSyntaxKind, MyBase.ContainingBinder.GetExitLabel(exitSyntaxKind), Me._exitLabel)
			Return labelSymbol
		End Function

		Public Overrides Function GetReturnLabel() As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._exitKind
			labelSymbol = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement, Me._exitLabel, MyBase.ContainingBinder.GetReturnLabel())
			Return labelSymbol
		End Function
	End Class
End Namespace