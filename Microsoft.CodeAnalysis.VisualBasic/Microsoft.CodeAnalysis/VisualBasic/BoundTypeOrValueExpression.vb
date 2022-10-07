Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTypeOrValueExpression
		Inherits BoundExpression
		Private ReadOnly _Data As BoundTypeOrValueData

		Public ReadOnly Property Data As BoundTypeOrValueData
			Get
				Return Me._Data
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal data As BoundTypeOrValueData, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.TypeOrValueExpression, syntax, type, hasErrors)
			Me._Data = data
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal data As BoundTypeOrValueData, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.TypeOrValueExpression, syntax, type)
			Me._Data = data
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTypeOrValueExpression(Me)
		End Function

		Public Function Update(ByVal data As BoundTypeOrValueData, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOrValueExpression
			Dim boundTypeOrValueExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOrValueExpression
			If (data <> Me.Data OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundTypeOrValueExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOrValueExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundTypeOrValueExpression(MyBase.Syntax, data, type, MyBase.HasErrors)
				boundTypeOrValueExpression1.CopyAttributes(Me)
				boundTypeOrValueExpression = boundTypeOrValueExpression1
			Else
				boundTypeOrValueExpression = Me
			End If
			Return boundTypeOrValueExpression
		End Function
	End Class
End Namespace