Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundForEachStatement
		Inherits BoundForStatement
		Private ReadOnly _Collection As BoundExpression

		Private ReadOnly _EnumeratorInfo As ForEachEnumeratorInfo

		Public ReadOnly Property Collection As BoundExpression
			Get
				Return Me._Collection
			End Get
		End Property

		Public ReadOnly Property EnumeratorInfo As ForEachEnumeratorInfo
			Get
				Return Me._EnumeratorInfo
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal collection As BoundExpression, ByVal enumeratorInfo As ForEachEnumeratorInfo, ByVal declaredOrInferredLocalOpt As LocalSymbol, ByVal controlVariable As BoundExpression, ByVal body As BoundStatement, ByVal nextVariablesOpt As ImmutableArray(Of BoundExpression), ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ForEachStatement, syntax, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, If(hasErrors OrElse collection.NonNullAndHasErrors() OrElse controlVariable.NonNullAndHasErrors() OrElse body.NonNullAndHasErrors(), True, nextVariablesOpt.NonNullAndHasErrors()))
			Me._Collection = collection
			Me._EnumeratorInfo = enumeratorInfo
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitForEachStatement(Me)
		End Function

		Public Function Update(ByVal collection As BoundExpression, ByVal enumeratorInfo As ForEachEnumeratorInfo, ByVal declaredOrInferredLocalOpt As LocalSymbol, ByVal controlVariable As BoundExpression, ByVal body As BoundStatement, ByVal nextVariablesOpt As ImmutableArray(Of BoundExpression), ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundForEachStatement
			Dim boundForEachStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForEachStatement
			If (collection <> Me.Collection OrElse enumeratorInfo <> Me.EnumeratorInfo OrElse CObj(declaredOrInferredLocalOpt) <> CObj(MyBase.DeclaredOrInferredLocalOpt) OrElse controlVariable <> MyBase.ControlVariable OrElse body <> MyBase.Body OrElse nextVariablesOpt <> MyBase.NextVariablesOpt OrElse CObj(continueLabel) <> CObj(MyBase.ContinueLabel) OrElse CObj(exitLabel) <> CObj(MyBase.ExitLabel)) Then
				Dim boundForEachStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundForEachStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundForEachStatement(MyBase.Syntax, collection, enumeratorInfo, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, MyBase.HasErrors)
				boundForEachStatement1.CopyAttributes(Me)
				boundForEachStatement = boundForEachStatement1
			Else
				boundForEachStatement = Me
			End If
			Return boundForEachStatement
		End Function
	End Class
End Namespace