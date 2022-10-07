Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class TypeArgumentInference
		Private Sub New()
			MyBase.New()
		End Sub

		Public Shared Function Infer(ByVal candidate As MethodSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal parameterToArgumentMap As ArrayBuilder(Of Integer), ByVal paramArrayItems As ArrayBuilder(Of Integer), ByVal delegateReturnType As TypeSymbol, ByVal delegateReturnTypeReferenceBoundNode As BoundNode, ByRef typeArguments As ImmutableArray(Of TypeSymbol), ByRef inferenceLevel As TypeArgumentInference.InferenceLevel, ByRef allFailedInferenceIsDueToObject As Boolean, ByRef someInferenceFailed As Boolean, ByRef inferenceErrorReasons As Microsoft.CodeAnalysis.VisualBasic.InferenceErrorReasons, <Out> ByRef inferredTypeByAssumption As BitVector, <Out> ByRef typeArgumentsLocation As ImmutableArray(Of SyntaxNodeOrToken), <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByRef diagnostic As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal inferTheseTypeParameters As BitVector = Nothing) As Boolean
			Return TypeArgumentInference.InferenceGraph.Infer(candidate, arguments, parameterToArgumentMap, paramArrayItems, delegateReturnType, delegateReturnTypeReferenceBoundNode, typeArguments, inferenceLevel, allFailedInferenceIsDueToObject, someInferenceFailed, inferenceErrorReasons, inferredTypeByAssumption, typeArgumentsLocation, asyncLambdaSubToFunctionMismatch, useSiteInfo, diagnostic, inferTheseTypeParameters)
		End Function

		Private Class ArgumentNode
			Inherits TypeArgumentInference.InferenceNode
			Public ReadOnly ParameterType As TypeSymbol

			Public ReadOnly Expression As BoundExpression

			Public ReadOnly Parameter As ParameterSymbol

			Public Sub New(ByVal graph As TypeArgumentInference.InferenceGraph, ByVal expression As BoundExpression, ByVal parameterType As TypeSymbol, ByVal parameter As ParameterSymbol)
				MyBase.New(graph, TypeArgumentInference.InferenceNodeType.ArgumentNode)
				Me.Expression = expression
				Me.ParameterType = parameterType
				Me.Parameter = parameter
			End Sub

			Public Overrides Function InferTypeAndPropagateHints() As Boolean
				Dim flag As Boolean
				Dim arrayLiteralTypeSymbol As TypeSymbol
				Dim enumerator As ArrayBuilder(Of TypeArgumentInference.InferenceNode).Enumerator = Me.IncomingEdges.GetEnumerator()
				While True
					If (enumerator.MoveNext()) Then
						Dim current As TypeArgumentInference.TypeParameterNode = DirectCast(enumerator.Current, TypeArgumentInference.TypeParameterNode)
						If (current.InferredType Is Nothing) Then
							Dim flag1 As Boolean = True
							If (Me.Expression.Kind = BoundKind.UnboundLambda AndAlso Me.ParameterType.IsDelegateType()) Then
								Dim delegateInvokeMethod As MethodSymbol = DirectCast(Me.ParameterType, NamedTypeSymbol).DelegateInvokeMethod
								If (delegateInvokeMethod IsNot Nothing AndAlso delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo Is Nothing) Then
									Dim expression As UnboundLambda = DirectCast(Me.Expression, UnboundLambda)
									Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = expression.Parameters
									Dim parameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = delegateInvokeMethod.Parameters
									Dim num As Integer = Math.Min(parameters.Length, parameterSymbols.Length) - 1
									Dim num1 As Integer = 0
									While num1 <= num
										Dim item As UnboundLambdaParameterSymbol = DirectCast(parameters(num1), UnboundLambdaParameterSymbol)
										Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameterSymbols(num1)
										If (item.Type IsNot Nothing OrElse Not parameterSymbol.Type.Equals(current.DeclaredTypeParam)) Then
											num1 = num1 + 1
										Else
											If (MyBase.Graph.Diagnostic Is Nothing) Then
												MyBase.Graph.Diagnostic = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Create(True, MyBase.Graph.UseSiteInfo.AccumulatesDependencies)
											End If
											If (MyBase.Graph.ObjectType Is Nothing) Then
												MyBase.Graph.ObjectType = expression.Binder.GetSpecialType(SpecialType.System_Object, item.IdentifierSyntax, MyBase.Graph.Diagnostic)
											End If
											current.RegisterInferredType(MyBase.Graph.ObjectType, item.TypeSyntax, current.InferredTypeByAssumption)
											expression.Binder.ReportLambdaParameterInferredToBeObject(item, MyBase.Graph.Diagnostic)
											flag1 = False
											Exit While
										End If
									End While
								End If
							End If
							If (flag1) Then
								Me.InferenceComplete = True
								flag = False
								Exit While
							End If
						End If
					Else
						Dim flag2 As Boolean = False
						Dim kind As BoundKind = Me.Expression.Kind
						If (kind > BoundKind.LateAddressOfOperator) Then
							If (kind <> BoundKind.UnboundLambda AndAlso kind <> BoundKind.QueryLambda AndAlso kind <> BoundKind.GroupTypeInferenceLambda) Then
								GoTo Label3
							End If
							MyBase.Graph.MarkInferenceLevel(TypeArgumentInference.InferenceLevel.Orcas)
							flag2 = MyBase.Graph.InferTypeArgumentsFromLambdaArgument(Me.Expression, Me.ParameterType, Me.Parameter)
							GoTo Label0
						ElseIf (kind = BoundKind.AddressOfOperator) Then
							flag2 = MyBase.Graph.InferTypeArgumentsFromAddressOfArgument(Me.Expression, Me.ParameterType, Me.Parameter)
							GoTo Label0
						Else
							If (kind <> BoundKind.LateAddressOfOperator) Then
								GoTo Label3
							End If
							MyBase.Graph.ReportNotFailedInferenceDueToObject()
							flag2 = True
							GoTo Label0
						End If
					Label3:
						If (Not Me.Expression.IsStrictNothingLiteral()) Then
							Dim requiredConversion As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any
							If (Me.Parameter IsNot Nothing AndAlso Me.Parameter.IsByRef AndAlso (Me.Expression.IsLValue OrElse Me.Expression.IsPropertySupportingAssignment())) Then
								requiredConversion = Conversions.CombineConversionRequirements(requiredConversion, Conversions.InvertConversionRequirement(requiredConversion))
							End If
							Dim numberOfCandidates As Boolean = False
							If (Me.Expression.Kind = BoundKind.ArrayLiteral) Then
								Dim boundArrayLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral = DirectCast(Me.Expression, Microsoft.CodeAnalysis.VisualBasic.BoundArrayLiteral)
								numberOfCandidates = boundArrayLiteral.NumberOfCandidates <> 1
								arrayLiteralTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol(boundArrayLiteral)
							ElseIf (Me.Expression.Kind <> BoundKind.TupleLiteral) Then
								arrayLiteralTypeSymbol = Me.Expression.Type
							Else
								arrayLiteralTypeSymbol = DirectCast(Me.Expression, BoundTupleLiteral).InferredType
							End If
							flag2 = MyBase.Graph.InferTypeArgumentsFromArgument(Me.Expression.Syntax, arrayLiteralTypeSymbol, numberOfCandidates, Me.ParameterType, Me.Parameter, TypeArgumentInference.MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, requiredConversion)
						Else
							Me.InferenceComplete = True
							flag = False
							Exit While
						End If
					Label0:
						If (Not flag2) Then
							MyBase.Graph.MarkInferenceFailure()
							If (Me.Expression.Type Is Nothing OrElse Not Me.Expression.Type.IsObjectType()) Then
								MyBase.Graph.ReportNotFailedInferenceDueToObject()
							End If
						End If
						Me.InferenceComplete = True
						flag = False
						Exit While
					End If
				End While
				Return flag
			End Function
		End Class

		Private Enum DFSColor As Byte
			None
			Grey
			Black
		End Enum

		Private Class DominantTypeDataTypeInference
			Inherits DominantTypeData
			Public ByAssumption As Boolean

			Public Parameter As ParameterSymbol

			Public InferredFromObject As Boolean

			Public TypeParameter As TypeParameterSymbol

			Public ArgumentLocation As SyntaxNode

			Public Sub New()
				MyBase.New()
			End Sub
		End Class

		Private Class Graph(Of TGraphNode As TypeArgumentInference.GraphNode(Of TGraphNode))
			Public ReadOnly Vertices As ArrayBuilder(Of TGraphNode)

			Public Sub New()
				MyBase.New()
				Me.Vertices = New ArrayBuilder(Of TGraphNode)()
			End Sub

			Public Sub AddEdge(ByVal source As TGraphNode, ByVal target As TGraphNode)
				Me.AddNode(source)
				Me.AddNode(target)
				source.OutgoingEdges.Add(target)
				target.IncomingEdges.Add(source)
			End Sub

			Public Sub AddNode(ByVal node As TGraphNode)
				If (Not node.IsAddedToVertices) Then
					Me.Vertices.Add(node)
					node.IsAddedToVertices = True
				End If
			End Sub

			Public Function BuildStronglyConnectedComponents() As TypeArgumentInference.Graph(Of TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode))
				Dim graph As TypeArgumentInference.Graph(Of TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode)) = New TypeArgumentInference.Graph(Of TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode))()
				Dim instance As ArrayBuilder(Of TGraphNode) = ArrayBuilder(Of TGraphNode).GetInstance()
				Me.Dfs(instance)
				Dim enumerator As ArrayBuilder(Of TGraphNode).Enumerator = instance.GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.AlgorithmData = New TypeArgumentInference.GraphAlgorithmData(Of TGraphNode)()
				End While
				Dim enumerator1 As ArrayBuilder(Of TGraphNode).Enumerator = instance.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As TGraphNode = enumerator1.Current
					If (current.AlgorithmData.Color <> TypeArgumentInference.DFSColor.None) Then
						Continue While
					End If
					Dim stronglyConnectedComponent As TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode) = New TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode)(graph)
					Me.CollectSccChildren(current, stronglyConnectedComponent)
					graph.AddNode(stronglyConnectedComponent)
				End While
				instance.Free()
				instance = Nothing
				Dim enumerator2 As ArrayBuilder(Of TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode)).Enumerator = graph.Vertices.GetEnumerator()
				While enumerator2.MoveNext()
					Dim current1 As TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode) = enumerator2.Current
					Dim enumerator3 As ArrayBuilder(Of TGraphNode).Enumerator = current1.ChildNodes.GetEnumerator()
					While enumerator3.MoveNext()
						Dim enumerator4 As ArrayBuilder(Of TGraphNode).Enumerator = enumerator3.Current.OutgoingEdges.GetEnumerator()
						While enumerator4.MoveNext()
							Dim stronglyConnectedComponent1 As TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode) = enumerator4.Current.AlgorithmData.StronglyConnectedComponent
							If (current1 = stronglyConnectedComponent1) Then
								Continue While
							End If
							graph.AddEdge(current1, stronglyConnectedComponent1)
						End While
					End While
				End While
				Return graph
			End Function

			Private Sub CollectSccChildren(ByVal node As TGraphNode, ByVal sccNode As TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode))
				node.AlgorithmData.Color = TypeArgumentInference.DFSColor.Grey
				Dim enumerator As ArrayBuilder(Of TGraphNode).Enumerator = node.IncomingEdges.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TGraphNode = enumerator.Current
					If (current.AlgorithmData.Color <> TypeArgumentInference.DFSColor.None) Then
						Continue While
					End If
					Me.CollectSccChildren(current, sccNode)
				End While
				node.AlgorithmData.Color = TypeArgumentInference.DFSColor.Black
				sccNode.ChildNodes.Add(node)
				node.AlgorithmData.StronglyConnectedComponent = sccNode
			End Sub

			Private Function Contains(ByVal node As TGraphNode) As Boolean
				If (node.Graph <> Me) Then
					Return False
				End If
				Return node.IsAddedToVertices
			End Function

			Private Sub Dfs(ByVal resultList As ArrayBuilder(Of TGraphNode))
				Dim enumerator As ArrayBuilder(Of TGraphNode).Enumerator = Me.Vertices.GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.AlgorithmData = New TypeArgumentInference.GraphAlgorithmData(Of TGraphNode)()
				End While
				Dim count As Integer = resultList.Count
				resultList.AddMany(Nothing, Me.Vertices.Count)
				Dim num As Integer = resultList.Count - 1
				Dim enumerator1 As ArrayBuilder(Of TGraphNode).Enumerator = Me.Vertices.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As TGraphNode = enumerator1.Current
					If (current.AlgorithmData.Color <> TypeArgumentInference.DFSColor.None) Then
						Continue While
					End If
					Me.DfsVisit(current, resultList, num)
				End While
			End Sub

			Private Sub DfsVisit(ByVal node As TGraphNode, ByVal resultList As ArrayBuilder(Of TGraphNode), ByRef insertAt As Integer)
				node.AlgorithmData.Color = TypeArgumentInference.DFSColor.Grey
				Dim enumerator As ArrayBuilder(Of TGraphNode).Enumerator = node.OutgoingEdges.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TGraphNode = enumerator.Current
					If (current.AlgorithmData.Color <> TypeArgumentInference.DFSColor.None) Then
						Continue While
					End If
					Me.DfsVisit(current, resultList, insertAt)
				End While
				node.AlgorithmData.Color = TypeArgumentInference.DFSColor.Black
				resultList(insertAt) = node
				insertAt = insertAt - 1
			End Sub

			Private Shared Sub Remove(ByVal list As ArrayBuilder(Of TGraphNode), ByVal toRemove As TGraphNode)
				Dim count As Integer = list.Count - 1
				Dim num As Integer = count
				Dim item As Integer = 0
				Do
					If (list(item) = toRemove) Then
						If (item < count) Then
							list(item) = list(count)
						End If
						list.Clip(count)
						Return
					End If
					item = item + 1
				Loop While item <= num
				Throw ExceptionUtilities.Unreachable
			End Sub

			Public Sub RemoveEdge(ByVal source As TGraphNode, ByVal target As TGraphNode)
				TypeArgumentInference.Graph(Of TGraphNode).Remove(source.OutgoingEdges, target)
				TypeArgumentInference.Graph(Of TGraphNode).Remove(target.IncomingEdges, source)
			End Sub

			Public Sub TopoSort(ByVal resultList As ArrayBuilder(Of TGraphNode))
				Me.Dfs(resultList)
			End Sub
		End Class

		Private Structure GraphAlgorithmData(Of TGraphNode As TypeArgumentInference.GraphNode(Of TGraphNode))
			Public Color As TypeArgumentInference.DFSColor

			Public StronglyConnectedComponent As TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode)
		End Structure

		Private Class GraphNode(Of TGraphNode As TypeArgumentInference.GraphNode(Of TGraphNode))
			Public ReadOnly Graph As TypeArgumentInference.Graph(Of TGraphNode)

			Public IsAddedToVertices As Boolean

			Public ReadOnly IncomingEdges As ArrayBuilder(Of TGraphNode)

			Public ReadOnly OutgoingEdges As ArrayBuilder(Of TGraphNode)

			Public AlgorithmData As TypeArgumentInference.GraphAlgorithmData(Of TGraphNode)

			Protected Sub New(ByVal graph As TypeArgumentInference.Graph(Of TGraphNode))
				MyBase.New()
				Me.Graph = graph
				Me.IsAddedToVertices = False
				Me.IncomingEdges = New ArrayBuilder(Of TGraphNode)()
				Me.OutgoingEdges = New ArrayBuilder(Of TGraphNode)()
			End Sub
		End Class

		Private Class InferenceGraph
			Inherits TypeArgumentInference.Graph(Of TypeArgumentInference.InferenceNode)
			Public Diagnostic As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

			Public ObjectType As NamedTypeSymbol

			Public ReadOnly Candidate As MethodSymbol

			Public ReadOnly Arguments As ImmutableArray(Of BoundExpression)

			Public ReadOnly ParameterToArgumentMap As ArrayBuilder(Of Integer)

			Public ReadOnly ParamArrayItems As ArrayBuilder(Of Integer)

			Public ReadOnly DelegateReturnType As TypeSymbol

			Public ReadOnly DelegateReturnTypeReferenceBoundNode As BoundNode

			Public UseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)

			Private _someInferenceFailed As Boolean

			Private _inferenceErrorReasons As InferenceErrorReasons

			Private _allFailedInferenceIsDueToObject As Boolean

			Private _typeInferenceLevel As TypeArgumentInference.InferenceLevel

			Private _asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression)

			Private ReadOnly _typeParameterNodes As ImmutableArray(Of TypeArgumentInference.TypeParameterNode)

			Private ReadOnly _verifyingAssertions As Boolean

			Public ReadOnly Property AllFailedInferenceIsDueToObject As Boolean
				Get
					Return Me._allFailedInferenceIsDueToObject
				End Get
			End Property

			Public ReadOnly Property InferenceErrorReasons As InferenceErrorReasons
				Get
					Return Me._inferenceErrorReasons
				End Get
			End Property

			Public ReadOnly Property SomeInferenceHasFailed As Boolean
				Get
					Return Me._someInferenceFailed
				End Get
			End Property

			Public ReadOnly Property TypeInferenceLevel As TypeArgumentInference.InferenceLevel
				Get
					Return Me._typeInferenceLevel
				End Get
			End Property

			Private Sub New(ByVal diagnostic As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal candidate As MethodSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal parameterToArgumentMap As ArrayBuilder(Of Integer), ByVal paramArrayItems As ArrayBuilder(Of Integer), ByVal delegateReturnType As TypeSymbol, ByVal delegateReturnTypeReferenceBoundNode As BoundNode, ByVal asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), ByVal useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
				MyBase.New()
				Me._allFailedInferenceIsDueToObject = True
				Me._typeInferenceLevel = TypeArgumentInference.InferenceLevel.None
				Me.Diagnostic = diagnostic
				Me.Candidate = candidate
				Me.Arguments = arguments
				Me.ParameterToArgumentMap = parameterToArgumentMap
				Me.ParamArrayItems = paramArrayItems
				Me.DelegateReturnType = delegateReturnType
				Me.DelegateReturnTypeReferenceBoundNode = delegateReturnTypeReferenceBoundNode
				Me._asyncLambdaSubToFunctionMismatch = asyncLambdaSubToFunctionMismatch
				Me.UseSiteInfo = useSiteInfo
				Dim arity As Integer = candidate.Arity
				Dim typeParameterNode(arity - 1 + 1 - 1) As TypeArgumentInference.TypeParameterNode
				Dim num As Integer = arity - 1
				Dim num1 As Integer = 0
				Do
					Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = candidate.TypeParameters
					typeParameterNode(num1) = New TypeArgumentInference.TypeParameterNode(Me, typeParameters(num1))
					num1 = num1 + 1
				Loop While num1 <= num
				Me._typeParameterNodes = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeArgumentInference.TypeParameterNode)(typeParameterNode)
			End Sub

			Private Sub AddAddressOfToGraph(ByVal argNode As TypeArgumentInference.ArgumentNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				Me.AddAddressOfToGraph(argNode.ParameterType, argNode, binder)
			End Sub

			Private Sub AddAddressOfToGraph(ByVal parameterType As TypeSymbol, ByVal argNode As TypeArgumentInference.ArgumentNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				Dim typeParameterNodes As ImmutableArray(Of TypeArgumentInference.TypeParameterNode)
				If (parameterType.IsTypeParameter()) Then
					typeParameterNodes = Me._typeParameterNodes
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(typeParameterNodes.Length)
					Me.AddTypeToGraph(parameterType, argNode, True, bitVector)
					Return
				End If
				If (parameterType.IsDelegateType()) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(parameterType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Dim delegateInvokeMethod As MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
					If (delegateInvokeMethod IsNot Nothing AndAlso delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo Is Nothing AndAlso namedTypeSymbol.IsGenericType) Then
						typeParameterNodes = Me._typeParameterNodes
						Dim bitVector1 As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(typeParameterNodes.Length)
						Me.AddTypeToGraph(delegateInvokeMethod.ReturnType, argNode, True, bitVector1)
						bitVector1.Clear()
						Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = delegateInvokeMethod.Parameters.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As ParameterSymbol = enumerator.Current
							Me.AddTypeToGraph(current.Type, argNode, False, bitVector1)
						End While
						Return
					End If
				ElseIf (TypeSymbol.Equals(parameterType.OriginalDefinition, binder.Compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T), TypeCompareKind.ConsiderEverything)) Then
					Me.AddAddressOfToGraph(DirectCast(parameterType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).TypeArgumentWithDefinitionUseSiteDiagnostics(0, Me.UseSiteInfo), argNode, binder)
				End If
			End Sub

			Private Sub AddDelegateReturnTypeToGraph()
				If (Me.DelegateReturnType IsNot Nothing AndAlso Not Me.DelegateReturnType.IsVoidType()) Then
					Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder(Me.DelegateReturnTypeReferenceBoundNode.Syntax, Me.DelegateReturnType)
					Dim argumentNode As TypeArgumentInference.ArgumentNode = New TypeArgumentInference.ArgumentNode(Me, boundRValuePlaceholder, Me.Candidate.ReturnType, Nothing)
					Dim enumerator As ArrayBuilder(Of TypeArgumentInference.InferenceNode).Enumerator = Me.Vertices.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypeArgumentInference.InferenceNode = enumerator.Current
						If (current.NodeType <> TypeArgumentInference.InferenceNodeType.TypeParameterNode) Then
							Continue While
						End If
						MyBase.AddEdge(current, argumentNode)
					End While
					Me.AddTypeToGraph(argumentNode, True)
				End If
			End Sub

			Private Sub AddLambdaToGraph(ByVal argNode As TypeArgumentInference.ArgumentNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				Me.AddLambdaToGraph(argNode.ParameterType, argNode, binder)
			End Sub

			Private Sub AddLambdaToGraph(ByVal parameterType As TypeSymbol, ByVal argNode As TypeArgumentInference.ArgumentNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				Dim typeParameterNodes As ImmutableArray(Of TypeArgumentInference.TypeParameterNode)
				Dim parameters As ImmutableArray(Of ParameterSymbol)
				If (parameterType.IsTypeParameter()) Then
					typeParameterNodes = Me._typeParameterNodes
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(typeParameterNodes.Length)
					Me.AddTypeToGraph(parameterType, argNode, True, bitVector)
					Return
				End If
				If (parameterType.IsDelegateType()) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(parameterType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Dim delegateInvokeMethod As MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
					If (delegateInvokeMethod IsNot Nothing AndAlso delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo Is Nothing AndAlso namedTypeSymbol.IsGenericType) Then
						Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = delegateInvokeMethod.Parameters
						Dim kind As BoundKind = argNode.Expression.Kind
						If (kind = BoundKind.UnboundLambda) Then
							parameters = DirectCast(argNode.Expression, UnboundLambda).Parameters
						ElseIf (kind = BoundKind.QueryLambda) Then
							parameters = DirectCast(argNode.Expression, BoundQueryLambda).LambdaSymbol.Parameters
						Else
							If (kind <> BoundKind.GroupTypeInferenceLambda) Then
								Throw ExceptionUtilities.UnexpectedValue(argNode.Expression.Kind)
							End If
							parameters = DirectCast(argNode.Expression, GroupTypeInferenceLambda).Parameters
						End If
						typeParameterNodes = Me._typeParameterNodes
						Dim bitVector1 As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(typeParameterNodes.Length)
						Dim num As Integer = Math.Min(parameterSymbols.Length, parameters.Length) - 1
						Dim num1 As Integer = 0
						Do
							If (parameters(num1).Type IsNot Nothing) Then
								Me.InferTypeArgumentsFromArgument(argNode.Expression.Syntax, parameters(num1).Type, False, parameterSymbols(num1).Type, parameterSymbols(num1), TypeArgumentInference.MatchGenericArgumentToParameter.MatchArgumentToBaseOfGenericParameter, RequiredConversion.Any)
							End If
							Me.AddTypeToGraph(parameterSymbols(num1).Type, argNode, False, bitVector1)
							num1 = num1 + 1
						Loop While num1 <= num
						bitVector1.Clear()
						Me.AddTypeToGraph(delegateInvokeMethod.ReturnType, argNode, True, bitVector1)
						Return
					End If
				ElseIf (TypeSymbol.Equals(parameterType.OriginalDefinition, binder.Compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T), TypeCompareKind.ConsiderEverything)) Then
					Me.AddLambdaToGraph(DirectCast(parameterType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).TypeArgumentWithDefinitionUseSiteDiagnostics(0, Me.UseSiteInfo), argNode, binder)
				End If
			End Sub

			Private Sub AddTupleLiteralToGraph(ByVal argNode As TypeArgumentInference.ArgumentNode)
				Me.AddTupleLiteralToGraph(argNode.ParameterType, argNode)
			End Sub

			Private Sub AddTupleLiteralToGraph(ByVal parameterType As TypeSymbol, ByVal argNode As TypeArgumentInference.ArgumentNode)
				Dim arguments As ImmutableArray(Of BoundExpression) = DirectCast(argNode.Expression, BoundTupleLiteral).Arguments
				If (Not parameterType.IsTupleOrCompatibleWithTupleOfCardinality(arguments.Length)) Then
					Me.AddTypeToGraph(argNode, True)
					Return
				End If
				Dim elementTypesOfTupleOrCompatible As ImmutableArray(Of TypeSymbol) = parameterType.GetElementTypesOfTupleOrCompatible()
				Dim length As Integer = arguments.Length - 1
				For i As Integer = 0 To length
					Me.RegisterArgument(arguments(i), elementTypesOfTupleOrCompatible(i), argNode.Parameter)
				Next

			End Sub

			Private Sub AddTypeToGraph(ByVal node As TypeArgumentInference.ArgumentNode, ByVal isOutgoingEdge As Boolean)
				Dim parameterType As TypeSymbol = node.ParameterType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Create(Me._typeParameterNodes.Length)
				Me.AddTypeToGraph(parameterType, node, isOutgoingEdge, bitVector)
			End Sub

			Private Sub AddTypeToGraph(ByVal parameterType As TypeSymbol, ByVal argNode As TypeArgumentInference.ArgumentNode, ByVal isOutgoingEdge As Boolean, ByRef haveSeenTypeParameters As BitVector)
				Dim kind As SymbolKind = parameterType.Kind
				If (kind = SymbolKind.ArrayType) Then
					Me.AddTypeToGraph(DirectCast(parameterType, ArrayTypeSymbol).ElementType, argNode, isOutgoingEdge, haveSeenTypeParameters)
					Return
				End If
				If (kind = SymbolKind.NamedType) Then
					Dim containingType As NamedTypeSymbol = DirectCast(parameterType, NamedTypeSymbol)
					Dim typeSymbols As ImmutableArray(Of TypeSymbol) = New ImmutableArray(Of TypeSymbol)()
					If (containingType.TryGetElementTypesIfTupleOrCompatible(typeSymbols)) Then
						Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeSymbols.GetEnumerator()
						While enumerator.MoveNext()
							Me.AddTypeToGraph(enumerator.Current, argNode, isOutgoingEdge, haveSeenTypeParameters)
						End While
						Return
					End If
					Do
						Dim typeSymbols1 As ImmutableArray(Of TypeSymbol) = containingType.TypeArgumentsWithDefinitionUseSiteDiagnostics(Me.UseSiteInfo)
						Dim enumerator1 As ImmutableArray(Of TypeSymbol).Enumerator = typeSymbols1.GetEnumerator()
						While enumerator1.MoveNext()
							Me.AddTypeToGraph(enumerator1.Current, argNode, isOutgoingEdge, haveSeenTypeParameters)
						End While
						containingType = containingType.ContainingType
					Loop While containingType IsNot Nothing
				ElseIf (kind = SymbolKind.TypeParameter) Then
					Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(parameterType, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
					Dim typeParameterNode As TypeArgumentInference.TypeParameterNode = Me.FindTypeParameterNode(typeParameterSymbol)
					If (typeParameterNode IsNot Nothing AndAlso Not haveSeenTypeParameters(typeParameterSymbol.Ordinal)) Then
						If (typeParameterNode.Parameter Is Nothing) Then
							typeParameterNode.SetParameter(argNode.Parameter)
						End If
						If (Not isOutgoingEdge) Then
							MyBase.AddEdge(typeParameterNode, argNode)
						Else
							MyBase.AddEdge(argNode, typeParameterNode)
						End If
						haveSeenTypeParameters(typeParameterSymbol.Ordinal) = True
						Return
					End If
				End If
			End Sub

			Private Shared Function ArgumentTypePossiblyMatchesParamarrayShape(ByVal argument As BoundExpression, ByVal paramType As TypeSymbol) As Boolean
				Dim flag As Boolean
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol
				Dim arrayTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol
				Dim type As TypeSymbol = argument.Type
				Dim flag1 As Boolean = False
				If (type Is Nothing) Then
					If (argument.Kind = BoundKind.ArrayLiteral) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				End If
				While paramType.IsArrayType()
					If (type.IsArrayType()) Then
						arrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
						arrayTypeSymbol1 = DirectCast(paramType, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
						If (arrayTypeSymbol.Rank <> arrayTypeSymbol1.Rank OrElse Not flag1 AndAlso arrayTypeSymbol.IsSZArray <> arrayTypeSymbol1.IsSZArray) Then
							flag = False
							Return flag
						Else
							flag1 = False
							type = arrayTypeSymbol.ElementType
							paramType = arrayTypeSymbol1.ElementType
						End If
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
				Return flag
			Label1:
				flag1 = True
				type = DirectCast(argument, BoundArrayLiteral).InferredType
				While paramType.IsArrayType()
					If (type.IsArrayType()) Then
						arrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
						arrayTypeSymbol1 = DirectCast(paramType, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
						If (arrayTypeSymbol.Rank <> arrayTypeSymbol1.Rank OrElse Not flag1 AndAlso arrayTypeSymbol.IsSZArray <> arrayTypeSymbol1.IsSZArray) Then
							flag = False
							Return flag
						Else
							flag1 = False
							type = arrayTypeSymbol.ElementType
							paramType = arrayTypeSymbol1.ElementType
						End If
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
				Return flag
			End Function

			Public Function ConstructParameterTypeIfNeeded(ByVal parameterType As TypeSymbol) As TypeSymbol
				Dim item As TypeSymbol
				Dim candidate As MethodSymbol = Me.Candidate
				Dim instance As ArrayBuilder(Of TypeWithModifiers) = ArrayBuilder(Of TypeWithModifiers).GetInstance(Me._typeParameterNodes.Length)
				Dim length As Integer = Me._typeParameterNodes.Length - 1
				Dim num As Integer = 0
				Do
					Dim typeParameterNode As TypeArgumentInference.TypeParameterNode = Me._typeParameterNodes(num)
					If (typeParameterNode Is Nothing OrElse typeParameterNode.CandidateInferredType Is Nothing) Then
						item = candidate.TypeParameters(num)
					Else
						item = typeParameterNode.CandidateInferredType
					End If
					instance.Add(New TypeWithModifiers(item))
					num = num + 1
				Loop While num <= length
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.CreateAdditionalMethodTypeParameterSubstitution(candidate.ConstructedFrom, instance.ToImmutableAndFree())
				Return parameterType.InternalSubstituteTypeParameters(typeSubstitution).Type
			End Function

			Private Function FindMatchingBase(ByRef baseSearchType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByRef fixedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Boolean
				Dim flag As Boolean
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				If (fixedType.Kind = SymbolKind.NamedType) Then
					namedTypeSymbol = DirectCast(fixedType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Else
					namedTypeSymbol = Nothing
				End If
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol
				If (namedTypeSymbol1 Is Nothing OrElse Not namedTypeSymbol1.IsGenericType) Then
					flag = False
				Else
					Dim typeKind As Microsoft.CodeAnalysis.TypeKind = fixedType.TypeKind
					If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Class] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.[Interface]) Then
						Dim kind As SymbolKind = baseSearchType.Kind
						If (kind <> SymbolKind.NamedType AndAlso kind <> SymbolKind.TypeParameter AndAlso (kind <> SymbolKind.ArrayType OrElse Not DirectCast(baseSearchType, ArrayTypeSymbol).IsSZArray)) Then
							flag = False
						ElseIf (Not baseSearchType.IsSameTypeIgnoringAll(fixedType)) Then
							Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
							If (typeKind <> Microsoft.CodeAnalysis.TypeKind.[Class]) Then
								Me.FindMatchingBaseInterface(baseSearchType, fixedType, typeSymbol)
							Else
								Me.FindMatchingBaseClass(baseSearchType, fixedType, typeSymbol)
							End If
							If (typeSymbol IsNot Nothing) Then
								baseSearchType = typeSymbol
								flag = True
							Else
								flag = False
							End If
						Else
							flag = False
						End If
					Else
						flag = False
					End If
				End If
				Return flag
			End Function

			Private Function FindMatchingBaseClass(ByVal derivedType As TypeSymbol, ByVal baseClass As TypeSymbol, ByRef match As TypeSymbol) As Boolean
				Dim flag As Boolean
				If (derivedType.Kind <> SymbolKind.TypeParameter) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = derivedType.BaseTypeWithDefinitionUseSiteDiagnostics(Me.UseSiteInfo)
					While namedTypeSymbol IsNot Nothing
						If (Not namedTypeSymbol.OriginalDefinition.IsSameTypeIgnoringAll(baseClass.OriginalDefinition)) Then
							namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(Me.UseSiteInfo)
						Else
							If (TypeArgumentInference.InferenceGraph.SetMatchIfNothingOrEqual(namedTypeSymbol, match)) Then
								Exit While
							End If
							flag = False
							Return flag
						End If
					End While
				Else
					Dim typeSymbols As ImmutableArray(Of TypeSymbol) = DirectCast(derivedType, TypeParameterSymbol).ConstraintTypesWithDefinitionUseSiteDiagnostics(Me.UseSiteInfo)
					Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypeSymbol = enumerator.Current
						If (Not current.OriginalDefinition.IsSameTypeIgnoringAll(baseClass.OriginalDefinition) OrElse TypeArgumentInference.InferenceGraph.SetMatchIfNothingOrEqual(current, match)) Then
							If (Me.FindMatchingBaseClass(current, baseClass, match)) Then
								Continue While
							End If
							flag = False
							Return flag
						Else
							flag = False
							Return flag
						End If
					End While
				End If
				flag = True
				Return flag
			End Function

			Private Function FindMatchingBaseInterface(ByVal derivedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal baseInterface As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByRef match As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Boolean
				Dim flag As Boolean
				Dim current As NamedTypeSymbol
				If (derivedType.Kind <> SymbolKind.TypeParameter) Then
					Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = derivedType.AllInterfacesWithDefinitionUseSiteDiagnostics(Me.UseSiteInfo)
					Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = namedTypeSymbols.GetEnumerator()
					Do
						If (Not enumerator.MoveNext()) Then
							flag = True
							Return flag
						End If
						current = enumerator.Current
					Loop While Not current.OriginalDefinition.IsSameTypeIgnoringAll(baseInterface.OriginalDefinition) OrElse TypeArgumentInference.InferenceGraph.SetMatchIfNothingOrEqual(current, match)
					flag = False
					Return flag
				Else
					Dim typeSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = DirectCast(derivedType, TypeParameterSymbol).ConstraintTypesWithDefinitionUseSiteDiagnostics(Me.UseSiteInfo)
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).Enumerator = typeSymbols.GetEnumerator()
					While enumerator1.MoveNext()
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator1.Current
						If (Not typeSymbol.OriginalDefinition.IsSameTypeIgnoringAll(baseInterface.OriginalDefinition) OrElse TypeArgumentInference.InferenceGraph.SetMatchIfNothingOrEqual(typeSymbol, match)) Then
							If (Me.FindMatchingBaseInterface(typeSymbol, baseInterface, match)) Then
								Continue While
							End If
							flag = False
							Return flag
						Else
							flag = False
							Return flag
						End If
					End While
				End If
				flag = True
				Return flag
			End Function

			Private Function FindTypeParameterNode(ByVal typeParameter As TypeParameterSymbol) As TypeArgumentInference.TypeParameterNode
				Dim item As TypeArgumentInference.TypeParameterNode
				Dim ordinal As Integer = typeParameter.Ordinal
				If (ordinal >= Me._typeParameterNodes.Length OrElse Me._typeParameterNodes(ordinal) Is Nothing OrElse Not typeParameter.Equals(Me._typeParameterNodes(ordinal).DeclaredTypeParam)) Then
					item = Nothing
				Else
					item = Me._typeParameterNodes(ordinal)
				End If
				Return item
			End Function

			Public Shared Function Infer(ByVal candidate As MethodSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal parameterToArgumentMap As ArrayBuilder(Of Integer), ByVal paramArrayItems As ArrayBuilder(Of Integer), ByVal delegateReturnType As TypeSymbol, ByVal delegateReturnTypeReferenceBoundNode As BoundNode, ByRef typeArguments As ImmutableArray(Of TypeSymbol), ByRef inferenceLevel As TypeArgumentInference.InferenceLevel, ByRef allFailedInferenceIsDueToObject As Boolean, ByRef someInferenceFailed As Boolean, ByRef inferenceErrorReasons As Microsoft.CodeAnalysis.VisualBasic.InferenceErrorReasons, <Out> ByRef inferredTypeByAssumption As BitVector, <Out> ByRef typeArgumentsLocation As ImmutableArray(Of SyntaxNodeOrToken), <InAttribute> <Out> ByRef asyncLambdaSubToFunctionMismatch As HashSet(Of BoundExpression), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), ByRef diagnostic As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal inferTheseTypeParameters As BitVector) As Boolean
				Dim flag As Boolean
				Dim inferenceGraph As TypeArgumentInference.InferenceGraph = New TypeArgumentInference.InferenceGraph(diagnostic, candidate, arguments, parameterToArgumentMap, paramArrayItems, delegateReturnType, delegateReturnTypeReferenceBoundNode, asyncLambdaSubToFunctionMismatch, useSiteInfo)
				inferenceGraph.PopulateGraph()
				Dim instance As ArrayBuilder(Of TypeArgumentInference.StronglyConnectedComponent(Of TypeArgumentInference.InferenceNode)) = ArrayBuilder(Of TypeArgumentInference.StronglyConnectedComponent(Of TypeArgumentInference.InferenceNode)).GetInstance()
				Do
					flag = False
					Dim graph As TypeArgumentInference.Graph(Of TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode)) = inferenceGraph.BuildStronglyConnectedComponents()
					instance.Clear()
					graph.TopoSort(instance)
					Dim enumerator As ArrayBuilder(Of TypeArgumentInference.StronglyConnectedComponent(Of TypeArgumentInference.InferenceNode)).Enumerator = instance.GetEnumerator()
					Do
					Label1:
						If (Not enumerator.MoveNext()) Then
							GoTo Label0
						End If
						Dim childNodes As ArrayBuilder(Of TypeArgumentInference.InferenceNode) = enumerator.Current.ChildNodes
						If (childNodes.Count <> 1) Then
							Dim flag1 As Boolean = False
							Dim enumerator1 As ArrayBuilder(Of TypeArgumentInference.InferenceNode).Enumerator = childNodes.GetEnumerator()
							While enumerator1.MoveNext()
								Dim current As TypeArgumentInference.InferenceNode = enumerator1.Current
								If (current.NodeType <> TypeArgumentInference.InferenceNodeType.TypeParameterNode OrElse DirectCast(current, TypeArgumentInference.TypeParameterNode).InferenceTypeCollection.GetTypeDataList().Count <= 0) Then
									Continue While
								End If
								If (current.InferTypeAndPropagateHints()) Then
									flag = True
								End If
								flag1 = True
							End While
							If (flag1) Then
								Continue Do
							End If
							Dim enumerator2 As ArrayBuilder(Of TypeArgumentInference.InferenceNode).Enumerator = childNodes.GetEnumerator()
							While enumerator2.MoveNext()
								Dim inferenceNode As TypeArgumentInference.InferenceNode = enumerator2.Current
								If (inferenceNode.NodeType <> TypeArgumentInference.InferenceNodeType.TypeParameterNode OrElse Not inferenceNode.InferTypeAndPropagateHints()) Then
									Continue While
								End If
								flag = True
							End While
						ElseIf (childNodes(0).InferTypeAndPropagateHints()) Then
							Throw ExceptionUtilities.Unreachable
						Else
							GoTo Label1
						End If
					Loop While Not flag
				Label0:
				Loop While flag
				instance.Free()
				Dim someInferenceHasFailed As Boolean = Not inferenceGraph.SomeInferenceHasFailed
				someInferenceFailed = inferenceGraph.SomeInferenceHasFailed
				allFailedInferenceIsDueToObject = inferenceGraph.AllFailedInferenceIsDueToObject
				inferenceErrorReasons = inferenceGraph.InferenceErrorReasons
				If (Not someInferenceFailed OrElse delegateReturnType IsNot Nothing) Then
					allFailedInferenceIsDueToObject = False
				End If
				Dim arity As Integer = candidate.Arity
				Dim typeSymbolArray(arity - 1 + 1 - 1) As TypeSymbol
				Dim inferredFromLocation(arity - 1 + 1 - 1) As SyntaxNodeOrToken
				Dim num As Integer = arity - 1
				Dim num1 As Integer = 0
				Do
					Dim item As TypeArgumentInference.TypeParameterNode = inferenceGraph._typeParameterNodes(num1)
					Dim inferredType As TypeSymbol = item.InferredType
					If (inferredType Is Nothing AndAlso (inferTheseTypeParameters.IsNull OrElse inferTheseTypeParameters(num1))) Then
						someInferenceHasFailed = False
					End If
					If (item.InferredTypeByAssumption) Then
						If (inferredTypeByAssumption.IsNull) Then
							inferredTypeByAssumption = BitVector.Create(arity)
						End If
						inferredTypeByAssumption(num1) = True
					End If
					typeSymbolArray(num1) = inferredType
					inferredFromLocation(num1) = item.InferredFromLocation
					num1 = num1 + 1
				Loop While num1 <= num
				typeArguments = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(typeSymbolArray)
				typeArgumentsLocation = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of SyntaxNodeOrToken)(inferredFromLocation)
				inferenceLevel = inferenceGraph._typeInferenceLevel
				diagnostic = inferenceGraph.Diagnostic
				asyncLambdaSubToFunctionMismatch = inferenceGraph._asyncLambdaSubToFunctionMismatch
				useSiteInfo = inferenceGraph.UseSiteInfo
				Return someInferenceHasFailed
			End Function

			Public Function InferTypeArgumentsFromAddressOfArgument(ByVal argument As BoundExpression, ByVal parameterType As TypeSymbol, ByVal param As ParameterSymbol) As Boolean
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.TypeArgumentInference/InferenceGraph::InferTypeArgumentsFromAddressOfArgument(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean InferTypeArgumentsFromAddressOfArgument(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function

			Friend Function InferTypeArgumentsFromArgument(ByVal argumentLocation As SyntaxNode, ByVal argumentType As TypeSymbol, ByVal argumentTypeByAssumption As Boolean, ByVal parameterType As TypeSymbol, ByVal param As ParameterSymbol, ByVal digThroughToBasesAndImplements As TypeArgumentInference.MatchGenericArgumentToParameter, ByVal inferenceRestrictions As RequiredConversion) As Boolean
				Dim flag As Boolean
				If (Not Me.RefersToGenericParameterToInferArgumentFor(parameterType)) Then
					flag = True
				ElseIf (Me.InferTypeArgumentsFromArgumentDirectly(argumentLocation, argumentType, argumentTypeByAssumption, parameterType, param, digThroughToBasesAndImplements, inferenceRestrictions)) Then
					flag = True
				ElseIf (parameterType.IsTypeParameter()) Then
					flag = False
				ElseIf (digThroughToBasesAndImplements <> TypeArgumentInference.MatchGenericArgumentToParameter.MatchGenericArgumentToParameterExactly) Then
					If (argumentType IsNot Nothing AndAlso argumentType.IsDelegateType() AndAlso parameterType.IsDelegateType() AndAlso digThroughToBasesAndImplements = TypeArgumentInference.MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter AndAlso (inferenceRestrictions = RequiredConversion.Any OrElse inferenceRestrictions = RequiredConversion.AnyReverse OrElse inferenceRestrictions = RequiredConversion.AnyAndReverse)) Then
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(argumentType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						Dim delegateInvokeMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
						Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(parameterType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = namedTypeSymbol1.DelegateInvokeMethod
						If (Not namedTypeSymbol.IsAnonymousType OrElse namedTypeSymbol1.IsAnonymousType OrElse methodSymbol Is Nothing OrElse methodSymbol.GetUseSiteInfo().DiagnosticInfo IsNot Nothing) Then
							GoTo Label1
						End If
						Dim parameters As ImmutableArray(Of ParameterSymbol) = delegateInvokeMethod.Parameters
						Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = methodSymbol.Parameters
						If (parameterSymbols.Length = parameters.Length OrElse parameters.Length = 0) Then
							Dim length As Integer = parameters.Length - 1
							Dim num As Integer = 0
							While num <= length
								If (parameters(num).IsByRef <> parameterSymbols(num).IsByRef) Then
									flag = False
									Return flag
								ElseIf (Me.InferTypeArgumentsFromArgument(argumentLocation, parameters(num).Type, argumentTypeByAssumption, parameterSymbols(num).Type, param, TypeArgumentInference.MatchGenericArgumentToParameter.MatchArgumentToBaseOfGenericParameter, RequiredConversion.AnyReverse)) Then
									num = num + 1
								Else
									flag = False
									Return flag
								End If
							End While
							If (methodSymbol.IsSub) Then
								flag = True
								Return flag
							ElseIf (Not delegateInvokeMethod.IsSub) Then
								flag = Me.InferTypeArgumentsFromArgument(argumentLocation, delegateInvokeMethod.ReturnType, argumentTypeByAssumption, methodSymbol.ReturnType, param, TypeArgumentInference.MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, RequiredConversion.Any)
								Return flag
							Else
								flag = False
								Return flag
							End If
						Else
							flag = False
							Return flag
						End If
					End If
				Label1:
					Dim flag1 As Boolean = False
					flag1 = If(digThroughToBasesAndImplements <> TypeArgumentInference.MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, Me.FindMatchingBase(parameterType, argumentType), Me.FindMatchingBase(argumentType, parameterType))
					flag = If(flag1, Me.InferTypeArgumentsFromArgumentDirectly(argumentLocation, argumentType, argumentTypeByAssumption, parameterType, param, digThroughToBasesAndImplements, inferenceRestrictions), False)
				Else
					flag = False
				End If
				Return flag
			End Function

			Private Function InferTypeArgumentsFromArgumentDirectly(ByVal argumentLocation As SyntaxNode, ByVal argumentType As TypeSymbol, ByVal argumentTypeByAssumption As Boolean, ByVal parameterType As TypeSymbol, ByVal param As ParameterSymbol, ByVal digThroughToBasesAndImplements As TypeArgumentInference.MatchGenericArgumentToParameter, ByVal inferenceRestrictions As RequiredConversion) As Boolean
				Dim flag As Boolean
				Dim reference As RequiredConversion
				Dim matchGenericArgumentToParameter As TypeArgumentInference.MatchGenericArgumentToParameter
				Dim tupleUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				If (argumentType Is Nothing OrElse argumentType.IsVoidType()) Then
					flag = False
				ElseIf (Not parameterType.IsTypeParameter()) Then
					Dim typeSymbols As ImmutableArray(Of TypeSymbol) = New ImmutableArray(Of TypeSymbol)()
					Dim typeSymbols1 As ImmutableArray(Of TypeSymbol) = New ImmutableArray(Of TypeSymbol)()
					If (parameterType.GetNullableUnderlyingTypeOrSelf().TryGetElementTypesIfTupleOrCompatible(typeSymbols)) Then
						If (Not If(parameterType.IsNullableType(), argumentType.GetNullableUnderlyingTypeOrSelf(), argumentType).TryGetElementTypesIfTupleOrCompatible(typeSymbols1)) Then
							GoTo Label1
						End If
						If (typeSymbols.Length = typeSymbols1.Length) Then
							Dim length As Integer = typeSymbols.Length - 1
							Dim num As Integer = 0
							While num <= length
								Dim item As TypeSymbol = typeSymbols(num)
								If (Me.InferTypeArgumentsFromArgument(argumentLocation, typeSymbols1(num), argumentTypeByAssumption, item, param, digThroughToBasesAndImplements, inferenceRestrictions)) Then
									num = num + 1
								Else
									flag = False
									Return flag
								End If
							End While
							flag = True
							Return flag
						Else
							flag = False
							Return flag
						End If
					End If
				Label1:
					If (parameterType.Kind <> SymbolKind.NamedType) Then
						If (Not parameterType.IsArrayType()) Then
							GoTo Label2
						End If
						If (argumentType.IsArrayType()) Then
							Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(parameterType, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
							Dim arrayTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(argumentType, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
							Dim flag1 As Boolean = TypeOf arrayTypeSymbol1 Is ArrayLiteralTypeSymbol
							If (arrayTypeSymbol.Rank <> arrayTypeSymbol1.Rank OrElse Not flag1 AndAlso arrayTypeSymbol.IsSZArray <> arrayTypeSymbol1.IsSZArray) Then
								flag = False
								Return flag
							End If
							flag = Me.InferTypeArgumentsFromArgument(argumentLocation, arrayTypeSymbol1.ElementType, argumentTypeByAssumption, arrayTypeSymbol.ElementType, param, digThroughToBasesAndImplements, Conversions.CombineConversionRequirements(inferenceRestrictions, If(flag1, RequiredConversion.Any, RequiredConversion.ArrayElement)))
							Return flag
						End If
						flag = False
						Return flag
					Else
						Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(parameterType.GetTupleUnderlyingTypeOrSelf(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						If (Not containingType.IsGenericType) Then
							GoTo Label2
						End If
						If (argumentType.Kind = SymbolKind.NamedType) Then
							tupleUnderlyingTypeOrSelf = DirectCast(argumentType.GetTupleUnderlyingTypeOrSelf(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						Else
							tupleUnderlyingTypeOrSelf = Nothing
						End If
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = tupleUnderlyingTypeOrSelf
						If (namedTypeSymbol Is Nothing OrElse Not namedTypeSymbol.IsGenericType) Then
							If (Not containingType.IsNullableType()) Then
								flag = False
								Return flag
							End If
							flag = Me.InferTypeArgumentsFromArgument(argumentLocation, argumentType, argumentTypeByAssumption, containingType.GetNullableUnderlyingType(), param, digThroughToBasesAndImplements, Conversions.CombineConversionRequirements(inferenceRestrictions, RequiredConversion.ArrayElement))
							Return flag
						Else
							If (Not namedTypeSymbol.OriginalDefinition.IsSameTypeIgnoringAll(containingType.OriginalDefinition)) Then
								flag = False
								Return flag
							End If
							Do
								Dim arity As Integer = containingType.Arity - 1
								Dim num1 As Integer = 0
								While num1 <= arity
									Dim variance As VarianceKind = containingType.TypeParameters(num1).Variance
									If (variance = VarianceKind.Out) Then
										reference = Conversions.StrengthenConversionRequirementToReference(inferenceRestrictions)
									Else
										reference = If(variance <> VarianceKind.[In], RequiredConversion.Identity, Conversions.InvertConversionRequirement(Conversions.StrengthenConversionRequirementToReference(inferenceRestrictions)))
									End If
									If (reference <> RequiredConversion.Reference) Then
										matchGenericArgumentToParameter = If(reference <> RequiredConversion.ReverseReference, TypeArgumentInference.MatchGenericArgumentToParameter.MatchGenericArgumentToParameterExactly, TypeArgumentInference.MatchGenericArgumentToParameter.MatchArgumentToBaseOfGenericParameter)
									Else
										matchGenericArgumentToParameter = TypeArgumentInference.MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter
									End If
									If (Me.InferTypeArgumentsFromArgument(argumentLocation, namedTypeSymbol.TypeArgumentWithDefinitionUseSiteDiagnostics(num1, Me.UseSiteInfo), argumentTypeByAssumption, containingType.TypeArgumentWithDefinitionUseSiteDiagnostics(num1, Me.UseSiteInfo), param, matchGenericArgumentToParameter, reference)) Then
										num1 = num1 + 1
									Else
										flag = False
										Return flag
									End If
								End While
								containingType = containingType.ContainingType
								namedTypeSymbol = namedTypeSymbol.ContainingType
							Loop While containingType IsNot Nothing
							flag = True
							Return flag
						End If
						flag = False
						Return flag
					End If
				Label2:
					flag = True
				Else
					Me.RegisterTypeParameterHint(DirectCast(parameterType, TypeParameterSymbol), argumentType, argumentTypeByAssumption, argumentLocation, param, False, inferenceRestrictions)
					flag = True
				End If
				Return flag
			End Function

			Public Function InferTypeArgumentsFromLambdaArgument(ByVal argument As BoundExpression, ByVal parameterType As TypeSymbol, ByVal param As ParameterSymbol) As Boolean
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.TypeArgumentInference/InferenceGraph::InferTypeArgumentsFromLambdaArgument(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean InferTypeArgumentsFromLambdaArgument(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function

			Public Sub MarkInferenceFailure()
				Me._someInferenceFailed = True
			End Sub

			Public Sub MarkInferenceLevel(ByVal typeInferenceLevel As TypeArgumentInference.InferenceLevel)
				If (Me._typeInferenceLevel < typeInferenceLevel) Then
					Me._typeInferenceLevel = typeInferenceLevel
				End If
			End Sub

			Private Sub PopulateGraph()
				Dim item As Integer
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim item1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim candidate As MethodSymbol = Me.Candidate
				Dim arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.Arguments
				Dim parameterToArgumentMap As ArrayBuilder(Of Integer) = Me.ParameterToArgumentMap
				Dim paramArrayItems As ArrayBuilder(Of Integer) = Me.ParamArrayItems
				Dim flag As Boolean = paramArrayItems IsNot Nothing
				Dim parameterCount As Integer = candidate.ParameterCount - 1
				Dim num As Integer = 0
				Do
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = candidate.Parameters(num)
					Dim type As TypeSymbol = parameterSymbol.Type
					If (Not parameterSymbol.IsParamArray OrElse num <> candidate.ParameterCount - 1) Then
						item = parameterToArgumentMap(num)
						If (item = -1) Then
							boundExpression = Nothing
						Else
							boundExpression = arguments(item)
						End If
						Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression
						If (boundExpression1 IsNot Nothing AndAlso Not boundExpression1.HasErrors AndAlso Not type.IsErrorType() AndAlso boundExpression1.Kind <> BoundKind.OmittedArgument) Then
							Me.RegisterArgument(boundExpression1, type, parameterSymbol)
						End If
					ElseIf (type.Kind = SymbolKind.ArrayType) Then
						If (Not flag) Then
							item = parameterToArgumentMap(num)
							If (item = -1) Then
								item1 = Nothing
							Else
								item1 = arguments(item)
							End If
							Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = item1
							If (boundExpression2 IsNot Nothing AndAlso Not boundExpression2.HasErrors AndAlso TypeArgumentInference.InferenceGraph.ArgumentTypePossiblyMatchesParamarrayShape(boundExpression2, type)) Then
								Me.RegisterArgument(boundExpression2, type, parameterSymbol)
							End If
						ElseIf (paramArrayItems.Count <> 1 OrElse Not arguments(paramArrayItems(0)).IsNothingLiteral()) Then
							Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
							If (arrayTypeSymbol.IsSZArray) Then
								type = arrayTypeSymbol.ElementType
								If (type.Kind <> SymbolKind.ErrorType) Then
									Dim count As Integer = paramArrayItems.Count - 1
									For i As Integer = 0 To count
										If (Not arguments(paramArrayItems(i)).HasErrors) Then
											Me.RegisterArgument(arguments(paramArrayItems(i)), type, parameterSymbol)
										End If
									Next

								End If
							End If
						End If
					End If
					num = num + 1
				Loop While num <= parameterCount
				Me.AddDelegateReturnTypeToGraph()
			End Sub

			Private Function RefersToGenericParameterToInferArgumentFor(ByVal parameterType As TypeSymbol) As Boolean
				Dim genericParameterToInferArgumentFor As Boolean
				Dim kind As SymbolKind = parameterType.Kind
				If (kind = SymbolKind.ArrayType) Then
					genericParameterToInferArgumentFor = Me.RefersToGenericParameterToInferArgumentFor(DirectCast(parameterType, ArrayTypeSymbol).ElementType)
				Else
					If (kind = SymbolKind.NamedType) Then
						Dim containingType As NamedTypeSymbol = DirectCast(parameterType, NamedTypeSymbol)
						Dim typeSymbols As ImmutableArray(Of TypeSymbol) = New ImmutableArray(Of TypeSymbol)()
						If (Not containingType.TryGetElementTypesIfTupleOrCompatible(typeSymbols)) Then
							Do
								Dim typeSymbols1 As ImmutableArray(Of TypeSymbol) = containingType.TypeArgumentsWithDefinitionUseSiteDiagnostics(Me.UseSiteInfo)
								Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeSymbols1.GetEnumerator()
								While enumerator.MoveNext()
									If (Not Me.RefersToGenericParameterToInferArgumentFor(enumerator.Current)) Then
										Continue While
									End If
									genericParameterToInferArgumentFor = True
									Return genericParameterToInferArgumentFor
								End While
								containingType = containingType.ContainingType
							Loop While containingType IsNot Nothing
						Else
							Dim enumerator1 As ImmutableArray(Of TypeSymbol).Enumerator = typeSymbols.GetEnumerator()
							While enumerator1.MoveNext()
								If (Not Me.RefersToGenericParameterToInferArgumentFor(enumerator1.Current)) Then
									Continue While
								End If
								genericParameterToInferArgumentFor = True
								Return genericParameterToInferArgumentFor
							End While
						End If
					Else
						If (kind <> SymbolKind.TypeParameter OrElse Me.FindTypeParameterNode(DirectCast(parameterType, TypeParameterSymbol)) Is Nothing) Then
							GoTo Label1
						End If
						genericParameterToInferArgumentFor = True
						Return genericParameterToInferArgumentFor
					End If
				Label1:
					genericParameterToInferArgumentFor = False
				End If
				Return genericParameterToInferArgumentFor
			End Function

			Private Sub RegisterArgument(ByVal argument As BoundExpression, ByVal targetType As TypeSymbol, ByVal param As ParameterSymbol)
				If (Not argument.IsNothingLiteral()) Then
					argument = argument.GetMostEnclosedParenthesizedExpression()
				End If
				Dim argumentNode As TypeArgumentInference.ArgumentNode = New TypeArgumentInference.ArgumentNode(Me, argument, targetType, param)
				Dim kind As BoundKind = argument.Kind
				If (kind <= BoundKind.TupleLiteral) Then
					If (kind = BoundKind.AddressOfOperator) Then
						Me.AddAddressOfToGraph(argumentNode, DirectCast(argument, BoundAddressOfOperator).Binder)
						Return
					End If
					If (kind = BoundKind.TupleLiteral) Then
						Me.AddTupleLiteralToGraph(argumentNode)
						Return
					End If
				ElseIf (kind = BoundKind.UnboundLambda OrElse kind = BoundKind.QueryLambda OrElse kind = BoundKind.GroupTypeInferenceLambda) Then
					Me.AddLambdaToGraph(argumentNode, argument.GetBinderFromLambda())
					Return
				End If
				Me.AddTypeToGraph(argumentNode, True)
			End Sub

			Public Sub RegisterErrorReasons(ByVal inferenceErrorReasons As Microsoft.CodeAnalysis.VisualBasic.InferenceErrorReasons)
				Me._inferenceErrorReasons = Me._inferenceErrorReasons Or inferenceErrorReasons
			End Sub

			Public Sub RegisterTypeParameterHint(ByVal genericParameter As TypeParameterSymbol, ByVal inferredType As TypeSymbol, ByVal inferredTypeByAssumption As Boolean, ByVal argumentLocation As SyntaxNode, ByVal parameter As ParameterSymbol, ByVal inferredFromObject As Boolean, ByVal inferenceRestrictions As RequiredConversion)
				Dim typeParameterNode As TypeArgumentInference.TypeParameterNode = Me.FindTypeParameterNode(genericParameter)
				If (typeParameterNode IsNot Nothing) Then
					typeParameterNode.AddTypeHint(inferredType, inferredTypeByAssumption, argumentLocation, parameter, inferredFromObject, inferenceRestrictions)
				End If
			End Sub

			Public Sub ReportAmbiguousInferenceError(ByVal typeInfos As ArrayBuilder(Of TypeArgumentInference.DominantTypeDataTypeInference))
				Dim count As Integer = typeInfos.Count - 1
				For i As Integer = 1 To count
					If (Not typeInfos(i).InferredFromObject) Then
						Me.ReportNotFailedInferenceDueToObject()
					End If
				Next

			End Sub

			Public Sub ReportIncompatibleInferenceError(ByVal typeInfos As ArrayBuilder(Of TypeArgumentInference.DominantTypeDataTypeInference))
				If (typeInfos.Count >= 1) Then
					Dim count As Integer = typeInfos.Count - 1
					For i As Integer = 1 To count
						If (Not typeInfos(i).InferredFromObject) Then
							Me.ReportNotFailedInferenceDueToObject()
						End If
					Next

				End If
			End Sub

			Public Sub ReportNotFailedInferenceDueToObject()
				Me._allFailedInferenceIsDueToObject = False
			End Sub

			Private Shared Function SetMatchIfNothingOrEqual(ByVal type As TypeSymbol, ByRef match As TypeSymbol) As Boolean
				Dim flag As Boolean
				If (match Is Nothing) Then
					match = type
					flag = True
				ElseIf (Not match.IsSameTypeIgnoringAll(type)) Then
					match = Nothing
					flag = False
				Else
					flag = True
				End If
				Return flag
			End Function
		End Class

		Public Enum InferenceLevel As Byte
			None = 0
			Whidbey = 0
			Orcas = 1
			Invalid = 2
		End Enum

		Private MustInherit Class InferenceNode
			Inherits TypeArgumentInference.GraphNode(Of TypeArgumentInference.InferenceNode)
			Public ReadOnly NodeType As TypeArgumentInference.InferenceNodeType

			Public InferenceComplete As Boolean

			Public ReadOnly Property Graph As TypeArgumentInference.InferenceGraph
				Get
					Return DirectCast(Me.Graph, TypeArgumentInference.InferenceGraph)
				End Get
			End Property

			Protected Sub New(ByVal graph As TypeArgumentInference.InferenceGraph, ByVal nodeType As TypeArgumentInference.InferenceNodeType)
				MyBase.New(graph)
				Me.NodeType = nodeType
			End Sub

			Public MustOverride Function InferTypeAndPropagateHints() As Boolean

			<Conditional("DEBUG")>
			Public Sub VerifyIncomingInferenceComplete(ByVal nodeType As TypeArgumentInference.InferenceNodeType)
				If (Not Me.Graph.SomeInferenceHasFailed) Then
					Dim enumerator As ArrayBuilder(Of TypeArgumentInference.InferenceNode).Enumerator = Me.IncomingEdges.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypeArgumentInference.InferenceNode = enumerator.Current
					End While
				End If
			End Sub
		End Class

		Private Enum InferenceNodeType As Byte
			ArgumentNode
			TypeParameterNode
		End Enum

		Public Enum MatchGenericArgumentToParameter
			MatchBaseOfGenericArgumentToParameter
			MatchArgumentToBaseOfGenericParameter
			MatchGenericArgumentToParameterExactly
		End Enum

		Private Class StronglyConnectedComponent(Of TGraphNode As TypeArgumentInference.GraphNode(Of TGraphNode))
			Inherits TypeArgumentInference.GraphNode(Of TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode))
			Public ReadOnly ChildNodes As ArrayBuilder(Of TGraphNode)

			Public Sub New(ByVal graph As TypeArgumentInference.Graph(Of TypeArgumentInference.StronglyConnectedComponent(Of TGraphNode)))
				MyBase.New(graph)
				Me.ChildNodes = New ArrayBuilder(Of TGraphNode)()
			End Sub
		End Class

		Private Class TypeParameterNode
			Inherits TypeArgumentInference.InferenceNode
			Public ReadOnly DeclaredTypeParam As TypeParameterSymbol

			Public ReadOnly InferenceTypeCollection As TypeInferenceCollection(Of TypeArgumentInference.DominantTypeDataTypeInference)

			Private _inferredType As TypeSymbol

			Private _inferredFromLocation As SyntaxNodeOrToken

			Private _inferredTypeByAssumption As Boolean

			Private _candidateInferredType As TypeSymbol

			Private _parameter As ParameterSymbol

			Public ReadOnly Property CandidateInferredType As TypeSymbol
				Get
					Return Me._candidateInferredType
				End Get
			End Property

			Public ReadOnly Property InferredFromLocation As SyntaxNodeOrToken
				Get
					Return Me._inferredFromLocation
				End Get
			End Property

			Public ReadOnly Property InferredType As TypeSymbol
				Get
					Return Me._inferredType
				End Get
			End Property

			Public ReadOnly Property InferredTypeByAssumption As Boolean
				Get
					Return Me._inferredTypeByAssumption
				End Get
			End Property

			Public ReadOnly Property Parameter As ParameterSymbol
				Get
					Return Me._parameter
				End Get
			End Property

			Public Sub New(ByVal graph As TypeArgumentInference.InferenceGraph, ByVal typeParameter As TypeParameterSymbol)
				MyBase.New(graph, TypeArgumentInference.InferenceNodeType.TypeParameterNode)
				Me.DeclaredTypeParam = typeParameter
				Me.InferenceTypeCollection = New TypeInferenceCollection(Of TypeArgumentInference.DominantTypeDataTypeInference)()
			End Sub

			Public Sub AddTypeHint(ByVal type As TypeSymbol, ByVal typeByAssumption As Boolean, ByVal argumentLocation As SyntaxNode, ByVal parameter As ParameterSymbol, ByVal inferredFromObject As Boolean, ByVal inferenceRestrictions As RequiredConversion)
				If (Not type.IsErrorType()) Then
					Dim flag As Boolean = False
					If (Not TypeOf type Is ArrayLiteralTypeSymbol) Then
						Dim enumerator As ArrayBuilder(Of TypeArgumentInference.DominantTypeDataTypeInference).Enumerator = Me.InferenceTypeCollection.GetTypeDataList().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As TypeArgumentInference.DominantTypeDataTypeInference = enumerator.Current
							If (TypeOf current.ResultType Is ArrayLiteralTypeSymbol OrElse Not type.IsSameTypeIgnoringAll(current.ResultType)) Then
								Continue While
							End If
							current.ResultType = TypeInferenceCollection.MergeTupleNames(current.ResultType, type)
							current.InferenceRestrictions = Conversions.CombineConversionRequirements(current.InferenceRestrictions, inferenceRestrictions)
							current.ByAssumption = If(Not current.ByAssumption, False, typeByAssumption)
							flag = True
						End While
					End If
					If (Not flag) Then
						Dim dominantTypeDataTypeInference As TypeArgumentInference.DominantTypeDataTypeInference = New TypeArgumentInference.DominantTypeDataTypeInference() With
						{
							.ResultType = type,
							.ByAssumption = typeByAssumption,
							.InferenceRestrictions = inferenceRestrictions,
							.ArgumentLocation = argumentLocation,
							.Parameter = parameter,
							.InferredFromObject = inferredFromObject,
							.TypeParameter = Me.DeclaredTypeParam
						}
						Me.InferenceTypeCollection.GetTypeDataList().Add(dominantTypeDataTypeInference)
					End If
				End If
			End Sub

			Public Overrides Function InferTypeAndPropagateHints() As Boolean
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.TypeArgumentInference/TypeParameterNode::InferTypeAndPropagateHints()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean InferTypeAndPropagateHints()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function

			Public Sub RegisterInferredType(ByVal inferredType As TypeSymbol, ByVal inferredFromLocation As SyntaxNodeOrToken, ByVal inferredTypeByAssumption As Boolean)
				Dim arrayLiteralTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol = TryCast(inferredType, Microsoft.CodeAnalysis.VisualBasic.ArrayLiteralTypeSymbol)
				If (arrayLiteralTypeSymbol IsNot Nothing) Then
					Dim arrayLiteral As BoundArrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral
					Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = arrayLiteral.InferredType
					If ((Not arrayLiteral.HasDominantType OrElse arrayLiteral.NumberOfCandidates <> 1) AndAlso arrayTypeSymbol.ElementType.SpecialType = SpecialType.System_Object) Then
						Dim elementType As TypeSymbol = arrayTypeSymbol.ElementType
						Dim customModifiers As ImmutableArray(Of CustomModifier) = New ImmutableArray(Of CustomModifier)()
						inferredType = Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol.CreateVBArray(elementType, customModifiers, arrayTypeSymbol.Rank, arrayLiteral.Binder.Compilation.Assembly)
					Else
						inferredType = arrayLiteral.InferredType
					End If
				End If
				Me._inferredType = inferredType
				Me._inferredFromLocation = inferredFromLocation
				Me._inferredTypeByAssumption = inferredTypeByAssumption
				If (Me._candidateInferredType Is Nothing) Then
					Me._candidateInferredType = inferredType
				End If
			End Sub

			Public Sub SetParameter(ByVal parameter As ParameterSymbol)
				Me._parameter = parameter
			End Sub
		End Class
	End Class
End Namespace