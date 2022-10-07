Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlMemberAccess
		Inherits BoundExpression
		Private ReadOnly _MemberAccess As BoundExpression

		Public ReadOnly Property MemberAccess As BoundExpression
			Get
				Return Me._MemberAccess
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal memberAccess As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlMemberAccess, syntax, type, If(hasErrors, True, memberAccess.NonNullAndHasErrors()))
			Me._MemberAccess = memberAccess
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlMemberAccess(Me)
		End Function

		Public Function Update(ByVal memberAccess As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlMemberAccess
			Dim boundXmlMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundXmlMemberAccess
			If (memberAccess <> Me.MemberAccess OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlMemberAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlMemberAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlMemberAccess(MyBase.Syntax, memberAccess, type, MyBase.HasErrors)
				boundXmlMemberAccess1.CopyAttributes(Me)
				boundXmlMemberAccess = boundXmlMemberAccess1
			Else
				boundXmlMemberAccess = Me
			End If
			Return boundXmlMemberAccess
		End Function
	End Class
End Namespace