Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundForToStatement
		Inherits BoundForStatement
		Private ReadOnly _InitialValue As BoundExpression

		Private ReadOnly _LimitValue As BoundExpression

		Private ReadOnly _StepValue As BoundExpression

		Private ReadOnly _Checked As Boolean

		Private ReadOnly _OperatorsOpt As BoundForToUserDefinedOperators

		Public ReadOnly Property Checked As Boolean
			Get
				Return Me._Checked
			End Get
		End Property

		Public ReadOnly Property InitialValue As BoundExpression
			Get
				Return Me._InitialValue
			End Get
		End Property

		Public ReadOnly Property LimitValue As BoundExpression
			Get
				Return Me._LimitValue
			End Get
		End Property

		Public ReadOnly Property OperatorsOpt As BoundForToUserDefinedOperators
			Get
				Return Me._OperatorsOpt
			End Get
		End Property

		Public ReadOnly Property StepValue As BoundExpression
			Get
				Return Me._StepValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal initialValue As BoundExpression, ByVal limitValue As BoundExpression, ByVal stepValue As BoundExpression, ByVal checked As Boolean, ByVal operatorsOpt As BoundForToUserDefinedOperators, ByVal declaredOrInferredLocalOpt As LocalSymbol, ByVal controlVariable As BoundExpression, ByVal body As BoundStatement, ByVal nextVariablesOpt As ImmutableArray(Of BoundExpression), ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ForToStatement, syntax, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, If(hasErrors OrElse initialValue.NonNullAndHasErrors() OrElse limitValue.NonNullAndHasErrors() OrElse stepValue.NonNullAndHasErrors() OrElse operatorsOpt.NonNullAndHasErrors() OrElse controlVariable.NonNullAndHasErrors() OrElse body.NonNullAndHasErrors(), True, nextVariablesOpt.NonNullAndHasErrors()))
			Me._InitialValue = initialValue
			Me._LimitValue = limitValue
			Me._StepValue = stepValue
			Me._Checked = checked
			Me._OperatorsOpt = operatorsOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitForToStatement(Me)
		End Function

		Public Function Update(ByVal initialValue As BoundExpression, ByVal limitValue As BoundExpression, ByVal stepValue As BoundExpression, ByVal checked As Boolean, ByVal operatorsOpt As BoundForToUserDefinedOperators, ByVal declaredOrInferredLocalOpt As LocalSymbol, ByVal controlVariable As BoundExpression, ByVal body As BoundStatement, ByVal nextVariablesOpt As ImmutableArray(Of BoundExpression), ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundForToStatement
			Dim boundForToStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForToStatement
			If (initialValue <> Me.InitialValue OrElse limitValue <> Me.LimitValue OrElse stepValue <> Me.StepValue OrElse checked <> Me.Checked OrElse operatorsOpt <> Me.OperatorsOpt OrElse CObj(declaredOrInferredLocalOpt) <> CObj(MyBase.DeclaredOrInferredLocalOpt) OrElse controlVariable <> MyBase.ControlVariable OrElse body <> MyBase.Body OrElse nextVariablesOpt <> MyBase.NextVariablesOpt OrElse CObj(continueLabel) <> CObj(MyBase.ContinueLabel) OrElse CObj(exitLabel) <> CObj(MyBase.ExitLabel)) Then
				Dim boundForToStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundForToStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundForToStatement(MyBase.Syntax, initialValue, limitValue, stepValue, checked, operatorsOpt, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, MyBase.HasErrors)
				boundForToStatement1.CopyAttributes(Me)
				boundForToStatement = boundForToStatement1
			Else
				boundForToStatement = Me
			End If
			Return boundForToStatement
		End Function
	End Class
End Namespace