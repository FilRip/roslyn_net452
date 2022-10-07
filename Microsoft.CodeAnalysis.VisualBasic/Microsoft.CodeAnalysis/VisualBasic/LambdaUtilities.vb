Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LambdaUtilities
		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Sub AddFirstJoinVariableRecursive(ByVal result As ArrayBuilder(Of SyntaxNode), ByVal joinClause As JoinClauseSyntax)
			result.Add(joinClause.JoinedVariables.First().Expression)
			Dim enumerator As SyntaxList(Of JoinClauseSyntax).Enumerator = joinClause.AdditionalJoins.GetEnumerator()
			While enumerator.MoveNext()
				LambdaUtilities.AddFirstJoinVariableRecursive(result, enumerator.Current)
			End While
		End Sub

		Public Shared Function AreEquivalentIgnoringLambdaBodies(ByVal oldNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal newNode As Microsoft.CodeAnalysis.SyntaxNode) As Boolean
			Dim syntaxTokens As IEnumerable(Of SyntaxToken) = oldNode.DescendantTokens(Function(node As Microsoft.CodeAnalysis.SyntaxNode)
				If (node = oldNode) Then
					Return True
				End If
				Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = Nothing
				Return Not LambdaUtilities.IsLambdaBodyStatementOrExpression(node, syntaxNode1)
			End Function, False)
			Dim syntaxTokens1 As IEnumerable(Of SyntaxToken) = newNode.DescendantTokens(Function(node As Microsoft.CodeAnalysis.SyntaxNode)
				If (node = newNode) Then
					Return True
				End If
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Nothing
				Return Not LambdaUtilities.IsLambdaBodyStatementOrExpression(node, syntaxNode)
			End Function, False)
			Return syntaxTokens.SequenceEqual(syntaxTokens1, New Func(Of SyntaxToken, SyntaxToken, Boolean)(AddressOf SyntaxFactory.AreEquivalent))
		End Function

		Private Shared Function EnumerateExpressions(ByVal variables As SeparatedSyntaxList(Of ExpressionRangeVariableSyntax)) As IEnumerable(Of SyntaxNode)
			Return New LambdaUtilities.VB$StateMachine_24_EnumerateExpressions(-2) With
			{
				.$P_variables = variables
			}
		End Function

		Private Shared Function EnumerateJoinClauseLeftExpressions(ByVal clause As JoinClauseSyntax) As IEnumerable(Of SyntaxNode)
			Return New LambdaUtilities.VB$StateMachine_25_EnumerateJoinClauseLeftExpressions(-2) With
			{
				.$P_clause = clause
			}
		End Function

		Private Shared Function EnumerateJoinClauseRightExpressions(ByVal clause As JoinClauseSyntax) As IEnumerable(Of SyntaxNode)
			Return New LambdaUtilities.VB$StateMachine_26_EnumerateJoinClauseRightExpressions(-2) With
			{
				.$P_clause = clause
			}
		End Function

		Friend Shared Function GetAggregateLambdaBody(ByVal aggregateClause As AggregateClauseSyntax) As VisualBasicSyntaxNode
			Return aggregateClause.Variables.First().Expression
		End Function

		Private Shared Function GetAggregateLambdaBodyExpressions(ByVal clause As AggregateClauseSyntax) As IEnumerable(Of SyntaxNode)
			Dim instance As ArrayBuilder(Of SyntaxNode) = ArrayBuilder(Of SyntaxNode).GetInstance()
			instance.Add(clause.Variables.First().Expression)
			Dim enumerator As SyntaxList(Of QueryClauseSyntax).Enumerator = clause.AdditionalQueryOperators.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As QueryClauseSyntax = enumerator.Current
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = current.Kind()
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipClause) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					instance.Add(DirectCast(current, PartitionClauseSyntax).Count)
				ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleJoinClause) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					LambdaUtilities.AddFirstJoinVariableRecursive(instance, DirectCast(current, JoinClauseSyntax))
				End If
			End While
			Return DirectCast(instance.ToImmutableAndFree(), IEnumerable(Of SyntaxNode))
		End Function

		Friend Shared Function GetAggregationLambdaBody(ByVal aggregation As FunctionAggregationSyntax) As VisualBasicSyntaxNode
			Return aggregation.Argument
		End Function

		Private Shared Function GetCollectionRangeVariables(ByVal clause As SyntaxNode) As SeparatedSyntaxList(Of CollectionRangeVariableSyntax)
			Dim variables As SeparatedSyntaxList(Of CollectionRangeVariableSyntax)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = clause.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromClause) Then
				variables = DirectCast(clause, FromClauseSyntax).Variables
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause) Then
				variables = DirectCast(clause, AggregateClauseSyntax).Variables
			Else
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleJoinClause) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					Throw ExceptionUtilities.UnexpectedValue(clause.Kind())
				End If
				variables = DirectCast(clause, JoinClauseSyntax).JoinedVariables
			End If
			Return variables
		End Function

		Public Shared Function GetCorrespondingLambdaBody(ByVal oldBody As Microsoft.CodeAnalysis.SyntaxNode, ByVal newLambdaOrPeer As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.SyntaxNode
			Dim expression As Microsoft.CodeAnalysis.SyntaxNode
			Dim lambda As Microsoft.CodeAnalysis.SyntaxNode = LambdaUtilities.GetLambda(oldBody)
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Nothing
			If (Not LambdaUtilities.TryGetSimpleLambdaBody(newLambdaOrPeer, syntaxNode)) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = lambda.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable) Then
					expression = DirectCast(newLambdaOrPeer, CollectionRangeVariableSyntax).Expression
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable) Then
					expression = LambdaUtilities.GetExpressionRangeVariableLambdaBody(DirectCast(newLambdaOrPeer, ExpressionRangeVariableSyntax))
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinCondition) Then
						Throw ExceptionUtilities.UnexpectedValue(lambda.Kind())
					End If
					Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax = DirectCast(lambda, Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
					Dim parent As JoinClauseSyntax = DirectCast(DirectCast(newLambdaOrPeer, Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax).Parent, JoinClauseSyntax)
					expression = If(joinConditionSyntax.Left = oldBody, LambdaUtilities.GetJoinLeftLambdaBody(parent), LambdaUtilities.GetJoinRightLambdaBody(parent))
				End If
			Else
				expression = syntaxNode
			End If
			Return expression
		End Function

		Private Shared Function GetExpressionRangeVariableLambdaBody(ByVal rangeVariable As ExpressionRangeVariableSyntax) As SyntaxNode
			Dim letVariableLambdaBody As SyntaxNode
			Dim parent As VisualBasicSyntaxNode = rangeVariable.Parent
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetClause) Then
				letVariableLambdaBody = LambdaUtilities.GetLetVariableLambdaBody(rangeVariable)
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupByClause) Then
				Dim groupByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax)
				letVariableLambdaBody = If(rangeVariable.SpanStart < groupByClauseSyntax.ByKeyword.SpanStart OrElse rangeVariable.SpanStart = groupByClauseSyntax.ByKeyword.SpanStart AndAlso rangeVariable = groupByClauseSyntax.Items.Last(), LambdaUtilities.GetGroupByItemsLambdaBody(groupByClauseSyntax), LambdaUtilities.GetGroupByKeysLambdaBody(groupByClauseSyntax))
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectClause) Then
					Throw ExceptionUtilities.UnexpectedValue(parent.Kind())
				End If
				letVariableLambdaBody = LambdaUtilities.GetSelectLambdaBody(DirectCast(parent, SelectClauseSyntax))
			End If
			Return letVariableLambdaBody
		End Function

		Friend Shared Function GetFromOrAggregateVariableLambdaBody(ByVal rangeVariable As CollectionRangeVariableSyntax) As VisualBasicSyntaxNode
			Return rangeVariable.Expression
		End Function

		Friend Shared Function GetGroupByItemsLambdaBody(ByVal groupByClause As GroupByClauseSyntax) As VisualBasicSyntaxNode
			Return groupByClause.Items.First().Expression
		End Function

		Friend Shared Function GetGroupByKeysLambdaBody(ByVal groupByClause As GroupByClauseSyntax) As VisualBasicSyntaxNode
			Return groupByClause.Keys.First().Expression
		End Function

		Friend Shared Function GetJoinLeftLambdaBody(ByVal joinClause As JoinClauseSyntax) As VisualBasicSyntaxNode
			Return joinClause.JoinConditions.First().Left
		End Function

		Friend Shared Function GetJoinRightLambdaBody(ByVal joinClause As JoinClauseSyntax) As VisualBasicSyntaxNode
			Return joinClause.JoinConditions.First().Right
		End Function

		Public Shared Function GetLambda(ByVal lambdaBody As SyntaxNode) As SyntaxNode
			Return lambdaBody.Parent
		End Function

		Friend Shared Function GetLambdaBodyExpressionsAndStatements(ByVal lambdaBody As Microsoft.CodeAnalysis.SyntaxNode) As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)
			Dim statements As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)
			Dim lambda As Microsoft.CodeAnalysis.SyntaxNode = LambdaUtilities.GetLambda(lambdaBody)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = lambda.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionAggregation) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression
						statements = SpecializedCollections.SingletonEnumerable(Of VisualBasicSyntaxNode)(DirectCast(lambda, SingleLineLambdaExpressionSyntax).Body)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryConditionalExpression
						Throw ExceptionUtilities.UnexpectedValue(lambda.Kind())
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineFunctionLambdaExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineSubLambdaExpression
						statements = DirectCast(DirectCast(lambda, MultiLineLambdaExpressionSyntax).Statements, IEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode))
						Exit Select
					Case Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable
								Dim parent As Microsoft.CodeAnalysis.SyntaxNode = lambda.Parent
								Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
								If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromClause) Then
									statements = SpecializedCollections.SingletonEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)(lambdaBody)
								Else
									If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause) Then
										Throw ExceptionUtilities.UnexpectedValue(parent.Kind())
									End If
									Dim aggregateClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax)
									statements = If(lambda <> aggregateClauseSyntax.Variables.First(), SpecializedCollections.SingletonEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)(lambdaBody), LambdaUtilities.GetAggregateLambdaBodyExpressions(aggregateClauseSyntax))
								End If

							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable
								Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = lambda.Parent
								Dim syntaxKind2 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntaxNode.Kind()
								If (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetClause) Then
									statements = SpecializedCollections.SingletonEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)(lambdaBody)
								ElseIf (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupByClause) Then
									Dim groupByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax = DirectCast(syntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax)
									statements = If(lambdaBody.SpanStart >= groupByClauseSyntax.ByKeyword.SpanStart, LambdaUtilities.EnumerateExpressions(groupByClauseSyntax.Keys), LambdaUtilities.EnumerateExpressions(groupByClauseSyntax.Items))
								Else
									If (syntaxKind2 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectClause) Then
										Throw ExceptionUtilities.UnexpectedValue(syntaxNode.Kind())
									End If
									statements = LambdaUtilities.EnumerateExpressions(DirectCast(syntaxNode, SelectClauseSyntax).Variables)
								End If

							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregationRangeVariable
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableNameEquals
								Throw ExceptionUtilities.UnexpectedValue(lambda.Kind())
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionAggregation
								statements = SpecializedCollections.SingletonEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)(lambdaBody)
								Return statements
							Case Else
								Throw ExceptionUtilities.UnexpectedValue(lambda.Kind())
						End Select

				End Select
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereClause) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinCondition) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingOrdering) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						statements = SpecializedCollections.SingletonEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)(lambdaBody)
						Return statements
					End If
					Throw ExceptionUtilities.UnexpectedValue(lambda.Kind())
				End If
				Dim joinClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax = DirectCast(lambda.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)
				statements = If(lambdaBody <> DirectCast(lambda, JoinConditionSyntax).Left, LambdaUtilities.EnumerateJoinClauseRightExpressions(joinClauseSyntax), LambdaUtilities.EnumerateJoinClauseLeftExpressions(joinClauseSyntax))
			Else
				statements = SpecializedCollections.SingletonEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode)(lambdaBody)
				Return statements
			End If
			Return statements
		End Function

		Friend Shared Function GetLambdaExpressionLambdaBody(ByVal lambda As LambdaExpressionSyntax) As VisualBasicSyntaxNode
			Return lambda.SubOrFunctionHeader
		End Function

		Friend Shared Function GetLetVariableLambdaBody(ByVal rangeVariable As ExpressionRangeVariableSyntax) As VisualBasicSyntaxNode
			Return rangeVariable.Expression
		End Function

		Friend Shared Function GetOrderingLambdaBody(ByVal ordering As OrderingSyntax) As VisualBasicSyntaxNode
			Return ordering.Expression
		End Function

		Friend Shared Function GetSelectLambdaBody(ByVal selectClause As SelectClauseSyntax) As VisualBasicSyntaxNode
			Return selectClause.Variables.First().Expression
		End Function

		Friend Shared Function IsClosureScope(ByVal node As Microsoft.CodeAnalysis.SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim parent As Microsoft.CodeAnalysis.SyntaxNode
			Dim parent1 As Boolean
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyBlock) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression) Then
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause) Then
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleJoinClause) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							flag = True
							Return flag
						End If
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) <= 4) Then
							flag = True
							Return flag
						End If
						If (Not LambdaUtilities.IsLambdaBody(node)) Then
							parent = node.Parent
							If (parent IsNot Nothing) Then
								parent1 = parent.Parent
							Else
								syntaxNode = parent
								parent1 = False
							End If
							flag = If(Not parent1 OrElse node.Parent.Parent.Parent IsNot Nothing, False, True)
						Else
							flag = True
						End If
						Return flag
					Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
							flag = True
							Return flag
						End If
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause) Then
							flag = True
							Return flag
						End If
						If (Not LambdaUtilities.IsLambdaBody(node)) Then
							parent = node.Parent
							If (parent IsNot Nothing) Then
								parent1 = parent.Parent
							Else
								syntaxNode = parent
								parent1 = False
							End If
							flag = If(Not parent1 OrElse node.Parent.Parent.Parent IsNot Nothing, False, True)
						Else
							flag = True
						End If
						Return flag
					End If
					flag = True
					Return flag
				ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseBlock) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						flag = True
						Return flag
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression) Then
						flag = True
						Return flag
					End If
					If (Not LambdaUtilities.IsLambdaBody(node)) Then
						parent = node.Parent
						If (parent IsNot Nothing) Then
							parent1 = parent.Parent
						Else
							syntaxNode = parent
							parent1 = False
						End If
						flag = If(Not parent1 OrElse node.Parent.Parent.Parent IsNot Nothing, False, True)
					Else
						flag = True
					End If
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseBlock) Then
						flag = True
						Return flag
					End If
					If (Not LambdaUtilities.IsLambdaBody(node)) Then
						parent = node.Parent
						If (parent IsNot Nothing) Then
							parent1 = parent.Parent
						Else
							syntaxNode = parent
							parent1 = False
						End If
						flag = If(Not parent1 OrElse node.Parent.Parent.Parent IsNot Nothing, False, True)
					Else
						flag = True
					End If
					Return flag
				End If
				flag = True
				Return flag
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement)) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock) Then
							flag = True
							Return flag
						End If
						If (Not LambdaUtilities.IsLambdaBody(node)) Then
							parent = node.Parent
							If (parent IsNot Nothing) Then
								parent1 = parent.Parent
							Else
								syntaxNode = parent
								parent1 = False
							End If
							flag = If(Not parent1 OrElse node.Parent.Parent.Parent IsNot Nothing, False, True)
						Else
							flag = True
						End If
						Return flag
					End If
					flag = True
					Return flag
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock) Then
						If (Not LambdaUtilities.IsLambdaBody(node)) Then
							parent = node.Parent
							If (parent IsNot Nothing) Then
								parent1 = parent.Parent
							Else
								syntaxNode = parent
								parent1 = False
							End If
							flag = If(Not parent1 OrElse node.Parent.Parent.Parent IsNot Nothing, False, True)
						Else
							flag = True
						End If
						Return flag
					End If
					flag = True
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement) Then
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement) Then
					flag = True
					Return flag
				End If
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyBlock
						flag = True
						Return flag
				End Select
			Else
				flag = True
				Return flag
			End If
			If (Not LambdaUtilities.IsLambdaBody(node)) Then
				parent = node.Parent
				If (parent IsNot Nothing) Then
					parent1 = parent.Parent
				Else
					syntaxNode = parent
					parent1 = False
				End If
				flag = If(Not parent1 OrElse node.Parent.Parent.Parent IsNot Nothing, False, True)
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function IsJoinClause(ByVal node As SyntaxNode) As Boolean
			If (node.IsKind(SyntaxKind.GroupJoinClause)) Then
				Return True
			End If
			Return node.IsKind(SyntaxKind.SimpleJoinClause)
		End Function

		Public Shared Function IsLambda(ByVal node As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionAggregation) Then
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable
							flag = LambdaUtilities.IsLambdaCollectionRangeVariable(node)
							Return flag
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable
							flag = LambdaUtilities.IsLambdaExpressionRangeVariable(node)
							Return flag
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregationRangeVariable
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableNameEquals
							flag = False
							Return flag
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionAggregation
							Exit Select
						Case Else
							flag = False
							Return flag
					End Select
				End If
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereClause) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinCondition) Then
					GoTo Label2
				End If
				flag = LambdaUtilities.IsLambdaJoinCondition(node)
				Return flag
			End If
			flag = True
			Return flag
		Label2:
			If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingOrdering) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				flag = False
				Return flag
			Else
				flag = True
				Return flag
			End If
		End Function

		Public Shared Function IsLambdaBody(ByVal node As Microsoft.CodeAnalysis.SyntaxNode) As Boolean
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Nothing
			If (Not LambdaUtilities.IsLambdaBodyStatementOrExpression(node, syntaxNode)) Then
				Return False
			End If
			Return node = syntaxNode
		End Function

		Public Shared Function IsLambdaBodyStatementOrExpression(ByVal node As Microsoft.CodeAnalysis.SyntaxNode, <Out> Optional ByRef lambdaBody As Microsoft.CodeAnalysis.SyntaxNode = Nothing) As Boolean
			Dim flag As Boolean
			Dim parent As Microsoft.CodeAnalysis.SyntaxNode
			If (node IsNot Nothing) Then
				parent = node.Parent
			Else
				parent = Nothing
			End If
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = parent
			If (syntaxNode Is Nothing) Then
				lambdaBody = Nothing
				flag = False
			ElseIf (Not LambdaUtilities.TryGetSimpleLambdaBody(syntaxNode, lambdaBody)) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntaxNode.Kind()
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipClause) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						Dim parent1 As Microsoft.CodeAnalysis.SyntaxNode = syntaxNode.Parent
						If (Not parent1.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause) OrElse LambdaUtilities.IsQueryStartingClause(parent1)) Then
							GoTo Label1
						End If
						lambdaBody = LambdaUtilities.GetAggregateLambdaBody(DirectCast(parent1, AggregateClauseSyntax))
						flag = True
						Return flag
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinCondition) Then
							GoTo Label1
						End If
						Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax = DirectCast(syntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax)
						Dim joinClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax = DirectCast(syntaxNode.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinClauseSyntax)
						If (node <> joinConditionSyntax.Left) Then
							lambdaBody = LambdaUtilities.GetJoinRightLambdaBody(joinClauseSyntax)
						Else
							lambdaBody = LambdaUtilities.GetJoinLeftLambdaBody(joinClauseSyntax)
						End If
						flag = True
						Return flag
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable) Then
						Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax = DirectCast(syntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
						If (node <> expressionRangeVariableSyntax.Expression) Then
							GoTo Label1
						End If
						lambdaBody = LambdaUtilities.GetExpressionRangeVariableLambdaBody(expressionRangeVariableSyntax)
						flag = True
						Return flag
					End If
				ElseIf (node = DirectCast(syntaxNode, CollectionRangeVariableSyntax).Expression) Then
					Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = syntaxNode.Parent
					If (LambdaUtilities.IsLambdaCollectionRangeVariable(syntaxNode)) Then
						lambdaBody = node
						flag = True
						Return flag
					ElseIf (LambdaUtilities.IsJoinClause(syntaxNode1)) Then
						Dim parent2 As Microsoft.CodeAnalysis.SyntaxNode = syntaxNode1.Parent
						Do
							syntaxNode1 = syntaxNode1.Parent
						Loop While LambdaUtilities.IsJoinClause(syntaxNode1)
						If (Not syntaxNode1.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause) OrElse LambdaUtilities.IsQueryStartingClause(syntaxNode1)) Then
							GoTo Label1
						End If
						lambdaBody = LambdaUtilities.GetAggregateLambdaBody(DirectCast(syntaxNode1, AggregateClauseSyntax))
						flag = True
						Return flag
					End If
				End If
			Label1:
				lambdaBody = Nothing
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Shared Function IsLambdaCollectionRangeVariable(ByVal collectionRangeVariable As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim parent As SyntaxNode = collectionRangeVariable.Parent
			If (LambdaUtilities.IsJoinClause(parent)) Then
				flag = False
			ElseIf (Not LambdaUtilities.IsQueryStartingClause(parent)) Then
				flag = True
			Else
				Dim collectionRangeVariables As SeparatedSyntaxList(Of CollectionRangeVariableSyntax) = LambdaUtilities.GetCollectionRangeVariables(parent)
				flag = collectionRangeVariable <> collectionRangeVariables.First()
			End If
			Return flag
		End Function

		Friend Shared Function IsLambdaExpressionRangeVariable(ByVal expressionRangeVariable As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim items As SeparatedSyntaxList(Of ExpressionRangeVariableSyntax)
			Dim flag1 As Boolean
			Dim parent As SyntaxNode = expressionRangeVariable.Parent
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetClause) Then
				flag = True
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupByClause) Then
				Dim groupByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax)
				If (expressionRangeVariable = groupByClauseSyntax.Keys.First()) Then
					flag1 = True
				Else
					items = groupByClauseSyntax.Items
					flag1 = expressionRangeVariable = items.FirstOrDefault()
				End If
				flag = flag1
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectClause) Then
				items = DirectCast(parent, SelectClauseSyntax).Variables
				flag = expressionRangeVariable = items.First()
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function IsLambdaJoinCondition(ByVal joinCondition As SyntaxNode) As Boolean
			Return joinCondition = DirectCast(joinCondition.Parent, JoinClauseSyntax).JoinConditions.First()
		End Function

		Friend Shared Function IsNonUserCodeQueryLambda(ByVal syntax As SyntaxNode) As Boolean
			If (syntax.IsKind(SyntaxKind.GroupJoinClause) OrElse syntax.IsKind(SyntaxKind.SimpleJoinClause) OrElse syntax.IsKind(SyntaxKind.AggregateClause) OrElse syntax.IsKind(SyntaxKind.FromClause) OrElse syntax.IsKind(SyntaxKind.GroupByClause)) Then
				Return True
			End If
			Return syntax.IsKind(SyntaxKind.SimpleAsClause)
		End Function

		Public Shared Function IsNotLambda(ByVal node As SyntaxNode) As Boolean
			Return Not LambdaUtilities.IsLambda(node)
		End Function

		Private Shared Function IsQueryStartingClause(ByVal clause As SyntaxNode) As Boolean
			If (Not clause.Parent.IsKind(SyntaxKind.QueryExpression)) Then
				Return False
			End If
			Return clause = DirectCast(clause.Parent, QueryExpressionSyntax).Clauses.First()
		End Function

		Public Shared Function TryGetLambdaBodies(ByVal node As SyntaxNode, <Out> ByRef lambdaBody1 As SyntaxNode, <Out> ByRef lambdaBody2 As SyntaxNode) As Boolean
			Dim flag As Boolean
			lambdaBody1 = Nothing
			lambdaBody2 = Nothing
			If (Not LambdaUtilities.TryGetSimpleLambdaBody(node, lambdaBody1)) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable) Then
					If (Not LambdaUtilities.IsLambdaCollectionRangeVariable(node)) Then
						flag = False
						Return flag
					End If
					lambdaBody1 = DirectCast(node, CollectionRangeVariableSyntax).Expression
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable) Then
					If (Not LambdaUtilities.IsLambdaExpressionRangeVariable(node)) Then
						flag = False
						Return flag
					End If
					lambdaBody1 = DirectCast(node, ExpressionRangeVariableSyntax).Expression
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinCondition) Then
					Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax = DirectCast(node.Parent, JoinClauseSyntax).JoinConditions.First()
					If (node <> joinConditionSyntax) Then
						flag = False
						Return flag
					End If
					lambdaBody1 = joinConditionSyntax.Left
					lambdaBody2 = joinConditionSyntax.Right
				Else
					flag = False
					Return flag
				End If
				flag = True
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function TryGetSimpleLambdaBody(ByVal node As SyntaxNode, <Out> ByRef lambdaBody As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionAggregation) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
					lambdaBody = LambdaUtilities.GetLambdaExpressionLambdaBody(DirectCast(node, LambdaExpressionSyntax))
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionAggregation) Then
						lambdaBody = Nothing
						flag = False
						Return flag
					End If
					lambdaBody = LambdaUtilities.GetAggregationLambdaBody(DirectCast(node, FunctionAggregationSyntax))
					If (lambdaBody IsNot Nothing) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereClause) Then
				lambdaBody = DirectCast(node, WhereClauseSyntax).Condition
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipWhileClause) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				lambdaBody = DirectCast(node, PartitionWhileClauseSyntax).Condition
			Else
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingOrdering) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					lambdaBody = Nothing
					flag = False
					Return flag
				End If
				lambdaBody = LambdaUtilities.GetOrderingLambdaBody(DirectCast(node, OrderingSyntax))
			End If
			flag = True
			Return flag
		End Function
	End Class
End Namespace