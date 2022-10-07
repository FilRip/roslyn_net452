Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundConversionOrCast
		Inherits BoundExpression
		Public MustOverride ReadOnly Property ConversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind

		Public MustOverride ReadOnly Property ExplicitCastInCode As Boolean

		Public MustOverride ReadOnly Property Operand As BoundExpression

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, type, hasErrors)
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(kind, syntax, type)
		End Sub
	End Class
End Namespace