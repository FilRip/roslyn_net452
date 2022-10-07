Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRangeVariableAssignment
		Inherits BoundQueryPart
		Private ReadOnly _RangeVariable As RangeVariableSymbol

		Private ReadOnly _Value As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Value)
			End Get
		End Property

		Public ReadOnly Property RangeVariable As RangeVariableSymbol
			Get
				Return Me._RangeVariable
			End Get
		End Property

		Public ReadOnly Property Value As BoundExpression
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal rangeVariable As RangeVariableSymbol, ByVal value As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RangeVariableAssignment, syntax, type, If(hasErrors, True, value.NonNullAndHasErrors()))
			Me._RangeVariable = rangeVariable
			Me._Value = value
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRangeVariableAssignment(Me)
		End Function

		Public Function Update(ByVal rangeVariable As RangeVariableSymbol, ByVal value As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariableAssignment
			Dim boundRangeVariableAssignment As Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariableAssignment
			If (CObj(rangeVariable) <> CObj(Me.RangeVariable) OrElse value <> Me.Value OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundRangeVariableAssignment1 As Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariableAssignment = New Microsoft.CodeAnalysis.VisualBasic.BoundRangeVariableAssignment(MyBase.Syntax, rangeVariable, value, type, MyBase.HasErrors)
				boundRangeVariableAssignment1.CopyAttributes(Me)
				boundRangeVariableAssignment = boundRangeVariableAssignment1
			Else
				boundRangeVariableAssignment = Me
			End If
			Return boundRangeVariableAssignment
		End Function
	End Class
End Namespace