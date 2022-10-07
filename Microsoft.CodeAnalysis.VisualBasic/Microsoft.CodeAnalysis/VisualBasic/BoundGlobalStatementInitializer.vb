Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundGlobalStatementInitializer
		Inherits BoundInitializer
		Private ReadOnly _Statement As BoundStatement

		Public ReadOnly Property Statement As BoundStatement
			Get
				Return Me._Statement
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal statement As BoundStatement, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.GlobalStatementInitializer, syntax, If(hasErrors, True, statement.NonNullAndHasErrors()))
			Me._Statement = statement
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitGlobalStatementInitializer(Me)
		End Function

		Public Function Update(ByVal statement As BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundGlobalStatementInitializer
			Dim boundGlobalStatementInitializer As Microsoft.CodeAnalysis.VisualBasic.BoundGlobalStatementInitializer
			If (statement = Me.Statement) Then
				boundGlobalStatementInitializer = Me
			Else
				Dim boundGlobalStatementInitializer1 As Microsoft.CodeAnalysis.VisualBasic.BoundGlobalStatementInitializer = New Microsoft.CodeAnalysis.VisualBasic.BoundGlobalStatementInitializer(MyBase.Syntax, statement, MyBase.HasErrors)
				boundGlobalStatementInitializer1.CopyAttributes(Me)
				boundGlobalStatementInitializer = boundGlobalStatementInitializer1
			End If
			Return boundGlobalStatementInitializer
		End Function
	End Class
End Namespace