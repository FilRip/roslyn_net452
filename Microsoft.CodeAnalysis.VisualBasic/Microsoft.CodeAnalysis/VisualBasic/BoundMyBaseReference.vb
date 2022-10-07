Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMyBaseReference
		Inherits BoundExpression
		Public NotOverridable Overrides ReadOnly Property SuppressVirtualCalls As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.MyBaseReference, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.MyBaseReference, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMyBaseReference(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference
			Dim boundMyBaseReference As Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundMyBaseReference = Me
			Else
				Dim boundMyBaseReference1 As Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference(MyBase.Syntax, type, MyBase.HasErrors)
				boundMyBaseReference1.CopyAttributes(Me)
				boundMyBaseReference = boundMyBaseReference1
			End If
			Return boundMyBaseReference
		End Function
	End Class
End Namespace