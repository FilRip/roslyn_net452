Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundArrayAccess
		Inherits BoundExpression
		Private ReadOnly _Expression As BoundExpression

		Private ReadOnly _Indices As ImmutableArray(Of BoundExpression)

		Private ReadOnly _IsLValue As Boolean

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public ReadOnly Property Indices As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Indices
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal indices As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, expression, indices, True, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal indices As ImmutableArray(Of BoundExpression), ByVal isLValue As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ArrayAccess, syntax, type, If(hasErrors OrElse expression.NonNullAndHasErrors(), True, indices.NonNullAndHasErrors()))
			Me._Expression = expression
			Me._Indices = indices
			Me._IsLValue = isLValue
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitArrayAccess(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess
			Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess
			boundArrayAccess = If(Not Me._IsLValue, Me, Me.Update(Me._Expression, Me._Indices, False, MyBase.Type))
			Return boundArrayAccess
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal indices As ImmutableArray(Of BoundExpression), ByVal isLValue As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess
			Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess
			If (expression <> Me.Expression OrElse indices <> Me.Indices OrElse isLValue <> Me.IsLValue OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundArrayAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess(MyBase.Syntax, expression, indices, isLValue, type, MyBase.HasErrors)
				boundArrayAccess1.CopyAttributes(Me)
				boundArrayAccess = boundArrayAccess1
			Else
				boundArrayAccess = Me
			End If
			Return boundArrayAccess
		End Function
	End Class
End Namespace