Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundEraseStatement
		Inherits BoundStatement
		Private ReadOnly _Clauses As ImmutableArray(Of BoundAssignmentOperator)

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return StaticCast(Of BoundNode).From(Of BoundAssignmentOperator)(Me.Clauses)
			End Get
		End Property

		Public ReadOnly Property Clauses As ImmutableArray(Of BoundAssignmentOperator)
			Get
				Return Me._Clauses
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal clauses As ImmutableArray(Of BoundAssignmentOperator), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.EraseStatement, syntax, If(hasErrors, True, clauses.NonNullAndHasErrors()))
			Me._Clauses = clauses
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitEraseStatement(Me)
		End Function

		Public Function Update(ByVal clauses As ImmutableArray(Of BoundAssignmentOperator)) As Microsoft.CodeAnalysis.VisualBasic.BoundEraseStatement
			Dim boundEraseStatement As Microsoft.CodeAnalysis.VisualBasic.BoundEraseStatement
			If (clauses = Me.Clauses) Then
				boundEraseStatement = Me
			Else
				Dim boundEraseStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundEraseStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundEraseStatement(MyBase.Syntax, clauses, MyBase.HasErrors)
				boundEraseStatement1.CopyAttributes(Me)
				boundEraseStatement = boundEraseStatement1
			End If
			Return boundEraseStatement
		End Function
	End Class
End Namespace