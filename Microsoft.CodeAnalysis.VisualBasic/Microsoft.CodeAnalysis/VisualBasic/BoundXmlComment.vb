Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlComment
		Inherits BoundExpression
		Private ReadOnly _Value As BoundExpression

		Private ReadOnly _ObjectCreation As BoundExpression

		Public ReadOnly Property ObjectCreation As BoundExpression
			Get
				Return Me._ObjectCreation
			End Get
		End Property

		Public ReadOnly Property Value As BoundExpression
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal value As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlComment, syntax, type, If(hasErrors OrElse value.NonNullAndHasErrors(), True, objectCreation.NonNullAndHasErrors()))
			Me._Value = value
			Me._ObjectCreation = objectCreation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlComment(Me)
		End Function

		Public Function Update(ByVal value As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlComment
			Dim boundXmlComment As Microsoft.CodeAnalysis.VisualBasic.BoundXmlComment
			If (value <> Me.Value OrElse objectCreation <> Me.ObjectCreation OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlComment1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlComment = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlComment(MyBase.Syntax, value, objectCreation, type, MyBase.HasErrors)
				boundXmlComment1.CopyAttributes(Me)
				boundXmlComment = boundXmlComment1
			Else
				boundXmlComment = Me
			End If
			Return boundXmlComment
		End Function
	End Class
End Namespace