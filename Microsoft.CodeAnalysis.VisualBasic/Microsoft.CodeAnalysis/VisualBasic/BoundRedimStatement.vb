Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRedimStatement
		Inherits BoundStatement
		Private ReadOnly _Clauses As ImmutableArray(Of BoundRedimClause)

		Public ReadOnly Property Clauses As ImmutableArray(Of BoundRedimClause)
			Get
				Return Me._Clauses
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal clauses As ImmutableArray(Of BoundRedimClause), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RedimStatement, syntax, If(hasErrors, True, clauses.NonNullAndHasErrors()))
			Me._Clauses = clauses
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRedimStatement(Me)
		End Function

		Public Function Update(ByVal clauses As ImmutableArray(Of BoundRedimClause)) As Microsoft.CodeAnalysis.VisualBasic.BoundRedimStatement
			Dim boundRedimStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRedimStatement
			If (clauses = Me.Clauses) Then
				boundRedimStatement = Me
			Else
				Dim boundRedimStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundRedimStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundRedimStatement(MyBase.Syntax, clauses, MyBase.HasErrors)
				boundRedimStatement1.CopyAttributes(Me)
				boundRedimStatement = boundRedimStatement1
			End If
			Return boundRedimStatement
		End Function
	End Class
End Namespace