Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundQuerySource
		Inherits BoundQueryPart
		Private ReadOnly _Expression As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Expression)
			End Get
		End Property

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.Expression.ExpressionSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me.Expression.ResultKind
			End Get
		End Property

		Public Sub New(ByVal source As BoundExpression)
			MyClass.New(source.Syntax, source, source.Type, False)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.QuerySource, syntax, type, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitQuerySource(Me)
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundQuerySource
			Dim boundQuerySource As Microsoft.CodeAnalysis.VisualBasic.BoundQuerySource
			If (expression <> Me.Expression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundQuerySource1 As Microsoft.CodeAnalysis.VisualBasic.BoundQuerySource = New Microsoft.CodeAnalysis.VisualBasic.BoundQuerySource(MyBase.Syntax, expression, type, MyBase.HasErrors)
				boundQuerySource1.CopyAttributes(Me)
				boundQuerySource = boundQuerySource1
			Else
				boundQuerySource = Me
			End If
			Return boundQuerySource
		End Function
	End Class
End Namespace