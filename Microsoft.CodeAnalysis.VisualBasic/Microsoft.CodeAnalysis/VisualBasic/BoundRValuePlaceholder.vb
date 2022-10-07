Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRValuePlaceholder
		Inherits BoundRValuePlaceholderBase
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.RValuePlaceholder, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.RValuePlaceholder, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRValuePlaceholder(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundRValuePlaceholder = Me
			Else
				Dim boundRValuePlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder(MyBase.Syntax, type, MyBase.HasErrors)
				boundRValuePlaceholder1.CopyAttributes(Me)
				boundRValuePlaceholder = boundRValuePlaceholder1
			End If
			Return boundRValuePlaceholder
		End Function
	End Class
End Namespace