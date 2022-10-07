Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundNameOfOperator
		Inherits BoundExpression
		Private ReadOnly _Argument As BoundExpression

		Private ReadOnly _ConstantValueOpt As ConstantValue

		Public ReadOnly Property Argument As BoundExpression
			Get
				Return Me._Argument
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Argument)
			End Get
		End Property

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me._ConstantValueOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal argument As BoundExpression, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.NameOfOperator, syntax, type, If(hasErrors, True, argument.NonNullAndHasErrors()))
			Me._Argument = argument
			Me._ConstantValueOpt = constantValueOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitNameOfOperator(Me)
		End Function

		Public Function Update(ByVal argument As BoundExpression, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundNameOfOperator
			Dim boundNameOfOperator As Microsoft.CodeAnalysis.VisualBasic.BoundNameOfOperator
			If (argument <> Me.Argument OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundNameOfOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundNameOfOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundNameOfOperator(MyBase.Syntax, argument, constantValueOpt, type, MyBase.HasErrors)
				boundNameOfOperator1.CopyAttributes(Me)
				boundNameOfOperator = boundNameOfOperator1
			Else
				boundNameOfOperator = Me
			End If
			Return boundNameOfOperator
		End Function
	End Class
End Namespace