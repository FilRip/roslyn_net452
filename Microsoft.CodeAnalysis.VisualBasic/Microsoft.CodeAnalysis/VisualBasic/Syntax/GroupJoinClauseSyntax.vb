Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class GroupJoinClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
		Friend _aggregationVariables As SyntaxNode

		Public Shadows ReadOnly Property AdditionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)(MyBase.GetRed(Me._additionalJoins, 3))
			End Get
		End Property

		Public ReadOnly Property AggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)
			Get
				Dim aggregationRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._aggregationVariables, 7)
				aggregationRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(red, MyBase.GetChildIndex(7)))
				Return aggregationRangeVariableSyntaxes
			End Get
		End Property

		Public ReadOnly Property GroupKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)._groupKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property IntoKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)._intoKeyword, Me.GetChildPosition(6), MyBase.GetChildIndex(6))
			End Get
		End Property

		Public Overrides ReadOnly Property JoinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
			Get
				Dim joinConditionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._joinConditions, 5)
				joinConditionSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(red, MyBase.GetChildIndex(5)))
				Return joinConditionSyntaxes
			End Get
		End Property

		Public Overrides ReadOnly Property JoinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
			Get
				Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._joinedVariables, 2)
				collectionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(red, MyBase.GetChildIndex(2)))
				Return collectionRangeVariableSyntaxes
			End Get
		End Property

		Public Shadows ReadOnly Property JoinKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)._joinKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public Shadows ReadOnly Property OnKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)._onKeyword, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal groupKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal joinKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal joinedVariables As SyntaxNode, ByVal additionalJoins As SyntaxNode, ByVal onKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal joinConditions As SyntaxNode, ByVal intoKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal aggregationVariables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(kind, errors, annotations, groupKeyword, joinKeyword, If(joinedVariables IsNot Nothing, joinedVariables.Green, Nothing), If(additionalJoins IsNot Nothing, additionalJoins.Green, Nothing), onKeyword, If(joinConditions IsNot Nothing, joinConditions.Green, Nothing), intoKeyword, If(aggregationVariables IsNot Nothing, aggregationVariables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitGroupJoinClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitGroupJoinClause(Me)
		End Sub

		Public Shadows Function AddAdditionalJoins(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.WithAdditionalJoins(Me.AdditionalJoins.AddRange(items))
		End Function

		Friend Overrides Function AddAdditionalJoinsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddAdditionalJoins(items)
		End Function

		Public Function AddAggregationVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.WithAggregationVariables(Me.AggregationVariables.AddRange(items))
		End Function

		Public Shadows Function AddJoinConditions(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.WithJoinConditions(Me.JoinConditions.AddRange(items))
		End Function

		Friend Overrides Function AddJoinConditionsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddJoinConditions(items)
		End Function

		Public Shadows Function AddJoinedVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
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
				Case 2
					syntaxNode = Me._joinedVariables
					Exit Select
				Case 3
					syntaxNode = Me._additionalJoins
					Exit Select
				Case 4
				Case 6
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 5
					syntaxNode = Me._joinConditions
					Exit Select
				Case 7
					syntaxNode = Me._aggregationVariables
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
				Case 2
					red = MyBase.GetRed(Me._joinedVariables, 2)
					Exit Select
				Case 3
					red = MyBase.GetRed(Me._additionalJoins, 3)
					Exit Select
				Case 4
				Case 6
				Label0:
					red = Nothing
					Exit Select
				Case 5
					red = MyBase.GetRed(Me._joinConditions, 5)
					Exit Select
				Case 7
					red = MyBase.GetRed(Me._aggregationVariables, 7)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return red
		End Function

		Friend Overrides Function GetOnKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.OnKeyword
		End Function

		Public Function Update(ByVal groupKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax), ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax), ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax), ByVal intoKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal aggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Dim groupJoinClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			If (groupKeyword <> Me.GroupKeyword OrElse joinKeyword <> Me.JoinKeyword OrElse joinedVariables <> Me.JoinedVariables OrElse additionalJoins <> Me.AdditionalJoins OrElse onKeyword <> Me.OnKeyword OrElse joinConditions <> Me.JoinConditions OrElse intoKeyword <> Me.IntoKeyword OrElse aggregationVariables <> Me.AggregationVariables) Then
				Dim groupJoinClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.GroupJoinClause(groupKeyword, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions, intoKeyword, aggregationVariables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				groupJoinClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, groupJoinClauseSyntax1, groupJoinClauseSyntax1.WithAnnotations(annotations))
			Else
				groupJoinClauseSyntax = Me
			End If
			Return groupJoinClauseSyntax
		End Function

		Public Shadows Function WithAdditionalJoins(ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.JoinKeyword, Me.JoinedVariables, additionalJoins, Me.OnKeyword, Me.JoinConditions, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Friend Overrides Function WithAdditionalJoinsCore(ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithAdditionalJoins(additionalJoins)
		End Function

		Public Function WithAggregationVariables(ByVal aggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.JoinKeyword, Me.JoinedVariables, Me.AdditionalJoins, Me.OnKeyword, Me.JoinConditions, Me.IntoKeyword, aggregationVariables)
		End Function

		Public Function WithGroupKeyword(ByVal groupKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(groupKeyword, Me.JoinKeyword, Me.JoinedVariables, Me.AdditionalJoins, Me.OnKeyword, Me.JoinConditions, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithIntoKeyword(ByVal intoKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.JoinKeyword, Me.JoinedVariables, Me.AdditionalJoins, Me.OnKeyword, Me.JoinConditions, intoKeyword, Me.AggregationVariables)
		End Function

		Public Shadows Function WithJoinConditions(ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.JoinKeyword, Me.JoinedVariables, Me.AdditionalJoins, Me.OnKeyword, joinConditions, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Friend Overrides Function WithJoinConditionsCore(ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinConditions(joinConditions)
		End Function

		Public Shadows Function WithJoinedVariables(ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.JoinKeyword, joinedVariables, Me.AdditionalJoins, Me.OnKeyword, Me.JoinConditions, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Friend Overrides Function WithJoinedVariablesCore(ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinedVariables(joinedVariables)
		End Function

		Public Shadows Function WithJoinKeyword(ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(Me.GroupKeyword, joinKeyword, Me.JoinedVariables, Me.AdditionalJoins, Me.OnKeyword, Me.JoinConditions, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Friend Overrides Function WithJoinKeywordCore(ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinKeyword(joinKeyword)
		End Function

		Public Shadows Function WithOnKeyword(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.JoinKeyword, Me.JoinedVariables, Me.AdditionalJoins, onKeyword, Me.JoinConditions, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Friend Overrides Function WithOnKeywordCore(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithOnKeyword(onKeyword)
		End Function
	End Class
End Namespace