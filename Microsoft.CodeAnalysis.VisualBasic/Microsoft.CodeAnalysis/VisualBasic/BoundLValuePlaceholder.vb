Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLValuePlaceholder
		Inherits BoundLValuePlaceholderBase
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.LValuePlaceholder, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.LValuePlaceholder, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLValuePlaceholder(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLValuePlaceholder
			Dim boundLValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundLValuePlaceholder
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundLValuePlaceholder = Me
			Else
				Dim boundLValuePlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundLValuePlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundLValuePlaceholder(MyBase.Syntax, type, MyBase.HasErrors)
				boundLValuePlaceholder1.CopyAttributes(Me)
				boundLValuePlaceholder = boundLValuePlaceholder1
			End If
			Return boundLValuePlaceholder
		End Function
	End Class
End Namespace