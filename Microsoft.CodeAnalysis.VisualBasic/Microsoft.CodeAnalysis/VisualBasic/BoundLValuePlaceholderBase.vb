Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundLValuePlaceholderBase
		Inherits BoundValuePlaceholderBase
		Public NotOverridable Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, type, hasErrors)
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(kind, syntax, type)
		End Sub

		Protected NotOverridable Overrides Function MakeRValueImpl() As BoundExpression
			Return (New BoundLValueToRValueWrapper(MyBase.Syntax, Me, MyBase.Type, False)).MakeCompilerGenerated()
		End Function
	End Class
End Namespace