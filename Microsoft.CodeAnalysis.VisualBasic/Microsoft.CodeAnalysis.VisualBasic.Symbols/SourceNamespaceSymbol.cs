using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceNamespaceSymbol : PEOrSourceOrMergedNamespaceSymbol
	{
		[Flags]
		private enum StateFlags
		{
			HasMultipleSpellings = 1,
			AllMembersIsSorted = 2,
			DeclarationValidated = 4
		}

		private struct NameToSymbolMapBuilder
		{
			private readonly Dictionary<string, object> _dictionary;

			public NameToSymbolMapBuilder(int capacity)
			{
				this = default(NameToSymbolMapBuilder);
				_dictionary = new Dictionary<string, object>(capacity, CaseInsensitiveComparison.Comparer);
			}

			public void Add(NamespaceOrTypeSymbol symbol)
			{
				string name = symbol.Name;
				object value = null;
				if (_dictionary.TryGetValue(name, out value))
				{
					ArrayBuilder<NamespaceOrTypeSymbol> arrayBuilder = value as ArrayBuilder<NamespaceOrTypeSymbol>;
					if (arrayBuilder == null)
					{
						arrayBuilder = ArrayBuilder<NamespaceOrTypeSymbol>.GetInstance();
						arrayBuilder.Add((NamespaceOrTypeSymbol)value);
						_dictionary[name] = arrayBuilder;
					}
					arrayBuilder.Add(symbol);
				}
				else
				{
					_dictionary[name] = symbol;
				}
			}

			public Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> CreateMap()
			{
				Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> dictionary = new Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>>(_dictionary.Count, CaseInsensitiveComparison.Comparer);
				foreach (KeyValuePair<string, object> item in _dictionary)
				{
					object objectValue = RuntimeHelpers.GetObjectValue(item.Value);
					ImmutableArray<NamespaceOrTypeSymbol> value;
					if (objectValue is ArrayBuilder<NamespaceOrTypeSymbol> arrayBuilder)
					{
						bool flag = false;
						int num = arrayBuilder.Count - 1;
						for (int i = 0; i <= num; i++)
						{
							if (arrayBuilder[i].Kind == SymbolKind.Namespace)
							{
								flag = true;
								break;
							}
						}
						value = ((!flag) ? StaticCast<NamespaceOrTypeSymbol>.From(arrayBuilder.ToDowncastedImmutable<NamedTypeSymbol>()) : arrayBuilder.ToImmutable());
						arrayBuilder.Free();
					}
					else
					{
						NamespaceOrTypeSymbol namespaceOrTypeSymbol = (NamespaceOrTypeSymbol)objectValue;
						value = ((namespaceOrTypeSymbol.Kind != SymbolKind.Namespace) ? StaticCast<NamespaceOrTypeSymbol>.From(ImmutableArray.Create((NamedTypeSymbol)namespaceOrTypeSymbol)) : ImmutableArray.Create(namespaceOrTypeSymbol));
					}
					dictionary.Add(item.Key, value);
				}
				return dictionary;
			}
		}

		private readonly MergedNamespaceDeclaration _declaration;

		private readonly SourceNamespaceSymbol _containingNamespace;

		private readonly SourceModuleSymbol _containingModule;

		private Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> _nameToMembersMap;

		private Dictionary<string, ImmutableArray<NamedTypeSymbol>> _nameToTypeMembersMap;

		private int _lazyEmbeddedKind;

		private int _lazyState;

		private ImmutableArray<NamedTypeSymbol> _lazyModuleMembers;

		private ImmutableArray<Symbol> _lazyAllMembers;

		private LexicalSortKey _lazyLexicalSortKey;

		public override string Name => _declaration.Name;

		internal override EmbeddedSymbolKind EmbeddedSymbolKind
		{
			get
			{
				if (_lazyEmbeddedKind == 1)
				{
					int num = 0;
					ImmutableArray<Location>.Enumerator enumerator = _declaration.NameLocations.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Location current = enumerator.Current;
						if (current.Kind == LocationKind.None && current is EmbeddedTreeLocation embeddedTreeLocation)
						{
							num |= (int)embeddedTreeLocation.EmbeddedKind;
						}
					}
					Interlocked.CompareExchange(ref _lazyEmbeddedKind, num, 1);
				}
				return (EmbeddedSymbolKind)_lazyEmbeddedKind;
			}
		}

		public override Symbol ContainingSymbol => (Symbol)(((object)_containingNamespace) ?? ((object)_containingModule));

		public override AssemblySymbol ContainingAssembly => _containingModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => _containingModule;

		internal override NamespaceExtent Extent => new NamespaceExtent(_containingModule);

		private Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> NameToMembersMap => GetNameToMembersMap();

		public override ImmutableArray<Location> Locations => StaticCast<Location>.From(_declaration.NameLocations);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ComputeDeclaringReferencesCore();

		internal override ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods
		{
			get
			{
				if (_containingModule.MightContainExtensionMethods)
				{
					return GetModuleMembers();
				}
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}
		}

		internal bool HasMultipleSpellings => (_lazyState & 1) != 0;

		public MergedNamespaceDeclaration MergedDeclaration => _declaration;

		internal SourceNamespaceSymbol(MergedNamespaceDeclaration decl, SourceNamespaceSymbol containingNamespace, SourceModuleSymbol containingModule)
		{
			_lazyEmbeddedKind = 1;
			_lazyLexicalSortKey = LexicalSortKey.NotInitialized;
			_declaration = decl;
			_containingNamespace = containingNamespace;
			_containingModule = containingModule;
			if (((object)containingNamespace != null && containingNamespace.HasMultipleSpellings) || decl.HasMultipleSpellings)
			{
				_lazyState = 1;
			}
		}

		private void RegisterDeclaredCorTypes()
		{
			AssemblySymbol containingAssembly = ContainingAssembly;
			if (!containingAssembly.KeepLookingForDeclaredSpecialTypes)
			{
				return;
			}
			foreach (ImmutableArray<NamespaceOrTypeSymbol> value in _nameToMembersMap.Values)
			{
				ImmutableArray<NamespaceOrTypeSymbol>.Enumerator enumerator2 = value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.SpecialType != 0)
					{
						containingAssembly.RegisterDeclaredSpecialType(namedTypeSymbol);
						if (!containingAssembly.KeepLookingForDeclaredSpecialTypes)
						{
							return;
						}
					}
				}
			}
		}

		private Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> GetNameToMembersMap()
		{
			if (_nameToMembersMap == null)
			{
				Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> value = MakeNameToMembersMap();
				if (Interlocked.CompareExchange(ref _nameToMembersMap, value, null) == null)
				{
					RegisterDeclaredCorTypes();
				}
			}
			return _nameToMembersMap;
		}

		private Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> MakeNameToMembersMap()
		{
			NameToSymbolMapBuilder nameToSymbolMapBuilder = new NameToSymbolMapBuilder(_declaration.Children.Length);
			ImmutableArray<MergedNamespaceOrTypeDeclaration>.Enumerator enumerator = _declaration.Children.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MergedNamespaceOrTypeDeclaration current = enumerator.Current;
				nameToSymbolMapBuilder.Add(BuildSymbol(current));
			}
			return nameToSymbolMapBuilder.CreateMap();
		}

		private NamespaceOrTypeSymbol BuildSymbol(MergedNamespaceOrTypeDeclaration decl)
		{
			if (decl is MergedNamespaceDeclaration decl2)
			{
				return new SourceNamespaceSymbol(decl2, this, _containingModule);
			}
			return SourceMemberContainerTypeSymbol.Create((MergedTypeDeclaration)decl, this, _containingModule);
		}

		private Dictionary<string, ImmutableArray<NamedTypeSymbol>> GetNameToTypeMembersMap()
		{
			if (_nameToTypeMembersMap == null)
			{
				Dictionary<string, ImmutableArray<NamedTypeSymbol>> dictionary = new Dictionary<string, ImmutableArray<NamedTypeSymbol>>(CaseInsensitiveComparison.Comparer);
				Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> nameToMembersMap = GetNameToMembersMap();
				foreach (KeyValuePair<string, ImmutableArray<NamespaceOrTypeSymbol>> item in nameToMembersMap)
				{
					ImmutableArray<NamespaceOrTypeSymbol> value = item.Value;
					bool flag = false;
					bool flag2 = false;
					ImmutableArray<NamespaceOrTypeSymbol>.Enumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Kind == SymbolKind.NamedType)
						{
							flag = true;
							if (flag2)
							{
								break;
							}
						}
						else
						{
							flag2 = true;
							if (flag)
							{
								break;
							}
						}
					}
					if (flag)
					{
						if (flag2)
						{
							dictionary.Add(item.Key, value.OfType<NamedTypeSymbol>().AsImmutable());
						}
						else
						{
							dictionary.Add(item.Key, value.As<NamedTypeSymbol>());
						}
					}
				}
				Interlocked.CompareExchange(ref _nameToTypeMembersMap, dictionary, null);
			}
			return _nameToTypeMembersMap;
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			if (((uint)_lazyState & 2u) != 0)
			{
				return _lazyAllMembers;
			}
			ImmutableArray<Symbol> immutableArray = GetMembersUnordered();
			if (immutableArray.Length >= 2)
			{
				immutableArray = immutableArray.Sort(LexicalOrderSymbolComparer.Instance);
				ImmutableInterlocked.InterlockedExchange(ref _lazyAllMembers, immutableArray);
			}
			ThreadSafeFlagOperations.Set(ref _lazyState, 2);
			return immutableArray;
		}

		internal override ImmutableArray<Symbol> GetMembersUnordered()
		{
			if (_lazyAllMembers.IsDefault)
			{
				ImmutableArray<Symbol> value = StaticCast<Symbol>.From(GetNameToMembersMap().Flatten());
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyAllMembers, value, default(ImmutableArray<Symbol>));
			}
			return _lazyAllMembers.ConditionallyDeOrder();
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			ImmutableArray<NamespaceOrTypeSymbol> value = default(ImmutableArray<NamespaceOrTypeSymbol>);
			if (GetNameToMembersMap().TryGetValue(name, out value))
			{
				return ImmutableArray<Symbol>.CastUp(value);
			}
			return ImmutableArray<Symbol>.Empty;
		}

		internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			return GetNameToTypeMembersMap().Flatten();
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return GetNameToTypeMembersMap().Flatten(LexicalOrderSymbolComparer.Instance);
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			ImmutableArray<NamedTypeSymbol> value = default(ImmutableArray<NamedTypeSymbol>);
			if (GetNameToTypeMembersMap().TryGetValue(name, out value))
			{
				return value;
			}
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers()
		{
			if (_lazyModuleMembers.IsDefault)
			{
				ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
				ImmutableArray<MergedNamespaceOrTypeDeclaration>.Enumerator enumerator = _declaration.Children.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MergedNamespaceOrTypeDeclaration current = enumerator.Current;
					if (current.Kind == DeclarationKind.Module)
					{
						instance.AddRange(GetModuleMembers(current.Name));
					}
				}
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyModuleMembers, instance.ToImmutableAndFree(), default(ImmutableArray<NamedTypeSymbol>));
			}
			return _lazyModuleMembers;
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			if (!_lazyLexicalSortKey.IsInitialized)
			{
				ref LexicalSortKey lazyLexicalSortKey = ref _lazyLexicalSortKey;
				LexicalSortKey other = _declaration.GetLexicalSortKey(DeclaringCompilation);
				lazyLexicalSortKey.SetFrom(ref other);
			}
			return _lazyLexicalSortKey;
		}

		private ImmutableArray<SyntaxReference> ComputeDeclaringReferencesCore()
		{
			ImmutableArray<SingleNamespaceDeclaration> declarations = _declaration.Declarations;
			ArrayBuilder<SyntaxReference> instance = ArrayBuilder<SyntaxReference>.GetInstance(declarations.Length);
			ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference syntaxReference = enumerator.Current.SyntaxReference;
				if (syntaxReference != null && !EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(syntaxReference.SyntaxTree))
				{
					instance.Add(new NamespaceDeclarationSyntaxReference(syntaxReference));
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (IsGlobalNamespace)
			{
				return true;
			}
			ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _declaration.Declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingleNamespaceDeclaration current = enumerator.Current;
				cancellationToken.ThrowIfCancellationRequested();
				SyntaxReference syntaxReference = current.SyntaxReference;
				if (syntaxReference != null && syntaxReference.SyntaxTree == tree)
				{
					if (!EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(syntaxReference.SyntaxTree))
					{
						SyntaxNode syntaxNode = new NamespaceDeclarationSyntaxReference(syntaxReference).GetSyntax(cancellationToken);
						if (syntaxNode is NamespaceStatementSyntax)
						{
							syntaxNode = syntaxNode.Parent;
						}
						if (Symbol.IsDefinedInSourceTree(syntaxNode, tree, definedWithinSpan, cancellationToken))
						{
							return true;
						}
					}
				}
				else if (current.IsPartOfRootNamespace)
				{
					return true;
				}
			}
			return false;
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			base.GenerateDeclarationErrors(cancellationToken);
			ValidateDeclaration(null, cancellationToken);
			GetMembers();
		}

		internal void GenerateDeclarationErrorsInTree(SyntaxTree tree, TextSpan? filterSpanWithinTree, CancellationToken cancellationToken)
		{
			ValidateDeclaration(tree, cancellationToken);
			GetMembers();
		}

		private void ValidateDeclaration(SyntaxTree tree, CancellationToken cancellationToken)
		{
			if (((uint)_lazyState & 4u) != 0)
			{
				return;
			}
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			bool reportedNamespaceMismatch = false;
			ImmutableArray<SyntaxReference>.Enumerator enumerator = _declaration.SyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				if (tree == null || current.SyntaxTree == tree)
				{
					_ = current.SyntaxTree;
					VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(current, cancellationToken);
					switch (visualBasicSyntax.Kind())
					{
					case SyntaxKind.IdentifierName:
						ValidateNamespaceNameSyntax((IdentifierNameSyntax)visualBasicSyntax, instance, ref reportedNamespaceMismatch);
						break;
					case SyntaxKind.QualifiedName:
						ValidateNamespaceNameSyntax(((QualifiedNameSyntax)visualBasicSyntax).Right, instance, ref reportedNamespaceMismatch);
						break;
					case SyntaxKind.GlobalName:
						ValidateNamespaceGlobalSyntax((GlobalNameSyntax)visualBasicSyntax, instance);
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(visualBasicSyntax.Kind());
					case SyntaxKind.CompilationUnit:
						break;
					}
					cancellationToken.ThrowIfCancellationRequested();
				}
			}
			if (_containingModule.AtomicSetFlagAndStoreDiagnostics(ref _lazyState, 4, 0, new BindingDiagnosticBag(instance)))
			{
				DeclaringCompilation.SymbolDeclaredEvent(this);
			}
			instance.Free();
		}

		private void ValidateNamespaceNameSyntax(SimpleNameSyntax node, DiagnosticBag diagnostics, ref bool reportedNamespaceMismatch)
		{
			if (VisualBasicExtensions.GetTypeCharacter(node.Identifier) != 0)
			{
				VBDiagnostic diag = new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_TypecharNotallowed), node.GetLocation());
				diagnostics.Add(diag);
			}
			if (!reportedNamespaceMismatch && string.Compare(node.Identifier.ValueText, Name, StringComparison.Ordinal) != 0)
			{
				object objectValue = RuntimeHelpers.GetObjectValue(GetSourcePathForDeclaration());
				VBDiagnostic diag2 = new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_NamespaceCaseMismatch3, node.Identifier.ValueText, Name, objectValue), node.GetLocation());
				diagnostics.Add(diag2);
				reportedNamespaceMismatch = true;
			}
		}

		private void ValidateNamespaceGlobalSyntax(GlobalNameSyntax node, DiagnosticBag diagnostics)
		{
			VisualBasicSyntaxNode parent = node.Parent;
			bool flag = false;
			while (parent != null)
			{
				if (parent.Kind() == SyntaxKind.NamespaceBlock)
				{
					if (flag)
					{
						VBDiagnostic diag = new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_NestedGlobalNamespace), node.GetLocation());
						diagnostics.Add(diag);
					}
					else
					{
						flag = true;
					}
				}
				parent = parent.Parent;
			}
		}

		private object GetSourcePathForDeclaration()
		{
			object obj = null;
			ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _declaration.Declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingleNamespaceDeclaration current = enumerator.Current;
				if (string.Compare(Name, current.Name, StringComparison.Ordinal) != 0)
				{
					continue;
				}
				if (current.IsPartOfRootNamespace)
				{
					obj = new LocalizableErrorArgument(ERRID.IDS_ProjectSettingsLocationName);
				}
				else if (current.SyntaxReference != null && current.SyntaxReference.SyntaxTree.FilePath != null)
				{
					string filePath = current.SyntaxReference.SyntaxTree.FilePath;
					if (obj == null)
					{
						obj = filePath;
					}
					else if (string.Compare(obj.ToString(), filePath.ToString(), StringComparison.Ordinal) > 0)
					{
						obj = filePath;
					}
				}
			}
			return obj;
		}

		internal string GetDeclarationSpelling(SyntaxTree tree, int location)
		{
			if (!HasMultipleSpellings)
			{
				return ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
			}
			SingleNamespaceDeclaration singleNamespaceDeclaration = _declaration.Declarations.FirstOrDefault(delegate(SingleNamespaceDeclaration decl)
			{
				NamespaceBlockSyntax namespaceBlockSyntax = decl.GetNamespaceBlockSyntax();
				return namespaceBlockSyntax != null && namespaceBlockSyntax.SyntaxTree == tree && namespaceBlockSyntax.Span.Contains(location);
			});
			if (singleNamespaceDeclaration == null)
			{
				singleNamespaceDeclaration = _declaration.Declarations.FirstOrDefault((SingleNamespaceDeclaration decl) => decl.GetNamespaceBlockSyntax() == null);
			}
			string text = ((singleNamespaceDeclaration != null) ? singleNamespaceDeclaration.Name : Name);
			return (!(base.ContainingNamespace is SourceNamespaceSymbol sourceNamespaceSymbol) || EmbeddedOperators.CompareString(sourceNamespaceSymbol.Name, "", TextCompare: false) == 0) ? text : (sourceNamespaceSymbol.GetDeclarationSpelling(tree, location) + "." + text);
		}
	}
}
