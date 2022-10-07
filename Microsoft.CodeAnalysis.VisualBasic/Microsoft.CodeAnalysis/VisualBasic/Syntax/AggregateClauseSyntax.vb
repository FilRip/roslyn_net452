Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AggregateClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _variables As SyntaxNode

		Friend _additionalQueryOperators As SyntaxNode

		Friend _aggregationVariables As SyntaxNode

		Public ReadOnly Property AdditionalQueryOperators As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)(MyBase.GetRed(Me._additionalQueryOperators, 2))
			End Get
		End Property

		Public ReadOnly Property AggregateKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax)._aggregateKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property AggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)
			Get
				Dim aggregationRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._aggregationVariables, 4)
				aggregationRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(red, MyBase.GetChildIndex(4)))
				Return aggregationRangeVariableSyntaxes
			End Get
		End Property

		Public ReadOnly Property IntoKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax)._intoKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property Variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
			Get
				Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._variables, 1)
				collectionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(red, MyBase.GetChildIndex(1)))
				Return collectionRangeVariableSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal aggregateKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal variables As SyntaxNode, ByVal additionalQueryOperators As SyntaxNode, ByVal intoKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal aggregationVariables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(kind, errors, annotations, aggregateKeyword, If(variables IsNot Nothing, variables.Green, Nothing), If(additionalQueryOperators IsNot Nothing, additionalQueryOperators.Green, Nothing), intoKeyword, If(aggregationVariables IsNot Nothing, aggregationVariables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAggregateClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAggregateClause(Me)
		End Sub

		Public Function AddAdditionalQueryOperators(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.WithAdditionalQueryOperators(Me.AdditionalQueryOperators.AddRange(items))
		End Function

		Public Function AddAggregationVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.WithAggregationVariables(Me.AggregationVariables.AddRange(items))
		End Function

		Public Function AddVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.WithVariables(Me.Variables.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._variables
					Exit Select
				Case 2
					syntaxNode = Me._additionalQueryOperators
					Exit Select
				Case 3
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 4
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
					red = MyBase.GetRed(Me._variables, 1)
					Exit Select
				Case 2
					red = MyBase.GetRed(Me._additionalQueryOperators, 2)
					Exit Select
				Case 3
				Label0:
					red = Nothing
					Exit Select
				Case 4
					red = MyBase.GetRed(Me._aggregationVariables, 4)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return red
		End Function

		Public Function Update(ByVal aggregateKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax), ByVal additionalQueryOperators As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax), ByVal intoKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal aggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Dim aggregateClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			If (aggregateKeyword <> Me.AggregateKeyword OrElse variables <> Me.Variables OrElse additionalQueryOperators <> Me.AdditionalQueryOperators OrElse intoKeyword <> Me.IntoKeyword OrElse aggregationVariables <> Me.AggregationVariables) Then
				Dim aggregateClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AggregateClause(aggregateKeyword, variables, additionalQueryOperators, intoKeyword, aggregationVariables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				aggregateClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, aggregateClauseSyntax1, aggregateClauseSyntax1.WithAnnotations(annotations))
			Else
				aggregateClauseSyntax = Me
			End If
			Return aggregateClauseSyntax
		End Function

		Public Function WithAdditionalQueryOperators(ByVal additionalQueryOperators As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.Update(Me.AggregateKeyword, Me.Variables, additionalQueryOperators, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithAggregateKeyword(ByVal aggregateKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.Update(aggregateKeyword, Me.Variables, Me.AdditionalQueryOperators, Me.IntoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithAggregationVariables(ByVal aggregationVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.Update(Me.AggregateKeyword, Me.Variables, Me.AdditionalQueryOperators, Me.IntoKeyword, aggregationVariables)
		End Function

		Public Function WithIntoKeyword(ByVal intoKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.Update(Me.AggregateKeyword, Me.Variables, Me.AdditionalQueryOperators, intoKeyword, Me.AggregationVariables)
		End Function

		Public Function WithVariables(ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax
			Return Me.Update(Me.AggregateKeyword, variables, Me.AdditionalQueryOperators, Me.IntoKeyword, Me.AggregationVariables)
		End Function
	End Class
End Namespace