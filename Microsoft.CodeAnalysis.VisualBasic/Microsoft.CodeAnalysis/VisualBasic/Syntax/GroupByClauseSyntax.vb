Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class GroupByClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _items As SyntaxNode

		Friend _keys As SyntaxNode

		Friend _aggregationVariables As SyntaxNode

		Public ReadOnly Property AggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)
			Get
				Dim aggregationRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._aggregationVariables, 5)
				aggregationRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(red, MyBase.GetChildIndex(5)))
				Return aggregationRangeVariableSyntaxes
			End Get
		End Property

		Public ReadOnly Property ByKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax)._byKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property GroupKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax)._groupKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property IntoKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax)._intoKeyword, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Public ReadOnly Property Items As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
			Get
				Dim expressionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._items, 1)
				expressionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(red, MyBase.GetChildIndex(1)))
				Return expressionRangeVariableSyntaxes
			End Get
		End Property

		Public ReadOnly Property Keys As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
			Get
				Dim expressionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._keys, 3)
				expressionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(red, MyBase.GetChildIndex(3)))
				Return expressionRangeVariableSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal groupKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal items As SyntaxNode, ByVal byKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal keys As SyntaxNode, ByVal intoKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal aggregationVariables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(kind, errors, annotations, groupKeyword, If(items IsNot Nothing, items.Green, Nothing), byKeyword, If(keys IsNot Nothing, keys.Green, Nothing), intoKeyword, If(aggregationVariables IsNot Nothing, aggregationVariables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitGroupByClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitGroupByClause(Me)
		End Sub

		Public Function AddAggregationVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.WithAggregationVariables(Me.AggregationVariables.AddRange(items))
		End Function

		Public Function AddItems(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.WithItems(Me.Items.AddRange(items))
		End Function

		Public Function AddKeys(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.WithKeys(Me.Keys.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._items
					Exit Select
				Case 2
				Case 4
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 3
					syntaxNode = Me._keys
					Exit Select
				Case 5
					syntaxNode = Me._aggregationVariables
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			Select Case i
				Case 1
					red = MyBase.GetRed(Me._items, 1)
					Exit Select
				Case 2
				Case 4
				Label0:
					red = Nothing
					Exit Select
				Case 3
					red = MyBase.GetRed(Me._keys, 3)
					Exit Select
				Case 5
					red = MyBase.GetRed(Me._aggregationVariables, 5)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return red
		End Function

		Public Function Update(ByVal groupKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal items As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax), ByVal byKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal keys As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax), ByVal intoKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal aggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Dim groupByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			If (groupKeyword <> Me.GroupKeyword OrElse items <> Me.Items OrElse byKeyword <> Me.ByKeyword OrElse keys <> Me.Keys OrElse intoKeyword <> Me.IntoKeyword OrElse aggregationVariables <> Me.AggregationVariables) Then
				Dim groupByClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.GroupByClause(groupKeyword, items, byKeyword, keys, intoKeyword, aggregationVariables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				groupByClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, groupByClauseSyntax1, groupByClauseSyntax1.WithAnnotations(annotations))
			Else
				groupByClauseSyntax = Me
			End If
			Return groupByClauseSyntax
		End Function

		Public Function WithAggregationVariables(ByVal aggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.Items, Me.ByKeyword, Me.Keys, Me.IntoKeyword, aggregationVariables)
		End Function

		Public Function WithByKeyword(ByVal byKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.Items, byKeyword, Me.Keys, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithGroupKeyword(ByVal groupKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.Update(groupKeyword, Me.Items, Me.ByKeyword, Me.Keys, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithIntoKeyword(ByVal intoKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.Items, Me.ByKeyword, Me.Keys, intoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithItems(ByVal items As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.Update(Me.GroupKeyword, items, Me.ByKeyword, Me.Keys, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithKeys(ByVal keys As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax
			Return Me.Update(Me.GroupKeyword, Me.Items, Me.ByKeyword, keys, Me.IntoKeyword, Me.AggregationVariables)
		End Function
	End Class
End Namespace