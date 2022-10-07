Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class VisualBasicSyntaxRewriter
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function VisitAccessorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim accessorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax = DirectCast(Me.Visit(node._accessorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax)
			If (node._accessorStatement <> accessorStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endAccessorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endAccessorStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), accessorStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAccessorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AccessorKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._accessorKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, parameterListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAddRemoveHandlerStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AddHandlerOrRemoveHandlerKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._addHandlerOrRemoveHandlerKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._eventExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._eventExpression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._commaToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._delegateExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._delegateExpression <> expressionSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, punctuationSyntax, expressionSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAggregateClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AggregateKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._aggregateKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(node.Variables)
			If (node._variables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)(node.AdditionalQueryOperators)
			If (node._additionalQueryOperators <> syntaxList.Node) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IntoKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._intoKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(node.AggregationVariables)
			If (node._aggregationVariables <> separatedSyntaxList1.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax1, separatedSyntaxList1.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAggregationRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax = DirectCast(Me.Visit(node._nameEquals), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)
			If (node._nameEquals <> variableNameEqualsSyntax) Then
				flag = True
			End If
			Dim aggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax = DirectCast(Me.Visit(node._aggregation), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax)
			If (node._aggregation <> aggregationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), variableNameEqualsSyntax, aggregationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAnonymousObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NewKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._newKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim objectMemberInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax = DirectCast(Me.Visit(node._initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)
			If (node._initializer <> objectMemberInitializerSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, objectMemberInitializerSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitArgumentList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)(node.Arguments)
			If (node._arguments <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitArrayCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NewKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._newKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._arrayBounds), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._arrayBounds <> argumentListSyntax) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)(node.RankSpecifiers)
			If (node._rankSpecifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax = DirectCast(Me.Visit(node._initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)
			If (node._initializer <> collectionInitializerSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax, argumentListSyntax, syntaxList1.Node, collectionInitializerSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitArrayRankSpecifier(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(node.CommaTokens)
			If (node._commaTokens <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitArrayType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._elementType), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._elementType <> typeSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)(node.RankSpecifiers)
			If (node._rankSpecifiers <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAsNewClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._asKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim newExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax = DirectCast(Me.Visit(node._newExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax)
			If (node._newExpression <> newExpressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, newExpressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAssignmentStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._left), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._left <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._operatorToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._right), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._right <> expressionSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim attributeTargetSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax = DirectCast(Me.Visit(node._target), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)
			If (node._target <> attributeTargetSyntax) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._name <> typeSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._argumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._argumentList <> argumentListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), attributeTargetSyntax, typeSyntax, argumentListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAttributeList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax)(node.Attributes)
			If (node._attributes <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.GreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._greaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAttributesStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAttributeTarget(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AttributeModifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._attributeModifier <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.ColonToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._colonToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitAwaitExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AwaitKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._awaitKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitBadDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitBinaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ifKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._firstExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._firstExpression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._commaToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._secondExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._secondExpression <> expressionSyntax1) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, expressionSyntax1, punctuationSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitBinaryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._left), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._left <> expressionSyntax) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node._operatorToken <> syntaxToken) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._right), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._right <> expressionSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, syntaxToken, expressionSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCallStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.CallKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._callKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._invocation), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._invocation <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCaseBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax = DirectCast(Me.Visit(node._caseStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)
			If (node._caseStatement <> caseStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), caseStatementSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCaseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.CaseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._caseKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax)(node.Cases)
			If (node._cases <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim catchStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax = DirectCast(Me.Visit(node._catchStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax)
			If (node._catchStatement <> catchStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), catchStatementSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCatchFilterClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WhenKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._whenKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._filter), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._filter <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCatchStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.CatchKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._catchKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(Me.Visit(node._identifierName), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (node._identifierName <> identifierNameSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim catchFilterClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax = DirectCast(Me.Visit(node._whenClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax)
			If (node._whenClause <> catchFilterClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, identifierNameSyntax, simpleAsClauseSyntax, catchFilterClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitClassBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim classStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax = DirectCast(Me.Visit(node._classStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax)
			If (node._classStatement <> classStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> syntaxList1.Node) Then
				flag = True
			End If
			Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Members)
			If (node._members <> syntaxList2.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endClassStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endClassStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), classStatementSyntax, syntaxList.Node, syntaxList1.Node, syntaxList2.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitClassStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ClassKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._classKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(Me.Visit(node._typeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (node._typeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCollectionInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openBraceToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(node.Initializers)
			If (node._initializers <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeBraceToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCollectionRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = DirectCast(Me.Visit(node._identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)
			If (node._identifier <> modifiedIdentifierSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.InKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._inKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCompilationUnit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)(node.Options)
			If (node._options <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)(node.[Imports])
			If (node._imports <> syntaxList1.Node) Then
				flag = True
			End If
			Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)(node.Attributes)
			If (node._attributes <> syntaxList2.Node) Then
				flag = True
			End If
			Dim syntaxList3 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Members)
			If (node._members <> syntaxList3.Node) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EndOfFileToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._endOfFileToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, syntaxList2.Node, syntaxList3.Node, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitConditionalAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.QuestionMarkToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._questionMarkToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._whenNotNull), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._whenNotNull <> expressionSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitConstDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ConstKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._constKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._name <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._value), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._value <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, identifierTokenSyntax, punctuationSyntax1, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitConstructorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim subNewStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax = DirectCast(Me.Visit(node._subNewStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)
			If (node._subNewStatement <> subNewStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endSubStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endSubStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), subNewStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitContinueStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ContinueKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._continueKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.BlockKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._blockKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCrefOperatorReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OperatorKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._operatorKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node._operatorToken <> syntaxToken) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCrefReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._name <> typeSyntax) Then
				flag = True
			End If
			Dim crefSignatureSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax = DirectCast(Me.Visit(node._signature), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)
			If (node._signature <> crefSignatureSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax, crefSignatureSyntax, simpleAsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCrefSignature(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax)(node.ArgumentTypes)
			If (node._argumentTypes <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCrefSignaturePart(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Modifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._modifier <> keywordSyntax) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, typeSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitCTypeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._commaToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDeclareStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.DeclareKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._declareKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.CharsetKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._charsetKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SubOrFunctionKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._subOrFunctionKeyword <> keywordSyntax2) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.LibKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._libKeyword <> keywordSyntax3) Then
				flag = True
			End If
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = DirectCast(Me.Visit(node._libraryName), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			If (node._libraryName <> literalExpressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AliasKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._aliasKeyword <> keywordSyntax4) Then
				flag = True
			End If
			Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = DirectCast(Me.Visit(node._aliasName), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			If (node._aliasName <> literalExpressionSyntax1) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, keywordSyntax1, keywordSyntax2, identifierTokenSyntax, keywordSyntax3, literalExpressionSyntax, keywordSyntax4, literalExpressionSyntax1, parameterListSyntax, simpleAsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDelegateStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.DelegateKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._delegateKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SubOrFunctionKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._subOrFunctionKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(Me.Visit(node._typeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (node._typeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, keywordSyntax1, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDirectCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._commaToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDisableWarningDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.DisableKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._disableKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WarningKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._warningKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(node.ErrorCodes)
			If (node._errorCodes <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDistinctClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.DistinctKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._distinctKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDocumentationCommentTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.Content)
			If (node._content <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDoLoopBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = DirectCast(Me.Visit(node._doStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			If (node._doStatement <> doStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = DirectCast(Me.Visit(node._loopStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			If (node._loopStatement <> loopStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), doStatementSyntax, syntaxList.Node, loopStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitDoStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.DoKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._doKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = DirectCast(Me.Visit(node._whileOrUntilClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			If (node._whileOrUntilClause <> whileOrUntilClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitElseBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim elseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax = DirectCast(Me.Visit(node._elseStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax)
			If (node._elseStatement <> elseStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), elseStatementSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitElseCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ElseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._elseKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitElseDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ElseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._elseKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitElseIfBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim elseIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax = DirectCast(Me.Visit(node._elseIfStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax)
			If (node._elseIfStatement <> elseIfStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), elseIfStatementSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitElseIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ElseIfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._elseIfKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ThenKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._thenKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitElseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ElseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._elseKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEmptyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Empty), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._empty <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEnableWarningDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EnableKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._enableKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WarningKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._warningKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(node.ErrorCodes)
			If (node._errorCodes <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEndBlockStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._endKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.BlockKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._blockKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEndExternalSourceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._endKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ExternalSourceKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._externalSourceKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEndIfDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._endKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ifKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEndRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._endKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.RegionKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._regionKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEnumBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim enumStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax = DirectCast(Me.Visit(node._enumStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)
			If (node._enumStatement <> enumStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Members)
			If (node._members <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endEnumStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endEnumStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), enumStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEnumMemberDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = DirectCast(Me.Visit(node._initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			If (node._initializer <> equalsValueSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, identifierTokenSyntax, equalsValueSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEnumStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EnumKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._enumKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = DirectCast(Me.Visit(node._underlyingType), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)
			If (node._underlyingType <> asClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, identifierTokenSyntax, asClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEqualsValue(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._value), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._value <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEraseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EraseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._eraseKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(node.Expressions)
			If (node._expressions <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitErrorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ErrorKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._errorKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._errorNumber), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._errorNumber <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEventBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax = DirectCast(Me.Visit(node._eventStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)
			If (node._eventStatement <> eventStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)(node.Accessors)
			If (node._accessors <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endEventStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endEventStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), eventStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitEventStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.CustomKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._customKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EventKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._eventKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = DirectCast(Me.Visit(node._implementsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			If (node._implementsClause <> implementsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, keywordSyntax1, identifierTokenSyntax, parameterListSyntax, simpleAsClauseSyntax, implementsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitExitStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ExitKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._exitKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.BlockKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._blockKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitExpressionRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax = DirectCast(Me.Visit(node._nameEquals), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)
			If (node._nameEquals <> variableNameEqualsSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), variableNameEqualsSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitExpressionStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitExternalChecksumDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ExternalChecksumKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._externalChecksumKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(Me.Visit(node.ExternalSource), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node._externalSource <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.FirstCommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._firstCommaToken <> punctuationSyntax2) Then
				flag = True
			End If
			Dim stringLiteralTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(Me.Visit(node.Guid), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node._guid <> stringLiteralTokenSyntax1) Then
				flag = True
			End If
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.SecondCommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._secondCommaToken <> punctuationSyntax3) Then
				flag = True
			End If
			Dim stringLiteralTokenSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(Me.Visit(node.Checksum), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node._checksum <> stringLiteralTokenSyntax2) Then
				flag = True
			End If
			Dim punctuationSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax4) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax1, stringLiteralTokenSyntax, punctuationSyntax2, stringLiteralTokenSyntax1, punctuationSyntax3, stringLiteralTokenSyntax2, punctuationSyntax4))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitExternalSourceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ExternalSourceKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._externalSourceKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(Me.Visit(node.ExternalSource), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node._externalSource <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._commaToken <> punctuationSyntax2) Then
				flag = True
			End If
			Dim integerLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax = DirectCast(Me.Visit(node.LineStart), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)
			If (node._lineStart <> integerLiteralTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax3) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax1, stringLiteralTokenSyntax, punctuationSyntax2, integerLiteralTokenSyntax, punctuationSyntax3))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitFieldDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = Me.VisitList(Of KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(node.Declarators)
			If (node._declarators <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitFinallyBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim finallyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax = DirectCast(Me.Visit(node._finallyStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax)
			If (node._finallyStatement <> finallyStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), finallyStatementSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitFinallyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.FinallyKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._finallyKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitForBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim forStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax = DirectCast(Me.Visit(node._forStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)
			If (node._forStatement <> forStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax = DirectCast(Me.Visit(node._nextStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			If (node._nextStatement <> nextStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), forStatementSyntax, syntaxList.Node, nextStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitForEachBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim forEachStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax = DirectCast(Me.Visit(node._forEachStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)
			If (node._forEachStatement <> forEachStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax = DirectCast(Me.Visit(node._nextStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			If (node._nextStatement <> nextStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), forEachStatementSyntax, syntaxList.Node, nextStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ForKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._forKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EachKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._eachKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim visualBasicSyntaxNode1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.Visit(node._controlVariable)
			If (node._controlVariable <> visualBasicSyntaxNode1) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.InKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._inKeyword <> keywordSyntax2) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, visualBasicSyntaxNode1, keywordSyntax2, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitForStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ForKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._forKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim visualBasicSyntaxNode1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.Visit(node._controlVariable)
			If (node._controlVariable <> visualBasicSyntaxNode1) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._fromValue), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._fromValue <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ToKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._toKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._toValue), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._toValue <> expressionSyntax1) Then
				flag = True
			End If
			Dim forStepClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax = DirectCast(Me.Visit(node._stepClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)
			If (node._stepClause <> forStepClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, visualBasicSyntaxNode1, punctuationSyntax, expressionSyntax, keywordSyntax1, expressionSyntax1, forStepClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitForStepClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.StepKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._stepKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._stepValue), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._stepValue <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitFromClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.FromKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._fromKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(node.Variables)
			If (node._variables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitFunctionAggregation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.FunctionName), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._functionName <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._argument), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._argument <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGenericName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeArgumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax = DirectCast(Me.Visit(node._typeArgumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)
			If (node._typeArgumentList <> typeArgumentListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, typeArgumentListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGetTypeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GetTypeKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._getTypeKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, typeSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGetXmlNamespaceExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GetXmlNamespaceKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._getXmlNamespaceKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlPrefixNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax)
			If (node._name <> xmlPrefixNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, xmlPrefixNameSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGlobalName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GlobalKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._globalKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGoToStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GoToKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._goToKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = DirectCast(Me.Visit(node._label), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			If (node._label <> labelSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, labelSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGroupAggregation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GroupKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._groupKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGroupByClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GroupKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._groupKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(node.Items)
			If (node._items <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ByKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._byKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(node.Keys)
			If (node._keys <> separatedSyntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IntoKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._intoKeyword <> keywordSyntax2) Then
				flag = True
			End If
			Dim separatedSyntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(node.AggregationVariables)
			If (node._aggregationVariables <> separatedSyntaxList2.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, keywordSyntax1, separatedSyntaxList1.Node, keywordSyntax2, separatedSyntaxList2.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitGroupJoinClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GroupKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._groupKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.JoinKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._joinKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(node.JoinedVariables)
			If (node._joinedVariables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)(node.AdditionalJoins)
			If (node._additionalJoins <> syntaxList.Node) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OnKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._onKeyword <> keywordSyntax2) Then
				flag = True
			End If
			Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(node.JoinConditions)
			If (node._joinConditions <> separatedSyntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IntoKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._intoKeyword <> keywordSyntax3) Then
				flag = True
			End If
			Dim separatedSyntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(node.AggregationVariables)
			If (node._aggregationVariables <> separatedSyntaxList2.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax2, separatedSyntaxList1.Node, keywordSyntax3, separatedSyntaxList2.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitHandlesClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.HandlesKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._handlesKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)(node.Events)
			If (node._events <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitHandlesClauseItem(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim eventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax = DirectCast(Me.Visit(node._eventContainer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax)
			If (node._eventContainer <> eventContainerSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.DotToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._dotToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(Me.Visit(node._eventMember), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (node._eventMember <> identifierNameSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), eventContainerSyntax, punctuationSyntax, identifierNameSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitIdentifierName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitIfDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ElseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._elseKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IfOrElseIfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ifOrElseIfKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ThenKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._thenKeyword <> keywordSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1, expressionSyntax, keywordSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ifKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ThenKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._thenKeyword <> keywordSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitImplementsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ImplementsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._implementsKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)(node.InterfaceMembers)
			If (node._interfaceMembers <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitImplementsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ImplementsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._implementsKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(node.Types)
			If (node._types <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitImportAliasClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitImportsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ImportsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._importsKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax)(node.ImportsClauses)
			If (node._importsClauses <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitIncompleteMember(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = Me.VisitList(Of KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.MissingIdentifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._missingIdentifier <> identifierTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, identifierTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInferredFieldInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.KeyKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInheritsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.InheritsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._inheritsKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(node.Types)
			If (node._types <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInterfaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim interfaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax = DirectCast(Me.Visit(node._interfaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax)
			If (node._interfaceStatement <> interfaceStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> syntaxList1.Node) Then
				flag = True
			End If
			Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Members)
			If (node._members <> syntaxList2.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endInterfaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endInterfaceStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), interfaceStatementSyntax, syntaxList.Node, syntaxList1.Node, syntaxList2.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInterfaceStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.InterfaceKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._interfaceKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(Me.Visit(node._typeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (node._typeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInterpolatedStringExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.DollarSignDoubleQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._dollarSignDoubleQuoteToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax)(node.Contents)
			If (node._contents <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.DoubleQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._doubleQuoteToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInterpolatedStringText(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim interpolatedStringTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax = DirectCast(Me.Visit(node.TextToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
			If (node._textToken <> interpolatedStringTextTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), interpolatedStringTextTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInterpolation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openBraceToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim interpolationAlignmentClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax = DirectCast(Me.Visit(node._alignmentClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)
			If (node._alignmentClause <> interpolationAlignmentClauseSyntax) Then
				flag = True
			End If
			Dim interpolationFormatClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax = DirectCast(Me.Visit(node._formatClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)
			If (node._formatClause <> interpolationFormatClauseSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeBraceToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax, interpolationAlignmentClauseSyntax, interpolationFormatClauseSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInterpolationAlignmentClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._commaToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._value), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._value <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInterpolationFormatClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.ColonToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._colonToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim interpolatedStringTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax = DirectCast(Me.Visit(node.FormatStringToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
			If (node._formatStringToken <> interpolatedStringTextTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, interpolatedStringTextTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitInvocationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._argumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._argumentList <> argumentListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, argumentListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitJoinCondition(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._left), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._left <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.EqualsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._equalsKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._right), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._right <> expressionSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitKeywordEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitLabel(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.Visit(node.LabelToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node._labelToken <> syntaxToken) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.Visit(node.LabelToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node._labelToken <> syntaxToken) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.ColonToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._colonToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitLambdaHeader(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SubOrFunctionKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._subOrFunctionKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, parameterListSyntax, simpleAsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitLetClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.LetKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._letKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(node.Variables)
			If (node._variables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Function VisitList(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of TNode)) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of TNode)
			Dim syntaxListBuilder As SyntaxListBuilder(Of TNode) = New SyntaxListBuilder(Of TNode)()
			Dim num As Integer = 0
			Dim count As Integer = list.Count
			While num < count
				Dim item As TNode = list(num)
				Dim tNode1 As TNode = DirectCast(Me.Visit(DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)), TNode)
				If (item <> tNode1 AndAlso syntaxListBuilder.IsNull) Then
					syntaxListBuilder = New SyntaxListBuilder(Of TNode)(count)
					syntaxListBuilder.AddRange(list, 0, num)
				End If
				If (Not syntaxListBuilder.IsNull AndAlso tNode1 IsNot Nothing AndAlso tNode1.Kind <> SyntaxKind.None) Then
					syntaxListBuilder.Add(tNode1)
				End If
				num = num + 1
			End While
			Return If(syntaxListBuilder.IsNull, list, syntaxListBuilder.ToList())
		End Function

		Public Function VisitList(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of TNode)) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of TNode)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of TNode) = New SeparatedSyntaxListBuilder(Of TNode)()
			Dim num As Integer = 0
			Dim count As Integer = list.Count
			Dim separatorCount As Integer = list.SeparatorCount
			While num < count
				Dim item As TNode = list(num)
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.Visit(DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
				Dim separator As Microsoft.CodeAnalysis.GreenNode = Nothing
				Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Nothing
				If (num < separatorCount) Then
					separator = list.GetSeparator(num)
					greenNode = DirectCast(Me.Visit(DirectCast(separator, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				End If
				If ((item <> visualBasicSyntaxNode OrElse separator <> greenNode) AndAlso separatedSyntaxListBuilder.IsNull) Then
					separatedSyntaxListBuilder = New SeparatedSyntaxListBuilder(Of TNode)(count)
					separatedSyntaxListBuilder.AddRange(list, num)
				End If
				If (Not separatedSyntaxListBuilder.IsNull) Then
					If (visualBasicSyntaxNode IsNot Nothing AndAlso visualBasicSyntaxNode.Kind <> SyntaxKind.None) Then
						separatedSyntaxListBuilder.Add(DirectCast(visualBasicSyntaxNode, TNode))
						If (greenNode IsNot Nothing) Then
							separatedSyntaxListBuilder.AddSeparator(greenNode)
						End If
					ElseIf (num >= separatorCount AndAlso separatedSyntaxListBuilder.Count > 0) Then
						separatedSyntaxListBuilder.RemoveLast()
					End If
				End If
				num = num + 1
			End While
			Return If(separatedSyntaxListBuilder.IsNull, list, separatedSyntaxListBuilder.ToList())
		End Function

		Public Overrides Function VisitLiteralExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.Visit(node.Token), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node._token <> syntaxToken) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitLocalDeclarationStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = Me.VisitList(Of KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList.Node) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(node.Declarators)
			If (node._declarators <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitLoopStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.LoopKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._loopKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = DirectCast(Me.Visit(node._whileOrUntilClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			If (node._whileOrUntilClause <> whileOrUntilClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMemberAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._operatorToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)
			If (node._name <> simpleNameSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, simpleNameSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMethodBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim methodStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax = DirectCast(Me.Visit(node._subOrFunctionStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax)
			If (node._subOrFunctionStatement <> methodStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endSubOrFunctionStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endSubOrFunctionStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), methodStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMethodStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SubOrFunctionKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._subOrFunctionKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(Me.Visit(node._typeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (node._typeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax = DirectCast(Me.Visit(node._handlesClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)
			If (node._handlesClause <> handlesClauseSyntax) Then
				flag = True
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = DirectCast(Me.Visit(node._implementsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			If (node._implementsClause <> implementsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax, handlesClauseSyntax, implementsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMidExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Mid), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._mid <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._argumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._argumentList <> argumentListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, argumentListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitModifiedIdentifier(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Nullable), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._nullable <> punctuationSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._arrayBounds), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._arrayBounds <> argumentListSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)(node.ArrayRankSpecifiers)
			If (node._arrayRankSpecifiers <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, argumentListSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitModuleBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim moduleStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax = DirectCast(Me.Visit(node._moduleStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax)
			If (node._moduleStatement <> moduleStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> syntaxList1.Node) Then
				flag = True
			End If
			Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Members)
			If (node._members <> syntaxList2.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endModuleStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endModuleStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), moduleStatementSyntax, syntaxList.Node, syntaxList1.Node, syntaxList2.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitModuleStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ModuleKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._moduleKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(Me.Visit(node._typeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (node._typeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMultiLineIfBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = DirectCast(Me.Visit(node._ifStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
			If (node._ifStatement <> ifStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax)(node.ElseIfBlocks)
			If (node._elseIfBlocks <> syntaxList1.Node) Then
				flag = True
			End If
			Dim elseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax = DirectCast(Me.Visit(node._elseBlock), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax)
			If (node._elseBlock <> elseBlockSyntax) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endIfStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endIfStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), ifStatementSyntax, syntaxList.Node, syntaxList1.Node, elseBlockSyntax, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMultiLineLambdaExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim lambdaHeaderSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax = DirectCast(Me.Visit(node._subOrFunctionHeader), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)
			If (node._subOrFunctionHeader <> lambdaHeaderSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endSubOrFunctionStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endSubOrFunctionStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), lambdaHeaderSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMyBaseExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitMyClassExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNameColonEquals(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (node._name <> identifierNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.ColonEqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._colonEqualsToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierNameSyntax, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNamedFieldInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.KeyKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.DotToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._dotToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (node._name <> identifierNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, identifierNameSyntax, punctuationSyntax1, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNamedTupleElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax, simpleAsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNameOfExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NameOfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._nameOfKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._argument), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._argument <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNamespaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim namespaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax = DirectCast(Me.Visit(node._namespaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax)
			If (node._namespaceStatement <> namespaceStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Members)
			If (node._members <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endNamespaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endNamespaceStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), namespaceStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNamespaceStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NamespaceKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._namespaceKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)
			If (node._name <> nameSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, nameSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNextStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NextKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._nextKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(node.ControlVariables)
			If (node._controlVariables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitNullableType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._elementType), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._elementType <> typeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.QuestionMarkToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._questionMarkToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitObjectCollectionInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.FromKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._fromKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax = DirectCast(Me.Visit(node._initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)
			If (node._initializer <> collectionInitializerSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, collectionInitializerSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NewKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._newKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._argumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._argumentList <> argumentListSyntax) Then
				flag = True
			End If
			Dim objectCreationInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax = DirectCast(Me.Visit(node._initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax)
			If (node._initializer <> objectCreationInitializerSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax, argumentListSyntax, objectCreationInitializerSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitObjectMemberInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WithKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._withKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openBraceToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)(node.Initializers)
			If (node._initializers <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeBraceToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOmittedArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Empty), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._empty <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOnErrorGoToStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OnKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._onKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ErrorKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._errorKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.GoToKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._goToKeyword <> keywordSyntax2) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Minus), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._minus <> punctuationSyntax) Then
				flag = True
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = DirectCast(Me.Visit(node._label), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			If (node._label <> labelSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, keywordSyntax2, punctuationSyntax, labelSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOnErrorResumeNextStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OnKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._onKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ErrorKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._errorKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ResumeKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._resumeKeyword <> keywordSyntax2) Then
				flag = True
			End If
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NextKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._nextKeyword <> keywordSyntax3) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, keywordSyntax2, keywordSyntax3))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOperatorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim operatorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax = DirectCast(Me.Visit(node._operatorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax)
			If (node._operatorStatement <> operatorStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endOperatorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endOperatorStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), operatorStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOperatorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OperatorKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._operatorKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node._operatorToken <> syntaxToken) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, syntaxToken, parameterListSyntax, simpleAsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOptionStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OptionKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._optionKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NameKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._nameKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ValueKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._valueKeyword <> keywordSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, keywordSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOrderByClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OrderKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._orderKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ByKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._byKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)(node.Orderings)
			If (node._orderings <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitOrdering(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AscendingOrDescendingKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ascendingOrDescendingKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitParameter(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = Me.VisitList(Of KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = DirectCast(Me.Visit(node._identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)
			If (node._identifier <> modifiedIdentifierSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = DirectCast(Me.Visit(node._default), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			If (node._default <> equalsValueSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, modifiedIdentifierSyntax, simpleAsClauseSyntax, equalsValueSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitParameterList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)(node.Parameters)
			If (node._parameters <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitParenthesizedExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitPartitionClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SkipOrTakeKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._skipOrTakeKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._count), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._count <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitPartitionWhileClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SkipOrTakeKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._skipOrTakeKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WhileKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._whileKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitPredefinedCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitPredefinedType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitPrintStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.QuestionToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._questionToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitPropertyBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax = DirectCast(Me.Visit(node._propertyStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)
			If (node._propertyStatement <> propertyStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)(node.Accessors)
			If (node._accessors <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endPropertyStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endPropertyStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), propertyStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitPropertyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.PropertyKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._propertyKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)
			If (node._asClause <> asClauseSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = DirectCast(Me.Visit(node._initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			If (node._initializer <> equalsValueSyntax) Then
				flag = True
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = DirectCast(Me.Visit(node._implementsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			If (node._implementsClause <> implementsClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, identifierTokenSyntax, parameterListSyntax, asClauseSyntax, equalsValueSyntax, implementsClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitQualifiedCrefOperatorReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = DirectCast(Me.Visit(node._left), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)
			If (node._left <> nameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.DotToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._dotToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim crefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax = DirectCast(Me.Visit(node._right), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)
			If (node._right <> crefOperatorReferenceSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), nameSyntax, punctuationSyntax, crefOperatorReferenceSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitQualifiedName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = DirectCast(Me.Visit(node._left), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)
			If (node._left <> nameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.DotToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._dotToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax = DirectCast(Me.Visit(node._right), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)
			If (node._right <> simpleNameSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), nameSyntax, punctuationSyntax, simpleNameSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)(node.Clauses)
			If (node._clauses <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitRaiseEventStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.RaiseEventKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._raiseEventKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (node._name <> identifierNameSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._argumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._argumentList <> argumentListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, identifierNameSyntax, argumentListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitRangeArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._lowerBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._lowerBound <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ToKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._toKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._upperBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._upperBound <> expressionSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitRangeCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._lowerBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._lowerBound <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ToKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._toKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._upperBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._upperBound <> expressionSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(Me.Visit(node._arrayBounds), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (node._arrayBounds <> argumentListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, argumentListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitReDimStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ReDimKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._reDimKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.PreserveKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._preserveKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)(node.Clauses)
			If (node._clauses <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitReferenceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ReferenceKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._referenceKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(Me.Visit(node.File), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node._file <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.HashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._hashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.RegionKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._regionKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node._name <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitRelationalCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._isKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._operatorToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._value), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._value <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitResumeStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ResumeKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._resumeKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = DirectCast(Me.Visit(node._label), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			If (node._label <> labelSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, labelSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ReturnKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._returnKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSelectBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim selectStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax = DirectCast(Me.Visit(node._selectStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax)
			If (node._selectStatement <> selectStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)(node.CaseBlocks)
			If (node._caseBlocks <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endSelectStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endSelectStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), selectStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSelectClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SelectKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._selectKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(node.Variables)
			If (node._variables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SelectKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._selectKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.CaseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._caseKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, keywordSyntax1, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSimpleArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim nameColonEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax = DirectCast(Me.Visit(node._nameColonEquals), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax)
			If (node._nameColonEquals <> nameColonEqualsSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), nameColonEqualsSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSimpleAsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._asKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node, typeSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSimpleCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._value), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._value <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSimpleImportsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim importAliasClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax = DirectCast(Me.Visit(node._alias), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)
			If (node._alias <> importAliasClauseSyntax) Then
				flag = True
			End If
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)
			If (node._name <> nameSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), importAliasClauseSyntax, nameSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSimpleJoinClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.JoinKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._joinKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(node.JoinedVariables)
			If (node._joinedVariables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)(node.AdditionalJoins)
			If (node._additionalJoins <> syntaxList.Node) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OnKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._onKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(node.JoinConditions)
			If (node._joinConditions <> separatedSyntaxList1.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, separatedSyntaxList.Node, syntaxList.Node, keywordSyntax1, separatedSyntaxList1.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSingleLineElseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ElseKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._elseKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSingleLineIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ifKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ThenKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._thenKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim singleLineElseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax = DirectCast(Me.Visit(node._elseClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax)
			If (node._elseClause <> singleLineElseClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1, syntaxList.Node, singleLineElseClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSingleLineLambdaExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim lambdaHeaderSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax = DirectCast(Me.Visit(node._subOrFunctionHeader), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)
			If (node._subOrFunctionHeader <> lambdaHeaderSyntax) Then
				flag = True
			End If
			Dim visualBasicSyntaxNode1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.Visit(node._body)
			If (node._body <> visualBasicSyntaxNode1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), lambdaHeaderSyntax, visualBasicSyntaxNode1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSkippedTokensTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(node.Tokens)
			If (node._tokens <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSpecialConstraint(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ConstraintKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._constraintKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitStopOrEndStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.StopOrEndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._stopOrEndKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitStructureBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim structureStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax = DirectCast(Me.Visit(node._structureStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)
			If (node._structureStatement <> structureStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> syntaxList1.Node) Then
				flag = True
			End If
			Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Members)
			If (node._members <> syntaxList2.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endStructureStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endStructureStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), structureStatementSyntax, syntaxList.Node, syntaxList1.Node, syntaxList2.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitStructureStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.StructureKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._structureKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(Me.Visit(node._typeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (node._typeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSubNewStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(node.Modifiers)
			If (node._modifiers <> syntaxList1.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SubKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._subKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.NewKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._newKeyword <> keywordSyntax1) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = DirectCast(Me.Visit(node._parameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			If (node._parameterList <> parameterListSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node, syntaxList1.Node, keywordSyntax, keywordSyntax1, parameterListSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSyncLockBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syncLockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax = DirectCast(Me.Visit(node._syncLockStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax)
			If (node._syncLockStatement <> syncLockStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endSyncLockStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endSyncLockStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syncLockStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.SyncLockKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._syncLockKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTernaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.IfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ifKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.FirstCommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._firstCommaToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._whenTrue), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._whenTrue <> expressionSyntax1) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.SecondCommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._secondCommaToken <> punctuationSyntax2) Then
				flag = True
			End If
			Dim expressionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._whenFalse), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._whenFalse <> expressionSyntax2) Then
				flag = True
			End If
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax3) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, expressionSyntax1, punctuationSyntax2, expressionSyntax2, punctuationSyntax3))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitThrowStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.ThrowKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._throwKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTryBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim tryStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax = DirectCast(Me.Visit(node._tryStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)
			If (node._tryStatement <> tryStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax)(node.CatchBlocks)
			If (node._catchBlocks <> syntaxList1.Node) Then
				flag = True
			End If
			Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax = DirectCast(Me.Visit(node._finallyBlock), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax)
			If (node._finallyBlock <> finallyBlockSyntax) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endTryStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endTryStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), tryStatementSyntax, syntaxList.Node, syntaxList1.Node, finallyBlockSyntax, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTryCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.Keyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._keyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._commaToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.TryKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._tryKeyword <> keywordSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTupleExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)(node.Arguments)
			If (node._arguments <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTupleType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax)(node.Elements)
			If (node._elements <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypeArgumentList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ofKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(node.Arguments)
			If (node._arguments <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypeConstraint(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypedTupleElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), typeSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypeOfExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.TypeOfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._typeOfKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._operatorToken <> keywordSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(Me.Visit(node._type), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (node._type <> typeSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1, typeSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypeParameter(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.VarianceKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._varianceKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax = DirectCast(Me.Visit(node._typeParameterConstraintClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)
			If (node._typeParameterConstraintClause <> typeParameterConstraintClauseSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, identifierTokenSyntax, typeParameterConstraintClauseSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypeParameterList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openParenToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.OfKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._ofKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)(node.Parameters)
			If (node._parameters <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeParenToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypeParameterMultipleConstraintClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._asKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.OpenBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._openBraceToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)(node.Constraints)
			If (node._constraints <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.CloseBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._closeBraceToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, punctuationSyntax, separatedSyntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitTypeParameterSingleConstraintClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.AsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._asKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim constraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax = DirectCast(Me.Visit(node._constraint), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)
			If (node._constraint <> constraintSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, constraintSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitUnaryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.Visit(node.OperatorToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node._operatorToken <> syntaxToken) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._operand), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._operand <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxToken, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitUsingBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim usingStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax = DirectCast(Me.Visit(node._usingStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax)
			If (node._usingStatement <> usingStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endUsingStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endUsingStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), usingStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.UsingKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._usingKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(node.Variables)
			If (node._variables <> separatedSyntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax, separatedSyntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitVariableDeclarator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(node.Names)
			If (node._names <> separatedSyntaxList.Node) Then
				flag = True
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)
			If (node._asClause <> asClauseSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = DirectCast(Me.Visit(node._initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			If (node._initializer <> equalsValueSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), separatedSyntaxList.Node, asClauseSyntax, equalsValueSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitVariableNameEquals(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = DirectCast(Me.Visit(node._identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)
			If (node._identifier <> modifiedIdentifierSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node._asClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (node._asClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWhereClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WhereKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._whereKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWhileBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whileStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax = DirectCast(Me.Visit(node._whileStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax)
			If (node._whileStatement <> whileStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endWhileStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endWhileStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), whileStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWhileOrUntilClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WhileOrUntilKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._whileOrUntilKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WhileKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._whileKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._condition <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWithBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim withStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax = DirectCast(Me.Visit(node._withStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax)
			If (node._withStatement <> withStatementSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(node.Statements)
			If (node._statements <> syntaxList.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node._endWithStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (node._endWithStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), withStatementSyntax, syntaxList.Node, endBlockStatementSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWithEventsEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node._identifier <> identifierTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), identifierTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWithEventsPropertyEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim withEventsEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax = DirectCast(Me.Visit(node._withEventsContainer), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax)
			If (node._withEventsContainer <> withEventsEventContainerSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.DotToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._dotToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(Me.Visit(node._property), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (node._property <> identifierNameSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), withEventsEventContainerSyntax, punctuationSyntax, identifierNameSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.WithKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._withKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node._name <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node._value), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node._value <> xmlNodeSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNodeSyntax, punctuationSyntax, xmlNodeSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlBracketedName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (node._name <> xmlNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.GreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._greaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlCDataSection(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.BeginCDataToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._beginCDataToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = Me.VisitList(Of XmlTextTokenSyntax)(node.TextTokens)
			If (node._textTokens <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EndCDataToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._endCDataToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlComment(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanExclamationMinusMinusToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanExclamationMinusMinusToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = Me.VisitList(Of XmlTextTokenSyntax)(node.TextTokens)
			If (node._textTokens <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.MinusMinusGreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._minusMinusGreaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlCrefAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (node._name <> xmlNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.StartQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._startQuoteToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax = DirectCast(Me.Visit(node._reference), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax)
			If (node._reference <> crefReferenceSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EndQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._endQuoteToken <> punctuationSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax1, crefReferenceSyntax, punctuationSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanQuestionToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanQuestionToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.XmlKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._xmlKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim xmlDeclarationOptionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = DirectCast(Me.Visit(node._version), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			If (node._version <> xmlDeclarationOptionSyntax) Then
				flag = True
			End If
			Dim xmlDeclarationOptionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = DirectCast(Me.Visit(node._encoding), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			If (node._encoding <> xmlDeclarationOptionSyntax1) Then
				flag = True
			End If
			Dim xmlDeclarationOptionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = DirectCast(Me.Visit(node._standalone), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			If (node._standalone <> xmlDeclarationOptionSyntax2) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.QuestionGreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._questionGreaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, xmlDeclarationOptionSyntax, xmlDeclarationOptionSyntax1, xmlDeclarationOptionSyntax2, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlDeclarationOption(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node._name <> xmlNameTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Equals), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equals <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax = DirectCast(Me.Visit(node._value), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)
			If (node._value <> xmlStringSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax, xmlStringSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlDocument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax = DirectCast(Me.Visit(node._declaration), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)
			If (node._declaration <> xmlDeclarationSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.PrecedingMisc)
			If (node._precedingMisc <> syntaxList.Node) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node._root), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node._root <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.FollowingMisc)
			If (node._followingMisc <> syntaxList1.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlDeclarationSyntax, syntaxList.Node, xmlNodeSyntax, syntaxList1.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlElementStartTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax = DirectCast(Me.Visit(node._startTag), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)
			If (node._startTag <> xmlElementStartTagSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.Content)
			If (node._content <> syntaxList.Node) Then
				flag = True
			End If
			Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax = DirectCast(Me.Visit(node._endTag), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)
			If (node._endTag <> xmlElementEndTagSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlElementStartTagSyntax, syntaxList.Node, xmlElementEndTagSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlElementEndTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanSlashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanSlashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (node._name <> xmlNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.GreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._greaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlElementStartTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node._name <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.Attributes)
			If (node._attributes <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.GreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._greaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlEmbeddedExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanPercentEqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanPercentEqualsToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.PercentGreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._percentGreaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlEmptyElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node._name <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.Attributes)
			If (node._attributes <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.SlashGreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._slashGreaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlMemberAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._base), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._base <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Token1), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._token1 <> punctuationSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Token2), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._token2 <> punctuationSyntax1) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.Token3), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._token3 <> punctuationSyntax2) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node._name <> xmlNodeSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), expressionSyntax, punctuationSyntax, punctuationSyntax1, punctuationSyntax2, xmlNodeSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = DirectCast(Me.Visit(node._prefix), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)
			If (node._prefix <> xmlPrefixSyntax) Then
				flag = True
			End If
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.Visit(node.LocalName), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node._localName <> xmlNameTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlPrefixSyntax, xmlNameTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlNameAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(Me.Visit(node._name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (node._name <> xmlNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._equalsToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.StartQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._startQuoteToken <> punctuationSyntax1) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(Me.Visit(node._reference), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (node._reference <> identifierNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EndQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._endQuoteToken <> punctuationSyntax2) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax1, identifierNameSyntax, punctuationSyntax2))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlNamespaceImportsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax = DirectCast(Me.Visit(node._xmlNamespace), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)
			If (node._xmlNamespace <> xmlAttributeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.GreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._greaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlAttributeSyntax, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlPrefix(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node._name <> xmlNameTokenSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.ColonToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._colonToken <> punctuationSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlPrefixName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node._name <> xmlNameTokenSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlProcessingInstruction(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanQuestionToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanQuestionToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node._name <> xmlNameTokenSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = Me.VisitList(Of XmlTextTokenSyntax)(node.TextTokens)
			If (node._textTokens <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.QuestionGreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._questionGreaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNameTokenSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlString(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.StartQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._startQuoteToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = Me.VisitList(Of XmlTextTokenSyntax)(node.TextTokens)
			If (node._textTokens <> syntaxList.Node) Then
				flag = True
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.EndQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._endQuoteToken <> punctuationSyntax1) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, syntaxList.Node, punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlText(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = Me.VisitList(Of XmlTextTokenSyntax)(node.TextTokens)
			If (node._textTokens <> syntaxList.Node) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), syntaxList.Node))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitYieldStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.YieldKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._yieldKeyword <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(Me.Visit(node._expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (node._expression <> expressionSyntax) Then
				flag = True
			End If
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return visualBasicSyntaxNode
		End Function
	End Class
End Namespace