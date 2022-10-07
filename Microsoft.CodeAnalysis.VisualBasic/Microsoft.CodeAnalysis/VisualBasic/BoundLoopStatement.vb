Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundLoopStatement
		Inherits BoundStatement
		Private ReadOnly _ContinueLabel As LabelSymbol

		Private ReadOnly _ExitLabel As LabelSymbol

		Public ReadOnly Property ContinueLabel As LabelSymbol
			Get
				Return Me._ContinueLabel
			End Get
		End Property

		Public ReadOnly Property ExitLabel As LabelSymbol
			Get
				Return Me._ExitLabel
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, hasErrors)
			Me._ContinueLabel = continueLabel
			Me._ExitLabel = exitLabel
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol)
			MyBase.New(kind, syntax)
			Me._ContinueLabel = continueLabel
			Me._ExitLabel = exitLabel
		End Sub
	End Class
End Namespace