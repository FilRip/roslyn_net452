Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure BoundTypeOrValueData
		Implements IEquatable(Of BoundTypeOrValueData)
		Private ReadOnly _valueExpression As BoundExpression

		Private ReadOnly _valueDiagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Private ReadOnly _typeExpression As BoundExpression

		Private ReadOnly _typeDiagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Public ReadOnly Property TypeDiagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Get
				Return Me._typeDiagnostics
			End Get
		End Property

		Public ReadOnly Property TypeExpression As BoundExpression
			Get
				Return Me._typeExpression
			End Get
		End Property

		Public ReadOnly Property ValueDiagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Get
				Return Me._valueDiagnostics
			End Get
		End Property

		Public ReadOnly Property ValueExpression As BoundExpression
			Get
				Return Me._valueExpression
			End Get
		End Property

		Public Sub New(ByVal valueExpression As BoundExpression, ByVal valueDiagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal typeExpression As BoundExpression, ByVal typeDiagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Me = New BoundTypeOrValueData() With
			{
				._valueExpression = valueExpression,
				._valueDiagnostics = valueDiagnostics,
				._typeExpression = typeExpression,
				._typeDiagnostics = typeDiagnostics
			}
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (Not TypeOf obj Is BoundTypeOrValueData) Then
				Return False
			End If
			Return DirectCast(obj, BoundTypeOrValueData) = Me
		End Function

		Private Function ExplicitEquals(ByVal b As BoundTypeOrValueData) As Boolean Implements IEquatable(Of BoundTypeOrValueData).Equals
			Return b = Me
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me.ValueExpression.GetHashCode(), Hash.Combine(Me.ValueDiagnostics.GetHashCode(), Hash.Combine(Me.TypeExpression.GetHashCode(), Me.TypeDiagnostics.GetHashCode())))
		End Function

		Public Shared Operator =(ByVal a As BoundTypeOrValueData, ByVal b As BoundTypeOrValueData) As Boolean
			If (a.ValueExpression <> b.ValueExpression OrElse a.ValueDiagnostics <> b.ValueDiagnostics OrElse a.TypeExpression <> b.TypeExpression) Then
				Return False
			End If
			Return a.TypeDiagnostics = b.TypeDiagnostics
		End Operator

		Public Shared Operator <>(ByVal a As BoundTypeOrValueData, ByVal b As BoundTypeOrValueData) As Boolean
			Return Not (a = b)
		End Operator
	End Structure
End Namespace