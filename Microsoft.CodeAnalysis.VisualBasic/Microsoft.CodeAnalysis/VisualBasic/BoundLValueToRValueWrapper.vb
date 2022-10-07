Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLValueToRValueWrapper
		Inherits BoundExpression
		Private ReadOnly _UnderlyingLValue As BoundExpression

		Public ReadOnly Property UnderlyingLValue As BoundExpression
			Get
				Return Me._UnderlyingLValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal underlyingLValue As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.LValueToRValueWrapper, syntax, type, If(hasErrors, True, underlyingLValue.NonNullAndHasErrors()))
			Me._UnderlyingLValue = underlyingLValue
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLValueToRValueWrapper(Me)
		End Function

		Public Function Update(ByVal underlyingLValue As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLValueToRValueWrapper
			Dim boundLValueToRValueWrapper As Microsoft.CodeAnalysis.VisualBasic.BoundLValueToRValueWrapper
			If (underlyingLValue <> Me.UnderlyingLValue OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLValueToRValueWrapper1 As Microsoft.CodeAnalysis.VisualBasic.BoundLValueToRValueWrapper = New Microsoft.CodeAnalysis.VisualBasic.BoundLValueToRValueWrapper(MyBase.Syntax, underlyingLValue, type, MyBase.HasErrors)
				boundLValueToRValueWrapper1.CopyAttributes(Me)
				boundLValueToRValueWrapper = boundLValueToRValueWrapper1
			Else
				boundLValueToRValueWrapper = Me
			End If
			Return boundLValueToRValueWrapper
		End Function
	End Class
End Namespace