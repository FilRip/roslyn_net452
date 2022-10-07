Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundStatementList
		Inherits BoundStatement
		Private ReadOnly _Statements As ImmutableArray(Of BoundStatement)

		Public ReadOnly Property Statements As ImmutableArray(Of BoundStatement)
			Get
				Return Me._Statements
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal statements As ImmutableArray(Of BoundStatement), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.StatementList, syntax, If(hasErrors, True, statements.NonNullAndHasErrors()))
			Me._Statements = statements
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitStatementList(Me)
		End Function

		Public Function Update(ByVal statements As ImmutableArray(Of BoundStatement)) As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList
			If (statements = Me.Statements) Then
				boundStatementList = Me
			Else
				Dim boundStatementList1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(MyBase.Syntax, statements, MyBase.HasErrors)
				boundStatementList1.CopyAttributes(Me)
				boundStatementList = boundStatementList1
			End If
			Return boundStatementList
		End Function
	End Class
End Namespace