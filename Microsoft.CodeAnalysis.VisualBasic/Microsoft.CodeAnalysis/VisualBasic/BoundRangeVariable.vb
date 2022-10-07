Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRangeVariable
		Inherits BoundExpression
		Private ReadOnly _RangeVariable As RangeVariableSymbol

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.RangeVariable
			End Get
		End Property

		Public ReadOnly Property RangeVariable As RangeVariableSymbol
			Get
				Return Me._RangeVariable
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal rangeVariable As RangeVariableSymbol, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.RangeVariable, syntax, type, hasErrors)
			Me._RangeVariable = rangeVariable
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal rangeVariable As RangeVariableSymbol, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.RangeVariable, syntax, type)
			Me._RangeVariable = rangeVariable
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRangeVariable(Me)
		End Function

		Public Function Update(ByVal rangeVariable As RangeVariableSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariable
			Dim boundRangeVariable As Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariable
			If (CObj(rangeVariable) <> CObj(Me.RangeVariable) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundRangeVariable1 As Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariable = New Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariable(MyBase.Syntax, rangeVariable, type, MyBase.HasErrors)
				boundRangeVariable1.CopyAttributes(Me)
				boundRangeVariable = boundRangeVariable1
			Else
				boundRangeVariable = Me
			End If
			Return boundRangeVariable
		End Function
	End Class
End Namespace