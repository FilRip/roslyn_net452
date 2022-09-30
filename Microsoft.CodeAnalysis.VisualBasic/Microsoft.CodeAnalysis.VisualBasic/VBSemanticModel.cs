using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class VBSemanticModel : SemanticModel
	{
		internal enum SymbolInfoOptions
		{
			PreferTypeToConstructors = 1,
			PreferConstructorsToType = 2,
			ResolveAliases = 4,
			PreserveAliases = 8,
			DefaultOptions = 6
		}

		public new abstract VisualBasicCompilation Compilation { get; }

		internal new abstract SyntaxNode Root { get; }

		public new abstract SemanticModel ParentModel { get; }

		public new abstract SyntaxTree SyntaxTree { get; }

		public OptionStrict OptionStrict => GetEnclosingBinder(Root.SpanStart).OptionStrict;

		public bool OptionInfer => GetEnclosingBinder(Root.SpanStart).OptionInfer;

		public bool OptionExplicit => GetEnclosingBinder(Root.SpanStart).OptionExplicit;

		public bool OptionCompareText => GetEnclosingBinder(Root.SpanStart).OptionCompareText;

		public sealed override string Language => "Visual Basic";

		protected sealed override SemanticModel ParentModelCore => ParentModel;

		protected sealed override SyntaxTree SyntaxTreeCore => SyntaxTree;

		protected sealed override Compilation CompilationCore => Compilation;

		protected sealed override SyntaxNode RootCore => Root;

		public CollectionRangeVariableSymbolInfo GetCollectionRangeVariableSymbolInfo(CollectionRangeVariableSyntax variableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (variableSyntax == null)
			{
				throw new ArgumentNullException("variableSyntax");
			}
			if (!IsInTree(variableSyntax))
			{
				throw new ArgumentException(VBResources.VariableSyntaxNotWithinSyntaxTree);
			}
			return GetCollectionRangeVariableSymbolInfoWorker(variableSyntax, cancellationToken);
		}

		internal abstract CollectionRangeVariableSymbolInfo GetCollectionRangeVariableSymbolInfoWorker(CollectionRangeVariableSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		public AggregateClauseSymbolInfo GetAggregateClauseSymbolInfo(AggregateClauseSyntax aggregateSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (aggregateSyntax == null)
			{
				throw new ArgumentNullException("aggregateSyntax");
			}
			if (!IsInTree(aggregateSyntax))
			{
				throw new ArgumentException(VBResources.AggregateSyntaxNotWithinSyntaxTree);
			}
			if (aggregateSyntax.Parent == null || (aggregateSyntax.Parent.Kind() == SyntaxKind.QueryExpression && ((QueryExpressionSyntax)aggregateSyntax.Parent).Clauses.FirstOrDefault() == aggregateSyntax))
			{
				return new AggregateClauseSymbolInfo(SymbolInfo.None);
			}
			return GetAggregateClauseSymbolInfoWorker(aggregateSyntax, cancellationToken);
		}

		internal abstract AggregateClauseSymbolInfo GetAggregateClauseSymbolInfoWorker(AggregateClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		public SymbolInfo GetSymbolInfo(QueryClauseSyntax clauseSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(clauseSyntax);
			if (CanGetSemanticInfo(clauseSyntax))
			{
				switch (clauseSyntax.Kind())
				{
				case SyntaxKind.LetClause:
				case SyntaxKind.OrderByClause:
					return SymbolInfo.None;
				case SyntaxKind.AggregateClause:
					return SymbolInfo.None;
				default:
					return GetQueryClauseSymbolInfo(clauseSyntax, cancellationToken);
				}
			}
			return SymbolInfo.None;
		}

		internal abstract SymbolInfo GetQueryClauseSymbolInfo(QueryClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		public SymbolInfo GetSymbolInfo(ExpressionRangeVariableSyntax variableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(variableSyntax);
			if (CanGetSemanticInfo(variableSyntax))
			{
				if (variableSyntax.Parent == null || variableSyntax.Parent.Kind() != SyntaxKind.LetClause)
				{
					return SymbolInfo.None;
				}
				return GetLetClauseSymbolInfo(variableSyntax, cancellationToken);
			}
			return SymbolInfo.None;
		}

		internal abstract SymbolInfo GetLetClauseSymbolInfo(ExpressionRangeVariableSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		public SymbolInfo GetSymbolInfo(FunctionAggregationSyntax functionSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(functionSyntax);
			if (CanGetSemanticInfo(functionSyntax))
			{
				if (!IsInTree(functionSyntax))
				{
					throw new ArgumentException(VBResources.FunctionSyntaxNotWithinSyntaxTree);
				}
				return GetSymbolInfo((ExpressionSyntax)functionSyntax, cancellationToken);
			}
			return SymbolInfo.None;
		}

		public SymbolInfo GetSymbolInfo(OrderingSyntax orderingSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(orderingSyntax);
			if (CanGetSemanticInfo(orderingSyntax))
			{
				return GetOrderingSymbolInfo(orderingSyntax, cancellationToken);
			}
			return SymbolInfo.None;
		}

		internal abstract SymbolInfo GetOrderingSymbolInfo(OrderingSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract SymbolInfo GetExpressionSymbolInfo(ExpressionSyntax node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract SymbolInfo GetCollectionInitializerAddSymbolInfo(ObjectCreationExpressionSyntax collectionInitializer, ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract SymbolInfo GetAttributeSymbolInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract VisualBasicTypeInfo GetExpressionTypeInfo(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract VisualBasicTypeInfo GetAttributeTypeInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract ConstantValue GetExpressionConstantValue(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract ImmutableArray<Symbol> GetExpressionMemberGroup(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract ImmutableArray<Symbol> GetAttributeMemberGroup(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract SymbolInfo GetCrefReferenceSymbolInfo(CrefReferenceSyntax crefReference, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken));

		internal bool CanGetSemanticInfo(VisualBasicSyntaxNode node, bool allowNamedArgumentName = false)
		{
			if (node.Kind() == SyntaxKind.XmlName)
			{
				return false;
			}
			if (SyntaxNodeExtensions.EnclosingStructuredTrivia(node) != null)
			{
				return IsInCrefOrNameAttributeInterior(node);
			}
			return !node.IsMissing && ((node is ExpressionSyntax && (allowNamedArgumentName || !SyntaxFacts.IsNamedArgumentName(node))) || node is AttributeSyntax || node is QueryClauseSyntax || node is ExpressionRangeVariableSyntax || node is OrderingSyntax);
		}

		protected override IOperation GetOperationCore(SyntaxNode node, CancellationToken cancellationToken)
		{
			VisualBasicSyntaxNode node2 = (VisualBasicSyntaxNode)node;
			CheckSyntaxNode(node2);
			return GetOperationWorker(node2, cancellationToken);
		}

		internal virtual IOperation GetOperationWorker(VisualBasicSyntaxNode node, CancellationToken cancellationToken)
		{
			return null;
		}

		public SymbolInfo GetSymbolInfo(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(expression);
			if (CanGetSemanticInfo(expression, allowNamedArgumentName: true))
			{
				if (SyntaxFacts.IsNamedArgumentName(expression))
				{
					return GetNamedArgumentSymbolInfo((IdentifierNameSyntax)expression, cancellationToken);
				}
				return GetExpressionSymbolInfo(expression, SymbolInfoOptions.DefaultOptions, cancellationToken);
			}
			return SymbolInfo.None;
		}

		public SymbolInfo GetCollectionInitializerSymbolInfo(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(expression);
			if (expression.Parent != null && expression.Parent.Kind() == SyntaxKind.CollectionInitializer && expression.Parent.Parent != null && expression.Parent.Parent.Kind() == SyntaxKind.ObjectCollectionInitializer && ((ObjectCollectionInitializerSyntax)expression.Parent.Parent).Initializer == expression.Parent && expression.Parent.Parent.Parent != null && expression.Parent.Parent.Parent.Kind() == SyntaxKind.ObjectCreationExpression && CanGetSemanticInfo(expression.Parent.Parent.Parent))
			{
				ObjectCreationExpressionSyntax objectCreationExpressionSyntax = (ObjectCreationExpressionSyntax)expression.Parent.Parent.Parent;
				if (objectCreationExpressionSyntax.Initializer == expression.Parent.Parent)
				{
					return GetCollectionInitializerAddSymbolInfo(objectCreationExpressionSyntax, expression, cancellationToken);
				}
			}
			return SymbolInfo.None;
		}

		public SymbolInfo GetSymbolInfo(CrefReferenceSyntax crefReference, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(crefReference);
			return GetCrefReferenceSymbolInfo(crefReference, SymbolInfoOptions.DefaultOptions, cancellationToken);
		}

		public SymbolInfo GetSpeculativeSymbolInfo(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			Binder binder = null;
			BoundNodeSummary speculativelyBoundNodeSummary = GetSpeculativelyBoundNodeSummary(position, expression, bindingOption, out binder);
			if (speculativelyBoundNodeSummary.LowestBoundNode != null)
			{
				return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, speculativelyBoundNodeSummary, binder);
			}
			return SymbolInfo.None;
		}

		public SymbolInfo GetSpeculativeSymbolInfo(int position, AttributeSyntax attribute)
		{
			Binder binder = null;
			BoundNodeSummary speculativelyBoundAttributeSummary = GetSpeculativelyBoundAttributeSummary(position, attribute, out binder);
			if (speculativelyBoundAttributeSummary.LowestBoundNode != null)
			{
				return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, speculativelyBoundAttributeSummary, binder);
			}
			return SymbolInfo.None;
		}

		public SymbolInfo GetSymbolInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(attribute);
			if (CanGetSemanticInfo(attribute))
			{
				return GetAttributeSymbolInfo(attribute, cancellationToken);
			}
			return SymbolInfo.None;
		}

		internal SymbolInfo GetSymbolInfoForNode(SymbolInfoOptions options, BoundNodeSummary boundNodes, Binder binderOpt)
		{
			LookupResultKind resultKind = LookupResultKind.Empty;
			ImmutableArray<Symbol> memberGroup = default(ImmutableArray<Symbol>);
			return SymbolInfoFactory.Create(GetSemanticSymbols(boundNodes, binderOpt, options, ref resultKind, ref memberGroup), resultKind);
		}

		public TypeInfo GetTypeInfo(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetTypeInfoWorker(expression, cancellationToken);
		}

		internal VisualBasicTypeInfo GetTypeInfoWorker(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(expression);
			if (CanGetSemanticInfo(expression))
			{
				if (SyntaxFacts.IsNamedArgumentName(expression))
				{
					return VisualBasicTypeInfo.None;
				}
				return GetExpressionTypeInfo(expression, cancellationToken);
			}
			return VisualBasicTypeInfo.None;
		}

		public TypeInfo GetSpeculativeTypeInfo(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			return GetSpeculativeTypeInfoWorker(position, expression, bindingOption);
		}

		internal VisualBasicTypeInfo GetSpeculativeTypeInfoWorker(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			Binder binder = null;
			BoundNodeSummary speculativelyBoundNodeSummary = GetSpeculativelyBoundNodeSummary(position, expression, bindingOption, out binder);
			if (speculativelyBoundNodeSummary.LowestBoundNode != null)
			{
				return GetTypeInfoForNode(speculativelyBoundNodeSummary);
			}
			return VisualBasicTypeInfo.None;
		}

		public TypeInfo GetTypeInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetTypeInfoWorker(attribute, cancellationToken);
		}

		private VisualBasicTypeInfo GetTypeInfoWorker(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(attribute);
			if (CanGetSemanticInfo(attribute))
			{
				return GetAttributeTypeInfo(attribute, cancellationToken);
			}
			return VisualBasicTypeInfo.None;
		}

		internal VisualBasicTypeInfo GetTypeInfoForNode(BoundNodeSummary boundNodes)
		{
			TypeSymbol typeSymbol = null;
			TypeSymbol convertedType = null;
			Conversion conversion = default(Conversion);
			typeSymbol = GetSemanticType(boundNodes, ref convertedType, ref conversion);
			return new VisualBasicTypeInfo(typeSymbol, convertedType, conversion);
		}

		public Conversion GetConversion(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (node is ExpressionSyntax expression)
			{
				return GetTypeInfoWorker(expression, cancellationToken).ImplicitConversion;
			}
			if (node is AttributeSyntax attribute)
			{
				return GetTypeInfoWorker(attribute, cancellationToken).ImplicitConversion;
			}
			return VisualBasicTypeInfo.None.ImplicitConversion;
		}

		public Conversion GetSpeculativeConversion(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			return GetSpeculativeTypeInfoWorker(position, expression, bindingOption).ImplicitConversion;
		}

		public Optional<object> GetConstantValue(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(expression);
			Optional<object> result;
			if (CanGetSemanticInfo(expression))
			{
				ConstantValue expressionConstantValue = GetExpressionConstantValue(expression, cancellationToken);
				if ((object)expressionConstantValue != null && !expressionConstantValue.IsBad)
				{
					result = new Optional<object>(RuntimeHelpers.GetObjectValue(expressionConstantValue.Value));
					goto IL_0041;
				}
			}
			result = default(Optional<object>);
			goto IL_0041;
			IL_0041:
			return result;
		}

		public Optional<object> GetSpeculativeConstantValue(int position, ExpressionSyntax expression)
		{
			Binder binder = null;
			BoundNodeSummary speculativelyBoundNodeSummary = GetSpeculativelyBoundNodeSummary(position, expression, SpeculativeBindingOption.BindAsExpression, out binder);
			Optional<object> result;
			if (speculativelyBoundNodeSummary.LowestBoundNode != null)
			{
				ConstantValue constantValueForNode = GetConstantValueForNode(speculativelyBoundNodeSummary);
				if ((object)constantValueForNode != null && !constantValueForNode.IsBad)
				{
					result = new Optional<object>(RuntimeHelpers.GetObjectValue(constantValueForNode.Value));
					goto IL_0045;
				}
			}
			result = default(Optional<object>);
			goto IL_0045;
			IL_0045:
			return result;
		}

		internal ConstantValue GetConstantValueForNode(BoundNodeSummary boundNodes)
		{
			ConstantValue result = null;
			if (boundNodes.LowestBoundNode is BoundExpression boundExpression)
			{
				result = boundExpression.ConstantValueOpt;
			}
			return result;
		}

		public ImmutableArray<ISymbol> GetMemberGroup(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(expression);
			if (CanGetSemanticInfo(expression))
			{
				return StaticCast<ISymbol>.From(GetExpressionMemberGroup(expression, cancellationToken));
			}
			return ImmutableArray<ISymbol>.Empty;
		}

		public ImmutableArray<ISymbol> GetSpeculativeMemberGroup(int position, ExpressionSyntax expression)
		{
			Binder binder = null;
			BoundNodeSummary speculativelyBoundNodeSummary = GetSpeculativelyBoundNodeSummary(position, expression, SpeculativeBindingOption.BindAsExpression, out binder);
			if (speculativelyBoundNodeSummary.LowestBoundNode != null)
			{
				return StaticCast<ISymbol>.From(GetMemberGroupForNode(speculativelyBoundNodeSummary, null));
			}
			return ImmutableArray<ISymbol>.Empty;
		}

		public ImmutableArray<ISymbol> GetMemberGroup(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(attribute);
			if (CanGetSemanticInfo(attribute))
			{
				return StaticCast<ISymbol>.From(GetAttributeMemberGroup(attribute, cancellationToken));
			}
			return ImmutableArray<ISymbol>.Empty;
		}

		internal ImmutableArray<Symbol> GetMemberGroupForNode(BoundNodeSummary boundNodes, Binder binderOpt)
		{
			LookupResultKind resultKind = LookupResultKind.Empty;
			ImmutableArray<Symbol> memberGroup = default(ImmutableArray<Symbol>);
			GetSemanticSymbols(boundNodes, binderOpt, SymbolInfoOptions.DefaultOptions, ref resultKind, ref memberGroup);
			return memberGroup;
		}

		public IAliasSymbol GetAliasInfo(IdentifierNameSyntax nameSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(nameSyntax);
			if (CanGetSemanticInfo(nameSyntax))
			{
				return GetExpressionSymbolInfo(nameSyntax, (SymbolInfoOptions)9, cancellationToken).Symbol as IAliasSymbol;
			}
			return null;
		}

		public IAliasSymbol GetSpeculativeAliasInfo(int position, IdentifierNameSyntax nameSyntax, SpeculativeBindingOption bindingOption)
		{
			Binder binder = null;
			BoundNodeSummary speculativelyBoundNodeSummary = GetSpeculativelyBoundNodeSummary(position, nameSyntax, bindingOption, out binder);
			if (speculativelyBoundNodeSummary.LowestBoundNode != null)
			{
				return GetSymbolInfoForNode((SymbolInfoOptions)9, speculativelyBoundNodeSummary, binder).Symbol as IAliasSymbol;
			}
			return null;
		}

		internal abstract Binder GetEnclosingBinder(int position);

		internal bool IsInTree(SyntaxNode node)
		{
			return IsUnderNode(node, Root);
		}

		private static bool IsUnderNode(SyntaxNode node, SyntaxNode root)
		{
			while (node != null)
			{
				if (node == root)
				{
					return true;
				}
				node = ((!node.IsStructuredTrivia) ? node.Parent : ((StructuredTriviaSyntax)node).ParentTrivia.Token.Parent);
			}
			return false;
		}

		protected void CheckPosition(int position)
		{
			int position2 = Root.Position;
			int endPosition = Root.EndPosition;
			bool flag = position == endPosition && position == SyntaxTree.GetRoot().FullSpan.End;
			if ((position2 > position || position >= endPosition) && !flag && (position2 != endPosition || position != endPosition))
			{
				throw new ArgumentException(VBResources.PositionIsNotWithinSyntax);
			}
		}

		internal void CheckSyntaxNode(VisualBasicSyntaxNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (!IsInTree(node))
			{
				throw new ArgumentException(VBResources.NodeIsNotWithinSyntaxTree);
			}
		}

		private void CheckModelAndSyntaxNodeToSpeculate(VisualBasicSyntaxNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (IsSpeculativeSemanticModel)
			{
				throw new InvalidOperationException(VBResources.ChainingSpeculativeModelIsNotSupported);
			}
			if (Compilation.ContainsSyntaxTree(node.SyntaxTree))
			{
				throw new ArgumentException(VBResources.SpeculatedSyntaxNodeCannotBelongToCurrentCompilation);
			}
		}

		internal SyntaxNode FindInitialNodeFromPosition(int position)
		{
			int position2 = Root.Position;
			int endPosition = Root.EndPosition;
			bool flag = position == endPosition && position == SyntaxTree.GetRoot().FullSpan.End;
			if ((position2 <= position && position < endPosition) || flag)
			{
				SyntaxToken syntaxToken = ((!flag) ? Root.FindToken(position, findInsideTrivia: true) : SyntaxTree.GetRoot().FindToken(position, findInsideTrivia: true));
				if (SyntaxNodeExtensions.EnclosingStructuredTrivia((VisualBasicSyntaxNode)syntaxToken.Parent) == null || !IsInCrefOrNameAttributeInterior((VisualBasicSyntaxNode)syntaxToken.Parent))
				{
					syntaxToken = ((!flag) ? Root.FindToken(position) : SyntaxTree.GetRoot().FindToken(position));
				}
				if (position < syntaxToken.SpanStart)
				{
					syntaxToken = syntaxToken.GetPreviousToken();
				}
				if (syntaxToken.SpanStart < position2)
				{
					return Root;
				}
				if (syntaxToken.Parent != null)
				{
					return (VisualBasicSyntaxNode)syntaxToken.Parent;
				}
				return Root;
			}
			if (position2 == endPosition && position == endPosition)
			{
				return Root;
			}
			throw ExceptionUtilities.Unreachable;
		}

		internal static bool IsInCrefOrNameAttributeInterior(VisualBasicSyntaxNode node)
		{
			switch (node.Kind())
			{
			default:
				return false;
			case SyntaxKind.XmlString:
			case SyntaxKind.PredefinedType:
			case SyntaxKind.IdentifierName:
			case SyntaxKind.GenericName:
			case SyntaxKind.QualifiedName:
			case SyntaxKind.GlobalName:
			case SyntaxKind.CrefReference:
			case SyntaxKind.CrefOperatorReference:
			case SyntaxKind.QualifiedCrefOperatorReference:
			{
				VisualBasicSyntaxNode parent = node.Parent;
				bool flag = false;
				while (parent != null)
				{
					switch (parent.Kind())
					{
					case SyntaxKind.XmlCrefAttribute:
					case SyntaxKind.XmlNameAttribute:
						return true;
					case SyntaxKind.XmlAttribute:
						flag = true;
						parent = parent.Parent;
						break;
					case SyntaxKind.DocumentationCommentTrivia:
						if (flag)
						{
							return true;
						}
						parent = parent.Parent;
						break;
					default:
						parent = parent.Parent;
						break;
					}
				}
				return false;
			}
			}
		}

		internal SpeculativeBinder GetSpeculativeBinderForExpression(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			CheckPosition(position);
			if (bindingOption == SpeculativeBindingOption.BindAsTypeOrNamespace && !(expression is TypeSyntax))
			{
				return null;
			}
			Binder enclosingBinder = GetEnclosingBinder(position);
			return (enclosingBinder != null) ? SpeculativeBinder.Create(enclosingBinder) : null;
		}

		private BoundNode GetSpeculativelyBoundNode(Binder binder, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
		{
			if (bindingOption == SpeculativeBindingOption.BindAsTypeOrNamespace)
			{
				return binder.BindNamespaceOrTypeExpression((TypeSyntax)expression, BindingDiagnosticBag.Discarded);
			}
			BoundNode node = Bind(binder, expression, BindingDiagnosticBag.Discarded);
			return MakeValueIfPossible(binder, node);
		}

		internal BoundNode GetSpeculativelyBoundNode(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption, out Binder binder)
		{
			binder = GetSpeculativeBinderForExpression(position, expression, bindingOption);
			if (binder != null)
			{
				return GetSpeculativelyBoundNode(binder, expression, bindingOption);
			}
			return null;
		}

		private BoundNodeSummary GetSpeculativelyBoundNodeSummary(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption, out Binder binder)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			ExpressionSyntax standaloneExpression = SyntaxFactory.GetStandaloneExpression(expression);
			BoundNode speculativelyBoundNode = GetSpeculativelyBoundNode(position, standaloneExpression, bindingOption, out binder);
			return (speculativelyBoundNode == null) ? default(BoundNodeSummary) : new BoundNodeSummary(speculativelyBoundNode, speculativelyBoundNode, null);
		}

		private BoundNode MakeValueIfPossible(Binder binder, BoundNode node)
		{
			if (node is BoundExpression expr)
			{
				BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(DiagnosticBag.GetInstance());
				BoundExpression boundExpression = binder.ReclassifyAsValue(expr, bindingDiagnosticBag);
				if (!boundExpression.HasErrors && (object)boundExpression.Type == null)
				{
					boundExpression = binder.ReclassifyExpression(boundExpression, bindingDiagnosticBag);
				}
				bool num = !bindingDiagnosticBag.HasAnyErrors();
				bindingDiagnosticBag.Free();
				if (num)
				{
					return boundExpression;
				}
			}
			return node;
		}

		private AttributeBinder GetSpeculativeAttributeBinder(int position, AttributeSyntax attribute)
		{
			CheckPosition(position);
			Binder enclosingBinder = GetEnclosingBinder(position);
			if (enclosingBinder == null)
			{
				return null;
			}
			return BinderBuilder.CreateBinderForAttribute(enclosingBinder.SyntaxTree, enclosingBinder, attribute);
		}

		internal BoundAttribute GetSpeculativelyBoundAttribute(int position, AttributeSyntax attribute, out Binder binder)
		{
			binder = GetSpeculativeAttributeBinder(position, attribute);
			if (binder != null)
			{
				return binder.BindAttribute(attribute, BindingDiagnosticBag.Discarded);
			}
			return null;
		}

		private BoundNodeSummary GetSpeculativelyBoundAttributeSummary(int position, AttributeSyntax attribute, out Binder binder)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			BoundAttribute speculativelyBoundAttribute = GetSpeculativelyBoundAttribute(position, attribute, out binder);
			return (speculativelyBoundAttribute == null) ? default(BoundNodeSummary) : new BoundNodeSummary(speculativelyBoundAttribute, speculativelyBoundAttribute, null);
		}

		private void AddSymbolsFromDiagnosticInfo(ArrayBuilder<Symbol> symbolsBuilder, DiagnosticInfo diagnosticInfo)
		{
			if (diagnosticInfo is IDiagnosticInfoWithSymbols diagnosticInfoWithSymbols)
			{
				diagnosticInfoWithSymbols.GetAssociatedSymbols(symbolsBuilder);
			}
		}

		internal ImmutableArray<Symbol> RemoveErrorTypesAndDuplicates(ArrayBuilder<Symbol> symbolsBuilder, SymbolInfoOptions options)
		{
			if (symbolsBuilder.Count == 0)
			{
				return ImmutableArray<Symbol>.Empty;
			}
			if (symbolsBuilder.Count == 1)
			{
				Symbol symbol = symbolsBuilder[0];
				if ((options & SymbolInfoOptions.ResolveAliases) != 0)
				{
					symbol = SymbolExtensions.UnwrapAlias(symbol);
				}
				if (symbol is ErrorTypeSymbol)
				{
					symbolsBuilder.Clear();
					AddSymbolsFromDiagnosticInfo(symbolsBuilder, ((ErrorTypeSymbol)symbol).ErrorInfo);
					return symbolsBuilder.ToImmutable();
				}
				return ImmutableArray.Create(symbol);
			}
			PooledHashSet<Symbol> instance = PooledHashSet<Symbol>.GetInstance();
			ArrayBuilder<Symbol>.Enumerator enumerator = symbolsBuilder.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol symbol2 = enumerator.Current;
				if ((options & SymbolInfoOptions.ResolveAliases) != 0)
				{
					symbol2 = SymbolExtensions.UnwrapAlias(symbol2);
				}
				if (symbol2 is ErrorTypeSymbol)
				{
					ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance();
					AddSymbolsFromDiagnosticInfo(instance2, ((ErrorTypeSymbol)symbol2).ErrorInfo);
					ArrayBuilder<Symbol>.Enumerator enumerator2 = instance2.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Symbol current = enumerator2.Current;
						instance.Add(current);
					}
					instance2.Free();
				}
				else
				{
					instance.Add(symbol2);
				}
			}
			ImmutableArray<Symbol> result = ImmutableArray.CreateRange(instance);
			instance.Free();
			return result;
		}

		private TypeSymbol GetSemanticType(BoundNodeSummary boundNodes, ref TypeSymbol convertedType, ref Conversion conversion)
		{
			convertedType = null;
			conversion = new Conversion(Conversions.Identity);
			BoundExpression boundExpression = boundNodes.LowestBoundNode as BoundExpression;
			BoundExpression boundExpression2 = boundNodes.HighestBoundNode as BoundExpression;
			if (boundExpression == null)
			{
				return null;
			}
			if (boundNodes.LowestBoundNodeOfSyntacticParent != null && VisualBasicExtensions.Kind(boundNodes.LowestBoundNodeOfSyntacticParent.Syntax) == SyntaxKind.ObjectCreationExpression && ((ObjectCreationExpressionSyntax)boundNodes.LowestBoundNodeOfSyntacticParent.Syntax).Type == boundExpression.Syntax)
			{
				return null;
			}
			TypeSymbol typeSymbol;
			if (boundExpression.Kind != BoundKind.ArrayCreation || ((BoundArrayCreation)boundExpression).ArrayLiteralOpt == null)
			{
				typeSymbol = ((boundExpression.Kind != BoundKind.ConvertedTupleLiteral) ? boundExpression.Type : ((BoundConvertedTupleLiteral)boundExpression).NaturalTypeOpt);
			}
			else
			{
				typeSymbol = null;
				conversion = new Conversion(new KeyValuePair<ConversionKind, MethodSymbol>(((BoundArrayCreation)boundExpression).ArrayLiteralConversion, null));
			}
			bool flag = false;
			if ((object)typeSymbol == LocalSymbol.UseBeforeDeclarationResultType && boundExpression.Kind == BoundKind.Local)
			{
				flag = true;
				typeSymbol = ((BoundLocal)boundExpression).LocalSymbol.Type;
			}
			if (boundExpression2 != null && (object)boundExpression2.Type != null && boundExpression2.Type.TypeKind != TypeKind.Error)
			{
				convertedType = boundExpression2.Type;
				if (((object)typeSymbol == null || !TypeSymbolExtensions.IsSameTypeIgnoringAll(typeSymbol, convertedType)) && boundExpression2.Kind == BoundKind.Conversion)
				{
					BoundConversion boundConversion = (BoundConversion)boundExpression2;
					if (flag && !TypeSymbolExtensions.IsErrorType(typeSymbol))
					{
						TypeSymbol source = typeSymbol;
						TypeSymbol destination = convertedType;
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
						conversion = new Conversion(Conversions.ClassifyConversion(source, destination, ref useSiteInfo));
					}
					else
					{
						conversion = new Conversion(KeyValuePairUtil.Create(boundConversion.ConversionKind, boundConversion.ExpressionSymbol as MethodSymbol));
					}
				}
			}
			if ((object)typeSymbol == null && boundNodes.LowestBoundNodeOfSyntacticParent is BoundBadExpression)
			{
				SyntaxNode syntax = boundNodes.LowestBoundNodeOfSyntacticParent.Syntax;
				if (syntax != null && syntax == boundNodes.LowestBoundNode.Syntax.Parent && VisualBasicExtensions.Kind(syntax) == SyntaxKind.ObjectCreationExpression && ((ObjectCreationExpressionSyntax)syntax).Type == boundNodes.LowestBoundNode.Syntax)
				{
					typeSymbol = ((BoundBadExpression)boundNodes.LowestBoundNodeOfSyntacticParent).Type;
				}
			}
			if ((object)convertedType == null)
			{
				convertedType = typeSymbol;
			}
			return typeSymbol;
		}

		private ImmutableArray<Symbol> GetSemanticSymbols(BoundNodeSummary boundNodes, Binder binderOpt, SymbolInfoOptions options, ref LookupResultKind resultKind, ref ImmutableArray<Symbol> memberGroup)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance();
			resultKind = LookupResultKind.Good;
			if (boundNodes.LowestBoundNode != null)
			{
				switch (boundNodes.LowestBoundNode.Kind)
				{
				case BoundKind.MethodGroup:
					GetSemanticSymbolsForMethodGroup(boundNodes, instance, instance2, ref resultKind);
					break;
				case BoundKind.PropertyGroup:
					GetSemanticSymbolsForPropertyGroup(boundNodes, instance, instance2, ref resultKind);
					break;
				case BoundKind.TypeExpression:
				{
					if (boundNodes.LowestBoundNodeOfSyntacticParent != null && VisualBasicExtensions.Kind(boundNodes.LowestBoundNodeOfSyntacticParent.Syntax) == SyntaxKind.ObjectCreationExpression && ((ObjectCreationExpressionSyntax)boundNodes.LowestBoundNodeOfSyntacticParent.Syntax).Type == boundNodes.LowestBoundNode.Syntax && boundNodes.LowestBoundNodeOfSyntacticParent.Kind == BoundKind.BadExpression && ((BoundBadExpression)boundNodes.LowestBoundNodeOfSyntacticParent).ResultKind == LookupResultKind.NotCreatable)
					{
						resultKind = LookupResultKind.NotCreatable;
					}
					BoundTypeExpression boundTypeExpression = (BoundTypeExpression)boundNodes.LowestBoundNode;
					if ((object)boundTypeExpression.AliasOpt != null)
					{
						instance.Add(boundTypeExpression.AliasOpt);
						break;
					}
					TypeSymbol type2 = boundTypeExpression.Type;
					if (type2.OriginalDefinition is ErrorTypeSymbol errorTypeSymbol)
					{
						resultKind = errorTypeSymbol.ResultKind;
						instance.AddRange(errorTypeSymbol.CandidateSymbols);
					}
					else
					{
						instance.Add(type2);
					}
					break;
				}
				case BoundKind.Attribute:
				{
					BoundAttribute boundAttribute = (BoundAttribute)boundNodes.LowestBoundNode;
					resultKind = boundAttribute.ResultKind;
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)boundAttribute.Type;
					if (TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
					{
						ErrorTypeSymbol errorTypeSymbol2 = (ErrorTypeSymbol)namedTypeSymbol;
						ImmutableArray<Symbol> candidateSymbols = errorTypeSymbol2.CandidateSymbols;
						if (candidateSymbols.Length != 1 || candidateSymbols[0].Kind != SymbolKind.NamedType)
						{
							instance.AddRange(candidateSymbols);
							break;
						}
						namedTypeSymbol = (NamedTypeSymbol)errorTypeSymbol2.CandidateSymbols[0];
					}
					ImmutableArray<Symbol> bindingSymbols = ImmutableArray<Symbol>.Empty;
					AdjustSymbolsForObjectCreation(boundAttribute, namedTypeSymbol, boundAttribute.Constructor, binderOpt, ref bindingSymbols, instance2, ref resultKind);
					instance.AddRange(bindingSymbols);
					break;
				}
				case BoundKind.ObjectCreationExpression:
				{
					BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)boundNodes.LowestBoundNode;
					if (boundObjectCreationExpression.MethodGroupOpt != null)
					{
						BoundExpressionExtensions.GetExpressionSymbols(boundObjectCreationExpression.MethodGroupOpt, instance2);
						resultKind = boundObjectCreationExpression.MethodGroupOpt.ResultKind;
					}
					if ((object)boundObjectCreationExpression.ConstructorOpt != null)
					{
						instance.Add(boundObjectCreationExpression.ConstructorOpt);
					}
					else
					{
						instance.AddRange(instance2);
					}
					break;
				}
				case BoundKind.LateMemberAccess:
					GetSemanticSymbolsForLateBoundMemberAccess(boundNodes, instance, instance2, ref resultKind);
					break;
				case BoundKind.LateInvocation:
				{
					BoundLateInvocation lateInvocation = (BoundLateInvocation)boundNodes.LowestBoundNode;
					GetSemanticSymbolsForLateBoundInvocation(lateInvocation, instance, instance2, ref resultKind);
					break;
				}
				case BoundKind.MeReference:
				case BoundKind.MyBaseReference:
				case BoundKind.MyClassReference:
				{
					BoundExpression obj = (BoundExpression)boundNodes.LowestBoundNode;
					Binder obj2 = binderOpt ?? GetEnclosingBinder(boundNodes.LowestBoundNode.Syntax.SpanStart);
					ParameterSymbol item = GetMeParameter(containingType: obj2.ContainingType, containingMember: obj2.ContainingMember, referenceType: obj.Type, resultKind: ref resultKind);
					instance.Add(item);
					break;
				}
				case BoundKind.TypeOrValueExpression:
				{
					BoundTypeOrValueExpression boundTypeOrValueExpression = (BoundTypeOrValueExpression)boundNodes.LowestBoundNode;
					BoundNodeSummary boundNodes2 = new BoundNodeSummary(boundTypeOrValueExpression.Data.ValueExpression, boundNodes.HighestBoundNode, boundNodes.LowestBoundNodeOfSyntacticParent);
					return GetSemanticSymbols(boundNodes2, binderOpt, options, ref resultKind, ref memberGroup);
				}
				default:
				{
					if (!(boundNodes.LowestBoundNode is BoundExpression boundExpression))
					{
						break;
					}
					BoundExpressionExtensions.GetExpressionSymbols(boundExpression, instance);
					resultKind = boundExpression.ResultKind;
					if (boundExpression.Kind != BoundKind.BadExpression || VisualBasicExtensions.Kind(boundExpression.Syntax) != SyntaxKind.ObjectCreationExpression)
					{
						break;
					}
					TypeSyntax type = ((ObjectCreationExpressionSyntax)boundExpression.Syntax).Type;
					ImmutableArray<BoundExpression>.Enumerator enumerator = ((BoundBadExpression)boundExpression).ChildBoundNodes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						BoundExpression current = enumerator.Current;
						if (current.Kind == BoundKind.MethodGroup && current.Syntax == type)
						{
							BoundMethodGroup boundMethodGroup = (BoundMethodGroup)current;
							BoundExpressionExtensions.GetExpressionSymbols(boundMethodGroup, instance2);
							if (resultKind == LookupResultKind.NotCreatable)
							{
								resultKind = boundMethodGroup.ResultKind;
							}
							else
							{
								resultKind = LookupResult.WorseResultKind(resultKind, boundMethodGroup.ResultKind);
							}
							break;
						}
					}
					break;
				}
				}
			}
			ImmutableArray<Symbol> bindingSymbols2 = RemoveErrorTypesAndDuplicates(instance, options);
			instance.Free();
			if (boundNodes.LowestBoundNodeOfSyntacticParent != null && (options & SymbolInfoOptions.PreferConstructorsToType) != 0)
			{
				AdjustSymbolsForObjectCreation(boundNodes, binderOpt, ref bindingSymbols2, instance2, ref resultKind);
			}
			memberGroup = instance2.ToImmutableAndFree();
			if (boundNodes.HighestBoundNode is BoundExpression boundExpression2 && boundNodes.HighestBoundNode != boundNodes.LowestBoundNode)
			{
				if (boundExpression2.ResultKind != 0 && boundExpression2.ResultKind < resultKind)
				{
					resultKind = boundExpression2.ResultKind;
				}
				if (boundExpression2.Kind == BoundKind.BadExpression && bindingSymbols2.Length == 0)
				{
					bindingSymbols2 = ((BoundBadExpression)boundExpression2).Symbols;
				}
			}
			return bindingSymbols2;
		}

		private static ParameterSymbol GetMeParameter(TypeSymbol referenceType, TypeSymbol containingType, Symbol containingMember, ref LookupResultKind resultKind)
		{
			if ((object)containingMember == null || (object)containingType == null)
			{
				resultKind = LookupResultKind.NotReferencable;
				return new MeParameterSymbol(containingMember, referenceType);
			}
			SymbolKind kind = containingMember.Kind;
			ParameterSymbol result;
			if (kind == SymbolKind.Field || kind == SymbolKind.Method || kind == SymbolKind.Property)
			{
				if (containingMember.IsShared)
				{
					resultKind = LookupResultKind.MustNotBeInstance;
					result = new MeParameterSymbol(containingMember, containingType);
				}
				else if (TypeSymbol.Equals(referenceType, ErrorTypeSymbol.UnknownResultType, TypeCompareKind.ConsiderEverything))
				{
					result = new MeParameterSymbol(containingMember, containingType);
					resultKind = LookupResultKind.NotReferencable;
				}
				else
				{
					resultKind = LookupResultKind.Good;
					result = SymbolExtensions.GetMeParameter(containingMember);
				}
			}
			else
			{
				result = new MeParameterSymbol(containingMember, referenceType);
				resultKind = LookupResultKind.NotReferencable;
			}
			return result;
		}

		private void GetSemanticSymbolsForLateBoundInvocation(BoundLateInvocation lateInvocation, ArrayBuilder<Symbol> symbolsBuilder, ArrayBuilder<Symbol> memberGroupBuilder, ref LookupResultKind resultKind)
		{
			resultKind = LookupResultKind.LateBound;
			BoundMethodOrPropertyGroup methodOrPropertyGroupOpt = lateInvocation.MethodOrPropertyGroupOpt;
			if (methodOrPropertyGroupOpt != null)
			{
				BoundExpressionExtensions.GetExpressionSymbols(methodOrPropertyGroupOpt, memberGroupBuilder);
				BoundExpressionExtensions.GetExpressionSymbols(methodOrPropertyGroupOpt, symbolsBuilder);
			}
		}

		private void GetSemanticSymbolsForLateBoundMemberAccess(BoundNodeSummary boundNodes, ArrayBuilder<Symbol> symbolsBuilder, ArrayBuilder<Symbol> memberGroupBuilder, ref LookupResultKind resultKind)
		{
			if (boundNodes.LowestBoundNodeOfSyntacticParent != null && boundNodes.LowestBoundNodeOfSyntacticParent.Kind == BoundKind.LateInvocation)
			{
				GetSemanticSymbolsForLateBoundInvocation((BoundLateInvocation)boundNodes.LowestBoundNodeOfSyntacticParent, symbolsBuilder, memberGroupBuilder, ref resultKind);
			}
			else
			{
				resultKind = LookupResultKind.LateBound;
			}
		}

		private void GetSemanticSymbolsForMethodGroup(BoundNodeSummary boundNodes, ArrayBuilder<Symbol> symbolsBuilder, ArrayBuilder<Symbol> memberGroupBuilder, ref LookupResultKind resultKind)
		{
			BoundMethodGroup boundMethodGroup = (BoundMethodGroup)boundNodes.LowestBoundNode;
			resultKind = boundMethodGroup.ResultKind;
			BoundExpressionExtensions.GetExpressionSymbols(boundMethodGroup, memberGroupBuilder);
			bool flag = false;
			if (boundNodes.LowestBoundNodeOfSyntacticParent != null)
			{
				switch (boundNodes.LowestBoundNodeOfSyntacticParent.Kind)
				{
				case BoundKind.Call:
				{
					BoundCall boundCall = (BoundCall)boundNodes.LowestBoundNodeOfSyntacticParent;
					symbolsBuilder.Add(boundCall.Method);
					if (boundCall.ResultKind < resultKind)
					{
						resultKind = boundCall.ResultKind;
					}
					flag = true;
					break;
				}
				case BoundKind.DelegateCreationExpression:
				{
					BoundDelegateCreationExpression boundDelegateCreationExpression = (BoundDelegateCreationExpression)boundNodes.LowestBoundNodeOfSyntacticParent;
					symbolsBuilder.Add(boundDelegateCreationExpression.Method);
					if (boundDelegateCreationExpression.ResultKind < resultKind)
					{
						resultKind = boundDelegateCreationExpression.ResultKind;
					}
					flag = true;
					break;
				}
				case BoundKind.BadExpression:
				{
					BoundBadExpression boundBadExpression = (BoundBadExpression)boundNodes.LowestBoundNodeOfSyntacticParent;
					symbolsBuilder.AddRange(boundBadExpression.Symbols.Where((Symbol sym) => memberGroupBuilder.Contains(sym)));
					if (symbolsBuilder.Count > 0)
					{
						resultKind = boundBadExpression.ResultKind;
						flag = true;
					}
					break;
				}
				case BoundKind.NameOfOperator:
					symbolsBuilder.AddRange(memberGroupBuilder);
					resultKind = LookupResultKind.MemberGroup;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				symbolsBuilder.AddRange(memberGroupBuilder);
				resultKind = LookupResultKind.OverloadResolutionFailure;
			}
			if (boundMethodGroup.ResultKind < resultKind)
			{
				resultKind = boundMethodGroup.ResultKind;
			}
		}

		private void GetSemanticSymbolsForPropertyGroup(BoundNodeSummary boundNodes, ArrayBuilder<Symbol> symbolsBuilder, ArrayBuilder<Symbol> memberGroupBuilder, ref LookupResultKind resultKind)
		{
			BoundPropertyGroup boundPropertyGroup = (BoundPropertyGroup)boundNodes.LowestBoundNode;
			resultKind = boundPropertyGroup.ResultKind;
			memberGroupBuilder.AddRange(boundPropertyGroup.Properties);
			bool flag = false;
			if (boundNodes.LowestBoundNodeOfSyntacticParent != null)
			{
				switch (boundNodes.LowestBoundNodeOfSyntacticParent.Kind)
				{
				case BoundKind.PropertyAccess:
					if (boundNodes.LowestBoundNodeOfSyntacticParent is BoundPropertyAccess boundPropertyAccess)
					{
						symbolsBuilder.Add(boundPropertyAccess.PropertySymbol);
						if (boundPropertyAccess.ResultKind < resultKind)
						{
							resultKind = boundPropertyAccess.ResultKind;
						}
						flag = true;
					}
					break;
				case BoundKind.BadExpression:
				{
					BoundBadExpression boundBadExpression = (BoundBadExpression)boundNodes.LowestBoundNodeOfSyntacticParent;
					symbolsBuilder.AddRange(boundBadExpression.Symbols.Where((Symbol sym) => memberGroupBuilder.Contains(sym)));
					if (symbolsBuilder.Count > 0)
					{
						resultKind = boundBadExpression.ResultKind;
						flag = true;
					}
					break;
				}
				case BoundKind.NameOfOperator:
					symbolsBuilder.AddRange(memberGroupBuilder);
					resultKind = LookupResultKind.MemberGroup;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				symbolsBuilder.AddRange(memberGroupBuilder);
				resultKind = LookupResultKind.OverloadResolutionFailure;
			}
			if (boundPropertyGroup.ResultKind < resultKind)
			{
				resultKind = boundPropertyGroup.ResultKind;
			}
		}

		private static ImmutableArray<Symbol> UnwrapAliases(ImmutableArray<Symbol> symbols)
		{
			if (!symbols.Any((Symbol sym) => sym.Kind == SymbolKind.Alias))
			{
				return symbols;
			}
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			ImmutableArray<Symbol>.Enumerator enumerator = symbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				instance.Add(SymbolExtensions.UnwrapAlias(current));
			}
			return instance.ToImmutableAndFree();
		}

		private void AdjustSymbolsForObjectCreation(BoundNodeSummary boundNodes, Binder binderOpt, ref ImmutableArray<Symbol> bindingSymbols, ArrayBuilder<Symbol> memberGroupBuilder, ref LookupResultKind resultKind)
		{
			MethodSymbol constructor = null;
			BoundNode lowestBoundNode = boundNodes.LowestBoundNode;
			BoundNode lowestBoundNodeOfSyntacticParent = boundNodes.LowestBoundNodeOfSyntacticParent;
			SyntaxNode syntax = lowestBoundNodeOfSyntacticParent.Syntax;
			if (syntax == null || lowestBoundNode == null || syntax != lowestBoundNode.Syntax.Parent || VisualBasicExtensions.Kind(syntax) != SyntaxKind.Attribute || ((AttributeSyntax)syntax).Name != lowestBoundNode.Syntax)
			{
				return;
			}
			ImmutableArray<Symbol> immutableArray = UnwrapAliases(bindingSymbols);
			if (immutableArray.Length == 1 && immutableArray[0] is TypeSymbol)
			{
				NamedTypeSymbol namedTypeSymbol = ((TypeSymbol)immutableArray[0]) as NamedTypeSymbol;
				switch (lowestBoundNodeOfSyntacticParent.Kind)
				{
				case BoundKind.Attribute:
				{
					BoundAttribute boundAttribute = (BoundAttribute)lowestBoundNodeOfSyntacticParent;
					constructor = boundAttribute.Constructor;
					resultKind = LookupResult.WorseResultKind(resultKind, boundAttribute.ResultKind);
					break;
				}
				case BoundKind.BadExpression:
				{
					BoundBadExpression boundBadExpression = (BoundBadExpression)lowestBoundNodeOfSyntacticParent;
					resultKind = LookupResult.WorseResultKind(resultKind, boundBadExpression.ResultKind);
					break;
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(lowestBoundNodeOfSyntacticParent.Kind);
				}
				AdjustSymbolsForObjectCreation(lowestBoundNode, namedTypeSymbol, constructor, binderOpt, ref bindingSymbols, memberGroupBuilder, ref resultKind);
			}
		}

		private void AdjustSymbolsForObjectCreation(BoundNode lowestBoundNode, NamedTypeSymbol namedTypeSymbol, MethodSymbol constructor, Binder binderOpt, ref ImmutableArray<Symbol> bindingSymbols, ArrayBuilder<Symbol> memberGroupBuilder, ref LookupResultKind resultKind)
		{
			if ((object)namedTypeSymbol == null)
			{
				return;
			}
			Binder binder = binderOpt ?? GetEnclosingBinder(lowestBoundNode.Syntax.SpanStart);
			ImmutableArray<MethodSymbol> immutableArray;
			if (binder != null)
			{
				NamedTypeSymbol namedTypeSymbol2 = (namedTypeSymbol.IsInterface ? (namedTypeSymbol.CoClassType as NamedTypeSymbol) : null);
				NamedTypeSymbol type = namedTypeSymbol2 ?? namedTypeSymbol;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				immutableArray = binder.GetAccessibleConstructors(type, ref useSiteInfo);
				ImmutableArray<MethodSymbol> instanceConstructors = namedTypeSymbol.InstanceConstructors;
				if (!immutableArray.Any() && instanceConstructors.Any())
				{
					immutableArray = instanceConstructors;
				}
			}
			else
			{
				immutableArray = ImmutableArray<MethodSymbol>.Empty;
			}
			if ((object)constructor != null)
			{
				bindingSymbols = ImmutableArray.Create((Symbol)constructor);
			}
			else if (immutableArray.Length != 0)
			{
				bindingSymbols = StaticCast<Symbol>.From(immutableArray);
				resultKind = LookupResult.WorseResultKind(resultKind, LookupResultKind.OverloadResolutionFailure);
			}
			memberGroupBuilder.AddRange(immutableArray);
		}

		internal SymbolInfo GetSymbolInfoForSymbol(Symbol symbol, SymbolInfoOptions options)
		{
			TypeSymbol typeSymbol = SymbolExtensions.UnwrapAlias(symbol) as TypeSymbol;
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			ErrorTypeSymbol errorTypeSymbol = (((object)typeSymbol != null) ? (typeSymbol.OriginalDefinition as ErrorTypeSymbol) : null);
			LookupResultKind lookupResultKind;
			if ((object)errorTypeSymbol != null)
			{
				lookupResultKind = errorTypeSymbol.ResultKind;
				if (lookupResultKind != 0)
				{
					instance.AddRange(errorTypeSymbol.CandidateSymbols);
				}
			}
			else if (symbol.Kind == SymbolKind.Namespace && ((NamespaceSymbol)symbol).NamespaceKind == (NamespaceKind)0)
			{
				instance.AddRange(((NamespaceSymbol)symbol).ConstituentNamespaces);
				lookupResultKind = LookupResultKind.Ambiguous;
			}
			else
			{
				instance.Add(symbol);
				lookupResultKind = LookupResultKind.Good;
			}
			ImmutableArray<Symbol> symbols = RemoveErrorTypesAndDuplicates(instance, options);
			instance.Free();
			return SymbolInfoFactory.Create(symbols, lookupResultKind);
		}

		internal VisualBasicTypeInfo GetTypeInfoForSymbol(Symbol symbol)
		{
			TypeSymbol typeSymbol = SymbolExtensions.UnwrapAlias(symbol) as TypeSymbol;
			return new VisualBasicTypeInfo(typeSymbol, typeSymbol, new Conversion(Conversions.Identity));
		}

		internal virtual BoundNode Bind(Binder binder, SyntaxNode node, BindingDiagnosticBag diagnostics)
		{
			if (node is ExpressionSyntax node2)
			{
				return binder.BindNamespaceOrTypeOrExpressionSyntaxForSemanticModel(node2, diagnostics);
			}
			if (node is StatementSyntax node3)
			{
				return binder.BindStatement(node3, diagnostics);
			}
			return null;
		}

		public new ImmutableArray<ISymbol> LookupSymbols(int position, INamespaceOrTypeSymbol container = null, string name = null, bool includeReducedExtensionMethods = false)
		{
			LookupOptions options = ((!includeReducedExtensionMethods) ? LookupOptions.IgnoreExtensionMethods : LookupOptions.Default);
			return StaticCast<ISymbol>.From(LookupSymbolsInternal(position, ToLanguageSpecific(container), name, options, useBaseReferenceAccessibility: false));
		}

		public new ImmutableArray<ISymbol> LookupBaseMembers(int position, string name = null)
		{
			return StaticCast<ISymbol>.From(LookupSymbolsInternal(position, null, name, LookupOptions.Default, useBaseReferenceAccessibility: true));
		}

		public new ImmutableArray<ISymbol> LookupStaticMembers(int position, INamespaceOrTypeSymbol container = null, string name = null)
		{
			return StaticCast<ISymbol>.From(LookupSymbolsInternal(position, ToLanguageSpecific(container), name, LookupOptions.MustNotBeInstance | LookupOptions.IgnoreExtensionMethods, useBaseReferenceAccessibility: false));
		}

		public new ImmutableArray<ISymbol> LookupNamespacesAndTypes(int position, INamespaceOrTypeSymbol container = null, string name = null)
		{
			return StaticCast<ISymbol>.From(LookupSymbolsInternal(position, ToLanguageSpecific(container), name, LookupOptions.NamespacesOrTypesOnly, useBaseReferenceAccessibility: false));
		}

		public new ImmutableArray<ISymbol> LookupLabels(int position, string name = null)
		{
			return StaticCast<ISymbol>.From(LookupSymbolsInternal(position, null, name, LookupOptions.LabelsOnly, useBaseReferenceAccessibility: false));
		}

		private ImmutableArray<Symbol> LookupSymbolsInternal(int position, NamespaceOrTypeSymbol container, string name, LookupOptions options, bool useBaseReferenceAccessibility)
		{
			if (useBaseReferenceAccessibility)
			{
				options |= LookupOptions.UseBaseReferenceAccessibility;
			}
			CheckPosition(position);
			Binder enclosingBinder = GetEnclosingBinder(position);
			if (enclosingBinder == null)
			{
				return ImmutableArray<Symbol>.Empty;
			}
			if (useBaseReferenceAccessibility)
			{
				container = enclosingBinder.ContainingType?.BaseTypeNoUseSiteDiagnostics ?? throw new ArgumentException("position", "Not a valid position for a call to LookupBaseMembers (must be in a type with a base type)");
			}
			if (name == null)
			{
				LookupSymbolsInfo instance = LookupSymbolsInfo.GetInstance();
				AddLookupSymbolsInfo(position, instance, container, options);
				ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance(instance.Count);
				foreach (string name2 in instance.Names)
				{
					AppendSymbolsWithName(instance2, name2, enclosingBinder, container, options, instance);
				}
				instance.Free();
				ImmutableArray<Symbol> immutableArray = instance2.ToImmutableAndFree();
				ArrayBuilder<Symbol> arrayBuilder = null;
				int num = 0;
				ImmutableArray<Symbol>.Enumerator enumerator2 = immutableArray.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current2 = enumerator2.Current;
					if (current2.CanBeReferencedByName || (current2.Kind == SymbolKind.Method && ((MethodSymbol)current2).MethodKind == MethodKind.Constructor))
					{
						arrayBuilder?.Add(current2);
					}
					else if (arrayBuilder == null)
					{
						arrayBuilder = ArrayBuilder<Symbol>.GetInstance();
						arrayBuilder.AddRange(immutableArray, num);
					}
					num++;
				}
				return arrayBuilder?.ToImmutableAndFree() ?? immutableArray;
			}
			LookupSymbolsInfo instance3 = LookupSymbolsInfo.GetInstance();
			instance3.FilterName = name;
			AddLookupSymbolsInfo(position, instance3, container, options);
			ArrayBuilder<Symbol> instance4 = ArrayBuilder<Symbol>.GetInstance(instance3.Count);
			AppendSymbolsWithName(instance4, name, enclosingBinder, container, options, instance3);
			instance3.Free();
			return instance4.ToImmutableAndFree();
		}

		private void AppendSymbolsWithName(ArrayBuilder<Symbol> results, string name, Binder binder, NamespaceOrTypeSymbol container, LookupOptions options, LookupSymbolsInfo info)
		{
			AbstractLookupSymbolsInfo<Symbol>.IArityEnumerable arities = null;
			Symbol uniqueSymbol = null;
			if (info.TryGetAritiesAndUniqueSymbol(name, out arities, out uniqueSymbol))
			{
				if ((object)uniqueSymbol != null)
				{
					results.Add(uniqueSymbol);
				}
				else if (arities != null)
				{
					LookupSymbols(binder, container, name, arities, options, results);
				}
				else
				{
					LookupSymbols(binder, container, name, 0, options, results);
				}
			}
		}

		private void LookupSymbols(Binder binder, NamespaceOrTypeSymbol container, string name, AbstractLookupSymbolsInfo<Symbol>.IArityEnumerable arities, LookupOptions options, ArrayBuilder<Symbol> results)
		{
			PooledHashSet<Symbol> instance = PooledHashSet<Symbol>.GetInstance();
			ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance(arities.Count);
			foreach (int arity in arities)
			{
				LookupSymbols(binder, container, name, arity, options, instance2);
				instance.UnionWith(instance2);
				instance2.Clear();
			}
			instance2.Free();
			results.AddRange(instance);
			instance.Free();
		}

		private void LookupSymbols(Binder binder, NamespaceOrTypeSymbol container, string name, int arity, LookupOptions options, ArrayBuilder<Symbol> results)
		{
			if (EmbeddedOperators.CompareString(name, ".ctor", TextCompare: false) == 0)
			{
				LookupInstanceConstructors(binder, container, options, results);
				return;
			}
			LookupResult instance = LookupResult.GetInstance();
			options |= LookupOptions.EagerlyLookupExtensionMethods;
			if (LookupOptionExtensions.IsAttributeTypeLookup(options))
			{
				LookupOptions options2 = options;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				binder.LookupAttributeType(instance, container, name, options2, ref useSiteInfo);
			}
			else if ((object)container == null)
			{
				LookupOptions options3 = options;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				binder.Lookup(instance, name, arity, options3, ref useSiteInfo);
			}
			else
			{
				LookupOptions options4 = options;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				binder.LookupMember(instance, container, name, arity, options4, ref useSiteInfo);
			}
			if (instance.IsGoodOrAmbiguous)
			{
				if (instance.HasDiagnostic)
				{
					PooledHashSet<Symbol> instance2 = PooledHashSet<Symbol>.GetInstance();
					ArrayBuilder<Symbol> instance3 = ArrayBuilder<Symbol>.GetInstance();
					AddSymbolsFromDiagnosticInfo(instance3, instance.Diagnostic);
					instance2.UnionWith(instance3);
					instance2.UnionWith(instance.Symbols);
					instance3.Free();
					results.AddRange(instance2);
					instance2.Free();
				}
				else if (instance.HasSingleSymbol && instance.SingleSymbol.Kind == SymbolKind.Namespace && ((NamespaceSymbol)instance.SingleSymbol).NamespaceKind == (NamespaceKind)0)
				{
					results.AddRange(((NamespaceSymbol)instance.SingleSymbol).ConstituentNamespaces);
				}
				else
				{
					results.AddRange(instance.Symbols);
				}
			}
			instance.Free();
		}

		private void LookupInstanceConstructors(Binder binder, NamespaceOrTypeSymbol container, LookupOptions options, ArrayBuilder<Symbol> results)
		{
			ImmutableArray<MethodSymbol> items = ImmutableArray<MethodSymbol>.Empty;
			if (container is NamedTypeSymbol namedTypeSymbol && (options & (LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly | LookupOptions.MustNotBeInstance)) == 0)
			{
				if ((options & LookupOptions.IgnoreAccessibility) != 0)
				{
					items = namedTypeSymbol.InstanceConstructors;
				}
				else
				{
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					items = binder.GetAccessibleConstructors(namedTypeSymbol, ref useSiteInfo);
				}
			}
			results.AddRange(items);
		}

		private void AddLookupSymbolsInfo(int position, LookupSymbolsInfo info, NamespaceOrTypeSymbol container = null, LookupOptions options = LookupOptions.Default)
		{
			CheckPosition(position);
			Binder enclosingBinder = GetEnclosingBinder(position);
			if (enclosingBinder != null)
			{
				if ((object)container == null)
				{
					enclosingBinder.AddLookupSymbolsInfo(info, options);
				}
				else
				{
					enclosingBinder.AddMemberLookupSymbolsInfo(info, container, options);
				}
			}
		}

		public new bool IsAccessible(int position, ISymbol symbol)
		{
			CheckPosition(position);
			if (symbol == null)
			{
				throw new ArgumentNullException("symbol");
			}
			Symbol sym = SymbolExtensions.EnsureVbSymbolOrNothing<ISymbol, Symbol>(symbol, "symbol");
			Binder enclosingBinder = GetEnclosingBinder(position);
			if (enclosingBinder != null)
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				return enclosingBinder.IsAccessible(sym, ref useSiteInfo);
			}
			return false;
		}

		public virtual ControlFlowAnalysis AnalyzeControlFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
		{
			throw new NotSupportedException();
		}

		public virtual ControlFlowAnalysis AnalyzeControlFlow(StatementSyntax statement)
		{
			return AnalyzeControlFlow(statement, statement);
		}

		public virtual DataFlowAnalysis AnalyzeDataFlow(ExpressionSyntax expression)
		{
			throw new NotSupportedException();
		}

		public virtual DataFlowAnalysis AnalyzeDataFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
		{
			throw new NotSupportedException();
		}

		public virtual DataFlowAnalysis AnalyzeDataFlow(StatementSyntax statement)
		{
			return AnalyzeDataFlow(statement, statement);
		}

		public bool TryGetSpeculativeSemanticModelForMethodBody(int position, MethodBlockBaseSyntax method, out SemanticModel speculativeModel)
		{
			CheckPosition(position);
			CheckModelAndSyntaxNodeToSpeculate(method);
			return TryGetSpeculativeSemanticModelForMethodBodyCore((SyntaxTreeSemanticModel)this, position, method, out speculativeModel);
		}

		internal abstract bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, MethodBlockBaseSyntax method, out SemanticModel speculativeModel);

		public bool TryGetSpeculativeSemanticModel(int position, RangeArgumentSyntax rangeArgument, out SemanticModel speculativeModel)
		{
			CheckPosition(position);
			CheckModelAndSyntaxNodeToSpeculate(rangeArgument);
			return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, rangeArgument, out speculativeModel);
		}

		internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, RangeArgumentSyntax rangeArgument, out SemanticModel speculativeModel);

		public bool TryGetSpeculativeSemanticModel(int position, ExecutableStatementSyntax statement, out SemanticModel speculativeModel)
		{
			CheckPosition(position);
			CheckModelAndSyntaxNodeToSpeculate(statement);
			return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, statement, out speculativeModel);
		}

		internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ExecutableStatementSyntax statement, out SemanticModel speculativeModel);

		public bool TryGetSpeculativeSemanticModel(int position, EqualsValueSyntax initializer, out SemanticModel speculativeModel)
		{
			CheckPosition(position);
			CheckModelAndSyntaxNodeToSpeculate(initializer);
			return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, initializer, out speculativeModel);
		}

		internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueSyntax initializer, out SemanticModel speculativeModel);

		public bool TryGetSpeculativeSemanticModel(int position, AttributeSyntax attribute, out SemanticModel speculativeModel)
		{
			CheckPosition(position);
			CheckModelAndSyntaxNodeToSpeculate(attribute);
			Binder speculativeAttributeBinder = GetSpeculativeAttributeBinder(position, attribute);
			if (speculativeAttributeBinder == null)
			{
				speculativeModel = null;
				return false;
			}
			speculativeModel = AttributeSemanticModel.CreateSpeculative((SyntaxTreeSemanticModel)this, attribute, speculativeAttributeBinder, position);
			return true;
		}

		public bool TryGetSpeculativeSemanticModel(int position, TypeSyntax type, out SemanticModel speculativeModel, SpeculativeBindingOption bindingOption = SpeculativeBindingOption.BindAsExpression)
		{
			CheckPosition(position);
			CheckModelAndSyntaxNodeToSpeculate(type);
			return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, type, bindingOption, out speculativeModel);
		}

		internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, TypeSyntax type, SpeculativeBindingOption bindingOption, out SemanticModel speculativeModel);

		public abstract Conversion ClassifyConversion(ExpressionSyntax expression, ITypeSymbol destination);

		public Conversion ClassifyConversion(int position, ExpressionSyntax expression, ITypeSymbol destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			TypeSymbol typeSymbol = SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(destination, "destination");
			CheckPosition(position);
			Binder enclosingBinder = GetEnclosingBinder(position);
			Conversion result;
			if (enclosingBinder != null)
			{
				enclosingBinder = SpeculativeBinder.Create(enclosingBinder);
				BoundExpression boundExpression = enclosingBinder.BindValue(expression, BindingDiagnosticBag.Discarded);
				if (boundExpression != null && !TypeSymbolExtensions.IsErrorType(typeSymbol))
				{
					Binder binder = enclosingBinder;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					result = new Conversion(Conversions.ClassifyConversion(boundExpression, typeSymbol, binder, ref useSiteInfo));
					goto IL_0077;
				}
			}
			result = new Conversion(default(KeyValuePair<ConversionKind, MethodSymbol>));
			goto IL_0077;
			IL_0077:
			return result;
		}

		public virtual ISymbol GetDeclaredSymbol(ModifiedIdentifierSyntax identifierSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (identifierSyntax == null)
			{
				throw new ArgumentNullException("identifierSyntax");
			}
			if (!IsInTree(identifierSyntax))
			{
				throw new ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree);
			}
			Binder enclosingBinder = GetEnclosingBinder(identifierSyntax.SpanStart);
			if (StripSemanticModelBinder(enclosingBinder) is BlockBaseBinder blockBaseBinder)
			{
				LookupResult instance = LookupResult.GetInstance();
				try
				{
					string valueText = identifierSyntax.Identifier.ValueText;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					enclosingBinder.Lookup(instance, valueText, 0, LookupOptions.Default, ref useSiteInfo);
					if (instance.IsGood && instance.Symbols[0] is LocalSymbol localSymbol && localSymbol.IdentifierToken == identifierSyntax.Identifier)
					{
						return localSymbol;
					}
				}
				finally
				{
					instance.Free();
				}
				ImmutableArray<LocalSymbol>.Enumerator enumerator = blockBaseBinder.Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					if (current.IdentifierToken == identifierSyntax.Identifier)
					{
						return current;
					}
				}
			}
			return null;
		}

		public ISymbol GetDeclaredSymbol(TupleElementSyntax elementSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(elementSyntax);
			if (elementSyntax.Parent is TupleTypeSyntax tupleTypeSyntax)
			{
				return (GetSymbolInfo(tupleTypeSyntax, cancellationToken).Symbol as TupleTypeSymbol)?.TupleElements.ElementAtOrDefault(tupleTypeSyntax.Elements.IndexOf(elementSyntax));
			}
			return null;
		}

		public virtual IPropertySymbol GetDeclaredSymbol(FieldInitializerSyntax fieldInitializerSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (fieldInitializerSyntax == null)
			{
				throw new ArgumentNullException("fieldInitializerSyntax");
			}
			if (!IsInTree(fieldInitializerSyntax))
			{
				throw new ArgumentException(VBResources.FieldInitializerSyntaxNotWithinSyntaxTree);
			}
			return null;
		}

		public virtual INamedTypeSymbol GetDeclaredSymbol(AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (anonymousObjectCreationExpressionSyntax == null)
			{
				throw new ArgumentNullException("anonymousObjectCreationExpressionSyntax");
			}
			if (!IsInTree(anonymousObjectCreationExpressionSyntax))
			{
				throw new ArgumentException(VBResources.AnonymousObjectCreationExpressionSyntaxNotWithinTree);
			}
			return null;
		}

		public virtual IRangeVariableSymbol GetDeclaredSymbol(ExpressionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (rangeVariableSyntax == null)
			{
				throw new ArgumentNullException("rangeVariableSyntax");
			}
			if (!IsInTree(rangeVariableSyntax))
			{
				throw new ArgumentException(VBResources.RangeVariableSyntaxNotWithinSyntaxTree);
			}
			return null;
		}

		public virtual IRangeVariableSymbol GetDeclaredSymbol(CollectionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (rangeVariableSyntax == null)
			{
				throw new ArgumentNullException("rangeVariableSyntax");
			}
			if (!IsInTree(rangeVariableSyntax))
			{
				throw new ArgumentException(VBResources.RangeVariableSyntaxNotWithinSyntaxTree);
			}
			return null;
		}

		public virtual IRangeVariableSymbol GetDeclaredSymbol(AggregationRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (rangeVariableSyntax == null)
			{
				throw new ArgumentNullException("rangeVariableSyntax");
			}
			if (!IsInTree(rangeVariableSyntax))
			{
				throw new ArgumentException(VBResources.RangeVariableSyntaxNotWithinSyntaxTree);
			}
			return null;
		}

		public virtual ILabelSymbol GetDeclaredSymbol(LabelStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinSyntaxTree);
			}
			if (StripSemanticModelBinder(GetEnclosingBinder(declarationSyntax.SpanStart)) is BlockBaseBinder blockBaseBinder)
			{
				LabelSymbol labelSymbol = blockBaseBinder.LookupLabelByNameToken(declarationSyntax.LabelToken);
				if ((object)labelSymbol != null)
				{
					return labelSymbol;
				}
			}
			return null;
		}

		public abstract IFieldSymbol GetDeclaredSymbol(EnumMemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

		public abstract INamedTypeSymbol GetDeclaredSymbol(TypeStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

		public INamedTypeSymbol GetDeclaredSymbol(TypeBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDeclaredSymbol(declarationSyntax.BlockStatement, cancellationToken);
		}

		public abstract INamedTypeSymbol GetDeclaredSymbol(EnumStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

		public INamedTypeSymbol GetDeclaredSymbol(EnumBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDeclaredSymbol(declarationSyntax.EnumStatement, cancellationToken);
		}

		public abstract INamespaceSymbol GetDeclaredSymbol(NamespaceStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

		public INamespaceSymbol GetDeclaredSymbol(NamespaceBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDeclaredSymbol(declarationSyntax.NamespaceStatement, cancellationToken);
		}

		internal abstract ISymbol GetDeclaredSymbol(MethodBaseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

		public abstract IParameterSymbol GetDeclaredSymbol(ParameterSyntax parameter, CancellationToken cancellationToken = default(CancellationToken));

		public abstract ITypeParameterSymbol GetDeclaredSymbol(TypeParameterSyntax typeParameter, CancellationToken cancellationToken = default(CancellationToken));

		public NamedTypeSymbol GetDeclaredSymbol(DelegateStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (NamedTypeSymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public IMethodSymbol GetDeclaredSymbol(SubNewStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (MethodSymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public IMethodSymbol GetDeclaredSymbol(MethodStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (MethodSymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public IMethodSymbol GetDeclaredSymbol(DeclareStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (MethodSymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public IMethodSymbol GetDeclaredSymbol(OperatorStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (MethodSymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public IMethodSymbol GetDeclaredSymbol(MethodBlockBaseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (MethodSymbol)GetDeclaredSymbol(declarationSyntax.BlockStatement, cancellationToken);
		}

		public IPropertySymbol GetDeclaredSymbol(PropertyStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (PropertySymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public IEventSymbol GetDeclaredSymbol(EventStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (EventSymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public IPropertySymbol GetDeclaredSymbol(PropertyBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDeclaredSymbol(declarationSyntax.PropertyStatement, cancellationToken);
		}

		public IEventSymbol GetDeclaredSymbol(EventBlockSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDeclaredSymbol(declarationSyntax.EventStatement, cancellationToken);
		}

		public ILocalSymbol GetDeclaredSymbol(CatchStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (StripSemanticModelBinder(GetEnclosingBinder(declarationSyntax.SpanStart)) is CatchBlockBinder catchBlockBinder)
			{
				return catchBlockBinder.Locals.FirstOrDefault();
			}
			return null;
		}

		public IMethodSymbol GetDeclaredSymbol(AccessorStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (MethodSymbol)GetDeclaredSymbol((MethodBaseSyntax)declarationSyntax, cancellationToken);
		}

		public abstract IAliasSymbol GetDeclaredSymbol(SimpleImportsClauseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract ImmutableArray<ISymbol> GetDeclaredSymbols(FieldDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

		internal abstract BoundNodeSummary GetInvokeSummaryForRaiseEvent(RaiseEventStatementSyntax node);

		private SymbolInfo GetNamedArgumentSymbolInfo(IdentifierNameSyntax identifierNameSyntax, CancellationToken cancellationToken)
		{
			string valueText = identifierNameSyntax.Identifier.ValueText;
			if (valueText.Length == 0)
			{
				return SymbolInfo.None;
			}
			if (identifierNameSyntax.Parent.Parent.Parent.Parent.Kind() == SyntaxKind.RaiseEventStatement)
			{
				RaiseEventStatementSyntax containingRaiseEvent = (RaiseEventStatementSyntax)identifierNameSyntax.Parent.Parent.Parent.Parent;
				return GetNamedArgumentSymbolInfoInRaiseEvent(valueText, containingRaiseEvent);
			}
			ExpressionSyntax node = (ExpressionSyntax)identifierNameSyntax.Parent.Parent.Parent.Parent;
			SymbolInfo expressionSymbolInfo = GetExpressionSymbolInfo(node, SymbolInfoOptions.DefaultOptions, cancellationToken);
			return FindNameParameterInfo(expressionSymbolInfo.GetAllSymbols().Cast<Symbol>().ToImmutableArray(), valueText, expressionSymbolInfo.CandidateReason);
		}

		private SymbolInfo GetNamedArgumentSymbolInfoInRaiseEvent(string argumentName, RaiseEventStatementSyntax containingRaiseEvent)
		{
			BoundNodeSummary invokeSummaryForRaiseEvent = GetInvokeSummaryForRaiseEvent(containingRaiseEvent);
			LookupResultKind resultKind = LookupResultKind.Empty;
			ImmutableArray<Symbol> memberGroup = default(ImmutableArray<Symbol>);
			ImmutableArray<Symbol> semanticSymbols = GetSemanticSymbols(invokeSummaryForRaiseEvent, null, SymbolInfoOptions.DefaultOptions, ref resultKind, ref memberGroup);
			return FindNameParameterInfo(semanticSymbols, argumentName, (resultKind != LookupResultKind.Good) ? LookupResultKindExtensions.ToCandidateReason(resultKind) : CandidateReason.None);
		}

		private SymbolInfo FindNameParameterInfo(ImmutableArray<Symbol> invocationInfosymbols, string arGumentName, CandidateReason reason)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			ImmutableArray<Symbol>.Enumerator enumerator = invocationInfosymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				ParameterSymbol parameterSymbol = FindNamedParameter(current, arGumentName);
				if ((object)parameterSymbol != null)
				{
					instance.Add(parameterSymbol);
				}
			}
			if (instance.Count == 0)
			{
				instance.Free();
				return SymbolInfo.None;
			}
			return SymbolInfoFactory.Create(StaticCast<ISymbol>.From(instance.ToImmutableAndFree()), reason);
		}

		private ParameterSymbol FindNamedParameter(Symbol symbol, string argumentName)
		{
			ImmutableArray<ParameterSymbol> parameters;
			if (symbol.Kind == SymbolKind.Method)
			{
				parameters = ((MethodSymbol)symbol).Parameters;
			}
			else
			{
				if (symbol.Kind != SymbolKind.Property)
				{
					return null;
				}
				parameters = ((PropertySymbol)symbol).Parameters;
			}
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				if (CaseInsensitiveComparison.Equals(current.Name, argumentName))
				{
					return current;
				}
			}
			return null;
		}

		public ForEachStatementInfo GetForEachStatementInfo(ForEachStatementSyntax node)
		{
			if (node.Parent != null && node.Parent.Kind() == SyntaxKind.ForEachBlock)
			{
				return GetForEachStatementInfoWorker((ForEachBlockSyntax)node.Parent);
			}
			return default(ForEachStatementInfo);
		}

		public ForEachStatementInfo GetForEachStatementInfo(ForEachBlockSyntax node)
		{
			if (node.Kind() == SyntaxKind.ForEachBlock)
			{
				return GetForEachStatementInfoWorker(node);
			}
			return default(ForEachStatementInfo);
		}

		internal abstract ForEachStatementInfo GetForEachStatementInfoWorker(ForEachBlockSyntax node);

		public AwaitExpressionInfo GetAwaitExpressionInfo(AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckSyntaxNode(awaitExpression);
			if (CanGetSemanticInfo(awaitExpression))
			{
				return GetAwaitExpressionInfoWorker(awaitExpression, cancellationToken);
			}
			return default(AwaitExpressionInfo);
		}

		internal abstract AwaitExpressionInfo GetAwaitExpressionInfoWorker(AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken = default(CancellationToken));

		public VisualBasicPreprocessingSymbolInfo GetPreprocessingSymbolInfo(IdentifierNameSyntax node)
		{
			CheckSyntaxNode(node);
			if (SyntaxFacts.IsWithinPreprocessorConditionalExpression(node))
			{
				VisualBasicPreprocessingSymbolInfo preprocessingSymbolInfo = VisualBasicExtensions.GetPreprocessingSymbolInfo(node.SyntaxTree, node);
				if ((object)preprocessingSymbolInfo.Symbol != null)
				{
					return preprocessingSymbolInfo;
				}
				VisualBasicPreprocessingSymbolInfo result = new VisualBasicPreprocessingSymbolInfo(new PreprocessingSymbol(node.Identifier.ValueText), null, isDefined: false);
				return result;
			}
			return VisualBasicPreprocessingSymbolInfo.None;
		}

		internal void ValidateSymbolInfoOptions(SymbolInfoOptions options)
		{
		}

		public new ISymbol GetEnclosingSymbol(int position, CancellationToken cancellationToken = default(CancellationToken))
		{
			CheckPosition(position);
			return GetEnclosingBinder(position)?.ContainingMember;
		}

		internal static Binder StripSemanticModelBinder(Binder binder)
		{
			if (binder == null || !binder.IsSemanticModelBinder)
			{
				return binder;
			}
			return (binder is SemanticModelBinder) ? binder.ContainingBinder : binder;
		}

		private SymbolInfo GetSymbolInfoForNode(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node is ExpressionSyntax expression)
			{
				return GetSymbolInfo(expression, cancellationToken);
			}
			if (node is AttributeSyntax attribute)
			{
				return GetSymbolInfo(attribute, cancellationToken);
			}
			if (node is QueryClauseSyntax clauseSyntax)
			{
				return GetSymbolInfo(clauseSyntax, cancellationToken);
			}
			if (node is ExpressionRangeVariableSyntax variableSyntax)
			{
				return GetSymbolInfo(variableSyntax, cancellationToken);
			}
			if (node is OrderingSyntax orderingSyntax)
			{
				return GetSymbolInfo(orderingSyntax, cancellationToken);
			}
			if (node is FunctionAggregationSyntax functionSyntax)
			{
				return GetSymbolInfo(functionSyntax, cancellationToken);
			}
			if (node is CrefReferenceSyntax crefReference)
			{
				return GetSymbolInfo(crefReference, cancellationToken);
			}
			return SymbolInfo.None;
		}

		private VisualBasicTypeInfo GetTypeInfoForNode(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node is ExpressionSyntax expression)
			{
				return GetTypeInfoWorker(expression, cancellationToken);
			}
			if (node is AttributeSyntax attribute)
			{
				return GetTypeInfoWorker(attribute, cancellationToken);
			}
			return VisualBasicTypeInfo.None;
		}

		private ImmutableArray<ISymbol> GetMemberGroupForNode(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node is ExpressionSyntax expression)
			{
				return GetMemberGroup(expression, cancellationToken);
			}
			if (node is AttributeSyntax attribute)
			{
				return GetMemberGroup(attribute, cancellationToken);
			}
			return ImmutableArray<ISymbol>.Empty;
		}

		protected sealed override TypeInfo GetSpeculativeTypeInfoCore(int position, SyntaxNode expression, SpeculativeBindingOption bindingOption)
		{
			if (!(expression is ExpressionSyntax))
			{
				return default(TypeInfo);
			}
			return GetSpeculativeTypeInfo(position, (ExpressionSyntax)expression, bindingOption);
		}

		protected sealed override SymbolInfo GetSpeculativeSymbolInfoCore(int position, SyntaxNode expression, SpeculativeBindingOption bindingOption)
		{
			if (!(expression is ExpressionSyntax))
			{
				return default(SymbolInfo);
			}
			return GetSpeculativeSymbolInfo(position, (ExpressionSyntax)expression, bindingOption);
		}

		protected sealed override IAliasSymbol GetSpeculativeAliasInfoCore(int position, SyntaxNode nameSyntax, SpeculativeBindingOption bindingOption)
		{
			if (!(nameSyntax is IdentifierNameSyntax))
			{
				return null;
			}
			return GetSpeculativeAliasInfo(position, (IdentifierNameSyntax)nameSyntax, bindingOption);
		}

		protected sealed override SymbolInfo GetSymbolInfoCore(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetSymbolInfoForNode(node, cancellationToken);
		}

		protected sealed override TypeInfo GetTypeInfoCore(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetTypeInfoForNode(node, cancellationToken);
		}

		protected sealed override IAliasSymbol GetAliasInfoCore(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node is IdentifierNameSyntax nameSyntax)
			{
				return GetAliasInfo(nameSyntax, cancellationToken);
			}
			return null;
		}

		protected sealed override PreprocessingSymbolInfo GetPreprocessingSymbolInfoCore(SyntaxNode node)
		{
			if (node is IdentifierNameSyntax node2)
			{
				return GetPreprocessingSymbolInfo(node2);
			}
			return default(PreprocessingSymbolInfo);
		}

		protected sealed override ImmutableArray<ISymbol> GetMemberGroupCore(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberGroupForNode(node, cancellationToken);
		}

		protected sealed override ImmutableArray<ISymbol> LookupSymbolsCore(int position, INamespaceOrTypeSymbol container, string name, bool includeReducedExtensionMethods)
		{
			return LookupSymbols(position, ToLanguageSpecific(container), name, includeReducedExtensionMethods);
		}

		protected sealed override ImmutableArray<ISymbol> LookupBaseMembersCore(int position, string name)
		{
			return LookupBaseMembers(position, name);
		}

		protected sealed override ImmutableArray<ISymbol> LookupStaticMembersCore(int position, INamespaceOrTypeSymbol container, string name)
		{
			return LookupStaticMembers(position, ToLanguageSpecific(container), name);
		}

		protected sealed override ImmutableArray<ISymbol> LookupNamespacesAndTypesCore(int position, INamespaceOrTypeSymbol container, string name)
		{
			return LookupNamespacesAndTypes(position, ToLanguageSpecific(container), name);
		}

		protected sealed override ImmutableArray<ISymbol> LookupLabelsCore(int position, string name)
		{
			return LookupLabels(position, name);
		}

		private static NamespaceOrTypeSymbol ToLanguageSpecific(INamespaceOrTypeSymbol container)
		{
			if (container == null)
			{
				return null;
			}
			return (container as NamespaceOrTypeSymbol) ?? throw new ArgumentException(VBResources.NotAVbSymbol, "container");
		}

		protected sealed override ISymbol GetDeclaredSymbolCore(SyntaxNode declaration, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)declaration;
			switch (visualBasicSyntaxNode.Kind())
			{
			case SyntaxKind.SimpleImportsClause:
				return GetDeclaredSymbol((SimpleImportsClauseSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.TypedTupleElement:
			case SyntaxKind.NamedTupleElement:
				return GetDeclaredSymbol((TupleElementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.ModifiedIdentifier:
				return GetDeclaredSymbol((ModifiedIdentifierSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.EnumMemberDeclaration:
				return GetDeclaredSymbol((EnumMemberDeclarationSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.Parameter:
				return GetDeclaredSymbol((ParameterSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.TypeParameter:
				return GetDeclaredSymbol((TypeParameterSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.LabelStatement:
				return GetDeclaredSymbol((LabelStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.NamespaceStatement:
				return GetDeclaredSymbol((NamespaceStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.ModuleStatement:
			case SyntaxKind.StructureStatement:
			case SyntaxKind.InterfaceStatement:
			case SyntaxKind.ClassStatement:
				return GetDeclaredSymbol((TypeStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.EnumStatement:
				return GetDeclaredSymbol((EnumStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				return GetDeclaredSymbol((DelegateStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
				return GetDeclaredSymbol((MethodStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.PropertyStatement:
				return GetDeclaredSymbol((PropertyStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.EventStatement:
				return GetDeclaredSymbol((EventStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.SubNewStatement:
				return GetDeclaredSymbol((SubNewStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
				return GetDeclaredSymbol((AccessorStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.OperatorStatement:
				return GetDeclaredSymbol((MethodBaseSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.NamespaceBlock:
				return GetDeclaredSymbol((NamespaceBlockSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.ModuleBlock:
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
				return GetDeclaredSymbol((TypeBlockSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.EnumBlock:
				return GetDeclaredSymbol((EnumBlockSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.OperatorBlock:
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
				return GetDeclaredSymbol((MethodBlockBaseSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.PropertyBlock:
				return GetDeclaredSymbol((PropertyBlockSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.EventBlock:
				return GetDeclaredSymbol((EventBlockSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.CollectionRangeVariable:
				return GetDeclaredSymbol((CollectionRangeVariableSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.ExpressionRangeVariable:
				return GetDeclaredSymbol((ExpressionRangeVariableSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.AggregationRangeVariable:
				return GetDeclaredSymbol((AggregationRangeVariableSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.CatchStatement:
				return GetDeclaredSymbol((CatchStatementSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.InferredFieldInitializer:
			case SyntaxKind.NamedFieldInitializer:
				return GetDeclaredSymbol((FieldInitializerSyntax)visualBasicSyntaxNode, cancellationToken);
			case SyntaxKind.AnonymousObjectCreationExpression:
				return GetDeclaredSymbol((AnonymousObjectCreationExpressionSyntax)visualBasicSyntaxNode, cancellationToken);
			default:
				if (visualBasicSyntaxNode is TypeStatementSyntax declarationSyntax)
				{
					return GetDeclaredSymbol(declarationSyntax, cancellationToken);
				}
				if (visualBasicSyntaxNode is MethodBaseSyntax declarationSyntax2)
				{
					return GetDeclaredSymbol(declarationSyntax2, cancellationToken);
				}
				return null;
			}
		}

		protected sealed override ImmutableArray<ISymbol> GetDeclaredSymbolsCore(SyntaxNode declaration, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (declaration is FieldDeclarationSyntax declarationSyntax)
			{
				return GetDeclaredSymbols(declarationSyntax, cancellationToken);
			}
			ISymbol declaredSymbolCore = GetDeclaredSymbolCore(declaration, cancellationToken);
			if (declaredSymbolCore != null)
			{
				return ImmutableArray.Create(declaredSymbolCore);
			}
			return ImmutableArray.Create<ISymbol>();
		}

		protected sealed override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode firstStatement, SyntaxNode lastStatement)
		{
			return AnalyzeDataFlow(SafeCastArgument<StatementSyntax>(firstStatement, "firstStatement"), SafeCastArgument<StatementSyntax>(lastStatement, "lastStatement"));
		}

		protected sealed override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode statementOrExpression)
		{
			if (statementOrExpression == null)
			{
				throw new ArgumentNullException("statementOrExpression");
			}
			if (statementOrExpression is ExecutableStatementSyntax)
			{
				return AnalyzeDataFlow((StatementSyntax)statementOrExpression);
			}
			if (statementOrExpression is ExpressionSyntax)
			{
				return AnalyzeDataFlow((ExpressionSyntax)statementOrExpression);
			}
			throw new ArgumentException(VBResources.StatementOrExpressionIsNotAValidType);
		}

		protected sealed override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode firstStatement, SyntaxNode lastStatement)
		{
			return AnalyzeControlFlow(SafeCastArgument<StatementSyntax>(firstStatement, "firstStatement"), SafeCastArgument<StatementSyntax>(lastStatement, "lastStatement"));
		}

		protected sealed override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode statement)
		{
			return AnalyzeControlFlow(SafeCastArgument<StatementSyntax>(statement, "statement"));
		}

		private static T SafeCastArgument<T>(SyntaxNode node, string argName) where T : class
		{
			if (node == null)
			{
				throw new ArgumentNullException(argName);
			}
			return (node as T) ?? throw new ArgumentException(argName + " is not an " + typeof(T).Name);
		}

		protected sealed override Optional<object> GetConstantValueCore(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node is ExpressionSyntax)
			{
				return GetConstantValue((ExpressionSyntax)node, cancellationToken);
			}
			return default(Optional<object>);
		}

		protected sealed override ISymbol GetEnclosingSymbolCore(int position, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetEnclosingSymbol(position, cancellationToken);
		}

		protected sealed override bool IsAccessibleCore(int position, ISymbol symbol)
		{
			return IsAccessible(position, SymbolExtensions.EnsureVbSymbolOrNothing<ISymbol, Symbol>(symbol, "symbol"));
		}

		protected sealed override bool IsEventUsableAsFieldCore(int position, IEventSymbol symbol)
		{
			return false;
		}

		internal override void ComputeDeclarationsInSpan(TextSpan span, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken)
		{
			VisualBasicDeclarationComputer.ComputeDeclarationsInSpan(this, span, getSymbol, builder, cancellationToken);
		}

		internal override void ComputeDeclarationsInNode(SyntaxNode node, ISymbol associatedSymbol, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken, int? levelsToCompute = null)
		{
			VisualBasicDeclarationComputer.ComputeDeclarationsInNode(this, node, getSymbol, builder, cancellationToken);
		}

		protected override SyntaxNode GetTopmostNodeForDiagnosticAnalysis(ISymbol symbol, SyntaxNode declaringSyntax)
		{
			switch (symbol.Kind)
			{
			case SymbolKind.Namespace:
				if (declaringSyntax is NamespaceStatementSyntax && declaringSyntax.Parent != null && declaringSyntax.Parent is NamespaceBlockSyntax)
				{
					return declaringSyntax.Parent;
				}
				break;
			case SymbolKind.NamedType:
				if (declaringSyntax is TypeStatementSyntax && declaringSyntax.Parent != null && declaringSyntax.Parent is TypeBlockSyntax)
				{
					return declaringSyntax.Parent;
				}
				break;
			case SymbolKind.Method:
				if (declaringSyntax is MethodBaseSyntax && declaringSyntax.Parent != null && declaringSyntax.Parent is MethodBlockBaseSyntax)
				{
					return declaringSyntax.Parent;
				}
				break;
			case SymbolKind.Event:
				if (declaringSyntax is EventStatementSyntax && declaringSyntax.Parent != null && declaringSyntax.Parent is EventBlockSyntax)
				{
					return declaringSyntax.Parent;
				}
				break;
			case SymbolKind.Property:
				if (declaringSyntax is PropertyStatementSyntax && declaringSyntax.Parent != null && declaringSyntax.Parent is PropertyBlockSyntax)
				{
					return declaringSyntax.Parent;
				}
				break;
			case SymbolKind.Field:
			{
				FieldDeclarationSyntax fieldDeclarationSyntax = declaringSyntax.FirstAncestorOrSelf<FieldDeclarationSyntax>();
				if (fieldDeclarationSyntax != null)
				{
					return fieldDeclarationSyntax;
				}
				break;
			}
			}
			return declaringSyntax;
		}

		public sealed override NullableContext GetNullableContext(int position)
		{
			return NullableContext.ContextInherited;
		}

		internal string GetMessage(int position)
		{
			return $"{SyntaxTree.FilePath}: at {position}";
		}

		internal string GetMessage(VisualBasicSyntaxNode node)
		{
			if (node == null)
			{
				return SyntaxTree.FilePath;
			}
			return $"{SyntaxTree.FilePath}: {node.Kind().ToString()} ({node.Position})";
		}

		internal string GetMessage(VisualBasicSyntaxNode node, int position)
		{
			if (node == null)
			{
				return SyntaxTree.FilePath;
			}
			return $"{SyntaxTree.FilePath}: {node.Kind().ToString()} ({node.Position}) at {position}";
		}

		internal string GetMessage(StatementSyntax firstStatement, StatementSyntax lastStatement)
		{
			if (firstStatement == null || lastStatement == null)
			{
				return SyntaxTree.FilePath;
			}
			return $"{SyntaxTree.FilePath}: {firstStatement.Position} to {lastStatement.EndPosition}";
		}

		internal string GetMessage(ExpressionSyntax expression, TypeSymbol type)
		{
			if (expression == null || (object)type == null)
			{
				return SyntaxTree.FilePath;
			}
			return $"{SyntaxTree.FilePath}: {expression.Kind().ToString()} ({expression.Position}) -> {type.TypeKind.ToString()} {type.Name}";
		}

		internal string GetMessage(ExpressionSyntax expression, TypeSymbol type, int position)
		{
			if (expression == null || (object)type == null)
			{
				return SyntaxTree.FilePath;
			}
			return $"{SyntaxTree.FilePath}: {expression.Kind().ToString()} ({expression.Position}) -> {type.TypeKind.ToString()} {type.Name} at {position}";
		}

		internal string GetMessage(ExpressionSyntax expression, SpeculativeBindingOption option, int position)
		{
			if (expression == null)
			{
				return SyntaxTree.FilePath;
			}
			return $"{SyntaxTree.FilePath}: {expression.Kind().ToString()} ({expression.Position}) at {position} ({option.ToString()})";
		}

		internal string GetMessage(string name, LookupOptions option, int position)
		{
			return $"{SyntaxTree.FilePath}: {name} at {position} ({option.ToString()})";
		}

		internal string GetMessage(Symbol symbol, int position)
		{
			if ((object)symbol == null)
			{
				return SyntaxTree.FilePath;
			}
			return $"{SyntaxTree.FilePath}: {symbol.Kind.ToString()} {symbol.Name} at {position}";
		}

		internal string GetMessage(CompilationStage stage)
		{
			return $"{SyntaxTree.FilePath} ({stage.ToString()})";
		}
	}
}
