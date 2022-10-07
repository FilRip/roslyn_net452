Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundWithLValueExpressionPlaceholder
		Inherits BoundLValuePlaceholderBase
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.WithLValueExpressionPlaceholder, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.WithLValueExpressionPlaceholder, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitWithLValueExpressionPlaceholder(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder
			Dim boundWithLValueExpressionPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundWithLValueExpressionPlaceholder = Me
			Else
				Dim boundWithLValueExpressionPlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder(MyBase.Syntax, type, MyBase.HasErrors)
				boundWithLValueExpressionPlaceholder1.CopyAttributes(Me)
				boundWithLValueExpressionPlaceholder = boundWithLValueExpressionPlaceholder1
			End If
			Return boundWithLValueExpressionPlaceholder
		End Function
	End Class
End Namespace