using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class TypeArgumentInference
	{
		private class GraphNode<TGraphNode> where TGraphNode : GraphNode<TGraphNode>
		{
			public readonly Graph<TGraphNode> Graph;

			public bool IsAddedToVertices;

			public readonly ArrayBuilder<TGraphNode> IncomingEdges;

			public readonly ArrayBuilder<TGraphNode> OutgoingEdges;

			public GraphAlgorithmData<TGraphNode> AlgorithmData;

			protected GraphNode(Graph<TGraphNode> graph)
			{
				Graph = graph;
				IsAddedToVertices = false;
				IncomingEdges = new ArrayBuilder<TGraphNode>();
				OutgoingEdges = new ArrayBuilder<TGraphNode>();
			}
		}

		private enum DFSColor : byte
		{
			None,
			Grey,
			Black
		}

		private struct GraphAlgorithmData<TGraphNode> where TGraphNode : GraphNode<TGraphNode>
		{
			public DFSColor Color;

			public StronglyConnectedComponent<TGraphNode> StronglyConnectedComponent;
		}

		private class StronglyConnectedComponent<TGraphNode> : GraphNode<StronglyConnectedComponent<TGraphNode>> where TGraphNode : GraphNode<TGraphNode>
		{
			public readonly ArrayBuilder<TGraphNode> ChildNodes;

			public StronglyConnectedComponent(Graph<StronglyConnectedComponent<TGraphNode>> graph)
				: base(graph)
			{
				ChildNodes = new ArrayBuilder<TGraphNode>();
			}
		}

		private class Graph<TGraphNode> where TGraphNode : GraphNode<TGraphNode>
		{
			public readonly ArrayBuilder<TGraphNode> Vertices;

			public Graph()
			{
				Vertices = new ArrayBuilder<TGraphNode>();
			}

			public void AddEdge(TGraphNode source, TGraphNode target)
			{
				AddNode(source);
				AddNode(target);
				source.OutgoingEdges.Add(target);
				target.IncomingEdges.Add(source);
			}

			public void AddNode(TGraphNode node)
			{
				if (!node.IsAddedToVertices)
				{
					Vertices.Add(node);
					node.IsAddedToVertices = true;
				}
			}

			public void RemoveEdge(TGraphNode source, TGraphNode target)
			{
				Remove(source.OutgoingEdges, target);
				Remove(target.IncomingEdges, source);
			}

			private static void Remove(ArrayBuilder<TGraphNode> list, TGraphNode toRemove)
			{
				int num = list.Count - 1;
				int num2 = num;
				for (int i = 0; i <= num2; i++)
				{
					if (list[i] == toRemove)
					{
						if (i < num)
						{
							list[i] = list[num];
						}
						list.Clip(num);
						return;
					}
				}
				throw ExceptionUtilities.Unreachable;
			}

			public Graph<StronglyConnectedComponent<TGraphNode>> BuildStronglyConnectedComponents()
			{
				Graph<StronglyConnectedComponent<TGraphNode>> graph = new Graph<StronglyConnectedComponent<TGraphNode>>();
				ArrayBuilder<TGraphNode> instance = ArrayBuilder<TGraphNode>.GetInstance();
				Dfs(instance);
				ArrayBuilder<TGraphNode>.Enumerator enumerator = instance.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.AlgorithmData = default(GraphAlgorithmData<TGraphNode>);
				}
				ArrayBuilder<TGraphNode>.Enumerator enumerator2 = instance.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TGraphNode current = enumerator2.Current;
					if (current.AlgorithmData.Color == DFSColor.None)
					{
						StronglyConnectedComponent<TGraphNode> stronglyConnectedComponent = new StronglyConnectedComponent<TGraphNode>(graph);
						CollectSccChildren(current, stronglyConnectedComponent);
						graph.AddNode(stronglyConnectedComponent);
					}
				}
				instance.Free();
				instance = null;
				ArrayBuilder<StronglyConnectedComponent<TGraphNode>>.Enumerator enumerator3 = graph.Vertices.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					StronglyConnectedComponent<TGraphNode> current2 = enumerator3.Current;
					ArrayBuilder<TGraphNode>.Enumerator enumerator4 = current2.ChildNodes.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						ArrayBuilder<TGraphNode>.Enumerator enumerator5 = enumerator4.Current.OutgoingEdges.GetEnumerator();
						while (enumerator5.MoveNext())
						{
							StronglyConnectedComponent<TGraphNode> stronglyConnectedComponent2 = enumerator5.Current.AlgorithmData.StronglyConnectedComponent;
							if (current2 != stronglyConnectedComponent2)
							{
								graph.AddEdge(current2, stronglyConnectedComponent2);
							}
						}
					}
				}
				return graph;
			}

			private void CollectSccChildren(TGraphNode node, StronglyConnectedComponent<TGraphNode> sccNode)
			{
				node.AlgorithmData.Color = DFSColor.Grey;
				ArrayBuilder<TGraphNode>.Enumerator enumerator = node.IncomingEdges.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TGraphNode current = enumerator.Current;
					if (current.AlgorithmData.Color == DFSColor.None)
					{
						CollectSccChildren(current, sccNode);
					}
				}
				node.AlgorithmData.Color = DFSColor.Black;
				sccNode.ChildNodes.Add(node);
				node.AlgorithmData.StronglyConnectedComponent = sccNode;
			}

			public void TopoSort(ArrayBuilder<TGraphNode> resultList)
			{
				Dfs(resultList);
			}

			private void Dfs(ArrayBuilder<TGraphNode> resultList)
			{
				ArrayBuilder<TGraphNode>.Enumerator enumerator = Vertices.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.AlgorithmData = default(GraphAlgorithmData<TGraphNode>);
				}
				_ = resultList.Count;
				resultList.AddMany(null, Vertices.Count);
				int insertAt = resultList.Count - 1;
				ArrayBuilder<TGraphNode>.Enumerator enumerator2 = Vertices.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TGraphNode current = enumerator2.Current;
					if (current.AlgorithmData.Color == DFSColor.None)
					{
						DfsVisit(current, resultList, ref insertAt);
					}
				}
			}

			private void DfsVisit(TGraphNode node, ArrayBuilder<TGraphNode> resultList, ref int insertAt)
			{
				node.AlgorithmData.Color = DFSColor.Grey;
				ArrayBuilder<TGraphNode>.Enumerator enumerator = node.OutgoingEdges.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TGraphNode current = enumerator.Current;
					if (current.AlgorithmData.Color == DFSColor.None)
					{
						DfsVisit(current, resultList, ref insertAt);
					}
				}
				node.AlgorithmData.Color = DFSColor.Black;
				resultList[insertAt] = node;
				insertAt--;
			}

			private bool Contains(TGraphNode node)
			{
				if (node.Graph == this)
				{
					return node.IsAddedToVertices;
				}
				return false;
			}
		}

		public enum InferenceLevel : byte
		{
			None = 0,
			Whidbey = 0,
			Orcas = 1,
			Invalid = 2
		}

		public enum MatchGenericArgumentToParameter
		{
			MatchBaseOfGenericArgumentToParameter,
			MatchArgumentToBaseOfGenericParameter,
			MatchGenericArgumentToParameterExactly
		}

		private enum InferenceNodeType : byte
		{
			ArgumentNode,
			TypeParameterNode
		}

		private abstract class InferenceNode : GraphNode<InferenceNode>
		{
			public readonly InferenceNodeType NodeType;

			public bool InferenceComplete;

			public new InferenceGraph Graph => (InferenceGraph)base.Graph;

			protected InferenceNode(InferenceGraph graph, InferenceNodeType nodeType)
				: base((Graph<InferenceNode>)graph)
			{
				NodeType = nodeType;
			}

			public abstract bool InferTypeAndPropagateHints();

			[Conditional("DEBUG")]
			public void VerifyIncomingInferenceComplete(InferenceNodeType nodeType)
			{
				if (!Graph.SomeInferenceHasFailed)
				{
					ArrayBuilder<InferenceNode>.Enumerator enumerator = IncomingEdges.GetEnumerator();
					while (enumerator.MoveNext())
					{
						_ = enumerator.Current;
					}
				}
			}
		}

		private class DominantTypeDataTypeInference : DominantTypeData
		{
			public bool ByAssumption;

			public ParameterSymbol Parameter;

			public bool InferredFromObject;

			public TypeParameterSymbol TypeParameter;

			public SyntaxNode ArgumentLocation;
		}

		private class TypeParameterNode : InferenceNode
		{
			public readonly TypeParameterSymbol DeclaredTypeParam;

			public readonly TypeInferenceCollection<DominantTypeDataTypeInference> InferenceTypeCollection;

			private TypeSymbol _inferredType;

			private SyntaxNodeOrToken _inferredFromLocation;

			private bool _inferredTypeByAssumption;

			private TypeSymbol _candidateInferredType;

			private ParameterSymbol _parameter;

			public TypeSymbol InferredType => _inferredType;

			public TypeSymbol CandidateInferredType => _candidateInferredType;

			public SyntaxNodeOrToken InferredFromLocation => _inferredFromLocation;

			public bool InferredTypeByAssumption => _inferredTypeByAssumption;

			public ParameterSymbol Parameter => _parameter;

			public TypeParameterNode(InferenceGraph graph, TypeParameterSymbol typeParameter)
				: base(graph, InferenceNodeType.TypeParameterNode)
			{
				DeclaredTypeParam = typeParameter;
				InferenceTypeCollection = new TypeInferenceCollection<DominantTypeDataTypeInference>();
			}

			public void RegisterInferredType(TypeSymbol inferredType, SyntaxNodeOrToken inferredFromLocation, bool inferredTypeByAssumption)
			{
				if (inferredType is ArrayLiteralTypeSymbol arrayLiteralTypeSymbol)
				{
					BoundArrayLiteral arrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral;
					ArrayTypeSymbol inferredType2 = arrayLiteral.InferredType;
					inferredType = (((arrayLiteral.HasDominantType && arrayLiteral.NumberOfCandidates == 1) || inferredType2.ElementType.SpecialType != SpecialType.System_Object) ? arrayLiteral.InferredType : ArrayTypeSymbol.CreateVBArray(inferredType2.ElementType, default(ImmutableArray<CustomModifier>), inferredType2.Rank, arrayLiteral.Binder.Compilation.Assembly));
				}
				_inferredType = inferredType;
				_inferredFromLocation = inferredFromLocation;
				_inferredTypeByAssumption = inferredTypeByAssumption;
				if ((object)_candidateInferredType == null)
				{
					_candidateInferredType = inferredType;
				}
			}

			public void SetParameter(ParameterSymbol parameter)
			{
				_parameter = parameter;
			}

			public override bool InferTypeAndPropagateHints()
			{
				int count = IncomingEdges.Count;
				bool result = false;
				int num = 0;
				SyntaxNode syntaxNode = ((count <= 0) ? null : ((ArgumentNode)IncomingEdges[0]).Expression.Syntax);
				int num2 = 0;
				bool flag = false;
				_ = IncomingEdges;
				ArrayBuilder<InferenceNode>.Enumerator enumerator = IncomingEdges.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ArgumentNode argumentNode = (ArgumentNode)enumerator.Current;
					if ((object)argumentNode.Expression.Type != null && TypeSymbolExtensions.IsObjectType(argumentNode.Expression.Type))
					{
						flag = true;
					}
					if (!argumentNode.InferenceComplete)
					{
						base.Graph.RemoveEdge(argumentNode, this);
						result = true;
						num2++;
					}
					else if (BoundExpressionExtensions.IsStrictNothingLiteral(argumentNode.Expression))
					{
						num++;
					}
				}
				if (count > 0 && count == num)
				{
					base.Graph.MarkInferenceFailure();
					base.Graph.ReportNotFailedInferenceDueToObject();
				}
				switch (InferenceTypeCollection.GetTypeDataList().Count)
				{
				case 0:
					if (num2 == count)
					{
						base.Graph.MarkInferenceLevel(InferenceLevel.Orcas);
						break;
					}
					RegisterInferredType(null, default(SyntaxNodeOrToken), inferredTypeByAssumption: false);
					base.Graph.MarkInferenceFailure();
					if (!flag)
					{
						base.Graph.ReportNotFailedInferenceDueToObject();
					}
					break;
				case 1:
				{
					DominantTypeDataTypeInference dominantTypeDataTypeInference2 = InferenceTypeCollection.GetTypeDataList()[0];
					if (syntaxNode == null && dominantTypeDataTypeInference2.ArgumentLocation != null)
					{
						syntaxNode = dominantTypeDataTypeInference2.ArgumentLocation;
					}
					RegisterInferredType(dominantTypeDataTypeInference2.ResultType, syntaxNode, dominantTypeDataTypeInference2.ByAssumption);
					break;
				}
				default:
				{
					TypeSymbol typeSymbol = null;
					ArrayBuilder<DominantTypeDataTypeInference> typeDataList = InferenceTypeCollection.GetTypeDataList();
					ArrayBuilder<DominantTypeDataTypeInference>.Enumerator enumerator2 = typeDataList.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						DominantTypeDataTypeInference current = enumerator2.Current;
						if ((object)typeSymbol == null)
						{
							typeSymbol = current.ResultType;
						}
						else if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(typeSymbol, current.ResultType))
						{
							base.Graph.MarkInferenceLevel(InferenceLevel.Orcas);
						}
					}
					ArrayBuilder<DominantTypeDataTypeInference> instance = ArrayBuilder<DominantTypeDataTypeInference>.GetInstance();
					InferenceErrorReasons inferenceErrorReasons = InferenceErrorReasons.Other;
					InferenceTypeCollection.FindDominantType(instance, ref inferenceErrorReasons, ref base.Graph.UseSiteInfo);
					if (instance.Count == 1)
					{
						inferenceErrorReasons &= ~(InferenceErrorReasons.Ambiguous | InferenceErrorReasons.NoBest);
						DominantTypeDataTypeInference dominantTypeDataTypeInference = instance[0];
						RegisterInferredType(dominantTypeDataTypeInference.ResultType, dominantTypeDataTypeInference.ArgumentLocation, dominantTypeDataTypeInference.ByAssumption);
					}
					else
					{
						if ((inferenceErrorReasons & InferenceErrorReasons.Ambiguous) != 0)
						{
							base.Graph.ReportAmbiguousInferenceError(instance);
						}
						else
						{
							base.Graph.ReportIncompatibleInferenceError(typeDataList);
						}
						RegisterInferredType(typeDataList[0].ResultType, syntaxNode, inferredTypeByAssumption: false);
						base.Graph.MarkInferenceFailure();
					}
					base.Graph.RegisterErrorReasons(inferenceErrorReasons);
					instance.Free();
					break;
				}
				}
				InferenceComplete = true;
				return result;
			}

			public void AddTypeHint(TypeSymbol type, bool typeByAssumption, SyntaxNode argumentLocation, ParameterSymbol parameter, bool inferredFromObject, RequiredConversion inferenceRestrictions)
			{
				if (TypeSymbolExtensions.IsErrorType(type))
				{
					return;
				}
				bool flag = false;
				if (!(type is ArrayLiteralTypeSymbol))
				{
					ArrayBuilder<DominantTypeDataTypeInference>.Enumerator enumerator = InferenceTypeCollection.GetTypeDataList().GetEnumerator();
					while (enumerator.MoveNext())
					{
						DominantTypeDataTypeInference current = enumerator.Current;
						if (!(current.ResultType is ArrayLiteralTypeSymbol) && TypeSymbolExtensions.IsSameTypeIgnoringAll(type, current.ResultType))
						{
							current.ResultType = TypeInferenceCollection.MergeTupleNames(current.ResultType, type);
							current.InferenceRestrictions = Conversions.CombineConversionRequirements(current.InferenceRestrictions, inferenceRestrictions);
							current.ByAssumption = current.ByAssumption && typeByAssumption;
							flag = true;
						}
					}
				}
				if (!flag)
				{
					DominantTypeDataTypeInference dominantTypeDataTypeInference = new DominantTypeDataTypeInference();
					dominantTypeDataTypeInference.ResultType = type;
					dominantTypeDataTypeInference.ByAssumption = typeByAssumption;
					dominantTypeDataTypeInference.InferenceRestrictions = inferenceRestrictions;
					dominantTypeDataTypeInference.ArgumentLocation = argumentLocation;
					dominantTypeDataTypeInference.Parameter = parameter;
					dominantTypeDataTypeInference.InferredFromObject = inferredFromObject;
					dominantTypeDataTypeInference.TypeParameter = DeclaredTypeParam;
					InferenceTypeCollection.GetTypeDataList().Add(dominantTypeDataTypeInference);
				}
			}
		}

		private class ArgumentNode : InferenceNode
		{
			public readonly TypeSymbol ParameterType;

			public readonly BoundExpression Expression;

			public readonly ParameterSymbol Parameter;

			public ArgumentNode(InferenceGraph graph, BoundExpression expression, TypeSymbol parameterType, ParameterSymbol parameter)
				: base(graph, InferenceNodeType.ArgumentNode)
			{
				Expression = expression;
				ParameterType = parameterType;
				Parameter = parameter;
			}

			public override bool InferTypeAndPropagateHints()
			{
				ArrayBuilder<InferenceNode>.Enumerator enumerator = IncomingEdges.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeParameterNode typeParameterNode = (TypeParameterNode)enumerator.Current;
					if ((object)typeParameterNode.InferredType != null)
					{
						continue;
					}
					bool flag = true;
					if (Expression.Kind == BoundKind.UnboundLambda && TypeSymbolExtensions.IsDelegateType(ParameterType))
					{
						MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)ParameterType).DelegateInvokeMethod;
						if ((object)delegateInvokeMethod != null && delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo == null)
						{
							UnboundLambda unboundLambda = (UnboundLambda)Expression;
							ImmutableArray<ParameterSymbol> parameters = unboundLambda.Parameters;
							ImmutableArray<ParameterSymbol> parameters2 = delegateInvokeMethod.Parameters;
							int num = Math.Min(parameters.Length, parameters2.Length) - 1;
							for (int i = 0; i <= num; i++)
							{
								UnboundLambdaParameterSymbol unboundLambdaParameterSymbol = (UnboundLambdaParameterSymbol)parameters[i];
								ParameterSymbol parameterSymbol = parameters2[i];
								if ((object)unboundLambdaParameterSymbol.Type == null && parameterSymbol.Type.Equals(typeParameterNode.DeclaredTypeParam))
								{
									if (base.Graph.Diagnostic == null)
									{
										base.Graph.Diagnostic = BindingDiagnosticBag.Create(withDiagnostics: true, base.Graph.UseSiteInfo.AccumulatesDependencies);
									}
									if ((object)base.Graph.ObjectType == null)
									{
										base.Graph.ObjectType = unboundLambda.Binder.GetSpecialType(SpecialType.System_Object, unboundLambdaParameterSymbol.IdentifierSyntax, base.Graph.Diagnostic);
									}
									typeParameterNode.RegisterInferredType(base.Graph.ObjectType, unboundLambdaParameterSymbol.TypeSyntax, typeParameterNode.InferredTypeByAssumption);
									unboundLambda.Binder.ReportLambdaParameterInferredToBeObject(unboundLambdaParameterSymbol, base.Graph.Diagnostic);
									flag = false;
									break;
								}
							}
						}
					}
					if (flag)
					{
						InferenceComplete = true;
						return false;
					}
				}
				bool flag2 = false;
				switch (Expression.Kind)
				{
				case BoundKind.AddressOfOperator:
					flag2 = base.Graph.InferTypeArgumentsFromAddressOfArgument(Expression, ParameterType, Parameter);
					break;
				case BoundKind.LateAddressOfOperator:
					base.Graph.ReportNotFailedInferenceDueToObject();
					flag2 = true;
					break;
				case BoundKind.UnboundLambda:
				case BoundKind.QueryLambda:
				case BoundKind.GroupTypeInferenceLambda:
					base.Graph.MarkInferenceLevel(InferenceLevel.Orcas);
					flag2 = base.Graph.InferTypeArgumentsFromLambdaArgument(Expression, ParameterType, Parameter);
					break;
				default:
				{
					if (BoundExpressionExtensions.IsStrictNothingLiteral(Expression))
					{
						InferenceComplete = true;
						return false;
					}
					RequiredConversion requiredConversion = RequiredConversion.Any;
					if ((object)Parameter != null && Parameter.IsByRef && (Expression.IsLValue || BoundExpressionExtensions.IsPropertySupportingAssignment(Expression)))
					{
						requiredConversion = Conversions.CombineConversionRequirements(requiredConversion, Conversions.InvertConversionRequirement(requiredConversion));
					}
					bool argumentTypeByAssumption = false;
					TypeSymbol argumentType;
					if (Expression.Kind != BoundKind.ArrayLiteral)
					{
						argumentType = ((Expression.Kind != BoundKind.TupleLiteral) ? Expression.Type : ((BoundTupleLiteral)Expression).InferredType);
					}
					else
					{
						BoundArrayLiteral obj = (BoundArrayLiteral)Expression;
						argumentTypeByAssumption = obj.NumberOfCandidates != 1;
						argumentType = new ArrayLiteralTypeSymbol(obj);
					}
					flag2 = base.Graph.InferTypeArgumentsFromArgument(Expression.Syntax, argumentType, argumentTypeByAssumption, ParameterType, Parameter, MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, requiredConversion);
					break;
				}
				}
				if (!flag2)
				{
					base.Graph.MarkInferenceFailure();
					if ((object)Expression.Type == null || !TypeSymbolExtensions.IsObjectType(Expression.Type))
					{
						base.Graph.ReportNotFailedInferenceDueToObject();
					}
				}
				InferenceComplete = true;
				return false;
			}
		}

		private class InferenceGraph : Graph<InferenceNode>
		{
			public BindingDiagnosticBag Diagnostic;

			public NamedTypeSymbol ObjectType;

			public readonly MethodSymbol Candidate;

			public readonly ImmutableArray<BoundExpression> Arguments;

			public readonly ArrayBuilder<int> ParameterToArgumentMap;

			public readonly ArrayBuilder<int> ParamArrayItems;

			public readonly TypeSymbol DelegateReturnType;

			public readonly BoundNode DelegateReturnTypeReferenceBoundNode;

			public CompoundUseSiteInfo<AssemblySymbol> UseSiteInfo;

			private bool _someInferenceFailed;

			private InferenceErrorReasons _inferenceErrorReasons;

			private bool _allFailedInferenceIsDueToObject;

			private InferenceLevel _typeInferenceLevel;

			private HashSet<BoundExpression> _asyncLambdaSubToFunctionMismatch;

			private readonly ImmutableArray<TypeParameterNode> _typeParameterNodes;

			private readonly bool _verifyingAssertions;

			public bool SomeInferenceHasFailed => _someInferenceFailed;

			public bool AllFailedInferenceIsDueToObject => _allFailedInferenceIsDueToObject;

			public InferenceErrorReasons InferenceErrorReasons => _inferenceErrorReasons;

			public InferenceLevel TypeInferenceLevel => _typeInferenceLevel;

			private InferenceGraph(BindingDiagnosticBag diagnostic, MethodSymbol candidate, ImmutableArray<BoundExpression> arguments, ArrayBuilder<int> parameterToArgumentMap, ArrayBuilder<int> paramArrayItems, TypeSymbol delegateReturnType, BoundNode delegateReturnTypeReferenceBoundNode, HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
			{
				_allFailedInferenceIsDueToObject = true;
				_typeInferenceLevel = InferenceLevel.None;
				Diagnostic = diagnostic;
				Candidate = candidate;
				Arguments = arguments;
				ParameterToArgumentMap = parameterToArgumentMap;
				ParamArrayItems = paramArrayItems;
				DelegateReturnType = delegateReturnType;
				DelegateReturnTypeReferenceBoundNode = delegateReturnTypeReferenceBoundNode;
				_asyncLambdaSubToFunctionMismatch = asyncLambdaSubToFunctionMismatch;
				UseSiteInfo = useSiteInfo;
				int arity = candidate.Arity;
				TypeParameterNode[] array = new TypeParameterNode[arity - 1 + 1];
				int num = arity - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = new TypeParameterNode(this, candidate.TypeParameters[i]);
				}
				_typeParameterNodes = array.AsImmutableOrNull();
			}

			public void MarkInferenceFailure()
			{
				_someInferenceFailed = true;
			}

			public void ReportNotFailedInferenceDueToObject()
			{
				_allFailedInferenceIsDueToObject = false;
			}

			public void MarkInferenceLevel(InferenceLevel typeInferenceLevel)
			{
				if (_typeInferenceLevel < typeInferenceLevel)
				{
					_typeInferenceLevel = typeInferenceLevel;
				}
			}

			public static bool Infer(MethodSymbol candidate, ImmutableArray<BoundExpression> arguments, ArrayBuilder<int> parameterToArgumentMap, ArrayBuilder<int> paramArrayItems, TypeSymbol delegateReturnType, BoundNode delegateReturnTypeReferenceBoundNode, ref ImmutableArray<TypeSymbol> typeArguments, ref InferenceLevel inferenceLevel, ref bool allFailedInferenceIsDueToObject, ref bool someInferenceFailed, ref InferenceErrorReasons inferenceErrorReasons, out BitVector inferredTypeByAssumption, out ImmutableArray<SyntaxNodeOrToken> typeArgumentsLocation, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BindingDiagnosticBag diagnostic, BitVector inferTheseTypeParameters)
			{
				InferenceGraph inferenceGraph = new InferenceGraph(diagnostic, candidate, arguments, parameterToArgumentMap, paramArrayItems, delegateReturnType, delegateReturnTypeReferenceBoundNode, asyncLambdaSubToFunctionMismatch, useSiteInfo);
				inferenceGraph.PopulateGraph();
				ArrayBuilder<StronglyConnectedComponent<InferenceNode>> instance = ArrayBuilder<StronglyConnectedComponent<InferenceNode>>.GetInstance();
				bool flag;
				do
				{
					flag = false;
					Graph<StronglyConnectedComponent<InferenceNode>> graph = inferenceGraph.BuildStronglyConnectedComponents();
					instance.Clear();
					graph.TopoSort(instance);
					ArrayBuilder<StronglyConnectedComponent<InferenceNode>>.Enumerator enumerator = instance.GetEnumerator();
					while (enumerator.MoveNext())
					{
						ArrayBuilder<InferenceNode> childNodes = enumerator.Current.ChildNodes;
						if (childNodes.Count == 1)
						{
							if (childNodes[0].InferTypeAndPropagateHints())
							{
								throw ExceptionUtilities.Unreachable;
							}
							continue;
						}
						bool flag2 = false;
						ArrayBuilder<InferenceNode>.Enumerator enumerator2 = childNodes.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							InferenceNode current = enumerator2.Current;
							if (current.NodeType == InferenceNodeType.TypeParameterNode && ((TypeParameterNode)current).InferenceTypeCollection.GetTypeDataList().Count > 0)
							{
								if (current.InferTypeAndPropagateHints())
								{
									flag = true;
								}
								flag2 = true;
							}
						}
						if (!flag2)
						{
							ArrayBuilder<InferenceNode>.Enumerator enumerator3 = childNodes.GetEnumerator();
							while (enumerator3.MoveNext())
							{
								InferenceNode current2 = enumerator3.Current;
								if (current2.NodeType == InferenceNodeType.TypeParameterNode && current2.InferTypeAndPropagateHints())
								{
									flag = true;
								}
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
				while (flag);
				instance.Free();
				bool result = !inferenceGraph.SomeInferenceHasFailed;
				someInferenceFailed = inferenceGraph.SomeInferenceHasFailed;
				allFailedInferenceIsDueToObject = inferenceGraph.AllFailedInferenceIsDueToObject;
				inferenceErrorReasons = inferenceGraph.InferenceErrorReasons;
				if (!someInferenceFailed || (object)delegateReturnType != null)
				{
					allFailedInferenceIsDueToObject = false;
				}
				int arity = candidate.Arity;
				TypeSymbol[] array = new TypeSymbol[arity - 1 + 1];
				SyntaxNodeOrToken[] array2 = new SyntaxNodeOrToken[arity - 1 + 1];
				int num = arity - 1;
				for (int i = 0; i <= num; i++)
				{
					TypeParameterNode typeParameterNode = inferenceGraph._typeParameterNodes[i];
					TypeSymbol inferredType = typeParameterNode.InferredType;
					if ((object)inferredType == null && (inferTheseTypeParameters.IsNull || inferTheseTypeParameters[i]))
					{
						result = false;
					}
					if (typeParameterNode.InferredTypeByAssumption)
					{
						if (inferredTypeByAssumption.IsNull)
						{
							inferredTypeByAssumption = BitVector.Create(arity);
						}
						inferredTypeByAssumption[i] = true;
					}
					array[i] = inferredType;
					array2[i] = typeParameterNode.InferredFromLocation;
				}
				typeArguments = array.AsImmutableOrNull();
				typeArgumentsLocation = array2.AsImmutableOrNull();
				inferenceLevel = inferenceGraph._typeInferenceLevel;
				diagnostic = inferenceGraph.Diagnostic;
				asyncLambdaSubToFunctionMismatch = inferenceGraph._asyncLambdaSubToFunctionMismatch;
				useSiteInfo = inferenceGraph.UseSiteInfo;
				return result;
			}

			private void PopulateGraph()
			{
				MethodSymbol candidate = Candidate;
				ImmutableArray<BoundExpression> arguments = Arguments;
				ArrayBuilder<int> parameterToArgumentMap = ParameterToArgumentMap;
				ArrayBuilder<int> paramArrayItems = ParamArrayItems;
				bool flag = paramArrayItems != null;
				int num = candidate.ParameterCount - 1;
				for (int i = 0; i <= num; i++)
				{
					ParameterSymbol parameterSymbol = candidate.Parameters[i];
					TypeSymbol type = parameterSymbol.Type;
					if (parameterSymbol.IsParamArray && i == candidate.ParameterCount - 1)
					{
						if (type.Kind != SymbolKind.ArrayType)
						{
							continue;
						}
						if (!flag)
						{
							int num2 = parameterToArgumentMap[i];
							BoundExpression boundExpression = ((num2 == -1) ? null : arguments[num2]);
							if (boundExpression != null && !boundExpression.HasErrors && ArgumentTypePossiblyMatchesParamarrayShape(boundExpression, type))
							{
								RegisterArgument(boundExpression, type, parameterSymbol);
							}
						}
						else
						{
							if (paramArrayItems.Count == 1 && BoundExpressionExtensions.IsNothingLiteral(arguments[paramArrayItems[0]]))
							{
								continue;
							}
							ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
							if (!arrayTypeSymbol.IsSZArray)
							{
								continue;
							}
							type = arrayTypeSymbol.ElementType;
							if (type.Kind == SymbolKind.ErrorType)
							{
								continue;
							}
							int num3 = paramArrayItems.Count - 1;
							for (int j = 0; j <= num3; j++)
							{
								if (!arguments[paramArrayItems[j]].HasErrors)
								{
									RegisterArgument(arguments[paramArrayItems[j]], type, parameterSymbol);
								}
							}
						}
					}
					else
					{
						int num2 = parameterToArgumentMap[i];
						BoundExpression boundExpression2 = ((num2 == -1) ? null : arguments[num2]);
						if (boundExpression2 != null && !boundExpression2.HasErrors && !TypeSymbolExtensions.IsErrorType(type) && boundExpression2.Kind != BoundKind.OmittedArgument)
						{
							RegisterArgument(boundExpression2, type, parameterSymbol);
						}
					}
				}
				AddDelegateReturnTypeToGraph();
			}

			private void AddDelegateReturnTypeToGraph()
			{
				if ((object)DelegateReturnType == null || TypeSymbolExtensions.IsVoidType(DelegateReturnType))
				{
					return;
				}
				BoundRValuePlaceholder expression = new BoundRValuePlaceholder(DelegateReturnTypeReferenceBoundNode.Syntax, DelegateReturnType);
				ArgumentNode argumentNode = new ArgumentNode(this, expression, Candidate.ReturnType, null);
				ArrayBuilder<InferenceNode>.Enumerator enumerator = Vertices.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InferenceNode current = enumerator.Current;
					if (current.NodeType == InferenceNodeType.TypeParameterNode)
					{
						AddEdge(current, argumentNode);
					}
				}
				AddTypeToGraph(argumentNode, isOutgoingEdge: true);
			}

			private void RegisterArgument(BoundExpression argument, TypeSymbol targetType, ParameterSymbol param)
			{
				if (!BoundExpressionExtensions.IsNothingLiteral(argument))
				{
					argument = BoundExpressionExtensions.GetMostEnclosedParenthesizedExpression(argument);
				}
				ArgumentNode argumentNode = new ArgumentNode(this, argument, targetType, param);
				switch (argument.Kind)
				{
				case BoundKind.UnboundLambda:
				case BoundKind.QueryLambda:
				case BoundKind.GroupTypeInferenceLambda:
					AddLambdaToGraph(argumentNode, BoundNodeExtensions.GetBinderFromLambda(argument));
					break;
				case BoundKind.AddressOfOperator:
					AddAddressOfToGraph(argumentNode, ((BoundAddressOfOperator)argument).Binder);
					break;
				case BoundKind.TupleLiteral:
					AddTupleLiteralToGraph(argumentNode);
					break;
				default:
					AddTypeToGraph(argumentNode, isOutgoingEdge: true);
					break;
				}
			}

			private void AddTypeToGraph(ArgumentNode node, bool isOutgoingEdge)
			{
				TypeSymbol parameterType = node.ParameterType;
				BitVector haveSeenTypeParameters = BitVector.Create(_typeParameterNodes.Length);
				AddTypeToGraph(parameterType, node, isOutgoingEdge, ref haveSeenTypeParameters);
			}

			private TypeParameterNode FindTypeParameterNode(TypeParameterSymbol typeParameter)
			{
				int ordinal = typeParameter.Ordinal;
				if (ordinal < _typeParameterNodes.Length && _typeParameterNodes[ordinal] != null && typeParameter.Equals(_typeParameterNodes[ordinal].DeclaredTypeParam))
				{
					return _typeParameterNodes[ordinal];
				}
				return null;
			}

			private void AddTypeToGraph(TypeSymbol parameterType, ArgumentNode argNode, bool isOutgoingEdge, ref BitVector haveSeenTypeParameters)
			{
				switch (parameterType.Kind)
				{
				case SymbolKind.TypeParameter:
				{
					TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)parameterType;
					TypeParameterNode typeParameterNode = FindTypeParameterNode(typeParameterSymbol);
					if (typeParameterNode != null && !haveSeenTypeParameters[typeParameterSymbol.Ordinal])
					{
						if ((object)typeParameterNode.Parameter == null)
						{
							typeParameterNode.SetParameter(argNode.Parameter);
						}
						if (isOutgoingEdge)
						{
							AddEdge(argNode, typeParameterNode);
						}
						else
						{
							AddEdge(typeParameterNode, argNode);
						}
						haveSeenTypeParameters[typeParameterSymbol.Ordinal] = true;
					}
					break;
				}
				case SymbolKind.ArrayType:
					AddTypeToGraph(((ArrayTypeSymbol)parameterType).ElementType, argNode, isOutgoingEdge, ref haveSeenTypeParameters);
					break;
				case SymbolKind.NamedType:
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)parameterType;
					ImmutableArray<TypeSymbol> elementTypes = default(ImmutableArray<TypeSymbol>);
					if (TypeSymbolExtensions.TryGetElementTypesIfTupleOrCompatible(namedTypeSymbol, out elementTypes))
					{
						ImmutableArray<TypeSymbol>.Enumerator enumerator = elementTypes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							TypeSymbol current = enumerator.Current;
							AddTypeToGraph(current, argNode, isOutgoingEdge, ref haveSeenTypeParameters);
						}
						break;
					}
					do
					{
						ImmutableArray<TypeSymbol>.Enumerator enumerator2 = namedTypeSymbol.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref UseSiteInfo).GetEnumerator();
						while (enumerator2.MoveNext())
						{
							TypeSymbol current2 = enumerator2.Current;
							AddTypeToGraph(current2, argNode, isOutgoingEdge, ref haveSeenTypeParameters);
						}
						namedTypeSymbol = namedTypeSymbol.ContainingType;
					}
					while ((object)namedTypeSymbol != null);
					break;
				}
				}
			}

			private void AddTupleLiteralToGraph(ArgumentNode argNode)
			{
				AddTupleLiteralToGraph(argNode.ParameterType, argNode);
			}

			private void AddTupleLiteralToGraph(TypeSymbol parameterType, ArgumentNode argNode)
			{
				ImmutableArray<BoundExpression> arguments = ((BoundTupleLiteral)argNode.Expression).Arguments;
				if (parameterType.IsTupleOrCompatibleWithTupleOfCardinality(arguments.Length))
				{
					ImmutableArray<TypeSymbol> elementTypesOfTupleOrCompatible = TypeSymbolExtensions.GetElementTypesOfTupleOrCompatible(parameterType);
					int num = arguments.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						RegisterArgument(arguments[i], elementTypesOfTupleOrCompatible[i], argNode.Parameter);
					}
				}
				else
				{
					AddTypeToGraph(argNode, isOutgoingEdge: true);
				}
			}

			private void AddAddressOfToGraph(ArgumentNode argNode, Binder binder)
			{
				AddAddressOfToGraph(argNode.ParameterType, argNode, binder);
			}

			private void AddAddressOfToGraph(TypeSymbol parameterType, ArgumentNode argNode, Binder binder)
			{
				if (TypeSymbolExtensions.IsTypeParameter(parameterType))
				{
					BitVector haveSeenTypeParameters = BitVector.Create(_typeParameterNodes.Length);
					AddTypeToGraph(parameterType, argNode, isOutgoingEdge: true, ref haveSeenTypeParameters);
				}
				else if (TypeSymbolExtensions.IsDelegateType(parameterType))
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)parameterType;
					MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
					if ((object)delegateInvokeMethod != null && delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo == null && namedTypeSymbol.IsGenericType)
					{
						BitVector haveSeenTypeParameters2 = BitVector.Create(_typeParameterNodes.Length);
						AddTypeToGraph(delegateInvokeMethod.ReturnType, argNode, isOutgoingEdge: true, ref haveSeenTypeParameters2);
						haveSeenTypeParameters2.Clear();
						ImmutableArray<ParameterSymbol>.Enumerator enumerator = delegateInvokeMethod.Parameters.GetEnumerator();
						while (enumerator.MoveNext())
						{
							ParameterSymbol current = enumerator.Current;
							AddTypeToGraph(current.Type, argNode, isOutgoingEdge: false, ref haveSeenTypeParameters2);
						}
					}
				}
				else if (TypeSymbol.Equals(parameterType.OriginalDefinition, binder.Compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T), TypeCompareKind.ConsiderEverything))
				{
					AddAddressOfToGraph(((NamedTypeSymbol)parameterType).TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref UseSiteInfo), argNode, binder);
				}
			}

			private void AddLambdaToGraph(ArgumentNode argNode, Binder binder)
			{
				AddLambdaToGraph(argNode.ParameterType, argNode, binder);
			}

			private void AddLambdaToGraph(TypeSymbol parameterType, ArgumentNode argNode, Binder binder)
			{
				if (TypeSymbolExtensions.IsTypeParameter(parameterType))
				{
					BitVector haveSeenTypeParameters = BitVector.Create(_typeParameterNodes.Length);
					AddTypeToGraph(parameterType, argNode, isOutgoingEdge: true, ref haveSeenTypeParameters);
				}
				else if (TypeSymbolExtensions.IsDelegateType(parameterType))
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)parameterType;
					MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
					if ((object)delegateInvokeMethod == null || delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo != null || !namedTypeSymbol.IsGenericType)
					{
						return;
					}
					ImmutableArray<ParameterSymbol> parameters = delegateInvokeMethod.Parameters;
					ImmutableArray<ParameterSymbol> immutableArray = argNode.Expression.Kind switch
					{
						BoundKind.QueryLambda => ((BoundQueryLambda)argNode.Expression).LambdaSymbol.Parameters, 
						BoundKind.GroupTypeInferenceLambda => ((GroupTypeInferenceLambda)argNode.Expression).Parameters, 
						BoundKind.UnboundLambda => ((UnboundLambda)argNode.Expression).Parameters, 
						_ => throw ExceptionUtilities.UnexpectedValue(argNode.Expression.Kind), 
					};
					BitVector haveSeenTypeParameters2 = BitVector.Create(_typeParameterNodes.Length);
					int num = Math.Min(parameters.Length, immutableArray.Length) - 1;
					for (int i = 0; i <= num; i++)
					{
						if ((object)immutableArray[i].Type != null)
						{
							InferTypeArgumentsFromArgument(argNode.Expression.Syntax, immutableArray[i].Type, argumentTypeByAssumption: false, parameters[i].Type, parameters[i], MatchGenericArgumentToParameter.MatchArgumentToBaseOfGenericParameter, RequiredConversion.Any);
						}
						AddTypeToGraph(parameters[i].Type, argNode, isOutgoingEdge: false, ref haveSeenTypeParameters2);
					}
					haveSeenTypeParameters2.Clear();
					AddTypeToGraph(delegateInvokeMethod.ReturnType, argNode, isOutgoingEdge: true, ref haveSeenTypeParameters2);
				}
				else if (TypeSymbol.Equals(parameterType.OriginalDefinition, binder.Compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T), TypeCompareKind.ConsiderEverything))
				{
					AddLambdaToGraph(((NamedTypeSymbol)parameterType).TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref UseSiteInfo), argNode, binder);
				}
			}

			private static bool ArgumentTypePossiblyMatchesParamarrayShape(BoundExpression argument, TypeSymbol paramType)
			{
				TypeSymbol typeSymbol = argument.Type;
				bool flag = false;
				if ((object)typeSymbol == null)
				{
					if (argument.Kind != BoundKind.ArrayLiteral)
					{
						return false;
					}
					flag = true;
					typeSymbol = ((BoundArrayLiteral)argument).InferredType;
				}
				while (TypeSymbolExtensions.IsArrayType(paramType))
				{
					if (!TypeSymbolExtensions.IsArrayType(typeSymbol))
					{
						return false;
					}
					ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)typeSymbol;
					ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)paramType;
					if (arrayTypeSymbol.Rank != arrayTypeSymbol2.Rank || (!flag && arrayTypeSymbol.IsSZArray != arrayTypeSymbol2.IsSZArray))
					{
						return false;
					}
					flag = false;
					typeSymbol = arrayTypeSymbol.ElementType;
					paramType = arrayTypeSymbol2.ElementType;
				}
				return true;
			}

			public void RegisterTypeParameterHint(TypeParameterSymbol genericParameter, TypeSymbol inferredType, bool inferredTypeByAssumption, SyntaxNode argumentLocation, ParameterSymbol parameter, bool inferredFromObject, RequiredConversion inferenceRestrictions)
			{
				FindTypeParameterNode(genericParameter)?.AddTypeHint(inferredType, inferredTypeByAssumption, argumentLocation, parameter, inferredFromObject, inferenceRestrictions);
			}

			private bool RefersToGenericParameterToInferArgumentFor(TypeSymbol parameterType)
			{
				switch (parameterType.Kind)
				{
				case SymbolKind.TypeParameter:
				{
					TypeParameterSymbol typeParameter = (TypeParameterSymbol)parameterType;
					if (FindTypeParameterNode(typeParameter) != null)
					{
						return true;
					}
					break;
				}
				case SymbolKind.ArrayType:
					return RefersToGenericParameterToInferArgumentFor(((ArrayTypeSymbol)parameterType).ElementType);
				case SymbolKind.NamedType:
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)parameterType;
					ImmutableArray<TypeSymbol> elementTypes = default(ImmutableArray<TypeSymbol>);
					if (TypeSymbolExtensions.TryGetElementTypesIfTupleOrCompatible(namedTypeSymbol, out elementTypes))
					{
						ImmutableArray<TypeSymbol>.Enumerator enumerator = elementTypes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							TypeSymbol current = enumerator.Current;
							if (RefersToGenericParameterToInferArgumentFor(current))
							{
								return true;
							}
						}
						break;
					}
					do
					{
						ImmutableArray<TypeSymbol>.Enumerator enumerator2 = namedTypeSymbol.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref UseSiteInfo).GetEnumerator();
						while (enumerator2.MoveNext())
						{
							TypeSymbol current2 = enumerator2.Current;
							if (RefersToGenericParameterToInferArgumentFor(current2))
							{
								return true;
							}
						}
						namedTypeSymbol = namedTypeSymbol.ContainingType;
					}
					while ((object)namedTypeSymbol != null);
					break;
				}
				}
				return false;
			}

			private bool InferTypeArgumentsFromArgumentDirectly(SyntaxNode argumentLocation, TypeSymbol argumentType, bool argumentTypeByAssumption, TypeSymbol parameterType, ParameterSymbol param, MatchGenericArgumentToParameter digThroughToBasesAndImplements, RequiredConversion inferenceRestrictions)
			{
				if ((object)argumentType == null || TypeSymbolExtensions.IsVoidType(argumentType))
				{
					return false;
				}
				if (TypeSymbolExtensions.IsTypeParameter(parameterType))
				{
					RegisterTypeParameterHint((TypeParameterSymbol)parameterType, argumentType, argumentTypeByAssumption, argumentLocation, param, inferredFromObject: false, inferenceRestrictions);
					return true;
				}
				ImmutableArray<TypeSymbol> elementTypes = default(ImmutableArray<TypeSymbol>);
				ImmutableArray<TypeSymbol> elementTypes2 = default(ImmutableArray<TypeSymbol>);
				if (TypeSymbolExtensions.TryGetElementTypesIfTupleOrCompatible(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(parameterType), out elementTypes) && TypeSymbolExtensions.TryGetElementTypesIfTupleOrCompatible(TypeSymbolExtensions.IsNullableType(parameterType) ? TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(argumentType) : argumentType, out elementTypes2))
				{
					if (elementTypes.Length != elementTypes2.Length)
					{
						return false;
					}
					int num = elementTypes.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						TypeSymbol parameterType2 = elementTypes[i];
						TypeSymbol argumentType2 = elementTypes2[i];
						if (!InferTypeArgumentsFromArgument(argumentLocation, argumentType2, argumentTypeByAssumption, parameterType2, param, digThroughToBasesAndImplements, inferenceRestrictions))
						{
							return false;
						}
					}
					return true;
				}
				if (parameterType.Kind == SymbolKind.NamedType)
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(parameterType);
					if (namedTypeSymbol.IsGenericType)
					{
						NamedTypeSymbol namedTypeSymbol2 = ((argumentType.Kind == SymbolKind.NamedType) ? ((NamedTypeSymbol)TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(argumentType)) : null);
						if ((object)namedTypeSymbol2 != null && namedTypeSymbol2.IsGenericType)
						{
							if (TypeSymbolExtensions.IsSameTypeIgnoringAll(namedTypeSymbol2.OriginalDefinition, namedTypeSymbol.OriginalDefinition))
							{
								do
								{
									int num2 = namedTypeSymbol.Arity - 1;
									for (int j = 0; j <= num2; j++)
									{
										RequiredConversion requiredConversion = namedTypeSymbol.TypeParameters[j].Variance switch
										{
											VarianceKind.In => Conversions.InvertConversionRequirement(Conversions.StrengthenConversionRequirementToReference(inferenceRestrictions)), 
											VarianceKind.Out => Conversions.StrengthenConversionRequirementToReference(inferenceRestrictions), 
											_ => RequiredConversion.Identity, 
										};
										if (!InferTypeArgumentsFromArgument(argumentLocation, namedTypeSymbol2.TypeArgumentWithDefinitionUseSiteDiagnostics(j, ref UseSiteInfo), argumentTypeByAssumption, namedTypeSymbol.TypeArgumentWithDefinitionUseSiteDiagnostics(j, ref UseSiteInfo), param, requiredConversion switch
										{
											RequiredConversion.Reference => MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, 
											RequiredConversion.ReverseReference => MatchGenericArgumentToParameter.MatchArgumentToBaseOfGenericParameter, 
											_ => MatchGenericArgumentToParameter.MatchGenericArgumentToParameterExactly, 
										}, requiredConversion))
										{
											return false;
										}
									}
									namedTypeSymbol = namedTypeSymbol.ContainingType;
									namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
								}
								while ((object)namedTypeSymbol != null);
								return true;
							}
						}
						else if (TypeSymbolExtensions.IsNullableType(namedTypeSymbol))
						{
							return InferTypeArgumentsFromArgument(argumentLocation, argumentType, argumentTypeByAssumption, TypeSymbolExtensions.GetNullableUnderlyingType(namedTypeSymbol), param, digThroughToBasesAndImplements, Conversions.CombineConversionRequirements(inferenceRestrictions, RequiredConversion.ArrayElement));
						}
						return false;
					}
				}
				else if (TypeSymbolExtensions.IsArrayType(parameterType))
				{
					if (TypeSymbolExtensions.IsArrayType(argumentType))
					{
						ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)parameterType;
						ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)argumentType;
						bool flag = arrayTypeSymbol2 is ArrayLiteralTypeSymbol;
						if (arrayTypeSymbol.Rank == arrayTypeSymbol2.Rank && (flag || arrayTypeSymbol.IsSZArray == arrayTypeSymbol2.IsSZArray))
						{
							return InferTypeArgumentsFromArgument(argumentLocation, arrayTypeSymbol2.ElementType, argumentTypeByAssumption, arrayTypeSymbol.ElementType, param, digThroughToBasesAndImplements, Conversions.CombineConversionRequirements(inferenceRestrictions, flag ? RequiredConversion.Any : RequiredConversion.ArrayElement));
						}
					}
					return false;
				}
				return true;
			}

			internal bool InferTypeArgumentsFromArgument(SyntaxNode argumentLocation, TypeSymbol argumentType, bool argumentTypeByAssumption, TypeSymbol parameterType, ParameterSymbol param, MatchGenericArgumentToParameter digThroughToBasesAndImplements, RequiredConversion inferenceRestrictions)
			{
				if (!RefersToGenericParameterToInferArgumentFor(parameterType))
				{
					return true;
				}
				if (InferTypeArgumentsFromArgumentDirectly(argumentLocation, argumentType, argumentTypeByAssumption, parameterType, param, digThroughToBasesAndImplements, inferenceRestrictions))
				{
					return true;
				}
				if (TypeSymbolExtensions.IsTypeParameter(parameterType))
				{
					return false;
				}
				if (digThroughToBasesAndImplements == MatchGenericArgumentToParameter.MatchGenericArgumentToParameterExactly)
				{
					return false;
				}
				if ((object)argumentType != null && TypeSymbolExtensions.IsDelegateType(argumentType) && TypeSymbolExtensions.IsDelegateType(parameterType) && digThroughToBasesAndImplements == MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter && (inferenceRestrictions == RequiredConversion.Any || inferenceRestrictions == RequiredConversion.AnyReverse || inferenceRestrictions == RequiredConversion.AnyAndReverse))
				{
					NamedTypeSymbol obj = (NamedTypeSymbol)argumentType;
					MethodSymbol delegateInvokeMethod = obj.DelegateInvokeMethod;
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)parameterType;
					MethodSymbol delegateInvokeMethod2 = namedTypeSymbol.DelegateInvokeMethod;
					if (obj.IsAnonymousType && !namedTypeSymbol.IsAnonymousType && (object)delegateInvokeMethod2 != null && delegateInvokeMethod2.GetUseSiteInfo().DiagnosticInfo == null)
					{
						ImmutableArray<ParameterSymbol> parameters = delegateInvokeMethod.Parameters;
						ImmutableArray<ParameterSymbol> parameters2 = delegateInvokeMethod2.Parameters;
						if (parameters2.Length != parameters.Length && parameters.Length != 0)
						{
							return false;
						}
						int num = parameters.Length - 1;
						for (int i = 0; i <= num; i++)
						{
							if (parameters[i].IsByRef != parameters2[i].IsByRef)
							{
								return false;
							}
							if (!InferTypeArgumentsFromArgument(argumentLocation, parameters[i].Type, argumentTypeByAssumption, parameters2[i].Type, param, MatchGenericArgumentToParameter.MatchArgumentToBaseOfGenericParameter, RequiredConversion.AnyReverse))
							{
								return false;
							}
						}
						if (delegateInvokeMethod2.IsSub)
						{
							return true;
						}
						if (delegateInvokeMethod.IsSub)
						{
							return false;
						}
						return InferTypeArgumentsFromArgument(argumentLocation, delegateInvokeMethod.ReturnType, argumentTypeByAssumption, delegateInvokeMethod2.ReturnType, param, MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, RequiredConversion.Any);
					}
				}
				bool flag = false;
				if (!((digThroughToBasesAndImplements != 0) ? FindMatchingBase(ref parameterType, ref argumentType) : FindMatchingBase(ref argumentType, ref parameterType)))
				{
					return false;
				}
				return InferTypeArgumentsFromArgumentDirectly(argumentLocation, argumentType, argumentTypeByAssumption, parameterType, param, digThroughToBasesAndImplements, inferenceRestrictions);
			}

			private bool FindMatchingBase(ref TypeSymbol baseSearchType, ref TypeSymbol fixedType)
			{
				NamedTypeSymbol namedTypeSymbol = ((fixedType.Kind == SymbolKind.NamedType) ? ((NamedTypeSymbol)fixedType) : null);
				if ((object)namedTypeSymbol == null || !namedTypeSymbol.IsGenericType)
				{
					return false;
				}
				TypeKind typeKind = fixedType.TypeKind;
				if (typeKind != TypeKind.Class && typeKind != TypeKind.Interface)
				{
					return false;
				}
				SymbolKind kind = baseSearchType.Kind;
				if (kind != SymbolKind.NamedType && kind != SymbolKind.TypeParameter && (kind != SymbolKind.ArrayType || !((ArrayTypeSymbol)baseSearchType).IsSZArray))
				{
					return false;
				}
				if (TypeSymbolExtensions.IsSameTypeIgnoringAll(baseSearchType, fixedType))
				{
					return false;
				}
				TypeSymbol match = null;
				if (typeKind == TypeKind.Class)
				{
					FindMatchingBaseClass(baseSearchType, fixedType, ref match);
				}
				else
				{
					FindMatchingBaseInterface(baseSearchType, fixedType, ref match);
				}
				if ((object)match == null)
				{
					return false;
				}
				baseSearchType = match;
				return true;
			}

			private static bool SetMatchIfNothingOrEqual(TypeSymbol type, ref TypeSymbol match)
			{
				if ((object)match == null)
				{
					match = type;
					return true;
				}
				if (TypeSymbolExtensions.IsSameTypeIgnoringAll(match, type))
				{
					return true;
				}
				match = null;
				return false;
			}

			private bool FindMatchingBaseInterface(TypeSymbol derivedType, TypeSymbol baseInterface, ref TypeSymbol match)
			{
				SymbolKind kind = derivedType.Kind;
				if (kind == SymbolKind.TypeParameter)
				{
					ImmutableArray<TypeSymbol>.Enumerator enumerator = ((TypeParameterSymbol)derivedType).ConstraintTypesWithDefinitionUseSiteDiagnostics(ref UseSiteInfo).GetEnumerator();
					while (enumerator.MoveNext())
					{
						TypeSymbol current = enumerator.Current;
						if (TypeSymbolExtensions.IsSameTypeIgnoringAll(current.OriginalDefinition, baseInterface.OriginalDefinition) && !SetMatchIfNothingOrEqual(current, ref match))
						{
							return false;
						}
						if (!FindMatchingBaseInterface(current, baseInterface, ref match))
						{
							return false;
						}
					}
				}
				else
				{
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = derivedType.AllInterfacesWithDefinitionUseSiteDiagnostics(ref UseSiteInfo).GetEnumerator();
					while (enumerator2.MoveNext())
					{
						NamedTypeSymbol current2 = enumerator2.Current;
						if (TypeSymbolExtensions.IsSameTypeIgnoringAll(current2.OriginalDefinition, baseInterface.OriginalDefinition) && !SetMatchIfNothingOrEqual(current2, ref match))
						{
							return false;
						}
					}
				}
				return true;
			}

			private bool FindMatchingBaseClass(TypeSymbol derivedType, TypeSymbol baseClass, ref TypeSymbol match)
			{
				SymbolKind kind = derivedType.Kind;
				if (kind == SymbolKind.TypeParameter)
				{
					ImmutableArray<TypeSymbol>.Enumerator enumerator = ((TypeParameterSymbol)derivedType).ConstraintTypesWithDefinitionUseSiteDiagnostics(ref UseSiteInfo).GetEnumerator();
					while (enumerator.MoveNext())
					{
						TypeSymbol current = enumerator.Current;
						if (TypeSymbolExtensions.IsSameTypeIgnoringAll(current.OriginalDefinition, baseClass.OriginalDefinition) && !SetMatchIfNothingOrEqual(current, ref match))
						{
							return false;
						}
						if (!FindMatchingBaseClass(current, baseClass, ref match))
						{
							return false;
						}
					}
				}
				else
				{
					NamedTypeSymbol namedTypeSymbol = derivedType.BaseTypeWithDefinitionUseSiteDiagnostics(ref UseSiteInfo);
					while ((object)namedTypeSymbol != null)
					{
						if (TypeSymbolExtensions.IsSameTypeIgnoringAll(namedTypeSymbol.OriginalDefinition, baseClass.OriginalDefinition))
						{
							if (SetMatchIfNothingOrEqual(namedTypeSymbol, ref match))
							{
								break;
							}
							return false;
						}
						namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref UseSiteInfo);
					}
				}
				return true;
			}

			public bool InferTypeArgumentsFromAddressOfArgument(BoundExpression argument, TypeSymbol parameterType, ParameterSymbol param)
			{
				if (TypeSymbolExtensions.IsDelegateType(parameterType))
				{
					MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)ConstructParameterTypeIfNeeded(parameterType)).DelegateInvokeMethod;
					if ((object)delegateInvokeMethod == null || delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo != null)
					{
						return false;
					}
					TypeSymbol returnType = delegateInvokeMethod.ReturnType;
					if (!RefersToGenericParameterToInferArgumentFor(returnType))
					{
						return true;
					}
					BoundAddressOfOperator boundAddressOfOperator = (BoundAddressOfOperator)argument;
					MethodSymbol methodSymbol = null;
					MethodConversionKind methodConversionKind = MethodConversionKind.Identity;
					KeyValuePair<MethodSymbol, MethodConversionKind> keyValuePair = Binder.ResolveMethodForDelegateInvokeFullAndRelaxed(boundAddressOfOperator, delegateInvokeMethod, ignoreMethodReturnType: true, BindingDiagnosticBag.Discarded);
					methodSymbol = keyValuePair.Key;
					methodConversionKind = keyValuePair.Value;
					if ((object)methodSymbol == null || (methodConversionKind & MethodConversionKind.AllErrorReasons) != 0 || (boundAddressOfOperator.Binder.OptionStrict == OptionStrict.On && Conversions.IsNarrowingMethodConversion(methodConversionKind, isForAddressOf: true)))
					{
						return false;
					}
					if (methodSymbol.IsSub)
					{
						ReportNotFailedInferenceDueToObject();
						return true;
					}
					TypeSymbol returnType2 = methodSymbol.ReturnType;
					if (RefersToGenericParameterToInferArgumentFor(returnType2))
					{
						return false;
					}
					return InferTypeArgumentsFromArgument(argument.Syntax, returnType2, argumentTypeByAssumption: false, returnType, param, MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, RequiredConversion.Any);
				}
				ReportNotFailedInferenceDueToObject();
				return true;
			}

			public bool InferTypeArgumentsFromLambdaArgument(BoundExpression argument, TypeSymbol parameterType, ParameterSymbol param)
			{
				if (TypeSymbolExtensions.IsTypeParameter(parameterType))
				{
					TypeSymbol typeSymbol = null;
					switch (argument.Kind)
					{
					case BoundKind.UnboundLambda:
					{
						KeyValuePair<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> inferredAnonymousDelegate = ((UnboundLambda)argument).InferredAnonymousDelegate;
						if (inferredAnonymousDelegate.Value.Diagnostics.IsDefault || !inferredAnonymousDelegate.Value.Diagnostics.HasAnyErrors())
						{
							MethodSymbol methodSymbol = null;
							if ((object)inferredAnonymousDelegate.Key != null)
							{
								methodSymbol = inferredAnonymousDelegate.Key.DelegateInvokeMethod;
							}
							if ((object)methodSymbol != null && (object)methodSymbol.ReturnType != LambdaSymbol.ReturnTypeIsUnknown)
							{
								typeSymbol = inferredAnonymousDelegate.Key;
							}
						}
						break;
					}
					default:
						throw ExceptionUtilities.UnexpectedValue(argument.Kind);
					case BoundKind.QueryLambda:
					case BoundKind.GroupTypeInferenceLambda:
						break;
					}
					if ((object)typeSymbol != null)
					{
						return InferTypeArgumentsFromArgument(argument.Syntax, typeSymbol, argumentTypeByAssumption: false, parameterType, param, MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, RequiredConversion.Any);
					}
					return true;
				}
				if (TypeSymbolExtensions.IsDelegateType(parameterType))
				{
					NamedTypeSymbol parameterType2 = (NamedTypeSymbol)parameterType;
					MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)ConstructParameterTypeIfNeeded(parameterType2)).DelegateInvokeMethod;
					if ((object)delegateInvokeMethod == null || delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo != null)
					{
						return true;
					}
					TypeSymbol typeSymbol2 = delegateInvokeMethod.ReturnType;
					if (!RefersToGenericParameterToInferArgumentFor(typeSymbol2))
					{
						return true;
					}
					ImmutableArray<ParameterSymbol> immutableArray = argument.Kind switch
					{
						BoundKind.QueryLambda => ((BoundQueryLambda)argument).LambdaSymbol.Parameters, 
						BoundKind.GroupTypeInferenceLambda => ((GroupTypeInferenceLambda)argument).Parameters, 
						BoundKind.UnboundLambda => ((UnboundLambda)argument).Parameters, 
						_ => throw ExceptionUtilities.UnexpectedValue(argument.Kind), 
					};
					ImmutableArray<ParameterSymbol> parameters = delegateInvokeMethod.Parameters;
					if (immutableArray.Length > parameters.Length)
					{
						return true;
					}
					int num = immutableArray.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						ParameterSymbol parameterSymbol = immutableArray[i];
						ParameterSymbol parameterSymbol2 = parameters[i];
						if ((object)parameterSymbol.Type == null)
						{
							if (!RefersToGenericParameterToInferArgumentFor(parameterSymbol2.Type))
							{
							}
						}
						else
						{
							InferTypeArgumentsFromArgument(argument.Syntax, parameterSymbol.Type, argumentTypeByAssumption: false, parameterSymbol2.Type, param, MatchGenericArgumentToParameter.MatchArgumentToBaseOfGenericParameter, RequiredConversion.Any);
						}
					}
					TypeSymbol typeSymbol3;
					switch (argument.Kind)
					{
					case BoundKind.QueryLambda:
					{
						BoundQueryLambda boundQueryLambda = (BoundQueryLambda)argument;
						typeSymbol3 = boundQueryLambda.LambdaSymbol.ReturnType;
						if ((object)typeSymbol3 != LambdaSymbol.ReturnTypePendingDelegate)
						{
							break;
						}
						typeSymbol3 = boundQueryLambda.Expression.Type;
						if ((object)typeSymbol3 == null)
						{
							if (Diagnostic == null)
							{
								Diagnostic = BindingDiagnosticBag.Create(withDiagnostics: true, UseSiteInfo.AccumulatesDependencies);
							}
							typeSymbol3 = boundQueryLambda.LambdaSymbol.ContainingBinder.MakeRValue(boundQueryLambda.Expression, Diagnostic).Type;
						}
						break;
					}
					case BoundKind.GroupTypeInferenceLambda:
						typeSymbol3 = ((GroupTypeInferenceLambda)argument).InferLambdaReturnType(parameters);
						break;
					case BoundKind.UnboundLambda:
					{
						UnboundLambda unboundLambda = (UnboundLambda)argument;
						if (unboundLambda.IsFunctionLambda)
						{
							UnboundLambda.TargetSignature targetSignature = new UnboundLambda.TargetSignature(parameters, unboundLambda.Binder.Compilation.GetSpecialType(SpecialType.System_Void), returnsByRef: false);
							KeyValuePair<TypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> keyValuePair = unboundLambda.InferReturnType(targetSignature);
							if (!keyValuePair.Value.Diagnostics.IsDefault && keyValuePair.Value.Diagnostics.HasAnyErrors())
							{
								typeSymbol3 = null;
								if (Diagnostic == null)
								{
									Diagnostic = BindingDiagnosticBag.Create(withDiagnostics: true, UseSiteInfo.AccumulatesDependencies);
								}
								Diagnostic.AddRange(keyValuePair.Value);
							}
							else if ((object)keyValuePair.Key == LambdaSymbol.ReturnTypeIsUnknown)
							{
								typeSymbol3 = null;
							}
							else
							{
								BoundLambda boundLambda = unboundLambda.Bind(new UnboundLambda.TargetSignature(targetSignature.ParameterTypes, targetSignature.ParameterIsByRef, keyValuePair.Key, returnsByRef: false));
								if (!boundLambda.HasErrors && !boundLambda.Diagnostics.Diagnostics.HasAnyErrors())
								{
									typeSymbol3 = keyValuePair.Key;
									if (Diagnostic == null)
									{
										Diagnostic = BindingDiagnosticBag.Create(withDiagnostics: true, UseSiteInfo.AccumulatesDependencies);
									}
									Diagnostic.AddRange(keyValuePair.Value);
									Diagnostic.AddDependencies(boundLambda.Diagnostics.Dependencies);
								}
								else
								{
									typeSymbol3 = null;
									if (!boundLambda.Diagnostics.Diagnostics.IsDefaultOrEmpty)
									{
										if (Diagnostic == null)
										{
											Diagnostic = BindingDiagnosticBag.Create(withDiagnostics: true, UseSiteInfo.AccumulatesDependencies);
										}
										Diagnostic.AddRange(boundLambda.Diagnostics);
									}
								}
							}
							if ((unboundLambda.Flags & (SourceMemberFlags.Async | SourceMemberFlags.Iterator)) != 0 && (object)typeSymbol3 != null && typeSymbol3.Kind == SymbolKind.NamedType && (object)typeSymbol2 != null && typeSymbol2.Kind == SymbolKind.NamedType)
							{
								NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)typeSymbol3;
								NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)typeSymbol2;
								if (namedTypeSymbol.Arity == 1 && TypeSymbolExtensions.IsSameTypeIgnoringAll(namedTypeSymbol.OriginalDefinition, namedTypeSymbol2.OriginalDefinition))
								{
									typeSymbol3 = namedTypeSymbol.TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref UseSiteInfo);
									typeSymbol2 = namedTypeSymbol2.TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref UseSiteInfo);
								}
							}
							break;
						}
						typeSymbol3 = null;
						if (delegateInvokeMethod.IsSub || (unboundLambda.Flags & SourceMemberFlags.Async) == 0)
						{
							break;
						}
						BoundLambda boundLambda2 = unboundLambda.Bind(new UnboundLambda.TargetSignature(parameters, unboundLambda.Binder.Compilation.GetSpecialType(SpecialType.System_Void), returnsByRef: false));
						if (!boundLambda2.HasErrors && !boundLambda2.Diagnostics.Diagnostics.HasAnyErrors())
						{
							if (_asyncLambdaSubToFunctionMismatch == null)
							{
								_asyncLambdaSubToFunctionMismatch = new HashSet<BoundExpression>(ReferenceEqualityComparer.Instance);
							}
							_asyncLambdaSubToFunctionMismatch.Add(unboundLambda);
						}
						break;
					}
					default:
						throw ExceptionUtilities.UnexpectedValue(argument.Kind);
					}
					if ((object)typeSymbol3 == null)
					{
						return false;
					}
					if (TypeSymbolExtensions.IsErrorType(typeSymbol3))
					{
						return true;
					}
					return InferTypeArgumentsFromArgument(argument.Syntax, typeSymbol3, argumentTypeByAssumption: false, typeSymbol2, param, MatchGenericArgumentToParameter.MatchBaseOfGenericArgumentToParameter, RequiredConversion.Any);
				}
				if (TypeSymbol.Equals(parameterType.OriginalDefinition, BoundNodeExtensions.GetBinderFromLambda(argument).Compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T), TypeCompareKind.ConsiderEverything))
				{
					return InferTypeArgumentsFromLambdaArgument(argument, ((NamedTypeSymbol)parameterType).TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref UseSiteInfo), param);
				}
				return true;
			}

			public TypeSymbol ConstructParameterTypeIfNeeded(TypeSymbol parameterType)
			{
				MethodSymbol candidate = Candidate;
				ArrayBuilder<TypeWithModifiers> instance = ArrayBuilder<TypeWithModifiers>.GetInstance(_typeParameterNodes.Length);
				int num = _typeParameterNodes.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					TypeParameterNode typeParameterNode = _typeParameterNodes[i];
					TypeSymbol type = ((typeParameterNode != null && (object)typeParameterNode.CandidateInferredType != null) ? typeParameterNode.CandidateInferredType : candidate.TypeParameters[i]);
					instance.Add(new TypeWithModifiers(type));
				}
				TypeSubstitution substitution = TypeSubstitution.CreateAdditionalMethodTypeParameterSubstitution(candidate.ConstructedFrom, instance.ToImmutableAndFree());
				return parameterType.InternalSubstituteTypeParameters(substitution).Type;
			}

			public void ReportAmbiguousInferenceError(ArrayBuilder<DominantTypeDataTypeInference> typeInfos)
			{
				int num = typeInfos.Count - 1;
				for (int i = 1; i <= num; i++)
				{
					if (!typeInfos[i].InferredFromObject)
					{
						ReportNotFailedInferenceDueToObject();
					}
				}
			}

			public void ReportIncompatibleInferenceError(ArrayBuilder<DominantTypeDataTypeInference> typeInfos)
			{
				if (typeInfos.Count < 1)
				{
					return;
				}
				int num = typeInfos.Count - 1;
				for (int i = 1; i <= num; i++)
				{
					if (!typeInfos[i].InferredFromObject)
					{
						ReportNotFailedInferenceDueToObject();
					}
				}
			}

			public void RegisterErrorReasons(InferenceErrorReasons inferenceErrorReasons)
			{
				_inferenceErrorReasons |= inferenceErrorReasons;
			}
		}

		public static bool Infer(MethodSymbol candidate, ImmutableArray<BoundExpression> arguments, ArrayBuilder<int> parameterToArgumentMap, ArrayBuilder<int> paramArrayItems, TypeSymbol delegateReturnType, BoundNode delegateReturnTypeReferenceBoundNode, ref ImmutableArray<TypeSymbol> typeArguments, ref InferenceLevel inferenceLevel, ref bool allFailedInferenceIsDueToObject, ref bool someInferenceFailed, ref InferenceErrorReasons inferenceErrorReasons, out BitVector inferredTypeByAssumption, out ImmutableArray<SyntaxNodeOrToken> typeArgumentsLocation, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BindingDiagnosticBag diagnostic, BitVector inferTheseTypeParameters = default(BitVector))
		{
			return InferenceGraph.Infer(candidate, arguments, parameterToArgumentMap, paramArrayItems, delegateReturnType, delegateReturnTypeReferenceBoundNode, ref typeArguments, ref inferenceLevel, ref allFailedInferenceIsDueToObject, ref someInferenceFailed, ref inferenceErrorReasons, out inferredTypeByAssumption, out typeArgumentsLocation, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo, ref diagnostic, inferTheseTypeParameters);
		}

		private TypeArgumentInference()
		{
		}
	}
}
