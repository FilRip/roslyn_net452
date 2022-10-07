Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class SyntaxFactory
		Friend ReadOnly Shared CarriageReturnLineFeed As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared LineFeed As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared CarriageReturn As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared Space As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared Tab As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared ElasticCarriageReturnLineFeed As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared ElasticLineFeed As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared ElasticCarriageReturn As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared ElasticSpace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared ElasticTab As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Friend ReadOnly Shared ElasticZeroSpace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Private Shared s_notMissingEmptyToken As PunctuationSyntax

		Private Shared s_missingEmptyToken As PunctuationSyntax

		Private Shared s_statementTerminatorToken As PunctuationSyntax

		Private Shared s_colonToken As PunctuationSyntax

		Private ReadOnly Shared s_missingExpr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Private ReadOnly Shared s_emptyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax

		Private ReadOnly Shared s_omittedArgument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax

		Friend ReadOnly Shared Property ColonToken As PunctuationSyntax
			Get
				If (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_colonToken Is Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_colonToken = New PunctuationSyntax(SyntaxKind.ColonToken, "", Nothing, Nothing)
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_colonToken.SetFlags(GreenNode.NodeFlags.IsNotMissing)
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_colonToken
			End Get
		End Property

		Friend ReadOnly Shared Property MissingEmptyToken As PunctuationSyntax
			Get
				If (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_missingEmptyToken Is Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_missingEmptyToken = New PunctuationSyntax(SyntaxKind.EmptyToken, "", Nothing, Nothing)
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_missingEmptyToken.ClearFlags(GreenNode.NodeFlags.IsNotMissing)
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_missingEmptyToken
			End Get
		End Property

		Friend ReadOnly Shared Property NotMissingEmptyToken As PunctuationSyntax
			Get
				If (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_notMissingEmptyToken Is Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_notMissingEmptyToken = New PunctuationSyntax(SyntaxKind.EmptyToken, "", Nothing, Nothing)
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_notMissingEmptyToken
			End Get
		End Property

		Friend ReadOnly Shared Property StatementTerminatorToken As PunctuationSyntax
			Get
				If (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_statementTerminatorToken Is Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_statementTerminatorToken = New PunctuationSyntax(SyntaxKind.StatementTerminatorToken, "", Nothing, Nothing)
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_statementTerminatorToken.SetFlags(GreenNode.NodeFlags.IsNotMissing)
				End If
				Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_statementTerminatorToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CarriageReturnLineFeed = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLine("" & VbCrLf & "", False)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.LineFeed = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLine("" & VbCrLf & "", False)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CarriageReturn = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLine("", False)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Space = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace(" ", False)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Tab = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace("	", False)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturnLineFeed = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLine("" & VbCrLf & "", True)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticLineFeed = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLine("" & VbCrLf & "", True)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturn = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLine("", True)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticSpace = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace(" ", True)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticTab = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace("	", True)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticZeroSpace = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace([String].Empty, True)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_notMissingEmptyToken = Nothing
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_missingEmptyToken = Nothing
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_statementTerminatorToken = Nothing
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_colonToken = Nothing
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_missingExpr = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Identifier("", Nothing, Nothing))
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_emptyStatement = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.NotMissingEmptyToken)
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_omittedArgument = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.OmittedArgument(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.NotMissingEmptyToken)
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function AccessorBlock(ByVal kind As SyntaxKind, ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), accessorStatement, statements.Node, endAccessorStatement, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(kind, accessorStatement, statements.Node, endAccessorStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Shared Function AccessorStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(kind, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList)
		End Function

		Friend Shared Function AddAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(249, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.AddAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function AddExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(307, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.AddExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function AddHandlerAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(85, accessorStatement, statements.Node, endAccessorStatement, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.AddHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Shared Function AddHandlerAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.AddHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList)
		End Function

		Friend Shared Function AddHandlerStatement(ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(SyntaxKind.AddHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression)
		End Function

		Friend Shared Function AddRemoveHandlerStatement(ByVal kind As SyntaxKind, ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(kind, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression)
		End Function

		Friend Shared Function AddressOfExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(336, operatorToken, operand, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.AddressOfExpression, operatorToken, operand)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Shared Function AggregateClause(ByVal aggregateKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal additionalQueryOperators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(SyntaxKind.AggregateClause, aggregateKeyword, variables.Node, additionalQueryOperators.Node, intoKeyword, aggregationVariables.Node)
		End Function

		Friend Shared Function AggregationRangeVariable(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax, ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax
			Dim aggregationRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(355, nameEquals, aggregation, num)
			If (greenNode Is Nothing) Then
				Dim aggregationRangeVariableSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(SyntaxKind.AggregationRangeVariable, nameEquals, aggregation)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(aggregationRangeVariableSyntax1, num)
				End If
				aggregationRangeVariableSyntax = aggregationRangeVariableSyntax1
			Else
				aggregationRangeVariableSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)
			End If
			Return aggregationRangeVariableSyntax
		End Function

		Friend Shared Function AndAlsoExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(332, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.AndAlsoExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function AndExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(330, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.AndExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function AnonymousObjectCreationExpression(ByVal newKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax
			Dim anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(298, newKeyword, attributeLists.Node, initializer, num)
			If (greenNode Is Nothing) Then
				Dim anonymousObjectCreationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(SyntaxKind.AnonymousObjectCreationExpression, newKeyword, attributeLists.Node, initializer)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(anonymousObjectCreationExpressionSyntax1, num)
				End If
				anonymousObjectCreationExpressionSyntax = anonymousObjectCreationExpressionSyntax1
			Else
				anonymousObjectCreationExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)
			End If
			Return anonymousObjectCreationExpressionSyntax
		End Function

		Friend Shared Function ArgumentList(ByVal openParenToken As PunctuationSyntax, ByVal arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(347, openParenToken, arguments.Node, closeParenToken, num)
			If (greenNode Is Nothing) Then
				Dim argumentListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax(SyntaxKind.ArgumentList, openParenToken, arguments.Node, closeParenToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(argumentListSyntax1, num)
				End If
				argumentListSyntax = argumentListSyntax1
			Else
				argumentListSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			End If
			Return argumentListSyntax
		End Function

		Friend Shared Function ArrayCreationExpression(ByVal newKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal rankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(SyntaxKind.ArrayCreationExpression, newKeyword, attributeLists.Node, type, arrayBounds, rankSpecifiers.Node, initializer)
		End Function

		Friend Shared Function ArrayRankSpecifier(ByVal openParenToken As PunctuationSyntax, ByVal commaTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax
			Dim arrayRankSpecifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(134, openParenToken, commaTokens.Node, closeParenToken, num)
			If (greenNode Is Nothing) Then
				Dim arrayRankSpecifierSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax(SyntaxKind.ArrayRankSpecifier, openParenToken, commaTokens.Node, closeParenToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(arrayRankSpecifierSyntax1, num)
				End If
				arrayRankSpecifierSyntax = arrayRankSpecifierSyntax1
			Else
				arrayRankSpecifierSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)
			End If
			Return arrayRankSpecifierSyntax
		End Function

		Friend Shared Function ArrayType(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal rankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax
			Dim arrayTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(396, elementType, rankSpecifiers.Node, num)
			If (greenNode Is Nothing) Then
				Dim arrayTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax(SyntaxKind.ArrayType, elementType, rankSpecifiers.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(arrayTypeSyntax1, num)
				End If
				arrayTypeSyntax = arrayTypeSyntax1
			Else
				arrayTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax)
			End If
			Return arrayTypeSyntax
		End Function

		Friend Shared Function AscendingOrdering(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal ascendingOrDescendingKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(375, expression, ascendingOrDescendingKeyword, num)
			If (greenNode Is Nothing) Then
				Dim orderingSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(SyntaxKind.AscendingOrdering, expression, ascendingOrDescendingKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderingSyntax1, num)
				End If
				orderingSyntax = orderingSyntax1
			Else
				orderingSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			End If
			Return orderingSyntax
		End Function

		Friend Shared Function AsNewClause(ByVal asKeyword As KeywordSyntax, ByVal newExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax
			Dim asNewClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(124, asKeyword, newExpression, num)
			If (greenNode Is Nothing) Then
				Dim asNewClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax(SyntaxKind.AsNewClause, asKeyword, newExpression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(asNewClauseSyntax1, num)
				End If
				asNewClauseSyntax = asNewClauseSyntax1
			Else
				asNewClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax)
			End If
			Return asNewClauseSyntax
		End Function

		Friend Shared Function AssignmentStatement(ByVal kind As SyntaxKind, ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(kind, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function Attribute(ByVal target As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax
			Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(136, target, name, argumentList, num)
			If (greenNode Is Nothing) Then
				Dim attributeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax(SyntaxKind.Attribute, target, name, argumentList)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributeSyntax1, num)
				End If
				attributeSyntax = attributeSyntax1
			Else
				attributeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax)
			End If
			Return attributeSyntax
		End Function

		Friend Shared Function AttributeList(ByVal lessThanToken As PunctuationSyntax, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax
			Dim attributeListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(135, lessThanToken, attributes.Node, greaterThanToken, num)
			If (greenNode Is Nothing) Then
				Dim attributeListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax(SyntaxKind.AttributeList, lessThanToken, attributes.Node, greaterThanToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributeListSyntax1, num)
				End If
				attributeListSyntax = attributeListSyntax1
			Else
				attributeListSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			End If
			Return attributeListSyntax
		End Function

		Friend Shared Function AttributesStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax
			Dim attributesStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(138, attributeLists.Node, num)
			If (greenNode Is Nothing) Then
				Dim attributesStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax(SyntaxKind.AttributesStatement, attributeLists.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributesStatementSyntax1, num)
				End If
				attributesStatementSyntax = attributesStatementSyntax1
			Else
				attributesStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)
			End If
			Return attributesStatementSyntax
		End Function

		Friend Shared Function AttributeTarget(ByVal attributeModifier As KeywordSyntax, ByVal colonToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax
			Dim attributeTargetSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(137, attributeModifier, colonToken, num)
			If (greenNode Is Nothing) Then
				Dim attributeTargetSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax(SyntaxKind.AttributeTarget, attributeModifier, colonToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(attributeTargetSyntax1, num)
				End If
				attributeTargetSyntax = attributeTargetSyntax1
			Else
				attributeTargetSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)
			End If
			Return attributeTargetSyntax
		End Function

		Friend Shared Function AwaitExpression(ByVal awaitKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax
			Dim awaitExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(412, awaitKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim awaitExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax(SyntaxKind.AwaitExpression, awaitKeyword, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(awaitExpressionSyntax1, num)
				End If
				awaitExpressionSyntax = awaitExpressionSyntax1
			Else
				awaitExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax)
			End If
			Return awaitExpressionSyntax
		End Function

		Friend Shared Function BadDirectiveTrivia(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax(SyntaxKind.BadDirectiveTrivia, hashToken)
		End Function

		Friend Shared Function BadToken(ByVal SubKind As SyntaxSubKind, ByVal text As String, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode) As BadTokenSyntax
			Return New BadTokenSyntax(SyntaxKind.BadToken, SubKind, Nothing, Nothing, text, precedingTrivia, followingTrivia)
		End Function

		Friend Shared Function BinaryConditionalExpression(ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal firstExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal secondExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax(SyntaxKind.BinaryConditionalExpression, ifKeyword, openParenToken, firstExpression, commaToken, secondExpression, closeParenToken)
		End Function

		Friend Shared Function BinaryExpression(ByVal kind As SyntaxKind, ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(kind, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function CallStatement(ByVal callKeyword As KeywordSyntax, ByVal invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax
			Dim callStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(261, callKeyword, invocation, num)
			If (greenNode Is Nothing) Then
				Dim callStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax(SyntaxKind.CallStatement, callKeyword, invocation)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(callStatementSyntax1, num)
				End If
				callStatementSyntax = callStatementSyntax1
			Else
				callStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax)
			End If
			Return callStatementSyntax
		End Function

		Friend Shared Function CaseBlock(ByVal caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim caseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(207, caseStatement, statements.Node, num)
			If (greenNode Is Nothing) Then
				Dim caseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax(SyntaxKind.CaseBlock, caseStatement, statements.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseBlockSyntax1, num)
				End If
				caseBlockSyntax = caseBlockSyntax1
			Else
				caseBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)
			End If
			Return caseBlockSyntax
		End Function

		Friend Shared Function CaseElseBlock(ByVal caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim caseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(210, caseStatement, statements.Node, num)
			If (greenNode Is Nothing) Then
				Dim caseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax(SyntaxKind.CaseElseBlock, caseStatement, statements.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseBlockSyntax1, num)
				End If
				caseBlockSyntax = caseBlockSyntax1
			Else
				caseBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)
			End If
			Return caseBlockSyntax
		End Function

		Friend Shared Function CaseElseStatement(ByVal caseKeyword As KeywordSyntax, ByVal cases As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(212, caseKeyword, cases.Node, num)
			If (greenNode Is Nothing) Then
				Dim caseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax(SyntaxKind.CaseElseStatement, caseKeyword, cases.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseStatementSyntax1, num)
				End If
				caseStatementSyntax = caseStatementSyntax1
			Else
				caseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)
			End If
			Return caseStatementSyntax
		End Function

		Friend Shared Function CaseEqualsClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(216, isKeyword, operatorToken, value, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseEqualsClause, isKeyword, operatorToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Shared Function CaseGreaterThanClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(223, isKeyword, operatorToken, value, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanClause, isKeyword, operatorToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Shared Function CaseGreaterThanOrEqualClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(222, isKeyword, operatorToken, value, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanOrEqualClause, isKeyword, operatorToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Shared Function CaseLessThanClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(218, isKeyword, operatorToken, value, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanClause, isKeyword, operatorToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Shared Function CaseLessThanOrEqualClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(219, isKeyword, operatorToken, value, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanOrEqualClause, isKeyword, operatorToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Shared Function CaseNotEqualsClause(ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(217, isKeyword, operatorToken, value, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(SyntaxKind.CaseNotEqualsClause, isKeyword, operatorToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Shared Function CaseStatement(ByVal caseKeyword As KeywordSyntax, ByVal cases As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(211, caseKeyword, cases.Node, num)
			If (greenNode Is Nothing) Then
				Dim caseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax(SyntaxKind.CaseStatement, caseKeyword, cases.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(caseStatementSyntax1, num)
				End If
				caseStatementSyntax = caseStatementSyntax1
			Else
				caseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)
			End If
			Return caseStatementSyntax
		End Function

		Friend Shared Function CatchBlock(ByVal catchStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax
			Dim catchBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(187, catchStatement, statements.Node, num)
			If (greenNode Is Nothing) Then
				Dim catchBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax(SyntaxKind.CatchBlock, catchStatement, statements.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(catchBlockSyntax1, num)
				End If
				catchBlockSyntax = catchBlockSyntax1
			Else
				catchBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax)
			End If
			Return catchBlockSyntax
		End Function

		Friend Shared Function CatchFilterClause(ByVal whenKeyword As KeywordSyntax, ByVal filter As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax
			Dim catchFilterClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(191, whenKeyword, filter, num)
			If (greenNode Is Nothing) Then
				Dim catchFilterClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax(SyntaxKind.CatchFilterClause, whenKeyword, filter)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(catchFilterClauseSyntax1, num)
				End If
				catchFilterClauseSyntax = catchFilterClauseSyntax1
			Else
				catchFilterClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax)
			End If
			Return catchFilterClauseSyntax
		End Function

		Friend Shared Function CatchStatement(ByVal catchKeyword As KeywordSyntax, ByVal identifierName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal whenClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax(SyntaxKind.CatchStatement, catchKeyword, identifierName, asClause, whenClause)
		End Function

		Friend Shared Function CharacterLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(272, token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.CharacterLiteralExpression, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function CharacterLiteralToken(ByVal text As String, ByVal value As Char, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As CharacterLiteralTokenSyntax
			Return New CharacterLiteralTokenSyntax(SyntaxKind.CharacterLiteralToken, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function ClassBlock(ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(SyntaxKind.ClassBlock, classStatement, [inherits].Node, [implements].Node, members.Node, endClassStatement)
		End Function

		Friend Shared Function ClassConstraint(ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(73, constraintKeyword, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(SyntaxKind.ClassConstraint, constraintKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Shared Function ClassStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal classKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax(SyntaxKind.ClassStatement, attributeLists.Node, modifiers.Node, classKeyword, identifier, typeParameterList)
		End Function

		Friend Shared Function CollectionInitializer(ByVal openBraceToken As PunctuationSyntax, ByVal initializers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(302, openBraceToken, initializers.Node, closeBraceToken, num)
			If (greenNode Is Nothing) Then
				Dim collectionInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(SyntaxKind.CollectionInitializer, openBraceToken, initializers.Node, closeBraceToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(collectionInitializerSyntax1, num)
				End If
				collectionInitializerSyntax = collectionInitializerSyntax1
			Else
				collectionInitializerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)
			End If
			Return collectionInitializerSyntax
		End Function

		Friend Shared Function CollectionRangeVariable(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax(SyntaxKind.CollectionRangeVariable, identifier, asClause, inKeyword, expression)
		End Function

		Friend Shared Function ColonTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.ColonTrivia, text)
		End Function

		Friend Shared Function CommentTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.CommentTrivia, text)
		End Function

		Friend Shared Function CompilationUnit(ByVal options As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [imports] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endOfFileToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(SyntaxKind.CompilationUnit, options.Node, [imports].Node, attributes.Node, members.Node, endOfFileToken)
		End Function

		Friend Shared Function ConcatenateAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(259, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.ConcatenateAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function ConcatenateExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(317, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ConcatenateExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function ConditionalAccessExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal questionMarkToken As PunctuationSyntax, ByVal whenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax
			Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(713, expression, questionMarkToken, whenNotNull, num)
			If (greenNode Is Nothing) Then
				Dim conditionalAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax(SyntaxKind.ConditionalAccessExpression, expression, questionMarkToken, whenNotNull)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(conditionalAccessExpressionSyntax1, num)
				End If
				conditionalAccessExpressionSyntax = conditionalAccessExpressionSyntax1
			Else
				conditionalAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)
			End If
			Return conditionalAccessExpressionSyntax
		End Function

		Friend Shared Function ConflictMarkerTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.ConflictMarkerTrivia, text)
		End Function

		Friend Shared Function ConstDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal constKeyword As KeywordSyntax, ByVal name As IdentifierTokenSyntax, ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax(SyntaxKind.ConstDirectiveTrivia, hashToken, constKeyword, name, equalsToken, value)
		End Function

		Friend Shared Function ConstructorBlock(ByVal subNewStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax
			Dim constructorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(81, subNewStatement, statements.Node, endSubStatement, num)
			If (greenNode Is Nothing) Then
				Dim constructorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax(SyntaxKind.ConstructorBlock, subNewStatement, statements.Node, endSubStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(constructorBlockSyntax1, num)
				End If
				constructorBlockSyntax = constructorBlockSyntax1
			Else
				constructorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax)
			End If
			Return constructorBlockSyntax
		End Function

		Friend Shared Function ContinueDoStatement(ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(167, continueKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(SyntaxKind.ContinueDoStatement, continueKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Shared Function ContinueForStatement(ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(168, continueKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(SyntaxKind.ContinueForStatement, continueKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Shared Function ContinueStatement(ByVal kind As SyntaxKind, ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), continueKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(kind, continueKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Shared Function ContinueWhileStatement(ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(166, continueKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(SyntaxKind.ContinueWhileStatement, continueKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(continueStatementSyntax1, num)
				End If
				continueStatementSyntax = continueStatementSyntax1
			Else
				continueStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)
			End If
			Return continueStatementSyntax
		End Function

		Friend Shared Function CrefOperatorReference(ByVal operatorKeyword As KeywordSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax
			Dim crefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(409, operatorKeyword, operatorToken, num)
			If (greenNode Is Nothing) Then
				Dim crefOperatorReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax(SyntaxKind.CrefOperatorReference, operatorKeyword, operatorToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefOperatorReferenceSyntax1, num)
				End If
				crefOperatorReferenceSyntax = crefOperatorReferenceSyntax1
			Else
				crefOperatorReferenceSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)
			End If
			Return crefOperatorReferenceSyntax
		End Function

		Friend Shared Function CrefReference(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax
			Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(404, name, signature, asClause, num)
			If (greenNode Is Nothing) Then
				Dim crefReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(SyntaxKind.CrefReference, name, signature, asClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefReferenceSyntax1, num)
				End If
				crefReferenceSyntax = crefReferenceSyntax1
			Else
				crefReferenceSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax)
			End If
			Return crefReferenceSyntax
		End Function

		Friend Shared Function CrefSignature(ByVal openParenToken As PunctuationSyntax, ByVal argumentTypes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax
			Dim crefSignatureSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(407, openParenToken, argumentTypes.Node, closeParenToken, num)
			If (greenNode Is Nothing) Then
				Dim crefSignatureSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax(SyntaxKind.CrefSignature, openParenToken, argumentTypes.Node, closeParenToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefSignatureSyntax1, num)
				End If
				crefSignatureSyntax = crefSignatureSyntax1
			Else
				crefSignatureSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)
			End If
			Return crefSignatureSyntax
		End Function

		Friend Shared Function CrefSignaturePart(ByVal modifier As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax
			Dim crefSignaturePartSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(408, modifier, type, num)
			If (greenNode Is Nothing) Then
				Dim crefSignaturePartSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax(SyntaxKind.CrefSignaturePart, modifier, type)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(crefSignaturePartSyntax1, num)
				End If
				crefSignaturePartSyntax = crefSignaturePartSyntax1
			Else
				crefSignaturePartSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax)
			End If
			Return crefSignaturePartSyntax
		End Function

		Friend Shared Function CTypeExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax(SyntaxKind.CTypeExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		End Function

		Friend Shared Function DateLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(276, token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.DateLiteralExpression, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function DateLiteralToken(ByVal text As String, ByVal value As DateTime, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As DateLiteralTokenSyntax
			Return New DateLiteralTokenSyntax(SyntaxKind.DateLiteralToken, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function DecimalLiteralToken(ByVal text As String, ByVal typeSuffix As TypeCharacter, ByVal value As [Decimal], ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As DecimalLiteralTokenSyntax
			Return New DecimalLiteralTokenSyntax(SyntaxKind.DecimalLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, value)
		End Function

		Friend Shared Function DeclareFunctionStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(SyntaxKind.DeclareFunctionStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause)
		End Function

		Friend Shared Function DeclareStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(kind, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause)
		End Function

		Friend Shared Function DeclareSubStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(SyntaxKind.DeclareSubStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause)
		End Function

		Friend Shared Function DelegateFunctionStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal delegateKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(SyntaxKind.DelegateFunctionStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause)
		End Function

		Friend Shared Function DelegateStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal delegateKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(kind, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause)
		End Function

		Friend Shared Function DelegateSubStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal delegateKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(SyntaxKind.DelegateSubStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause)
		End Function

		Friend Shared Function DescendingOrdering(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal ascendingOrDescendingKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(376, expression, ascendingOrDescendingKeyword, num)
			If (greenNode Is Nothing) Then
				Dim orderingSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(SyntaxKind.DescendingOrdering, expression, ascendingOrDescendingKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderingSyntax1, num)
				End If
				orderingSyntax = orderingSyntax1
			Else
				orderingSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			End If
			Return orderingSyntax
		End Function

		Friend Shared Function DictionaryAccessExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(292, expression, operatorToken, name, num)
			If (greenNode Is Nothing) Then
				Dim memberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(SyntaxKind.DictionaryAccessExpression, expression, operatorToken, name)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(memberAccessExpressionSyntax1, num)
				End If
				memberAccessExpressionSyntax = memberAccessExpressionSyntax1
			Else
				memberAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
			End If
			Return memberAccessExpressionSyntax
		End Function

		Friend Shared Function DirectCastExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax(SyntaxKind.DirectCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		End Function

		Friend Shared Function DisabledTextTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.DisabledTextTrivia, text)
		End Function

		Friend Shared Function DisableWarningDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal disableKeyword As KeywordSyntax, ByVal warningKeyword As KeywordSyntax, ByVal errorCodes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax(SyntaxKind.DisableWarningDirectiveTrivia, hashToken, disableKeyword, warningKeyword, errorCodes.Node)
		End Function

		Friend Shared Function DistinctClause(ByVal distinctKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax
			Dim distinctClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(362, distinctKeyword, num)
			If (greenNode Is Nothing) Then
				Dim distinctClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax(SyntaxKind.DistinctClause, distinctKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(distinctClauseSyntax1, num)
				End If
				distinctClauseSyntax = distinctClauseSyntax1
			Else
				distinctClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax)
			End If
			Return distinctClauseSyntax
		End Function

		Friend Shared Function DivideAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(252, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.DivideAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function DivideExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(310, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.DivideExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function DocumentationCommentExteriorTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, text)
		End Function

		Friend Shared Function DocumentationCommentLineBreakToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(SyntaxKind.DocumentationCommentLineBreakToken, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function DocumentationCommentTrivia(ByVal content As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax(SyntaxKind.DocumentationCommentTrivia, content.Node)
		End Function

		Friend Shared Function DoLoopBlock(ByVal kind As SyntaxKind, ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), doStatement, statements.Node, loopStatement, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(kind, doStatement, statements.Node, loopStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Shared Function DoLoopUntilBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(760, doStatement, statements.Node, loopStatement, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoLoopUntilBlock, doStatement, statements.Node, loopStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Shared Function DoLoopWhileBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(759, doStatement, statements.Node, loopStatement, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoLoopWhileBlock, doStatement, statements.Node, loopStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Shared Function DoStatement(ByVal kind As SyntaxKind, ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), doKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(kind, doKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Shared Function DoUntilLoopBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(758, doStatement, statements.Node, loopStatement, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoUntilLoopBlock, doStatement, statements.Node, loopStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Shared Function DoUntilStatement(ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(772, doKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(SyntaxKind.DoUntilStatement, doKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Shared Function DoWhileLoopBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(757, doStatement, statements.Node, loopStatement, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.DoWhileLoopBlock, doStatement, statements.Node, loopStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Shared Function DoWhileStatement(ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(771, doKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(SyntaxKind.DoWhileStatement, doKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Shared Function ElseBlock(ByVal elseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax
			Dim elseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(181, elseStatement, statements.Node, num)
			If (greenNode Is Nothing) Then
				Dim elseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax(SyntaxKind.ElseBlock, elseStatement, statements.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseBlockSyntax1, num)
				End If
				elseBlockSyntax = elseBlockSyntax1
			Else
				elseBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax)
			End If
			Return elseBlockSyntax
		End Function

		Friend Shared Function ElseCaseClause(ByVal elseKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax
			Dim elseCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(213, elseKeyword, num)
			If (greenNode Is Nothing) Then
				Dim elseCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax(SyntaxKind.ElseCaseClause, elseKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseCaseClauseSyntax1, num)
				End If
				elseCaseClauseSyntax = elseCaseClauseSyntax1
			Else
				elseCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax)
			End If
			Return elseCaseClauseSyntax
		End Function

		Friend Shared Function ElseDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax(SyntaxKind.ElseDirectiveTrivia, hashToken, elseKeyword)
		End Function

		Friend Shared Function ElseIfBlock(ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax
			Dim elseIfBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(180, elseIfStatement, statements.Node, num)
			If (greenNode Is Nothing) Then
				Dim elseIfBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(SyntaxKind.ElseIfBlock, elseIfStatement, statements.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseIfBlockSyntax1, num)
				End If
				elseIfBlockSyntax = elseIfBlockSyntax1
			Else
				elseIfBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax)
			End If
			Return elseIfBlockSyntax
		End Function

		Friend Shared Function ElseIfDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(SyntaxKind.ElseIfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword)
		End Function

		Friend Shared Function ElseIfStatement(ByVal elseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax
			Dim elseIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(183, elseIfKeyword, condition, thenKeyword, num)
			If (greenNode Is Nothing) Then
				Dim elseIfStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax(SyntaxKind.ElseIfStatement, elseIfKeyword, condition, thenKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseIfStatementSyntax1, num)
				End If
				elseIfStatementSyntax = elseIfStatementSyntax1
			Else
				elseIfStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax)
			End If
			Return elseIfStatementSyntax
		End Function

		Friend Shared Function ElseStatement(ByVal elseKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax
			Dim elseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(184, elseKeyword, num)
			If (greenNode Is Nothing) Then
				Dim elseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax(SyntaxKind.ElseStatement, elseKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(elseStatementSyntax1, num)
				End If
				elseStatementSyntax = elseStatementSyntax1
			Else
				elseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax)
			End If
			Return elseStatementSyntax
		End Function

		Friend Shared Function EmptyStatement(ByVal empty As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax
			Dim emptyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(2, empty, num)
			If (greenNode Is Nothing) Then
				Dim emptyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax(SyntaxKind.EmptyStatement, empty)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(emptyStatementSyntax1, num)
				End If
				emptyStatementSyntax = emptyStatementSyntax1
			Else
				emptyStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax)
			End If
			Return emptyStatementSyntax
		End Function

		Friend Shared Function EmptyStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_emptyStatement
		End Function

		Friend Shared Function EnableWarningDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal enableKeyword As KeywordSyntax, ByVal warningKeyword As KeywordSyntax, ByVal errorCodes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax(SyntaxKind.EnableWarningDirectiveTrivia, hashToken, enableKeyword, warningKeyword, errorCodes.Node)
		End Function

		Friend Shared Function EndAddHandlerStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(22, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndAddHandlerStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndBlockStatement(ByVal kind As SyntaxKind, ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(kind, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndClassStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(12, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndClassStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndEnumStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(10, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndEnumStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndEventStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(21, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndEventStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndExternalSourceDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal externalSourceKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax(SyntaxKind.EndExternalSourceDirectiveTrivia, hashToken, endKeyword, externalSourceKeyword)
		End Function

		Friend Shared Function EndFunctionStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(16, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndFunctionStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndGetStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(17, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndGetStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndIfDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal ifKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax(SyntaxKind.EndIfDirectiveTrivia, hashToken, endKeyword, ifKeyword)
		End Function

		Friend Shared Function EndIfStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(5, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndIfStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndInterfaceStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(11, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndInterfaceStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndModuleStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(13, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndModuleStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndNamespaceStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(14, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndNamespaceStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndOfFileToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia) As PunctuationSyntax
			Return New PunctuationSyntax(SyntaxKind.EndOfFileToken, "", precedingTrivia, Nothing)
		End Function

		Friend Shared Function EndOfFileToken() As PunctuationSyntax
			Return New PunctuationSyntax(SyntaxKind.EndOfFileToken, "", Nothing, Nothing)
		End Function

		Friend Shared Function EndOfLine(ByVal text As String, Optional ByVal elastic As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Nothing
			Dim str As String = text
			If (EmbeddedOperators.CompareString(str, "", False) = 0) Then
				syntaxTrivium1 = If(elastic, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturn, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CarriageReturn)
			ElseIf (EmbeddedOperators.CompareString(str, "" & VbCrLf & "", False) = 0) Then
				syntaxTrivium1 = If(elastic, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticLineFeed, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.LineFeed)
			ElseIf (EmbeddedOperators.CompareString(str, "" & VbCrLf & "", False) = 0) Then
				syntaxTrivium1 = If(elastic, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturnLineFeed, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CarriageReturnLineFeed)
			End If
			If (syntaxTrivium1 Is Nothing) Then
				syntaxTrivium1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, text)
				syntaxTrivium = If(elastic, syntaxTrivium1.WithAnnotations(New SyntaxAnnotation() { SyntaxAnnotation.ElasticAnnotation }), syntaxTrivium1)
			Else
				syntaxTrivium = syntaxTrivium1
			End If
			Return syntaxTrivium
		End Function

		Friend Shared Function EndOfLineTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, text)
		End Function

		Friend Shared Function EndOperatorStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(20, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndOperatorStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndPropertyStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(19, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndPropertyStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndRaiseEventStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(24, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndRaiseEventStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndRegionDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal regionKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax(SyntaxKind.EndRegionDirectiveTrivia, hashToken, endKeyword, regionKeyword)
		End Function

		Friend Shared Function EndRemoveHandlerStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(23, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndRemoveHandlerStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndSelectStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(8, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSelectStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndSetStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(18, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSetStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndStatement(ByVal stopOrEndKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim stopOrEndStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(156, stopOrEndKeyword, num)
			If (greenNode Is Nothing) Then
				Dim stopOrEndStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(SyntaxKind.EndStatement, stopOrEndKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(stopOrEndStatementSyntax1, num)
				End If
				stopOrEndStatementSyntax = stopOrEndStatementSyntax1
			Else
				stopOrEndStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)
			End If
			Return stopOrEndStatementSyntax
		End Function

		Friend Shared Function EndStructureStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(9, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndStructureStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndSubStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(15, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSubStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndSyncLockStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(27, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndSyncLockStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndTryStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(26, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndTryStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndUsingStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(6, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndUsingStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndWhileStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(25, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndWhileStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EndWithStatement(ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(7, endKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(SyntaxKind.EndWithStatement, endKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(endBlockStatementSyntax1, num)
				End If
				endBlockStatementSyntax = endBlockStatementSyntax1
			Else
				endBlockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			End If
			Return endBlockStatementSyntax
		End Function

		Friend Shared Function EnumBlock(ByVal enumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax, ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endEnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax
			Dim enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(54, enumStatement, members.Node, endEnumStatement, num)
			If (greenNode Is Nothing) Then
				Dim enumBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax(SyntaxKind.EnumBlock, enumStatement, members.Node, endEnumStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(enumBlockSyntax1, num)
				End If
				enumBlockSyntax = enumBlockSyntax1
			Else
				enumBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax)
			End If
			Return enumBlockSyntax
		End Function

		Friend Shared Function EnumMemberDeclaration(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal identifier As IdentifierTokenSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax
			Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(78, attributeLists.Node, identifier, initializer, num)
			If (greenNode Is Nothing) Then
				Dim enumMemberDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax(SyntaxKind.EnumMemberDeclaration, attributeLists.Node, identifier, initializer)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(enumMemberDeclarationSyntax1, num)
				End If
				enumMemberDeclarationSyntax = enumMemberDeclarationSyntax1
			Else
				enumMemberDeclarationSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax)
			End If
			Return enumMemberDeclarationSyntax
		End Function

		Friend Shared Function EnumStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal enumKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal underlyingType As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax(SyntaxKind.EnumStatement, attributeLists.Node, modifiers.Node, enumKeyword, identifier, underlyingType)
		End Function

		Friend Shared Function EqualsExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(319, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.EqualsExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function EqualsValue(ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(129, equalsToken, value, num)
			If (greenNode Is Nothing) Then
				Dim equalsValueSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax(SyntaxKind.EqualsValue, equalsToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(equalsValueSyntax1, num)
				End If
				equalsValueSyntax = equalsValueSyntax1
			Else
				equalsValueSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			End If
			Return equalsValueSyntax
		End Function

		Friend Shared Function EraseStatement(ByVal eraseKeyword As KeywordSyntax, ByVal expressions As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax
			Dim eraseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(271, eraseKeyword, expressions.Node, num)
			If (greenNode Is Nothing) Then
				Dim eraseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax(SyntaxKind.EraseStatement, eraseKeyword, expressions.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(eraseStatementSyntax1, num)
				End If
				eraseStatementSyntax = eraseStatementSyntax1
			Else
				eraseStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax)
			End If
			Return eraseStatementSyntax
		End Function

		Friend Shared Function ErrorStatement(ByVal errorKeyword As KeywordSyntax, ByVal errorNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax
			Dim errorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(195, errorKeyword, errorNumber, num)
			If (greenNode Is Nothing) Then
				Dim errorStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax(SyntaxKind.ErrorStatement, errorKeyword, errorNumber)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(errorStatementSyntax1, num)
				End If
				errorStatementSyntax = errorStatementSyntax1
			Else
				errorStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax)
			End If
			Return errorStatementSyntax
		End Function

		Friend Shared Function EventBlock(ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax, ByVal accessors As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax
			Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(89, eventStatement, accessors.Node, endEventStatement, num)
			If (greenNode Is Nothing) Then
				Dim eventBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(SyntaxKind.EventBlock, eventStatement, accessors.Node, endEventStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(eventBlockSyntax1, num)
				End If
				eventBlockSyntax = eventBlockSyntax1
			Else
				eventBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax)
			End If
			Return eventBlockSyntax
		End Function

		Friend Shared Function EventStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal customKeyword As KeywordSyntax, ByVal eventKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(SyntaxKind.EventStatement, attributeLists.Node, modifiers.Node, customKeyword, eventKeyword, identifier, parameterList, asClause, implementsClause)
		End Function

		Friend Shared Function ExclusiveOrExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(329, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ExclusiveOrExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function ExitDoStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(157, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitDoStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitForStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(158, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitForStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitFunctionStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(160, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitFunctionStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitOperatorStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(161, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitOperatorStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitPropertyStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(162, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitPropertyStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitSelectStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(164, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitSelectStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitStatement(ByVal kind As SyntaxKind, ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(kind, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitSubStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(159, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitSubStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitTryStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(163, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitTryStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExitWhileStatement(ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(165, exitKeyword, blockKeyword, num)
			If (greenNode Is Nothing) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(SyntaxKind.ExitWhileStatement, exitKeyword, blockKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(exitStatementSyntax1, num)
				End If
				exitStatementSyntax = exitStatementSyntax1
			Else
				exitStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)
			End If
			Return exitStatementSyntax
		End Function

		Friend Shared Function ExponentiateAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(254, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.ExponentiateAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function ExponentiateExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(314, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ExponentiateExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function ExpressionRangeVariable(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax
			Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(354, nameEquals, expression, num)
			If (greenNode Is Nothing) Then
				Dim expressionRangeVariableSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax(SyntaxKind.ExpressionRangeVariable, nameEquals, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(expressionRangeVariableSyntax1, num)
				End If
				expressionRangeVariableSyntax = expressionRangeVariableSyntax1
			Else
				expressionRangeVariableSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)
			End If
			Return expressionRangeVariableSyntax
		End Function

		Friend Shared Function ExpressionStatement(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax
			Dim expressionStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(139, expression, num)
			If (greenNode Is Nothing) Then
				Dim expressionStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax(SyntaxKind.ExpressionStatement, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(expressionStatementSyntax1, num)
				End If
				expressionStatementSyntax = expressionStatementSyntax1
			Else
				expressionStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax)
			End If
			Return expressionStatementSyntax
		End Function

		Friend Shared Function ExternalChecksumDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal externalChecksumKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal guid As StringLiteralTokenSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal checksum As StringLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(SyntaxKind.ExternalChecksumDirectiveTrivia, hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken)
		End Function

		Friend Shared Function ExternalSourceDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal externalSourceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal commaToken As PunctuationSyntax, ByVal lineStart As IntegerLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(SyntaxKind.ExternalSourceDirectiveTrivia, hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken)
		End Function

		Friend Shared Function FalseLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(274, token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function FieldDeclaration(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal declarators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax
			Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(119, attributeLists.Node, modifiers.Node, declarators.Node, num)
			If (greenNode Is Nothing) Then
				Dim fieldDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax(SyntaxKind.FieldDeclaration, attributeLists.Node, modifiers.Node, declarators.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(fieldDeclarationSyntax1, num)
				End If
				fieldDeclarationSyntax = fieldDeclarationSyntax1
			Else
				fieldDeclarationSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)
			End If
			Return fieldDeclarationSyntax
		End Function

		Friend Shared Function FinallyBlock(ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax
			Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(188, finallyStatement, statements.Node, num)
			If (greenNode Is Nothing) Then
				Dim finallyBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(SyntaxKind.FinallyBlock, finallyStatement, statements.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(finallyBlockSyntax1, num)
				End If
				finallyBlockSyntax = finallyBlockSyntax1
			Else
				finallyBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax)
			End If
			Return finallyBlockSyntax
		End Function

		Friend Shared Function FinallyStatement(ByVal finallyKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax
			Dim finallyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(194, finallyKeyword, num)
			If (greenNode Is Nothing) Then
				Dim finallyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax(SyntaxKind.FinallyStatement, finallyKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(finallyStatementSyntax1, num)
				End If
				finallyStatementSyntax = finallyStatementSyntax1
			Else
				finallyStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax)
			End If
			Return finallyStatementSyntax
		End Function

		Friend Shared Function FloatingLiteralToken(ByVal text As String, ByVal typeSuffix As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal value As Double, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FloatingLiteralTokenSyntax
			Dim floatingLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FloatingLiteralTokenSyntax
			Dim typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter = typeSuffix
			If (typeCharacter <= Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Single]) Then
				If (typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None) Then
					floatingLiteralTokenSyntax = New FloatingLiteralTokenSyntax(Of Double)(SyntaxKind.FloatingLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, value)
					Return floatingLiteralTokenSyntax
				End If
				If (typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Single]) Then
					floatingLiteralTokenSyntax = New FloatingLiteralTokenSyntax(Of Single)(SyntaxKind.FloatingLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, CSng(value))
					Return floatingLiteralTokenSyntax
				End If
				Throw New ArgumentException("typeSuffix")
			ElseIf (typeCharacter <> Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Double]) Then
				If (typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.SingleLiteral) Then
					floatingLiteralTokenSyntax = New FloatingLiteralTokenSyntax(Of Single)(SyntaxKind.FloatingLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, CSng(value))
					Return floatingLiteralTokenSyntax
				End If
				If (typeCharacter <> Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.DoubleLiteral) Then
					Throw New ArgumentException("typeSuffix")
				End If
			End If
			floatingLiteralTokenSyntax = New FloatingLiteralTokenSyntax(Of Double)(SyntaxKind.FloatingLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, value)
			Return floatingLiteralTokenSyntax
		End Function

		Friend Shared Function ForBlock(ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax
			Dim forBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(237, forStatement, statements.Node, nextStatement, num)
			If (greenNode Is Nothing) Then
				Dim forBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(SyntaxKind.ForBlock, forStatement, statements.Node, nextStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(forBlockSyntax1, num)
				End If
				forBlockSyntax = forBlockSyntax1
			Else
				forBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax)
			End If
			Return forBlockSyntax
		End Function

		Friend Shared Function ForEachBlock(ByVal forEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax
			Dim forEachBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(238, forEachStatement, statements.Node, nextStatement, num)
			If (greenNode Is Nothing) Then
				Dim forEachBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax(SyntaxKind.ForEachBlock, forEachStatement, statements.Node, nextStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(forEachBlockSyntax1, num)
				End If
				forEachBlockSyntax = forEachBlockSyntax1
			Else
				forEachBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax)
			End If
			Return forEachBlockSyntax
		End Function

		Friend Shared Function ForEachStatement(ByVal forKeyword As KeywordSyntax, ByVal eachKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(SyntaxKind.ForEachStatement, forKeyword, eachKeyword, controlVariable, inKeyword, expression)
		End Function

		Friend Shared Function ForStatement(ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal equalsToken As PunctuationSyntax, ByVal fromValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal toValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal stepClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax(SyntaxKind.ForStatement, forKeyword, controlVariable, equalsToken, fromValue, toKeyword, toValue, stepClause)
		End Function

		Friend Shared Function ForStepClause(ByVal stepKeyword As KeywordSyntax, ByVal stepValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax
			Dim forStepClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(240, stepKeyword, stepValue, num)
			If (greenNode Is Nothing) Then
				Dim forStepClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax(SyntaxKind.ForStepClause, stepKeyword, stepValue)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(forStepClauseSyntax1, num)
				End If
				forStepClauseSyntax = forStepClauseSyntax1
			Else
				forStepClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)
			End If
			Return forStepClauseSyntax
		End Function

		Friend Shared Function FromClause(ByVal fromKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax
			Dim fromClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(359, fromKeyword, variables.Node, num)
			If (greenNode Is Nothing) Then
				Dim fromClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(SyntaxKind.FromClause, fromKeyword, variables.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(fromClauseSyntax1, num)
				End If
				fromClauseSyntax = fromClauseSyntax1
			Else
				fromClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax)
			End If
			Return fromClauseSyntax
		End Function

		Friend Shared Function FunctionAggregation(ByVal functionName As IdentifierTokenSyntax, ByVal openParenToken As PunctuationSyntax, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax(SyntaxKind.FunctionAggregation, functionName, openParenToken, argument, closeParenToken)
		End Function

		Friend Shared Function FunctionBlock(ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim methodBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(80, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, num)
			If (greenNode Is Nothing) Then
				Dim methodBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(SyntaxKind.FunctionBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(methodBlockSyntax1, num)
				End If
				methodBlockSyntax = methodBlockSyntax1
			Else
				methodBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax)
			End If
			Return methodBlockSyntax
		End Function

		Friend Shared Function FunctionLambdaHeader(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(SyntaxKind.FunctionLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause)
		End Function

		Friend Shared Function FunctionStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(SyntaxKind.FunctionStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause)
		End Function

		Friend Shared Function GenericName(ByVal identifier As IdentifierTokenSyntax, ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax
			Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(400, identifier, typeArgumentList, num)
			If (greenNode Is Nothing) Then
				Dim genericNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(SyntaxKind.GenericName, identifier, typeArgumentList)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(genericNameSyntax1, num)
				End If
				genericNameSyntax = genericNameSyntax1
			Else
				genericNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax)
			End If
			Return genericNameSyntax
		End Function

		Friend Shared Function GetAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(83, accessorStatement, statements.Node, endAccessorStatement, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.GetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Shared Function GetAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.GetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList)
		End Function

		Friend Shared Function GetNodeTypes() As IEnumerable(Of Object)
			Return New [Object]() { GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclarationStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsOrImplementsStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InstanceExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BaseXmlAttributeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken), GetType(KeywordSyntax), GetType(PunctuationSyntax), GetType(BadTokenSyntax), GetType(XmlNameTokenSyntax), GetType(XmlTextTokenSyntax), GetType(InterpolatedStringTextTokenSyntax), GetType(IdentifierTokenSyntax), GetType(IntegerLiteralTokenSyntax), GetType(FloatingLiteralTokenSyntax), GetType(DecimalLiteralTokenSyntax), GetType(DateLiteralTokenSyntax), GetType(StringLiteralTokenSyntax), GetType(CharacterLiteralTokenSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructuredTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax), GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax) }
		End Function

		Friend Shared Function GetTypeExpression(ByVal getTypeKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax(SyntaxKind.GetTypeExpression, getTypeKeyword, openParenToken, type, closeParenToken)
		End Function

		Friend Shared Function GetWellKnownTrivia() As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia() { Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CarriageReturn, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CarriageReturnLineFeed, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.LineFeed, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Space, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Tab, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturn, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticLineFeed, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturnLineFeed, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticSpace, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticTab, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ElasticZeroSpace, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace("  ", False), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace("   ", False), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Whitespace("    ", False) }
		End Function

		Friend Shared Function GetXmlNamespaceExpression(ByVal getXmlNamespaceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(SyntaxKind.GetXmlNamespaceExpression, getXmlNamespaceKeyword, openParenToken, name, closeParenToken)
		End Function

		Friend Shared Function GlobalName(ByVal globalKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax
			Dim globalNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(402, globalKeyword, num)
			If (greenNode Is Nothing) Then
				Dim globalNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax(SyntaxKind.GlobalName, globalKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(globalNameSyntax1, num)
				End If
				globalNameSyntax = globalNameSyntax1
			Else
				globalNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax)
			End If
			Return globalNameSyntax
		End Function

		Friend Shared Function GoToStatement(ByVal goToKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax
			Dim goToStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(149, goToKeyword, label, num)
			If (greenNode Is Nothing) Then
				Dim goToStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax(SyntaxKind.GoToStatement, goToKeyword, label)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(goToStatementSyntax1, num)
				End If
				goToStatementSyntax = goToStatementSyntax1
			Else
				goToStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax)
			End If
			Return goToStatementSyntax
		End Function

		Friend Shared Function GreaterThanExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(324, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.GreaterThanExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function GreaterThanOrEqualExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(323, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.GreaterThanOrEqualExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function GroupAggregation(ByVal groupKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax
			Dim groupAggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(358, groupKeyword, num)
			If (greenNode Is Nothing) Then
				Dim groupAggregationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax(SyntaxKind.GroupAggregation, groupKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(groupAggregationSyntax1, num)
				End If
				groupAggregationSyntax = groupAggregationSyntax1
			Else
				groupAggregationSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax)
			End If
			Return groupAggregationSyntax
		End Function

		Friend Shared Function GroupByClause(ByVal groupKeyword As KeywordSyntax, ByVal items As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal byKeyword As KeywordSyntax, ByVal keys As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(SyntaxKind.GroupByClause, groupKeyword, items.Node, byKeyword, keys.Node, intoKeyword, aggregationVariables.Node)
		End Function

		Friend Shared Function GroupJoinClause(ByVal groupKeyword As KeywordSyntax, ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal additionalJoins As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal onKeyword As KeywordSyntax, ByVal joinConditions As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal intoKeyword As KeywordSyntax, ByVal aggregationVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(SyntaxKind.GroupJoinClause, groupKeyword, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node, intoKeyword, aggregationVariables.Node)
		End Function

		Friend Shared Function HandlesClause(ByVal handlesKeyword As KeywordSyntax, ByVal events As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(113, handlesKeyword, events.Node, num)
			If (greenNode Is Nothing) Then
				Dim handlesClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax(SyntaxKind.HandlesClause, handlesKeyword, events.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(handlesClauseSyntax1, num)
				End If
				handlesClauseSyntax = handlesClauseSyntax1
			Else
				handlesClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)
			End If
			Return handlesClauseSyntax
		End Function

		Friend Shared Function HandlesClauseItem(ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax
			Dim handlesClauseItemSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(117, eventContainer, dotToken, eventMember, num)
			If (greenNode Is Nothing) Then
				Dim handlesClauseItemSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(SyntaxKind.HandlesClauseItem, eventContainer, dotToken, eventMember)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(handlesClauseItemSyntax1, num)
				End If
				handlesClauseItemSyntax = handlesClauseItemSyntax1
			Else
				handlesClauseItemSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)
			End If
			Return handlesClauseItemSyntax
		End Function

		Friend Shared Function Identifier(ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As IdentifierTokenSyntax
			Return New SimpleIdentifierSyntax(SyntaxKind.IdentifierToken, Nothing, Nothing, text, leadingTrivia, trailingTrivia)
		End Function

		Friend Shared Function Identifier(ByVal text As String, ByVal possibleKeywordKind As SyntaxKind, ByVal isBracketed As Boolean, ByVal identifierText As String, ByVal typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As IdentifierTokenSyntax
			Return New ComplexIdentifierSyntax(SyntaxKind.IdentifierToken, Nothing, Nothing, text, leadingTrivia, trailingTrivia, possibleKeywordKind, isBracketed, identifierText, typeCharacter)
		End Function

		Friend Shared Function Identifier(ByVal text As String) As IdentifierTokenSyntax
			Return New SimpleIdentifierSyntax(SyntaxKind.IdentifierToken, Nothing, Nothing, text, Nothing, Nothing)
		End Function

		Friend Shared Function Identifier(ByVal text As String, ByVal isBracketed As Boolean, ByVal baseText As String, ByVal typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode) As IdentifierTokenSyntax
			Return New ComplexIdentifierSyntax(SyntaxKind.IdentifierToken, Nothing, Nothing, text, precedingTrivia, followingTrivia, SyntaxKind.IdentifierToken, isBracketed, baseText, typeCharacter)
		End Function

		Friend Shared Function IdentifierLabel(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(150, labelToken, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(SyntaxKind.IdentifierLabel, labelToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Shared Function IdentifierName(ByVal identifier As IdentifierTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(399, identifier, num)
			If (greenNode Is Nothing) Then
				Dim identifierNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax(SyntaxKind.IdentifierName, identifier)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(identifierNameSyntax1, num)
				End If
				identifierNameSyntax = identifierNameSyntax1
			Else
				identifierNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			End If
			Return identifierNameSyntax
		End Function

		Friend Shared Function IfDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(SyntaxKind.IfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword)
		End Function

		Friend Shared Function IfStatement(ByVal ifKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax
			Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(182, ifKeyword, condition, thenKeyword, num)
			If (greenNode Is Nothing) Then
				Dim ifStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax(SyntaxKind.IfStatement, ifKeyword, condition, thenKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(ifStatementSyntax1, num)
				End If
				ifStatementSyntax = ifStatementSyntax1
			Else
				ifStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
			End If
			Return ifStatementSyntax
		End Function

		Friend Shared Function ImplementsClause(ByVal implementsKeyword As KeywordSyntax, ByVal interfaceMembers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(112, implementsKeyword, interfaceMembers.Node, num)
			If (greenNode Is Nothing) Then
				Dim implementsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax(SyntaxKind.ImplementsClause, implementsKeyword, interfaceMembers.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(implementsClauseSyntax1, num)
				End If
				implementsClauseSyntax = implementsClauseSyntax1
			Else
				implementsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			End If
			Return implementsClauseSyntax
		End Function

		Friend Shared Function ImplementsStatement(ByVal implementsKeyword As KeywordSyntax, ByVal types As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax
			Dim implementsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(58, implementsKeyword, types.Node, num)
			If (greenNode Is Nothing) Then
				Dim implementsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax(SyntaxKind.ImplementsStatement, implementsKeyword, types.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(implementsStatementSyntax1, num)
				End If
				implementsStatementSyntax = implementsStatementSyntax1
			Else
				implementsStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)
			End If
			Return implementsStatementSyntax
		End Function

		Friend Shared Function ImportAliasClause(ByVal identifier As IdentifierTokenSyntax, ByVal equalsToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax
			Dim importAliasClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(754, identifier, equalsToken, num)
			If (greenNode Is Nothing) Then
				Dim importAliasClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax(SyntaxKind.ImportAliasClause, identifier, equalsToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(importAliasClauseSyntax1, num)
				End If
				importAliasClauseSyntax = importAliasClauseSyntax1
			Else
				importAliasClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)
			End If
			Return importAliasClauseSyntax
		End Function

		Friend Shared Function ImportsStatement(ByVal importsKeyword As KeywordSyntax, ByVal importsClauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax
			Dim importsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(42, importsKeyword, importsClauses.Node, num)
			If (greenNode Is Nothing) Then
				Dim importsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax(SyntaxKind.ImportsStatement, importsKeyword, importsClauses.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(importsStatementSyntax1, num)
				End If
				importsStatementSyntax = importsStatementSyntax1
			Else
				importsStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)
			End If
			Return importsStatementSyntax
		End Function

		Friend Shared Function IncompleteMember(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal missingIdentifier As IdentifierTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax
			Dim incompleteMemberSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(118, attributeLists.Node, modifiers.Node, missingIdentifier, num)
			If (greenNode Is Nothing) Then
				Dim incompleteMemberSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(SyntaxKind.IncompleteMember, attributeLists.Node, modifiers.Node, missingIdentifier)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(incompleteMemberSyntax1, num)
				End If
				incompleteMemberSyntax = incompleteMemberSyntax1
			Else
				incompleteMemberSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax)
			End If
			Return incompleteMemberSyntax
		End Function

		Friend Shared Function InferredFieldInitializer(ByVal keyKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax
			Dim inferredFieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(127, keyKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim inferredFieldInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax(SyntaxKind.InferredFieldInitializer, keyKeyword, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(inferredFieldInitializerSyntax1, num)
				End If
				inferredFieldInitializerSyntax = inferredFieldInitializerSyntax1
			Else
				inferredFieldInitializerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax)
			End If
			Return inferredFieldInitializerSyntax
		End Function

		Friend Shared Function InheritsStatement(ByVal inheritsKeyword As KeywordSyntax, ByVal types As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax
			Dim inheritsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(57, inheritsKeyword, types.Node, num)
			If (greenNode Is Nothing) Then
				Dim inheritsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax(SyntaxKind.InheritsStatement, inheritsKeyword, types.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(inheritsStatementSyntax1, num)
				End If
				inheritsStatementSyntax = inheritsStatementSyntax1
			Else
				inheritsStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)
			End If
			Return inheritsStatementSyntax
		End Function

		Friend Shared Function IntegerDivideAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(253, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.IntegerDivideAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function IntegerDivideExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(311, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.IntegerDivideExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function IntegerLiteralToken(ByVal text As String, ByVal base As LiteralBase, ByVal typeSuffix As TypeCharacter, ByVal value As ULong, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax
			Dim integerLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax
			Select Case typeSuffix
				Case TypeCharacter.None
					Dim flag As Boolean = False
					flag = If(base <> LiteralBase.[Decimal], [Decimal].Compare(New [Decimal](value And -4294967296L), [Decimal].Zero) = 0, [Decimal].Compare(New [Decimal](value), New [Decimal](CLng(2147483647))) <= 0)
					If (Not flag) Then
						integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of Long)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, CLng(value))
						Exit Select
					Else
						integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of Integer)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, CInt(value))
						Exit Select
					End If
				Case TypeCharacter.[Integer]
				Case TypeCharacter.IntegerLiteral
					integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of Integer)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, CInt(value))
					Exit Select
				Case TypeCharacter.[Long]
				Case TypeCharacter.LongLiteral
					integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of Long)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, CLng(value))
					Exit Select
				Case TypeCharacter.[Decimal]
				Case TypeCharacter.[Single]
				Case TypeCharacter.[Double]
				Case TypeCharacter.[String]
					Throw New ArgumentException("typeSuffix")
				Case TypeCharacter.ShortLiteral
					integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of Short)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, CShort(value))
					Exit Select
				Case TypeCharacter.UShortLiteral
					integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of UShort)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, CUShort(value))
					Exit Select
				Case TypeCharacter.UIntegerLiteral
					integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of UInteger)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, CUInt(value))
					Exit Select
				Case TypeCharacter.ULongLiteral
					integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of ULong)(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, base, typeSuffix, value)
					Exit Select
				Case Else
					Throw New ArgumentException("typeSuffix")
			End Select
			Return integerLiteralTokenSyntax
		End Function

		Friend Shared Function InterfaceBlock(ByVal interfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endInterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(SyntaxKind.InterfaceBlock, interfaceStatement, [inherits].Node, [implements].Node, members.Node, endInterfaceStatement)
		End Function

		Friend Shared Function InterfaceStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal interfaceKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax(SyntaxKind.InterfaceStatement, attributeLists.Node, modifiers.Node, interfaceKeyword, identifier, typeParameterList)
		End Function

		Friend Shared Function InterpolatedStringExpression(ByVal dollarSignDoubleQuoteToken As PunctuationSyntax, ByVal contents As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal doubleQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax
			Dim interpolatedStringExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(780, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken, num)
			If (greenNode Is Nothing) Then
				Dim interpolatedStringExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(SyntaxKind.InterpolatedStringExpression, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolatedStringExpressionSyntax1, num)
				End If
				interpolatedStringExpressionSyntax = interpolatedStringExpressionSyntax1
			Else
				interpolatedStringExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)
			End If
			Return interpolatedStringExpressionSyntax
		End Function

		Friend Shared Function InterpolatedStringText(ByVal textToken As InterpolatedStringTextTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax
			Dim interpolatedStringTextSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(781, textToken, num)
			If (greenNode Is Nothing) Then
				Dim interpolatedStringTextSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(SyntaxKind.InterpolatedStringText, textToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolatedStringTextSyntax1, num)
				End If
				interpolatedStringTextSyntax = interpolatedStringTextSyntax1
			Else
				interpolatedStringTextSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax)
			End If
			Return interpolatedStringTextSyntax
		End Function

		Friend Shared Function InterpolatedStringTextToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As InterpolatedStringTextTokenSyntax
			Return New InterpolatedStringTextTokenSyntax(SyntaxKind.InterpolatedStringTextToken, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function Interpolation(ByVal openBraceToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax, ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax, ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(SyntaxKind.Interpolation, openBraceToken, expression, alignmentClause, formatClause, closeBraceToken)
		End Function

		Friend Shared Function InterpolationAlignmentClause(ByVal commaToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax
			Dim interpolationAlignmentClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(783, commaToken, value, num)
			If (greenNode Is Nothing) Then
				Dim interpolationAlignmentClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax(SyntaxKind.InterpolationAlignmentClause, commaToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolationAlignmentClauseSyntax1, num)
				End If
				interpolationAlignmentClauseSyntax = interpolationAlignmentClauseSyntax1
			Else
				interpolationAlignmentClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)
			End If
			Return interpolationAlignmentClauseSyntax
		End Function

		Friend Shared Function InterpolationFormatClause(ByVal colonToken As PunctuationSyntax, ByVal formatStringToken As InterpolatedStringTextTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax
			Dim interpolationFormatClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(784, colonToken, formatStringToken, num)
			If (greenNode Is Nothing) Then
				Dim interpolationFormatClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax(SyntaxKind.InterpolationFormatClause, colonToken, formatStringToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(interpolationFormatClauseSyntax1, num)
				End If
				interpolationFormatClauseSyntax = interpolationFormatClauseSyntax1
			Else
				interpolationFormatClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)
			End If
			Return interpolationFormatClauseSyntax
		End Function

		Friend Shared Function InvocationExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax
			Dim invocationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(296, expression, argumentList, num)
			If (greenNode Is Nothing) Then
				Dim invocationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax(SyntaxKind.InvocationExpression, expression, argumentList)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(invocationExpressionSyntax1, num)
				End If
				invocationExpressionSyntax = invocationExpressionSyntax1
			Else
				invocationExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax)
			End If
			Return invocationExpressionSyntax
		End Function

		Friend Shared Function IsExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(325, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.IsExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function IsNotExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(326, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.IsNotExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function JoinCondition(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal equalsKeyword As KeywordSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax
			Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(369, left, equalsKeyword, right, num)
			If (greenNode Is Nothing) Then
				Dim joinConditionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax(SyntaxKind.JoinCondition, left, equalsKeyword, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(joinConditionSyntax1, num)
				End If
				joinConditionSyntax = joinConditionSyntax1
			Else
				joinConditionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)
			End If
			Return joinConditionSyntax
		End Function

		Friend Shared Function KeywordEventContainer(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax
			Dim keywordEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(114, keyword, num)
			If (greenNode Is Nothing) Then
				Dim keywordEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax(SyntaxKind.KeywordEventContainer, keyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(keywordEventContainerSyntax1, num)
				End If
				keywordEventContainerSyntax = keywordEventContainerSyntax1
			Else
				keywordEventContainerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax)
			End If
			Return keywordEventContainerSyntax
		End Function

		Friend Shared Function Label(ByVal kind As SyntaxKind, ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), labelToken, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(kind, labelToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Shared Function LabelStatement(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal colonToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax
			Dim labelStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(148, labelToken, colonToken, num)
			If (greenNode Is Nothing) Then
				Dim labelStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax(SyntaxKind.LabelStatement, labelToken, colonToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelStatementSyntax1, num)
				End If
				labelStatementSyntax = labelStatementSyntax1
			Else
				labelStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)
			End If
			Return labelStatementSyntax
		End Function

		Friend Shared Function LambdaHeader(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause)
		End Function

		Friend Shared Function LeftShiftAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(255, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.LeftShiftAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function LeftShiftExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(315, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LeftShiftExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function LessThanExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(321, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LessThanExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function LessThanOrEqualExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(322, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LessThanOrEqualExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function LetClause(ByVal letKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax
			Dim letClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(360, letKeyword, variables.Node, num)
			If (greenNode Is Nothing) Then
				Dim letClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax(SyntaxKind.LetClause, letKeyword, variables.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(letClauseSyntax1, num)
				End If
				letClauseSyntax = letClauseSyntax1
			Else
				letClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax)
			End If
			Return letClauseSyntax
		End Function

		Friend Shared Function LikeExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(327, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.LikeExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function LineContinuationTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.LineContinuationTrivia, text)
		End Function

		Friend Shared Function LiteralExpression(ByVal kind As SyntaxKind, ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(kind, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function LocalDeclarationStatement(ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal declarators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax
			Dim localDeclarationStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(147, modifiers.Node, declarators.Node, num)
			If (greenNode Is Nothing) Then
				Dim localDeclarationStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(SyntaxKind.LocalDeclarationStatement, modifiers.Node, declarators.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(localDeclarationStatementSyntax1, num)
				End If
				localDeclarationStatementSyntax = localDeclarationStatementSyntax1
			Else
				localDeclarationStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax)
			End If
			Return localDeclarationStatementSyntax
		End Function

		Friend Shared Function LoopStatement(ByVal kind As SyntaxKind, ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), loopKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(kind, loopKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Shared Function LoopUntilStatement(ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(775, loopKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(SyntaxKind.LoopUntilStatement, loopKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Shared Function LoopWhileStatement(ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(774, loopKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(SyntaxKind.LoopWhileStatement, loopKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Shared Function MeExpression(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax
			Dim meExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(282, keyword, num)
			If (greenNode Is Nothing) Then
				Dim meExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax(SyntaxKind.MeExpression, keyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(meExpressionSyntax1, num)
				End If
				meExpressionSyntax = meExpressionSyntax1
			Else
				meExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax)
			End If
			Return meExpressionSyntax
		End Function

		Friend Shared Function MemberAccessExpression(ByVal kind As SyntaxKind, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), expression, operatorToken, name, num)
			If (greenNode Is Nothing) Then
				Dim memberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(kind, expression, operatorToken, name)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(memberAccessExpressionSyntax1, num)
				End If
				memberAccessExpressionSyntax = memberAccessExpressionSyntax1
			Else
				memberAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
			End If
			Return memberAccessExpressionSyntax
		End Function

		Friend Shared Function MethodBlock(ByVal kind As SyntaxKind, ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim methodBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, num)
			If (greenNode Is Nothing) Then
				Dim methodBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(kind, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(methodBlockSyntax1, num)
				End If
				methodBlockSyntax = methodBlockSyntax1
			Else
				methodBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax)
			End If
			Return methodBlockSyntax
		End Function

		Friend Shared Function MethodStatement(ByVal kind As SyntaxKind, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause)
		End Function

		Friend Shared Function MidAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(248, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.MidAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function MidExpression(ByVal mid As IdentifierTokenSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax
			Dim midExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(260, mid, argumentList, num)
			If (greenNode Is Nothing) Then
				Dim midExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax(SyntaxKind.MidExpression, mid, argumentList)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(midExpressionSyntax1, num)
				End If
				midExpressionSyntax = midExpressionSyntax1
			Else
				midExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax)
			End If
			Return midExpressionSyntax
		End Function

		Friend Shared Function MissingCharacterLiteralToken() As CharacterLiteralTokenSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CharacterLiteralToken("", Strings.ChrW(0), Nothing, Nothing)
		End Function

		Friend Shared Function MissingExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_missingExpr
		End Function

		Friend Shared Function MissingIdentifier() As IdentifierTokenSyntax
			Return New SimpleIdentifierSyntax(SyntaxKind.IdentifierToken, Nothing, Nothing, "", Nothing, Nothing)
		End Function

		Friend Shared Function MissingIdentifier(ByVal kind As SyntaxKind) As IdentifierTokenSyntax
			Return New ComplexIdentifierSyntax(SyntaxKind.IdentifierToken, Nothing, Nothing, "", Nothing, Nothing, kind, False, "", TypeCharacter.None)
		End Function

		Friend Shared Function MissingIntegerLiteralToken() As IntegerLiteralTokenSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IntegerLiteralToken("", LiteralBase.[Decimal], TypeCharacter.None, CULng(0), Nothing, Nothing)
		End Function

		Friend Shared Function MissingKeyword(ByVal kind As SyntaxKind) As KeywordSyntax
			Return New KeywordSyntax(kind, "", Nothing, Nothing)
		End Function

		Friend Shared Function MissingPunctuation(ByVal kind As SyntaxKind) As PunctuationSyntax
			Return New PunctuationSyntax(kind, "", Nothing, Nothing)
		End Function

		Friend Shared Function MissingStringLiteral() As StringLiteralTokenSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.StringLiteralToken("", "", Nothing, Nothing)
		End Function

		Friend Shared Function MissingToken(ByVal kind As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Select Case kind
				Case SyntaxKind.AddHandlerKeyword
				Case SyntaxKind.AddressOfKeyword
				Case SyntaxKind.AliasKeyword
				Case SyntaxKind.AndKeyword
				Case SyntaxKind.AndAlsoKeyword
				Case SyntaxKind.AsKeyword
				Case SyntaxKind.BooleanKeyword
				Case SyntaxKind.ByRefKeyword
				Case SyntaxKind.ByteKeyword
				Case SyntaxKind.ByValKeyword
				Case SyntaxKind.CallKeyword
				Case SyntaxKind.CaseKeyword
				Case SyntaxKind.CatchKeyword
				Case SyntaxKind.CBoolKeyword
				Case SyntaxKind.CByteKeyword
				Case SyntaxKind.CCharKeyword
				Case SyntaxKind.CDateKeyword
				Case SyntaxKind.CDecKeyword
				Case SyntaxKind.CDblKeyword
				Case SyntaxKind.CharKeyword
				Case SyntaxKind.CIntKeyword
				Case SyntaxKind.ClassKeyword
				Case SyntaxKind.CLngKeyword
				Case SyntaxKind.CObjKeyword
				Case SyntaxKind.ConstKeyword
				Case SyntaxKind.ContinueKeyword
				Case SyntaxKind.CSByteKeyword
				Case SyntaxKind.CShortKeyword
				Case SyntaxKind.CSngKeyword
				Case SyntaxKind.CStrKeyword
				Case SyntaxKind.CTypeKeyword
				Case SyntaxKind.CUIntKeyword
				Case SyntaxKind.CULngKeyword
				Case SyntaxKind.CUShortKeyword
				Case SyntaxKind.DateKeyword
				Case SyntaxKind.DecimalKeyword
				Case SyntaxKind.DeclareKeyword
				Case SyntaxKind.DefaultKeyword
				Case SyntaxKind.DelegateKeyword
				Case SyntaxKind.DimKeyword
				Case SyntaxKind.DirectCastKeyword
				Case SyntaxKind.DoKeyword
				Case SyntaxKind.DoubleKeyword
				Case SyntaxKind.EachKeyword
				Case SyntaxKind.ElseKeyword
				Case SyntaxKind.ElseIfKeyword
				Case SyntaxKind.EndKeyword
				Case SyntaxKind.EnumKeyword
				Case SyntaxKind.EraseKeyword
				Case SyntaxKind.ErrorKeyword
				Case SyntaxKind.EventKeyword
				Case SyntaxKind.ExitKeyword
				Case SyntaxKind.FalseKeyword
				Case SyntaxKind.FinallyKeyword
				Case SyntaxKind.ForKeyword
				Case SyntaxKind.FriendKeyword
				Case SyntaxKind.FunctionKeyword
				Case SyntaxKind.GetKeyword
				Case SyntaxKind.GetTypeKeyword
				Case SyntaxKind.GetXmlNamespaceKeyword
				Case SyntaxKind.GlobalKeyword
				Case SyntaxKind.GoToKeyword
				Case SyntaxKind.HandlesKeyword
				Case SyntaxKind.IfKeyword
				Case SyntaxKind.ImplementsKeyword
				Case SyntaxKind.ImportsKeyword
				Case SyntaxKind.InKeyword
				Case SyntaxKind.InheritsKeyword
				Case SyntaxKind.IntegerKeyword
				Case SyntaxKind.InterfaceKeyword
				Case SyntaxKind.IsKeyword
				Case SyntaxKind.IsNotKeyword
				Case SyntaxKind.LetKeyword
				Case SyntaxKind.LibKeyword
				Case SyntaxKind.LikeKeyword
				Case SyntaxKind.LongKeyword
				Case SyntaxKind.LoopKeyword
				Case SyntaxKind.MeKeyword
				Case SyntaxKind.ModKeyword
				Case SyntaxKind.ModuleKeyword
				Case SyntaxKind.MustInheritKeyword
				Case SyntaxKind.MustOverrideKeyword
				Case SyntaxKind.MyBaseKeyword
				Case SyntaxKind.MyClassKeyword
				Case SyntaxKind.NamespaceKeyword
				Case SyntaxKind.NarrowingKeyword
				Case SyntaxKind.NextKeyword
				Case SyntaxKind.NewKeyword
				Case SyntaxKind.NotKeyword
				Case SyntaxKind.NothingKeyword
				Case SyntaxKind.NotInheritableKeyword
				Case SyntaxKind.NotOverridableKeyword
				Case SyntaxKind.ObjectKeyword
				Case SyntaxKind.OfKeyword
				Case SyntaxKind.OnKeyword
				Case SyntaxKind.OperatorKeyword
				Case SyntaxKind.OptionKeyword
				Case SyntaxKind.OptionalKeyword
				Case SyntaxKind.OrKeyword
				Case SyntaxKind.OrElseKeyword
				Case SyntaxKind.OverloadsKeyword
				Case SyntaxKind.OverridableKeyword
				Case SyntaxKind.OverridesKeyword
				Case SyntaxKind.ParamArrayKeyword
				Case SyntaxKind.PartialKeyword
				Case SyntaxKind.PrivateKeyword
				Case SyntaxKind.PropertyKeyword
				Case SyntaxKind.ProtectedKeyword
				Case SyntaxKind.PublicKeyword
				Case SyntaxKind.RaiseEventKeyword
				Case SyntaxKind.ReadOnlyKeyword
				Case SyntaxKind.ReDimKeyword
				Case SyntaxKind.REMKeyword
				Case SyntaxKind.RemoveHandlerKeyword
				Case SyntaxKind.ResumeKeyword
				Case SyntaxKind.ReturnKeyword
				Case SyntaxKind.SByteKeyword
				Case SyntaxKind.SelectKeyword
				Case SyntaxKind.SetKeyword
				Case SyntaxKind.ShadowsKeyword
				Case SyntaxKind.SharedKeyword
				Case SyntaxKind.ShortKeyword
				Case SyntaxKind.SingleKeyword
				Case SyntaxKind.StaticKeyword
				Case SyntaxKind.StepKeyword
				Case SyntaxKind.StopKeyword
				Case SyntaxKind.StringKeyword
				Case SyntaxKind.StructureKeyword
				Case SyntaxKind.SubKeyword
				Case SyntaxKind.SyncLockKeyword
				Case SyntaxKind.ThenKeyword
				Case SyntaxKind.ThrowKeyword
				Case SyntaxKind.ToKeyword
				Case SyntaxKind.TrueKeyword
				Case SyntaxKind.TryKeyword
				Case SyntaxKind.TryCastKeyword
				Case SyntaxKind.TypeOfKeyword
				Case SyntaxKind.UIntegerKeyword
				Case SyntaxKind.ULongKeyword
				Case SyntaxKind.UShortKeyword
				Case SyntaxKind.UsingKeyword
				Case SyntaxKind.WhenKeyword
				Case SyntaxKind.WhileKeyword
				Case SyntaxKind.WideningKeyword
				Case SyntaxKind.WithKeyword
				Case SyntaxKind.WithEventsKeyword
				Case SyntaxKind.WriteOnlyKeyword
				Case SyntaxKind.XorKeyword
				Case SyntaxKind.EndIfKeyword
				Case SyntaxKind.GosubKeyword
				Case SyntaxKind.VariantKeyword
				Case SyntaxKind.WendKeyword
				Case SyntaxKind.OutKeyword
				Case SyntaxKind.NameOfKeyword
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(kind)
					Exit Select
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.MidExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.AndKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RemoveHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.RedimClause Or SyntaxKind.EraseStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.XmlAttributeAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.CTypeExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlPrefix Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.XmlCDataSection Or SyntaxKind.XmlEmbeddedExpression Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.PredefinedType Or SyntaxKind.IdentifierName Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByteKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CatchKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanOrEqualExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CULngKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.MidExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.CTypeKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.UsingBlock Or SyntaxKind.NextLabel Or SyntaxKind.ResumeStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.RaiseEventStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.OrExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.MultiLineSubLambdaExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlComment Or SyntaxKind.GenericName Or SyntaxKind.CrefSignaturePart Or SyntaxKind.CTypeKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.ElseKeyword
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.ParenthesizedExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.OrExpression Or SyntaxKind.ExclusiveOrExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.BinaryConditionalExpression Or SyntaxKind.MultiLineSubLambdaExpression Or SyntaxKind.SubLambdaHeader Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.GenericName Or SyntaxKind.QualifiedName Or SyntaxKind.CrefSignaturePart Or SyntaxKind.CrefOperatorReference Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DefaultKeyword Or SyntaxKind.ElseKeyword Or SyntaxKind.ElseIfKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.ForBlock Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsExpression Or SyntaxKind.OrExpression Or SyntaxKind.ExclusiveOrExpression Or SyntaxKind.AndAlsoExpression Or SyntaxKind.UnaryPlusExpression Or SyntaxKind.QueryExpression Or SyntaxKind.CollectionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.FunctionAggregation Or SyntaxKind.LetClause Or SyntaxKind.AggregateClause Or SyntaxKind.SkipWhileClause Or SyntaxKind.TakeWhileClause Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CUShortKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DefaultKeyword Or SyntaxKind.DirectCastKeyword Or SyntaxKind.DoKeyword Or SyntaxKind.GetTypeKeyword Or SyntaxKind.GetXmlNamespaceKeyword Or SyntaxKind.HandlesKeyword Or SyntaxKind.IfKeyword Or SyntaxKind.InKeyword Or SyntaxKind.InheritsKeyword Or SyntaxKind.IsKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForEachBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsNotExpression Or SyntaxKind.OrExpression Or SyntaxKind.AndExpression Or SyntaxKind.AndAlsoExpression Or SyntaxKind.UnaryMinusExpression Or SyntaxKind.QueryExpression Or SyntaxKind.ExpressionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.GroupAggregation Or SyntaxKind.LetClause Or SyntaxKind.DistinctClause Or SyntaxKind.SkipWhileClause Or SyntaxKind.SkipClause Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CTypeKeyword Or SyntaxKind.CULngKeyword Or SyntaxKind.DateKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DelegateKeyword Or SyntaxKind.DirectCastKeyword Or SyntaxKind.DoubleKeyword Or SyntaxKind.GetTypeKeyword Or SyntaxKind.GlobalKeyword Or SyntaxKind.HandlesKeyword Or SyntaxKind.ImplementsKeyword Or SyntaxKind.InKeyword Or SyntaxKind.IntegerKeyword Or SyntaxKind.IsKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.NotKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.StepKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword
				Case 576
				Case SyntaxKind.List Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.PreserveKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword
				Case SyntaxKind.TypeKeyword
				Case SyntaxKind.XmlKeyword
				Case SyntaxKind.AsyncKeyword
				Case SyntaxKind.AwaitKeyword
				Case SyntaxKind.IteratorKeyword
				Case SyntaxKind.YieldKeyword
				Case SyntaxKind.AtToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.NotKeyword
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken
				Case SyntaxKind.EmptyToken
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.EndOfXmlToken
				Case SyntaxKind.BadToken
				Case SyntaxKind.XmlEntityLiteralToken
				Case SyntaxKind.DocumentationCommentLineBreakToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.NextLabel Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.XmlEntityLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.SkippedTokensTrivia
				Case SyntaxKind.DocumentationCommentTrivia
				Case SyntaxKind.XmlCrefAttribute
				Case SyntaxKind.XmlNameAttribute
				Case SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.NewConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.Attribute Or SyntaxKind.PrintStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.ColonToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseElseStatement Or SyntaxKind.SimpleCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.CaretToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NumericLabel Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.SimpleCaseClause Or SyntaxKind.RangeCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.CaretToken Or SyntaxKind.ColonEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.UsingBlock Or SyntaxKind.NextLabel Or SyntaxKind.ResumeStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.PlusToken Or SyntaxKind.EqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.WhitespaceTrivia
				Case SyntaxKind.EndOfLineTrivia
				Case SyntaxKind.ColonTrivia
				Case SyntaxKind.CommentTrivia
				Case SyntaxKind.LineContinuationTrivia
				Case SyntaxKind.DocumentationCommentExteriorTrivia
				Case SyntaxKind.DisabledTextTrivia
				Case SyntaxKind.ConstDirectiveTrivia
				Case SyntaxKind.IfDirectiveTrivia
				Case SyntaxKind.ElseIfDirectiveTrivia
				Case SyntaxKind.ElseDirectiveTrivia
				Case SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.EndRegionDirectiveTrivia
				Case SyntaxKind.ExternalSourceDirectiveTrivia
				Case SyntaxKind.EndExternalSourceDirectiveTrivia
				Case SyntaxKind.ExternalChecksumDirectiveTrivia
				Case SyntaxKind.EnableWarningDirectiveTrivia
				Case SyntaxKind.DisableWarningDirectiveTrivia
				Case SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.ConstDirectiveTrivia
				Case SyntaxKind.BadDirectiveTrivia
				Case SyntaxKind.ImportAliasClause
				Case SyntaxKind.NameColonEquals
				Case SyntaxKind.SimpleDoLoopBlock
				Case SyntaxKind.DoWhileLoopBlock
				Case SyntaxKind.DoUntilLoopBlock
				Case SyntaxKind.DoLoopWhileBlock
				Case SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InheritsStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.VariableDeclarator Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.NextLabel Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NextStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseLessThanOrEqualClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.NextStatement Or SyntaxKind.UsingStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.MultiplyAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ColonTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.NameColonEquals Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.StructureStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.ParameterList Or SyntaxKind.DeclareSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.AsNewClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.Attribute Or SyntaxKind.PrintStatement Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NextLabel Or SyntaxKind.EndStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.ForStepClause Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.CommaToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.ColonToken Or SyntaxKind.EqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.IdentifierToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.CommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.EndStatement Or SyntaxKind.ExitDoStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseBlock Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.FinallyBlock Or SyntaxKind.TryStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.ForBlock Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.IntegerDivideAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.BackslashEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.BadToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.IdentifierToken Or SyntaxKind.IntegerLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.LineContinuationTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoWhileLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.EnumBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.StructureStatement Or SyntaxKind.ClassStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.ParameterList Or SyntaxKind.FunctionStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.IncompleteMember Or SyntaxKind.VariableDeclarator Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NextLabel Or SyntaxKind.EndStatement Or SyntaxKind.ExitForStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.ElseIfBlock Or SyntaxKind.IfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyBlock Or SyntaxKind.CatchStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseElseStatement Or SyntaxKind.SimpleCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseGreaterThanOrEqualClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStepClause Or SyntaxKind.NextStatement Or SyntaxKind.ThrowStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.ExponentiateAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.CommaToken Or SyntaxKind.AmpersandToken Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.CaretToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.CaretEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.XmlNameToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.IdentifierToken Or SyntaxKind.FloatingLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.DocumentationCommentExteriorTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoUntilLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NumericLabel Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.EndStatement Or SyntaxKind.ExitDoStatement Or SyntaxKind.ExitForStatement Or SyntaxKind.ExitSubStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseBlock Or SyntaxKind.IfStatement Or SyntaxKind.ElseIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.FinallyBlock Or SyntaxKind.TryStatement Or SyntaxKind.CatchStatement Or SyntaxKind.CatchFilterClause Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.SimpleCaseClause Or SyntaxKind.RangeCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseLessThanOrEqualClause Or SyntaxKind.CaseGreaterThanOrEqualClause Or SyntaxKind.CaseGreaterThanClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.NextStatement Or SyntaxKind.UsingStatement Or SyntaxKind.ThrowStatement Or SyntaxKind.SimpleAssignmentStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.MultiplyAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.IntegerDivideAssignmentStatement Or SyntaxKind.ExponentiateAssignmentStatement Or SyntaxKind.LeftShiftAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.CaretToken Or SyntaxKind.ColonEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.BackslashEqualsToken Or SyntaxKind.CaretEqualsToken Or SyntaxKind.LessThanLessThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.BadToken Or SyntaxKind.XmlNameToken Or SyntaxKind.XmlTextLiteralToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.IdentifierToken Or SyntaxKind.IntegerLiteralToken Or SyntaxKind.FloatingLiteralToken Or SyntaxKind.DecimalLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ColonTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.LineContinuationTrivia Or SyntaxKind.DocumentationCommentExteriorTrivia Or SyntaxKind.DisabledTextTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.NameColonEquals Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoWhileLoopBlock Or SyntaxKind.DoUntilLoopBlock Or SyntaxKind.DoLoopWhileBlock Or SyntaxKind.DoLoopUntilBlock
				Case 768
				Case SyntaxKind.List Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.SimpleDoStatement
				Case SyntaxKind.DoWhileStatement
				Case SyntaxKind.DoUntilStatement
				Case SyntaxKind.SimpleLoopStatement
				Case SyntaxKind.LoopWhileStatement
				Case SyntaxKind.LoopUntilStatement
				Case SyntaxKind.WhileClause
				Case SyntaxKind.UntilClause
				Case SyntaxKind.NameOfExpression
				Case SyntaxKind.InterpolatedStringExpression
				Case SyntaxKind.InterpolatedStringText
				Case SyntaxKind.Interpolation
				Case SyntaxKind.InterpolationAlignmentClause
				Case SyntaxKind.InterpolationFormatClause
				Case SyntaxKind.DollarSignDoubleQuoteToken
					Throw ExceptionUtilities.UnexpectedValue(kind)
				Case SyntaxKind.ReferenceKeyword
				Case SyntaxKind.AggregateKeyword
				Case SyntaxKind.AllKeyword
				Case SyntaxKind.AnsiKeyword
				Case SyntaxKind.AscendingKeyword
				Case SyntaxKind.AssemblyKeyword
				Case SyntaxKind.AutoKeyword
				Case SyntaxKind.BinaryKeyword
				Case SyntaxKind.ByKeyword
				Case SyntaxKind.CompareKeyword
				Case SyntaxKind.CustomKeyword
				Case SyntaxKind.DescendingKeyword
				Case SyntaxKind.DisableKeyword
				Case SyntaxKind.DistinctKeyword
				Case SyntaxKind.EnableKeyword
				Case SyntaxKind.EqualsKeyword
				Case SyntaxKind.ExplicitKeyword
				Case SyntaxKind.ExternalSourceKeyword
				Case SyntaxKind.ExternalChecksumKeyword
				Case SyntaxKind.FromKeyword
				Case SyntaxKind.GroupKeyword
				Case SyntaxKind.InferKeyword
				Case SyntaxKind.IntoKeyword
				Case SyntaxKind.IsFalseKeyword
				Case SyntaxKind.IsTrueKeyword
				Case SyntaxKind.JoinKeyword
				Case SyntaxKind.KeyKeyword
				Case SyntaxKind.MidKeyword
				Case SyntaxKind.OffKeyword
				Case SyntaxKind.OrderKeyword
				Case SyntaxKind.PreserveKeyword
				Case SyntaxKind.RegionKeyword
				Case SyntaxKind.SkipKeyword
				Case SyntaxKind.StrictKeyword
				Case SyntaxKind.TakeKeyword
				Case SyntaxKind.TextKeyword
				Case SyntaxKind.UnicodeKeyword
				Case SyntaxKind.UntilKeyword
				Case SyntaxKind.WarningKeyword
				Case SyntaxKind.WhereKeyword
					syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(kind)
					Return syntaxToken
				Case SyntaxKind.ExclamationToken
				Case SyntaxKind.CommaToken
				Case SyntaxKind.HashToken
				Case SyntaxKind.AmpersandToken
				Case SyntaxKind.SingleQuoteToken
				Case SyntaxKind.OpenParenToken
				Case SyntaxKind.CloseParenToken
				Case SyntaxKind.OpenBraceToken
				Case SyntaxKind.CloseBraceToken
				Case SyntaxKind.SemicolonToken
				Case SyntaxKind.AsteriskToken
				Case SyntaxKind.PlusToken
				Case SyntaxKind.MinusToken
				Case SyntaxKind.DotToken
				Case SyntaxKind.SlashToken
				Case SyntaxKind.ColonToken
				Case SyntaxKind.LessThanToken
				Case SyntaxKind.LessThanEqualsToken
				Case SyntaxKind.LessThanGreaterThanToken
				Case SyntaxKind.EqualsToken
				Case SyntaxKind.GreaterThanToken
				Case SyntaxKind.GreaterThanEqualsToken
				Case SyntaxKind.BackslashToken
				Case SyntaxKind.CaretToken
				Case SyntaxKind.ColonEqualsToken
				Case SyntaxKind.AmpersandEqualsToken
				Case SyntaxKind.AsteriskEqualsToken
				Case SyntaxKind.PlusEqualsToken
				Case SyntaxKind.MinusEqualsToken
				Case SyntaxKind.SlashEqualsToken
				Case SyntaxKind.BackslashEqualsToken
				Case SyntaxKind.CaretEqualsToken
				Case SyntaxKind.LessThanLessThanToken
				Case SyntaxKind.GreaterThanGreaterThanToken
				Case SyntaxKind.LessThanLessThanEqualsToken
				Case SyntaxKind.GreaterThanGreaterThanEqualsToken
				Case SyntaxKind.QuestionToken
				Case SyntaxKind.DoubleQuoteToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(kind)
					Exit Select
				Case SyntaxKind.StatementTerminatorToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(Nothing, SyntaxKind.StatementTerminatorToken, Nothing, [String].Empty)
					Exit Select
				Case SyntaxKind.EndOfFileToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfFileToken()
					Exit Select
				Case SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.LessThanSlashToken
				Case SyntaxKind.LessThanExclamationMinusMinusToken
				Case SyntaxKind.MinusMinusGreaterThanToken
				Case SyntaxKind.LessThanQuestionToken
				Case SyntaxKind.QuestionGreaterThanToken
				Case SyntaxKind.LessThanPercentEqualsToken
				Case SyntaxKind.PercentGreaterThanToken
				Case SyntaxKind.BeginCDataToken
				Case SyntaxKind.EndCDataToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(kind)
					Exit Select
				Case SyntaxKind.XmlNameToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, Nothing, Nothing)
					Exit Select
				Case SyntaxKind.XmlTextLiteralToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteralToken("", "", Nothing, Nothing)
					Exit Select
				Case SyntaxKind.IdentifierToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
					Exit Select
				Case SyntaxKind.IntegerLiteralToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIntegerLiteralToken()
					Exit Select
				Case SyntaxKind.FloatingLiteralToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.FloatingLiteralToken("", TypeCharacter.None, 0, Nothing, Nothing)
					Exit Select
				Case SyntaxKind.DecimalLiteralToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DecimalLiteralToken("", TypeCharacter.None, [Decimal].Zero, Nothing, Nothing)
					Exit Select
				Case SyntaxKind.DateLiteralToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DateLiteralToken("", DateTime.MinValue, Nothing, Nothing)
					Exit Select
				Case SyntaxKind.StringLiteralToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingStringLiteral()
					Exit Select
				Case SyntaxKind.CharacterLiteralToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingCharacterLiteralToken()
					Exit Select
				Case SyntaxKind.InterpolatedStringTextToken
					syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringTextToken("", "", Nothing, Nothing)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(kind)
			End Select
			syntaxToken = syntaxToken1
			Return syntaxToken
		End Function

		Friend Shared Function ModifiedIdentifier(ByVal identifier As IdentifierTokenSyntax, ByVal nullable As PunctuationSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal arrayRankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(SyntaxKind.ModifiedIdentifier, identifier, nullable, arrayBounds, arrayRankSpecifiers.Node)
		End Function

		Friend Shared Function ModuleBlock(ByVal moduleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(SyntaxKind.ModuleBlock, moduleStatement, [inherits].Node, [implements].Node, members.Node, endModuleStatement)
		End Function

		Friend Shared Function ModuleStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal moduleKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax(SyntaxKind.ModuleStatement, attributeLists.Node, modifiers.Node, moduleKeyword, identifier, typeParameterList)
		End Function

		Friend Shared Function ModuloExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(318, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.ModuloExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function MultiLineFunctionLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(343, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, num)
			If (greenNode Is Nothing) Then
				Dim multiLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineFunctionLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax1, num)
				End If
				multiLineLambdaExpressionSyntax = multiLineLambdaExpressionSyntax1
			Else
				multiLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax)
			End If
			Return multiLineLambdaExpressionSyntax
		End Function

		Friend Shared Function MultiLineIfBlock(ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal elseIfBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax, ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(SyntaxKind.MultiLineIfBlock, ifStatement, statements.Node, elseIfBlocks.Node, elseBlock, endIfStatement)
		End Function

		Friend Shared Function MultiLineLambdaExpression(ByVal kind As SyntaxKind, ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, num)
			If (greenNode Is Nothing) Then
				Dim multiLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(kind, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax1, num)
				End If
				multiLineLambdaExpressionSyntax = multiLineLambdaExpressionSyntax1
			Else
				multiLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax)
			End If
			Return multiLineLambdaExpressionSyntax
		End Function

		Friend Shared Function MultiLineSubLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(344, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, num)
			If (greenNode Is Nothing) Then
				Dim multiLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineSubLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax1, num)
				End If
				multiLineLambdaExpressionSyntax = multiLineLambdaExpressionSyntax1
			Else
				multiLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax)
			End If
			Return multiLineLambdaExpressionSyntax
		End Function

		Friend Shared Function MultiplyAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(251, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.MultiplyAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function MultiplyExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(309, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.MultiplyExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function MyBaseExpression(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax
			Dim myBaseExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(283, keyword, num)
			If (greenNode Is Nothing) Then
				Dim myBaseExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax(SyntaxKind.MyBaseExpression, keyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(myBaseExpressionSyntax1, num)
				End If
				myBaseExpressionSyntax = myBaseExpressionSyntax1
			Else
				myBaseExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax)
			End If
			Return myBaseExpressionSyntax
		End Function

		Friend Shared Function MyClassExpression(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax
			Dim myClassExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(284, keyword, num)
			If (greenNode Is Nothing) Then
				Dim myClassExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax(SyntaxKind.MyClassExpression, keyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(myClassExpressionSyntax1, num)
				End If
				myClassExpressionSyntax = myClassExpressionSyntax1
			Else
				myClassExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax)
			End If
			Return myClassExpressionSyntax
		End Function

		Friend Shared Function NameColonEquals(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal colonEqualsToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax
			Dim nameColonEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(755, name, colonEqualsToken, num)
			If (greenNode Is Nothing) Then
				Dim nameColonEqualsSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax(SyntaxKind.NameColonEquals, name, colonEqualsToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(nameColonEqualsSyntax1, num)
				End If
				nameColonEqualsSyntax = nameColonEqualsSyntax1
			Else
				nameColonEqualsSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax)
			End If
			Return nameColonEqualsSyntax
		End Function

		Friend Shared Function NamedFieldInitializer(ByVal keyKeyword As KeywordSyntax, ByVal dotToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax(SyntaxKind.NamedFieldInitializer, keyKeyword, dotToken, name, equalsToken, expression)
		End Function

		Friend Shared Function NamedTupleElement(ByVal identifier As IdentifierTokenSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax
			Dim namedTupleElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(791, identifier, asClause, num)
			If (greenNode Is Nothing) Then
				Dim namedTupleElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax(SyntaxKind.NamedTupleElement, identifier, asClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(namedTupleElementSyntax1, num)
				End If
				namedTupleElementSyntax = namedTupleElementSyntax1
			Else
				namedTupleElementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax)
			End If
			Return namedTupleElementSyntax
		End Function

		Friend Shared Function NameOfExpression(ByVal nameOfKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax(SyntaxKind.NameOfExpression, nameOfKeyword, openParenToken, argument, closeParenToken)
		End Function

		Friend Shared Function NamespaceBlock(ByVal namespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax, ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endNamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax
			Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(48, namespaceStatement, members.Node, endNamespaceStatement, num)
			If (greenNode Is Nothing) Then
				Dim namespaceBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax(SyntaxKind.NamespaceBlock, namespaceStatement, members.Node, endNamespaceStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(namespaceBlockSyntax1, num)
				End If
				namespaceBlockSyntax = namespaceBlockSyntax1
			Else
				namespaceBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax)
			End If
			Return namespaceBlockSyntax
		End Function

		Friend Shared Function NamespaceStatement(ByVal namespaceKeyword As KeywordSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax
			Dim namespaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(49, namespaceKeyword, name, num)
			If (greenNode Is Nothing) Then
				Dim namespaceStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax(SyntaxKind.NamespaceStatement, namespaceKeyword, name)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(namespaceStatementSyntax1, num)
				End If
				namespaceStatementSyntax = namespaceStatementSyntax1
			Else
				namespaceStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax)
			End If
			Return namespaceStatementSyntax
		End Function

		Friend Shared Function NewConstraint(ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(72, constraintKeyword, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(SyntaxKind.NewConstraint, constraintKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Shared Function NextLabel(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(152, labelToken, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(SyntaxKind.NextLabel, labelToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Shared Function NextStatement(ByVal nextKeyword As KeywordSyntax, ByVal controlVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(242, nextKeyword, controlVariables.Node, num)
			If (greenNode Is Nothing) Then
				Dim nextStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax(SyntaxKind.NextStatement, nextKeyword, controlVariables.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(nextStatementSyntax1, num)
				End If
				nextStatementSyntax = nextStatementSyntax1
			Else
				nextStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			End If
			Return nextStatementSyntax
		End Function

		Friend Shared Function NotEqualsExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(320, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.NotEqualsExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function NotExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(335, operatorToken, operand, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.NotExpression, operatorToken, operand)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Shared Function NothingLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(280, token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.NothingLiteralExpression, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function NullableType(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal questionMarkToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax
			Dim nullableTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(397, elementType, questionMarkToken, num)
			If (greenNode Is Nothing) Then
				Dim nullableTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax(SyntaxKind.NullableType, elementType, questionMarkToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(nullableTypeSyntax1, num)
				End If
				nullableTypeSyntax = nullableTypeSyntax1
			Else
				nullableTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax)
			End If
			Return nullableTypeSyntax
		End Function

		Friend Shared Function NumericLabel(ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(151, labelToken, num)
			If (greenNode Is Nothing) Then
				Dim labelSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(SyntaxKind.NumericLabel, labelToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(labelSyntax1, num)
				End If
				labelSyntax = labelSyntax1
			Else
				labelSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			End If
			Return labelSyntax
		End Function

		Friend Shared Function NumericLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(275, token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.NumericLiteralExpression, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function ObjectCollectionInitializer(ByVal fromKeyword As KeywordSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax
			Dim objectCollectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(126, fromKeyword, initializer, num)
			If (greenNode Is Nothing) Then
				Dim objectCollectionInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax(SyntaxKind.ObjectCollectionInitializer, fromKeyword, initializer)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(objectCollectionInitializerSyntax1, num)
				End If
				objectCollectionInitializerSyntax = objectCollectionInitializerSyntax1
			Else
				objectCollectionInitializerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax)
			End If
			Return objectCollectionInitializerSyntax
		End Function

		Friend Shared Function ObjectCreationExpression(ByVal newKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, newKeyword, attributeLists.Node, type, argumentList, initializer)
		End Function

		Friend Shared Function ObjectMemberInitializer(ByVal withKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal initializers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(SyntaxKind.ObjectMemberInitializer, withKeyword, openBraceToken, initializers.Node, closeBraceToken)
		End Function

		Friend Shared Function OmittedArgument(ByVal empty As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax
			Dim omittedArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(348, empty, num)
			If (greenNode Is Nothing) Then
				Dim omittedArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax(SyntaxKind.OmittedArgument, empty)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(omittedArgumentSyntax1, num)
				End If
				omittedArgumentSyntax = omittedArgumentSyntax1
			Else
				omittedArgumentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax)
			End If
			Return omittedArgumentSyntax
		End Function

		Friend Shared Function OmittedArgument() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.s_omittedArgument
		End Function

		Friend Shared Function OnErrorGoToLabelStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToLabelStatement, onKeyword, errorKeyword, goToKeyword, minus, label)
		End Function

		Friend Shared Function OnErrorGoToMinusOneStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToMinusOneStatement, onKeyword, errorKeyword, goToKeyword, minus, label)
		End Function

		Friend Shared Function OnErrorGoToStatement(ByVal kind As SyntaxKind, ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(kind, onKeyword, errorKeyword, goToKeyword, minus, label)
		End Function

		Friend Shared Function OnErrorGoToZeroStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToZeroStatement, onKeyword, errorKeyword, goToKeyword, minus, label)
		End Function

		Friend Shared Function OnErrorResumeNextStatement(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal resumeKeyword As KeywordSyntax, ByVal nextKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(SyntaxKind.OnErrorResumeNextStatement, onKeyword, errorKeyword, resumeKeyword, nextKeyword)
		End Function

		Friend Shared Function OperatorBlock(ByVal operatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endOperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax
			Dim operatorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(82, operatorStatement, statements.Node, endOperatorStatement, num)
			If (greenNode Is Nothing) Then
				Dim operatorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax(SyntaxKind.OperatorBlock, operatorStatement, statements.Node, endOperatorStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(operatorBlockSyntax1, num)
				End If
				operatorBlockSyntax = operatorBlockSyntax1
			Else
				operatorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax)
			End If
			Return operatorBlockSyntax
		End Function

		Friend Shared Function OperatorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal operatorKeyword As KeywordSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax(SyntaxKind.OperatorStatement, attributeLists.Node, modifiers.Node, operatorKeyword, operatorToken, parameterList, asClause)
		End Function

		Friend Shared Function OptionStatement(ByVal optionKeyword As KeywordSyntax, ByVal nameKeyword As KeywordSyntax, ByVal valueKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax
			Dim optionStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(41, optionKeyword, nameKeyword, valueKeyword, num)
			If (greenNode Is Nothing) Then
				Dim optionStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(SyntaxKind.OptionStatement, optionKeyword, nameKeyword, valueKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(optionStatementSyntax1, num)
				End If
				optionStatementSyntax = optionStatementSyntax1
			Else
				optionStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)
			End If
			Return optionStatementSyntax
		End Function

		Friend Shared Function OrderByClause(ByVal orderKeyword As KeywordSyntax, ByVal byKeyword As KeywordSyntax, ByVal orderings As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax
			Dim orderByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(372, orderKeyword, byKeyword, orderings.Node, num)
			If (greenNode Is Nothing) Then
				Dim orderByClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(SyntaxKind.OrderByClause, orderKeyword, byKeyword, orderings.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderByClauseSyntax1, num)
				End If
				orderByClauseSyntax = orderByClauseSyntax1
			Else
				orderByClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax)
			End If
			Return orderByClauseSyntax
		End Function

		Friend Shared Function Ordering(ByVal kind As SyntaxKind, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal ascendingOrDescendingKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), expression, ascendingOrDescendingKeyword, num)
			If (greenNode Is Nothing) Then
				Dim orderingSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(kind, expression, ascendingOrDescendingKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(orderingSyntax1, num)
				End If
				orderingSyntax = orderingSyntax1
			Else
				orderingSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			End If
			Return orderingSyntax
		End Function

		Friend Shared Function OrElseExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(331, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.OrElseExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function OrExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(328, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.OrExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function Parameter(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(SyntaxKind.Parameter, attributeLists.Node, modifiers.Node, identifier, asClause, [default])
		End Function

		Friend Shared Function ParameterList(ByVal openParenToken As PunctuationSyntax, ByVal parameters As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(92, openParenToken, parameters.Node, closeParenToken, num)
			If (greenNode Is Nothing) Then
				Dim parameterListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax(SyntaxKind.ParameterList, openParenToken, parameters.Node, closeParenToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(parameterListSyntax1, num)
				End If
				parameterListSyntax = parameterListSyntax1
			Else
				parameterListSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			End If
			Return parameterListSyntax
		End Function

		Friend Shared Function ParenthesizedExpression(ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax
			Dim parenthesizedExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(281, openParenToken, expression, closeParenToken, num)
			If (greenNode Is Nothing) Then
				Dim parenthesizedExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax(SyntaxKind.ParenthesizedExpression, openParenToken, expression, closeParenToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(parenthesizedExpressionSyntax1, num)
				End If
				parenthesizedExpressionSyntax = parenthesizedExpressionSyntax1
			Else
				parenthesizedExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax)
			End If
			Return parenthesizedExpressionSyntax
		End Function

		Friend Shared Function PartitionClause(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As KeywordSyntax, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim partitionClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), skipOrTakeKeyword, count, num)
			If (greenNode Is Nothing) Then
				Dim partitionClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(kind, skipOrTakeKeyword, count)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionClauseSyntax1, num)
				End If
				partitionClauseSyntax = partitionClauseSyntax1
			Else
				partitionClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)
			End If
			Return partitionClauseSyntax
		End Function

		Friend Shared Function PartitionWhileClause(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), skipOrTakeKeyword, whileKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim partitionWhileClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(kind, skipOrTakeKeyword, whileKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionWhileClauseSyntax1, num)
				End If
				partitionWhileClauseSyntax = partitionWhileClauseSyntax1
			Else
				partitionWhileClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)
			End If
			Return partitionWhileClauseSyntax
		End Function

		Friend Shared Function PredefinedCastExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax(SyntaxKind.PredefinedCastExpression, keyword, openParenToken, expression, closeParenToken)
		End Function

		Friend Shared Function PredefinedType(ByVal keyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax
			Dim predefinedTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(398, keyword, num)
			If (greenNode Is Nothing) Then
				Dim predefinedTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax(SyntaxKind.PredefinedType, keyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(predefinedTypeSyntax1, num)
				End If
				predefinedTypeSyntax = predefinedTypeSyntax1
			Else
				predefinedTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax)
			End If
			Return predefinedTypeSyntax
		End Function

		Friend Shared Function PrintStatement(ByVal questionToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax
			Dim printStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(140, questionToken, expression, num)
			If (greenNode Is Nothing) Then
				Dim printStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax(SyntaxKind.PrintStatement, questionToken, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(printStatementSyntax1, num)
				End If
				printStatementSyntax = printStatementSyntax1
			Else
				printStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax)
			End If
			Return printStatementSyntax
		End Function

		Friend Shared Function PropertyBlock(ByVal propertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax, ByVal accessors As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endPropertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax
			Dim propertyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(88, propertyStatement, accessors.Node, endPropertyStatement, num)
			If (greenNode Is Nothing) Then
				Dim propertyBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax(SyntaxKind.PropertyBlock, propertyStatement, accessors.Node, endPropertyStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(propertyBlockSyntax1, num)
				End If
				propertyBlockSyntax = propertyBlockSyntax1
			Else
				propertyBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax)
			End If
			Return propertyBlockSyntax
		End Function

		Friend Shared Function PropertyStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal propertyKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax(SyntaxKind.PropertyStatement, attributeLists.Node, modifiers.Node, propertyKeyword, identifier, parameterList, asClause, initializer, implementsClause)
		End Function

		Friend Shared Function QualifiedCrefOperatorReference(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax
			Dim qualifiedCrefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(410, left, dotToken, right, num)
			If (greenNode Is Nothing) Then
				Dim qualifiedCrefOperatorReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(SyntaxKind.QualifiedCrefOperatorReference, left, dotToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(qualifiedCrefOperatorReferenceSyntax1, num)
				End If
				qualifiedCrefOperatorReferenceSyntax = qualifiedCrefOperatorReferenceSyntax1
			Else
				qualifiedCrefOperatorReferenceSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax)
			End If
			Return qualifiedCrefOperatorReferenceSyntax
		End Function

		Friend Shared Function QualifiedName(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax
			Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(401, left, dotToken, right, num)
			If (greenNode Is Nothing) Then
				Dim qualifiedNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax(SyntaxKind.QualifiedName, left, dotToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(qualifiedNameSyntax1, num)
				End If
				qualifiedNameSyntax = qualifiedNameSyntax1
			Else
				qualifiedNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)
			End If
			Return qualifiedNameSyntax
		End Function

		Friend Shared Function QueryExpression(ByVal clauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax
			Dim queryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(352, clauses.Node, num)
			If (greenNode Is Nothing) Then
				Dim queryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax(SyntaxKind.QueryExpression, clauses.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(queryExpressionSyntax1, num)
				End If
				queryExpressionSyntax = queryExpressionSyntax1
			Else
				queryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax)
			End If
			Return queryExpressionSyntax
		End Function

		Friend Shared Function RaiseEventAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(87, accessorStatement, statements.Node, endAccessorStatement, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.RaiseEventAccessorBlock, accessorStatement, statements.Node, endAccessorStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Shared Function RaiseEventAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.RaiseEventAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList)
		End Function

		Friend Shared Function RaiseEventStatement(ByVal raiseEventKeyword As KeywordSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax
			Dim raiseEventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(264, raiseEventKeyword, name, argumentList, num)
			If (greenNode Is Nothing) Then
				Dim raiseEventStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax(SyntaxKind.RaiseEventStatement, raiseEventKeyword, name, argumentList)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(raiseEventStatementSyntax1, num)
				End If
				raiseEventStatementSyntax = raiseEventStatementSyntax1
			Else
				raiseEventStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax)
			End If
			Return raiseEventStatementSyntax
		End Function

		Friend Shared Function RangeArgument(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax
			Dim rangeArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(351, lowerBound, toKeyword, upperBound, num)
			If (greenNode Is Nothing) Then
				Dim rangeArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax(SyntaxKind.RangeArgument, lowerBound, toKeyword, upperBound)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(rangeArgumentSyntax1, num)
				End If
				rangeArgumentSyntax = rangeArgumentSyntax1
			Else
				rangeArgumentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax)
			End If
			Return rangeArgumentSyntax
		End Function

		Friend Shared Function RangeCaseClause(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax
			Dim rangeCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(215, lowerBound, toKeyword, upperBound, num)
			If (greenNode Is Nothing) Then
				Dim rangeCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax(SyntaxKind.RangeCaseClause, lowerBound, toKeyword, upperBound)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(rangeCaseClauseSyntax1, num)
				End If
				rangeCaseClauseSyntax = rangeCaseClauseSyntax1
			Else
				rangeCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax)
			End If
			Return rangeCaseClauseSyntax
		End Function

		Friend Shared Function RedimClause(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax
			Dim redimClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(270, expression, arrayBounds, num)
			If (greenNode Is Nothing) Then
				Dim redimClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax(SyntaxKind.RedimClause, expression, arrayBounds)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(redimClauseSyntax1, num)
				End If
				redimClauseSyntax = redimClauseSyntax1
			Else
				redimClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)
			End If
			Return redimClauseSyntax
		End Function

		Friend Shared Function ReDimPreserveStatement(ByVal reDimKeyword As KeywordSyntax, ByVal preserveKeyword As KeywordSyntax, ByVal clauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim reDimStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(267, reDimKeyword, preserveKeyword, clauses.Node, num)
			If (greenNode Is Nothing) Then
				Dim reDimStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(SyntaxKind.ReDimPreserveStatement, reDimKeyword, preserveKeyword, clauses.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(reDimStatementSyntax1, num)
				End If
				reDimStatementSyntax = reDimStatementSyntax1
			Else
				reDimStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)
			End If
			Return reDimStatementSyntax
		End Function

		Friend Shared Function ReDimStatement(ByVal reDimKeyword As KeywordSyntax, ByVal preserveKeyword As KeywordSyntax, ByVal clauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim reDimStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(266, reDimKeyword, preserveKeyword, clauses.Node, num)
			If (greenNode Is Nothing) Then
				Dim reDimStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(SyntaxKind.ReDimStatement, reDimKeyword, preserveKeyword, clauses.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(reDimStatementSyntax1, num)
				End If
				reDimStatementSyntax = reDimStatementSyntax1
			Else
				reDimStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)
			End If
			Return reDimStatementSyntax
		End Function

		Friend Shared Function ReferenceDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal referenceKeyword As KeywordSyntax, ByVal file As StringLiteralTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(SyntaxKind.ReferenceDirectiveTrivia, hashToken, referenceKeyword, file)
		End Function

		Friend Shared Function RegionDirectiveTrivia(ByVal hashToken As PunctuationSyntax, ByVal regionKeyword As KeywordSyntax, ByVal name As StringLiteralTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(SyntaxKind.RegionDirectiveTrivia, hashToken, regionKeyword, name)
		End Function

		Friend Shared Function RelationalCaseClause(ByVal kind As SyntaxKind, ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), isKeyword, operatorToken, value, num)
			If (greenNode Is Nothing) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(kind, isKeyword, operatorToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(relationalCaseClauseSyntax1, num)
				End If
				relationalCaseClauseSyntax = relationalCaseClauseSyntax1
			Else
				relationalCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)
			End If
			Return relationalCaseClauseSyntax
		End Function

		Friend Shared Function RemoveHandlerAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(86, accessorStatement, statements.Node, endAccessorStatement, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.RemoveHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Shared Function RemoveHandlerAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.RemoveHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList)
		End Function

		Friend Shared Function RemoveHandlerStatement(ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(SyntaxKind.RemoveHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression)
		End Function

		Friend Shared Function ResumeLabelStatement(ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(201, resumeKeyword, label, num)
			If (greenNode Is Nothing) Then
				Dim resumeStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(SyntaxKind.ResumeLabelStatement, resumeKeyword, label)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(resumeStatementSyntax1, num)
				End If
				resumeStatementSyntax = resumeStatementSyntax1
			Else
				resumeStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)
			End If
			Return resumeStatementSyntax
		End Function

		Friend Shared Function ResumeNextStatement(ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(202, resumeKeyword, label, num)
			If (greenNode Is Nothing) Then
				Dim resumeStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(SyntaxKind.ResumeNextStatement, resumeKeyword, label)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(resumeStatementSyntax1, num)
				End If
				resumeStatementSyntax = resumeStatementSyntax1
			Else
				resumeStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)
			End If
			Return resumeStatementSyntax
		End Function

		Friend Shared Function ResumeStatement(ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(200, resumeKeyword, label, num)
			If (greenNode Is Nothing) Then
				Dim resumeStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(SyntaxKind.ResumeStatement, resumeKeyword, label)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(resumeStatementSyntax1, num)
				End If
				resumeStatementSyntax = resumeStatementSyntax1
			Else
				resumeStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)
			End If
			Return resumeStatementSyntax
		End Function

		Friend Shared Function ReturnStatement(ByVal returnKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax
			Dim returnStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(169, returnKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim returnStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax(SyntaxKind.ReturnStatement, returnKeyword, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(returnStatementSyntax1, num)
				End If
				returnStatementSyntax = returnStatementSyntax1
			Else
				returnStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax)
			End If
			Return returnStatementSyntax
		End Function

		Friend Shared Function RightShiftAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(258, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.RightShiftAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function RightShiftExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(316, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.RightShiftExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function SelectBlock(ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax, ByVal caseBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax
			Dim selectBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(203, selectStatement, caseBlocks.Node, endSelectStatement, num)
			If (greenNode Is Nothing) Then
				Dim selectBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(SyntaxKind.SelectBlock, selectStatement, caseBlocks.Node, endSelectStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(selectBlockSyntax1, num)
				End If
				selectBlockSyntax = selectBlockSyntax1
			Else
				selectBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax)
			End If
			Return selectBlockSyntax
		End Function

		Friend Shared Function SelectClause(ByVal selectKeyword As KeywordSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax
			Dim selectClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(377, selectKeyword, variables.Node, num)
			If (greenNode Is Nothing) Then
				Dim selectClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax(SyntaxKind.SelectClause, selectKeyword, variables.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(selectClauseSyntax1, num)
				End If
				selectClauseSyntax = selectClauseSyntax1
			Else
				selectClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax)
			End If
			Return selectClauseSyntax
		End Function

		Friend Shared Function SelectStatement(ByVal selectKeyword As KeywordSyntax, ByVal caseKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax
			Dim selectStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(204, selectKeyword, caseKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim selectStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax(SyntaxKind.SelectStatement, selectKeyword, caseKeyword, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(selectStatementSyntax1, num)
				End If
				selectStatementSyntax = selectStatementSyntax1
			Else
				selectStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax)
			End If
			Return selectStatementSyntax
		End Function

		Friend Shared Function SetAccessorBlock(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(84, accessorStatement, statements.Node, endAccessorStatement, num)
			If (greenNode Is Nothing) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(SyntaxKind.SetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(accessorBlockSyntax1, num)
				End If
				accessorBlockSyntax = accessorBlockSyntax1
			Else
				accessorBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			End If
			Return accessorBlockSyntax
		End Function

		Friend Shared Function SetAccessorStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal accessorKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(SyntaxKind.SetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList)
		End Function

		Friend Shared Function SimpleArgument(ByVal nameColonEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax
			Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(349, nameColonEquals, expression, num)
			If (greenNode Is Nothing) Then
				Dim simpleArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax(SyntaxKind.SimpleArgument, nameColonEquals, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleArgumentSyntax1, num)
				End If
				simpleArgumentSyntax = simpleArgumentSyntax1
			Else
				simpleArgumentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)
			End If
			Return simpleArgumentSyntax
		End Function

		Friend Shared Function SimpleAsClause(ByVal asKeyword As KeywordSyntax, ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(123, asKeyword, attributeLists.Node, type, num)
			If (greenNode Is Nothing) Then
				Dim simpleAsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(SyntaxKind.SimpleAsClause, asKeyword, attributeLists.Node, type)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleAsClauseSyntax1, num)
				End If
				simpleAsClauseSyntax = simpleAsClauseSyntax1
			Else
				simpleAsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			End If
			Return simpleAsClauseSyntax
		End Function

		Friend Shared Function SimpleAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(247, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.SimpleAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function SimpleCaseClause(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax
			Dim simpleCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(214, value, num)
			If (greenNode Is Nothing) Then
				Dim simpleCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax(SyntaxKind.SimpleCaseClause, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleCaseClauseSyntax1, num)
				End If
				simpleCaseClauseSyntax = simpleCaseClauseSyntax1
			Else
				simpleCaseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax)
			End If
			Return simpleCaseClauseSyntax
		End Function

		Friend Shared Function SimpleDoLoopBlock(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(756, doStatement, statements.Node, loopStatement, num)
			If (greenNode Is Nothing) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(SyntaxKind.SimpleDoLoopBlock, doStatement, statements.Node, loopStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doLoopBlockSyntax1, num)
				End If
				doLoopBlockSyntax = doLoopBlockSyntax1
			Else
				doLoopBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax)
			End If
			Return doLoopBlockSyntax
		End Function

		Friend Shared Function SimpleDoStatement(ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(770, doKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(SyntaxKind.SimpleDoStatement, doKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(doStatementSyntax1, num)
				End If
				doStatementSyntax = doStatementSyntax1
			Else
				doStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			End If
			Return doStatementSyntax
		End Function

		Friend Shared Function SimpleImportsClause(ByVal [alias] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax
			Dim simpleImportsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(44, [alias], name, num)
			If (greenNode Is Nothing) Then
				Dim simpleImportsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax(SyntaxKind.SimpleImportsClause, [alias], name)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(simpleImportsClauseSyntax1, num)
				End If
				simpleImportsClauseSyntax = simpleImportsClauseSyntax1
			Else
				simpleImportsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax)
			End If
			Return simpleImportsClauseSyntax
		End Function

		Friend Shared Function SimpleJoinClause(ByVal joinKeyword As KeywordSyntax, ByVal joinedVariables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal additionalJoins As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal onKeyword As KeywordSyntax, ByVal joinConditions As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(SyntaxKind.SimpleJoinClause, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node)
		End Function

		Friend Shared Function SimpleLoopStatement(ByVal loopKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(773, loopKeyword, whileOrUntilClause, num)
			If (greenNode Is Nothing) Then
				Dim loopStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(SyntaxKind.SimpleLoopStatement, loopKeyword, whileOrUntilClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(loopStatementSyntax1, num)
				End If
				loopStatementSyntax = loopStatementSyntax1
			Else
				loopStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			End If
			Return loopStatementSyntax
		End Function

		Friend Shared Function SimpleMemberAccessExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(291, expression, operatorToken, name, num)
			If (greenNode Is Nothing) Then
				Dim memberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(SyntaxKind.SimpleMemberAccessExpression, expression, operatorToken, name)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(memberAccessExpressionSyntax1, num)
				End If
				memberAccessExpressionSyntax = memberAccessExpressionSyntax1
			Else
				memberAccessExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
			End If
			Return memberAccessExpressionSyntax
		End Function

		Friend Shared Function SingleLineElseClause(ByVal elseKeyword As KeywordSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax
			Dim singleLineElseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(172, elseKeyword, statements.Node, num)
			If (greenNode Is Nothing) Then
				Dim singleLineElseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax(SyntaxKind.SingleLineElseClause, elseKeyword, statements.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineElseClauseSyntax1, num)
				End If
				singleLineElseClauseSyntax = singleLineElseClauseSyntax1
			Else
				singleLineElseClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax)
			End If
			Return singleLineElseClauseSyntax
		End Function

		Friend Shared Function SingleLineFunctionLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(339, subOrFunctionHeader, body, num)
			If (greenNode Is Nothing) Then
				Dim singleLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineFunctionLambdaExpression, subOrFunctionHeader, body)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax1, num)
				End If
				singleLineLambdaExpressionSyntax = singleLineLambdaExpressionSyntax1
			Else
				singleLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)
			End If
			Return singleLineLambdaExpressionSyntax
		End Function

		Friend Shared Function SingleLineIfStatement(ByVal ifKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal elseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax(SyntaxKind.SingleLineIfStatement, ifKeyword, condition, thenKeyword, statements.Node, elseClause)
		End Function

		Friend Shared Function SingleLineLambdaExpression(ByVal kind As SyntaxKind, ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), subOrFunctionHeader, body, num)
			If (greenNode Is Nothing) Then
				Dim singleLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(kind, subOrFunctionHeader, body)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax1, num)
				End If
				singleLineLambdaExpressionSyntax = singleLineLambdaExpressionSyntax1
			Else
				singleLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)
			End If
			Return singleLineLambdaExpressionSyntax
		End Function

		Friend Shared Function SingleLineSubLambdaExpression(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, ByVal body As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(342, subOrFunctionHeader, body, num)
			If (greenNode Is Nothing) Then
				Dim singleLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineSubLambdaExpression, subOrFunctionHeader, body)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax1, num)
				End If
				singleLineLambdaExpressionSyntax = singleLineLambdaExpressionSyntax1
			Else
				singleLineLambdaExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)
			End If
			Return singleLineLambdaExpressionSyntax
		End Function

		Friend Shared Function SkipClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim partitionClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(366, skipOrTakeKeyword, count, num)
			If (greenNode Is Nothing) Then
				Dim partitionClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(SyntaxKind.SkipClause, skipOrTakeKeyword, count)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionClauseSyntax1, num)
				End If
				partitionClauseSyntax = partitionClauseSyntax1
			Else
				partitionClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)
			End If
			Return partitionClauseSyntax
		End Function

		Friend Shared Function SkippedTokensTrivia(ByVal tokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax(SyntaxKind.SkippedTokensTrivia, tokens.Node)
		End Function

		Friend Shared Function SkipWhileClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(364, skipOrTakeKeyword, whileKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim partitionWhileClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(SyntaxKind.SkipWhileClause, skipOrTakeKeyword, whileKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionWhileClauseSyntax1, num)
				End If
				partitionWhileClauseSyntax = partitionWhileClauseSyntax1
			Else
				partitionWhileClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)
			End If
			Return partitionWhileClauseSyntax
		End Function

		Friend Shared Function SpecialConstraint(ByVal kind As SyntaxKind, ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), constraintKeyword, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(kind, constraintKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Shared Function StopOrEndStatement(ByVal kind As SyntaxKind, ByVal stopOrEndKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim stopOrEndStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), stopOrEndKeyword, num)
			If (greenNode Is Nothing) Then
				Dim stopOrEndStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(kind, stopOrEndKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(stopOrEndStatementSyntax1, num)
				End If
				stopOrEndStatementSyntax = stopOrEndStatementSyntax1
			Else
				stopOrEndStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)
			End If
			Return stopOrEndStatementSyntax
		End Function

		Friend Shared Function StopStatement(ByVal stopOrEndKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim stopOrEndStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(153, stopOrEndKeyword, num)
			If (greenNode Is Nothing) Then
				Dim stopOrEndStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(SyntaxKind.StopStatement, stopOrEndKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(stopOrEndStatementSyntax1, num)
				End If
				stopOrEndStatementSyntax = stopOrEndStatementSyntax1
			Else
				stopOrEndStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)
			End If
			Return stopOrEndStatementSyntax
		End Function

		Friend Shared Function StringLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(279, token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.StringLiteralExpression, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function StringLiteralToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As StringLiteralTokenSyntax
			Return New StringLiteralTokenSyntax(SyntaxKind.StringLiteralToken, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function StructureBlock(ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(SyntaxKind.StructureBlock, structureStatement, [inherits].Node, [implements].Node, members.Node, endStructureStatement)
		End Function

		Friend Shared Function StructureConstraint(ByVal constraintKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(74, constraintKeyword, num)
			If (greenNode Is Nothing) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(SyntaxKind.StructureConstraint, constraintKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(specialConstraintSyntax1, num)
				End If
				specialConstraintSyntax = specialConstraintSyntax1
			Else
				specialConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)
			End If
			Return specialConstraintSyntax
		End Function

		Friend Shared Function StructureStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal structureKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax(SyntaxKind.StructureStatement, attributeLists.Node, modifiers.Node, structureKeyword, identifier, typeParameterList)
		End Function

		Friend Shared Function SubBlock(ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim methodBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(79, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, num)
			If (greenNode Is Nothing) Then
				Dim methodBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(SyntaxKind.SubBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(methodBlockSyntax1, num)
				End If
				methodBlockSyntax = methodBlockSyntax1
			Else
				methodBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax)
			End If
			Return methodBlockSyntax
		End Function

		Friend Shared Function SubLambdaHeader(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(SyntaxKind.SubLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause)
		End Function

		Friend Shared Function SubNewStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subKeyword As KeywordSyntax, ByVal newKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(SyntaxKind.SubNewStatement, attributeLists.Node, modifiers.Node, subKeyword, newKeyword, parameterList)
		End Function

		Friend Shared Function SubStatement(ByVal attributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(SyntaxKind.SubStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause)
		End Function

		Friend Shared Function SubtractAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(250, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim assignmentStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(SyntaxKind.SubtractAssignmentStatement, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(assignmentStatementSyntax1, num)
				End If
				assignmentStatementSyntax = assignmentStatementSyntax1
			Else
				assignmentStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)
			End If
			Return assignmentStatementSyntax
		End Function

		Friend Shared Function SubtractExpression(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim binaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(308, left, operatorToken, right, num)
			If (greenNode Is Nothing) Then
				Dim binaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax(SyntaxKind.SubtractExpression, left, operatorToken, right)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(binaryExpressionSyntax1, num)
				End If
				binaryExpressionSyntax = binaryExpressionSyntax1
			Else
				binaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax)
			End If
			Return binaryExpressionSyntax
		End Function

		Friend Shared Function SyncLockBlock(ByVal syncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax
			Dim syncLockBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(145, syncLockStatement, statements.Node, endSyncLockStatement, num)
			If (greenNode Is Nothing) Then
				Dim syncLockBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax(SyntaxKind.SyncLockBlock, syncLockStatement, statements.Node, endSyncLockStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(syncLockBlockSyntax1, num)
				End If
				syncLockBlockSyntax = syncLockBlockSyntax1
			Else
				syncLockBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax)
			End If
			Return syncLockBlockSyntax
		End Function

		Friend Shared Function SyncLockStatement(ByVal syncLockKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax
			Dim syncLockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(226, syncLockKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim syncLockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax(SyntaxKind.SyncLockStatement, syncLockKeyword, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(syncLockStatementSyntax1, num)
				End If
				syncLockStatementSyntax = syncLockStatementSyntax1
			Else
				syncLockStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax)
			End If
			Return syncLockStatementSyntax
		End Function

		Friend Shared Function SyntaxTrivia(ByVal kind As SyntaxKind, ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(kind, text)
		End Function

		Friend Shared Function TakeClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim partitionClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(367, skipOrTakeKeyword, count, num)
			If (greenNode Is Nothing) Then
				Dim partitionClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(SyntaxKind.TakeClause, skipOrTakeKeyword, count)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionClauseSyntax1, num)
				End If
				partitionClauseSyntax = partitionClauseSyntax1
			Else
				partitionClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)
			End If
			Return partitionClauseSyntax
		End Function

		Friend Shared Function TakeWhileClause(ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(365, skipOrTakeKeyword, whileKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim partitionWhileClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(SyntaxKind.TakeWhileClause, skipOrTakeKeyword, whileKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(partitionWhileClauseSyntax1, num)
				End If
				partitionWhileClauseSyntax = partitionWhileClauseSyntax1
			Else
				partitionWhileClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)
			End If
			Return partitionWhileClauseSyntax
		End Function

		Friend Shared Function TernaryConditionalExpression(ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(SyntaxKind.TernaryConditionalExpression, ifKeyword, openParenToken, condition, firstCommaToken, whenTrue, secondCommaToken, whenFalse, closeParenToken)
		End Function

		Friend Shared Function ThrowStatement(ByVal throwKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax
			Dim throwStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(246, throwKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim throwStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(SyntaxKind.ThrowStatement, throwKeyword, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(throwStatementSyntax1, num)
				End If
				throwStatementSyntax = throwStatementSyntax1
			Else
				throwStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax)
			End If
			Return throwStatementSyntax
		End Function

		Friend Shared Function Token(ByVal leading As GreenNode, ByVal kind As SyntaxKind, ByVal trailing As GreenNode, Optional ByVal text As String = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.Create(kind, leading, trailing, If(text Is Nothing, SyntaxFacts.GetText(kind), text))
		End Function

		Friend Shared Function TrueLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(273, token, num)
			If (greenNode Is Nothing) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, token)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(literalExpressionSyntax1, num)
				End If
				literalExpressionSyntax = literalExpressionSyntax1
			Else
				literalExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			End If
			Return literalExpressionSyntax
		End Function

		Friend Shared Function TryBlock(ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal catchBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax, ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(SyntaxKind.TryBlock, tryStatement, statements.Node, catchBlocks.Node, finallyBlock, endTryStatement)
		End Function

		Friend Shared Function TryCastExpression(ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax(SyntaxKind.TryCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken)
		End Function

		Friend Shared Function TryStatement(ByVal tryKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax
			Dim tryStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(189, tryKeyword, num)
			If (greenNode Is Nothing) Then
				Dim tryStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax(SyntaxKind.TryStatement, tryKeyword)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(tryStatementSyntax1, num)
				End If
				tryStatementSyntax = tryStatementSyntax1
			Else
				tryStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)
			End If
			Return tryStatementSyntax
		End Function

		Friend Shared Function TupleExpression(ByVal openParenToken As PunctuationSyntax, ByVal arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax
			Dim tupleExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(788, openParenToken, arguments.Node, closeParenToken, num)
			If (greenNode Is Nothing) Then
				Dim tupleExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(SyntaxKind.TupleExpression, openParenToken, arguments.Node, closeParenToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(tupleExpressionSyntax1, num)
				End If
				tupleExpressionSyntax = tupleExpressionSyntax1
			Else
				tupleExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax)
			End If
			Return tupleExpressionSyntax
		End Function

		Friend Shared Function TupleType(ByVal openParenToken As PunctuationSyntax, ByVal elements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax
			Dim tupleTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(789, openParenToken, elements.Node, closeParenToken, num)
			If (greenNode Is Nothing) Then
				Dim tupleTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax(SyntaxKind.TupleType, openParenToken, elements.Node, closeParenToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(tupleTypeSyntax1, num)
				End If
				tupleTypeSyntax = tupleTypeSyntax1
			Else
				tupleTypeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax)
			End If
			Return tupleTypeSyntax
		End Function

		Friend Shared Function TypeArgumentList(ByVal openParenToken As PunctuationSyntax, ByVal ofKeyword As KeywordSyntax, ByVal arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(SyntaxKind.TypeArgumentList, openParenToken, ofKeyword, arguments.Node, closeParenToken)
		End Function

		Public Shared Function TypeBlock(ByVal blockKind As SyntaxKind, ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax, ByVal [inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax), ByVal [implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax), ByVal members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax
			Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax
			Select Case blockKind
				Case SyntaxKind.ModuleBlock
					typeBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ModuleBlock(DirectCast(begin, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax), [inherits], [implements], members, [end])
					Exit Select
				Case SyntaxKind.StructureBlock
					typeBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.StructureBlock(DirectCast(begin, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax), [inherits], [implements], members, [end])
					Exit Select
				Case SyntaxKind.InterfaceBlock
					typeBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterfaceBlock(DirectCast(begin, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax), [inherits], [implements], members, [end])
					Exit Select
				Case SyntaxKind.ClassBlock
					typeBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ClassBlock(DirectCast(begin, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax), [inherits], [implements], members, [end])
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(blockKind)
			End Select
			Return typeBlockSyntax
		End Function

		Friend Shared Function TypeConstraint(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax
			Dim typeConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(75, type, num)
			If (greenNode Is Nothing) Then
				Dim typeConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax(SyntaxKind.TypeConstraint, type)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typeConstraintSyntax1, num)
				End If
				typeConstraintSyntax = typeConstraintSyntax1
			Else
				typeConstraintSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax)
			End If
			Return typeConstraintSyntax
		End Function

		Friend Shared Function TypedTupleElement(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax
			Dim typedTupleElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(790, type, num)
			If (greenNode Is Nothing) Then
				Dim typedTupleElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax(SyntaxKind.TypedTupleElement, type)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typedTupleElementSyntax1, num)
				End If
				typedTupleElementSyntax = typedTupleElementSyntax1
			Else
				typedTupleElementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax)
			End If
			Return typedTupleElementSyntax
		End Function

		Friend Shared Function TypeOfExpression(ByVal kind As SyntaxKind, ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(kind, typeOfKeyword, expression, operatorToken, type)
		End Function

		Friend Shared Function TypeOfIsExpression(ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(SyntaxKind.TypeOfIsExpression, typeOfKeyword, expression, operatorToken, type)
		End Function

		Friend Shared Function TypeOfIsNotExpression(ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(SyntaxKind.TypeOfIsNotExpression, typeOfKeyword, expression, operatorToken, type)
		End Function

		Friend Shared Function TypeParameter(ByVal varianceKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax
			Dim typeParameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(67, varianceKeyword, identifier, typeParameterConstraintClause, num)
			If (greenNode Is Nothing) Then
				Dim typeParameterSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(SyntaxKind.TypeParameter, varianceKeyword, identifier, typeParameterConstraintClause)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typeParameterSyntax1, num)
				End If
				typeParameterSyntax = typeParameterSyntax1
			Else
				typeParameterSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)
			End If
			Return typeParameterSyntax
		End Function

		Friend Shared Function TypeParameterList(ByVal openParenToken As PunctuationSyntax, ByVal ofKeyword As KeywordSyntax, ByVal parameters As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeParenToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax(SyntaxKind.TypeParameterList, openParenToken, ofKeyword, parameters.Node, closeParenToken)
		End Function

		Friend Shared Function TypeParameterMultipleConstraintClause(ByVal asKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal constraints As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode), ByVal closeBraceToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(SyntaxKind.TypeParameterMultipleConstraintClause, asKeyword, openBraceToken, constraints.Node, closeBraceToken)
		End Function

		Friend Shared Function TypeParameterSingleConstraintClause(ByVal asKeyword As KeywordSyntax, ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax
			Dim typeParameterSingleConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(70, asKeyword, constraint, num)
			If (greenNode Is Nothing) Then
				Dim typeParameterSingleConstraintClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(SyntaxKind.TypeParameterSingleConstraintClause, asKeyword, constraint)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(typeParameterSingleConstraintClauseSyntax1, num)
				End If
				typeParameterSingleConstraintClauseSyntax = typeParameterSingleConstraintClauseSyntax1
			Else
				typeParameterSingleConstraintClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax)
			End If
			Return typeParameterSingleConstraintClauseSyntax
		End Function

		Public Shared Function TypeStatement(ByVal statementKind As SyntaxKind, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal keyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax
			Dim typeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax
			Select Case statementKind
				Case SyntaxKind.ModuleStatement
					typeStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ModuleStatement(attributes, modifiers, keyword, identifier, typeParameterList)
					Exit Select
				Case SyntaxKind.StructureStatement
					typeStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.StructureStatement(attributes, modifiers, keyword, identifier, typeParameterList)
					Exit Select
				Case SyntaxKind.InterfaceStatement
					typeStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterfaceStatement(attributes, modifiers, keyword, identifier, typeParameterList)
					Exit Select
				Case SyntaxKind.ClassStatement
					typeStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ClassStatement(attributes, modifiers, keyword, identifier, typeParameterList)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(statementKind)
			End Select
			Return typeStatementSyntax
		End Function

		Friend Shared Function UnaryExpression(ByVal kind As SyntaxKind, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), operatorToken, operand, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(kind, operatorToken, operand)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Shared Function UnaryMinusExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(334, operatorToken, operand, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.UnaryMinusExpression, operatorToken, operand)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Shared Function UnaryPlusExpression(ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(333, operatorToken, operand, num)
			If (greenNode Is Nothing) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(SyntaxKind.UnaryPlusExpression, operatorToken, operand)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(unaryExpressionSyntax1, num)
				End If
				unaryExpressionSyntax = unaryExpressionSyntax1
			Else
				unaryExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)
			End If
			Return unaryExpressionSyntax
		End Function

		Friend Shared Function UntilClause(ByVal whileOrUntilKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(777, whileOrUntilKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim whileOrUntilClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(SyntaxKind.UntilClause, whileOrUntilKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax1, num)
				End If
				whileOrUntilClauseSyntax = whileOrUntilClauseSyntax1
			Else
				whileOrUntilClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			End If
			Return whileOrUntilClauseSyntax
		End Function

		Friend Shared Function UsingBlock(ByVal usingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endUsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax
			Dim usingBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(144, usingStatement, statements.Node, endUsingStatement, num)
			If (greenNode Is Nothing) Then
				Dim usingBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax(SyntaxKind.UsingBlock, usingStatement, statements.Node, endUsingStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(usingBlockSyntax1, num)
				End If
				usingBlockSyntax = usingBlockSyntax1
			Else
				usingBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax)
			End If
			Return usingBlockSyntax
		End Function

		Friend Shared Function UsingStatement(ByVal usingKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax
			Dim usingStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(243, usingKeyword, expression, variables.Node, num)
			If (greenNode Is Nothing) Then
				Dim usingStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(SyntaxKind.UsingStatement, usingKeyword, expression, variables.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(usingStatementSyntax1, num)
				End If
				usingStatementSyntax = usingStatementSyntax1
			Else
				usingStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax)
			End If
			Return usingStatementSyntax
		End Function

		Friend Shared Function VariableDeclarator(ByVal names As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax
			Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(122, names.Node, asClause, initializer, num)
			If (greenNode Is Nothing) Then
				Dim variableDeclaratorSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(SyntaxKind.VariableDeclarator, names.Node, asClause, initializer)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(variableDeclaratorSyntax1, num)
				End If
				variableDeclaratorSyntax = variableDeclaratorSyntax1
			Else
				variableDeclaratorSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)
			End If
			Return variableDeclaratorSyntax
		End Function

		Friend Shared Function VariableNameEquals(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal equalsToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(356, identifier, asClause, equalsToken, num)
			If (greenNode Is Nothing) Then
				Dim variableNameEqualsSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax(SyntaxKind.VariableNameEquals, identifier, asClause, equalsToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(variableNameEqualsSyntax1, num)
				End If
				variableNameEqualsSyntax = variableNameEqualsSyntax1
			Else
				variableNameEqualsSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)
			End If
			Return variableNameEqualsSyntax
		End Function

		Friend Shared Function WhereClause(ByVal whereKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax
			Dim whereClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(363, whereKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim whereClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax(SyntaxKind.WhereClause, whereKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whereClauseSyntax1, num)
				End If
				whereClauseSyntax = whereClauseSyntax1
			Else
				whereClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax)
			End If
			Return whereClauseSyntax
		End Function

		Friend Shared Function WhileBlock(ByVal whileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endWhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax
			Dim whileBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(141, whileStatement, statements.Node, endWhileStatement, num)
			If (greenNode Is Nothing) Then
				Dim whileBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax(SyntaxKind.WhileBlock, whileStatement, statements.Node, endWhileStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileBlockSyntax1, num)
				End If
				whileBlockSyntax = whileBlockSyntax1
			Else
				whileBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax)
			End If
			Return whileBlockSyntax
		End Function

		Friend Shared Function WhileClause(ByVal whileOrUntilKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(776, whileOrUntilKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim whileOrUntilClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(SyntaxKind.WhileClause, whileOrUntilKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax1, num)
				End If
				whileOrUntilClauseSyntax = whileOrUntilClauseSyntax1
			Else
				whileOrUntilClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			End If
			Return whileOrUntilClauseSyntax
		End Function

		Friend Shared Function WhileOrUntilClause(ByVal kind As SyntaxKind, ByVal whileOrUntilKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(CInt(kind), whileOrUntilKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim whileOrUntilClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(kind, whileOrUntilKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax1, num)
				End If
				whileOrUntilClauseSyntax = whileOrUntilClauseSyntax1
			Else
				whileOrUntilClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			End If
			Return whileOrUntilClauseSyntax
		End Function

		Friend Shared Function WhileStatement(ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax
			Dim whileStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(234, whileKeyword, condition, num)
			If (greenNode Is Nothing) Then
				Dim whileStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax(SyntaxKind.WhileStatement, whileKeyword, condition)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(whileStatementSyntax1, num)
				End If
				whileStatementSyntax = whileStatementSyntax1
			Else
				whileStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax)
			End If
			Return whileStatementSyntax
		End Function

		Friend Shared Function Whitespace(ByVal text As String, Optional ByVal elastic As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, text)
			syntaxTrivium = If(elastic, syntaxTrivium1.WithAnnotations(New SyntaxAnnotation() { SyntaxAnnotation.ElasticAnnotation }), syntaxTrivium1)
			Return syntaxTrivium
		End Function

		Friend Shared Function WhitespaceTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, text)
		End Function

		Friend Shared Function WithBlock(ByVal withStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax, ByVal statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endWithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax
			Dim withBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(146, withStatement, statements.Node, endWithStatement, num)
			If (greenNode Is Nothing) Then
				Dim withBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax(SyntaxKind.WithBlock, withStatement, statements.Node, endWithStatement)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withBlockSyntax1, num)
				End If
				withBlockSyntax = withBlockSyntax1
			Else
				withBlockSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax)
			End If
			Return withBlockSyntax
		End Function

		Friend Shared Function WithEventsEventContainer(ByVal identifier As IdentifierTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax
			Dim withEventsEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(115, identifier, num)
			If (greenNode Is Nothing) Then
				Dim withEventsEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax(SyntaxKind.WithEventsEventContainer, identifier)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withEventsEventContainerSyntax1, num)
				End If
				withEventsEventContainerSyntax = withEventsEventContainerSyntax1
			Else
				withEventsEventContainerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax)
			End If
			Return withEventsEventContainerSyntax
		End Function

		Friend Shared Function WithEventsPropertyEventContainer(ByVal withEventsContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax
			Dim withEventsPropertyEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(116, withEventsContainer, dotToken, [property], num)
			If (greenNode Is Nothing) Then
				Dim withEventsPropertyEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax(SyntaxKind.WithEventsPropertyEventContainer, withEventsContainer, dotToken, [property])
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withEventsPropertyEventContainerSyntax1, num)
				End If
				withEventsPropertyEventContainerSyntax = withEventsPropertyEventContainerSyntax1
			Else
				withEventsPropertyEventContainerSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax)
			End If
			Return withEventsPropertyEventContainerSyntax
		End Function

		Friend Shared Function WithStatement(ByVal withKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax
			Dim withStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(265, withKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim withStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax(SyntaxKind.WithStatement, withKeyword, expression)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(withStatementSyntax1, num)
				End If
				withStatementSyntax = withStatementSyntax1
			Else
				withStatementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax)
			End If
			Return withStatementSyntax
		End Function

		Friend Shared Function XmlAttribute(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax
			Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(386, name, equalsToken, value, num)
			If (greenNode Is Nothing) Then
				Dim xmlAttributeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax(SyntaxKind.XmlAttribute, name, equalsToken, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlAttributeSyntax1, num)
				End If
				xmlAttributeSyntax = xmlAttributeSyntax1
			Else
				xmlAttributeSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)
			End If
			Return xmlAttributeSyntax
		End Function

		Friend Shared Function XmlAttributeAccessExpression(ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(SyntaxKind.XmlAttributeAccessExpression, base, token1, token2, token3, name)
		End Function

		Friend Shared Function XmlBracketedName(ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim xmlBracketedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(390, lessThanToken, name, greaterThanToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlBracketedNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax(SyntaxKind.XmlBracketedName, lessThanToken, name, greaterThanToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlBracketedNameSyntax1, num)
				End If
				xmlBracketedNameSyntax = xmlBracketedNameSyntax1
			Else
				xmlBracketedNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)
			End If
			Return xmlBracketedNameSyntax
		End Function

		Friend Shared Function XmlCDataSection(ByVal beginCDataToken As PunctuationSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endCDataToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax
			Dim xmlCDataSectionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(394, beginCDataToken, textTokens.Node, endCDataToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlCDataSectionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax(SyntaxKind.XmlCDataSection, beginCDataToken, textTokens.Node, endCDataToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlCDataSectionSyntax1, num)
				End If
				xmlCDataSectionSyntax = xmlCDataSectionSyntax1
			Else
				xmlCDataSectionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)
			End If
			Return xmlCDataSectionSyntax
		End Function

		Friend Shared Function XmlComment(ByVal lessThanExclamationMinusMinusToken As PunctuationSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal minusMinusGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax
			Dim xmlCommentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(392, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlCommentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(SyntaxKind.XmlComment, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlCommentSyntax1, num)
				End If
				xmlCommentSyntax = xmlCommentSyntax1
			Else
				xmlCommentSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)
			End If
			Return xmlCommentSyntax
		End Function

		Friend Shared Function XmlCrefAttribute(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax, ByVal endQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax(SyntaxKind.XmlCrefAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken)
		End Function

		Friend Shared Function XmlDeclaration(ByVal lessThanQuestionToken As PunctuationSyntax, ByVal xmlKeyword As KeywordSyntax, ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal questionGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(SyntaxKind.XmlDeclaration, lessThanQuestionToken, xmlKeyword, version, encoding, standalone, questionGreaterThanToken)
		End Function

		Friend Shared Function XmlDeclarationOption(ByVal name As XmlNameTokenSyntax, ByVal equals As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim xmlDeclarationOptionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(380, name, equals, value, num)
			If (greenNode Is Nothing) Then
				Dim xmlDeclarationOptionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(SyntaxKind.XmlDeclarationOption, name, equals, value)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlDeclarationOptionSyntax1, num)
				End If
				xmlDeclarationOptionSyntax = xmlDeclarationOptionSyntax1
			Else
				xmlDeclarationOptionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			End If
			Return xmlDeclarationOptionSyntax
		End Function

		Friend Shared Function XmlDescendantAccessExpression(ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(SyntaxKind.XmlDescendantAccessExpression, base, token1, token2, token3, name)
		End Function

		Friend Shared Function XmlDocument(ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax, ByVal precedingMisc As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal followingMisc As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(SyntaxKind.XmlDocument, declaration, precedingMisc.Node, root, followingMisc.Node)
		End Function

		Friend Shared Function XmlElement(ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax, ByVal content As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax
			Dim xmlElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(381, startTag, content.Node, endTag, num)
			If (greenNode Is Nothing) Then
				Dim xmlElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(SyntaxKind.XmlElement, startTag, content.Node, endTag)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlElementSyntax1, num)
				End If
				xmlElementSyntax = xmlElementSyntax1
			Else
				xmlElementSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax)
			End If
			Return xmlElementSyntax
		End Function

		Friend Shared Function XmlElementAccessExpression(ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(SyntaxKind.XmlElementAccessExpression, base, token1, token2, token3, name)
		End Function

		Friend Shared Function XmlElementEndTag(ByVal lessThanSlashToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax
			Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(384, lessThanSlashToken, name, greaterThanToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlElementEndTagSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax(SyntaxKind.XmlElementEndTag, lessThanSlashToken, name, greaterThanToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlElementEndTagSyntax1, num)
				End If
				xmlElementEndTagSyntax = xmlElementEndTagSyntax1
			Else
				xmlElementEndTagSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)
			End If
			Return xmlElementEndTagSyntax
		End Function

		Friend Shared Function XmlElementStartTag(ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(SyntaxKind.XmlElementStartTag, lessThanToken, name, attributes.Node, greaterThanToken)
		End Function

		Friend Shared Function XmlEmbeddedExpression(ByVal lessThanPercentEqualsToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal percentGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax
			Dim xmlEmbeddedExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(395, lessThanPercentEqualsToken, expression, percentGreaterThanToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlEmbeddedExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax(SyntaxKind.XmlEmbeddedExpression, lessThanPercentEqualsToken, expression, percentGreaterThanToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlEmbeddedExpressionSyntax1, num)
				End If
				xmlEmbeddedExpressionSyntax = xmlEmbeddedExpressionSyntax1
			Else
				xmlEmbeddedExpressionSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)
			End If
			Return xmlEmbeddedExpressionSyntax
		End Function

		Friend Shared Function XmlEmptyElement(ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal slashGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax(SyntaxKind.XmlEmptyElement, lessThanToken, name, attributes.Node, slashGreaterThanToken)
		End Function

		Friend Shared Function XmlEntityLiteralToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(SyntaxKind.XmlEntityLiteralToken, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function XmlMemberAccessExpression(ByVal kind As SyntaxKind, ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(kind, base, token1, token2, token3, name)
		End Function

		Friend Shared Function XmlName(ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax, ByVal localName As XmlNameTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(389, prefix, localName, num)
			If (greenNode Is Nothing) Then
				Dim xmlNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(SyntaxKind.XmlName, prefix, localName)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlNameSyntax1, num)
				End If
				xmlNameSyntax = xmlNameSyntax1
			Else
				xmlNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			End If
			Return xmlNameSyntax
		End Function

		Friend Shared Function XmlNameAttribute(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal endQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(SyntaxKind.XmlNameAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken)
		End Function

		Friend Shared Function XmlNamespaceImportsClause(ByVal lessThanToken As PunctuationSyntax, ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax, ByVal greaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax
			Dim xmlNamespaceImportsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(45, lessThanToken, xmlNamespace, greaterThanToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlNamespaceImportsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(SyntaxKind.XmlNamespaceImportsClause, lessThanToken, xmlNamespace, greaterThanToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlNamespaceImportsClauseSyntax1, num)
				End If
				xmlNamespaceImportsClauseSyntax = xmlNamespaceImportsClauseSyntax1
			Else
				xmlNamespaceImportsClauseSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax)
			End If
			Return xmlNamespaceImportsClauseSyntax
		End Function

		Friend Shared Function XmlNameToken(ByVal text As String, ByVal possibleKeywordKind As SyntaxKind, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlNameTokenSyntax
			Return New XmlNameTokenSyntax(SyntaxKind.XmlNameToken, text, leadingTrivia, trailingTrivia, possibleKeywordKind)
		End Function

		Friend Shared Function XmlPrefix(ByVal name As XmlNameTokenSyntax, ByVal colonToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(391, name, colonToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlPrefixSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax(SyntaxKind.XmlPrefix, name, colonToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlPrefixSyntax1, num)
				End If
				xmlPrefixSyntax = xmlPrefixSyntax1
			Else
				xmlPrefixSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)
			End If
			Return xmlPrefixSyntax
		End Function

		Friend Shared Function XmlPrefixName(ByVal name As XmlNameTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax
			Dim xmlPrefixNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(388, name, num)
			If (greenNode Is Nothing) Then
				Dim xmlPrefixNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(SyntaxKind.XmlPrefixName, name)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlPrefixNameSyntax1, num)
				End If
				xmlPrefixNameSyntax = xmlPrefixNameSyntax1
			Else
				xmlPrefixNameSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax)
			End If
			Return xmlPrefixNameSyntax
		End Function

		Friend Shared Function XmlProcessingInstruction(ByVal lessThanQuestionToken As PunctuationSyntax, ByVal name As XmlNameTokenSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal questionGreaterThanToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(SyntaxKind.XmlProcessingInstruction, lessThanQuestionToken, name, textTokens.Node, questionGreaterThanToken)
		End Function

		Friend Shared Function XmlString(ByVal startQuoteToken As PunctuationSyntax, ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode), ByVal endQuoteToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(387, startQuoteToken, textTokens.Node, endQuoteToken, num)
			If (greenNode Is Nothing) Then
				Dim xmlStringSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax(SyntaxKind.XmlString, startQuoteToken, textTokens.Node, endQuoteToken)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlStringSyntax1, num)
				End If
				xmlStringSyntax = xmlStringSyntax1
			Else
				xmlStringSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)
			End If
			Return xmlStringSyntax
		End Function

		Friend Shared Function XmlText(ByVal textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax
			Dim xmlTextSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(382, textTokens.Node, num)
			If (greenNode Is Nothing) Then
				Dim xmlTextSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax(SyntaxKind.XmlText, textTokens.Node)
				If (num >= 0) Then
					SyntaxNodeCache.AddNode(xmlTextSyntax1, num)
				End If
				xmlTextSyntax = xmlTextSyntax1
			Else
				xmlTextSyntax = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax)
			End If
			Return xmlTextSyntax
		End Function

		Friend Shared Function XmlTextLiteralToken(ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(SyntaxKind.XmlTextLiteralToken, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function XmlTextToken(ByVal kind As SyntaxKind, ByVal text As String, ByVal value As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As XmlTextTokenSyntax
			Return New XmlTextTokenSyntax(kind, text, leadingTrivia, trailingTrivia, value)
		End Function

		Friend Shared Function YieldStatement(ByVal yieldKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax
			Dim yieldStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax
			Dim num As Integer
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = SyntaxNodeCache.TryGetNode(411, yieldKeyword, expression, num)
			If (greenNode Is Nothing) Then
				Dim yieldStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax(SyntaxKind.YieldStatement, yieldKeyword, expression)
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