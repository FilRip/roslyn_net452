Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SimpleJoinClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
		Public Shadows ReadOnly Property AdditionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)(MyBase.GetRed(Me._additionalJoins, 2))
			End Get
		End Property

		Public Overrides ReadOnly Property JoinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
			Get
				Dim joinConditionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._joinConditions, 4)
				joinConditionSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(red, MyBase.GetChildIndex(4)))
				Return joinConditionSyntaxes
			End Get
		End Property

		Public Overrides ReadOnly Property JoinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
			Get
				Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._joinedVariables, 1)
				collectionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(red, MyBase.GetChildIndex(1)))
				Return collectionRangeVariableSyntaxes
			End Get
		End Property

		Public Shadows ReadOnly Property JoinKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax)._joinKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public Shadows ReadOnly Property OnKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax)._onKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal joinKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal joinedVariables As SyntaxNode, ByVal additionalJoins As SyntaxNode, ByVal onKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal joinConditions As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(kind, errors, annotations, joinKeyword, If(joinedVariables IsNot Nothing, joinedVariables.Green, Nothing), If(additionalJoins IsNot Nothing, additionalJoins.Green, Nothing), onKeyword, If(joinConditions IsNot Nothing, joinConditions.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSimpleJoinClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSimpleJoinClause(Me)
		End Sub

		Public Shadows Function AddAdditionalJoins(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.WithAdditionalJoins(Me.AdditionalJoins.AddRange(items))
		End Function

		Friend Overrides Function AddAdditionalJoinsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddAdditionalJoins(items)
		End Function

		Public Shadows Function AddJoinConditions(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.WithJoinConditions(Me.JoinConditions.AddRange(items))
		End Function

		Friend Overrides Function AddJoinConditionsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddJoinConditions(items)
		End Function

		Public Shadows Function AddJoinedVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.WithJoinedVariables(Me.JoinedVariables.AddRange(items))
		End Function

		Friend Overrides Function AddJoinedVariablesCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddJoinedVariables(items)
		End Function

		Friend Overrides Function GetAdditionalJoinsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)
			Return Me.AdditionalJoins
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._joinedVariables
					Exit Select
				Case 2
					syntaxNode = Me._additionalJoins
					Exit Select
				Case 3
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 4
					syntaxNode = Me._joinConditions
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetJoinKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.JoinKeyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			Select Case i
				Case 1
					red = MyBase.GetRed(Me._joinedVariables, 1)
					Exit Select
				Case 2
					red = MyBase.GetRed(Me._additionalJoins, 2)
					Exit Select
				Case 3
				Label0:
					red = Nothing
					Exit Select
				Case 4
					red = MyBase.GetRed(Me._joinConditions, 4)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return red
		End Function

		Friend Overrides Function GetOnKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.OnKeyword
		End Function

		Public Function Update(ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax), ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax), ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Dim simpleJoinClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			If (joinKeyword <> Me.JoinKeyword OrElse joinedVariables <> Me.JoinedVariables OrElse additionalJoins <> Me.AdditionalJoins OrElse onKeyword <> Me.OnKeyword OrElse joinConditions <> Me.JoinConditions) Then
				Dim simpleJoinClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SimpleJoinClause(joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				simpleJoinClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, simpleJoinClauseSyntax1, simpleJoinClauseSyntax1.WithAnnotations(annotations))
			Else
				simpleJoinClauseSyntax = Me
			End If
			Return simpleJoinClauseSyntax
		End Function

		Public Shadows Function WithAdditionalJoins(ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.Update(Me.JoinKeyword, Me.JoinedVariables, additionalJoins, Me.OnKeyword, Me.JoinConditions)
		End Function

		Friend Overrides Function WithAdditionalJoinsCore(ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithAdditionalJoins(additionalJoins)
		End Function

		Public Shadows Function WithJoinConditions(ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.Update(Me.JoinKeyword, Me.JoinedVariables, Me.AdditionalJoins, Me.OnKeyword, joinConditions)
		End Function

		Friend Overrides Function WithJoinConditionsCore(ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinConditions(joinConditions)
		End Function

		Public Shadows Function WithJoinedVariables(ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.Update(Me.JoinKeyword, joinedVariables, Me.AdditionalJoins, Me.OnKeyword, Me.JoinConditions)
		End Function

		Friend Overrides Function WithJoinedVariablesCore(ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinedVariables(joinedVariables)
		End Function

		Public Shadows Function WithJoinKeyword(ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.Update(joinKeyword, Me.JoinedVariables, Me.AdditionalJoins, Me.OnKeyword, Me.JoinConditions)
		End Function

		Friend Overrides Function WithJoinKeywordCore(ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinKeyword(joinKeyword)
		End Function

		Public Shadows Function WithOnKeyword(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax
			Return Me.Update(Me.JoinKeyword, Me.JoinedVariables, Me.AdditionalJoins, onKeyword, Me.JoinConditions)
		End Function

		Friend Overrides Function WithOnKeywordCore(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithOnKeyword(onKeyword)
		End Function
	End Class
End Namespace