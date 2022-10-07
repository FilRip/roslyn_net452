Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundBadVariable
		Inherits BoundExpression
		Private ReadOnly _Expression As BoundExpression

		Private ReadOnly _IsLValue As Boolean

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return LookupResultKind.NotAValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, expression, True, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal isLValue As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.BadVariable, syntax, type, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
			Me._IsLValue = isLValue
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitBadVariable(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundBadVariable
			Dim boundBadVariable As Microsoft.CodeAnalysis.VisualBasic.BoundBadVariable
			boundBadVariable = If(Not Me._IsLValue, Me, Me.Update(Me._Expression, False, MyBase.Type))
			Return boundBadVariable
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal isLValue As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundBadVariable
			Dim boundBadVariable As Microsoft.CodeAnalysis.VisualBasic.BoundBadVariable
			If (expression <> Me.Expression OrElse isLValue <> Me.IsLValue OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundBadVariable1 As Microsoft.CodeAnalysis.VisualBasic.BoundBadVariable = New Microsoft.CodeAnalysis.VisualBasic.BoundBadVariable(MyBase.Syntax, expression, isLValue, type, MyBase.HasErrors)
				boundBadVariable1.CopyAttributes(Me)
				boundBadVariable = boundBadVariable1
			Else
				boundBadVariable = Me
			End If
			Return boundBadVariable
		End Function
	End Class
End Namespace