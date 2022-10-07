Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundCompoundAssignmentTargetPlaceholder
		Inherits BoundValuePlaceholderBase
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.CompoundAssignmentTargetPlaceholder, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.CompoundAssignmentTargetPlaceholder, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitCompoundAssignmentTargetPlaceholder(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder
			Dim boundCompoundAssignmentTargetPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundCompoundAssignmentTargetPlaceholder = Me
			Else
				Dim boundCompoundAssignmentTargetPlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder(MyBase.Syntax, type, MyBase.HasErrors)
				boundCompoundAssignmentTargetPlaceholder1.CopyAttributes(Me)
				boundCompoundAssignmentTargetPlaceholder = boundCompoundAssignmentTargetPlaceholder1
			End If
			Return boundCompoundAssignmentTargetPlaceholder
		End Function
	End Class
End Namespace