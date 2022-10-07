Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ForStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
		Friend _fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax

		Public Shadows ReadOnly Property ControlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me._controlVariable, 1)
			End Get
		End Property

		Public ReadOnly Property EqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)._equalsToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public Shadows ReadOnly Property ForKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)._forKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property FromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._fromValue, 3)
			End Get
		End Property

		Public ReadOnly Property StepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax)(Me._stepClause, 6)
			End Get
		End Property

		Public ReadOnly Property ToKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)._toKeyword, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Public ReadOnly Property ToValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._toValue, 5)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal equalsToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal toKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(kind, errors, annotations, forKeyword, DirectCast(controlVariable.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), equalsToken, DirectCast(fromValue.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), toKeyword, DirectCast(toValue.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), If(stepClause IsNot Nothing, DirectCast(stepClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitForStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitForStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._controlVariable
					Exit Select
				Case 2
				Case 4
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 3
					syntaxNode = Me._fromValue
					Exit Select
				Case 5
					syntaxNode = Me._toValue
					Exit Select
				Case 6
					syntaxNode = Me._stepClause
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetControlVariableCore() As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Return Me.ControlVariable
		End Function

		Friend Overrides Function GetForKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.ForKeyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim controlVariable As SyntaxNode
			Select Case i
				Case 1
					controlVariable = Me.ControlVariable
					Exit Select
				Case 2
				Case 4
				Label0:
					controlVariable = Nothing
					Exit Select
				Case 3
					controlVariable = Me.FromValue
					Exit Select
				Case 5
					controlVariable = Me.ToValue
					Exit Select
				Case 6
					controlVariable = Me.StepClause
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return controlVariable
		End Function

		Public Function Update(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal toKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Dim forStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			If (forKeyword <> Me.ForKeyword OrElse controlVariable <> Me.ControlVariable OrElse equalsToken <> Me.EqualsToken OrElse fromValue <> Me.FromValue OrElse toKeyword <> Me.ToKeyword OrElse toValue <> Me.ToValue OrElse stepClause <> Me.StepClause) Then
				Dim forStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ForStatement(forKeyword, controlVariable, equalsToken, fromValue, toKeyword, toValue, stepClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				forStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, forStatementSyntax1, forStatementSyntax1.WithAnnotations(annotations))
			Else
				forStatementSyntax = Me
			End If
			Return forStatementSyntax
		End Function

		Public Shadows Function WithControlVariable(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Return Me.Update(Me.ForKeyword, controlVariable, Me.EqualsToken, Me.FromValue, Me.ToKeyword, Me.ToValue, Me.StepClause)
		End Function

		Friend Overrides Function WithControlVariableCore(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Return Me.WithControlVariable(controlVariable)
		End Function

		Public Function WithEqualsToken(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.ControlVariable, equalsToken, Me.FromValue, Me.ToKeyword, Me.ToValue, Me.StepClause)
		End Function

		Public Shadows Function WithForKeyword(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Return Me.Update(forKeyword, Me.ControlVariable, Me.EqualsToken, Me.FromValue, Me.ToKeyword, Me.ToValue, Me.StepClause)
		End Function

		Friend Overrides Function WithForKeywordCore(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Return Me.WithForKeyword(forKeyword)
		End Function

		Public Function WithFromValue(ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.ControlVariable, Me.EqualsToken, fromValue, Me.ToKeyword, Me.ToValue, Me.StepClause)
		End Function

		Public Function WithStepClause(ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.ControlVariable, Me.EqualsToken, Me.FromValue, Me.ToKeyword, Me.ToValue, stepClause)
		End Function

		Public Function WithToKeyword(ByVal toKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.ControlVariable, Me.EqualsToken, Me.FromValue, toKeyword, Me.ToValue, Me.StepClause)
		End Function

		Public Function WithToValue(ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.ControlVariable, Me.EqualsToken, Me.FromValue, Me.ToKeyword, toValue, Me.StepClause)
		End Function
	End Class
End Namespace