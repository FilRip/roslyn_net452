Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundWithStatement
		Inherits BoundStatement
		Private ReadOnly _OriginalExpression As BoundExpression

		Private ReadOnly _Body As BoundBlock

		Private ReadOnly _Binder As WithBlockBinder

		Public ReadOnly Property Binder As WithBlockBinder
			Get
				Return Me._Binder
			End Get
		End Property

		Public ReadOnly Property Body As BoundBlock
			Get
				Return Me._Body
			End Get
		End Property

		Friend ReadOnly Property DraftInitializers As ImmutableArray(Of BoundExpression)
			Get
				Return Me.Binder.DraftInitializers
			End Get
		End Property

		Friend ReadOnly Property DraftPlaceholderSubstitute As BoundExpression
			Get
				Return Me.Binder.DraftPlaceholderSubstitute
			End Get
		End Property

		Friend ReadOnly Property ExpressionPlaceholder As BoundValuePlaceholderBase
			Get
				Return Me.Binder.ExpressionPlaceholder
			End Get
		End Property

		Public ReadOnly Property OriginalExpression As BoundExpression
			Get
				Return Me._OriginalExpression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal originalExpression As BoundExpression, ByVal body As BoundBlock, ByVal binder As WithBlockBinder, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.WithStatement, syntax, If(hasErrors OrElse originalExpression.NonNullAndHasErrors(), True, body.NonNullAndHasErrors()))
			Me._OriginalExpression = originalExpression
			Me._Body = body
			Me._Binder = binder
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitWithStatement(Me)
		End Function

		Public Function Update(ByVal originalExpression As BoundExpression, ByVal body As BoundBlock, ByVal binder As WithBlockBinder) As Microsoft.CodeAnalysis.VisualBasic.BoundWithStatement
			Dim boundWithStatement As Microsoft.CodeAnalysis.VisualBasic.BoundWithStatement
			If (originalExpression <> Me.OriginalExpression OrElse body <> Me.Body OrElse binder <> Me.Binder) Then
				Dim boundWithStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundWithStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundWithStatement(MyBase.Syntax, originalExpression, body, binder, MyBase.HasErrors)
				boundWithStatement1.CopyAttributes(Me)
				boundWithStatement = boundWithStatement1
			Else
				boundWithStatement = Me
			End If
			Return boundWithStatement
		End Function
	End Class
End Namespace