Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSequencePoint
		Inherits BoundStatement
		Private ReadOnly _StatementOpt As BoundStatement

		Public ReadOnly Property StatementOpt As BoundStatement
			Get
				Return Me._StatementOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal statementOpt As BoundStatement, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.SequencePoint, syntax, If(hasErrors, True, statementOpt.NonNullAndHasErrors()))
			Me._StatementOpt = statementOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSequencePoint(Me)
		End Function

		Public Function Update(ByVal statementOpt As BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint
			Dim boundSequencePoint As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint
			If (statementOpt = Me.StatementOpt) Then
				boundSequencePoint = Me
			Else
				Dim boundSequencePoint1 As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint(MyBase.Syntax, statementOpt, MyBase.HasErrors)
				boundSequencePoint1.CopyAttributes(Me)
				boundSequencePoint = boundSequencePoint1
			End If
			Return boundSequencePoint
		End Function
	End Class
End Namespace