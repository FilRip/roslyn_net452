Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public MustInherit Class VisualBasicSyntaxRewriter
		Inherits VisualBasicSyntaxVisitor(Of SyntaxNode)
		Private ReadOnly _visitIntoStructuredTrivia As Boolean

		Private _recursionDepth As Integer

		Public Overridable ReadOnly Property VisitIntoStructuredTrivia As Boolean
			Get
				Return Me._visitIntoStructuredTrivia
			End Get
		End Property

		Public Sub New(Optional ByVal visitIntoStructuredTrivia As Boolean = False)
			MyBase.New()
			Me._visitIntoStructuredTrivia = visitIntoStructuredTrivia
		End Sub

		Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (node Is Nothing) Then
				syntaxNode = node
			Else
				Me._recursionDepth = Me._recursionDepth + 1
				StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
				Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).Accept(Of Microsoft.CodeAnalysis.SyntaxNode)(Me)
				Me._recursionDepth = Me._recursionDepth - 1
				syntaxNode = syntaxNode1
			End If
			Return syntaxNode
		End Function

		Public Overrides Function VisitAccessorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim accessorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax = DirectCast(Me.Visit(node.AccessorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax)
			If (node.AccessorStatement <> accessorStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndAccessorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndAccessorStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), accessorStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAccessorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.AccessorKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AccessorKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, parameterListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAddRemoveHandlerStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.AddHandlerOrRemoveHandlerKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AddHandlerOrRemoveHandlerKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.EventExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.EventExpression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CommaToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CommaToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.DelegateExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.DelegateExpression <> expressionSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, punctuationSyntax, expressionSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAggregateClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.AggregateKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AggregateKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(node.Variables)
			If (node._variables <> collectionRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			Dim queryClauseSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)(node.AdditionalQueryOperators)
			If (node._additionalQueryOperators <> queryClauseSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.IntoKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IntoKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim aggregationRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(node.AggregationVariables)
			If (node._aggregationVariables <> aggregationRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, collectionRangeVariableSyntaxes.Node, queryClauseSyntaxes.Node, keywordSyntax1, aggregationRangeVariableSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAggregationRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax = DirectCast(Me.Visit(node.NameEquals), Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax)
			If (node.NameEquals <> variableNameEqualsSyntax) Then
				flag = True
			End If
			Dim aggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax = DirectCast(Me.Visit(node.Aggregation), Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax)
			If (node.Aggregation <> aggregationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), variableNameEqualsSyntax, aggregationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAnonymousObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.NewKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NewKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim objectMemberInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax)
			If (node.Initializer <> objectMemberInitializerSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, attributeListSyntaxes.Node, objectMemberInitializerSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitArgumentList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim argumentSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax)(node.Arguments)
			If (node._arguments <> argumentSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, argumentSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitArrayCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.NewKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NewKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArrayBounds), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArrayBounds <> argumentListSyntax) Then
				flag = True
			End If
			Dim arrayRankSpecifierSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)(node.RankSpecifiers)
			If (node._rankSpecifiers <> arrayRankSpecifierSyntaxes.Node) Then
				flag = True
			End If
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)
			If (node.Initializer <> collectionInitializerSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, attributeListSyntaxes.Node, typeSyntax, argumentListSyntax, arrayRankSpecifierSyntaxes.Node, collectionInitializerSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitArrayRankSpecifier(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.CommaTokens)
			If (node.CommaTokens.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenLists.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitArrayType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.ElementType), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.ElementType <> typeSyntax) Then
				flag = True
			End If
			Dim arrayRankSpecifierSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)(node.RankSpecifiers)
			If (node._rankSpecifiers <> arrayRankSpecifierSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax, arrayRankSpecifierSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAsNewClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.AsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim newExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax = DirectCast(Me.Visit(node.NewExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax)
			If (node.NewExpression <> newExpressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, newExpressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAssignmentStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Left <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.OperatorToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OperatorToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Right <> expressionSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeTargetSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax = DirectCast(Me.Visit(node.Target), Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax)
			If (node.Target <> attributeTargetSyntax) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Name <> typeSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArgumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArgumentList <> argumentListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeTargetSyntax, typeSyntax, argumentListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAttributeList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim attributeSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)(node.Attributes)
			If (node._attributes <> attributeSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.GreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.GreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, attributeSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAttributesStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAttributeTarget(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.AttributeModifier)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AttributeModifier.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ColonToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.ColonToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitAwaitExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.AwaitExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.AwaitKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AwaitKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.AwaitExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitBadDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.HashToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitBinaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.IfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.FirstExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.FirstExpression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CommaToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CommaToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.SecondExpression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.SecondExpression <> expressionSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, expressionSyntax1, punctuationSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitBinaryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Left <> expressionSyntax) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.VisitToken(node.OperatorToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node.OperatorToken.Node <> syntaxToken) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Right <> expressionSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, syntaxToken, expressionSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCallStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.CallKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.CallKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Invocation), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Invocation <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCaseBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax = DirectCast(Me.Visit(node.CaseStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax)
			If (node.CaseStatement <> caseStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), caseStatementSyntax, statementSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCaseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.CaseKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.CaseKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim caseClauseSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax)(node.Cases)
			If (node._cases <> caseClauseSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, caseClauseSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim catchStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax = DirectCast(Me.Visit(node.CatchStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax)
			If (node.CatchStatement <> catchStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), catchStatementSyntax, statementSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCatchFilterClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.WhenKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WhenKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Filter), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Filter <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCatchStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.CatchKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.CatchKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(Me.Visit(node.IdentifierName), Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (node.IdentifierName <> identifierNameSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim catchFilterClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax = DirectCast(Me.Visit(node.WhenClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax)
			If (node.WhenClause <> catchFilterClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, identifierNameSyntax, simpleAsClauseSyntax, catchFilterClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitClassBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim classStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax = DirectCast(Me.Visit(node.ClassStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax)
			If (node.ClassStatement <> classStatementSyntax) Then
				flag = True
			End If
			Dim inheritsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> inheritsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim implementsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> implementsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Members)
			If (node._members <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndClassStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndClassStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), classStatementSyntax, inheritsStatementSyntaxes.Node, implementsStatementSyntaxes.Node, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitClassStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ClassKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ClassKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = DirectCast(Me.Visit(node.TypeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)
			If (node.TypeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCollectionInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenBraceToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenBraceToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(node.Initializers)
			If (node._initializers <> expressionSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseBraceToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseBraceToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCollectionRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)
			If (node.Identifier <> modifiedIdentifierSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.InKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.InKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCompilationUnit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim optionStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax)(node.Options)
			If (node._options <> optionStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim importsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax)(node.[Imports])
			If (node._imports <> importsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim attributesStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax)(node.Attributes)
			If (node._attributes <> attributesStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Members)
			If (node._members <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.EndOfFileToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EndOfFileToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), optionStatementSyntaxes.Node, importsStatementSyntaxes.Node, attributesStatementSyntaxes.Node, statementSyntaxes.Node, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitConditionalAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.QuestionMarkToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.QuestionMarkToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.WhenNotNull), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.WhenNotNull <> expressionSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, expressionSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitConstDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ConstKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ConstKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Name)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Name.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EqualsToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Value <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, identifierTokenSyntax, punctuationSyntax1, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitConstructorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim subNewStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax = DirectCast(Me.Visit(node.SubNewStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax)
			If (node.SubNewStatement <> subNewStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndSubStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndSubStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), subNewStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitContinueStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ContinueKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ContinueKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.BlockKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.BlockKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCrefOperatorReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OperatorKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OperatorKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OperatorToken)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node.OperatorToken.Node <> syntaxToken1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, syntaxToken1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCrefReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Name <> typeSyntax) Then
				flag = True
			End If
			Dim crefSignatureSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax = DirectCast(Me.Visit(node.Signature), Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax)
			If (node.Signature <> crefSignatureSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax, crefSignatureSyntax, simpleAsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCrefSignature(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim crefSignaturePartSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax)(node.ArgumentTypes)
			If (node._argumentTypes <> crefSignaturePartSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, crefSignaturePartSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCrefSignaturePart(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.Modifier).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Modifier.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, typeSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitCTypeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Keyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CommaToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CommaToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDeclareStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.DeclareKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.DeclareKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CharsetKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.CharsetKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.SubOrFunctionKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SubOrFunctionKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.LibKeyword)
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.LibKeyword.Node <> keywordSyntax3) Then
				flag = True
			End If
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax = DirectCast(Me.Visit(node.LibraryName), Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax)
			If (node.LibraryName <> literalExpressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.AliasKeyword)
			Dim keywordSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AliasKeyword.Node <> keywordSyntax4) Then
				flag = True
			End If
			Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax = DirectCast(Me.Visit(node.AliasName), Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax)
			If (node.AliasName <> literalExpressionSyntax1) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, keywordSyntax1, keywordSyntax2, identifierTokenSyntax, keywordSyntax3, literalExpressionSyntax, keywordSyntax4, literalExpressionSyntax1, parameterListSyntax, simpleAsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDelegateStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.DelegateKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.DelegateKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.SubOrFunctionKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SubOrFunctionKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = DirectCast(Me.Visit(node.TypeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)
			If (node.TypeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, keywordSyntax1, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDirectCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectCastExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Keyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CommaToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CommaToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectCastExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDisableWarningDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.DisableKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.DisableKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.WarningKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WarningKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim identifierNameSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(node.ErrorCodes)
			If (node._errorCodes <> identifierNameSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1, identifierNameSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDistinctClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.DistinctKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.DistinctKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDocumentationCommentTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlNodeSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(node.Content)
			If (node._content <> xmlNodeSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNodeSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDoLoopBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax = DirectCast(Me.Visit(node.DoStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax)
			If (node.DoStatement <> doStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax = DirectCast(Me.Visit(node.LoopStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax)
			If (node.LoopStatement <> loopStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), doStatementSyntax, statementSyntaxes.Node, loopStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitDoStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.DoKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.DoKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax = DirectCast(Me.Visit(node.WhileOrUntilClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax)
			If (node.WhileOrUntilClause <> whileOrUntilClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim elseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax = DirectCast(Me.Visit(node.ElseStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax)
			If (node.ElseStatement <> elseStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), elseStatementSyntax, statementSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ElseKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ElseKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ElseKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ElseKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseIfBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim elseIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax = DirectCast(Me.Visit(node.ElseIfStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax)
			If (node.ElseIfStatement <> elseIfStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), elseIfStatementSyntax, statementSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ElseIfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ElseIfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ThenKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ThenKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ElseKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ElseKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEmptyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.Empty).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Empty.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEnableWarningDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnableWarningDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EnableKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EnableKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.WarningKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WarningKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim identifierNameSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(node.ErrorCodes)
			If (node._errorCodes <> identifierNameSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EnableWarningDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1, identifierNameSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEndBlockStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.EndKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EndKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.BlockKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.BlockKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEndExternalSourceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EndKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EndKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ExternalSourceKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ExternalSourceKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EndExternalSourceDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEndIfDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EndKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EndKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.IfKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IfKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEndRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EndKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EndKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.RegionKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.RegionKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEnumBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim enumStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax = DirectCast(Me.Visit(node.EnumStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax)
			If (node.EnumStatement <> enumStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Members)
			If (node._members <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndEnumStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndEnumStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), enumStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEnumMemberDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.VisitToken(node.Identifier).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
			If (node.Initializer <> equalsValueSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, identifierTokenSyntax, equalsValueSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEnumStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.EnumKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EnumKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax = DirectCast(Me.Visit(node.UnderlyingType), Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)
			If (node.UnderlyingType <> asClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, identifierTokenSyntax, asClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEqualsValue(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.EqualsToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Value <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEraseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.EraseKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EraseKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(node.Expressions)
			If (node._expressions <> expressionSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitErrorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ErrorKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ErrorKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.ErrorNumber), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.ErrorNumber <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEventBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax = DirectCast(Me.Visit(node.EventStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax)
			If (node.EventStatement <> eventStatementSyntax) Then
				flag = True
			End If
			Dim accessorBlockSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)(node.Accessors)
			If (node._accessors <> accessorBlockSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndEventStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndEventStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), eventStatementSyntax, accessorBlockSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitEventStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.CustomKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.CustomKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EventKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EventKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax = DirectCast(Me.Visit(node.ImplementsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)
			If (node.ImplementsClause <> implementsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, keywordSyntax1, identifierTokenSyntax, parameterListSyntax, simpleAsClauseSyntax, implementsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitExitStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ExitKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ExitKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.BlockKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.BlockKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitExpressionRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax = DirectCast(Me.Visit(node.NameEquals), Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax)
			If (node.NameEquals <> variableNameEqualsSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), variableNameEqualsSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitExpressionStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitExternalChecksumDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ExternalChecksumKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ExternalChecksumKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ExternalSource)
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node.ExternalSource.Node <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.FirstCommaToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.FirstCommaToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Guid)
			Dim stringLiteralTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node.Guid.Node <> stringLiteralTokenSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.SecondCommaToken)
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.SecondCommaToken.Node <> punctuationSyntax3) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Checksum)
			Dim stringLiteralTokenSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node.Checksum.Node <> stringLiteralTokenSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax4) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax1, stringLiteralTokenSyntax, punctuationSyntax2, stringLiteralTokenSyntax1, punctuationSyntax3, stringLiteralTokenSyntax2, punctuationSyntax4))
			Return syntaxNode
		End Function

		Public Overrides Function VisitExternalSourceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ExternalSourceKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ExternalSourceKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ExternalSource)
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node.ExternalSource.Node <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CommaToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CommaToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.LineStart)
			Dim integerLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)
			If (node.LineStart.Node <> integerLiteralTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax3) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, punctuationSyntax1, stringLiteralTokenSyntax, punctuationSyntax2, integerLiteralTokenSyntax, punctuationSyntax3))
			Return syntaxNode
		End Function

		Public Overrides Function VisitFieldDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim variableDeclaratorSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(node.Declarators)
			If (node._declarators <> variableDeclaratorSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, variableDeclaratorSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitFinallyBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim finallyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax = DirectCast(Me.Visit(node.FinallyStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax)
			If (node.FinallyStatement <> finallyStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), finallyStatementSyntax, statementSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitFinallyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.FinallyKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.FinallyKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitForBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim forStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax = DirectCast(Me.Visit(node.ForStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax)
			If (node.ForStatement <> forStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax = DirectCast(Me.Visit(node.NextStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax)
			If (node.NextStatement <> nextStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), forStatementSyntax, statementSyntaxes.Node, nextStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitForEachBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim forEachStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax = DirectCast(Me.Visit(node.ForEachStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax)
			If (node.ForEachStatement <> forEachStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax = DirectCast(Me.Visit(node.NextStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax)
			If (node.NextStatement <> nextStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), forEachStatementSyntax, statementSyntaxes.Node, nextStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ForKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ForKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EachKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EachKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(Me.Visit(node.ControlVariable), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			If (node.ControlVariable <> visualBasicSyntaxNode) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.InKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.InKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, visualBasicSyntaxNode, keywordSyntax2, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitForStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ForKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ForKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(Me.Visit(node.ControlVariable), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			If (node.ControlVariable <> visualBasicSyntaxNode) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EqualsToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.FromValue), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.FromValue <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ToKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ToKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.ToValue), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.ToValue <> expressionSyntax1) Then
				flag = True
			End If
			Dim forStepClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax = DirectCast(Me.Visit(node.StepClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax)
			If (node.StepClause <> forStepClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, visualBasicSyntaxNode, punctuationSyntax, expressionSyntax, keywordSyntax1, expressionSyntax1, forStepClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitForStepClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.StepKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.StepKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.StepValue), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.StepValue <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitFromClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.FromKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.FromKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(node.Variables)
			If (node._variables <> collectionRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, collectionRangeVariableSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitFunctionAggregation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.FunctionName)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.FunctionName.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Argument), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Argument <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGenericName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.VisitToken(node.Identifier).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeArgumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax = DirectCast(Me.Visit(node.TypeArgumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax)
			If (node.TypeArgumentList <> typeArgumentListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, typeArgumentListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGetTypeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetTypeExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.GetTypeKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GetTypeKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GetTypeExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, typeSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGetXmlNamespaceExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.GetXmlNamespaceKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GetXmlNamespaceKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlPrefixNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax)
			If (node.Name <> xmlPrefixNameSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, xmlPrefixNameSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGlobalName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.GlobalKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GlobalKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGoToStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.GoToKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GoToKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax = DirectCast(Me.Visit(node.Label), Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)
			If (node.Label <> labelSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, labelSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGroupAggregation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.GroupKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GroupKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGroupByClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.GroupKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GroupKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(node.Items)
			If (node._items <> expressionRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ByKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ByKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionRangeVariableSyntaxes1 As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(node.Keys)
			If (node._keys <> expressionRangeVariableSyntaxes1.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.IntoKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IntoKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			Dim aggregationRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(node.AggregationVariables)
			If (node._aggregationVariables <> aggregationRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionRangeVariableSyntaxes.Node, keywordSyntax1, expressionRangeVariableSyntaxes1.Node, keywordSyntax2, aggregationRangeVariableSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitGroupJoinClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.GroupKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GroupKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.JoinKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.JoinKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(node.JoinedVariables)
			If (node._joinedVariables <> collectionRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			Dim joinClauseSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)(node.AdditionalJoins)
			If (node._additionalJoins <> joinClauseSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OnKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OnKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			Dim joinConditionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(node.JoinConditions)
			If (node._joinConditions <> joinConditionSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.IntoKeyword)
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IntoKeyword.Node <> keywordSyntax3) Then
				flag = True
			End If
			Dim aggregationRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)(node.AggregationVariables)
			If (node._aggregationVariables <> aggregationRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, collectionRangeVariableSyntaxes.Node, joinClauseSyntaxes.Node, keywordSyntax2, joinConditionSyntaxes.Node, keywordSyntax3, aggregationRangeVariableSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitHandlesClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.HandlesKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.HandlesKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim handlesClauseItemSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax)(node.Events)
			If (node._events <> handlesClauseItemSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, handlesClauseItemSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitHandlesClauseItem(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim eventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax = DirectCast(Me.Visit(node.EventContainer), Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax)
			If (node.EventContainer <> eventContainerSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.DotToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.DotToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(Me.Visit(node.EventMember), Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (node.EventMember <> identifierNameSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), eventContainerSyntax, punctuationSyntax, identifierNameSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitIdentifierName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.VisitToken(node.Identifier).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitIfDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ElseKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ElseKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.IfOrElseIfKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IfOrElseIfKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ThenKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ThenKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, keywordSyntax1, expressionSyntax, keywordSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.IfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ThenKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ThenKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitImplementsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ImplementsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ImplementsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim qualifiedNameSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)(node.InterfaceMembers)
			If (node._interfaceMembers <> qualifiedNameSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, qualifiedNameSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitImplementsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ImplementsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ImplementsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim typeSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(node.Types)
			If (node._types <> typeSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, typeSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitImportAliasClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EqualsToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitImportsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ImportsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ImportsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim importsClauseSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax)(node.ImportsClauses)
			If (node._importsClauses <> importsClauseSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, importsClauseSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitIncompleteMember(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.VisitToken(node.MissingIdentifier).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.MissingIdentifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, identifierTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInferredFieldInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.KeyKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.KeyKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInheritsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.InheritsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.InheritsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim typeSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(node.Types)
			If (node._types <> typeSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, typeSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterfaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim interfaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax = DirectCast(Me.Visit(node.InterfaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax)
			If (node.InterfaceStatement <> interfaceStatementSyntax) Then
				flag = True
			End If
			Dim inheritsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> inheritsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim implementsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> implementsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Members)
			If (node._members <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndInterfaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndInterfaceStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), interfaceStatementSyntax, inheritsStatementSyntaxes.Node, implementsStatementSyntaxes.Node, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterfaceStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.InterfaceKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.InterfaceKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = DirectCast(Me.Visit(node.TypeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)
			If (node.TypeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterpolatedStringExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.DollarSignDoubleQuoteToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.DollarSignDoubleQuoteToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim interpolatedStringContentSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax)(node.Contents)
			If (node._contents <> interpolatedStringContentSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.DoubleQuoteToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.DoubleQuoteToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, interpolatedStringContentSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterpolatedStringText(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim interpolatedStringTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax = DirectCast(Me.VisitToken(node.TextToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
			If (node.TextToken.Node <> interpolatedStringTextTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), interpolatedStringTextTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterpolation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenBraceToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenBraceToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			Dim interpolationAlignmentClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax = DirectCast(Me.Visit(node.AlignmentClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax)
			If (node.AlignmentClause <> interpolationAlignmentClauseSyntax) Then
				flag = True
			End If
			Dim interpolationFormatClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax = DirectCast(Me.Visit(node.FormatClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax)
			If (node.FormatClause <> interpolationFormatClauseSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseBraceToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseBraceToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax, interpolationAlignmentClauseSyntax, interpolationFormatClauseSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterpolationAlignmentClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.CommaToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CommaToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Value <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterpolationFormatClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ColonToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.ColonToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.FormatStringToken)
			Dim interpolatedStringTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
			If (node.FormatStringToken.Node <> interpolatedStringTextTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, interpolatedStringTextTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitInvocationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArgumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArgumentList <> argumentListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, argumentListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitJoinCondition(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Left <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.EqualsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.EqualsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Right <> expressionSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitKeywordEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.Keyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitLabel(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.VisitToken(node.LabelToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node.LabelToken.Node <> syntaxToken) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken))
			Return syntaxNode
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LabelToken)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node.LabelToken.Node <> syntaxToken1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ColonToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.ColonToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken1, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitLambdaHeader(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.SubOrFunctionKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SubOrFunctionKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, parameterListSyntax, simpleAsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitLetClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.LetKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.LetKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(node.Variables)
			If (node._variables <> expressionRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionRangeVariableSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overridable Function VisitList(Of TNode As SyntaxNode)(ByVal list As SyntaxList(Of TNode)) As SyntaxList(Of TNode)
			Dim tNodes As SyntaxList(Of TNode)
			Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.SyntaxListBuilder = Nothing
			Dim num As Integer = 0
			Dim count As Integer = list.Count
			While num < count
				Dim item As TNode = list(num)
				Dim tNode1 As TNode = Me.VisitListElement(Of TNode)(item)
				If (item <> tNode1 AndAlso syntaxListBuilder Is Nothing) Then
					syntaxListBuilder = New Microsoft.CodeAnalysis.Syntax.SyntaxListBuilder(count)
					syntaxListBuilder.AddRange(Of TNode)(list, 0, num)
				End If
				If (syntaxListBuilder IsNot Nothing AndAlso tNode1 IsNot Nothing AndAlso DirectCast(tNode1, SyntaxNode).Kind() <> SyntaxKind.None) Then
					syntaxListBuilder.Add(DirectCast(tNode1, SyntaxNode))
				End If
				num = num + 1
			End While
			tNodes = If(syntaxListBuilder Is Nothing, list, syntaxListBuilder.ToList(Of TNode)())
			Return tNodes
		End Function

		Public Overridable Function VisitList(ByVal list As SyntaxTokenList) As SyntaxTokenList
			Dim syntaxTokenListBuilder As Microsoft.CodeAnalysis.Syntax.SyntaxTokenListBuilder = Nothing
			Dim num As Integer = -1
			Dim count As Integer = list.Count
			Dim enumerator As SyntaxTokenList.Enumerator = list.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.SyntaxToken = enumerator.Current
				num = num + 1
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitListElement(current)
				If (current <> syntaxToken AndAlso syntaxTokenListBuilder Is Nothing) Then
					syntaxTokenListBuilder = New Microsoft.CodeAnalysis.Syntax.SyntaxTokenListBuilder(count)
					syntaxTokenListBuilder.Add(list, 0, num)
				End If
				If (syntaxTokenListBuilder Is Nothing OrElse syntaxToken.Kind() = SyntaxKind.None) Then
					Continue While
				End If
				syntaxTokenListBuilder.Add(syntaxToken)
			End While
			Return If(syntaxTokenListBuilder Is Nothing, list, syntaxTokenListBuilder.ToList())
		End Function

		Public Overridable Function VisitList(Of TNode As SyntaxNode)(ByVal list As SeparatedSyntaxList(Of TNode)) As SeparatedSyntaxList(Of TNode)
			Dim count As Integer = list.Count
			Dim separatorCount As Integer = list.SeparatorCount
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of TNode) = New SeparatedSyntaxListBuilder(Of TNode)()
			Dim num As Integer = 0
			Do
				Dim item As TNode = list(num)
				Dim tNode1 As TNode = Me.VisitListElement(Of TNode)(item)
				Dim separator As Microsoft.CodeAnalysis.SyntaxToken = list.GetSeparator(num)
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitListSeparator(separator)
				If (separatedSyntaxListBuilder.IsNull AndAlso (item <> tNode1 OrElse separator <> syntaxToken)) Then
					separatedSyntaxListBuilder = New SeparatedSyntaxListBuilder(Of TNode)(count)
					separatedSyntaxListBuilder.AddRange(list, num)
				End If
				If (Not separatedSyntaxListBuilder.IsNull) Then
					If (tNode1 IsNot Nothing) Then
						separatedSyntaxListBuilder.Add(tNode1)
						If (syntaxToken.RawKind = 0) Then
							Throw New InvalidOperationException("separator is expected")
						End If
						separatedSyntaxListBuilder.AddSeparator(syntaxToken)
					ElseIf (tNode1 Is Nothing) Then
						Throw New InvalidOperationException("element is expected")
					End If
				End If
				num = num + 1
			Loop While num < separatorCount
			If (num < count) Then
				Dim item1 As TNode = list(num)
				Dim tNode2 As TNode = Me.VisitListElement(Of TNode)(item1)
				If (separatedSyntaxListBuilder.IsNull AndAlso item1 <> tNode2) Then
					separatedSyntaxListBuilder = New SeparatedSyntaxListBuilder(Of TNode)(count)
					separatedSyntaxListBuilder.AddRange(list, num)
				End If
				If (Not separatedSyntaxListBuilder.IsNull AndAlso tNode2 IsNot Nothing) Then
					separatedSyntaxListBuilder.Add(tNode2)
				End If
			End If
			Return If(separatedSyntaxListBuilder.IsNull, list, separatedSyntaxListBuilder.ToList())
		End Function

		Public Overridable Function VisitList(ByVal list As SyntaxTriviaList) As SyntaxTriviaList
			Dim syntaxTriviaLists As SyntaxTriviaList
			Dim count As Integer = list.Count
			If (count <> 0) Then
				Dim syntaxTriviaListBuilder As Microsoft.CodeAnalysis.Syntax.SyntaxTriviaListBuilder = Nothing
				Dim num As Integer = -1
				Dim enumerator As SyntaxTriviaList.Enumerator = list.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.SyntaxTrivia = enumerator.Current
					num = num + 1
					Dim syntaxTrivium As Microsoft.CodeAnalysis.SyntaxTrivia = Me.VisitListElement(current)
					If (syntaxTrivium <> current AndAlso syntaxTriviaListBuilder Is Nothing) Then
						syntaxTriviaListBuilder = New Microsoft.CodeAnalysis.Syntax.SyntaxTriviaListBuilder(count)
						syntaxTriviaListBuilder.Add(list, 0, num)
					End If
					If (syntaxTriviaListBuilder Is Nothing OrElse syntaxTrivium.Kind() = SyntaxKind.None) Then
						Continue While
					End If
					syntaxTriviaListBuilder.Add(syntaxTrivium)
				End While
				If (syntaxTriviaListBuilder Is Nothing) Then
					syntaxTriviaLists = list
					Return syntaxTriviaLists
				End If
				syntaxTriviaLists = syntaxTriviaListBuilder.ToList()
				Return syntaxTriviaLists
			End If
			syntaxTriviaLists = list
			Return syntaxTriviaLists
		End Function

		Public Overridable Function VisitListElement(Of TNode As SyntaxNode)(ByVal node As TNode) As TNode
			Return DirectCast(Me.Visit(DirectCast(node, SyntaxNode)), TNode)
		End Function

		Public Overridable Function VisitListElement(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.VisitToken(token)
		End Function

		Public Overridable Function VisitListElement(ByVal element As Microsoft.CodeAnalysis.SyntaxTrivia) As Microsoft.CodeAnalysis.SyntaxTrivia
			Return Me.VisitTrivia(element)
		End Function

		Public Overridable Function VisitListSeparator(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.VisitToken(token)
		End Function

		Public Overrides Function VisitLiteralExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.VisitToken(node.Token).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node.Token.Node <> syntaxToken) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken))
			Return syntaxNode
		End Function

		Public Overrides Function VisitLocalDeclarationStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim variableDeclaratorSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(node.Declarators)
			If (node._declarators <> variableDeclaratorSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxTokenLists.Node, variableDeclaratorSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitLoopStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.LoopKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.LoopKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax = DirectCast(Me.Visit(node.WhileOrUntilClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax)
			If (node.WhileOrUntilClause <> whileOrUntilClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, whileOrUntilClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.Keyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMemberAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.OperatorToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OperatorToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)
			If (node.Name <> simpleNameSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, simpleNameSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMethodBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim methodStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax = DirectCast(Me.Visit(node.SubOrFunctionStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax)
			If (node.SubOrFunctionStatement <> methodStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndSubOrFunctionStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndSubOrFunctionStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), methodStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMethodStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.SubOrFunctionKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SubOrFunctionKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = DirectCast(Me.Visit(node.TypeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)
			If (node.TypeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax = DirectCast(Me.Visit(node.HandlesClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax)
			If (node.HandlesClause <> handlesClauseSyntax) Then
				flag = True
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax = DirectCast(Me.Visit(node.ImplementsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)
			If (node.ImplementsClause <> implementsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax, handlesClauseSyntax, implementsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMidExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.VisitToken(node.Mid).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Mid.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArgumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArgumentList <> argumentListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, argumentListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitModifiedIdentifier(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Nullable)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Nullable.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArrayBounds), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArrayBounds <> argumentListSyntax) Then
				flag = True
			End If
			Dim arrayRankSpecifierSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)(node.ArrayRankSpecifiers)
			If (node._arrayRankSpecifiers <> arrayRankSpecifierSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, punctuationSyntax, argumentListSyntax, arrayRankSpecifierSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitModuleBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim moduleStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax = DirectCast(Me.Visit(node.ModuleStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax)
			If (node.ModuleStatement <> moduleStatementSyntax) Then
				flag = True
			End If
			Dim inheritsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> inheritsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim implementsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> implementsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Members)
			If (node._members <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndModuleStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndModuleStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), moduleStatementSyntax, inheritsStatementSyntaxes.Node, implementsStatementSyntaxes.Node, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitModuleStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ModuleKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ModuleKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = DirectCast(Me.Visit(node.TypeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)
			If (node.TypeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMultiLineIfBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax = DirectCast(Me.Visit(node.IfStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax)
			If (node.IfStatement <> ifStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim elseIfBlockSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax)(node.ElseIfBlocks)
			If (node._elseIfBlocks <> elseIfBlockSyntaxes.Node) Then
				flag = True
			End If
			Dim elseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax = DirectCast(Me.Visit(node.ElseBlock), Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax)
			If (node.ElseBlock <> elseBlockSyntax) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndIfStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndIfStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), ifStatementSyntax, statementSyntaxes.Node, elseIfBlockSyntaxes.Node, elseBlockSyntax, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMultiLineLambdaExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim lambdaHeaderSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax = DirectCast(Me.Visit(node.SubOrFunctionHeader), Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax)
			If (node.SubOrFunctionHeader <> lambdaHeaderSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndSubOrFunctionStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndSubOrFunctionStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), lambdaHeaderSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMyBaseExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.Keyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitMyClassExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.MyClassExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.Keyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.MyClassExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNameColonEquals(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (node.Name <> identifierNameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.ColonEqualsToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.ColonEqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierNameSyntax, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNamedFieldInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.KeyKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.KeyKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.DotToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.DotToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (node.Name <> identifierNameSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EqualsToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, identifierNameSyntax, punctuationSyntax1, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNamedTupleElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.VisitToken(node.Identifier).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax, simpleAsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNameOfExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.NameOfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NameOfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Argument), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Argument <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNamespaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim namespaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax = DirectCast(Me.Visit(node.NamespaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax)
			If (node.NamespaceStatement <> namespaceStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Members)
			If (node._members <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndNamespaceStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndNamespaceStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), namespaceStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNamespaceStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.NamespaceKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NamespaceKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)
			If (node.Name <> nameSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, nameSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNextStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.NextKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NextKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(node.ControlVariables)
			If (node._controlVariables <> expressionSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitNullableType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.ElementType), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.ElementType <> typeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.QuestionMarkToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.QuestionMarkToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitObjectCollectionInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.FromKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.FromKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)
			If (node.Initializer <> collectionInitializerSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, collectionInitializerSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.NewKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NewKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArgumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArgumentList <> argumentListSyntax) Then
				flag = True
			End If
			Dim objectCreationInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax)
			If (node.Initializer <> objectCreationInitializerSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, attributeListSyntaxes.Node, typeSyntax, argumentListSyntax, objectCreationInitializerSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitObjectMemberInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.WithKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WithKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenBraceToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenBraceToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim fieldInitializerSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax)(node.Initializers)
			If (node._initializers <> fieldInitializerSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseBraceToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseBraceToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, fieldInitializerSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOmittedArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.Empty).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Empty.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOnErrorGoToStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OnKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OnKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ErrorKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ErrorKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.GoToKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.GoToKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Minus)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Minus.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax = DirectCast(Me.Visit(node.Label), Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)
			If (node.Label <> labelSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, keywordSyntax2, punctuationSyntax, labelSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOnErrorResumeNextStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OnKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OnKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ErrorKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ErrorKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ResumeKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ResumeKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.NextKeyword)
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NextKeyword.Node <> keywordSyntax3) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, keywordSyntax2, keywordSyntax3))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOperatorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim operatorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax = DirectCast(Me.Visit(node.OperatorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax)
			If (node.OperatorStatement <> operatorStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndOperatorStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndOperatorStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), operatorStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOperatorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OperatorKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OperatorKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OperatorToken)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node.OperatorToken.Node <> syntaxToken1) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, syntaxToken1, parameterListSyntax, simpleAsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOptionStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OptionKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OptionKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.NameKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NameKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ValueKeyword)
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ValueKeyword.Node <> keywordSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, keywordSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOrderByClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OrderKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OrderKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ByKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ByKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim orderingSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)(node.Orderings)
			If (node._orderings <> orderingSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, orderingSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitOrdering(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.AscendingOrDescendingKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AscendingOrDescendingKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitParameter(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)
			If (node.Identifier <> modifiedIdentifierSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax = DirectCast(Me.Visit(node.[Default]), Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
			If (node.[Default] <> equalsValueSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, modifiedIdentifierSyntax, simpleAsClauseSyntax, equalsValueSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitParameterList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim parameterSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)(node.Parameters)
			If (node._parameters <> parameterSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, parameterSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitParenthesizedExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitPartitionClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.SkipOrTakeKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SkipOrTakeKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Count), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Count <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitPartitionWhileClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.SkipOrTakeKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SkipOrTakeKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.WhileKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WhileKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitPredefinedCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedCastExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Keyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedCastExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitPredefinedType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.Keyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitPrintStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.QuestionToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.QuestionToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitPropertyBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax = DirectCast(Me.Visit(node.PropertyStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax)
			If (node.PropertyStatement <> propertyStatementSyntax) Then
				flag = True
			End If
			Dim accessorBlockSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax)(node.Accessors)
			If (node._accessors <> accessorBlockSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndPropertyStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndPropertyStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), propertyStatementSyntax, accessorBlockSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitPropertyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.PropertyKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.PropertyKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)
			If (node.AsClause <> asClauseSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
			If (node.Initializer <> equalsValueSyntax) Then
				flag = True
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax = DirectCast(Me.Visit(node.ImplementsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)
			If (node.ImplementsClause <> implementsClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, identifierTokenSyntax, parameterListSyntax, asClauseSyntax, equalsValueSyntax, implementsClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitQualifiedCrefOperatorReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)
			If (node.Left <> nameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.DotToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.DotToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim crefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax)
			If (node.Right <> crefOperatorReferenceSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), nameSyntax, punctuationSyntax, crefOperatorReferenceSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitQualifiedName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)
			If (node.Left <> nameSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.DotToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.DotToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)
			If (node.Right <> simpleNameSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), nameSyntax, punctuationSyntax, simpleNameSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim queryClauseSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)(node.Clauses)
			If (node._clauses <> queryClauseSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), queryClauseSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitRaiseEventStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.RaiseEventKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.RaiseEventKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (node.Name <> identifierNameSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArgumentList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArgumentList <> argumentListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, identifierNameSyntax, argumentListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitRangeArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.LowerBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.LowerBound <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ToKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ToKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.UpperBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.UpperBound <> expressionSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitRangeCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.LowerBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.LowerBound <> expressionSyntax) Then
				flag = True
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ToKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ToKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.UpperBound), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.UpperBound <> expressionSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, keywordSyntax, expressionSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = DirectCast(Me.Visit(node.ArrayBounds), Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			If (node.ArrayBounds <> argumentListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, argumentListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitReDimStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.ReDimKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ReDimKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.PreserveKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.PreserveKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim redimClauseSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax)(node.Clauses)
			If (node._clauses <> redimClauseSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, redimClauseSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitReferenceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ReferenceKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ReferenceKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.File)
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node.File.Node <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.RegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.HashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.HashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.RegionKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.RegionKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Name)
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (node.Name.Node <> stringLiteralTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.RegionDirectiveTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, stringLiteralTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitRelationalCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.IsKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OperatorToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OperatorToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Value <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitResumeStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ResumeKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ResumeKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax = DirectCast(Me.Visit(node.Label), Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)
			If (node.Label <> labelSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, labelSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReturnStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ReturnKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ReturnKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ReturnStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSelectBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim selectStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax = DirectCast(Me.Visit(node.SelectStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax)
			If (node.SelectStatement <> selectStatementSyntax) Then
				flag = True
			End If
			Dim caseBlockSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax)(node.CaseBlocks)
			If (node._caseBlocks <> caseBlockSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndSelectStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndSelectStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), selectStatementSyntax, caseBlockSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSelectClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.SelectKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SelectKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(node.Variables)
			If (node._variables <> expressionRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionRangeVariableSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.SelectKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SelectKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CaseKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.CaseKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, keywordSyntax1, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSimpleArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim nameColonEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax = DirectCast(Me.Visit(node.NameColonEquals), Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax)
			If (node.NameColonEquals <> nameColonEqualsSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), nameColonEqualsSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSimpleAsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.AsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, attributeListSyntaxes.Node, typeSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSimpleCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Value <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSimpleImportsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim importAliasClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax = DirectCast(Me.Visit(node.[Alias]), Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax)
			If (node.[Alias] <> importAliasClauseSyntax) Then
				flag = True
			End If
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)
			If (node.Name <> nameSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), importAliasClauseSyntax, nameSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSimpleJoinClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.JoinKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.JoinKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(node.JoinedVariables)
			If (node._joinedVariables <> collectionRangeVariableSyntaxes.Node) Then
				flag = True
			End If
			Dim joinClauseSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)(node.AdditionalJoins)
			If (node._additionalJoins <> joinClauseSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OnKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OnKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim joinConditionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)(node.JoinConditions)
			If (node._joinConditions <> joinConditionSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, collectionRangeVariableSyntaxes.Node, joinClauseSyntaxes.Node, keywordSyntax1, joinConditionSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSingleLineElseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ElseKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ElseKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, statementSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSingleLineIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.IfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ThenKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ThenKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim singleLineElseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax = DirectCast(Me.Visit(node.ElseClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax)
			If (node.ElseClause <> singleLineElseClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1, statementSyntaxes.Node, singleLineElseClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSingleLineLambdaExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim lambdaHeaderSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax = DirectCast(Me.Visit(node.SubOrFunctionHeader), Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax)
			If (node.SubOrFunctionHeader <> lambdaHeaderSyntax) Then
				flag = True
			End If
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			If (node.Body <> visualBasicSyntaxNode) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), lambdaHeaderSyntax, visualBasicSyntaxNode))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSkippedTokensTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Tokens)
			If (node.Tokens.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxTokenLists.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSpecialConstraint(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ConstraintKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ConstraintKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitStopOrEndStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.StopOrEndKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.StopOrEndKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitStructureBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim structureStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax = DirectCast(Me.Visit(node.StructureStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax)
			If (node.StructureStatement <> structureStatementSyntax) Then
				flag = True
			End If
			Dim inheritsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax)(node.[Inherits])
			If (node._inherits <> inheritsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim implementsStatementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax)(node.[Implements])
			If (node._implements <> implementsStatementSyntaxes.Node) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Members)
			If (node._members <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndStructureStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndStructureStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), structureStatementSyntax, inheritsStatementSyntaxes.Node, implementsStatementSyntaxes.Node, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitStructureStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.StructureKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.StructureKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = DirectCast(Me.Visit(node.TypeParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)
			If (node.TypeParameterList <> typeParameterListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSubNewStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim attributeListSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(node.AttributeLists)
			If (node._attributeLists <> attributeListSyntaxes.Node) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.Modifiers)
			If (node.Modifiers.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.SubKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SubKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.NewKeyword)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.NewKeyword.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = DirectCast(Me.Visit(node.ParameterList), Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			If (node.ParameterList <> parameterListSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), attributeListSyntaxes.Node, syntaxTokenLists.Node, keywordSyntax, keywordSyntax1, parameterListSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSyncLockBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syncLockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax = DirectCast(Me.Visit(node.SyncLockStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax)
			If (node.SyncLockStatement <> syncLockStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndSyncLockStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndSyncLockStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syncLockStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.SyncLockKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.SyncLockKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTernaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.IfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.IfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.FirstCommaToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.FirstCommaToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.WhenTrue), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.WhenTrue <> expressionSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.SecondCommaToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.SecondCommaToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			Dim expressionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.WhenFalse), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.WhenFalse <> expressionSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax3) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, expressionSyntax1, punctuationSyntax2, expressionSyntax2, punctuationSyntax3))
			Return syntaxNode
		End Function

		Public Overrides Function VisitThrowStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ThrowStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.ThrowKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.ThrowKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.ThrowStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overridable Function VisitToken(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxTriviaLists As SyntaxTriviaList = Me.VisitList(token.LeadingTrivia)
			Dim syntaxTriviaLists1 As SyntaxTriviaList = Me.VisitList(token.TrailingTrivia)
			If (syntaxTriviaLists <> token.LeadingTrivia OrElse syntaxTriviaLists1 <> token.TrailingTrivia) Then
				If (syntaxTriviaLists <> token.LeadingTrivia) Then
					token = token.WithLeadingTrivia(syntaxTriviaLists)
				End If
				If (syntaxTriviaLists1 <> token.TrailingTrivia) Then
					token = token.WithTrailingTrivia(syntaxTriviaLists1)
				End If
			End If
			Return token
		End Function

		Public Overridable Function VisitTrivia(ByVal trivia As Microsoft.CodeAnalysis.SyntaxTrivia) As Microsoft.CodeAnalysis.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.SyntaxTrivia
			If (Me.VisitIntoStructuredTrivia AndAlso trivia.HasStructure) Then
				Dim [structure] As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(trivia.GetStructure(), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
				Dim structuredTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax = DirectCast(Me.Visit([structure]), Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax)
				If (structuredTriviaSyntax = [structure]) Then
					syntaxTrivium = trivia
					Return syntaxTrivium
				End If
				If (structuredTriviaSyntax Is Nothing) Then
					syntaxTrivium = New Microsoft.CodeAnalysis.SyntaxTrivia()
					Return syntaxTrivium
				Else
					syntaxTrivium = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.Trivia(structuredTriviaSyntax)
					Return syntaxTrivium
				End If
			End If
			syntaxTrivium = trivia
			Return syntaxTrivium
		End Function

		Public Overrides Function VisitTryBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim tryStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax = DirectCast(Me.Visit(node.TryStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax)
			If (node.TryStatement <> tryStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim catchBlockSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax)(node.CatchBlocks)
			If (node._catchBlocks <> catchBlockSyntaxes.Node) Then
				flag = True
			End If
			Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax = DirectCast(Me.Visit(node.FinallyBlock), Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax)
			If (node.FinallyBlock <> finallyBlockSyntax) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndTryStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndTryStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), tryStatementSyntax, statementSyntaxes.Node, catchBlockSyntaxes.Node, finallyBlockSyntax, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTryCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryCastExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Keyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.Keyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CommaToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CommaToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TryCastExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.TryKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.TryKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTupleExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim simpleArgumentSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax)(node.Arguments)
			If (node._arguments <> simpleArgumentSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, simpleArgumentSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTupleType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim tupleElementSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax)(node.Elements)
			If (node._elements <> tupleElementSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, tupleElementSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypeArgumentList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim typeSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(node.Arguments)
			If (node._arguments <> typeSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, typeSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypeConstraint(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypedTupleElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypedTupleElementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypedTupleElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), typeSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypeOfExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.TypeOfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.TypeOfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OperatorToken)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OperatorToken.Node <> keywordSyntax1) Then
				flag = True
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(Me.Visit(node.Type), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			If (node.Type <> typeSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, keywordSyntax1, typeSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypeParameter(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.VarianceKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.VarianceKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Identifier)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			Dim typeParameterConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax = DirectCast(Me.Visit(node.TypeParameterConstraintClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax)
			If (node.TypeParameterConstraintClause <> typeParameterConstraintClauseSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, identifierTokenSyntax, typeParameterConstraintClauseSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypeParameterList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.OpenParenToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenParenToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OfKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.OfKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim typeParameterSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax)(node.Parameters)
			If (node._parameters <> typeParameterSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseParenToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseParenToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, typeParameterSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypeParameterMultipleConstraintClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.AsKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.OpenBraceToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.OpenBraceToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim constraintSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)(node.Constraints)
			If (node._constraints <> constraintSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.CloseBraceToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.CloseBraceToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, punctuationSyntax, constraintSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitTypeParameterSingleConstraintClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.AsKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.AsKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim constraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax = DirectCast(Me.Visit(node.Constraint), Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)
			If (node.Constraint <> constraintSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, constraintSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitUnaryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(Me.VisitToken(node.OperatorToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (node.OperatorToken.Node <> syntaxToken) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Operand <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxToken, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitUsingBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim usingStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax = DirectCast(Me.Visit(node.UsingStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax)
			If (node.UsingStatement <> usingStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndUsingStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndUsingStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), usingStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.UsingKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.UsingKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			Dim variableDeclaratorSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(node.Variables)
			If (node._variables <> variableDeclaratorSyntaxes.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax, variableDeclaratorSyntaxes.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitVariableDeclarator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim modifiedIdentifierSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)(node.Names)
			If (node._names <> modifiedIdentifierSyntaxes.Node) Then
				flag = True
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)
			If (node.AsClause <> asClauseSyntax) Then
				flag = True
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
			If (node.Initializer <> equalsValueSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), modifiedIdentifierSyntaxes.Node, asClauseSyntax, equalsValueSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitVariableNameEquals(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = DirectCast(Me.Visit(node.Identifier), Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)
			If (node.Identifier <> modifiedIdentifierSyntax) Then
				flag = True
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = DirectCast(Me.Visit(node.AsClause), Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			If (node.AsClause <> simpleAsClauseSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.EqualsToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), modifiedIdentifierSyntax, simpleAsClauseSyntax, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWhereClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.WhereKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WhereKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWhileBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim whileStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax = DirectCast(Me.Visit(node.WhileStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax)
			If (node.WhileStatement <> whileStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndWhileStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndWhileStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), whileStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWhileOrUntilClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.WhileOrUntilKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WhileOrUntilKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.WhileKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WhileKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Condition <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWithBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim withStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax = DirectCast(Me.Visit(node.WithStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax)
			If (node.WithStatement <> withStatementSyntax) Then
				flag = True
			End If
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(node.Statements)
			If (node._statements <> statementSyntaxes.Node) Then
				flag = True
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = DirectCast(Me.Visit(node.EndWithStatement), Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			If (node.EndWithStatement <> endBlockStatementSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), withStatementSyntax, statementSyntaxes.Node, endBlockStatementSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWithEventsEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.VisitToken(node.Identifier).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (node.Identifier.Node <> identifierTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), identifierTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWithEventsPropertyEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim withEventsEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax = DirectCast(Me.Visit(node.WithEventsContainer), Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax)
			If (node.WithEventsContainer <> withEventsEventContainerSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.DotToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.DotToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(Me.Visit(node.[Property]), Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (node.[Property] <> identifierNameSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), withEventsEventContainerSyntax, punctuationSyntax, identifierNameSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.WithKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.WithKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitToken(node.EqualsToken).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			If (node.Value <> xmlNodeSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNodeSyntax, punctuationSyntax, xmlNodeSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlBracketedName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)
			If (node.Name <> xmlNameSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.GreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.GreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlCDataSection(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.BeginCDataToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.BeginCDataToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.TextTokens)
			If (node.TextTokens.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EndCDataToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EndCDataToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenLists.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlComment(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanExclamationMinusMinusToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanExclamationMinusMinusToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.TextTokens)
			If (node.TextTokens.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.MinusMinusGreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.MinusMinusGreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenLists.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlCrefAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)
			If (node.Name <> xmlNameSyntax) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.EqualsToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.StartQuoteToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.StartQuoteToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax = DirectCast(Me.Visit(node.Reference), Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax)
			If (node.Reference <> crefReferenceSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EndQuoteToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EndQuoteToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCrefAttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax1, crefReferenceSyntax, punctuationSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanQuestionToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanQuestionToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.XmlKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.XmlKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim xmlDeclarationOptionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax = DirectCast(Me.Visit(node.Version), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)
			If (node.Version <> xmlDeclarationOptionSyntax) Then
				flag = True
			End If
			Dim xmlDeclarationOptionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax = DirectCast(Me.Visit(node.Encoding), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)
			If (node.Encoding <> xmlDeclarationOptionSyntax1) Then
				flag = True
			End If
			Dim xmlDeclarationOptionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax = DirectCast(Me.Visit(node.Standalone), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax)
			If (node.Standalone <> xmlDeclarationOptionSyntax2) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.QuestionGreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.QuestionGreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, keywordSyntax, xmlDeclarationOptionSyntax, xmlDeclarationOptionSyntax1, xmlDeclarationOptionSyntax2, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlDeclarationOption(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Name)
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node.Name.Node <> xmlNameTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Equals)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Equals.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax)
			If (node.Value <> xmlStringSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax, xmlStringSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlDocument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax = DirectCast(Me.Visit(node.Declaration), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax)
			If (node.Declaration <> xmlDeclarationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(node.PrecedingMisc)
			If (node._precedingMisc <> xmlNodeSyntaxes.Node) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Root), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			If (node.Root <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntaxes1 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(node.FollowingMisc)
			If (node._followingMisc <> xmlNodeSyntaxes1.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlDeclarationSyntax, xmlNodeSyntaxes.Node, xmlNodeSyntax, xmlNodeSyntaxes1.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlElementStartTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax = DirectCast(Me.Visit(node.StartTag), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax)
			If (node.StartTag <> xmlElementStartTagSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(node.Content)
			If (node._content <> xmlNodeSyntaxes.Node) Then
				flag = True
			End If
			Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax = DirectCast(Me.Visit(node.EndTag), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax)
			If (node.EndTag <> xmlElementEndTagSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlElementStartTagSyntax, xmlNodeSyntaxes.Node, xmlElementEndTagSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlElementEndTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanSlashToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanSlashToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)
			If (node.Name <> xmlNameSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.GreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.GreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNameSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlElementStartTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(node.Attributes)
			If (node._attributes <> xmlNodeSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.GreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.GreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, xmlNodeSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlEmbeddedExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanPercentEqualsToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanPercentEqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.PercentGreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.PercentGreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, expressionSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlEmptyElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(node.Attributes)
			If (node._attributes <> xmlNodeSyntaxes.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.SlashGreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.SlashGreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNodeSyntax, xmlNodeSyntaxes.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlMemberAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Base), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Base <> expressionSyntax) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Token1)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Token1.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Token2)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Token2.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Token3)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.Token3.Node <> punctuationSyntax2) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), expressionSyntax, punctuationSyntax, punctuationSyntax1, punctuationSyntax2, xmlNodeSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax = DirectCast(Me.Visit(node.Prefix), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax)
			If (node.Prefix <> xmlPrefixSyntax) Then
				flag = True
			End If
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.VisitToken(node.LocalName).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node.LocalName.Node <> xmlNameTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlPrefixSyntax, xmlNameTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlNameAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)
			If (node.Name <> xmlNameSyntax) Then
				flag = True
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.EqualsToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EqualsToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.StartQuoteToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.StartQuoteToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(Me.Visit(node.Reference), Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (node.Reference <> identifierNameSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EndQuoteToken)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EndQuoteToken.Node <> punctuationSyntax2) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameSyntax, punctuationSyntax, punctuationSyntax1, identifierNameSyntax, punctuationSyntax2))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlNamespaceImportsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax = DirectCast(Me.Visit(node.XmlNamespace), Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax)
			If (node.XmlNamespace <> xmlAttributeSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.GreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.GreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlAttributeSyntax, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlPrefix(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.Name)
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node.Name.Node <> xmlNameTokenSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.ColonToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.ColonToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlPrefixName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.VisitToken(node.Name).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node.Name.Node <> xmlNameTokenSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), xmlNameTokenSyntax))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlProcessingInstruction(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.LessThanQuestionToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanQuestionToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.Name)
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node.Name.Node <> xmlNameTokenSyntax) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.TextTokens)
			If (node.TextTokens.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.QuestionGreaterThanToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.QuestionGreaterThanToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, xmlNameTokenSyntax, syntaxTokenLists.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlString(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me.VisitToken(node.StartQuoteToken)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.StartQuoteToken.Node <> punctuationSyntax) Then
				flag = True
			End If
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.TextTokens)
			If (node.TextTokens.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			syntaxToken = Me.VisitToken(node.EndQuoteToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(syntaxToken.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.EndQuoteToken.Node <> punctuationSyntax1) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), punctuationSyntax, syntaxTokenLists.Node, punctuationSyntax1))
			Return syntaxNode
		End Function

		Public Overrides Function VisitXmlText(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim syntaxTokenLists As SyntaxTokenList = Me.VisitList(node.TextTokens)
			If (node.TextTokens.Node <> syntaxTokenLists.Node) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), syntaxTokenLists.Node))
			Return syntaxNode
		End Function

		Public Overrides Function VisitYieldStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.YieldStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim flag As Boolean = False
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.VisitToken(node.YieldKeyword).Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node.YieldKeyword.Node <> keywordSyntax) Then
				flag = True
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (node.Expression <> expressionSyntax) Then
				flag = True
			End If
			syntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.YieldStatementSyntax(node.Kind(), node.Green.GetDiagnostics(), node.Green.GetAnnotations(), keywordSyntax, expressionSyntax))
			Return syntaxNode
		End Function
	End Class
End Namespace