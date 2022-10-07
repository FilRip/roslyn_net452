Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTypeOf
		Inherits BoundExpression
		Private ReadOnly _Operand As BoundExpression

		Private ReadOnly _IsTypeOfIsNotExpression As Boolean

		Private ReadOnly _TargetType As TypeSymbol

		Public ReadOnly Property IsTypeOfIsNotExpression As Boolean
			Get
				Return Me._IsTypeOfIsNotExpression
			End Get
		End Property

		Public ReadOnly Property Operand As BoundExpression
			Get
				Return Me._Operand
			End Get
		End Property

		Public ReadOnly Property TargetType As TypeSymbol
			Get
				Return Me._TargetType
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal isTypeOfIsNotExpression As Boolean, ByVal targetType As TypeSymbol, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.[TypeOf], syntax, type, If(hasErrors, True, operand.NonNullAndHasErrors()))
			Me._Operand = operand
			Me._IsTypeOfIsNotExpression = isTypeOfIsNotExpression
			Me._TargetType = targetType
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTypeOf(Me)
		End Function

		Public Function Update(ByVal operand As BoundExpression, ByVal isTypeOfIsNotExpression As Boolean, ByVal targetType As TypeSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf
			Dim boundTypeOf As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf
			If (operand <> Me.Operand OrElse isTypeOfIsNotExpression <> Me.IsTypeOfIsNotExpression OrElse CObj(targetType) <> CObj(Me.TargetType) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundTypeOf1 As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf = New Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf(MyBase.Syntax, operand, isTypeOfIsNotExpression, targetType, type, MyBase.HasErrors)
				boundTypeOf1.CopyAttributes(Me)
				boundTypeOf = boundTypeOf1
			Else
				boundTypeOf = Me
			End If
			Return boundTypeOf
		End Function
	End Class
End Namespace