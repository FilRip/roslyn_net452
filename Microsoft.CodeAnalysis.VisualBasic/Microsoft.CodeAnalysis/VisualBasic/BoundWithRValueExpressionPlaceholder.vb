Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundWithRValueExpressionPlaceholder
		Inherits BoundRValuePlaceholderBase
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.WithRValueExpressionPlaceholder, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.WithRValueExpressionPlaceholder, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitWithRValueExpressionPlaceholder(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundWithRValueExpressionPlaceholder
			Dim boundWithRValueExpressionPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundWithRValueExpressionPlaceholder
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundWithRValueExpressionPlaceholder = Me
			Else
				Dim boundWithRValueExpressionPlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundWithRValueExpressionPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundWithRValueExpressionPlaceholder(MyBase.Syntax, type, MyBase.HasErrors)
				boundWithRValueExpressionPlaceholder1.CopyAttributes(Me)
				boundWithRValueExpressionPlaceholder = boundWithRValueExpressionPlaceholder1
			End If
			Return boundWithRValueExpressionPlaceholder
		End Function
	End Class
End Namespace