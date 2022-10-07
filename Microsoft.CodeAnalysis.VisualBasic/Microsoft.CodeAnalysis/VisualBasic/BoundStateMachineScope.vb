Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundStateMachineScope
		Inherits BoundStatement
		Private ReadOnly _Fields As ImmutableArray(Of FieldSymbol)

		Private ReadOnly _Statement As BoundStatement

		Public ReadOnly Property Fields As ImmutableArray(Of FieldSymbol)
			Get
				Return Me._Fields
			End Get
		End Property

		Public ReadOnly Property Statement As BoundStatement
			Get
				Return Me._Statement
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal fields As ImmutableArray(Of FieldSymbol), ByVal statement As BoundStatement, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.StateMachineScope, syntax, If(hasErrors, True, statement.NonNullAndHasErrors()))
			Me._Fields = fields
			Me._Statement = statement
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitStateMachineScope(Me)
		End Function

		Public Function Update(ByVal fields As ImmutableArray(Of FieldSymbol), ByVal statement As BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStateMachineScope
			Dim boundStateMachineScope As Microsoft.CodeAnalysis.VisualBasic.BoundStateMachineScope
			If (fields <> Me.Fields OrElse statement <> Me.Statement) Then
				Dim boundStateMachineScope1 As Microsoft.CodeAnalysis.VisualBasic.BoundStateMachineScope = New Microsoft.CodeAnalysis.VisualBasic.BoundStateMachineScope(MyBase.Syntax, fields, statement, MyBase.HasErrors)
				boundStateMachineScope1.CopyAttributes(Me)
				boundStateMachineScope = boundStateMachineScope1
			Else
				boundStateMachineScope = Me
			End If
			Return boundStateMachineScope
		End Function
	End Class
End Namespace