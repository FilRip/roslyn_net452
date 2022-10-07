Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMyClassReference
		Inherits BoundExpression
		Public NotOverridable Overrides ReadOnly Property SuppressVirtualCalls As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.MyClassReference, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.MyClassReference, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMyClassReference(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundMyClassReference
			Dim boundMyClassReference As Microsoft.CodeAnalysis.VisualBasic.BoundMyClassReference
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundMyClassReference = Me
			Else
				Dim boundMyClassReference1 As Microsoft.CodeAnalysis.VisualBasic.BoundMyClassReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMyClassReference(MyBase.Syntax, type, MyBase.HasErrors)
				boundMyClassReference1.CopyAttributes(Me)
				boundMyClassReference = boundMyClassReference1
			End If
			Return boundMyClassReference
		End Function
	End Class
End Namespace