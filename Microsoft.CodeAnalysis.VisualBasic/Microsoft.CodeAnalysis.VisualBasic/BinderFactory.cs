using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class BinderFactory
	{
		private sealed class BinderFactoryVisitor : VisualBasicSyntaxVisitor<Binder>
		{
			private int _position;

			private readonly BinderFactory _factory;

			internal int Position
			{
				set
				{
					_position = value;
				}
			}

			public BinderFactoryVisitor(BinderFactory factory)
			{
				_factory = factory;
			}

			public override Binder VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
			{
				StructuredTriviaSyntax structuredTriviaSyntax = SyntaxNodeExtensions.EnclosingStructuredTrivia(node);
				if (structuredTriviaSyntax != null && structuredTriviaSyntax.Kind() == SyntaxKind.DocumentationCommentTrivia)
				{
					return _factory.CreateDocumentationCommentBinder((DocumentationCommentTriviaSyntax)structuredTriviaSyntax, DocumentationCommentBinder.BinderType.Cref);
				}
				return base.VisitXmlCrefAttribute(node);
			}

			public override Binder VisitXmlNameAttribute(XmlNameAttributeSyntax node)
			{
				StructuredTriviaSyntax structuredTriviaSyntax = SyntaxNodeExtensions.EnclosingStructuredTrivia(node);
				if (structuredTriviaSyntax != null && structuredTriviaSyntax.Kind() == SyntaxKind.DocumentationCommentTrivia)
				{
					DocumentationCommentBinder.BinderType binderTypeForNameAttribute = DocumentationCommentBinder.GetBinderTypeForNameAttribute(node);
					if (binderTypeForNameAttribute != 0)
					{
						return _factory.CreateDocumentationCommentBinder((DocumentationCommentTriviaSyntax)structuredTriviaSyntax, binderTypeForNameAttribute);
					}
				}
				return base.VisitXmlNameAttribute(node);
			}

			public override Binder VisitXmlAttribute(XmlAttributeSyntax node)
			{
				if (node.Name.Kind() == SyntaxKind.XmlName)
				{
					string valueText = ((XmlNameSyntax)node.Name).LocalName.ValueText;
					if (DocumentationCommentXmlNames.AttributeEquals(valueText, "cref"))
					{
						StructuredTriviaSyntax structuredTriviaSyntax = SyntaxNodeExtensions.EnclosingStructuredTrivia(node);
						if (structuredTriviaSyntax != null && structuredTriviaSyntax.Kind() == SyntaxKind.DocumentationCommentTrivia)
						{
							return _factory.CreateDocumentationCommentBinder((DocumentationCommentTriviaSyntax)structuredTriviaSyntax, DocumentationCommentBinder.BinderType.Cref);
						}
					}
					else if (DocumentationCommentXmlNames.AttributeEquals(valueText, "name"))
					{
						StructuredTriviaSyntax structuredTriviaSyntax2 = SyntaxNodeExtensions.EnclosingStructuredTrivia(node);
						if (structuredTriviaSyntax2 != null && structuredTriviaSyntax2.Kind() == SyntaxKind.DocumentationCommentTrivia)
						{
							DocumentationCommentBinder.BinderType binderTypeForNameAttribute = DocumentationCommentBinder.GetBinderTypeForNameAttribute(node);
							if (binderTypeForNameAttribute != 0)
							{
								return _factory.CreateDocumentationCommentBinder((DocumentationCommentTriviaSyntax)structuredTriviaSyntax2, binderTypeForNameAttribute);
							}
						}
					}
				}
				return base.VisitXmlAttribute(node);
			}

			private Binder VisitMethodBaseDeclaration(MethodBaseSyntax methodBaseSyntax)
			{
				VisualBasicSyntaxNode parentNode = ((methodBaseSyntax.Parent is MethodBlockBaseSyntax methodBlockBaseSyntax) ? methodBlockBaseSyntax.Parent : methodBaseSyntax.Parent);
				return GetBinderForNodeAndUsage(methodBaseSyntax, NodeUsage.MethodFull, parentNode, _position);
			}

			public override Binder DefaultVisit(SyntaxNode node)
			{
				if (_factory.InScript && VisualBasicExtensions.Kind(node.Parent) == SyntaxKind.CompilationUnit)
				{
					return GetBinderForNodeAndUsage((VisualBasicSyntaxNode)node.Parent, NodeUsage.TopLevelExecutableStatement, (VisualBasicSyntaxNode)node.Parent, _position);
				}
				return null;
			}

			public override Binder VisitCompilationUnit(CompilationUnitSyntax node)
			{
				return GetBinderForNodeAndUsage(node, _factory.InScript ? NodeUsage.ScriptCompilationUnit : NodeUsage.CompilationUnit);
			}

			public override Binder VisitNamespaceBlock(NamespaceBlockSyntax nsBlockSyntax)
			{
				if (SyntaxFacts.InBlockInterior(nsBlockSyntax, _position))
				{
					return GetBinderForNodeAndUsage(nsBlockSyntax, NodeUsage.NamespaceBlockInterior, nsBlockSyntax.Parent, _position);
				}
				return null;
			}

			public override Binder VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
			{
				if (IsNotNothingAndContains(node.Initializer, _position))
				{
					return GetBinderForNodeAndUsage(node, NodeUsage.FieldOrPropertyInitializer, node.Parent, _position);
				}
				return null;
			}

			public override Binder VisitVariableDeclarator(VariableDeclaratorSyntax node)
			{
				if (IsNotNothingAndContains(node.Initializer, _position) || IsNotNothingAndContains(node.AsClause as AsNewClauseSyntax, _position))
				{
					return GetBinderForNodeAndUsage(node, NodeUsage.FieldOrPropertyInitializer, node.Parent, _position);
				}
				SeparatedSyntaxList<ModifiedIdentifierSyntax>.Enumerator enumerator = node.Names.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ModifiedIdentifierSyntax current = enumerator.Current;
					if (IsNotNothingAndContains(current.ArrayBounds, _position))
					{
						return GetBinderForNodeAndUsage(current, NodeUsage.FieldArrayBounds, node.Parent, _position);
					}
				}
				return null;
			}

			public override Binder VisitPropertyStatement(PropertyStatementSyntax node)
			{
				if (IsNotNothingAndContains(node.Initializer, _position) || IsNotNothingAndContains(node.AsClause as AsNewClauseSyntax, _position))
				{
					return GetBinderForNodeAndUsage(node, NodeUsage.FieldOrPropertyInitializer, node.Parent, _position);
				}
				return null;
			}

			public override Binder VisitModuleBlock(ModuleBlockSyntax moduleSyntax)
			{
				return GetBinderForNodeAndUsage(moduleSyntax.BlockStatement, NodeUsage.TypeBlockFull, moduleSyntax.Parent, _position);
			}

			public override Binder VisitClassBlock(ClassBlockSyntax classSyntax)
			{
				return GetBinderForNodeAndUsage(classSyntax.BlockStatement, NodeUsage.TypeBlockFull, classSyntax.Parent, _position);
			}

			public override Binder VisitStructureBlock(StructureBlockSyntax structureSyntax)
			{
				return GetBinderForNodeAndUsage(structureSyntax.BlockStatement, NodeUsage.TypeBlockFull, structureSyntax.Parent, _position);
			}

			public override Binder VisitAttribute(AttributeSyntax node)
			{
				return GetBinderForNodeAndUsage(node, NodeUsage.Attribute, node.Parent, _position);
			}

			public override Binder VisitInterfaceBlock(InterfaceBlockSyntax interfaceSyntax)
			{
				return GetBinderForNodeAndUsage(interfaceSyntax.BlockStatement, NodeUsage.TypeBlockFull, interfaceSyntax.Parent, _position);
			}

			public override Binder VisitEnumBlock(EnumBlockSyntax enumBlockSyntax)
			{
				return GetBinderForNodeAndUsage(enumBlockSyntax.EnumStatement, NodeUsage.EnumBlockFull, enumBlockSyntax.Parent, _position);
			}

			public override Binder VisitDelegateStatement(DelegateStatementSyntax delegateSyntax)
			{
				return GetBinderForNodeAndUsage(delegateSyntax, NodeUsage.DelegateDeclaration, delegateSyntax.Parent, _position);
			}

			public override Binder VisitInheritsStatement(InheritsStatementSyntax inheritsSyntax)
			{
				return GetBinderForNodeAndUsage(inheritsSyntax, NodeUsage.InheritsStatement, inheritsSyntax.Parent, _position);
			}

			public override Binder VisitImplementsStatement(ImplementsStatementSyntax implementsSyntax)
			{
				return GetBinderForNodeAndUsage(implementsSyntax, NodeUsage.ImplementsStatement, implementsSyntax.Parent, _position);
			}

			public override Binder VisitMethodStatement(MethodStatementSyntax node)
			{
				if (node.ContainsDiagnostics && node.Parent.Kind() == SyntaxKind.SingleLineSubLambdaExpression)
				{
					return DefaultVisit(node);
				}
				return VisitMethodBaseDeclaration(node);
			}

			public override Binder VisitSubNewStatement(SubNewStatementSyntax node)
			{
				return VisitMethodBaseDeclaration(node);
			}

			public override Binder VisitOperatorStatement(OperatorStatementSyntax node)
			{
				return VisitMethodBaseDeclaration(node);
			}

			public override Binder VisitDeclareStatement(DeclareStatementSyntax node)
			{
				return VisitMethodBaseDeclaration(node);
			}

			public override Binder VisitAccessorStatement(AccessorStatementSyntax node)
			{
				return VisitMethodBaseDeclaration(node);
			}

			public override Binder VisitParameter(ParameterSyntax node)
			{
				if (IsNotNothingAndContains(node.Default, _position))
				{
					return GetBinderForNodeAndUsage(node, NodeUsage.ParameterDefaultValue, node.Parent, _position);
				}
				return null;
			}

			private Binder VisitMethodBlockBase(MethodBlockBaseSyntax methodBlockSyntax, MethodBaseSyntax begin)
			{
				NodeUsage usage = ((!SyntaxFacts.InBlockInterior(methodBlockSyntax, _position)) ? NodeUsage.MethodFull : NodeUsage.MethodInterior);
				return GetBinderForNodeAndUsage(begin, usage, methodBlockSyntax.Parent, _position);
			}

			public override Binder VisitMethodBlock(MethodBlockSyntax node)
			{
				return VisitMethodBlockBase(node, node.BlockStatement);
			}

			public override Binder VisitConstructorBlock(ConstructorBlockSyntax node)
			{
				return VisitMethodBlockBase(node, node.BlockStatement);
			}

			public override Binder VisitOperatorBlock(OperatorBlockSyntax node)
			{
				return VisitMethodBlockBase(node, node.BlockStatement);
			}

			public override Binder VisitAccessorBlock(AccessorBlockSyntax node)
			{
				return VisitMethodBlockBase(node, node.BlockStatement);
			}

			public override Binder VisitPropertyBlock(PropertyBlockSyntax node)
			{
				return GetBinderForNodeAndUsage(node.PropertyStatement, NodeUsage.PropertyFull, node.Parent, _position);
			}

			public override Binder VisitImportsStatement(ImportsStatementSyntax node)
			{
				return BinderBuilder.CreateBinderForSourceFileImports(_factory._sourceModule, _factory._tree);
			}

			private Binder GetBinderForNodeAndUsage(VisualBasicSyntaxNode node, NodeUsage usage, VisualBasicSyntaxNode parentNode = null, int position = -1)
			{
				return _factory.GetBinderForNodeAndUsage(node, usage, parentNode, position);
			}

			private static bool IsNotNothingAndContains(VisualBasicSyntaxNode nodeOpt, int position)
			{
				if (nodeOpt != null)
				{
					return SyntaxFacts.InSpanOrEffectiveTrailingOfNode(nodeOpt, position);
				}
				return false;
			}
		}

		private enum NodeUsage : byte
		{
			CompilationUnit,
			ImplicitClass,
			ScriptCompilationUnit,
			TopLevelExecutableStatement,
			ImportsStatement,
			NamespaceBlockInterior,
			TypeBlockFull,
			EnumBlockFull,
			DelegateDeclaration,
			InheritsStatement,
			ImplementsStatement,
			MethodFull,
			MethodInterior,
			FieldOrPropertyInitializer,
			FieldArrayBounds,
			Attribute,
			ParameterDefaultValue,
			PropertyFull
		}

		private readonly SourceModuleSymbol _sourceModule;

		private readonly SyntaxTree _tree;

		private readonly ConcurrentDictionary<(VisualBasicSyntaxNode, byte), Binder> _cache;

		private readonly ObjectPool<BinderFactoryVisitor> _binderFactoryVisitorPool;

		private bool InScript => _tree.Options.Kind == SourceCodeKind.Script;

		public BinderFactory(SourceModuleSymbol sourceModule, SyntaxTree tree)
		{
			_sourceModule = sourceModule;
			_tree = tree;
			_cache = new ConcurrentDictionary<(VisualBasicSyntaxNode, byte), Binder>();
			_binderFactoryVisitorPool = new ObjectPool<BinderFactoryVisitor>(() => new BinderFactoryVisitor(this));
		}

		private Binder MakeBinder(SyntaxNode node, int position)
		{
			if (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(node, position) || VisualBasicExtensions.Kind(node) == SyntaxKind.CompilationUnit)
			{
				BinderFactoryVisitor binderFactoryVisitor = _binderFactoryVisitorPool.Allocate();
				binderFactoryVisitor.Position = position;
				Binder result = binderFactoryVisitor.Visit(node);
				_binderFactoryVisitorPool.Free(binderFactoryVisitor);
				return result;
			}
			return null;
		}

		public Binder GetNamespaceBinder(NamespaceBlockSyntax node)
		{
			return GetBinderForNodeAndUsage(node, NodeUsage.NamespaceBlockInterior, node.Parent, node.SpanStart);
		}

		public Binder GetNamedTypeBinder(TypeStatementSyntax node)
		{
			VisualBasicSyntaxNode parentNode = ((node.Parent is TypeBlockSyntax typeBlockSyntax) ? typeBlockSyntax.Parent : node.Parent);
			return GetBinderForNodeAndUsage(node, NodeUsage.TypeBlockFull, parentNode, node.SpanStart);
		}

		public Binder GetNamedTypeBinder(EnumStatementSyntax node)
		{
			VisualBasicSyntaxNode parentNode = ((node.Parent is EnumBlockSyntax enumBlockSyntax) ? enumBlockSyntax.Parent : node.Parent);
			return GetBinderForNodeAndUsage(node, NodeUsage.EnumBlockFull, parentNode, node.SpanStart);
		}

		public Binder GetNamedTypeBinder(DelegateStatementSyntax node)
		{
			return GetBinderForNodeAndUsage(node, NodeUsage.DelegateDeclaration, node.Parent, node.SpanStart);
		}

		public Binder GetBinderForPosition(SyntaxNode node, int position)
		{
			return GetBinderAtOrAbove(node, position);
		}

		private Binder GetBinderAtOrAbove(SyntaxNode node, int position)
		{
			Binder binder;
			while (true)
			{
				binder = MakeBinder(node, position);
				if (binder != null)
				{
					break;
				}
				node = ((VisualBasicExtensions.Kind(node) != SyntaxKind.DocumentationCommentTrivia) ? node.Parent : ((VisualBasicSyntaxNode)((StructuredTriviaSyntax)node).ParentTrivia.Token.Parent));
			}
			return binder;
		}

		private Binder GetBinderForNodeAndUsage(VisualBasicSyntaxNode node, NodeUsage usage, VisualBasicSyntaxNode parentNode = null, int position = -1, Binder containingBinder = null)
		{
			Binder value = null;
			(VisualBasicSyntaxNode, byte) key = (node, (byte)usage);
			if (!_cache.TryGetValue(key, out value))
			{
				if (containingBinder == null && parentNode != null)
				{
					containingBinder = GetBinderAtOrAbove(parentNode, position);
				}
				value = CreateBinderForNodeAndUsage(node, usage, containingBinder);
				_cache.TryAdd(key, value);
			}
			return value;
		}

		private Binder CreateBinderForNodeAndUsage(VisualBasicSyntaxNode node, NodeUsage usage, Binder containingBinder)
		{
			switch (usage)
			{
			case NodeUsage.CompilationUnit:
				return BinderBuilder.CreateBinderForNamespace(_sourceModule, _tree, _sourceModule.RootNamespace);
			case NodeUsage.ImplicitClass:
			{
				NamedTypeSymbol typeSymbol = ((node.Kind() == SyntaxKind.CompilationUnit && _tree.Options.Kind != 0) ? _sourceModule.ContainingSourceAssembly.DeclaringCompilation.SourceScriptClass : ((NamedTypeSymbol)containingBinder.ContainingNamespaceOrType.GetMembers("<invalid-global-code>").Single()));
				return new NamedTypeBinder(containingBinder, typeSymbol);
			}
			case NodeUsage.ScriptCompilationUnit:
				return new NamedTypeBinder(GetBinderForNodeAndUsage(node, NodeUsage.CompilationUnit), _sourceModule.ContainingSourceAssembly.DeclaringCompilation.SourceScriptClass);
			case NodeUsage.TopLevelExecutableStatement:
				return new TopLevelCodeBinder(containingBinder.ContainingType.InstanceConstructors.Single(), containingBinder);
			case NodeUsage.ImportsStatement:
				return BinderBuilder.CreateBinderForSourceFileImports(_sourceModule, _tree);
			case NodeUsage.NamespaceBlockInterior:
			{
				NamespaceBlockSyntax namespaceBlockSyntax = (NamespaceBlockSyntax)node;
				NamespaceBinder namespaceBinder = containingBinder as NamespaceBinder;
				if (namespaceBinder == null && containingBinder is NamedTypeBinder namedTypeBinder3 && namedTypeBinder3.ContainingType.IsScriptClass)
				{
					namespaceBinder = (NamespaceBinder)GetBinderForNodeAndUsage(node, NodeUsage.CompilationUnit);
				}
				if (namespaceBinder != null)
				{
					return BuildNamespaceBinder(namespaceBinder, namespaceBlockSyntax.NamespaceStatement.Name, namespaceBlockSyntax.Parent.Kind() == SyntaxKind.CompilationUnit);
				}
				return containingBinder;
			}
			case NodeUsage.TypeBlockFull:
			{
				SourceNamedTypeSymbol sourceNamedTypeSymbol = SourceMemberContainerTypeSymbol.FindSymbolFromSyntax((TypeStatementSyntax)node, containingBinder.ContainingNamespaceOrType, _sourceModule);
				return ((object)sourceNamedTypeSymbol != null) ? new NamedTypeBinder(containingBinder, sourceNamedTypeSymbol) : containingBinder;
			}
			case NodeUsage.EnumBlockFull:
			{
				SourceNamedTypeSymbol sourceNamedTypeSymbol3 = SourceMemberContainerTypeSymbol.FindSymbolFromSyntax((EnumStatementSyntax)node, containingBinder.ContainingNamespaceOrType, _sourceModule);
				return ((object)sourceNamedTypeSymbol3 != null) ? new NamedTypeBinder(containingBinder, sourceNamedTypeSymbol3) : containingBinder;
			}
			case NodeUsage.DelegateDeclaration:
			{
				SourceNamedTypeSymbol sourceNamedTypeSymbol2 = SourceMemberContainerTypeSymbol.FindSymbolFromSyntax((DelegateStatementSyntax)node, containingBinder.ContainingNamespaceOrType, _sourceModule);
				return ((object)sourceNamedTypeSymbol2 != null) ? new NamedTypeBinder(containingBinder, sourceNamedTypeSymbol2) : containingBinder;
			}
			case NodeUsage.InheritsStatement:
				if (containingBinder is NamedTypeBinder namedTypeBinder4)
				{
					return new BasesBeingResolvedBinder(containingBinder, BasesBeingResolved.Empty.PrependInheritsBeingResolved(namedTypeBinder4.ContainingType));
				}
				return containingBinder;
			case NodeUsage.ImplementsStatement:
				if (containingBinder is NamedTypeBinder namedTypeBinder)
				{
					return new BasesBeingResolvedBinder(containingBinder, BasesBeingResolved.Empty.PrependImplementsBeingResolved(namedTypeBinder.ContainingType));
				}
				return containingBinder;
			case NodeUsage.PropertyFull:
				return GetContainingNamedTypeBinderForMemberNode(((PropertyStatementSyntax)node).Parent.Parent, containingBinder);
			case NodeUsage.MethodFull:
			case NodeUsage.MethodInterior:
			{
				MethodBaseSyntax methodBaseSyntax2 = (MethodBaseSyntax)node;
				NamedTypeBinder containingNamedTypeBinderForMemberNode = GetContainingNamedTypeBinderForMemberNode(node.Parent.Parent, containingBinder);
				if (containingNamedTypeBinderForMemberNode == null)
				{
					return containingBinder;
				}
				switch (methodBaseSyntax2.Kind())
				{
				default:
					return containingBinder;
				case SyntaxKind.SubStatement:
				case SyntaxKind.FunctionStatement:
				case SyntaxKind.SubNewStatement:
				case SyntaxKind.OperatorStatement:
				case SyntaxKind.GetAccessorStatement:
				case SyntaxKind.SetAccessorStatement:
				case SyntaxKind.AddHandlerAccessorStatement:
				case SyntaxKind.RemoveHandlerAccessorStatement:
				case SyntaxKind.RaiseEventAccessorStatement:
					return BuildMethodBinder(containingNamedTypeBinderForMemberNode, methodBaseSyntax2, usage == NodeUsage.MethodInterior);
				}
			}
			case NodeUsage.FieldOrPropertyInitializer:
			{
				Symbol symbol2 = null;
				ImmutableArray<Symbol> additionalFieldsOrProperties = ImmutableArray<Symbol>.Empty;
				NamedTypeBinder namedTypeBinder5;
				switch (node.Kind())
				{
				case SyntaxKind.VariableDeclarator:
				{
					VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)node;
					namedTypeBinder5 = GetContainingNamedTypeBinderForMemberNode(node.Parent.Parent, containingBinder);
					if (namedTypeBinder5 == null)
					{
						return null;
					}
					SyntaxToken identifier4 = variableDeclaratorSyntax.Names[0].Identifier;
					symbol2 = NamedTypeSymbolExtensions.FindFieldOrProperty(namedTypeBinder5.ContainingType, identifier4.ValueText, identifier4.Span, _tree);
					if (variableDeclaratorSyntax.Names.Count <= 1)
					{
						break;
					}
					ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
					foreach (ModifiedIdentifierSyntax item2 in variableDeclaratorSyntax.Names.Skip(1))
					{
						identifier4 = item2.Identifier;
						Symbol item = NamedTypeSymbolExtensions.FindFieldOrProperty(namedTypeBinder5.ContainingType, identifier4.ValueText, identifier4.Span, _tree);
						instance.Add(item);
					}
					additionalFieldsOrProperties = instance.ToImmutableAndFree();
					break;
				}
				case SyntaxKind.EnumMemberDeclaration:
				{
					EnumMemberDeclarationSyntax obj = (EnumMemberDeclarationSyntax)node;
					namedTypeBinder5 = (NamedTypeBinder)containingBinder;
					SyntaxToken identifier3 = obj.Identifier;
					symbol2 = NamedTypeSymbolExtensions.FindMember(namedTypeBinder5.ContainingType, identifier3.ValueText, SymbolKind.Field, identifier3.Span, _tree);
					break;
				}
				case SyntaxKind.PropertyStatement:
				{
					PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)node;
					namedTypeBinder5 = GetContainingNamedTypeBinderForMemberNode(node.Parent, containingBinder);
					if (namedTypeBinder5 == null)
					{
						return null;
					}
					SyntaxToken identifier2 = propertyStatementSyntax.Identifier;
					symbol2 = NamedTypeSymbolExtensions.FindMember(namedTypeBinder5.ContainingType, identifier2.ValueText, SymbolKind.Property, identifier2.Span, _tree);
					break;
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(node.Kind());
				}
				if ((object)symbol2 != null)
				{
					return BuildInitializerBinder(namedTypeBinder5, symbol2, additionalFieldsOrProperties);
				}
				return null;
			}
			case NodeUsage.FieldArrayBounds:
			{
				ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)node;
				if (containingBinder is NamedTypeBinder namedTypeBinder2)
				{
					NamedTypeSymbol containingType = namedTypeBinder2.ContainingType;
					SyntaxToken identifier = modifiedIdentifierSyntax.Identifier;
					Symbol symbol = NamedTypeSymbolExtensions.FindMember(containingType, identifier.ValueText, SymbolKind.Field, identifier.Span, _tree);
					if ((object)symbol != null)
					{
						return BuildInitializerBinder(namedTypeBinder2, symbol, ImmutableArray<Symbol>.Empty);
					}
				}
				return null;
			}
			case NodeUsage.Attribute:
				return BuildAttributeBinder(containingBinder, node);
			case NodeUsage.ParameterDefaultValue:
			{
				ParameterSyntax parameterSyntax = (ParameterSyntax)node;
				if (parameterSyntax.Default != null)
				{
					MethodBaseSyntax methodBaseSyntax = (MethodBaseSyntax)((ParameterListSyntax)parameterSyntax.Parent).Parent;
					ParameterSymbol parameterSymbol = null;
					switch (methodBaseSyntax.Kind())
					{
					case SyntaxKind.SubStatement:
					case SyntaxKind.FunctionStatement:
					case SyntaxKind.SubNewStatement:
					case SyntaxKind.DeclareSubStatement:
					case SyntaxKind.DeclareFunctionStatement:
					case SyntaxKind.OperatorStatement:
					{
						NamedTypeSymbol parameterDeclarationContainingType3 = GetParameterDeclarationContainingType(containingBinder);
						if ((object)parameterDeclarationContainingType3 != null)
						{
							SourceMethodSymbol sourceMethodSymbol = (SourceMethodSymbol)SourceMethodSymbol.FindSymbolFromSyntax(methodBaseSyntax, _tree, parameterDeclarationContainingType3);
							if ((object)sourceMethodSymbol != null)
							{
								parameterSymbol = MethodSymbolExtensions.GetParameterSymbol(sourceMethodSymbol.Parameters, parameterSyntax);
							}
						}
						break;
					}
					case SyntaxKind.DelegateSubStatement:
					case SyntaxKind.DelegateFunctionStatement:
					{
						NamedTypeSymbol parameterDeclarationContainingType2 = GetParameterDeclarationContainingType(containingBinder);
						if ((object)parameterDeclarationContainingType2 != null && parameterDeclarationContainingType2.TypeKind == TypeKind.Delegate)
						{
							parameterSymbol = MethodSymbolExtensions.GetParameterSymbol(parameterDeclarationContainingType2.DelegateInvokeMethod.Parameters, parameterSyntax);
						}
						break;
					}
					case SyntaxKind.EventStatement:
					{
						NamedTypeSymbol parameterDeclarationContainingType4 = GetParameterDeclarationContainingType(containingBinder);
						if ((object)parameterDeclarationContainingType4 != null)
						{
							SourceEventSymbol sourceEventSymbol = (SourceEventSymbol)SourceMethodSymbol.FindSymbolFromSyntax(methodBaseSyntax, _tree, parameterDeclarationContainingType4);
							if ((object)sourceEventSymbol != null)
							{
								parameterSymbol = MethodSymbolExtensions.GetParameterSymbol(sourceEventSymbol.DelegateParameters, parameterSyntax);
							}
						}
						break;
					}
					case SyntaxKind.PropertyStatement:
					{
						NamedTypeSymbol parameterDeclarationContainingType = GetParameterDeclarationContainingType(containingBinder);
						if ((object)parameterDeclarationContainingType != null)
						{
							SourcePropertySymbol sourcePropertySymbol = (SourcePropertySymbol)SourceMethodSymbol.FindSymbolFromSyntax(methodBaseSyntax, _tree, parameterDeclarationContainingType);
							if ((object)sourcePropertySymbol != null)
							{
								parameterSymbol = MethodSymbolExtensions.GetParameterSymbol(sourcePropertySymbol.Parameters, parameterSyntax);
							}
						}
						break;
					}
					case SyntaxKind.GetAccessorStatement:
					case SyntaxKind.SetAccessorStatement:
					case SyntaxKind.AddHandlerAccessorStatement:
					case SyntaxKind.RemoveHandlerAccessorStatement:
					case SyntaxKind.RaiseEventAccessorStatement:
					case SyntaxKind.SubLambdaHeader:
					case SyntaxKind.FunctionLambdaHeader:
						return null;
					default:
						throw ExceptionUtilities.UnexpectedValue(methodBaseSyntax.Kind());
					}
					if ((object)parameterSymbol != null)
					{
						return BinderBuilder.CreateBinderForParameterDefaultValue(parameterSymbol, containingBinder, parameterSyntax);
					}
				}
				return null;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(usage);
			}
		}

		private Binder CreateDocumentationCommentBinder(DocumentationCommentTriviaSyntax node, DocumentationCommentBinder.BinderType binderType)
		{
			VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)node.ParentTrivia.Token.Parent;
			VisualBasicSyntaxNode visualBasicSyntaxNode2 = null;
			while (true)
			{
				switch (visualBasicSyntaxNode.Kind())
				{
				case SyntaxKind.ModuleStatement:
				case SyntaxKind.StructureStatement:
				case SyntaxKind.InterfaceStatement:
				case SyntaxKind.ClassStatement:
				case SyntaxKind.EnumStatement:
					visualBasicSyntaxNode2 = visualBasicSyntaxNode;
					break;
				case SyntaxKind.SubStatement:
				case SyntaxKind.FunctionStatement:
				case SyntaxKind.SubNewStatement:
				case SyntaxKind.DeclareSubStatement:
				case SyntaxKind.DeclareFunctionStatement:
				case SyntaxKind.DelegateSubStatement:
				case SyntaxKind.DelegateFunctionStatement:
				case SyntaxKind.OperatorStatement:
					visualBasicSyntaxNode2 = visualBasicSyntaxNode.Parent;
					if (visualBasicSyntaxNode2 != null && visualBasicSyntaxNode2 is MethodBlockBaseSyntax)
					{
						visualBasicSyntaxNode2 = visualBasicSyntaxNode2.Parent;
					}
					break;
				case SyntaxKind.PropertyStatement:
					visualBasicSyntaxNode2 = visualBasicSyntaxNode.Parent;
					if (visualBasicSyntaxNode2 != null && visualBasicSyntaxNode2.Kind() == SyntaxKind.PropertyBlock)
					{
						visualBasicSyntaxNode2 = visualBasicSyntaxNode2.Parent;
					}
					break;
				case SyntaxKind.EventStatement:
					visualBasicSyntaxNode2 = visualBasicSyntaxNode.Parent;
					if (visualBasicSyntaxNode2 != null && visualBasicSyntaxNode2.Kind() == SyntaxKind.EventStatement)
					{
						visualBasicSyntaxNode2 = visualBasicSyntaxNode2.Parent;
					}
					break;
				case SyntaxKind.EnumMemberDeclaration:
				case SyntaxKind.FieldDeclaration:
					visualBasicSyntaxNode2 = visualBasicSyntaxNode.Parent;
					break;
				case SyntaxKind.AttributeList:
					visualBasicSyntaxNode2 = visualBasicSyntaxNode.Parent;
					if (visualBasicSyntaxNode2 != null)
					{
						goto IL_0113;
					}
					break;
				}
				break;
				IL_0113:
				visualBasicSyntaxNode = visualBasicSyntaxNode2;
				visualBasicSyntaxNode2 = null;
			}
			if (visualBasicSyntaxNode2 == null)
			{
				return GetBinderAtOrAbove(visualBasicSyntaxNode, visualBasicSyntaxNode.SpanStart);
			}
			Binder binderAtOrAbove = GetBinderAtOrAbove(visualBasicSyntaxNode2, visualBasicSyntaxNode.SpanStart);
			Symbol commentedSymbol = null;
			switch (visualBasicSyntaxNode.Kind())
			{
			case SyntaxKind.StructureStatement:
			case SyntaxKind.InterfaceStatement:
			case SyntaxKind.ClassStatement:
				commentedSymbol = binderAtOrAbove.ContainingNamespaceOrType;
				break;
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.EventStatement:
			case SyntaxKind.OperatorStatement:
			case SyntaxKind.PropertyStatement:
				if ((object)binderAtOrAbove.ContainingType != null)
				{
					commentedSymbol = SourceMethodSymbol.FindSymbolFromSyntax((MethodBaseSyntax)visualBasicSyntaxNode, _tree, binderAtOrAbove.ContainingType);
				}
				break;
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				commentedSymbol = (((object)binderAtOrAbove.ContainingType == null) ? SourceMemberContainerTypeSymbol.FindSymbolFromSyntax((DelegateStatementSyntax)visualBasicSyntaxNode, binderAtOrAbove.ContainingNamespaceOrType, _sourceModule) : SourceMethodSymbol.FindSymbolFromSyntax((MethodBaseSyntax)visualBasicSyntaxNode, _tree, binderAtOrAbove.ContainingType));
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(visualBasicSyntaxNode.Kind());
			case SyntaxKind.ModuleStatement:
			case SyntaxKind.EnumStatement:
			case SyntaxKind.EnumMemberDeclaration:
			case SyntaxKind.FieldDeclaration:
				break;
			}
			return BinderBuilder.CreateBinderForDocumentationComment(binderAtOrAbove, commentedSymbol, binderType);
		}

		private NamedTypeBinder GetContainingNamedTypeBinderForMemberNode(VisualBasicSyntaxNode node, Binder containingBinder)
		{
			if (containingBinder is NamedTypeBinder result)
			{
				return result;
			}
			if (node != null && (node.Kind() == SyntaxKind.NamespaceBlock || node.Kind() == SyntaxKind.CompilationUnit))
			{
				return (NamedTypeBinder)GetBinderForNodeAndUsage(node, NodeUsage.ImplicitClass, null, -1, containingBinder);
			}
			return null;
		}

		private static NamedTypeSymbol GetParameterDeclarationContainingType(Binder containingBinder)
		{
			NamedTypeBinder namedTypeBinder = containingBinder as NamedTypeBinder;
			if (namedTypeBinder == null)
			{
				if (!(containingBinder is MethodTypeParametersBinder methodTypeParametersBinder))
				{
					return null;
				}
				namedTypeBinder = (NamedTypeBinder)methodTypeParametersBinder.ContainingBinder;
			}
			return namedTypeBinder.ContainingType;
		}

		private NamespaceBinder BuildNamespaceBinder(NamespaceBinder containingBinder, NameSyntax childName, bool globalNamespaceAllowed)
		{
			string name;
			switch (childName.Kind())
			{
			case SyntaxKind.GlobalName:
				if (globalNamespaceAllowed)
				{
					return BinderBuilder.CreateBinderForNamespace(_sourceModule, _tree, _sourceModule.GlobalNamespace);
				}
				name = "Global";
				break;
			case SyntaxKind.QualifiedName:
			{
				QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)childName;
				containingBinder = BuildNamespaceBinder(containingBinder, qualifiedNameSyntax.Left, globalNamespaceAllowed);
				name = qualifiedNameSyntax.Right.Identifier.ValueText;
				break;
			}
			case SyntaxKind.IdentifierName:
				name = ((IdentifierNameSyntax)childName).Identifier.ValueText;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(childName.Kind());
			}
			ImmutableArray<Symbol>.Enumerator enumerator = containingBinder.NamespaceSymbol.GetMembers(name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if ((NamespaceOrTypeSymbol)enumerator.Current is NamespaceSymbol nsSymbol)
				{
					return new NamespaceBinder(containingBinder, nsSymbol);
				}
			}
			throw ExceptionUtilities.Unreachable;
		}

		private Binder BuildMethodBinder(NamedTypeBinder containingBinder, MethodBaseSyntax methodSyntax, bool forBody)
		{
			NamedTypeSymbol containingType = containingBinder.ContainingType;
			Symbol symbol = SourceMethodSymbol.FindSymbolFromSyntax(methodSyntax, _tree, containingType);
			if ((object)symbol != null && symbol.Kind == SymbolKind.Method)
			{
				SourceMethodSymbol sourceMethodSymbol = (SourceMethodSymbol)symbol;
				if (forBody)
				{
					return BinderBuilder.CreateBinderForMethodBody(sourceMethodSymbol, sourceMethodSymbol.Syntax, containingBinder);
				}
				return BinderBuilder.CreateBinderForMethodDeclaration(sourceMethodSymbol, containingBinder);
			}
			return containingBinder;
		}

		private Binder BuildInitializerBinder(Binder containingBinder, Symbol fieldOrProperty, ImmutableArray<Symbol> additionalFieldsOrProperties)
		{
			return BinderBuilder.CreateBinderForInitializer(containingBinder, fieldOrProperty, additionalFieldsOrProperties);
		}

		private Binder BuildAttributeBinder(Binder containingBinder, VisualBasicSyntaxNode node)
		{
			if (containingBinder != null && node.Parent != null)
			{
				VisualBasicSyntaxNode parent = node.Parent;
				if (parent.Parent != null)
				{
					SyntaxKind syntaxKind = parent.Parent.Kind();
					if (syntaxKind - 59 <= (SyntaxKind)4 && containingBinder is NamedTypeBinder)
					{
						containingBinder = containingBinder.ContainingBinder;
					}
				}
			}
			return BinderBuilder.CreateBinderForAttribute(_tree, containingBinder, node);
		}
	}
}
