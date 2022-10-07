Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundHostObjectMemberReference
		Inherits BoundExpression
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.HostObjectMemberReference, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.HostObjectMemberReference, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitHostObjectMemberReference(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundHostObjectMemberReference
			Dim boundHostObjectMemberReference As Microsoft.CodeAnalysis.VisualBasic.BoundHostObjectMemberReference
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundHostObjectMemberReference = Me
			Else
				Dim boundHostObjectMemberReference1 As Microsoft.CodeAnalysis.VisualBasic.BoundHostObjectMemberReference = New Microsoft.CodeAnalysis.VisualBasic.BoundHostObjectMemberReference(MyBase.Syntax, type, MyBase.HasErrors)
				boundHostObjectMemberReference1.CopyAttributes(Me)
				boundHostObjectMemberReference = boundHostObjectMemberReference1
			End If
			Return boundHostObjectMemberReference
		End Function
	End Class
End Namespace