Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMeReference
		Inherits BoundExpression
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.MeReference, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.MeReference, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMeReference(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference
			Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundMeReference = Me
			Else
				Dim boundMeReference1 As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(MyBase.Syntax, type, MyBase.HasErrors)
				boundMeReference1.CopyAttributes(Me)
				boundMeReference = boundMeReference1
			End If
			Return boundMeReference
		End Function
	End Class
End Namespace