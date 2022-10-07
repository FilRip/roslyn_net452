Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundParameterEqualsValue
		Inherits BoundNode
		Private ReadOnly _Parameter As ParameterSymbol

		Private ReadOnly _Value As BoundExpression

		Public ReadOnly Property Parameter As ParameterSymbol
			Get
				Return Me._Parameter
			End Get
		End Property

		Public ReadOnly Property Value As BoundExpression
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal parameter As ParameterSymbol, ByVal value As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ParameterEqualsValue, syntax, If(hasErrors, True, value.NonNullAndHasErrors()))
			Me._Parameter = parameter
			Me._Value = value
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitParameterEqualsValue(Me)
		End Function

		Public Function Update(ByVal parameter As ParameterSymbol, ByVal value As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundParameterEqualsValue
			Dim boundParameterEqualsValue As Microsoft.CodeAnalysis.VisualBasic.BoundParameterEqualsValue
			If (CObj(parameter) <> CObj(Me.Parameter) OrElse value <> Me.Value) Then
				Dim boundParameterEqualsValue1 As Microsoft.CodeAnalysis.VisualBasic.BoundParameterEqualsValue = New Microsoft.CodeAnalysis.VisualBasic.BoundParameterEqualsValue(MyBase.Syntax, parameter, value, MyBase.HasErrors)
				boundParameterEqualsValue1.CopyAttributes(Me)
				boundParameterEqualsValue = boundParameterEqualsValue1
			Else
				boundParameterEqualsValue = Me
			End If
			Return boundParameterEqualsValue
		End Function
	End Class
End Namespace