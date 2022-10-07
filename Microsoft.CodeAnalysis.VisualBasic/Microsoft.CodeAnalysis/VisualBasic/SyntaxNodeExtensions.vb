Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module SyntaxNodeExtensions
		<Extension>
		Public Function AllAreMissing(ByVal arguments As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode), ByVal kind As SyntaxKind) As Boolean
			Return Not arguments.Any(Function(arg As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
				If (arg.Kind() <> kind) Then
					Return True
				End If
				Return Not DirectCast(arg, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax).IsMissing
			End Function)
		End Function

		<Extension>
		Public Function AllAreMissingIdentifierName(ByVal arguments As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)) As Boolean
			Return arguments.AllAreMissing(SyntaxKind.IdentifierName)
		End Function

		<Extension>
		Public Function ContainingWithStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax
			Dim withStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax
			If (node IsNot Nothing) Then
				node = node.Parent
				While node IsNot Nothing
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock) <= CUShort((Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						Exit While
					End If
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock) Then
						node = node.Parent
					Else
						withStatement = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax).WithStatement
						Return withStatement
					End If
				End While
				withStatement = Nothing
			Else
				withStatement = Nothing
			End If
			Return withStatement
		End Function

		<Extension>
		Friend Function EnclosingStructuredTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax
			Dim structuredTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax
			While True
				If (node Is Nothing) Then
					structuredTriviaSyntax = Nothing
					Exit While
				ElseIf (Not node.IsStructuredTrivia) Then
					node = node.Parent
				Else
					structuredTriviaSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax)
					Exit While
				End If
			End While
			Return structuredTriviaSyntax
		End Function

		<Extension>
		Friend Function ExtractAnonymousTypeMemberName(ByVal input As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, <Out> ByRef failedToInferFromXmlName As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax) As Microsoft.CodeAnalysis.SyntaxToken
			Dim localName As Microsoft.CodeAnalysis.SyntaxToken
			failedToInferFromXmlName = Nothing
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = input.Kind()
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression
							Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax)
							If (input.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression) Then
								Dim expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = If(memberAccessExpressionSyntax.Expression, Microsoft.CodeAnalysis.VisualBasic.SyntaxNodeExtensions.GetCorrespondingConditionalAccessReceiver(memberAccessExpressionSyntax))
								If (expression IsNot Nothing AndAlso CUShort(expression.Kind()) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
									input = expression
									Continue While
								End If
							End If
							input = memberAccessExpressionSyntax.Name
							Continue While
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttributeAccessExpression
							input = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax).Name
							Continue While
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression
							Dim invocationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax)
							Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = If(invocationExpressionSyntax.Expression, Microsoft.CodeAnalysis.VisualBasic.SyntaxNodeExtensions.GetCorrespondingConditionalAccessReceiver(invocationExpressionSyntax))
							If (expressionSyntax Is Nothing) Then
								localName = New Microsoft.CodeAnalysis.SyntaxToken()
								Return localName
							End If
							If (invocationExpressionSyntax.ArgumentList Is Nothing OrElse invocationExpressionSyntax.ArgumentList.Arguments.Count = 0) Then
								input = expressionSyntax
								Continue While
							Else
								If (invocationExpressionSyntax.ArgumentList.Arguments.Count <> 1 OrElse CUShort(expressionSyntax.Kind()) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
									localName = New Microsoft.CodeAnalysis.SyntaxToken()
									Return localName
								End If
								input = expressionSyntax
								Continue While
							End If
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName) Then
								Exit Select
							Else
								localName = New Microsoft.CodeAnalysis.SyntaxToken()
								Return localName
							End If
					End Select
					Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)
					If (Scanner.IsIdentifier(xmlNameSyntax.LocalName.ToString())) Then
						localName = xmlNameSyntax.LocalName
						Return localName
					Else
						failedToInferFromXmlName = xmlNameSyntax
						localName = New Microsoft.CodeAnalysis.SyntaxToken()
						Return localName
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlBracketedName) Then
					input = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax).Name
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName) Then
					localName = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax).Identifier
					Return localName
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConditionalAccessExpression) Then
					input = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax).WhenNotNull
				Else
					Exit While
				End If
			End While
			localName = New Microsoft.CodeAnalysis.SyntaxToken()
			Return localName
		End Function

		<Extension>
		Public Function GetAncestorOrSelf(Of T As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(ByVal node As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As T
			Dim t1 As T
			While True
				If (node IsNot Nothing) Then
					Dim t2 As T = DirectCast(TryCast(node, T), T)
					If (t2 Is Nothing) Then
						node = node.Parent
					Else
						t1 = t2
						Exit While
					End If
				Else
					t1 = Nothing
					Exit While
				End If
			End While
			Return t1
		End Function

		<Extension>
		Public Sub GetAncestors(Of T As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, C As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(ByVal node As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal result As ArrayBuilder(Of T))
			Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = node.Parent
			While parent IsNot Nothing AndAlso Not TypeOf parent Is C
				If (TypeOf parent Is T) Then
					result.Add(DirectCast(parent, T))
				End If
				parent = parent.Parent
			End While
			result.ReverseContents()
		End Sub

		<Extension>
		Friend Function GetCorrespondingConditionalAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax
			Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = node
			Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = visualBasicSyntaxNode.Parent
			While True
				If (parent IsNot Nothing) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression
							If (DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax).Expression = visualBasicSyntaxNode) Then
								Exit Select
							End If
							conditionalAccessExpressionSyntax = Nothing
							Return conditionalAccessExpressionSyntax
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttributeAccessExpression
							If (DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax).Base = visualBasicSyntaxNode) Then
								Exit Select
							End If
							conditionalAccessExpressionSyntax = Nothing
							Return conditionalAccessExpressionSyntax
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression
							If (DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax).Expression = visualBasicSyntaxNode) Then
								Exit Select
							End If
							conditionalAccessExpressionSyntax = Nothing
							Return conditionalAccessExpressionSyntax
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConditionalAccessExpression) Then
								Dim conditionalAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax)
								If (conditionalAccessExpressionSyntax1.WhenNotNull <> visualBasicSyntaxNode) Then
									If (conditionalAccessExpressionSyntax1.Expression = visualBasicSyntaxNode) Then
										Exit Select
									End If
									conditionalAccessExpressionSyntax = Nothing
									Return conditionalAccessExpressionSyntax
								Else
									conditionalAccessExpressionSyntax = conditionalAccessExpressionSyntax1
									Return conditionalAccessExpressionSyntax
								End If
							Else
								conditionalAccessExpressionSyntax = Nothing
								Return conditionalAccessExpressionSyntax
							End If
					End Select
					visualBasicSyntaxNode = parent
					parent = visualBasicSyntaxNode.Parent
				Else
					conditionalAccessExpressionSyntax = Nothing
					Exit While
				End If
			End While
			Return conditionalAccessExpressionSyntax
		End Function

		Private Function GetCorrespondingConditionalAccessReceiver(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Dim expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Dim correspondingConditionalAccessExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax = node.GetCorrespondingConditionalAccessExpression()
			If (correspondingConditionalAccessExpression Is Nothing) Then
				expression = Nothing
			Else
				expression = correspondingConditionalAccessExpression.Expression
			End If
			Return expression
		End Function

		<Extension>
		Friend Function GetLeafAccess(ByVal conditionalAccess As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Dim whenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = conditionalAccess.WhenNotNull
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = whenNotNull.Kind()
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression
						Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax = DirectCast(whenNotNull, Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax)
						If (memberAccessExpressionSyntax.Expression IsNot Nothing) Then
							whenNotNull = memberAccessExpressionSyntax.Expression
							Continue While
						Else
							expressionSyntax = memberAccessExpressionSyntax
						End If

					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttributeAccessExpression
						Dim xmlMemberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax = DirectCast(whenNotNull, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax)
						If (xmlMemberAccessExpressionSyntax.Base IsNot Nothing) Then
							whenNotNull = xmlMemberAccessExpressionSyntax.Base
							Continue While
						Else
							expressionSyntax = xmlMemberAccessExpressionSyntax
						End If

					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression
						Dim invocationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax = DirectCast(whenNotNull, Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax)
						If (invocationExpressionSyntax.Expression IsNot Nothing) Then
							whenNotNull = invocationExpressionSyntax.Expression
							Continue While
						Else
							expressionSyntax = invocationExpressionSyntax
						End If

					Case Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConditionalAccessExpression) Then
							whenNotNull = DirectCast(whenNotNull, Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax).Expression
							If (whenNotNull IsNot Nothing) Then
								Continue While
							End If
							expressionSyntax = Nothing
						Else
							expressionSyntax = Nothing
						End If

				End Select
			End While
			Return expressionSyntax
		End Function

		<Extension>
		Public Function IsLambdaExpressionSyntax(ByVal this As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = this.Kind()
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement), True, False)
			Return flag
		End Function

		<Extension>
		Public Function QueryClauseKeywordOrRangeVariableIdentifier(ByVal syntax As SyntaxNode) As Microsoft.CodeAnalysis.SyntaxToken
			Dim identifier As Microsoft.CodeAnalysis.SyntaxToken
			Select Case syntax.Kind()
				Case SyntaxKind.CollectionRangeVariable
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax).Identifier.Identifier
					Exit Select
				Case SyntaxKind.ExpressionRangeVariable
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax).NameEquals.Identifier.Identifier
					Exit Select
				Case SyntaxKind.AggregationRangeVariable
				Case SyntaxKind.VariableNameEquals
				Case SyntaxKind.FunctionAggregation
				Case SyntaxKind.GroupAggregation
				Case SyntaxKind.JoinCondition
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.TryCastExpression Or SyntaxKind.SubtractExpression Or SyntaxKind.MultiplyExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.BinaryConditionalExpression Or SyntaxKind.QueryExpression Or SyntaxKind.CollectionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.FunctionAggregation Or SyntaxKind.GroupByClause Or SyntaxKind.JoinCondition Or SyntaxKind.OrderByClause
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.EnumBlock Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.IncompleteMember Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.FalseLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.PredefinedCastExpression Or SyntaxKind.SubtractExpression Or SyntaxKind.DivideExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsNotExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.TernaryConditionalExpression Or SyntaxKind.SingleLineSubLambdaExpression Or SyntaxKind.QueryExpression Or SyntaxKind.ExpressionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.GroupAggregation Or SyntaxKind.GroupByClause Or SyntaxKind.SimpleJoinClause Or SyntaxKind.OrderByClause
				Case SyntaxKind.AscendingOrdering
				Case SyntaxKind.DescendingOrdering
					Throw ExceptionUtilities.UnexpectedValue(syntax.Kind())
				Case SyntaxKind.FromClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax).FromKeyword
					Exit Select
				Case SyntaxKind.LetClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax).LetKeyword
					Exit Select
				Case SyntaxKind.AggregateClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax).AggregateKeyword
					Exit Select
				Case SyntaxKind.DistinctClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax).DistinctKeyword
					Exit Select
				Case SyntaxKind.WhereClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax).WhereKeyword
					Exit Select
				Case SyntaxKind.SkipWhileClause
				Case SyntaxKind.TakeWhileClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax).SkipOrTakeKeyword
					Exit Select
				Case SyntaxKind.SkipClause
				Case SyntaxKind.TakeClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax).SkipOrTakeKeyword
					Exit Select
				Case SyntaxKind.GroupByClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax).GroupKeyword
					Exit Select
				Case SyntaxKind.SimpleJoinClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax).JoinKeyword
					Exit Select
				Case SyntaxKind.GroupJoinClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax).GroupKeyword
					Exit Select
				Case SyntaxKind.OrderByClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax).OrderKeyword
					Exit Select
				Case SyntaxKind.SelectClause
					identifier = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax).SelectKeyword
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(syntax.Kind())
			End Select
			Return identifier
		End Function

		<Extension>
		Public Function WithAnnotations(Of TNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(ByVal node As TNode, ByVal ParamArray annotations As SyntaxAnnotation()) As TNode
			Return DirectCast(node.Green.SetAnnotations(annotations).CreateRed(), TNode)
		End Function
	End Module
End Namespace