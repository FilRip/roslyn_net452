using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SyntaxTreeSemanticModel : VBSemanticModel
	{
		private readonly VisualBasicCompilation _compilation;

		private readonly SourceModuleSymbol _sourceModule;

		private readonly SyntaxTree _syntaxTree;

		private readonly BinderFactory _binderFactory;

		private readonly bool _ignoresAccessibility;

		private readonly ConcurrentDictionary<(Binder binder, bool ignoresAccessibility), MemberSemanticModel> _semanticModelCache;

		private readonly Func<(Binder binder, bool ignoresAccessibility), MemberSemanticModel> _methodBodySemanticModelCreator;

		private readonly Func<(Binder binder, bool ignoresAccessibility), MemberSemanticModel> _initializerSemanticModelCreator;

		private readonly Func<(Binder binder, bool ignoresAccessibility), MemberSemanticModel> _attributeSemanticModelCreator;

		public override VisualBasicCompilation Compilation => _compilation;

		internal override SyntaxNode Root => (VisualBasicSyntaxNode)_syntaxTree.GetRoot();

		public override SyntaxTree SyntaxTree => _syntaxTree;

		public sealed override bool IgnoresAccessibility => _ignoresAccessibility;

		public override bool IsSpeculativeSemanticModel => false;

		public override int OriginalPositionForSpeculation => 0;

		public override SemanticModel ParentModel => null;

		internal override SemanticModel ContainingModelOrSelf => this;

		internal SyntaxTreeSemanticModel(VisualBasicCompilation compilation, SourceModuleSymbol sourceModule, SyntaxTree syntaxTree, bool ignoreAccessibility = false)
		{
			_semanticModelCache = new ConcurrentDictionary<(Binder, bool), MemberSemanticModel>();
			_methodBodySemanticModelCreator = ((Binder binder, bool ignoresAccessibility) key) => MethodBodySemanticModel.Create(this, (SubOrFunctionBodyBinder)key.binder, key.ignoresAccessibility);
			_initializerSemanticModelCreator = ((Binder binder, bool ignoresAccessibility) key) => InitializerSemanticModel.Create(this, (DeclarationInitializerBinder)key.binder, key.ignoresAccessibility);
			_attributeSemanticModelCreator = ((Binder binder, bool ignoresAccessibility) key) => AttributeSemanticModel.Create(this, (AttributeBinder)key.binder, key.ignoresAccessibility);
			_compilation = compilation;
			_sourceModule = sourceModule;
			_syntaxTree = syntaxTree;
			_ignoresAccessibility = ignoreAccessibility;
			_binderFactory = new BinderFactory(sourceModule, syntaxTree);
		}

		public override ImmutableArray<Diagnostic> GetDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Compile, _syntaxTree, span, includeEarlierStages: true, cancellationToken);
		}

		public override ImmutableArray<Diagnostic> GetSyntaxDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Parse, _syntaxTree, span, includeEarlierStages: false, cancellationToken);
		}

		public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Declare, _syntaxTree, span, includeEarlierStages: false, cancellationToken);
		}

		public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Compile, _syntaxTree, span, includeEarlierStages: false, cancellationToken);
		}

		public MemberSemanticModel GetMemberSemanticModel(Binder binder)
		{
			if (binder is MethodBodyBinder)
			{
				return _semanticModelCache.GetOrAdd((binder, IgnoresAccessibility), _methodBodySemanticModelCreator);
			}
			if (binder is DeclarationInitializerBinder)
			{
				return _semanticModelCache.GetOrAdd((binder, IgnoresAccessibility), _initializerSemanticModelCreator);
			}
			if (binder is AttributeBinder)
			{
				return _semanticModelCache.GetOrAdd((binder, IgnoresAccessibility), _attributeSemanticModelCreator);
			}
			if (binder is TopLevelCodeBinder)
			{
				return _semanticModelCache.GetOrAdd((binder, IgnoresAccessibility), _methodBodySemanticModelCreator);
			}
			return null;
		}

		internal MemberSemanticModel GetMemberSemanticModel(int position)
		{
			Binder binderForPosition = _binderFactory.GetBinderForPosition(FindInitialNodeFromPosition(position), position);
			return GetMemberSemanticModel(binderForPosition);
		}

		internal MemberSemanticModel GetMemberSemanticModel(SyntaxNode node)
		{
			return GetMemberSemanticModel(node.SpanStart);
		}

		internal override Binder GetEnclosingBinder(int position)
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(position);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetEnclosingBinder(position);
			}
			return SemanticModelBinder.Mark(_binderFactory.GetBinderForPosition(FindInitialNodeFromPosition(position), position), IgnoresAccessibility);
		}

		internal override BoundNodeSummary GetInvokeSummaryForRaiseEvent(RaiseEventStatementSyntax node)
		{
			return GetMemberSemanticModel(node)?.GetInvokeSummaryForRaiseEvent(node) ?? default(BoundNodeSummary);
		}

		internal override SymbolInfo GetCrefReferenceSymbolInfo(CrefReferenceSyntax crefReference, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
		{
			ValidateSymbolInfoOptions(options);
			return GetSymbolInfoForCrefOrNameAttributeReference(crefReference, options);
		}

		internal override SymbolInfo GetExpressionSymbolInfo(ExpressionSyntax node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
		{
			ValidateSymbolInfoOptions(options);
			node = SyntaxFactory.GetStandaloneExpression(node);
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(node);
			SymbolInfo result;
			if (memberSemanticModel != null)
			{
				result = memberSemanticModel.GetExpressionSymbolInfo(node, options, cancellationToken);
				if (result.IsEmpty && SyntaxFacts.IsInNamespaceOrTypeContext(node))
				{
					SymbolInfo symbolInfo = TryBindNamespaceOrTypeAsExpression(node, options);
					if (!symbolInfo.IsEmpty)
					{
						result = symbolInfo;
					}
				}
			}
			else if (SyntaxFacts.IsImplementedMember(node))
			{
				result = GetImplementedMemberSymbolInfo((QualifiedNameSyntax)node, options);
			}
			else if (SyntaxFacts.IsHandlesEvent(node))
			{
				result = GetHandlesEventSymbolInfo((HandlesClauseItemSyntax)node.Parent, options);
			}
			else if (!SyntaxFacts.IsHandlesContainer(node))
			{
				result = (SyntaxFacts.IsHandlesProperty(node) ? GetHandlesPropertySymbolInfo((HandlesClauseItemSyntax)node.Parent.Parent, options) : (VBSemanticModel.IsInCrefOrNameAttributeInterior(node) ? GetSymbolInfoForCrefOrNameAttributeReference(node, options) : ((!SyntaxFacts.IsInNamespaceOrTypeContext(node)) ? SymbolInfo.None : GetTypeOrNamespaceSymbolInfoNotInMember((TypeSyntax)node, options))));
			}
			else
			{
				VisualBasicSyntaxNode parent = node.Parent;
				if (parent.Kind() != SyntaxKind.HandlesClauseItem)
				{
					parent = parent.Parent;
				}
				result = GetHandlesContainerSymbolInfo((HandlesClauseItemSyntax)parent, options);
			}
			return result;
		}

		internal override SymbolInfo GetCollectionInitializerAddSymbolInfo(ObjectCreationExpressionSyntax collectionInitializer, ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(collectionInitializer)?.GetCollectionInitializerAddSymbolInfo(collectionInitializer, node, cancellationToken) ?? SymbolInfo.None;
		}

		private SymbolInfo TryBindNamespaceOrTypeAsExpression(ExpressionSyntax node, SymbolInfoOptions options)
		{
			Binder enclosingBinder = GetEnclosingBinder(node.SpanStart);
			if (enclosingBinder != null)
			{
				BoundExpression boundExpression = enclosingBinder.BindExpression(node, BindingDiagnosticBag.Discarded);
				SymbolInfo symbolInfoForNode = GetSymbolInfoForNode(options, new BoundNodeSummary(boundExpression, boundExpression, null), null);
				if (!symbolInfoForNode.GetAllSymbols().IsDefaultOrEmpty)
				{
					return SymbolInfoFactory.Create(symbolInfoForNode.GetAllSymbols(), LookupResultKind.NotATypeOrNamespace);
				}
			}
			return SymbolInfo.None;
		}

		internal override VisualBasicTypeInfo GetExpressionTypeInfo(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			node = SyntaxFactory.GetStandaloneExpression(node);
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(node);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetExpressionTypeInfo(node, cancellationToken);
			}
			if (SyntaxFacts.IsImplementedMember(node))
			{
				return GetImplementedMemberTypeInfo((QualifiedNameSyntax)node);
			}
			if (SyntaxFacts.IsHandlesEvent(node))
			{
				return GetHandlesEventTypeInfo((IdentifierNameSyntax)node);
			}
			if (SyntaxFacts.IsHandlesContainer(node))
			{
				VisualBasicSyntaxNode parent = node.Parent;
				if (parent.Kind() != SyntaxKind.HandlesClauseItem)
				{
					parent = parent.Parent;
				}
				return GetHandlesContainerTypeInfo((HandlesClauseItemSyntax)parent);
			}
			if (SyntaxFacts.IsHandlesProperty(node))
			{
				return GetHandlesPropertyTypeInfo((HandlesClauseItemSyntax)node.Parent.Parent);
			}
			if (VBSemanticModel.IsInCrefOrNameAttributeInterior(node))
			{
				if (node is TypeSyntax name)
				{
					return GetTypeInfoForCrefOrNameAttributeReference(name);
				}
			}
			else if (SyntaxFacts.IsInNamespaceOrTypeContext(node))
			{
				return GetTypeOrNamespaceTypeInfoNotInMember((TypeSyntax)node);
			}
			return VisualBasicTypeInfo.None;
		}

		internal override ImmutableArray<Symbol> GetExpressionMemberGroup(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			node = SyntaxFactory.GetStandaloneExpression(node);
			return GetMemberSemanticModel(node)?.GetExpressionMemberGroup(node, cancellationToken) ?? ImmutableArray<Symbol>.Empty;
		}

		internal override ConstantValue GetExpressionConstantValue(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			node = SyntaxFactory.GetStandaloneExpression(node);
			return GetMemberSemanticModel(node)?.GetExpressionConstantValue(node, cancellationToken);
		}

		internal override IOperation GetOperationWorker(VisualBasicSyntaxNode node, CancellationToken cancellationToken)
		{
			return ((!(node is MethodBlockBaseSyntax methodBlockBaseSyntax)) ? GetMemberSemanticModel(node) : GetMemberSemanticModel(methodBlockBaseSyntax.BlockStatement.EndPosition))?.GetOperationWorker(node, cancellationToken);
		}

		internal override SymbolInfo GetAttributeSymbolInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(attribute)?.GetAttributeSymbolInfo(attribute, cancellationToken) ?? SymbolInfo.None;
		}

		internal override SymbolInfo GetQueryClauseSymbolInfo(QueryClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(node)?.GetQueryClauseSymbolInfo(node, cancellationToken) ?? SymbolInfo.None;
		}

		internal override SymbolInfo GetLetClauseSymbolInfo(ExpressionRangeVariableSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(node)?.GetLetClauseSymbolInfo(node, cancellationToken) ?? SymbolInfo.None;
		}

		internal override SymbolInfo GetOrderingSymbolInfo(OrderingSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(node)?.GetOrderingSymbolInfo(node, cancellationToken) ?? SymbolInfo.None;
		}

		internal override AggregateClauseSymbolInfo GetAggregateClauseSymbolInfoWorker(AggregateClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(node)?.GetAggregateClauseSymbolInfoWorker(node, cancellationToken) ?? new AggregateClauseSymbolInfo(SymbolInfo.None, SymbolInfo.None);
		}

		internal override CollectionRangeVariableSymbolInfo GetCollectionRangeVariableSymbolInfoWorker(CollectionRangeVariableSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(node)?.GetCollectionRangeVariableSymbolInfoWorker(node, cancellationToken) ?? CollectionRangeVariableSymbolInfo.None;
		}

		internal override VisualBasicTypeInfo GetAttributeTypeInfo(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(attribute)?.GetAttributeTypeInfo(attribute, cancellationToken) ?? VisualBasicTypeInfo.None;
		}

		internal override ImmutableArray<Symbol> GetAttributeMemberGroup(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(attribute)?.GetAttributeMemberGroup(attribute, cancellationToken) ?? ImmutableArray<Symbol>.Empty;
		}

		private Symbol GetTypeOrNamespaceSymbolNotInMember(TypeSyntax expression)
		{
			Binder enclosingBinder = GetEnclosingBinder(expression.SpanStart);
			if (SyntaxFacts.IsInTypeOnlyContext(expression))
			{
				return enclosingBinder.BindTypeOrAliasSyntax(expression, BindingDiagnosticBag.Discarded);
			}
			return enclosingBinder.BindNamespaceOrTypeOrAliasSyntax(expression, BindingDiagnosticBag.Discarded);
		}

		private SymbolInfo GetSymbolInfoForCrefOrNameAttributeReference(VisualBasicSyntaxNode node, SymbolInfoOptions options)
		{
			ImmutableArray<Symbol> typeParameters = default(ImmutableArray<Symbol>);
			ImmutableArray<Symbol> items = GetCrefOrNameAttributeReferenceSymbols(node, (options & SymbolInfoOptions.ResolveAliases) == 0, out typeParameters);
			if (items.IsDefaultOrEmpty)
			{
				if (typeParameters.IsDefaultOrEmpty)
				{
					return SymbolInfo.None;
				}
				return SymbolInfoFactory.Create(typeParameters, LookupResultKind.NotReferencable);
			}
			if (items.Length == 1)
			{
				SymbolInfo symbolInfoForSymbol = GetSymbolInfoForSymbol(items[0], options);
				if (symbolInfoForSymbol.CandidateReason == CandidateReason.None)
				{
					return symbolInfoForSymbol;
				}
				items = ImmutableArray<Symbol>.Empty;
			}
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			instance.AddRange(items);
			ImmutableArray<Symbol> symbols = RemoveErrorTypesAndDuplicates(instance, options);
			instance.Free();
			if (symbols.Length == 0)
			{
				return SymbolInfoFactory.Create(symbols, LookupResultKind.Empty);
			}
			return SymbolInfoFactory.Create(symbols, (symbols.Length == 1) ? LookupResultKind.Good : LookupResultKind.Ambiguous);
		}

		private VisualBasicTypeInfo GetTypeInfoForCrefOrNameAttributeReference(TypeSyntax name)
		{
			ImmutableArray<Symbol> typeParameters = default(ImmutableArray<Symbol>);
			ImmutableArray<Symbol> immutableArray = GetCrefOrNameAttributeReferenceSymbols(name, preserveAlias: false, out typeParameters);
			if (immutableArray.IsDefaultOrEmpty)
			{
				immutableArray = typeParameters;
				if (immutableArray.IsDefaultOrEmpty)
				{
					return VisualBasicTypeInfo.None;
				}
			}
			if (immutableArray.Length > 1)
			{
				return VisualBasicTypeInfo.None;
			}
			Symbol symbol = immutableArray[0];
			SymbolKind kind = symbol.Kind;
			if (kind == SymbolKind.ArrayType || kind == SymbolKind.NamedType || kind == SymbolKind.TypeParameter)
			{
				return GetTypeInfoForSymbol(symbol);
			}
			return VisualBasicTypeInfo.None;
		}

		private ImmutableArray<Symbol> GetCrefOrNameAttributeReferenceSymbols(VisualBasicSyntaxNode node, bool preserveAlias, out ImmutableArray<Symbol> typeParameters)
		{
			typeParameters = ImmutableArray<Symbol>.Empty;
			ImmutableArray<Symbol> result;
			if (node.Kind() == SyntaxKind.XmlString)
			{
				result = default(ImmutableArray<Symbol>);
			}
			else
			{
				VisualBasicSyntaxNode parent = node.Parent;
				BaseXmlAttributeSyntax baseXmlAttributeSyntax = null;
				while (true)
				{
					switch (parent.Kind())
					{
					case SyntaxKind.XmlCrefAttribute:
					case SyntaxKind.XmlNameAttribute:
						baseXmlAttributeSyntax = (BaseXmlAttributeSyntax)parent;
						goto IL_0051;
					default:
						goto IL_0051;
					case SyntaxKind.DocumentationCommentTrivia:
						break;
					}
					break;
					IL_0051:
					parent = parent.Parent;
				}
				if (baseXmlAttributeSyntax == null)
				{
					result = default(ImmutableArray<Symbol>);
				}
				else
				{
					bool flag = baseXmlAttributeSyntax.Kind() == SyntaxKind.XmlCrefAttribute;
					SyntaxTrivia parentTrivia = ((DocumentationCommentTriviaSyntax)parent).ParentTrivia;
					if (VisualBasicExtensions.Kind(parentTrivia) == SyntaxKind.None)
					{
						result = default(ImmutableArray<Symbol>);
					}
					else
					{
						if (VisualBasicExtensions.Kind(parentTrivia.Token) != 0)
						{
							Binder binderForPosition = _binderFactory.GetBinderForPosition(node, node.SpanStart);
							binderForPosition = SemanticModelBinder.Mark(binderForPosition, IgnoresAccessibility);
							CompoundUseSiteInfo<AssemblySymbol> useSiteInfo;
							if (flag)
							{
								bool flag2;
								ImmutableArray<Symbol> immutableArray;
								if (node.Kind() == SyntaxKind.CrefReference)
								{
									flag2 = true;
									Binder binder = binderForPosition;
									CrefReferenceSyntax reference = (CrefReferenceSyntax)node;
									useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
									immutableArray = binder.BindInsideCrefAttributeValue(reference, preserveAlias, null, ref useSiteInfo);
								}
								else
								{
									flag2 = node.Parent != null && node.Parent.Kind() == SyntaxKind.CrefReference;
									Binder binder2 = binderForPosition;
									TypeSyntax name = (TypeSyntax)node;
									useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
									immutableArray = binder2.BindInsideCrefAttributeValue(name, preserveAlias, null, ref useSiteInfo);
								}
								if (flag2)
								{
									ArrayBuilder<Symbol> arrayBuilder = null;
									ArrayBuilder<Symbol> arrayBuilder2 = null;
									int num = immutableArray.Length - 1;
									for (int i = 0; i <= num; i++)
									{
										Symbol symbol = immutableArray[i];
										if (symbol.Kind == SymbolKind.TypeParameter)
										{
											if (arrayBuilder == null)
											{
												arrayBuilder = ArrayBuilder<Symbol>.GetInstance(i);
												arrayBuilder2 = ArrayBuilder<Symbol>.GetInstance();
												arrayBuilder.AddRange(immutableArray, i);
											}
											arrayBuilder2.Add((TypeParameterSymbol)symbol);
										}
										else
										{
											arrayBuilder?.Add(symbol);
										}
									}
									if (arrayBuilder != null)
									{
										immutableArray = arrayBuilder.ToImmutableAndFree();
										typeParameters = arrayBuilder2.ToImmutableAndFree();
									}
								}
								return immutableArray;
							}
							Binder binder3 = binderForPosition;
							IdentifierNameSyntax identifier = (IdentifierNameSyntax)node;
							useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
							return binder3.BindXmlNameAttributeValue(identifier, ref useSiteInfo);
						}
						result = default(ImmutableArray<Symbol>);
					}
				}
			}
			return result;
		}

		private SymbolInfo GetTypeOrNamespaceSymbolInfoNotInMember(TypeSyntax expression, SymbolInfoOptions options)
		{
			Symbol symbol = GetTypeOrNamespaceSymbolNotInMember(expression);
			if (symbol.Kind == SymbolKind.Namespace && expression.Parent != null && expression.Parent.Kind() == SyntaxKind.QualifiedName && ((QualifiedNameSyntax)expression.Parent).Left == expression)
			{
				NamespaceSymbol namespaceSymbol = (NamespaceSymbol)symbol;
				if (namespaceSymbol.NamespaceKind == (NamespaceKind)0)
				{
					SymbolInfo typeOrNamespaceSymbolInfoNotInMember = GetTypeOrNamespaceSymbolInfoNotInMember((QualifiedNameSyntax)expression.Parent, (SymbolInfoOptions)0);
					if (!typeOrNamespaceSymbolInfoNotInMember.IsEmpty)
					{
						SmallDictionary<NamespaceSymbol, bool> smallDictionary = new SmallDictionary<NamespaceSymbol, bool>();
						if (typeOrNamespaceSymbolInfoNotInMember.Symbol != null)
						{
							if (!Binder.AddReceiverNamespaces(smallDictionary, (Symbol)typeOrNamespaceSymbolInfoNotInMember.Symbol, Compilation))
							{
								smallDictionary = null;
							}
						}
						else
						{
							ImmutableArray<ISymbol>.Enumerator enumerator = typeOrNamespaceSymbolInfoNotInMember.CandidateSymbols.GetEnumerator();
							while (enumerator.MoveNext())
							{
								ISymbol current = enumerator.Current;
								if (!Binder.AddReceiverNamespaces(smallDictionary, (Symbol)current, Compilation))
								{
									smallDictionary = null;
									break;
								}
							}
						}
						if (smallDictionary != null && smallDictionary.Count() < namespaceSymbol.ConstituentNamespaces.Length)
						{
							symbol = ((MergedNamespaceSymbol)namespaceSymbol).Shrink(smallDictionary.Keys);
						}
					}
				}
			}
			SymbolInfo result = GetSymbolInfoForSymbol(symbol, options);
			if (result.IsEmpty)
			{
				SymbolInfo symbolInfo = TryBindNamespaceOrTypeAsExpression(expression, options);
				if (!symbolInfo.IsEmpty)
				{
					result = symbolInfo;
				}
			}
			return result;
		}

		private VisualBasicTypeInfo GetTypeOrNamespaceTypeInfoNotInMember(TypeSyntax expression)
		{
			Symbol typeOrNamespaceSymbolNotInMember = GetTypeOrNamespaceSymbolNotInMember(expression);
			return GetTypeInfoForSymbol(typeOrNamespaceSymbolNotInMember);
		}

		private LookupResultKind GetImplementedMemberAndResultKind(ArrayBuilder<Symbol> symbolBuilder, QualifiedNameSyntax memberName)
		{
			LookupResultKind resultKind = LookupResultKind.Good;
			Binder enclosingBinder = GetEnclosingBinder(memberName.SpanStart);
			if (memberName.Parent.Parent is MethodBaseSyntax declarationSyntax)
			{
				ISymbol declaredSymbol = GetDeclaredSymbol(declarationSyntax);
				if (declaredSymbol != null)
				{
					switch (declaredSymbol.Kind)
					{
					case SymbolKind.Method:
						ImplementsHelper.FindExplicitlyImplementedMember((MethodSymbol)declaredSymbol, ((MethodSymbol)declaredSymbol).ContainingType, memberName, enclosingBinder, BindingDiagnosticBag.Discarded, symbolBuilder, ref resultKind);
						break;
					case SymbolKind.Property:
						ImplementsHelper.FindExplicitlyImplementedMember((PropertySymbol)declaredSymbol, ((PropertySymbol)declaredSymbol).ContainingType, memberName, enclosingBinder, BindingDiagnosticBag.Discarded, symbolBuilder, ref resultKind);
						break;
					case SymbolKind.Event:
						ImplementsHelper.FindExplicitlyImplementedMember((EventSymbol)declaredSymbol, ((EventSymbol)declaredSymbol).ContainingType, memberName, enclosingBinder, BindingDiagnosticBag.Discarded, symbolBuilder, ref resultKind);
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(declaredSymbol.Kind);
					}
				}
			}
			return resultKind;
		}

		private LookupResultKind GetHandledEventOrContainerSymbolsAndResultKind(ArrayBuilder<Symbol> eventSymbolBuilder, ArrayBuilder<Symbol> containerSymbolBuilder, ArrayBuilder<Symbol> propertySymbolBuilder, HandlesClauseItemSyntax handlesClause)
		{
			LookupResultKind resultKind = LookupResultKind.Good;
			Binder enclosingBinder = GetEnclosingBinder(handlesClause.SpanStart);
			if (handlesClause.Parent.Parent is MethodStatementSyntax declarationSyntax)
			{
				IMethodSymbol declaredSymbol = GetDeclaredSymbol(declarationSyntax);
				if (declaredSymbol != null)
				{
					((SourceMemberMethodSymbol)declaredSymbol).BindSingleHandlesClause(handlesClause, enclosingBinder, BindingDiagnosticBag.Discarded, eventSymbolBuilder, containerSymbolBuilder, propertySymbolBuilder, ref resultKind);
				}
			}
			return resultKind;
		}

		private SymbolInfo GetImplementedMemberSymbolInfo(QualifiedNameSyntax memberName, SymbolInfoOptions options)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			LookupResultKind implementedMemberAndResultKind = GetImplementedMemberAndResultKind(instance, memberName);
			ImmutableArray<Symbol> symbols = RemoveErrorTypesAndDuplicates(instance, options);
			instance.Free();
			return SymbolInfoFactory.Create(symbols, implementedMemberAndResultKind);
		}

		private SymbolInfo GetHandlesEventSymbolInfo(HandlesClauseItemSyntax handlesClause, SymbolInfoOptions options)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			LookupResultKind handledEventOrContainerSymbolsAndResultKind = GetHandledEventOrContainerSymbolsAndResultKind(instance, null, null, handlesClause);
			ImmutableArray<Symbol> symbols = RemoveErrorTypesAndDuplicates(instance, options);
			instance.Free();
			return SymbolInfoFactory.Create(symbols, handledEventOrContainerSymbolsAndResultKind);
		}

		private SymbolInfo GetHandlesContainerSymbolInfo(HandlesClauseItemSyntax handlesClause, SymbolInfoOptions options)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			LookupResultKind handledEventOrContainerSymbolsAndResultKind = GetHandledEventOrContainerSymbolsAndResultKind(null, instance, null, handlesClause);
			ImmutableArray<Symbol> symbols = RemoveErrorTypesAndDuplicates(instance, options);
			instance.Free();
			return SymbolInfoFactory.Create(symbols, handledEventOrContainerSymbolsAndResultKind);
		}

		private SymbolInfo GetHandlesPropertySymbolInfo(HandlesClauseItemSyntax handlesClause, SymbolInfoOptions options)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			LookupResultKind handledEventOrContainerSymbolsAndResultKind = GetHandledEventOrContainerSymbolsAndResultKind(null, null, instance, handlesClause);
			ImmutableArray<Symbol> symbols = RemoveErrorTypesAndDuplicates(instance, options);
			instance.Free();
			return SymbolInfoFactory.Create(symbols, handledEventOrContainerSymbolsAndResultKind);
		}

		private VisualBasicTypeInfo GetImplementedMemberTypeInfo(QualifiedNameSyntax memberName)
		{
			return VisualBasicTypeInfo.None;
		}

		private VisualBasicTypeInfo GetHandlesEventTypeInfo(IdentifierNameSyntax memberName)
		{
			return VisualBasicTypeInfo.None;
		}

		private VisualBasicTypeInfo GetHandlesContainerTypeInfo(HandlesClauseItemSyntax memberName)
		{
			return VisualBasicTypeInfo.None;
		}

		private VisualBasicTypeInfo GetHandlesPropertyTypeInfo(HandlesClauseItemSyntax memberName)
		{
			return VisualBasicTypeInfo.None;
		}

		private NamedTypeSymbol CheckSymbolLocationsAgainstSyntax(NamedTypeSymbol symbol, VisualBasicSyntaxNode nodeToCheck)
		{
			ImmutableArray<Location>.Enumerator enumerator = symbol.Locations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Location current = enumerator.Current;
				if (current.SourceTree == SyntaxTree && nodeToCheck.Span.Contains(current.SourceSpan))
				{
					return symbol;
				}
			}
			return null;
		}

		public new NamedTypeSymbol GetDeclaredSymbol(DelegateStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			Binder namedTypeBinder = _binderFactory.GetNamedTypeBinder(declarationSyntax);
			if (namedTypeBinder != null && namedTypeBinder is NamedTypeBinder)
			{
				return CheckSymbolLocationsAgainstSyntax(namedTypeBinder.ContainingType, declarationSyntax);
			}
			return null;
		}

		public override INamedTypeSymbol GetDeclaredSymbol(TypeStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			Binder namedTypeBinder = _binderFactory.GetNamedTypeBinder(declarationSyntax);
			if (namedTypeBinder != null && namedTypeBinder is NamedTypeBinder)
			{
				return CheckSymbolLocationsAgainstSyntax(namedTypeBinder.ContainingType, declarationSyntax);
			}
			return null;
		}

		public override INamedTypeSymbol GetDeclaredSymbol(EnumStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			Binder namedTypeBinder = _binderFactory.GetNamedTypeBinder(declarationSyntax);
			if (namedTypeBinder != null && namedTypeBinder is NamedTypeBinder)
			{
				return CheckSymbolLocationsAgainstSyntax(namedTypeBinder.ContainingType, declarationSyntax);
			}
			return null;
		}

		public override INamespaceSymbol GetDeclaredSymbol(NamespaceStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			if (declarationSyntax.Parent is NamespaceBlockSyntax node)
			{
				Binder namespaceBinder = _binderFactory.GetNamespaceBinder(node);
				if (namespaceBinder != null && namespaceBinder is NamespaceBinder)
				{
					return (NamespaceSymbol)namespaceBinder.ContainingNamespaceOrType;
				}
			}
			return null;
		}

		internal override ISymbol GetDeclaredSymbol(MethodBaseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			if (declarationSyntax.Kind() == SyntaxKind.DelegateFunctionStatement || declarationSyntax.Kind() == SyntaxKind.DelegateSubStatement)
			{
				return GetDeclaredSymbol((DelegateStatementSyntax)declarationSyntax, cancellationToken);
			}
			if (declarationSyntax.Parent is StatementSyntax statementSyntax)
			{
				TypeBlockSyntax typeBlockSyntax = null;
				switch (statementSyntax.Kind())
				{
				case SyntaxKind.ModuleBlock:
				case SyntaxKind.StructureBlock:
				case SyntaxKind.InterfaceBlock:
				case SyntaxKind.ClassBlock:
				case SyntaxKind.EnumBlock:
					typeBlockSyntax = statementSyntax as TypeBlockSyntax;
					break;
				case SyntaxKind.SubBlock:
				case SyntaxKind.FunctionBlock:
				case SyntaxKind.ConstructorBlock:
				case SyntaxKind.OperatorBlock:
				case SyntaxKind.PropertyBlock:
				case SyntaxKind.EventBlock:
				{
					typeBlockSyntax = statementSyntax.Parent as TypeBlockSyntax;
					if (typeBlockSyntax != null || statementSyntax.Parent == null)
					{
						break;
					}
					INamespaceSymbol namespaceSymbol = null;
					switch (statementSyntax.Parent.Kind())
					{
					case SyntaxKind.CompilationUnit:
						namespaceSymbol = _sourceModule.RootNamespace;
						break;
					case SyntaxKind.NamespaceBlock:
						namespaceSymbol = GetDeclaredSymbol((NamespaceBlockSyntax)statementSyntax.Parent, cancellationToken);
						break;
					}
					if (namespaceSymbol != null)
					{
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)namespaceSymbol.GetMembers("<invalid-global-code>").SingleOrDefault();
						if ((object)namedTypeSymbol != null)
						{
							return SourceMethodSymbol.FindSymbolFromSyntax(declarationSyntax, _syntaxTree, namedTypeSymbol);
						}
					}
					break;
				}
				case SyntaxKind.GetAccessorBlock:
				case SyntaxKind.SetAccessorBlock:
				case SyntaxKind.AddHandlerAccessorBlock:
				case SyntaxKind.RemoveHandlerAccessorBlock:
				case SyntaxKind.RaiseEventAccessorBlock:
					if (statementSyntax.Parent != null)
					{
						typeBlockSyntax = statementSyntax.Parent.Parent as TypeBlockSyntax;
					}
					break;
				default:
					return null;
				}
				if (typeBlockSyntax != null)
				{
					NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)GetDeclaredSymbol(typeBlockSyntax.BlockStatement, cancellationToken);
					if ((object)namedTypeSymbol2 != null)
					{
						return SourceMethodSymbol.FindSymbolFromSyntax(declarationSyntax, _syntaxTree, namedTypeSymbol2);
					}
				}
			}
			return null;
		}

		public override IParameterSymbol GetDeclaredSymbol(ParameterSyntax parameter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (parameter == null)
			{
				throw new ArgumentNullException("parameter");
			}
			if (parameter.Parent is ParameterListSyntax parameterListSyntax && parameterListSyntax.Parent is MethodBaseSyntax methodBaseSyntax)
			{
				ISymbol declaredSymbol = GetDeclaredSymbol(methodBaseSyntax, cancellationToken);
				if (declaredSymbol != null)
				{
					switch (declaredSymbol.Kind)
					{
					case SymbolKind.Method:
						return MethodSymbolExtensions.GetParameterSymbol(((MethodSymbol)declaredSymbol).Parameters, parameter);
					case SymbolKind.Event:
					{
						EventSymbol eventSymbol = (EventSymbol)declaredSymbol;
						NamedTypeSymbol namedTypeSymbol2 = eventSymbol.Type as NamedTypeSymbol;
						if ((object)namedTypeSymbol2?.AssociatedSymbol == eventSymbol)
						{
							return MethodSymbolExtensions.GetParameterSymbol(namedTypeSymbol2.DelegateInvokeMethod.Parameters, parameter);
						}
						return null;
					}
					case SymbolKind.Property:
						return MethodSymbolExtensions.GetParameterSymbol(((PropertySymbol)declaredSymbol).Parameters, parameter);
					case SymbolKind.NamedType:
					{
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)declaredSymbol;
						if ((object)namedTypeSymbol.DelegateInvokeMethod != null)
						{
							return MethodSymbolExtensions.GetParameterSymbol(namedTypeSymbol.DelegateInvokeMethod.Parameters, parameter);
						}
						break;
					}
					}
				}
				else if (methodBaseSyntax is LambdaHeaderSyntax)
				{
					MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(methodBaseSyntax);
					if (memberSemanticModel != null)
					{
						return memberSemanticModel.GetDeclaredSymbol(parameter, cancellationToken);
					}
				}
			}
			return null;
		}

		public override ITypeParameterSymbol GetDeclaredSymbol(TypeParameterSyntax typeParameter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (typeParameter == null)
			{
				throw new ArgumentNullException("typeParameter");
			}
			if (!IsInTree(typeParameter))
			{
				throw new ArgumentException(VBResources.TypeParameterNotWithinTree);
			}
			ISymbol symbol = null;
			if (typeParameter.Parent is TypeParameterListSyntax typeParameterListSyntax && typeParameterListSyntax.Parent != null)
			{
				if (typeParameterListSyntax.Parent is MethodStatementSyntax)
				{
					symbol = GetDeclaredSymbol((MethodStatementSyntax)typeParameterListSyntax.Parent, cancellationToken);
				}
				else if (typeParameterListSyntax.Parent is TypeStatementSyntax)
				{
					symbol = GetDeclaredSymbol((TypeStatementSyntax)typeParameterListSyntax.Parent, cancellationToken);
				}
				else if (typeParameterListSyntax.Parent is DelegateStatementSyntax)
				{
					symbol = GetDeclaredSymbol((DelegateStatementSyntax)typeParameterListSyntax.Parent, cancellationToken);
				}
				if (symbol != null)
				{
					if (symbol is NamedTypeSymbol namedTypeSymbol)
					{
						return GetTypeParameterSymbol(namedTypeSymbol.TypeParameters, typeParameter);
					}
					if (symbol is MethodSymbol methodSymbol)
					{
						return GetTypeParameterSymbol(methodSymbol.TypeParameters, typeParameter);
					}
				}
			}
			return null;
		}

		private TypeParameterSymbol GetTypeParameterSymbol(ImmutableArray<TypeParameterSymbol> parameters, TypeParameterSyntax parameter)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Location current2 = enumerator2.Current;
					if (current2.IsInSource && current2.SourceTree == _syntaxTree && parameter.Span.Contains(current2.SourceSpan))
					{
						return current;
					}
				}
			}
			return null;
		}

		public override IFieldSymbol GetDeclaredSymbol(EnumMemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			EnumBlockSyntax enumBlockSyntax = (EnumBlockSyntax)declarationSyntax.Parent;
			if (enumBlockSyntax != null)
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)GetDeclaredSymbol(enumBlockSyntax.EnumStatement, cancellationToken);
				if ((object)namedTypeSymbol != null)
				{
					return (FieldSymbol)SourceFieldSymbol.FindFieldOrWithEventsSymbolFromSyntax(declarationSyntax.Identifier, _syntaxTree, namedTypeSymbol);
				}
			}
			return null;
		}

		public override ISymbol GetDeclaredSymbol(ModifiedIdentifierSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			VisualBasicSyntaxNode parent = declarationSyntax.Parent;
			FieldDeclarationSyntax fieldDeclarationSyntax = null;
			if (parent != null)
			{
				fieldDeclarationSyntax = parent.Parent as FieldDeclarationSyntax;
			}
			TypeBlockSyntax typeBlockSyntax = null;
			if (fieldDeclarationSyntax != null)
			{
				typeBlockSyntax = fieldDeclarationSyntax.Parent as TypeBlockSyntax;
			}
			if (typeBlockSyntax != null)
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)GetDeclaredSymbol(typeBlockSyntax.BlockStatement, cancellationToken);
				if ((object)namedTypeSymbol != null)
				{
					return SourceFieldSymbol.FindFieldOrWithEventsSymbolFromSyntax(declarationSyntax.Identifier, _syntaxTree, namedTypeSymbol);
				}
			}
			if (parent is ParameterSyntax parameter)
			{
				return GetDeclaredSymbol(parameter, cancellationToken);
			}
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(declarationSyntax);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
			}
			return base.GetDeclaredSymbol(declarationSyntax, cancellationToken);
		}

		public override IPropertySymbol GetDeclaredSymbol(FieldInitializerSyntax fieldInitializerSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(fieldInitializerSyntax);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetDeclaredSymbol(fieldInitializerSyntax, cancellationToken);
			}
			return base.GetDeclaredSymbol(fieldInitializerSyntax, cancellationToken);
		}

		public override INamedTypeSymbol GetDeclaredSymbol(AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(anonymousObjectCreationExpressionSyntax);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetDeclaredSymbol(anonymousObjectCreationExpressionSyntax, cancellationToken);
			}
			return base.GetDeclaredSymbol(anonymousObjectCreationExpressionSyntax, cancellationToken);
		}

		public override IRangeVariableSymbol GetDeclaredSymbol(ExpressionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(rangeVariableSyntax);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
			}
			return base.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
		}

		public override IRangeVariableSymbol GetDeclaredSymbol(CollectionRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(rangeVariableSyntax);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
			}
			return base.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
		}

		public override IRangeVariableSymbol GetDeclaredSymbol(AggregationRangeVariableSyntax rangeVariableSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(rangeVariableSyntax);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
			}
			return base.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken);
		}

		public override IAliasSymbol GetDeclaredSymbol(SimpleImportsClauseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			if (declarationSyntax.Alias == null)
			{
				return null;
			}
			string valueText = declarationSyntax.Alias.Identifier.ValueText;
			if (!string.IsNullOrEmpty(valueText))
			{
				IReadOnlyDictionary<string, AliasAndImportsClausePosition> aliasImportsOpt = _sourceModule.TryGetSourceFile(SyntaxTree).AliasImportsOpt;
				AliasAndImportsClausePosition value = default(AliasAndImportsClausePosition);
				if (aliasImportsOpt != null && aliasImportsOpt.TryGetValue(valueText, out value))
				{
					ImmutableArray<Location>.Enumerator enumerator = value.Alias.Locations.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Location current = enumerator.Current;
						if (current.IsInSource && current.SourceTree == _syntaxTree && declarationSyntax.Span.Contains(current.SourceSpan))
						{
							return value.Alias;
						}
					}
					Binder enclosingBinder = GetEnclosingBinder(declarationSyntax.SpanStart);
					NamespaceOrTypeSymbol namespaceOrTypeSymbol = enclosingBinder.BindNamespaceOrTypeSyntax(declarationSyntax.Name, BindingDiagnosticBag.Discarded);
					if ((object)namespaceOrTypeSymbol != null)
					{
						return new AliasSymbol(enclosingBinder.Compilation, enclosingBinder.ContainingNamespaceOrType, valueText, namespaceOrTypeSymbol, declarationSyntax.GetLocation());
					}
				}
			}
			return null;
		}

		internal override ImmutableArray<ISymbol> GetDeclaredSymbols(FieldDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (declarationSyntax == null)
			{
				throw new ArgumentNullException("declarationSyntax");
			}
			if (!IsInTree(declarationSyntax))
			{
				throw new ArgumentException(VBResources.DeclarationSyntaxNotWithinTree);
			}
			ArrayBuilder<ISymbol> arrayBuilder = new ArrayBuilder<ISymbol>();
			SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = declarationSyntax.Declarators.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SeparatedSyntaxList<ModifiedIdentifierSyntax>.Enumerator enumerator2 = enumerator.Current.Names.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ModifiedIdentifierSyntax current = enumerator2.Current;
					if (GetDeclaredSymbol(current, cancellationToken) is IFieldSymbol item)
					{
						arrayBuilder.Add(item);
					}
				}
			}
			return arrayBuilder.ToImmutableAndFree();
		}

		public override Conversion ClassifyConversion(ExpressionSyntax expression, ITypeSymbol destination)
		{
			CheckSyntaxNode(expression);
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			TypeSymbol destination2 = SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(destination, "destination");
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(expression);
			if (memberSemanticModel == null)
			{
				Conversion result = new Conversion(default(KeyValuePair<ConversionKind, MethodSymbol>));
				return result;
			}
			return memberSemanticModel.ClassifyConversion(expression, destination2);
		}

		internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, MethodBlockBaseSyntax method, out SemanticModel speculativeModel)
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(position);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.TryGetSpeculativeSemanticModelForMethodBodyCore(parentModel, position, method, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, TypeSyntax type, SpeculativeBindingOption bindingOption, out SemanticModel speculativeModel)
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(position);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, type, bindingOption, out speculativeModel);
			}
			Binder speculativeBinderForExpression = GetSpeculativeBinderForExpression(position, type, bindingOption);
			if (speculativeBinderForExpression != null)
			{
				speculativeModel = SpeculativeSyntaxTreeSemanticModel.Create(this, type, speculativeBinderForExpression, position, bindingOption);
				return true;
			}
			speculativeModel = null;
			return false;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, RangeArgumentSyntax rangeArgument, out SemanticModel speculativeModel)
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(position);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, rangeArgument, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ExecutableStatementSyntax statement, out SemanticModel speculativeModel)
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(position);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, statement, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueSyntax initializer, out SemanticModel speculativeModel)
		{
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(position);
			if (memberSemanticModel != null)
			{
				return memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, initializer, out speculativeModel);
			}
			speculativeModel = null;
			return false;
		}

		public override ControlFlowAnalysis AnalyzeControlFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
		{
			RegionAnalysisContext context = (ValidateRegionDefiningStatementsRange(firstStatement, lastStatement) ? CreateRegionAnalysisContext(firstStatement, lastStatement) : CreateFailedRegionAnalysisContext());
			return new VisualBasicControlFlowAnalysis(context);
		}

		public override DataFlowAnalysis AnalyzeDataFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
		{
			RegionAnalysisContext context = (ValidateRegionDefiningStatementsRange(firstStatement, lastStatement) ? CreateRegionAnalysisContext(firstStatement, lastStatement) : CreateFailedRegionAnalysisContext());
			return new VisualBasicDataFlowAnalysis(context);
		}

		public override DataFlowAnalysis AnalyzeDataFlow(ExpressionSyntax expression)
		{
			RegionAnalysisContext context = (ValidateRegionDefiningExpression(expression) ? CreateRegionAnalysisContext(expression) : CreateFailedRegionAnalysisContext());
			return new VisualBasicDataFlowAnalysis(context);
		}

		[Conditional("DEBUG")]
		private void CheckSucceededFlagInAnalyzeDataFlow(ExpressionSyntax expression, VisualBasicDataFlowAnalysis result, RegionAnalysisContext context)
		{
			if (result.Succeeded || result.InvalidRegionDetectedInternal || context.Failed)
			{
				return;
			}
			VisualBasicSyntaxNode parent = expression.Parent;
			if (expression.Kind() == SyntaxKind.IdentifierName && parent != null && parent.Kind() == SyntaxKind.SimpleMemberAccessExpression && ((MemberAccessExpressionSyntax)parent).Expression == expression)
			{
				return;
			}
			if (expression.Kind() == SyntaxKind.NumericLiteralExpression && parent != null && parent.Kind() == SyntaxKind.SimpleArgument && !((SimpleArgumentSyntax)parent).IsNamed)
			{
				VisualBasicSyntaxNode parent2 = parent.Parent;
				if (parent2 != null && parent2.Kind() == SyntaxKind.ArgumentList)
				{
					VisualBasicSyntaxNode parent3 = parent2.Parent;
					if (parent3 != null && parent3.Kind() == SyntaxKind.ModifiedIdentifier)
					{
						VisualBasicSyntaxNode parent4 = parent3.Parent;
						if (parent4 != null && parent4.Kind() == SyntaxKind.VariableDeclarator && ((VariableDeclaratorSyntax)parent4).Initializer != null)
						{
							return;
						}
					}
				}
			}
			throw ExceptionUtilities.Unreachable;
		}

		private static bool IsNodeInsideAttributeArguments(VisualBasicSyntaxNode node)
		{
			while (node != null)
			{
				if (node.Kind() == SyntaxKind.Attribute)
				{
					return true;
				}
				node = node.Parent;
			}
			return false;
		}

		private static bool IsExpressionInValidContext(ExpressionSyntax expression)
		{
			VisualBasicSyntaxNode visualBasicSyntaxNode = expression;
			while (true)
			{
				VisualBasicSyntaxNode parent = visualBasicSyntaxNode.Parent;
				if (parent == null)
				{
					return true;
				}
				if (!(parent is ExpressionSyntax))
				{
					switch (parent.Kind())
					{
					case SyntaxKind.NextStatement:
						return false;
					case SyntaxKind.EqualsValue:
						parent = parent.Parent;
						if (parent == null)
						{
							return true;
						}
						switch (parent.Kind())
						{
						case SyntaxKind.EnumMemberDeclaration:
						case SyntaxKind.Parameter:
							return false;
						case SyntaxKind.VariableDeclarator:
							if (parent.Parent is LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
							{
								SyntaxTokenList.Enumerator enumerator = localDeclarationStatementSyntax.Modifiers.GetEnumerator();
								while (enumerator.MoveNext())
								{
									SyntaxKind syntaxKind = VisualBasicExtensions.Kind(enumerator.Current);
									if (syntaxKind == SyntaxKind.ConstKeyword)
									{
										return false;
									}
								}
							}
							return true;
						default:
							return true;
						}
					case SyntaxKind.RaiseEventStatement:
						return false;
					case SyntaxKind.NamedFieldInitializer:
						if (((NamedFieldInitializerSyntax)parent).Name == visualBasicSyntaxNode)
						{
							return false;
						}
						break;
					case SyntaxKind.NameColonEquals:
						return false;
					case SyntaxKind.RangeArgument:
						if (((RangeArgumentSyntax)parent).LowerBound == visualBasicSyntaxNode)
						{
							return false;
						}
						break;
					case SyntaxKind.GoToStatement:
						return false;
					case SyntaxKind.XmlDeclarationOption:
						return false;
					default:
						return true;
					case SyntaxKind.ObjectMemberInitializer:
					case SyntaxKind.ArgumentList:
					case SyntaxKind.SimpleArgument:
						break;
					}
				}
				else
				{
					SyntaxKind syntaxKind2 = parent.Kind();
					if (syntaxKind2 == SyntaxKind.XmlElementEndTag)
					{
						break;
					}
				}
				visualBasicSyntaxNode = parent;
			}
			return false;
		}

		private void AssertNodeInTree(VisualBasicSyntaxNode node, string argName)
		{
			if (node == null)
			{
				throw new ArgumentNullException(argName);
			}
			if (!IsInTree(node))
			{
				throw new ArgumentException(argName + VBResources.NotWithinTree);
			}
		}

		private bool ValidateRegionDefiningExpression(ExpressionSyntax expression)
		{
			AssertNodeInTree(expression, "expression");
			if (expression.Kind() == SyntaxKind.PredefinedType || SyntaxFacts.IsInNamespaceOrTypeContext(expression))
			{
				return false;
			}
			if (SyntaxFactory.GetStandaloneExpression(expression) != expression)
			{
				return false;
			}
			switch (expression.Kind())
			{
			case SyntaxKind.CollectionInitializer:
			{
				VisualBasicSyntaxNode parent = expression.Parent;
				if (parent == null)
				{
					break;
				}
				switch (parent.Kind())
				{
				case SyntaxKind.ObjectCollectionInitializer:
					if (((ObjectCollectionInitializerSyntax)parent).Initializer == expression)
					{
						return false;
					}
					break;
				case SyntaxKind.ArrayCreationExpression:
					if (((ArrayCreationExpressionSyntax)parent).Initializer == expression)
					{
						return false;
					}
					break;
				case SyntaxKind.CollectionInitializer:
					parent = parent.Parent;
					if (parent != null && parent.Kind() == SyntaxKind.CollectionInitializer)
					{
						VisualBasicSyntaxNode visualBasicSyntaxNode = parent;
						parent = parent.Parent;
						if (parent != null && parent.Kind() == SyntaxKind.ObjectCollectionInitializer && ((ObjectCollectionInitializerSyntax)parent).Initializer == visualBasicSyntaxNode)
						{
							break;
						}
					}
					return false;
				}
				break;
			}
			case SyntaxKind.IdentifierLabel:
			case SyntaxKind.NumericLabel:
			case SyntaxKind.NextLabel:
				return false;
			}
			if (!IsExpressionInValidContext(expression) || IsNodeInsideAttributeArguments(expression))
			{
				return false;
			}
			return true;
		}

		private bool ValidateRegionDefiningStatementsRange(StatementSyntax firstStatement, StatementSyntax lastStatement)
		{
			AssertNodeInTree(firstStatement, "firstStatement");
			AssertNodeInTree(lastStatement, "lastStatement");
			if (firstStatement.Parent == null || firstStatement.Parent != lastStatement.Parent)
			{
				throw new ArgumentException("statements not within the same statement list");
			}
			if (firstStatement.SpanStart > lastStatement.SpanStart)
			{
				throw new ArgumentException("first statement does not precede last statement");
			}
			if (!(firstStatement is ExecutableStatementSyntax) || !(lastStatement is ExecutableStatementSyntax))
			{
				return false;
			}
			if (IsNotUppermostForBlock(firstStatement))
			{
				return false;
			}
			if (firstStatement != lastStatement && IsNotUppermostForBlock(lastStatement))
			{
				return false;
			}
			if (IsNodeInsideAttributeArguments(firstStatement) || (firstStatement != lastStatement && IsNodeInsideAttributeArguments(lastStatement)))
			{
				return false;
			}
			return true;
		}

		private bool IsNotUppermostForBlock(VisualBasicSyntaxNode forBlockOrStatement)
		{
			ForOrForEachBlockSyntax forOrForEachBlockSyntax = forBlockOrStatement as ForOrForEachBlockSyntax;
			if (forOrForEachBlockSyntax == null)
			{
				return false;
			}
			NextStatementSyntax nextStatement = forOrForEachBlockSyntax.NextStatement;
			if (nextStatement != null)
			{
				return nextStatement.ControlVariables.Count > 1;
			}
			int num = 1;
			while (true)
			{
				if (forOrForEachBlockSyntax.Statements.Count == 0)
				{
					return true;
				}
				if (!(forOrForEachBlockSyntax.Statements.Last() is ForOrForEachBlockSyntax forOrForEachBlockSyntax2))
				{
					return true;
				}
				num++;
				nextStatement = forOrForEachBlockSyntax2.NextStatement;
				if (nextStatement != null)
				{
					break;
				}
				forOrForEachBlockSyntax = forOrForEachBlockSyntax2;
			}
			return nextStatement.ControlVariables.Count != num;
		}

		internal override ForEachStatementInfo GetForEachStatementInfoWorker(ForEachBlockSyntax node)
		{
			return GetMemberSemanticModel(node)?.GetForEachStatementInfoWorker(node) ?? default(ForEachStatementInfo);
		}

		internal override AwaitExpressionInfo GetAwaitExpressionInfoWorker(AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetMemberSemanticModel(awaitExpression)?.GetAwaitExpressionInfoWorker(awaitExpression, cancellationToken) ?? default(AwaitExpressionInfo);
		}

		private RegionAnalysisContext CreateFailedRegionAnalysisContext()
		{
			return new RegionAnalysisContext(Compilation);
		}

		private RegionAnalysisContext CreateRegionAnalysisContext(ExpressionSyntax expression)
		{
			TextSpan span = expression.Span;
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(expression);
			RegionAnalysisContext result;
			if (memberSemanticModel == null)
			{
				BoundBadStatement boundBadStatement = new BoundBadStatement(expression, ImmutableArray<BoundNode>.Empty);
				result = new RegionAnalysisContext(Compilation, null, boundBadStatement, boundBadStatement, boundBadStatement, span);
			}
			else
			{
				BoundNode boundRoot = memberSemanticModel.GetBoundRoot();
				BoundNode upperBoundNode = memberSemanticModel.GetUpperBoundNode(expression);
				result = new RegionAnalysisContext(Compilation, memberSemanticModel.MemberSymbol, boundRoot, upperBoundNode, upperBoundNode, span);
			}
			return result;
		}

		private RegionAnalysisContext CreateRegionAnalysisContext(StatementSyntax firstStatement, StatementSyntax lastStatement)
		{
			TextSpan region = TextSpan.FromBounds(firstStatement.SpanStart, lastStatement.Span.End);
			MemberSemanticModel memberSemanticModel = GetMemberSemanticModel(firstStatement);
			RegionAnalysisContext result;
			if (memberSemanticModel == null)
			{
				BoundBadStatement boundBadStatement = new BoundBadStatement(firstStatement, ImmutableArray<BoundNode>.Empty);
				result = new RegionAnalysisContext(Compilation, null, boundBadStatement, boundBadStatement, boundBadStatement, region);
			}
			else
			{
				BoundNode boundRoot = memberSemanticModel.GetBoundRoot();
				BoundNode upperBoundNode = memberSemanticModel.GetUpperBoundNode(firstStatement);
				BoundNode upperBoundNode2 = memberSemanticModel.GetUpperBoundNode(lastStatement);
				result = new RegionAnalysisContext(Compilation, memberSemanticModel.MemberSymbol, boundRoot, upperBoundNode, upperBoundNode2, region);
			}
			return result;
		}
	}
}
