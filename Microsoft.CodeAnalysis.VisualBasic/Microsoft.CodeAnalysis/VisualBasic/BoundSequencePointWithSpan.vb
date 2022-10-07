Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSequencePointWithSpan
		Inherits BoundStatement
		Private ReadOnly _StatementOpt As BoundStatement

		Private ReadOnly _Span As TextSpan

		Public ReadOnly Property Span As TextSpan
			Get
				Return Me._Span
			End Get
		End Property

		Public ReadOnly Property StatementOpt As BoundStatement
			Get
				Return Me._StatementOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal statementOpt As BoundStatement, ByVal span As TextSpan, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.SequencePointWithSpan, syntax, If(hasErrors, True, statementOpt.NonNullAndHasErrors()))
			Me._StatementOpt = statementOpt
			Me._Span = span
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSequencePointWithSpan(Me)
		End Function

		Public Function Update(ByVal statementOpt As BoundStatement, ByVal span As TextSpan) As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointWithSpan
			Dim boundSequencePointWithSpan As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointWithSpan
			If (statementOpt <> Me.StatementOpt OrElse span <> Me.Span) Then
				Dim boundSequencePointWithSpan1 As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointWithSpan = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointWithSpan(MyBase.Syntax, statementOpt, span, MyBase.HasErrors)
				boundSequencePointWithSpan1.CopyAttributes(Me)
				boundSequencePointWithSpan = boundSequencePointWithSpan1
			Else
				boundSequencePointWithSpan = Me
			End If
			Return boundSequencePointWithSpan
		End Function
	End Class
End Namespace