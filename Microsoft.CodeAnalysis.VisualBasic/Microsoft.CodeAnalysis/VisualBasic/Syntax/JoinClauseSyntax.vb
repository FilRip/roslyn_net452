Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class JoinClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _joinedVariables As SyntaxNode

		Friend _additionalJoins As SyntaxNode

		Friend _joinConditions As SyntaxNode

		Public ReadOnly Property AdditionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)
			Get
				Return Me.GetAdditionalJoinsCore()
			End Get
		End Property

		Public Overridable ReadOnly Property JoinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
			Get
				Dim joinConditionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._joinConditions, 4)
				joinConditionSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(red, MyBase.GetChildIndex(4)))
				Return joinConditionSyntaxes
			End Get
		End Property

		Public Overridable ReadOnly Property JoinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
			Get
				Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._joinedVariables, 1)
				collectionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(red, MyBase.GetChildIndex(1)))
				Return collectionRangeVariableSyntaxes
			End Get
		End Property

		Public ReadOnly Property JoinKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetJoinKeywordCore()
			End Get
		End Property

		Public ReadOnly Property OnKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetOnKeywordCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Public Function AddAdditionalJoins(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddAdditionalJoinsCore(items)
		End Function

		Friend MustOverride Function AddAdditionalJoinsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax

		Public Function AddJoinConditions(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddJoinConditionsCore(items)
		End Function

		Friend MustOverride Function AddJoinConditionsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax

		Public Function AddJoinedVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.AddJoinedVariablesCore(items)
		End Function

		Friend MustOverride Function AddJoinedVariablesCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax

		Friend Overridable Function GetAdditionalJoinsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)
			Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)(MyBase.GetRed(Me._additionalJoins, 2))
		End Function

		Friend Overridable Function GetJoinKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)._joinKeyword, MyBase.Position, 0)
		End Function

		Friend Overridable Function GetOnKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)._onKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
		End Function

		Public Function WithAdditionalJoins(ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithAdditionalJoinsCore(additionalJoins)
		End Function

		Friend MustOverride Function WithAdditionalJoinsCore(ByVal additionalJoins As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax

		Public Function WithJoinConditions(ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinConditionsCore(joinConditions)
		End Function

		Friend MustOverride Function WithJoinConditionsCore(ByVal joinConditions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax

		Public Function WithJoinedVariables(ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinedVariablesCore(joinedVariables)
		End Function

		Friend MustOverride Function WithJoinedVariablesCore(ByVal joinedVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax

		Public Function WithJoinKeyword(ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithJoinKeywordCore(joinKeyword)
		End Function

		Friend MustOverride Function WithJoinKeywordCore(ByVal joinKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax

		Public Function WithOnKeyword(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
			Return Me.WithOnKeywordCore(onKeyword)
		End Function

		Friend MustOverride Function WithOnKeywordCore(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax
	End Class
End Namespace