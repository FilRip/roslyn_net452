Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class ContextAwareSyntaxFactory
		Private ReadOnly _factoryContext As ISyntaxFactoryContext

		Public Sub New(ByVal factoryContext As ISyntaxFactoryContext)
			MyBase.New()
			Me._factoryContext = factoryContext
		End Sub

		Friend Function AccessorBlock(ByVal kind As SyntaxKind, ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(kind, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Function AccessorStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(kind, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, Me._factoryContext)
		End Function

		Friend Function AddAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(249, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.AddAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function AddExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(307, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.AddExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function AddHandlerAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(85, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.AddHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Function AddHandlerAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.AddHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, Me._factoryContext)
		End Function

		Friend Function AddHandlerStatement(ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(SyntaxKind.AddHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression, Me._factoryContext)
		End Function

		Friend Function AddRemoveHandlerStatement(ByVal kind As SyntaxKind, ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(kind, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression, Me._factoryContext)
		End Function

		Friend Function AddressOfExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(336, operatorToken, operand, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.AddressOfExpression, operatorToken, operand, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Function AggregateClause(ByVal aggregateKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal additionalQueryOperators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(SyntaxKind.AggregateClause, aggregateKeyword, variables.Node, additionalQueryOperators.Node, intoKeyword, aggregationVariables.Node, Me._factoryContext)
		End Function

		Friend Function AggregationRangeVariable(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax, ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax
			Dim aggregationRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(355, nameEquals, aggregation, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim aggregationRangeVariableSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(SyntaxKind.AggregationRangeVariable, nameEquals, aggregation, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(aggregationRangeVariableSyntax1, num)
				End If
				aggregationRangeVariableSyntax = aggregationRangeVariableSyntax1
			Else
				aggregationRangeVariableSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)
			End If
			Return aggregationRangeVariableSyntax
		End Function

		Friend Function AndAlsoExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(332, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.AndAlsoExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function AndExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(330, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.AndExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function AnonymousObjectCreationExpression(ByVal newKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax
			Dim anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(298, newKeyword, attributeLists.Node, initializer, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim anonymousObjectCreationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(SyntaxKind.AnonymousObjectCreationExpression, newKeyword, attributeLists.Node, initializer, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(anonymousObjectCreationExpressionSyntax1, num)
				End If
				anonymousObjectCreationExpressionSyntax = anonymousObjectCreationExpressionSyntax1
			Else
				anonymousObjectCreationExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)
			End If
			Return anonymousObjectCreationExpressionSyntax
		End Function

		Friend Function ArgumentList(ByVal openParenToken As PunctuationSyntax, ByVal arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(347, openParenToken, arguments.Node, closeParenToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim argumentListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax(SyntaxKind.ArgumentList, openParenToken, arguments.Node, closeParenToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(argumentListSyntax1, num)
				End If
				argumentListSyntax = argumentListSyntax1
			Else
				argumentListSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			End If
			Return argumentListSyntax
		End Function

		Friend Function ArrayCreationExpression(ByVal newKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal rankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(SyntaxKind.ArrayCreationExpression, newKeyword, attributeLists.Node, type, arrayBounds, rankSpecifiers.Node, initializer, Me._factoryContext)
		End Function

		Friend Function ArrayRankSpecifier(ByVal openParenToken As PunctuationSyntax, ByVal commaTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax
			Dim arrayRankSpecifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(134, openParenToken, commaTokens.Node, closeParenToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim arrayRankSpecifierSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax(SyntaxKind.ArrayRankSpecifier, openParenToken, commaTokens.Node, closeParenToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(arrayRankSpecifierSyntax1, num)
				End If
				arrayRankSpecifierSyntax = arrayRankSpecifierSyntax1
			Else
				arrayRankSpecifierSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)
			End If
			Return arrayRankSpecifierSyntax
		End Function

		Friend Function ArrayType(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal rankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax
			Dim arrayTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(396, elementType, rankSpecifiers.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim arrayTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax(SyntaxKind.ArrayType, elementType, rankSpecifiers.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(arrayTypeSyntax1, num)
				End If
				arrayTypeSyntax = arrayTypeSyntax1
			Else
				arrayTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax)
			End If
			Return arrayTypeSyntax
		End Function

		Friend Function AscendingOrdering(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal ascendingOrDescendingKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(375, expression, ascendingOrDescendingKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim orderingSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(SyntaxKind.AscendingOrdering, expression, ascendingOrDescendingKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderingSyntax1, num)
				End If
				orderingSyntax = orderingSyntax1
			Else
				orderingSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			End If
			Return orderingSyntax
		End Function

		Friend Function AsNewClause(ByVal asKeyword As KeywordSyntax, ByVal newExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax
			Dim asNewClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(124, asKeyword, newExpression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim asNewClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax(SyntaxKind.AsNewClause, asKeyword, newExpression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(asNewClauseSyntax1, num)
				End If
				asNewClauseSyntax = asNewClauseSyntax1
			Else
				asNewClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax)
			End If
			Return asNewClauseSyntax
		End Function

		Friend Function AssignmentStatement(ByVal kind As SyntaxKind, ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(kind, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function Attribute(ByVal target As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax
			Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(136, target, name, argumentList, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim attributeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax(SyntaxKind.Attribute, target, name, argumentList, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributeSyntax1, num)
				End If
				attributeSyntax = attributeSyntax1
			Else
				attributeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax)
			End If
			Return attributeSyntax
		End Function

		Friend Function AttributeList(ByVal lessThanToken As PunctuationSyntax, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax
			Dim attributeListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(135, lessThanToken, attributes.Node, greaterThanToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim attributeListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax(SyntaxKind.AttributeList, lessThanToken, attributes.Node, greaterThanToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributeListSyntax1, num)
				End If
				attributeListSyntax = attributeListSyntax1
			Else
				attributeListSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			End If
			Return attributeListSyntax
		End Function

		Friend Function AttributesStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax
			Dim attributesStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(138, attributeLists.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim attributesStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax(SyntaxKind.AttributesStatement, attributeLists.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributesStatementSyntax1, num)
				End If
				attributesStatementSyntax = attributesStatementSyntax1
			Else
				attributesStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)
			End If
			Return attributesStatementSyntax
		End Function

		Friend Function AttributeTarget(ByVal attributeModifier As KeywordSyntax, ByVal colonToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax
			Dim attributeTargetSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(137, attributeModifier, colonToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim attributeTargetSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax(SyntaxKind.AttributeTarget, attributeModifier, colonToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributeTargetSyntax1, num)
				End If
				attributeTargetSyntax = attributeTargetSyntax1
			Else
				attributeTargetSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)
			End If
			Return attributeTargetSyntax
		End Function

		Friend Function AwaitExpression(ByVal awaitKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax
			Dim awaitExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(412, awaitKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim awaitExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax(SyntaxKind.AwaitExpression, awaitKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(awaitExpressionSyntax1, num)
				End If
				awaitExpressionSyntax = awaitExpressionSyntax1
			Else
				awaitExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax)
			End If
			Return awaitExpressionSyntax
		End Function

		Friend Function BadDirectiveTrivia(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax(SyntaxKind.BadDirectiveTrivia, hashToken, Me._factoryContext)
		End Function

		Friend Function BinaryConditionalExpression(ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal firstExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal secondExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax(SyntaxKind.BinaryConditionalExpression, ifKeyword, openParenToken, firstExpression, commaToken, secondExpression, closeParenToken, Me._factoryContext)
		End Function

		Friend Function BinaryExpression(ByVal kind As SyntaxKind, ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(kind, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function CallStatement(ByVal callKeyword As KeywordSyntax, ByVal invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax
			Dim callStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(261, callKeyword, invocation, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim callStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax(SyntaxKind.CallStatement, callKeyword, invocation, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(callStatementSyntax1, num)
				End If
				callStatementSyntax = callStatementSyntax1
			Else
				callStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax)
			End If
			Return callStatementSyntax
		End Function

		Friend Function CaseBlock(ByVal caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim caseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(207, caseStatement, statements.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim caseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax(SyntaxKind.CaseBlock, caseStatement, statements.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseBlockSyntax1, num)
				End If
				caseBlockSyntax = caseBlockSyntax1
			Else
				caseBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)
			End If
			Return caseBlockSyntax
		End Function

		Friend Function CaseElseBlock(ByVal caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim caseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(210, caseStatement, statements.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim caseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax(SyntaxKind.CaseElseBlock, caseStatement, statements.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseBlockSyntax1, num)
				End If
				caseBlockSyntax = caseBlockSyntax1
			Else
				caseBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)
			End If
			Return caseBlockSyntax
		End Function

		Friend Function CaseElseStatement(ByVal caseKeyword As KeywordSyntax, ByVal cases As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(212, caseKeyword, cases.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim caseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax(SyntaxKind.CaseElseStatement, caseKeyword, cases.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseStatementSyntax1, num)
				End If
				caseStatementSyntax = caseStatementSyntax1
			Else
				caseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)
			End If
			Return caseStatementSyntax
		End Function

		Friend Function CaseEqualsClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(216, isKeyword, operatorToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseEqualsClause, isKeyword, operatorToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Function CaseGreaterThanClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(223, isKeyword, operatorToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanClause, isKeyword, operatorToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Function CaseGreaterThanOrEqualClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(222, isKeyword, operatorToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanOrEqualClause, isKeyword, operatorToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Function CaseLessThanClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(218, isKeyword, operatorToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanClause, isKeyword, operatorToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Function CaseLessThanOrEqualClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(219, isKeyword, operatorToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanOrEqualClause, isKeyword, operatorToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Function CaseNotEqualsClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(217, isKeyword, operatorToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseNotEqualsClause, isKeyword, operatorToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Function CaseStatement(ByVal caseKeyword As KeywordSyntax, ByVal cases As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(211, caseKeyword, cases.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim caseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax(SyntaxKind.CaseStatement, caseKeyword, cases.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseStatementSyntax1, num)
				End If
				caseStatementSyntax = caseStatementSyntax1
			Else
				caseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)
			End If
			Return caseStatementSyntax
		End Function

		Friend Function CatchBlock(ByVal catchStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax
			Dim catchBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(187, catchStatement, statements.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim catchBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax(SyntaxKind.CatchBlock, catchStatement, statements.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(catchBlockSyntax1, num)
				End If
				catchBlockSyntax = catchBlockSyntax1
			Else
				catchBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax)
			End If
			Return catchBlockSyntax
		End Function

		Friend Function CatchFilterClause(ByVal whenKeyword As KeywordSyntax, ByVal filter As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax
			Dim catchFilterClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(191, whenKeyword, filter, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim catchFilterClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax(SyntaxKind.CatchFilterClause, whenKeyword, filter, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(catchFilterClauseSyntax1, num)
				End If
				catchFilterClauseSyntax = catchFilterClauseSyntax1
			Else
				catchFilterClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax)
			End If
			Return catchFilterClauseSyntax
		End Function

		Friend Function CatchStatement(ByVal catchKeyword As KeywordSyntax, ByVal identifierName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal whenClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax(SyntaxKind.CatchStatement, catchKeyword, identifierName, asClause, whenClause, Me._factoryContext)
		End Function

		Friend Function CharacterLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(272, token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.CharacterLiteralExpression, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function CharacterLiteralToken(ByVal text As String, ByVal value As Char, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As CharacterLiteralTokenSyntax
			Return New CharacterLiteralTokenSyntax(SyntaxKind.CharacterLiteralToken, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function ClassBlock(ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(SyntaxKind.ClassBlock, classStatement, [inherits].Node, [implements].Node, members.Node, endClassStatement, Me._factoryContext)
		End Function

		Friend Function ClassConstraint(ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(73, constraintKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(SyntaxKind.ClassConstraint, constraintKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Function ClassStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal classKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax(SyntaxKind.ClassStatement, attributeLists.Node, modifiers.Node, classKeyword, identifier, typeParameterList, Me._factoryContext)
		End Function

		Friend Function CollectionInitializer(ByVal openBraceToken As PunctuationSyntax, ByVal initializers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(302, openBraceToken, initializers.Node, closeBraceToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim collectionInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(SyntaxKind.CollectionInitializer, openBraceToken, initializers.Node, closeBraceToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(collectionInitializerSyntax1, num)
				End If
				collectionInitializerSyntax = collectionInitializerSyntax1
			Else
				collectionInitializerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)
			End If
			Return collectionInitializerSyntax
		End Function

		Friend Function CollectionRangeVariable(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax(SyntaxKind.CollectionRangeVariable, identifier, asClause, inKeyword, expression, Me._factoryContext)
		End Function

		Friend Function ColonTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.ColonTrivia, text, Me._factoryContext)
		End Function

		Friend Function CommentTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.CommentTrivia, text, Me._factoryContext)
		End Function

		Friend Function CompilationUnit(ByVal options As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [imports] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endOfFileToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(SyntaxKind.CompilationUnit, options.Node, [imports].Node, attributes.Node, members.Node, endOfFileToken, Me._factoryContext)
		End Function

		Friend Function ConcatenateAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(259, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.ConcatenateAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function ConcatenateExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(317, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ConcatenateExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function ConditionalAccessExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal questionMarkToken As PunctuationSyntax, ByVal whenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax
			Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(713, expression, questionMarkToken, whenNotNull, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim conditionalAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax(SyntaxKind.ConditionalAccessExpression, expression, questionMarkToken, whenNotNull, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(conditionalAccessExpressionSyntax1, num)
				End If
				conditionalAccessExpressionSyntax = conditionalAccessExpressionSyntax1
			Else
				conditionalAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)
			End If
			Return conditionalAccessExpressionSyntax
		End Function

		Friend Function ConflictMarkerTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.ConflictMarkerTrivia, text, Me._factoryContext)
		End Function

		Friend Function ConstDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal constKeyword As KeywordSyntax, ByVal name As IdentifierTokenSyntax, ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax(SyntaxKind.ConstDirectiveTrivia, hashToken, constKeyword, name, equalsToken, value, Me._factoryContext)
		End Function

		Friend Function ConstructorBlock(ByVal subNewStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax
			Dim constructorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(81, subNewStatement, statements.Node, endSubStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim constructorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax(SyntaxKind.ConstructorBlock, subNewStatement, statements.Node, endSubStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(constructorBlockSyntax1, num)
				End If
				constructorBlockSyntax = constructorBlockSyntax1
			Else
				constructorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax)
			End If
			Return constructorBlockSyntax
		End Function

		Friend Function ContinueDoStatement(ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(167, continueKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(SyntaxKind.ContinueDoStatement, continueKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Function ContinueForStatement(ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(168, continueKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(SyntaxKind.ContinueForStatement, continueKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Function ContinueStatement(ByVal kind As SyntaxKind, ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), continueKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(kind, continueKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Function ContinueWhileStatement(ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(166, continueKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(SyntaxKind.ContinueWhileStatement, continueKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Function CrefOperatorReference(ByVal operatorKeyword As KeywordSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax
			Dim crefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(409, operatorKeyword, operatorToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim crefOperatorReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax(SyntaxKind.CrefOperatorReference, operatorKeyword, operatorToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefOperatorReferenceSyntax1, num)
				End If
				crefOperatorReferenceSyntax = crefOperatorReferenceSyntax1
			Else
				crefOperatorReferenceSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)
			End If
			Return crefOperatorReferenceSyntax
		End Function

		Friend Function CrefReference(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax
			Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(404, name, signature, asClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim crefReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(SyntaxKind.CrefReference, name, signature, asClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefReferenceSyntax1, num)
				End If
				crefReferenceSyntax = crefReferenceSyntax1
			Else
				crefReferenceSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax)
			End If
			Return crefReferenceSyntax
		End Function

		Friend Function CrefSignature(ByVal openParenToken As PunctuationSyntax, ByVal argumentTypes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax
			Dim crefSignatureSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(407, openParenToken, argumentTypes.Node, closeParenToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim crefSignatureSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax(SyntaxKind.CrefSignature, openParenToken, argumentTypes.Node, closeParenToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefSignatureSyntax1, num)
				End If
				crefSignatureSyntax = crefSignatureSyntax1
			Else
				crefSignatureSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)
			End If
			Return crefSignatureSyntax
		End Function

		Friend Function CrefSignaturePart(ByVal modifier As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax
			Dim crefSignaturePartSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(408, modifier, type, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim crefSignaturePartSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax(SyntaxKind.CrefSignaturePart, modifier, type, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefSignaturePartSyntax1, num)
				End If
				crefSignaturePartSyntax = crefSignaturePartSyntax1
			Else
				crefSignaturePartSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax)
			End If
			Return crefSignaturePartSyntax
		End Function

		Friend Function CTypeExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax(SyntaxKind.CTypeExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken, Me._factoryContext)
		End Function

		Friend Function DateLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(276, token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.DateLiteralExpression, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function DateLiteralToken(ByVal text As String, ByVal value As DateTime, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As DateLiteralTokenSyntax
			Return New DateLiteralTokenSyntax(SyntaxKind.DateLiteralToken, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function DecimalLiteralToken(ByVal text As String, ByVal typeSuffix As TypeCharacter, ByVal value As [Decimal], ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As DecimalLiteralTokenSyntax
			Return New DecimalLiteralTokenSyntax(SyntaxKind.DecimalLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, value, Me._factoryContext)
		End Function

		Friend Function DeclareFunctionStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(SyntaxKind.DeclareFunctionStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function DeclareStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(kind, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function DeclareSubStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(SyntaxKind.DeclareSubStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function DelegateFunctionStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal delegateKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(SyntaxKind.DelegateFunctionStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function DelegateStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal delegateKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(kind, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function DelegateSubStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal delegateKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(SyntaxKind.DelegateSubStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function DescendingOrdering(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal ascendingOrDescendingKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(376, expression, ascendingOrDescendingKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim orderingSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(SyntaxKind.DescendingOrdering, expression, ascendingOrDescendingKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderingSyntax1, num)
				End If
				orderingSyntax = orderingSyntax1
			Else
				orderingSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			End If
			Return orderingSyntax
		End Function

		Friend Function DictionaryAccessExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(292, expression, operatorToken, name, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim memberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(SyntaxKind.DictionaryAccessExpression, expression, operatorToken, name, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(memberAccessExpressionSyntax1, num)
				End If
				memberAccessExpressionSyntax = memberAccessExpressionSyntax1
			Else
				memberAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
			End If
			Return memberAccessExpressionSyntax
		End Function

		Friend Function DirectCastExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax(SyntaxKind.DirectCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken, Me._factoryContext)
		End Function

		Friend Function DisabledTextTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.DisabledTextTrivia, text, Me._factoryContext)
		End Function

		Friend Function DisableWarningDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal disableKeyword As KeywordSyntax, ByVal warningKeyword As KeywordSyntax, ByVal errorCodes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax(SyntaxKind.DisableWarningDirectiveTrivia, hashToken, disableKeyword, warningKeyword, errorCodes.Node, Me._factoryContext)
		End Function

		Friend Function DistinctClause(ByVal distinctKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax
			Dim distinctClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(362, distinctKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim distinctClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax(SyntaxKind.DistinctClause, distinctKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(distinctClauseSyntax1, num)
				End If
				distinctClauseSyntax = distinctClauseSyntax1
			Else
				distinctClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax)
			End If
			Return distinctClauseSyntax
		End Function

		Friend Function DivideAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(252, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.DivideAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function DivideExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(310, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.DivideExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function DocumentationCommentExteriorTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, text, Me._factoryContext)
		End Function

		Friend Function DocumentationCommentLineBreakToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(SyntaxKind.DocumentationCommentLineBreakToken, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function DocumentationCommentTrivia(ByVal content As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax(SyntaxKind.DocumentationCommentTrivia, content.Node, Me._factoryContext)
		End Function

		Friend Function DoLoopBlock(ByVal kind As SyntaxKind, ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), doStatement, statements.Node, loopStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(kind, doStatement, statements.Node, loopStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Function DoLoopUntilBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(760, doStatement, statements.Node, loopStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoLoopUntilBlock, doStatement, statements.Node, loopStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Function DoLoopWhileBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(759, doStatement, statements.Node, loopStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoLoopWhileBlock, doStatement, statements.Node, loopStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Function DoStatement(ByVal kind As SyntaxKind, ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), doKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(kind, doKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Function DoUntilLoopBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(758, doStatement, statements.Node, loopStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoUntilLoopBlock, doStatement, statements.Node, loopStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Function DoUntilStatement(ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(772, doKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(SyntaxKind.DoUntilStatement, doKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Function DoWhileLoopBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(757, doStatement, statements.Node, loopStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoWhileLoopBlock, doStatement, statements.Node, loopStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Function DoWhileStatement(ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(771, doKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(SyntaxKind.DoWhileStatement, doKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Function ElseBlock(ByVal elseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax
			Dim elseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(181, elseStatement, statements.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim elseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax(SyntaxKind.ElseBlock, elseStatement, statements.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseBlockSyntax1, num)
				End If
				elseBlockSyntax = elseBlockSyntax1
			Else
				elseBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax)
			End If
			Return elseBlockSyntax
		End Function

		Friend Function ElseCaseClause(ByVal elseKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax
			Dim elseCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(213, elseKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim elseCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax(SyntaxKind.ElseCaseClause, elseKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseCaseClauseSyntax1, num)
				End If
				elseCaseClauseSyntax = elseCaseClauseSyntax1
			Else
				elseCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax)
			End If
			Return elseCaseClauseSyntax
		End Function

		Friend Function ElseDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax(SyntaxKind.ElseDirectiveTrivia, hashToken, elseKeyword, Me._factoryContext)
		End Function

		Friend Function ElseIfBlock(ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax
			Dim elseIfBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(180, elseIfStatement, statements.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim elseIfBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(SyntaxKind.ElseIfBlock, elseIfStatement, statements.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseIfBlockSyntax1, num)
				End If
				elseIfBlockSyntax = elseIfBlockSyntax1
			Else
				elseIfBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax)
			End If
			Return elseIfBlockSyntax
		End Function

		Friend Function ElseIfDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(SyntaxKind.ElseIfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword, Me._factoryContext)
		End Function

		Friend Function ElseIfStatement(ByVal elseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax
			Dim elseIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(183, elseIfKeyword, condition, thenKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim elseIfStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax(SyntaxKind.ElseIfStatement, elseIfKeyword, condition, thenKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseIfStatementSyntax1, num)
				End If
				elseIfStatementSyntax = elseIfStatementSyntax1
			Else
				elseIfStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax)
			End If
			Return elseIfStatementSyntax
		End Function

		Friend Function ElseStatement(ByVal elseKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax
			Dim elseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(184, elseKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim elseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax(SyntaxKind.ElseStatement, elseKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseStatementSyntax1, num)
				End If
				elseStatementSyntax = elseStatementSyntax1
			Else
				elseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax)
			End If
			Return elseStatementSyntax
		End Function

		Friend Function EmptyStatement(ByVal empty As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax
			Dim emptyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(2, empty, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim emptyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax(SyntaxKind.EmptyStatement, empty, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(emptyStatementSyntax1, num)
				End If
				emptyStatementSyntax = emptyStatementSyntax1
			Else
				emptyStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax)
			End If
			Return emptyStatementSyntax
		End Function

		Friend Function EnableWarningDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal enableKeyword As KeywordSyntax, ByVal warningKeyword As KeywordSyntax, ByVal errorCodes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax(SyntaxKind.EnableWarningDirectiveTrivia, hashToken, enableKeyword, warningKeyword, errorCodes.Node, Me._factoryContext)
		End Function

		Friend Function EndAddHandlerStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(22, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndAddHandlerStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndBlockStatement(ByVal kind As SyntaxKind, ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(kind, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndClassStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(12, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndClassStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndEnumStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(10, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndEnumStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndEventStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(21, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndEventStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndExternalSourceDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal externalSourceKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax(SyntaxKind.EndExternalSourceDirectiveTrivia, hashToken, endKeyword, externalSourceKeyword, Me._factoryContext)
		End Function

		Friend Function EndFunctionStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(16, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndFunctionStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndGetStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(17, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndGetStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndIfDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal ifKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax(SyntaxKind.EndIfDirectiveTrivia, hashToken, endKeyword, ifKeyword, Me._factoryContext)
		End Function

		Friend Function EndIfStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(5, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndIfStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndInterfaceStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(11, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndInterfaceStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndModuleStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(13, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndModuleStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndNamespaceStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(14, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndNamespaceStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndOfLineTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, text, Me._factoryContext)
		End Function

		Friend Function EndOperatorStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(20, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndOperatorStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndPropertyStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(19, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndPropertyStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndRaiseEventStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(24, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndRaiseEventStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndRegionDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal regionKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax(SyntaxKind.EndRegionDirectiveTrivia, hashToken, endKeyword, regionKeyword, Me._factoryContext)
		End Function

		Friend Function EndRemoveHandlerStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(23, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndRemoveHandlerStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndSelectStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(8, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSelectStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndSetStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(18, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSetStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndStatement(ByVal stopOrEndKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim stopOrEndStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(156, stopOrEndKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim stopOrEndStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(SyntaxKind.EndStatement, stopOrEndKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(stopOrEndStatementSyntax1, num)
				End If
				stopOrEndStatementSyntax = stopOrEndStatementSyntax1
			Else
				stopOrEndStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)
			End If
			Return stopOrEndStatementSyntax
		End Function

		Friend Function EndStructureStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(9, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndStructureStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndSubStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(15, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSubStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndSyncLockStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(27, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSyncLockStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndTryStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(26, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndTryStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndUsingStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(6, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndUsingStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndWhileStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(25, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndWhileStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EndWithStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(7, endKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndWithStatement, endKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Function EnumBlock(ByVal enumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax, ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endEnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax
			Dim enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(54, enumStatement, members.Node, endEnumStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim enumBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax(SyntaxKind.EnumBlock, enumStatement, members.Node, endEnumStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(enumBlockSyntax1, num)
				End If
				enumBlockSyntax = enumBlockSyntax1
			Else
				enumBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax)
			End If
			Return enumBlockSyntax
		End Function

		Friend Function EnumMemberDeclaration(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal identifier As IdentifierTokenSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax
			Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(78, attributeLists.Node, identifier, initializer, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim enumMemberDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax(SyntaxKind.EnumMemberDeclaration, attributeLists.Node, identifier, initializer, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(enumMemberDeclarationSyntax1, num)
				End If
				enumMemberDeclarationSyntax = enumMemberDeclarationSyntax1
			Else
				enumMemberDeclarationSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax)
			End If
			Return enumMemberDeclarationSyntax
		End Function

		Friend Function EnumStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal enumKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal underlyingType As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax(SyntaxKind.EnumStatement, attributeLists.Node, modifiers.Node, enumKeyword, identifier, underlyingType, Me._factoryContext)
		End Function

		Friend Function EqualsExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(319, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.EqualsExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function EqualsValue(ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(129, equalsToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim equalsValueSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax(SyntaxKind.EqualsValue, equalsToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(equalsValueSyntax1, num)
				End If
				equalsValueSyntax = equalsValueSyntax1
			Else
				equalsValueSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			End If
			Return equalsValueSyntax
		End Function

		Friend Function EraseStatement(ByVal eraseKeyword As KeywordSyntax, ByVal expressions As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax
			Dim eraseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(271, eraseKeyword, expressions.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim eraseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax(SyntaxKind.EraseStatement, eraseKeyword, expressions.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(eraseStatementSyntax1, num)
				End If
				eraseStatementSyntax = eraseStatementSyntax1
			Else
				eraseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax)
			End If
			Return eraseStatementSyntax
		End Function

		Friend Function ErrorStatement(ByVal errorKeyword As KeywordSyntax, ByVal errorNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax
			Dim errorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(195, errorKeyword, errorNumber, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim errorStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax(SyntaxKind.ErrorStatement, errorKeyword, errorNumber, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(errorStatementSyntax1, num)
				End If
				errorStatementSyntax = errorStatementSyntax1
			Else
				errorStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax)
			End If
			Return errorStatementSyntax
		End Function

		Friend Function EventBlock(ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax, ByVal accessors As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax
			Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(89, eventStatement, accessors.Node, endEventStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim eventBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(SyntaxKind.EventBlock, eventStatement, accessors.Node, endEventStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(eventBlockSyntax1, num)
				End If
				eventBlockSyntax = eventBlockSyntax1
			Else
				eventBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax)
			End If
			Return eventBlockSyntax
		End Function

		Friend Function EventStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal customKeyword As KeywordSyntax, ByVal eventKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(SyntaxKind.EventStatement, attributeLists.Node, modifiers.Node, customKeyword, eventKeyword, identifier, parameterList, asClause, implementsClause, Me._factoryContext)
		End Function

		Friend Function ExclusiveOrExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(329, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ExclusiveOrExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function ExitDoStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(157, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitDoStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitForStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(158, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitForStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitFunctionStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(160, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitFunctionStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitOperatorStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(161, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitOperatorStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitPropertyStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(162, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitPropertyStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitSelectStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(164, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitSelectStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitStatement(ByVal kind As SyntaxKind, ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(kind, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitSubStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(159, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitSubStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitTryStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(163, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitTryStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExitWhileStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(165, exitKeyword, blockKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitWhileStatement, exitKeyword, blockKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Function ExponentiateAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(254, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.ExponentiateAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function ExponentiateExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(314, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ExponentiateExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function ExpressionRangeVariable(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax
			Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(354, nameEquals, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim expressionRangeVariableSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax(SyntaxKind.ExpressionRangeVariable, nameEquals, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(expressionRangeVariableSyntax1, num)
				End If
				expressionRangeVariableSyntax = expressionRangeVariableSyntax1
			Else
				expressionRangeVariableSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)
			End If
			Return expressionRangeVariableSyntax
		End Function

		Friend Function ExpressionStatement(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax
			Dim expressionStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(139, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim expressionStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax(SyntaxKind.ExpressionStatement, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(expressionStatementSyntax1, num)
				End If
				expressionStatementSyntax = expressionStatementSyntax1
			Else
				expressionStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax)
			End If
			Return expressionStatementSyntax
		End Function

		Friend Function ExternalChecksumDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal externalChecksumKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal guid As StringLiteralTokenSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal checksum As StringLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(SyntaxKind.ExternalChecksumDirectiveTrivia, hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken, Me._factoryContext)
		End Function

		Friend Function ExternalSourceDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal externalSourceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal commaToken As PunctuationSyntax, ByVal lineStart As IntegerLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(SyntaxKind.ExternalSourceDirectiveTrivia, hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken, Me._factoryContext)
		End Function

		Friend Function FalseLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(274, token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function FieldDeclaration(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal declarators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax
			Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(119, attributeLists.Node, modifiers.Node, declarators.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim fieldDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax(SyntaxKind.FieldDeclaration, attributeLists.Node, modifiers.Node, declarators.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(fieldDeclarationSyntax1, num)
				End If
				fieldDeclarationSyntax = fieldDeclarationSyntax1
			Else
				fieldDeclarationSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)
			End If
			Return fieldDeclarationSyntax
		End Function

		Friend Function FinallyBlock(ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax
			Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(188, finallyStatement, statements.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim finallyBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(SyntaxKind.FinallyBlock, finallyStatement, statements.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(finallyBlockSyntax1, num)
				End If
				finallyBlockSyntax = finallyBlockSyntax1
			Else
				finallyBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax)
			End If
			Return finallyBlockSyntax
		End Function

		Friend Function FinallyStatement(ByVal finallyKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax
			Dim finallyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(194, finallyKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim finallyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax(SyntaxKind.FinallyStatement, finallyKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(finallyStatementSyntax1, num)
				End If
				finallyStatementSyntax = finallyStatementSyntax1
			Else
				finallyStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax)
			End If
			Return finallyStatementSyntax
		End Function

		Friend Function ForBlock(ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax
			Dim forBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(237, forStatement, statements.Node, nextStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim forBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(SyntaxKind.ForBlock, forStatement, statements.Node, nextStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(forBlockSyntax1, num)
				End If
				forBlockSyntax = forBlockSyntax1
			Else
				forBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax)
			End If
			Return forBlockSyntax
		End Function

		Friend Function ForEachBlock(ByVal forEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax
			Dim forEachBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(238, forEachStatement, statements.Node, nextStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim forEachBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax(SyntaxKind.ForEachBlock, forEachStatement, statements.Node, nextStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(forEachBlockSyntax1, num)
				End If
				forEachBlockSyntax = forEachBlockSyntax1
			Else
				forEachBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax)
			End If
			Return forEachBlockSyntax
		End Function

		Friend Function ForEachStatement(ByVal forKeyword As KeywordSyntax, ByVal eachKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(SyntaxKind.ForEachStatement, forKeyword, eachKeyword, controlVariable, inKeyword, expression, Me._factoryContext)
		End Function

		Friend Function ForStatement(ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal equalsToken As PunctuationSyntax, ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(SyntaxKind.ForStatement, forKeyword, controlVariable, equalsToken, fromValue, toKeyword, toValue, stepClause, Me._factoryContext)
		End Function

		Friend Function ForStepClause(ByVal stepKeyword As KeywordSyntax, ByVal stepValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax
			Dim forStepClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(240, stepKeyword, stepValue, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim forStepClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax(SyntaxKind.ForStepClause, stepKeyword, stepValue, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(forStepClauseSyntax1, num)
				End If
				forStepClauseSyntax = forStepClauseSyntax1
			Else
				forStepClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)
			End If
			Return forStepClauseSyntax
		End Function

		Friend Function FromClause(ByVal fromKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax
			Dim fromClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(359, fromKeyword, variables.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim fromClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(SyntaxKind.FromClause, fromKeyword, variables.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(fromClauseSyntax1, num)
				End If
				fromClauseSyntax = fromClauseSyntax1
			Else
				fromClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax)
			End If
			Return fromClauseSyntax
		End Function

		Friend Function FunctionAggregation(ByVal functionName As IdentifierTokenSyntax, ByVal openParenToken As PunctuationSyntax, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax(SyntaxKind.FunctionAggregation, functionName, openParenToken, argument, closeParenToken, Me._factoryContext)
		End Function

		Friend Function FunctionBlock(ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim methodBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(80, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim methodBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(SyntaxKind.FunctionBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(methodBlockSyntax1, num)
				End If
				methodBlockSyntax = methodBlockSyntax1
			Else
				methodBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax)
			End If
			Return methodBlockSyntax
		End Function

		Friend Function FunctionLambdaHeader(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(SyntaxKind.FunctionLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function FunctionStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(SyntaxKind.FunctionStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause, Me._factoryContext)
		End Function

		Friend Function GenericName(ByVal identifier As IdentifierTokenSyntax, ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax
			Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(400, identifier, typeArgumentList, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim genericNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(SyntaxKind.GenericName, identifier, typeArgumentList, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(genericNameSyntax1, num)
				End If
				genericNameSyntax = genericNameSyntax1
			Else
				genericNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax)
			End If
			Return genericNameSyntax
		End Function

		Friend Function GetAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(83, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.GetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Function GetAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.GetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, Me._factoryContext)
		End Function

		Friend Function GetTypeExpression(ByVal getTypeKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax(SyntaxKind.GetTypeExpression, getTypeKeyword, openParenToken, type, closeParenToken, Me._factoryContext)
		End Function

		Friend Function GetXmlNamespaceExpression(ByVal getXmlNamespaceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(SyntaxKind.GetXmlNamespaceExpression, getXmlNamespaceKeyword, openParenToken, name, closeParenToken, Me._factoryContext)
		End Function

		Friend Function GlobalName(ByVal globalKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax
			Dim globalNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(402, globalKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim globalNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax(SyntaxKind.GlobalName, globalKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(globalNameSyntax1, num)
				End If
				globalNameSyntax = globalNameSyntax1
			Else
				globalNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax)
			End If
			Return globalNameSyntax
		End Function

		Friend Function GoToStatement(ByVal goToKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax
			Dim goToStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(149, goToKeyword, label, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim goToStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax(SyntaxKind.GoToStatement, goToKeyword, label, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(goToStatementSyntax1, num)
				End If
				goToStatementSyntax = goToStatementSyntax1
			Else
				goToStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax)
			End If
			Return goToStatementSyntax
		End Function

		Friend Function GreaterThanExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(324, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.GreaterThanExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function GreaterThanOrEqualExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(323, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.GreaterThanOrEqualExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function GroupAggregation(ByVal groupKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax
			Dim groupAggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(358, groupKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim groupAggregationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax(SyntaxKind.GroupAggregation, groupKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(groupAggregationSyntax1, num)
				End If
				groupAggregationSyntax = groupAggregationSyntax1
			Else
				groupAggregationSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax)
			End If
			Return groupAggregationSyntax
		End Function

		Friend Function GroupByClause(ByVal groupKeyword As KeywordSyntax, ByVal items As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal byKeyword As KeywordSyntax, ByVal keys As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(SyntaxKind.GroupByClause, groupKeyword, items.Node, byKeyword, keys.Node, intoKeyword, aggregationVariables.Node, Me._factoryContext)
		End Function

		Friend Function GroupJoinClause(ByVal groupKeyword As KeywordSyntax, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal additionalJoins As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal onKeyword As KeywordSyntax, ByVal joinConditions As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(SyntaxKind.GroupJoinClause, groupKeyword, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node, intoKeyword, aggregationVariables.Node, Me._factoryContext)
		End Function

		Friend Function HandlesClause(ByVal handlesKeyword As KeywordSyntax, ByVal events As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(113, handlesKeyword, events.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim handlesClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax(SyntaxKind.HandlesClause, handlesKeyword, events.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(handlesClauseSyntax1, num)
				End If
				handlesClauseSyntax = handlesClauseSyntax1
			Else
				handlesClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)
			End If
			Return handlesClauseSyntax
		End Function

		Friend Function HandlesClauseItem(ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax
			Dim handlesClauseItemSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(117, eventContainer, dotToken, eventMember, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim handlesClauseItemSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(SyntaxKind.HandlesClauseItem, eventContainer, dotToken, eventMember, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(handlesClauseItemSyntax1, num)
				End If
				handlesClauseItemSyntax = handlesClauseItemSyntax1
			Else
				handlesClauseItemSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)
			End If
			Return handlesClauseItemSyntax
		End Function

		Friend Function IdentifierLabel(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(150, labelToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(SyntaxKind.IdentifierLabel, labelToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Function IdentifierName(ByVal identifier As IdentifierTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(399, identifier, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim identifierNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax(SyntaxKind.IdentifierName, identifier, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(identifierNameSyntax1, num)
				End If
				identifierNameSyntax = identifierNameSyntax1
			Else
				identifierNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			End If
			Return identifierNameSyntax
		End Function

		Friend Function IfDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(SyntaxKind.IfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword, Me._factoryContext)
		End Function

		Friend Function IfStatement(ByVal ifKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax
			Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(182, ifKeyword, condition, thenKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim ifStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax(SyntaxKind.IfStatement, ifKeyword, condition, thenKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(ifStatementSyntax1, num)
				End If
				ifStatementSyntax = ifStatementSyntax1
			Else
				ifStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
			End If
			Return ifStatementSyntax
		End Function

		Friend Function ImplementsClause(ByVal implementsKeyword As KeywordSyntax, ByVal interfaceMembers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(112, implementsKeyword, interfaceMembers.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim implementsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax(SyntaxKind.ImplementsClause, implementsKeyword, interfaceMembers.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(implementsClauseSyntax1, num)
				End If
				implementsClauseSyntax = implementsClauseSyntax1
			Else
				implementsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			End If
			Return implementsClauseSyntax
		End Function

		Friend Function ImplementsStatement(ByVal implementsKeyword As KeywordSyntax, ByVal types As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax
			Dim implementsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(58, implementsKeyword, types.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim implementsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax(SyntaxKind.ImplementsStatement, implementsKeyword, types.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(implementsStatementSyntax1, num)
				End If
				implementsStatementSyntax = implementsStatementSyntax1
			Else
				implementsStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)
			End If
			Return implementsStatementSyntax
		End Function

		Friend Function ImportAliasClause(ByVal identifier As IdentifierTokenSyntax, ByVal equalsToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax
			Dim importAliasClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(754, identifier, equalsToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim importAliasClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax(SyntaxKind.ImportAliasClause, identifier, equalsToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(importAliasClauseSyntax1, num)
				End If
				importAliasClauseSyntax = importAliasClauseSyntax1
			Else
				importAliasClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)
			End If
			Return importAliasClauseSyntax
		End Function

		Friend Function ImportsStatement(ByVal importsKeyword As KeywordSyntax, ByVal importsClauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax
			Dim importsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(42, importsKeyword, importsClauses.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim importsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax(SyntaxKind.ImportsStatement, importsKeyword, importsClauses.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(importsStatementSyntax1, num)
				End If
				importsStatementSyntax = importsStatementSyntax1
			Else
				importsStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)
			End If
			Return importsStatementSyntax
		End Function

		Friend Function IncompleteMember(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal missingIdentifier As IdentifierTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax
			Dim incompleteMemberSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(118, attributeLists.Node, modifiers.Node, missingIdentifier, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim incompleteMemberSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(SyntaxKind.IncompleteMember, attributeLists.Node, modifiers.Node, missingIdentifier, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(incompleteMemberSyntax1, num)
				End If
				incompleteMemberSyntax = incompleteMemberSyntax1
			Else
				incompleteMemberSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax)
			End If
			Return incompleteMemberSyntax
		End Function

		Friend Function InferredFieldInitializer(ByVal keyKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax
			Dim inferredFieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(127, keyKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim inferredFieldInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax(SyntaxKind.InferredFieldInitializer, keyKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(inferredFieldInitializerSyntax1, num)
				End If
				inferredFieldInitializerSyntax = inferredFieldInitializerSyntax1
			Else
				inferredFieldInitializerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax)
			End If
			Return inferredFieldInitializerSyntax
		End Function

		Friend Function InheritsStatement(ByVal inheritsKeyword As KeywordSyntax, ByVal types As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax
			Dim inheritsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(57, inheritsKeyword, types.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim inheritsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax(SyntaxKind.InheritsStatement, inheritsKeyword, types.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(inheritsStatementSyntax1, num)
				End If
				inheritsStatementSyntax = inheritsStatementSyntax1
			Else
				inheritsStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)
			End If
			Return inheritsStatementSyntax
		End Function

		Friend Function IntegerDivideAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(253, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.IntegerDivideAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function IntegerDivideExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(311, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.IntegerDivideExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function InterfaceBlock(ByVal interfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endInterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(SyntaxKind.InterfaceBlock, interfaceStatement, [inherits].Node, [implements].Node, members.Node, endInterfaceStatement, Me._factoryContext)
		End Function

		Friend Function InterfaceStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal interfaceKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax(SyntaxKind.InterfaceStatement, attributeLists.Node, modifiers.Node, interfaceKeyword, identifier, typeParameterList, Me._factoryContext)
		End Function

		Friend Function InterpolatedStringExpression(ByVal dollarSignDoubleQuoteToken As PunctuationSyntax, ByVal contents As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal doubleQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax
			Dim interpolatedStringExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(780, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim interpolatedStringExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(SyntaxKind.InterpolatedStringExpression, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolatedStringExpressionSyntax1, num)
				End If
				interpolatedStringExpressionSyntax = interpolatedStringExpressionSyntax1
			Else
				interpolatedStringExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)
			End If
			Return interpolatedStringExpressionSyntax
		End Function

		Friend Function InterpolatedStringText(ByVal textToken As InterpolatedStringTextTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax
			Dim interpolatedStringTextSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(781, textToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim interpolatedStringTextSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(SyntaxKind.InterpolatedStringText, textToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolatedStringTextSyntax1, num)
				End If
				interpolatedStringTextSyntax = interpolatedStringTextSyntax1
			Else
				interpolatedStringTextSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax)
			End If
			Return interpolatedStringTextSyntax
		End Function

		Friend Function InterpolatedStringTextToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As InterpolatedStringTextTokenSyntax
			Return New InterpolatedStringTextTokenSyntax(SyntaxKind.InterpolatedStringTextToken, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function Interpolation(ByVal openBraceToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax, ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax, ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(SyntaxKind.Interpolation, openBraceToken, expression, alignmentClause, formatClause, closeBraceToken, Me._factoryContext)
		End Function

		Friend Function InterpolationAlignmentClause(ByVal commaToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax
			Dim interpolationAlignmentClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(783, commaToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim interpolationAlignmentClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax(SyntaxKind.InterpolationAlignmentClause, commaToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolationAlignmentClauseSyntax1, num)
				End If
				interpolationAlignmentClauseSyntax = interpolationAlignmentClauseSyntax1
			Else
				interpolationAlignmentClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)
			End If
			Return interpolationAlignmentClauseSyntax
		End Function

		Friend Function InterpolationFormatClause(ByVal colonToken As PunctuationSyntax, ByVal formatStringToken As InterpolatedStringTextTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax
			Dim interpolationFormatClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(784, colonToken, formatStringToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim interpolationFormatClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax(SyntaxKind.InterpolationFormatClause, colonToken, formatStringToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolationFormatClauseSyntax1, num)
				End If
				interpolationFormatClauseSyntax = interpolationFormatClauseSyntax1
			Else
				interpolationFormatClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)
			End If
			Return interpolationFormatClauseSyntax
		End Function

		Friend Function InvocationExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax
			Dim invocationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(296, expression, argumentList, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim invocationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax(SyntaxKind.InvocationExpression, expression, argumentList, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(invocationExpressionSyntax1, num)
				End If
				invocationExpressionSyntax = invocationExpressionSyntax1
			Else
				invocationExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax)
			End If
			Return invocationExpressionSyntax
		End Function

		Friend Function IsExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(325, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.IsExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function IsNotExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(326, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.IsNotExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function JoinCondition(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal equalsKeyword As KeywordSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax
			Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(369, left, equalsKeyword, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim joinConditionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax(SyntaxKind.JoinCondition, left, equalsKeyword, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(joinConditionSyntax1, num)
				End If
				joinConditionSyntax = joinConditionSyntax1
			Else
				joinConditionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)
			End If
			Return joinConditionSyntax
		End Function

		Friend Function KeywordEventContainer(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax
			Dim keywordEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(114, keyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim keywordEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax(SyntaxKind.KeywordEventContainer, keyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(keywordEventContainerSyntax1, num)
				End If
				keywordEventContainerSyntax = keywordEventContainerSyntax1
			Else
				keywordEventContainerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax)
			End If
			Return keywordEventContainerSyntax
		End Function

		Friend Function Label(ByVal kind As SyntaxKind, ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), labelToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(kind, labelToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Function LabelStatement(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal colonToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax
			Dim labelStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(148, labelToken, colonToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim labelStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax(SyntaxKind.LabelStatement, labelToken, colonToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelStatementSyntax1, num)
				End If
				labelStatementSyntax = labelStatementSyntax1
			Else
				labelStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)
			End If
			Return labelStatementSyntax
		End Function

		Friend Function LambdaHeader(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function LeftShiftAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(255, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.LeftShiftAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function LeftShiftExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(315, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LeftShiftExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function LessThanExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(321, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LessThanExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function LessThanOrEqualExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(322, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LessThanOrEqualExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function LetClause(ByVal letKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax
			Dim letClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(360, letKeyword, variables.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim letClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax(SyntaxKind.LetClause, letKeyword, variables.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(letClauseSyntax1, num)
				End If
				letClauseSyntax = letClauseSyntax1
			Else
				letClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax)
			End If
			Return letClauseSyntax
		End Function

		Friend Function LikeExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(327, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LikeExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function LineContinuationTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.LineContinuationTrivia, text, Me._factoryContext)
		End Function

		Friend Function LiteralExpression(ByVal kind As SyntaxKind, ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(kind, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function LocalDeclarationStatement(ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal declarators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax
			Dim localDeclarationStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(147, modifiers.Node, declarators.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim localDeclarationStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(SyntaxKind.LocalDeclarationStatement, modifiers.Node, declarators.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(localDeclarationStatementSyntax1, num)
				End If
				localDeclarationStatementSyntax = localDeclarationStatementSyntax1
			Else
				localDeclarationStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax)
			End If
			Return localDeclarationStatementSyntax
		End Function

		Friend Function LoopStatement(ByVal kind As SyntaxKind, ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), loopKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(kind, loopKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Function LoopUntilStatement(ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(775, loopKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(SyntaxKind.LoopUntilStatement, loopKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Function LoopWhileStatement(ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(774, loopKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(SyntaxKind.LoopWhileStatement, loopKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Function MeExpression(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax
			Dim meExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(282, keyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim meExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax(SyntaxKind.MeExpression, keyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(meExpressionSyntax1, num)
				End If
				meExpressionSyntax = meExpressionSyntax1
			Else
				meExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax)
			End If
			Return meExpressionSyntax
		End Function

		Friend Function MemberAccessExpression(ByVal kind As SyntaxKind, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), expression, operatorToken, name, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim memberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(kind, expression, operatorToken, name, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(memberAccessExpressionSyntax1, num)
				End If
				memberAccessExpressionSyntax = memberAccessExpressionSyntax1
			Else
				memberAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
			End If
			Return memberAccessExpressionSyntax
		End Function

		Friend Function MethodBlock(ByVal kind As SyntaxKind, ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim methodBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim methodBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(kind, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(methodBlockSyntax1, num)
				End If
				methodBlockSyntax = methodBlockSyntax1
			Else
				methodBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax)
			End If
			Return methodBlockSyntax
		End Function

		Friend Function MethodStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause, Me._factoryContext)
		End Function

		Friend Function MidAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(248, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.MidAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function MidExpression(ByVal mid As IdentifierTokenSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax
			Dim midExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(260, mid, argumentList, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim midExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax(SyntaxKind.MidExpression, mid, argumentList, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(midExpressionSyntax1, num)
				End If
				midExpressionSyntax = midExpressionSyntax1
			Else
				midExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax)
			End If
			Return midExpressionSyntax
		End Function

		Friend Function ModifiedIdentifier(ByVal identifier As IdentifierTokenSyntax, ByVal nullable As PunctuationSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal arrayRankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(SyntaxKind.ModifiedIdentifier, identifier, nullable, arrayBounds, arrayRankSpecifiers.Node, Me._factoryContext)
		End Function

		Friend Function ModuleBlock(ByVal moduleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(SyntaxKind.ModuleBlock, moduleStatement, [inherits].Node, [implements].Node, members.Node, endModuleStatement, Me._factoryContext)
		End Function

		Friend Function ModuleStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal moduleKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax(SyntaxKind.ModuleStatement, attributeLists.Node, modifiers.Node, moduleKeyword, identifier, typeParameterList, Me._factoryContext)
		End Function

		Friend Function ModuloExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(318, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ModuloExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function MultiLineFunctionLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(343, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim multiLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineFunctionLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax1, num)
				End If
				multiLineLambdaExpressionSyntax = multiLineLambdaExpressionSyntax1
			Else
				multiLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax)
			End If
			Return multiLineLambdaExpressionSyntax
		End Function

		Friend Function MultiLineIfBlock(ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal elseIfBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax, ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(SyntaxKind.MultiLineIfBlock, ifStatement, statements.Node, elseIfBlocks.Node, elseBlock, endIfStatement, Me._factoryContext)
		End Function

		Friend Function MultiLineLambdaExpression(ByVal kind As SyntaxKind, ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim multiLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(kind, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax1, num)
				End If
				multiLineLambdaExpressionSyntax = multiLineLambdaExpressionSyntax1
			Else
				multiLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax)
			End If
			Return multiLineLambdaExpressionSyntax
		End Function

		Friend Function MultiLineSubLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(344, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim multiLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineSubLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax1, num)
				End If
				multiLineLambdaExpressionSyntax = multiLineLambdaExpressionSyntax1
			Else
				multiLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax)
			End If
			Return multiLineLambdaExpressionSyntax
		End Function

		Friend Function MultiplyAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(251, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.MultiplyAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function MultiplyExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(309, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.MultiplyExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function MyBaseExpression(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax
			Dim myBaseExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(283, keyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim myBaseExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax(SyntaxKind.MyBaseExpression, keyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(myBaseExpressionSyntax1, num)
				End If
				myBaseExpressionSyntax = myBaseExpressionSyntax1
			Else
				myBaseExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax)
			End If
			Return myBaseExpressionSyntax
		End Function

		Friend Function MyClassExpression(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax
			Dim myClassExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(284, keyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim myClassExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax(SyntaxKind.MyClassExpression, keyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(myClassExpressionSyntax1, num)
				End If
				myClassExpressionSyntax = myClassExpressionSyntax1
			Else
				myClassExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax)
			End If
			Return myClassExpressionSyntax
		End Function

		Friend Function NameColonEquals(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal colonEqualsToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax
			Dim nameColonEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(755, name, colonEqualsToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim nameColonEqualsSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax(SyntaxKind.NameColonEquals, name, colonEqualsToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(nameColonEqualsSyntax1, num)
				End If
				nameColonEqualsSyntax = nameColonEqualsSyntax1
			Else
				nameColonEqualsSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax)
			End If
			Return nameColonEqualsSyntax
		End Function

		Friend Function NamedFieldInitializer(ByVal keyKeyword As KeywordSyntax, ByVal dotToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax(SyntaxKind.NamedFieldInitializer, keyKeyword, dotToken, name, equalsToken, expression, Me._factoryContext)
		End Function

		Friend Function NamedTupleElement(ByVal identifier As IdentifierTokenSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax
			Dim namedTupleElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(791, identifier, asClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim namedTupleElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax(SyntaxKind.NamedTupleElement, identifier, asClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(namedTupleElementSyntax1, num)
				End If
				namedTupleElementSyntax = namedTupleElementSyntax1
			Else
				namedTupleElementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax)
			End If
			Return namedTupleElementSyntax
		End Function

		Friend Function NameOfExpression(ByVal nameOfKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax(SyntaxKind.NameOfExpression, nameOfKeyword, openParenToken, argument, closeParenToken, Me._factoryContext)
		End Function

		Friend Function NamespaceBlock(ByVal namespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax, ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endNamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax
			Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(48, namespaceStatement, members.Node, endNamespaceStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim namespaceBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax(SyntaxKind.NamespaceBlock, namespaceStatement, members.Node, endNamespaceStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(namespaceBlockSyntax1, num)
				End If
				namespaceBlockSyntax = namespaceBlockSyntax1
			Else
				namespaceBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax)
			End If
			Return namespaceBlockSyntax
		End Function

		Friend Function NamespaceStatement(ByVal namespaceKeyword As KeywordSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax
			Dim namespaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(49, namespaceKeyword, name, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim namespaceStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax(SyntaxKind.NamespaceStatement, namespaceKeyword, name, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(namespaceStatementSyntax1, num)
				End If
				namespaceStatementSyntax = namespaceStatementSyntax1
			Else
				namespaceStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax)
			End If
			Return namespaceStatementSyntax
		End Function

		Friend Function NewConstraint(ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(72, constraintKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(SyntaxKind.NewConstraint, constraintKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Function NextLabel(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(152, labelToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(SyntaxKind.NextLabel, labelToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Function NextStatement(ByVal nextKeyword As KeywordSyntax, ByVal controlVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(242, nextKeyword, controlVariables.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim nextStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax(SyntaxKind.NextStatement, nextKeyword, controlVariables.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(nextStatementSyntax1, num)
				End If
				nextStatementSyntax = nextStatementSyntax1
			Else
				nextStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			End If
			Return nextStatementSyntax
		End Function

		Friend Function NotEqualsExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(320, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.NotEqualsExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function NotExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(335, operatorToken, operand, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.NotExpression, operatorToken, operand, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Function NothingLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(280, token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.NothingLiteralExpression, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function NullableType(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal questionMarkToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax
			Dim nullableTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(397, elementType, questionMarkToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim nullableTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax(SyntaxKind.NullableType, elementType, questionMarkToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(nullableTypeSyntax1, num)
				End If
				nullableTypeSyntax = nullableTypeSyntax1
			Else
				nullableTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax)
			End If
			Return nullableTypeSyntax
		End Function

		Friend Function NumericLabel(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(151, labelToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(SyntaxKind.NumericLabel, labelToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Function NumericLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(275, token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.NumericLiteralExpression, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function ObjectCollectionInitializer(ByVal fromKeyword As KeywordSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax
			Dim objectCollectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(126, fromKeyword, initializer, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim objectCollectionInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax(SyntaxKind.ObjectCollectionInitializer, fromKeyword, initializer, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(objectCollectionInitializerSyntax1, num)
				End If
				objectCollectionInitializerSyntax = objectCollectionInitializerSyntax1
			Else
				objectCollectionInitializerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax)
			End If
			Return objectCollectionInitializerSyntax
		End Function

		Friend Function ObjectCreationExpression(ByVal newKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, newKeyword, attributeLists.Node, type, argumentList, initializer, Me._factoryContext)
		End Function

		Friend Function ObjectMemberInitializer(ByVal withKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal initializers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(SyntaxKind.ObjectMemberInitializer, withKeyword, openBraceToken, initializers.Node, closeBraceToken, Me._factoryContext)
		End Function

		Friend Function OmittedArgument(ByVal empty As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax
			Dim omittedArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(348, empty, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim omittedArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax(SyntaxKind.OmittedArgument, empty, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(omittedArgumentSyntax1, num)
				End If
				omittedArgumentSyntax = omittedArgumentSyntax1
			Else
				omittedArgumentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax)
			End If
			Return omittedArgumentSyntax
		End Function

		Friend Function OnErrorGoToLabelStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToLabelStatement, onKeyword, errorKeyword, goToKeyword, minus, label, Me._factoryContext)
		End Function

		Friend Function OnErrorGoToMinusOneStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToMinusOneStatement, onKeyword, errorKeyword, goToKeyword, minus, label, Me._factoryContext)
		End Function

		Friend Function OnErrorGoToStatement(ByVal kind As SyntaxKind, ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(kind, onKeyword, errorKeyword, goToKeyword, minus, label, Me._factoryContext)
		End Function

		Friend Function OnErrorGoToZeroStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToZeroStatement, onKeyword, errorKeyword, goToKeyword, minus, label, Me._factoryContext)
		End Function

		Friend Function OnErrorResumeNextStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal resumeKeyword As KeywordSyntax, ByVal nextKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(SyntaxKind.OnErrorResumeNextStatement, onKeyword, errorKeyword, resumeKeyword, nextKeyword, Me._factoryContext)
		End Function

		Friend Function OperatorBlock(ByVal operatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endOperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax
			Dim operatorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(82, operatorStatement, statements.Node, endOperatorStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim operatorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax(SyntaxKind.OperatorBlock, operatorStatement, statements.Node, endOperatorStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(operatorBlockSyntax1, num)
				End If
				operatorBlockSyntax = operatorBlockSyntax1
			Else
				operatorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax)
			End If
			Return operatorBlockSyntax
		End Function

		Friend Function OperatorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal operatorKeyword As KeywordSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax(SyntaxKind.OperatorStatement, attributeLists.Node, modifiers.Node, operatorKeyword, operatorToken, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function OptionStatement(ByVal optionKeyword As KeywordSyntax, ByVal nameKeyword As KeywordSyntax, ByVal valueKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax
			Dim optionStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(41, optionKeyword, nameKeyword, valueKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim optionStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(SyntaxKind.OptionStatement, optionKeyword, nameKeyword, valueKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(optionStatementSyntax1, num)
				End If
				optionStatementSyntax = optionStatementSyntax1
			Else
				optionStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)
			End If
			Return optionStatementSyntax
		End Function

		Friend Function OrderByClause(ByVal orderKeyword As KeywordSyntax, ByVal byKeyword As KeywordSyntax, ByVal orderings As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax
			Dim orderByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(372, orderKeyword, byKeyword, orderings.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim orderByClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(SyntaxKind.OrderByClause, orderKeyword, byKeyword, orderings.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderByClauseSyntax1, num)
				End If
				orderByClauseSyntax = orderByClauseSyntax1
			Else
				orderByClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax)
			End If
			Return orderByClauseSyntax
		End Function

		Friend Function Ordering(ByVal kind As SyntaxKind, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal ascendingOrDescendingKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), expression, ascendingOrDescendingKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim orderingSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(kind, expression, ascendingOrDescendingKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderingSyntax1, num)
				End If
				orderingSyntax = orderingSyntax1
			Else
				orderingSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			End If
			Return orderingSyntax
		End Function

		Friend Function OrElseExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(331, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.OrElseExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function OrExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(328, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.OrExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function Parameter(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(SyntaxKind.Parameter, attributeLists.Node, modifiers.Node, identifier, asClause, [default], Me._factoryContext)
		End Function

		Friend Function ParameterList(ByVal openParenToken As PunctuationSyntax, ByVal parameters As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(92, openParenToken, parameters.Node, closeParenToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim parameterListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax(SyntaxKind.ParameterList, openParenToken, parameters.Node, closeParenToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(parameterListSyntax1, num)
				End If
				parameterListSyntax = parameterListSyntax1
			Else
				parameterListSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			End If
			Return parameterListSyntax
		End Function

		Friend Function ParenthesizedExpression(ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax
			Dim parenthesizedExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(281, openParenToken, expression, closeParenToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim parenthesizedExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax(SyntaxKind.ParenthesizedExpression, openParenToken, expression, closeParenToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(parenthesizedExpressionSyntax1, num)
				End If
				parenthesizedExpressionSyntax = parenthesizedExpressionSyntax1
			Else
				parenthesizedExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax)
			End If
			Return parenthesizedExpressionSyntax
		End Function

		Friend Function PartitionClause(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As KeywordSyntax, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim partitionClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), skipOrTakeKeyword, count, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim partitionClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(kind, skipOrTakeKeyword, count, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionClauseSyntax1, num)
				End If
				partitionClauseSyntax = partitionClauseSyntax1
			Else
				partitionClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)
			End If
			Return partitionClauseSyntax
		End Function

		Friend Function PartitionWhileClause(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), skipOrTakeKeyword, whileKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim partitionWhileClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(kind, skipOrTakeKeyword, whileKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionWhileClauseSyntax1, num)
				End If
				partitionWhileClauseSyntax = partitionWhileClauseSyntax1
			Else
				partitionWhileClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)
			End If
			Return partitionWhileClauseSyntax
		End Function

		Friend Function PredefinedCastExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax(SyntaxKind.PredefinedCastExpression, keyword, openParenToken, expression, closeParenToken, Me._factoryContext)
		End Function

		Friend Function PredefinedType(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax
			Dim predefinedTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(398, keyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim predefinedTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax(SyntaxKind.PredefinedType, keyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(predefinedTypeSyntax1, num)
				End If
				predefinedTypeSyntax = predefinedTypeSyntax1
			Else
				predefinedTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax)
			End If
			Return predefinedTypeSyntax
		End Function

		Friend Function PrintStatement(ByVal questionToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax
			Dim printStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(140, questionToken, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim printStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax(SyntaxKind.PrintStatement, questionToken, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(printStatementSyntax1, num)
				End If
				printStatementSyntax = printStatementSyntax1
			Else
				printStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax)
			End If
			Return printStatementSyntax
		End Function

		Friend Function PropertyBlock(ByVal propertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax, ByVal accessors As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endPropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax
			Dim propertyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(88, propertyStatement, accessors.Node, endPropertyStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim propertyBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax(SyntaxKind.PropertyBlock, propertyStatement, accessors.Node, endPropertyStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(propertyBlockSyntax1, num)
				End If
				propertyBlockSyntax = propertyBlockSyntax1
			Else
				propertyBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax)
			End If
			Return propertyBlockSyntax
		End Function

		Friend Function PropertyStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal propertyKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax(SyntaxKind.PropertyStatement, attributeLists.Node, modifiers.Node, propertyKeyword, identifier, parameterList, asClause, initializer, implementsClause, Me._factoryContext)
		End Function

		Friend Function QualifiedCrefOperatorReference(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax
			Dim qualifiedCrefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(410, left, dotToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim qualifiedCrefOperatorReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(SyntaxKind.QualifiedCrefOperatorReference, left, dotToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(qualifiedCrefOperatorReferenceSyntax1, num)
				End If
				qualifiedCrefOperatorReferenceSyntax = qualifiedCrefOperatorReferenceSyntax1
			Else
				qualifiedCrefOperatorReferenceSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax)
			End If
			Return qualifiedCrefOperatorReferenceSyntax
		End Function

		Friend Function QualifiedName(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax
			Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(401, left, dotToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim qualifiedNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax(SyntaxKind.QualifiedName, left, dotToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(qualifiedNameSyntax1, num)
				End If
				qualifiedNameSyntax = qualifiedNameSyntax1
			Else
				qualifiedNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)
			End If
			Return qualifiedNameSyntax
		End Function

		Friend Function QueryExpression(ByVal clauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax
			Dim queryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(352, clauses.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim queryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax(SyntaxKind.QueryExpression, clauses.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(queryExpressionSyntax1, num)
				End If
				queryExpressionSyntax = queryExpressionSyntax1
			Else
				queryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax)
			End If
			Return queryExpressionSyntax
		End Function

		Friend Function RaiseEventAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(87, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.RaiseEventAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Function RaiseEventAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.RaiseEventAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, Me._factoryContext)
		End Function

		Friend Function RaiseEventStatement(ByVal raiseEventKeyword As KeywordSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax
			Dim raiseEventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(264, raiseEventKeyword, name, argumentList, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim raiseEventStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax(SyntaxKind.RaiseEventStatement, raiseEventKeyword, name, argumentList, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(raiseEventStatementSyntax1, num)
				End If
				raiseEventStatementSyntax = raiseEventStatementSyntax1
			Else
				raiseEventStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax)
			End If
			Return raiseEventStatementSyntax
		End Function

		Friend Function RangeArgument(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax
			Dim rangeArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(351, lowerBound, toKeyword, upperBound, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim rangeArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax(SyntaxKind.RangeArgument, lowerBound, toKeyword, upperBound, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(rangeArgumentSyntax1, num)
				End If
				rangeArgumentSyntax = rangeArgumentSyntax1
			Else
				rangeArgumentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax)
			End If
			Return rangeArgumentSyntax
		End Function

		Friend Function RangeCaseClause(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax
			Dim rangeCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(215, lowerBound, toKeyword, upperBound, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim rangeCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax(SyntaxKind.RangeCaseClause, lowerBound, toKeyword, upperBound, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(rangeCaseClauseSyntax1, num)
				End If
				rangeCaseClauseSyntax = rangeCaseClauseSyntax1
			Else
				rangeCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax)
			End If
			Return rangeCaseClauseSyntax
		End Function

		Friend Function RedimClause(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax
			Dim redimClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(270, expression, arrayBounds, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim redimClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax(SyntaxKind.RedimClause, expression, arrayBounds, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(redimClauseSyntax1, num)
				End If
				redimClauseSyntax = redimClauseSyntax1
			Else
				redimClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)
			End If
			Return redimClauseSyntax
		End Function

		Friend Function ReDimPreserveStatement(ByVal reDimKeyword As KeywordSyntax, ByVal preserveKeyword As KeywordSyntax, ByVal clauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim reDimStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(267, reDimKeyword, preserveKeyword, clauses.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim reDimStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(SyntaxKind.ReDimPreserveStatement, reDimKeyword, preserveKeyword, clauses.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(reDimStatementSyntax1, num)
				End If
				reDimStatementSyntax = reDimStatementSyntax1
			Else
				reDimStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)
			End If
			Return reDimStatementSyntax
		End Function

		Friend Function ReDimStatement(ByVal reDimKeyword As KeywordSyntax, ByVal preserveKeyword As KeywordSyntax, ByVal clauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim reDimStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(266, reDimKeyword, preserveKeyword, clauses.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim reDimStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(SyntaxKind.ReDimStatement, reDimKeyword, preserveKeyword, clauses.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(reDimStatementSyntax1, num)
				End If
				reDimStatementSyntax = reDimStatementSyntax1
			Else
				reDimStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)
			End If
			Return reDimStatementSyntax
		End Function

		Friend Function ReferenceDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal referenceKeyword As KeywordSyntax, ByVal file As StringLiteralTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(SyntaxKind.ReferenceDirectiveTrivia, hashToken, referenceKeyword, file, Me._factoryContext)
		End Function

		Friend Function RegionDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal regionKeyword As KeywordSyntax, ByVal name As StringLiteralTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(SyntaxKind.RegionDirectiveTrivia, hashToken, regionKeyword, name, Me._factoryContext)
		End Function

		Friend Function RelationalCaseClause(ByVal kind As SyntaxKind, ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), isKeyword, operatorToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(kind, isKeyword, operatorToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Function RemoveHandlerAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(86, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.RemoveHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Function RemoveHandlerAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.RemoveHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, Me._factoryContext)
		End Function

		Friend Function RemoveHandlerStatement(ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(SyntaxKind.RemoveHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression, Me._factoryContext)
		End Function

		Friend Function ResumeLabelStatement(ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(201, resumeKeyword, label, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim resumeStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(SyntaxKind.ResumeLabelStatement, resumeKeyword, label, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(resumeStatementSyntax1, num)
				End If
				resumeStatementSyntax = resumeStatementSyntax1
			Else
				resumeStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)
			End If
			Return resumeStatementSyntax
		End Function

		Friend Function ResumeNextStatement(ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(202, resumeKeyword, label, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim resumeStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(SyntaxKind.ResumeNextStatement, resumeKeyword, label, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(resumeStatementSyntax1, num)
				End If
				resumeStatementSyntax = resumeStatementSyntax1
			Else
				resumeStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)
			End If
			Return resumeStatementSyntax
		End Function

		Friend Function ResumeStatement(ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(200, resumeKeyword, label, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim resumeStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(SyntaxKind.ResumeStatement, resumeKeyword, label, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(resumeStatementSyntax1, num)
				End If
				resumeStatementSyntax = resumeStatementSyntax1
			Else
				resumeStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)
			End If
			Return resumeStatementSyntax
		End Function

		Friend Function ReturnStatement(ByVal returnKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax
			Dim returnStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(169, returnKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim returnStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax(SyntaxKind.ReturnStatement, returnKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(returnStatementSyntax1, num)
				End If
				returnStatementSyntax = returnStatementSyntax1
			Else
				returnStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax)
			End If
			Return returnStatementSyntax
		End Function

		Friend Function RightShiftAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(258, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.RightShiftAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function RightShiftExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(316, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.RightShiftExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function SelectBlock(ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax, ByVal caseBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax
			Dim selectBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(203, selectStatement, caseBlocks.Node, endSelectStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim selectBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(SyntaxKind.SelectBlock, selectStatement, caseBlocks.Node, endSelectStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(selectBlockSyntax1, num)
				End If
				selectBlockSyntax = selectBlockSyntax1
			Else
				selectBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax)
			End If
			Return selectBlockSyntax
		End Function

		Friend Function SelectClause(ByVal selectKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax
			Dim selectClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(377, selectKeyword, variables.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim selectClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax(SyntaxKind.SelectClause, selectKeyword, variables.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(selectClauseSyntax1, num)
				End If
				selectClauseSyntax = selectClauseSyntax1
			Else
				selectClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax)
			End If
			Return selectClauseSyntax
		End Function

		Friend Function SelectStatement(ByVal selectKeyword As KeywordSyntax, ByVal caseKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax
			Dim selectStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(204, selectKeyword, caseKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim selectStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax(SyntaxKind.SelectStatement, selectKeyword, caseKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(selectStatementSyntax1, num)
				End If
				selectStatementSyntax = selectStatementSyntax1
			Else
				selectStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax)
			End If
			Return selectStatementSyntax
		End Function

		Friend Function SetAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(84, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.SetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Function SetAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.SetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList, Me._factoryContext)
		End Function

		Friend Function SimpleArgument(ByVal nameColonEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax
			Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(349, nameColonEquals, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim simpleArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax(SyntaxKind.SimpleArgument, nameColonEquals, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleArgumentSyntax1, num)
				End If
				simpleArgumentSyntax = simpleArgumentSyntax1
			Else
				simpleArgumentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)
			End If
			Return simpleArgumentSyntax
		End Function

		Friend Function SimpleAsClause(ByVal asKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(123, asKeyword, attributeLists.Node, type, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim simpleAsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(SyntaxKind.SimpleAsClause, asKeyword, attributeLists.Node, type, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleAsClauseSyntax1, num)
				End If
				simpleAsClauseSyntax = simpleAsClauseSyntax1
			Else
				simpleAsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			End If
			Return simpleAsClauseSyntax
		End Function

		Friend Function SimpleAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(247, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.SimpleAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function SimpleCaseClause(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax
			Dim simpleCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(214, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim simpleCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax(SyntaxKind.SimpleCaseClause, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleCaseClauseSyntax1, num)
				End If
				simpleCaseClauseSyntax = simpleCaseClauseSyntax1
			Else
				simpleCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax)
			End If
			Return simpleCaseClauseSyntax
		End Function

		Friend Function SimpleDoLoopBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(756, doStatement, statements.Node, loopStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.SimpleDoLoopBlock, doStatement, statements.Node, loopStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Function SimpleDoStatement(ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(770, doKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(SyntaxKind.SimpleDoStatement, doKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Function SimpleImportsClause(ByVal [alias] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax
			Dim simpleImportsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(44, [alias], name, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim simpleImportsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax(SyntaxKind.SimpleImportsClause, [alias], name, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleImportsClauseSyntax1, num)
				End If
				simpleImportsClauseSyntax = simpleImportsClauseSyntax1
			Else
				simpleImportsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax)
			End If
			Return simpleImportsClauseSyntax
		End Function

		Friend Function SimpleJoinClause(ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal additionalJoins As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal onKeyword As KeywordSyntax, ByVal joinConditions As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(SyntaxKind.SimpleJoinClause, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node, Me._factoryContext)
		End Function

		Friend Function SimpleLoopStatement(ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(773, loopKeyword, whileOrUntilClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(SyntaxKind.SimpleLoopStatement, loopKeyword, whileOrUntilClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Function SimpleMemberAccessExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(291, expression, operatorToken, name, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim memberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(SyntaxKind.SimpleMemberAccessExpression, expression, operatorToken, name, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(memberAccessExpressionSyntax1, num)
				End If
				memberAccessExpressionSyntax = memberAccessExpressionSyntax1
			Else
				memberAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
			End If
			Return memberAccessExpressionSyntax
		End Function

		Friend Function SingleLineElseClause(ByVal elseKeyword As KeywordSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax
			Dim singleLineElseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(172, elseKeyword, statements.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim singleLineElseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax(SyntaxKind.SingleLineElseClause, elseKeyword, statements.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineElseClauseSyntax1, num)
				End If
				singleLineElseClauseSyntax = singleLineElseClauseSyntax1
			Else
				singleLineElseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax)
			End If
			Return singleLineElseClauseSyntax
		End Function

		Friend Function SingleLineFunctionLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(339, subOrFunctionHeader, body, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim singleLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineFunctionLambdaExpression, subOrFunctionHeader, body, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax1, num)
				End If
				singleLineLambdaExpressionSyntax = singleLineLambdaExpressionSyntax1
			Else
				singleLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)
			End If
			Return singleLineLambdaExpressionSyntax
		End Function

		Friend Function SingleLineIfStatement(ByVal ifKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal elseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax(SyntaxKind.SingleLineIfStatement, ifKeyword, condition, thenKeyword, statements.Node, elseClause, Me._factoryContext)
		End Function

		Friend Function SingleLineLambdaExpression(ByVal kind As SyntaxKind, ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), subOrFunctionHeader, body, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim singleLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(kind, subOrFunctionHeader, body, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax1, num)
				End If
				singleLineLambdaExpressionSyntax = singleLineLambdaExpressionSyntax1
			Else
				singleLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)
			End If
			Return singleLineLambdaExpressionSyntax
		End Function

		Friend Function SingleLineSubLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(342, subOrFunctionHeader, body, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim singleLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineSubLambdaExpression, subOrFunctionHeader, body, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax1, num)
				End If
				singleLineLambdaExpressionSyntax = singleLineLambdaExpressionSyntax1
			Else
				singleLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)
			End If
			Return singleLineLambdaExpressionSyntax
		End Function

		Friend Function SkipClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim partitionClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(366, skipOrTakeKeyword, count, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim partitionClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(SyntaxKind.SkipClause, skipOrTakeKeyword, count, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionClauseSyntax1, num)
				End If
				partitionClauseSyntax = partitionClauseSyntax1
			Else
				partitionClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)
			End If
			Return partitionClauseSyntax
		End Function

		Friend Function SkippedTokensTrivia(ByVal tokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax(SyntaxKind.SkippedTokensTrivia, tokens.Node, Me._factoryContext)
		End Function

		Friend Function SkipWhileClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(364, skipOrTakeKeyword, whileKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim partitionWhileClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(SyntaxKind.SkipWhileClause, skipOrTakeKeyword, whileKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionWhileClauseSyntax1, num)
				End If
				partitionWhileClauseSyntax = partitionWhileClauseSyntax1
			Else
				partitionWhileClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)
			End If
			Return partitionWhileClauseSyntax
		End Function

		Friend Function SpecialConstraint(ByVal kind As SyntaxKind, ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), constraintKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(kind, constraintKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Function StopOrEndStatement(ByVal kind As SyntaxKind, ByVal stopOrEndKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim stopOrEndStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), stopOrEndKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim stopOrEndStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(kind, stopOrEndKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(stopOrEndStatementSyntax1, num)
				End If
				stopOrEndStatementSyntax = stopOrEndStatementSyntax1
			Else
				stopOrEndStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)
			End If
			Return stopOrEndStatementSyntax
		End Function

		Friend Function StopStatement(ByVal stopOrEndKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim stopOrEndStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(153, stopOrEndKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim stopOrEndStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(SyntaxKind.StopStatement, stopOrEndKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(stopOrEndStatementSyntax1, num)
				End If
				stopOrEndStatementSyntax = stopOrEndStatementSyntax1
			Else
				stopOrEndStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)
			End If
			Return stopOrEndStatementSyntax
		End Function

		Friend Function StringLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(279, token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.StringLiteralExpression, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function StringLiteralToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As StringLiteralTokenSyntax
			Return New StringLiteralTokenSyntax(SyntaxKind.StringLiteralToken, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function StructureBlock(ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(SyntaxKind.StructureBlock, structureStatement, [inherits].Node, [implements].Node, members.Node, endStructureStatement, Me._factoryContext)
		End Function

		Friend Function StructureConstraint(ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(74, constraintKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(SyntaxKind.StructureConstraint, constraintKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Function StructureStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal structureKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax(SyntaxKind.StructureStatement, attributeLists.Node, modifiers.Node, structureKeyword, identifier, typeParameterList, Me._factoryContext)
		End Function

		Friend Function SubBlock(ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim methodBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(79, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim methodBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(SyntaxKind.SubBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(methodBlockSyntax1, num)
				End If
				methodBlockSyntax = methodBlockSyntax1
			Else
				methodBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax)
			End If
			Return methodBlockSyntax
		End Function

		Friend Function SubLambdaHeader(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(SyntaxKind.SubLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause, Me._factoryContext)
		End Function

		Friend Function SubNewStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subKeyword As KeywordSyntax, ByVal newKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(SyntaxKind.SubNewStatement, attributeLists.Node, modifiers.Node, subKeyword, newKeyword, parameterList, Me._factoryContext)
		End Function

		Friend Function SubStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(SyntaxKind.SubStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause, Me._factoryContext)
		End Function

		Friend Function SubtractAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(250, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.SubtractAssignmentStatement, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Function SubtractExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(308, left, operatorToken, right, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.SubtractExpression, left, operatorToken, right, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Function SyncLockBlock(ByVal syncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax
			Dim syncLockBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(145, syncLockStatement, statements.Node, endSyncLockStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim syncLockBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax(SyntaxKind.SyncLockBlock, syncLockStatement, statements.Node, endSyncLockStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(syncLockBlockSyntax1, num)
				End If
				syncLockBlockSyntax = syncLockBlockSyntax1
			Else
				syncLockBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax)
			End If
			Return syncLockBlockSyntax
		End Function

		Friend Function SyncLockStatement(ByVal syncLockKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax
			Dim syncLockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(226, syncLockKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim syncLockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax(SyntaxKind.SyncLockStatement, syncLockKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(syncLockStatementSyntax1, num)
				End If
				syncLockStatementSyntax = syncLockStatementSyntax1
			Else
				syncLockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax)
			End If
			Return syncLockStatementSyntax
		End Function

		Friend Function SyntaxTrivia(ByVal kind As SyntaxKind, ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(kind, text, Me._factoryContext)
		End Function

		Friend Function TakeClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim partitionClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(367, skipOrTakeKeyword, count, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim partitionClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(SyntaxKind.TakeClause, skipOrTakeKeyword, count, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionClauseSyntax1, num)
				End If
				partitionClauseSyntax = partitionClauseSyntax1
			Else
				partitionClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)
			End If
			Return partitionClauseSyntax
		End Function

		Friend Function TakeWhileClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(365, skipOrTakeKeyword, whileKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim partitionWhileClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(SyntaxKind.TakeWhileClause, skipOrTakeKeyword, whileKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionWhileClauseSyntax1, num)
				End If
				partitionWhileClauseSyntax = partitionWhileClauseSyntax1
			Else
				partitionWhileClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)
			End If
			Return partitionWhileClauseSyntax
		End Function

		Friend Function TernaryConditionalExpression(ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(SyntaxKind.TernaryConditionalExpression, ifKeyword, openParenToken, condition, firstCommaToken, whenTrue, secondCommaToken, whenFalse, closeParenToken, Me._factoryContext)
		End Function

		Friend Function ThrowStatement(ByVal throwKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax
			Dim throwStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(246, throwKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim throwStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(SyntaxKind.ThrowStatement, throwKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(throwStatementSyntax1, num)
				End If
				throwStatementSyntax = throwStatementSyntax1
			Else
				throwStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax)
			End If
			Return throwStatementSyntax
		End Function

		Friend Function TrueLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(273, token, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, token, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Function TryBlock(ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal catchBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax, ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(SyntaxKind.TryBlock, tryStatement, statements.Node, catchBlocks.Node, finallyBlock, endTryStatement, Me._factoryContext)
		End Function

		Friend Function TryCastExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax(SyntaxKind.TryCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken, Me._factoryContext)
		End Function

		Friend Function TryStatement(ByVal tryKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax
			Dim tryStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(189, tryKeyword, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim tryStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax(SyntaxKind.TryStatement, tryKeyword, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(tryStatementSyntax1, num)
				End If
				tryStatementSyntax = tryStatementSyntax1
			Else
				tryStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)
			End If
			Return tryStatementSyntax
		End Function

		Friend Function TupleExpression(ByVal openParenToken As PunctuationSyntax, ByVal arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax
			Dim tupleExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(788, openParenToken, arguments.Node, closeParenToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim tupleExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(SyntaxKind.TupleExpression, openParenToken, arguments.Node, closeParenToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(tupleExpressionSyntax1, num)
				End If
				tupleExpressionSyntax = tupleExpressionSyntax1
			Else
				tupleExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax)
			End If
			Return tupleExpressionSyntax
		End Function

		Friend Function TupleType(ByVal openParenToken As PunctuationSyntax, ByVal elements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax
			Dim tupleTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(789, openParenToken, elements.Node, closeParenToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim tupleTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax(SyntaxKind.TupleType, openParenToken, elements.Node, closeParenToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(tupleTypeSyntax1, num)
				End If
				tupleTypeSyntax = tupleTypeSyntax1
			Else
				tupleTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax)
			End If
			Return tupleTypeSyntax
		End Function

		Friend Function TypeArgumentList(ByVal openParenToken As PunctuationSyntax, ByVal ofKeyword As KeywordSyntax, ByVal arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(SyntaxKind.TypeArgumentList, openParenToken, ofKeyword, arguments.Node, closeParenToken, Me._factoryContext)
		End Function

		Friend Function TypeConstraint(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax
			Dim typeConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(75, type, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim typeConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax(SyntaxKind.TypeConstraint, type, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typeConstraintSyntax1, num)
				End If
				typeConstraintSyntax = typeConstraintSyntax1
			Else
				typeConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax)
			End If
			Return typeConstraintSyntax
		End Function

		Friend Function TypedTupleElement(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax
			Dim typedTupleElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(790, type, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim typedTupleElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax(SyntaxKind.TypedTupleElement, type, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typedTupleElementSyntax1, num)
				End If
				typedTupleElementSyntax = typedTupleElementSyntax1
			Else
				typedTupleElementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax)
			End If
			Return typedTupleElementSyntax
		End Function

		Friend Function TypeOfExpression(ByVal kind As SyntaxKind, ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(kind, typeOfKeyword, expression, operatorToken, type, Me._factoryContext)
		End Function

		Friend Function TypeOfIsExpression(ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(SyntaxKind.TypeOfIsExpression, typeOfKeyword, expression, operatorToken, type, Me._factoryContext)
		End Function

		Friend Function TypeOfIsNotExpression(ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(SyntaxKind.TypeOfIsNotExpression, typeOfKeyword, expression, operatorToken, type, Me._factoryContext)
		End Function

		Friend Function TypeParameter(ByVal varianceKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax
			Dim typeParameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(67, varianceKeyword, identifier, typeParameterConstraintClause, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim typeParameterSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(SyntaxKind.TypeParameter, varianceKeyword, identifier, typeParameterConstraintClause, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typeParameterSyntax1, num)
				End If
				typeParameterSyntax = typeParameterSyntax1
			Else
				typeParameterSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)
			End If
			Return typeParameterSyntax
		End Function

		Friend Function TypeParameterList(ByVal openParenToken As PunctuationSyntax, ByVal ofKeyword As KeywordSyntax, ByVal parameters As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax(SyntaxKind.TypeParameterList, openParenToken, ofKeyword, parameters.Node, closeParenToken, Me._factoryContext)
		End Function

		Friend Function TypeParameterMultipleConstraintClause(ByVal asKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal constraints As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(SyntaxKind.TypeParameterMultipleConstraintClause, asKeyword, openBraceToken, constraints.Node, closeBraceToken, Me._factoryContext)
		End Function

		Friend Function TypeParameterSingleConstraintClause(ByVal asKeyword As KeywordSyntax, ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax
			Dim typeParameterSingleConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(70, asKeyword, constraint, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim typeParameterSingleConstraintClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(SyntaxKind.TypeParameterSingleConstraintClause, asKeyword, constraint, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typeParameterSingleConstraintClauseSyntax1, num)
				End If
				typeParameterSingleConstraintClauseSyntax = typeParameterSingleConstraintClauseSyntax1
			Else
				typeParameterSingleConstraintClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax)
			End If
			Return typeParameterSingleConstraintClauseSyntax
		End Function

		Friend Function UnaryExpression(ByVal kind As SyntaxKind, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), operatorToken, operand, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(kind, operatorToken, operand, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Function UnaryMinusExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(334, operatorToken, operand, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.UnaryMinusExpression, operatorToken, operand, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Function UnaryPlusExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(333, operatorToken, operand, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.UnaryPlusExpression, operatorToken, operand, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Function UntilClause(ByVal whileOrUntilKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(777, whileOrUntilKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim whileOrUntilClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(SyntaxKind.UntilClause, whileOrUntilKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax1, num)
				End If
				whileOrUntilClauseSyntax = whileOrUntilClauseSyntax1
			Else
				whileOrUntilClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			End If
			Return whileOrUntilClauseSyntax
		End Function

		Friend Function UsingBlock(ByVal usingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endUsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax
			Dim usingBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(144, usingStatement, statements.Node, endUsingStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim usingBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax(SyntaxKind.UsingBlock, usingStatement, statements.Node, endUsingStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(usingBlockSyntax1, num)
				End If
				usingBlockSyntax = usingBlockSyntax1
			Else
				usingBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax)
			End If
			Return usingBlockSyntax
		End Function

		Friend Function UsingStatement(ByVal usingKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax
			Dim usingStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(243, usingKeyword, expression, variables.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim usingStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(SyntaxKind.UsingStatement, usingKeyword, expression, variables.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(usingStatementSyntax1, num)
				End If
				usingStatementSyntax = usingStatementSyntax1
			Else
				usingStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax)
			End If
			Return usingStatementSyntax
		End Function

		Friend Function VariableDeclarator(ByVal names As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax
			Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(122, names.Node, asClause, initializer, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim variableDeclaratorSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(SyntaxKind.VariableDeclarator, names.Node, asClause, initializer, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(variableDeclaratorSyntax1, num)
				End If
				variableDeclaratorSyntax = variableDeclaratorSyntax1
			Else
				variableDeclaratorSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)
			End If
			Return variableDeclaratorSyntax
		End Function

		Friend Function VariableNameEquals(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal equalsToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(356, identifier, asClause, equalsToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim variableNameEqualsSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax(SyntaxKind.VariableNameEquals, identifier, asClause, equalsToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(variableNameEqualsSyntax1, num)
				End If
				variableNameEqualsSyntax = variableNameEqualsSyntax1
			Else
				variableNameEqualsSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)
			End If
			Return variableNameEqualsSyntax
		End Function

		Friend Function WhereClause(ByVal whereKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax
			Dim whereClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(363, whereKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim whereClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax(SyntaxKind.WhereClause, whereKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whereClauseSyntax1, num)
				End If
				whereClauseSyntax = whereClauseSyntax1
			Else
				whereClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax)
			End If
			Return whereClauseSyntax
		End Function

		Friend Function WhileBlock(ByVal whileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endWhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax
			Dim whileBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(141, whileStatement, statements.Node, endWhileStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim whileBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax(SyntaxKind.WhileBlock, whileStatement, statements.Node, endWhileStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileBlockSyntax1, num)
				End If
				whileBlockSyntax = whileBlockSyntax1
			Else
				whileBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax)
			End If
			Return whileBlockSyntax
		End Function

		Friend Function WhileClause(ByVal whileOrUntilKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(776, whileOrUntilKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim whileOrUntilClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(SyntaxKind.WhileClause, whileOrUntilKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax1, num)
				End If
				whileOrUntilClauseSyntax = whileOrUntilClauseSyntax1
			Else
				whileOrUntilClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			End If
			Return whileOrUntilClauseSyntax
		End Function

		Friend Function WhileOrUntilClause(ByVal kind As SyntaxKind, ByVal whileOrUntilKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(CInt(kind), whileOrUntilKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim whileOrUntilClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(kind, whileOrUntilKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax1, num)
				End If
				whileOrUntilClauseSyntax = whileOrUntilClauseSyntax1
			Else
				whileOrUntilClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			End If
			Return whileOrUntilClauseSyntax
		End Function

		Friend Function WhileStatement(ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax
			Dim whileStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(234, whileKeyword, condition, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim whileStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax(SyntaxKind.WhileStatement, whileKeyword, condition, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileStatementSyntax1, num)
				End If
				whileStatementSyntax = whileStatementSyntax1
			Else
				whileStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax)
			End If
			Return whileStatementSyntax
		End Function

		Friend Function WhitespaceTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, text, Me._factoryContext)
		End Function

		Friend Function WithBlock(ByVal withStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endWithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax
			Dim withBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(146, withStatement, statements.Node, endWithStatement, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim withBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax(SyntaxKind.WithBlock, withStatement, statements.Node, endWithStatement, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withBlockSyntax1, num)
				End If
				withBlockSyntax = withBlockSyntax1
			Else
				withBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax)
			End If
			Return withBlockSyntax
		End Function

		Friend Function WithEventsEventContainer(ByVal identifier As IdentifierTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax
			Dim withEventsEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(115, identifier, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim withEventsEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax(SyntaxKind.WithEventsEventContainer, identifier, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withEventsEventContainerSyntax1, num)
				End If
				withEventsEventContainerSyntax = withEventsEventContainerSyntax1
			Else
				withEventsEventContainerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax)
			End If
			Return withEventsEventContainerSyntax
		End Function

		Friend Function WithEventsPropertyEventContainer(ByVal withEventsContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax
			Dim withEventsPropertyEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(116, withEventsContainer, dotToken, [property], Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim withEventsPropertyEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax(SyntaxKind.WithEventsPropertyEventContainer, withEventsContainer, dotToken, [property], Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withEventsPropertyEventContainerSyntax1, num)
				End If
				withEventsPropertyEventContainerSyntax = withEventsPropertyEventContainerSyntax1
			Else
				withEventsPropertyEventContainerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax)
			End If
			Return withEventsPropertyEventContainerSyntax
		End Function

		Friend Function WithStatement(ByVal withKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax
			Dim withStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(265, withKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim withStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax(SyntaxKind.WithStatement, withKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withStatementSyntax1, num)
				End If
				withStatementSyntax = withStatementSyntax1
			Else
				withStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax)
			End If
			Return withStatementSyntax
		End Function

		Friend Function XmlAttribute(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax
			Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(386, name, equalsToken, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlAttributeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax(SyntaxKind.XmlAttribute, name, equalsToken, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlAttributeSyntax1, num)
				End If
				xmlAttributeSyntax = xmlAttributeSyntax1
			Else
				xmlAttributeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)
			End If
			Return xmlAttributeSyntax
		End Function

		Friend Function XmlAttributeAccessExpression(ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(SyntaxKind.XmlAttributeAccessExpression, base, token1, token2, token3, name, Me._factoryContext)
		End Function

		Friend Function XmlBracketedName(ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim xmlBracketedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(390, lessThanToken, name, greaterThanToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlBracketedNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax(SyntaxKind.XmlBracketedName, lessThanToken, name, greaterThanToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlBracketedNameSyntax1, num)
				End If
				xmlBracketedNameSyntax = xmlBracketedNameSyntax1
			Else
				xmlBracketedNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)
			End If
			Return xmlBracketedNameSyntax
		End Function

		Friend Function XmlCDataSection(ByVal beginCDataToken As PunctuationSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endCDataToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax
			Dim xmlCDataSectionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(394, beginCDataToken, textTokens.Node, endCDataToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlCDataSectionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax(SyntaxKind.XmlCDataSection, beginCDataToken, textTokens.Node, endCDataToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlCDataSectionSyntax1, num)
				End If
				xmlCDataSectionSyntax = xmlCDataSectionSyntax1
			Else
				xmlCDataSectionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)
			End If
			Return xmlCDataSectionSyntax
		End Function

		Friend Function XmlComment(ByVal lessThanExclamationMinusMinusToken As PunctuationSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal minusMinusGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax
			Dim xmlCommentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(392, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlCommentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(SyntaxKind.XmlComment, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlCommentSyntax1, num)
				End If
				xmlCommentSyntax = xmlCommentSyntax1
			Else
				xmlCommentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)
			End If
			Return xmlCommentSyntax
		End Function

		Friend Function XmlCrefAttribute(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax, ByVal endQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax(SyntaxKind.XmlCrefAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken, Me._factoryContext)
		End Function

		Friend Function XmlDeclaration(ByVal lessThanQuestionToken As PunctuationSyntax, ByVal xmlKeyword As KeywordSyntax, ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal questionGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(SyntaxKind.XmlDeclaration, lessThanQuestionToken, xmlKeyword, version, encoding, standalone, questionGreaterThanToken, Me._factoryContext)
		End Function

		Friend Function XmlDeclarationOption(ByVal name As XmlNameTokenSyntax, ByVal equals As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim xmlDeclarationOptionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(380, name, equals, value, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlDeclarationOptionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(SyntaxKind.XmlDeclarationOption, name, equals, value, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlDeclarationOptionSyntax1, num)
				End If
				xmlDeclarationOptionSyntax = xmlDeclarationOptionSyntax1
			Else
				xmlDeclarationOptionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			End If
			Return xmlDeclarationOptionSyntax
		End Function

		Friend Function XmlDescendantAccessExpression(ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(SyntaxKind.XmlDescendantAccessExpression, base, token1, token2, token3, name, Me._factoryContext)
		End Function

		Friend Function XmlDocument(ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax, ByVal precedingMisc As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal followingMisc As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(SyntaxKind.XmlDocument, declaration, precedingMisc.Node, root, followingMisc.Node, Me._factoryContext)
		End Function

		Friend Function XmlElement(ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax, ByVal content As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax
			Dim xmlElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(381, startTag, content.Node, endTag, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(SyntaxKind.XmlElement, startTag, content.Node, endTag, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlElementSyntax1, num)
				End If
				xmlElementSyntax = xmlElementSyntax1
			Else
				xmlElementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax)
			End If
			Return xmlElementSyntax
		End Function

		Friend Function XmlElementAccessExpression(ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(SyntaxKind.XmlElementAccessExpression, base, token1, token2, token3, name, Me._factoryContext)
		End Function

		Friend Function XmlElementEndTag(ByVal lessThanSlashToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax
			Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(384, lessThanSlashToken, name, greaterThanToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlElementEndTagSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax(SyntaxKind.XmlElementEndTag, lessThanSlashToken, name, greaterThanToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlElementEndTagSyntax1, num)
				End If
				xmlElementEndTagSyntax = xmlElementEndTagSyntax1
			Else
				xmlElementEndTagSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)
			End If
			Return xmlElementEndTagSyntax
		End Function

		Friend Function XmlElementStartTag(ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(SyntaxKind.XmlElementStartTag, lessThanToken, name, attributes.Node, greaterThanToken, Me._factoryContext)
		End Function

		Friend Function XmlEmbeddedExpression(ByVal lessThanPercentEqualsToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal percentGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax
			Dim xmlEmbeddedExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(395, lessThanPercentEqualsToken, expression, percentGreaterThanToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlEmbeddedExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax(SyntaxKind.XmlEmbeddedExpression, lessThanPercentEqualsToken, expression, percentGreaterThanToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlEmbeddedExpressionSyntax1, num)
				End If
				xmlEmbeddedExpressionSyntax = xmlEmbeddedExpressionSyntax1
			Else
				xmlEmbeddedExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)
			End If
			Return xmlEmbeddedExpressionSyntax
		End Function

		Friend Function XmlEmptyElement(ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal slashGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax(SyntaxKind.XmlEmptyElement, lessThanToken, name, attributes.Node, slashGreaterThanToken, Me._factoryContext)
		End Function

		Friend Function XmlEntityLiteralToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(SyntaxKind.XmlEntityLiteralToken, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function XmlMemberAccessExpression(ByVal kind As SyntaxKind, ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(kind, base, token1, token2, token3, name, Me._factoryContext)
		End Function

		Friend Function XmlName(ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax, ByVal localName As XmlNameTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(389, prefix, localName, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(SyntaxKind.XmlName, prefix, localName, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlNameSyntax1, num)
				End If
				xmlNameSyntax = xmlNameSyntax1
			Else
				xmlNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			End If
			Return xmlNameSyntax
		End Function

		Friend Function XmlNameAttribute(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal endQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(SyntaxKind.XmlNameAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken, Me._factoryContext)
		End Function

		Friend Function XmlNamespaceImportsClause(ByVal lessThanToken As PunctuationSyntax, ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax, ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax
			Dim xmlNamespaceImportsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(45, lessThanToken, xmlNamespace, greaterThanToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlNamespaceImportsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(SyntaxKind.XmlNamespaceImportsClause, lessThanToken, xmlNamespace, greaterThanToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlNamespaceImportsClauseSyntax1, num)
				End If
				xmlNamespaceImportsClauseSyntax = xmlNamespaceImportsClauseSyntax1
			Else
				xmlNamespaceImportsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax)
			End If
			Return xmlNamespaceImportsClauseSyntax
		End Function

		Friend Function XmlNameToken(ByVal text As String, ByVal possibleKeywordKind As SyntaxKind, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlNameTokenSyntax
			Return New XmlNameTokenSyntax(SyntaxKind.XmlNameToken, text, leadingTrivia, trailingTrivia, possibleKeywordKind, Me._factoryContext)
		End Function

		Friend Function XmlPrefix(ByVal name As XmlNameTokenSyntax, ByVal colonToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(391, name, colonToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlPrefixSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax(SyntaxKind.XmlPrefix, name, colonToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlPrefixSyntax1, num)
				End If
				xmlPrefixSyntax = xmlPrefixSyntax1
			Else
				xmlPrefixSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)
			End If
			Return xmlPrefixSyntax
		End Function

		Friend Function XmlPrefixName(ByVal name As XmlNameTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax
			Dim xmlPrefixNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(388, name, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlPrefixNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(SyntaxKind.XmlPrefixName, name, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlPrefixNameSyntax1, num)
				End If
				xmlPrefixNameSyntax = xmlPrefixNameSyntax1
			Else
				xmlPrefixNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax)
			End If
			Return xmlPrefixNameSyntax
		End Function

		Friend Function XmlProcessingInstruction(ByVal lessThanQuestionToken As PunctuationSyntax, ByVal name As XmlNameTokenSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal questionGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(SyntaxKind.XmlProcessingInstruction, lessThanQuestionToken, name, textTokens.Node, questionGreaterThanToken, Me._factoryContext)
		End Function

		Friend Function XmlString(ByVal startQuoteToken As PunctuationSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(387, startQuoteToken, textTokens.Node, endQuoteToken, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlStringSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax(SyntaxKind.XmlString, startQuoteToken, textTokens.Node, endQuoteToken, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlStringSyntax1, num)
				End If
				xmlStringSyntax = xmlStringSyntax1
			Else
				xmlStringSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)
			End If
			Return xmlStringSyntax
		End Function

		Friend Function XmlText(ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax
			Dim xmlTextSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(382, textTokens.Node, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim xmlTextSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax(SyntaxKind.XmlText, textTokens.Node, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlTextSyntax1, num)
				End If
				xmlTextSyntax = xmlTextSyntax1
			Else
				xmlTextSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax)
			End If
			Return xmlTextSyntax
		End Function

		Friend Function XmlTextLiteralToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(SyntaxKind.XmlTextLiteralToken, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function XmlTextToken(ByVal kind As SyntaxKind, ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(kind, text, leadingTrivia, trailingTrivia, value, Me._factoryContext)
		End Function

		Friend Function YieldStatement(ByVal yieldKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax
			Dim yieldStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax
			Dim num As Integer = 0
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = VisualBasicSyntaxNodeCache.TryGetNode(411, yieldKeyword, expression, Me._factoryContext, num)
			If (greenNode Is Nothing) Then
				Dim yieldStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax(SyntaxKind.YieldStatement, yieldKeyword, expression, Me._factoryContext)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(yieldStatementSyntax1, num)
				End If
				yieldStatementSyntax = yieldStatementSyntax1
			Else
				yieldStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax)
			End If
			Return yieldStatementSyntax
		End Function
	End Class
End Namespace