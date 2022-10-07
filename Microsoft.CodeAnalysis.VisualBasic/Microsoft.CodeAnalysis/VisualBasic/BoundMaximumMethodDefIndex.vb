Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMaximumMethodDefIndex
		Inherits BoundExpression
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.MaximumMethodDefIndex, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.MaximumMethodDefIndex, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMaximumMethodDefIndex(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundMaximumMethodDefIndex
			Dim boundMaximumMethodDefIndex As Microsoft.CodeAnalysis.VisualBasic.BoundMaximumMethodDefIndex
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundMaximumMethodDefIndex = Me
			Else
				Dim boundMaximumMethodDefIndex1 As Microsoft.CodeAnalysis.VisualBasic.BoundMaximumMethodDefIndex = New Microsoft.CodeAnalysis.VisualBasic.BoundMaximumMethodDefIndex(MyBase.Syntax, type, MyBase.HasErrors)
				boundMaximumMethodDefIndex1.CopyAttributes(Me)
				boundMaximumMethodDefIndex = boundMaximumMethodDefIndex1
			End If
			Return boundMaximumMethodDefIndex
		End Function
	End Class
End Namespace