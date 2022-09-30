using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceModuleSymbol : NonMissingModuleSymbol, IAttributeTargetSymbol
	{
		private sealed class BoundImports
		{
			public readonly ImmutableArray<NamespaceOrTypeAndImportsClausePosition> MemberImports;

			public readonly ImmutableArray<GlobalImportInfo> MemberImportsInfo;

			public readonly Dictionary<string, AliasAndImportsClausePosition> AliasImportsMap;

			public readonly ImmutableArray<AliasAndImportsClausePosition> AliasImports;

			public readonly ImmutableArray<GlobalImportInfo> AliasImportsInfo;

			public readonly Dictionary<string, XmlNamespaceAndImportsClausePosition> XmlNamespaces;

			public readonly BindingDiagnosticBag Diagnostics;

			public BoundImports(ImmutableArray<NamespaceOrTypeAndImportsClausePosition> memberImports, ImmutableArray<GlobalImportInfo> memberImportsInfo, Dictionary<string, AliasAndImportsClausePosition> aliasImportsMap, ImmutableArray<AliasAndImportsClausePosition> aliasImports, ImmutableArray<GlobalImportInfo> aliasImportsInfo, Dictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespaces, BindingDiagnosticBag diags)
			{
				MemberImports = memberImports;
				MemberImportsInfo = memberImportsInfo;
				AliasImportsMap = aliasImportsMap;
				AliasImports = aliasImports;
				AliasImportsInfo = aliasImportsInfo;
				XmlNamespaces = xmlNamespaces;
				Diagnostics = diags;
			}
		}

		private struct GlobalImportInfo
		{
			public readonly GlobalImport Import;

			public readonly SyntaxReference SyntaxReference;

			public GlobalImportInfo(GlobalImport import, SyntaxReference syntaxReference)
			{
				this = default(GlobalImportInfo);
				Import = import;
				SyntaxReference = syntaxReference;
			}
		}

		private sealed class ModuleImportData : ImportData
		{
			private readonly GlobalImport _globalImport;

			private readonly ArrayBuilder<NamespaceOrTypeAndImportsClausePosition> _membersBuilder;

			private readonly ArrayBuilder<GlobalImportInfo> _membersInfoBuilder;

			private readonly ArrayBuilder<AliasAndImportsClausePosition> _aliasesBuilder;

			private readonly ArrayBuilder<GlobalImportInfo> _aliasesInfoBuilder;

			private readonly ICollection<AssemblySymbol> _dependencies;

			public ModuleImportData(GlobalImport globalImport, HashSet<NamespaceOrTypeSymbol> membersMap, Dictionary<string, AliasAndImportsClausePosition> aliasesMap, ArrayBuilder<NamespaceOrTypeAndImportsClausePosition> membersBuilder, ArrayBuilder<GlobalImportInfo> membersInfoBuilder, ArrayBuilder<AliasAndImportsClausePosition> aliasesBuilder, ArrayBuilder<GlobalImportInfo> aliasesInfoBuilder, Dictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespaces, ICollection<AssemblySymbol> dependencies)
				: base(membersMap, aliasesMap, xmlNamespaces)
			{
				_globalImport = globalImport;
				_membersBuilder = membersBuilder;
				_membersInfoBuilder = membersInfoBuilder;
				_aliasesBuilder = aliasesBuilder;
				_aliasesInfoBuilder = aliasesInfoBuilder;
				_dependencies = dependencies;
			}

			public override void AddMember(SyntaxReference syntaxRef, NamespaceOrTypeSymbol member, int importsClausePosition, IReadOnlyCollection<AssemblySymbol> dependencies)
			{
				NamespaceOrTypeAndImportsClausePosition item = new NamespaceOrTypeAndImportsClausePosition(member, importsClausePosition, ImmutableArray<AssemblySymbol>.Empty);
				Members.Add(member);
				_membersBuilder.Add(item);
				_membersInfoBuilder.Add(new GlobalImportInfo(_globalImport, syntaxRef));
				AddDependencies(dependencies);
			}

			private void AddDependencies(IReadOnlyCollection<AssemblySymbol> dependencies)
			{
				foreach (AssemblySymbol dependency in dependencies)
				{
					_dependencies.Add(dependency);
				}
			}

			public override void AddAlias(SyntaxReference syntaxRef, string name, AliasSymbol alias, int importsClausePosition, IReadOnlyCollection<AssemblySymbol> dependencies)
			{
				AliasAndImportsClausePosition aliasAndImportsClausePosition = new AliasAndImportsClausePosition(alias, importsClausePosition, ImmutableArray<AssemblySymbol>.Empty);
				Aliases.Add(name, aliasAndImportsClausePosition);
				_aliasesBuilder.Add(aliasAndImportsClausePosition);
				_aliasesInfoBuilder.Add(new GlobalImportInfo(_globalImport, syntaxRef));
				AddDependencies(dependencies);
			}
		}

		private readonly SourceAssemblySymbol _assemblySymbol;

		private readonly DeclarationTable _declarationTable;

		private readonly VisualBasicCompilationOptions _options;

		private CustomAttributesBag<VisualBasicAttributeData> _lazyCustomAttributesBag;

		private byte _lazyContainsExtensionMethods;

		private ImmutableArray<AssemblySymbol> _lazyAssembliesToEmbedTypesFrom;

		private ThreeState _lazyContainsExplicitDefinitionOfNoPiaLocalTypes;

		private ImmutableArray<Location> _locations;

		private readonly DiagnosticBag _diagnosticBagDeclare;

		private bool _hasBadAttributes;

		private readonly ConcurrentDictionary<SyntaxTree, SourceFile> _sourceFileMap;

		private SourceNamespaceSymbol _lazyGlobalNamespace;

		private BoundImports _lazyBoundImports;

		private QuickAttributeChecker _lazyQuickAttributeChecker;

		private ImmutableArray<Diagnostic> _lazyLinkedAssemblyDiagnostics;

		private Dictionary<NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol> _lazyTypesWithDefaultInstanceAlias;

		private static readonly Dictionary<NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol> s_noTypesWithDefaultInstanceAlias = new Dictionary<NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol>();

		private readonly string _nameAndExtension;

		private readonly object _diagnosticLock;

		internal VisualBasicCompilationOptions Options => _options;

		internal override int Ordinal => 0;

		internal override Machine Machine => DeclaringCompilation.Options.Platform switch
		{
			Platform.Arm64 => Machine.Arm64, 
			Platform.Arm => Machine.ArmThumb2, 
			Platform.X64 => Machine.Amd64, 
			Platform.Itanium => Machine.IA64, 
			_ => Machine.I386, 
		};

		internal override bool Bit32Required => DeclaringCompilation.Options.Platform == Platform.X86;

		public override Symbol ContainingSymbol => _assemblySymbol;

		public override AssemblySymbol ContainingAssembly => _assemblySymbol;

		public SourceAssemblySymbol ContainingSourceAssembly => _assemblySymbol;

		internal override VisualBasicCompilation DeclaringCompilation => _assemblySymbol.DeclaringCompilation;

		public override string Name => _nameAndExtension;

		public override NamespaceSymbol GlobalNamespace
		{
			get
			{
				if ((object)_lazyGlobalNamespace == null)
				{
					SourceNamespaceSymbol value = new SourceNamespaceSymbol(DeclaringCompilation.MergedRootDeclaration, null, this);
					Interlocked.CompareExchange(ref _lazyGlobalNamespace, value, null);
				}
				return _lazyGlobalNamespace;
			}
		}

		internal NamespaceSymbol RootNamespace => GlobalNamespace.LookupNestedNamespace(Options.GetRootNamespaceParts());

		public override ImmutableArray<Location> Locations
		{
			get
			{
				if (_locations.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _locations, DeclaringCompilation.MergedRootDeclaration.Declarations.SelectAsArray((SingleNamespaceDeclaration d) => d.Location));
				}
				return _locations;
			}
		}

		internal IEnumerable<SyntaxTree> SyntaxTrees => _assemblySymbol.DeclaringCompilation.AllSyntaxTrees;

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Module;

		public QuickAttributeChecker QuickAttributeChecker
		{
			get
			{
				if (_lazyQuickAttributeChecker == null)
				{
					Interlocked.CompareExchange(ref _lazyQuickAttributeChecker, CreateQuickAttributeChecker(), null);
				}
				return _lazyQuickAttributeChecker;
			}
		}

		internal bool AnyReferencedAssembliesAreLinked => GetAssembliesToEmbedTypesFrom().Length > 0;

		internal bool ContainsExplicitDefinitionOfNoPiaLocalTypes
		{
			get
			{
				if (_lazyContainsExplicitDefinitionOfNoPiaLocalTypes == ThreeState.Unknown)
				{
					_lazyContainsExplicitDefinitionOfNoPiaLocalTypes = NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes(GlobalNamespace).ToThreeState();
				}
				return _lazyContainsExplicitDefinitionOfNoPiaLocalTypes == ThreeState.True;
			}
		}

		internal ImmutableArray<NamespaceOrTypeAndImportsClausePosition> MemberImports
		{
			get
			{
				EnsureImportsAreBound(CancellationToken.None);
				return _lazyBoundImports.MemberImports;
			}
		}

		internal ImmutableArray<AliasAndImportsClausePosition> AliasImports
		{
			get
			{
				EnsureImportsAreBound(CancellationToken.None);
				return _lazyBoundImports.AliasImports;
			}
		}

		internal Dictionary<string, AliasAndImportsClausePosition> AliasImportsMap
		{
			get
			{
				EnsureImportsAreBound(CancellationToken.None);
				return _lazyBoundImports.AliasImportsMap;
			}
		}

		internal Dictionary<string, XmlNamespaceAndImportsClausePosition> XmlNamespaces
		{
			get
			{
				EnsureImportsAreBound(CancellationToken.None);
				return _lazyBoundImports.XmlNamespaces;
			}
		}

		internal bool HasBadAttributes => _hasBadAttributes;

		internal override ICollection<string> TypeNames => _declarationTable.TypeNames;

		internal override ICollection<string> NamespaceNames => _declarationTable.NamespaceNames;

		internal override bool MightContainExtensionMethods
		{
			get
			{
				if (_lazyContainsExtensionMethods == 0 && (object)_assemblySymbol.Modules[0] != this)
				{
					_lazyContainsExtensionMethods = 1;
				}
				return _lazyContainsExtensionMethods != 1;
			}
		}

		internal override bool HasAssemblyCompilationRelaxationsAttribute => ((SourceAssemblySymbol)ContainingAssembly).GetSourceDecodedWellKnownAttributeData()?.HasCompilationRelaxationsAttribute ?? false;

		internal override bool HasAssemblyRuntimeCompatibilityAttribute => ((SourceAssemblySymbol)ContainingAssembly).GetSourceDecodedWellKnownAttributeData()?.HasRuntimeCompatibilityAttribute ?? false;

		internal override CharSet? DefaultMarshallingCharSet
		{
			get
			{
				CommonModuleWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData == null || !decodedWellKnownAttributeData.HasDefaultCharSetAttribute)
				{
					return (CharSet?)(object)null;
				}
				return decodedWellKnownAttributeData.DefaultCharacterSet;
			}
		}

		internal SourceModuleSymbol(SourceAssemblySymbol assemblySymbol, DeclarationTable declarationTable, VisualBasicCompilationOptions options, string nameAndExtension)
		{
			_lazyContainsExtensionMethods = 0;
			_lazyContainsExplicitDefinitionOfNoPiaLocalTypes = ThreeState.Unknown;
			_diagnosticBagDeclare = new DiagnosticBag();
			_sourceFileMap = new ConcurrentDictionary<SyntaxTree, SourceFile>();
			_diagnosticLock = RuntimeHelpers.GetObjectValue(new object());
			_assemblySymbol = assemblySymbol;
			_declarationTable = declarationTable;
			_options = options;
			_nameAndExtension = nameAndExtension;
		}

		internal SourceFile TryGetSourceFile(SyntaxTree tree)
		{
			SourceFile value = null;
			if (_sourceFileMap.TryGetValue(tree, out value))
			{
				return value;
			}
			if (_assemblySymbol.DeclaringCompilation.AllSyntaxTrees.Contains(tree))
			{
				value = new SourceFile(this, tree);
				return _sourceFileMap.GetOrAdd(tree, value);
			}
			return null;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return GetAttributesBag().Attributes;
		}

		private CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			if (_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag.IsSealed)
			{
				ImmutableArray<SyntaxList<AttributeListSyntax>> attributeDeclarations = ((SourceAssemblySymbol)ContainingAssembly).GetAttributeDeclarations();
				LoadAndValidateAttributes(OneOrMany.Create(attributeDeclarations), ref _lazyCustomAttributesBag);
			}
			return _lazyCustomAttributesBag;
		}

		internal CommonModuleWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (CommonModuleWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		internal bool MightContainNoPiaLocalTypes()
		{
			if (!AnyReferencedAssembliesAreLinked)
			{
				return ContainsExplicitDefinitionOfNoPiaLocalTypes;
			}
			return true;
		}

		internal ImmutableArray<AssemblySymbol> GetAssembliesToEmbedTypesFrom()
		{
			if (_lazyAssembliesToEmbedTypesFrom.IsDefault)
			{
				ArrayBuilder<AssemblySymbol> instance = ArrayBuilder<AssemblySymbol>.GetInstance();
				ImmutableArray<AssemblySymbol>.Enumerator enumerator = GetReferencedAssemblySymbols().GetEnumerator();
				while (enumerator.MoveNext())
				{
					AssemblySymbol current = enumerator.Current;
					if (current.IsLinked)
					{
						instance.Add(current);
					}
				}
				ImmutableInterlocked.InterlockedInitialize(ref _lazyAssembliesToEmbedTypesFrom, instance.ToImmutableAndFree());
			}
			return _lazyAssembliesToEmbedTypesFrom;
		}

		private static bool NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes(NamespaceSymbol ns)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = ns.GetMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				switch (current.Kind)
				{
				case SymbolKind.Namespace:
					if (NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes((NamespaceSymbol)current))
					{
						return true;
					}
					break;
				case SymbolKind.NamedType:
					if (((NamedTypeSymbol)current).IsExplicitDefinitionOfNoPiaLocalType)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		private QuickAttributeChecker CreateQuickAttributeChecker()
		{
			QuickAttributeChecker quickAttributeChecker = new QuickAttributeChecker();
			quickAttributeChecker.AddName(AttributeDescription.CaseInsensitiveExtensionAttribute.Name, QuickAttributes.Extension);
			quickAttributeChecker.AddName(AttributeDescription.ObsoleteAttribute.Name, QuickAttributes.Obsolete);
			quickAttributeChecker.AddName(AttributeDescription.DeprecatedAttribute.Name, QuickAttributes.Obsolete);
			quickAttributeChecker.AddName(AttributeDescription.ExperimentalAttribute.Name, QuickAttributes.Obsolete);
			quickAttributeChecker.AddName(AttributeDescription.MyGroupCollectionAttribute.Name, QuickAttributes.MyGroupCollection);
			quickAttributeChecker.AddName(AttributeDescription.TypeIdentifierAttribute.Name, QuickAttributes.TypeIdentifier);
			ImmutableArray<GlobalImport>.Enumerator enumerator = Options.GlobalImports.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GlobalImport current = enumerator.Current;
				if (current.Clause.Kind() == SyntaxKind.SimpleImportsClause)
				{
					SimpleImportsClauseSyntax simpleImportsClauseSyntax = (SimpleImportsClauseSyntax)current.Clause;
					if (simpleImportsClauseSyntax.Alias != null)
					{
						quickAttributeChecker.AddAlias(simpleImportsClauseSyntax);
					}
				}
			}
			quickAttributeChecker.Seal();
			return quickAttributeChecker;
		}

		private void EnsureImportsAreBound(CancellationToken cancellationToken)
		{
			if (_lazyBoundImports == null && Interlocked.CompareExchange(ref _lazyBoundImports, BindImports(cancellationToken), null) == null)
			{
				ValidateImports(_lazyBoundImports.MemberImports, _lazyBoundImports.MemberImportsInfo, _lazyBoundImports.AliasImports, _lazyBoundImports.AliasImportsInfo, _lazyBoundImports.Diagnostics);
			}
		}

		private BoundImports BindImports(CancellationToken cancellationToken)
		{
			BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag();
			HashSet<NamespaceOrTypeSymbol> membersMap = new HashSet<NamespaceOrTypeSymbol>();
			Dictionary<string, AliasAndImportsClausePosition> dictionary = new Dictionary<string, AliasAndImportsClausePosition>(CaseInsensitiveComparison.Comparer);
			ArrayBuilder<NamespaceOrTypeAndImportsClausePosition> instance = ArrayBuilder<NamespaceOrTypeAndImportsClausePosition>.GetInstance();
			ArrayBuilder<GlobalImportInfo> instance2 = ArrayBuilder<GlobalImportInfo>.GetInstance();
			ArrayBuilder<AliasAndImportsClausePosition> instance3 = ArrayBuilder<AliasAndImportsClausePosition>.GetInstance();
			ArrayBuilder<GlobalImportInfo> instance4 = ArrayBuilder<GlobalImportInfo>.GetInstance();
			Dictionary<string, XmlNamespaceAndImportsClausePosition> dictionary2 = new Dictionary<string, XmlNamespaceAndImportsClausePosition>();
			BindingDiagnosticBag instance5 = BindingDiagnosticBag.GetInstance();
			try
			{
				ImmutableArray<GlobalImport>.Enumerator enumerator = Options.GlobalImports.GetEnumerator();
				while (enumerator.MoveNext())
				{
					GlobalImport current = enumerator.Current;
					cancellationToken.ThrowIfCancellationRequested();
					instance5.Clear();
					ModuleImportData data = new ModuleImportData(current, membersMap, dictionary, instance, instance2, instance3, instance4, dictionary2, instance5.DependenciesBag);
					BinderBuilder.CreateBinderForProjectImports(this, VisualBasicSyntaxTree.Dummy).BindImportClause(current.Clause, data, instance5.DiagnosticBag);
					foreach (Diagnostic item in instance5.DiagnosticBag!.AsEnumerable())
					{
						if (item.Code != 31051)
						{
							bindingDiagnosticBag.Add(current.MapDiagnostic(item));
						}
					}
					bindingDiagnosticBag.AddDependencies(instance5);
				}
				return new BoundImports(instance.ToImmutable(), instance2.ToImmutable(), dictionary, instance3.ToImmutable(), instance4.ToImmutable(), (dictionary2.Count > 0) ? dictionary2 : null, bindingDiagnosticBag);
			}
			finally
			{
				instance.Free();
				instance2.Free();
				instance3.Free();
				instance4.Free();
				instance5.Free();
			}
		}

		private void ValidateImports(ImmutableArray<NamespaceOrTypeAndImportsClausePosition> memberImports, ImmutableArray<GlobalImportInfo> memberImportsInfo, ImmutableArray<AliasAndImportsClausePosition> aliasImports, ImmutableArray<GlobalImportInfo> aliasImportsInfo, BindingDiagnosticBag diagnostics)
		{
			if (!memberImports.IsDefault)
			{
				int num = memberImports.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					NamespaceOrTypeSymbol namespaceOrType = memberImports[i].NamespaceOrType;
					if (namespaceOrType is TypeSymbol type)
					{
						ValidateImport(type, memberImportsInfo[i], diagnostics);
					}
					else
					{
						diagnostics.AddAssembliesUsedByNamespaceReference((NamespaceSymbol)namespaceOrType);
					}
				}
			}
			if (aliasImports.IsDefault)
			{
				return;
			}
			int num2 = aliasImports.Length - 1;
			for (int j = 0; j <= num2; j++)
			{
				NamespaceOrTypeSymbol target = aliasImports[j].Alias.Target;
				if (target is TypeSymbol type2)
				{
					ValidateImport(type2, aliasImportsInfo[j], diagnostics);
				}
				else
				{
					diagnostics.AddAssembliesUsedByNamespaceReference((NamespaceSymbol)target);
				}
			}
		}

		private void ValidateImport(TypeSymbol type, GlobalImportInfo info, BindingDiagnosticBag diagnostics)
		{
			ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
			ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
			ConstraintsHelper.CheckAllConstraints(type, instance, ref useSiteDiagnosticsBuilder, new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly));
			if (useSiteDiagnosticsBuilder != null)
			{
				instance.AddRange(useSiteDiagnosticsBuilder);
			}
			ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterDiagnosticInfo current = enumerator.Current;
				if (current.UseSiteInfo.DiagnosticInfo != null)
				{
					diagnostics.Add(info.Import.MapDiagnostic(new VBDiagnostic(current.UseSiteInfo.DiagnosticInfo, info.SyntaxReference.GetLocation())));
				}
				diagnostics.AddDependencies(current.UseSiteInfo);
			}
			instance.Free();
		}

		internal ImmutableArray<Diagnostic> GetDeclarationErrorsInTree(SyntaxTree tree, TextSpan? filterSpanWithinTree, Func<IEnumerable<Diagnostic>, SyntaxTree, TextSpan?, IEnumerable<Diagnostic>> locationFilter, CancellationToken cancellationToken)
		{
			_Closure_0024__77_002D0 arg = default(_Closure_0024__77_002D0);
			_Closure_0024__77_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__77_002D0(arg);
			CS_0024_003C_003E8__locals0._0024VB_0024Local_tree = tree;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_filterSpanWithinTree = filterSpanWithinTree;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken = cancellationToken;
			ArrayBuilder<Diagnostic> instance = ArrayBuilder<Diagnostic>.GetInstance();
			SourceFile sourceFile = TryGetSourceFile(CS_0024_003C_003E8__locals0._0024VB_0024Local_tree);
			if (CS_0024_003C_003E8__locals0._0024VB_0024Local_filterSpanWithinTree.HasValue)
			{
				IEnumerable<Diagnostic> declarationErrorsInSpan = sourceFile.GetDeclarationErrorsInSpan(CS_0024_003C_003E8__locals0._0024VB_0024Local_filterSpanWithinTree.Value, CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
				declarationErrorsInSpan = locationFilter(declarationErrorsInSpan, CS_0024_003C_003E8__locals0._0024VB_0024Local_tree, CS_0024_003C_003E8__locals0._0024VB_0024Local_filterSpanWithinTree);
				instance.AddRange(declarationErrorsInSpan);
			}
			else
			{
				sourceFile.GenerateAllDeclarationErrors();
			}
			GetAttributes();
			ContainingAssembly.GetAttributes();
			ConcurrentStack<Task> concurrentStack = (ContainingSourceAssembly.DeclaringCompilation.Options.ConcurrentBuild ? new ConcurrentStack<Task>() : null);
			VisitAllSourceTypesAndNamespaces(delegate(NamespaceOrTypeSymbol typeOrNamespace)
			{
				if (typeOrNamespace.IsDefinedInSourceTree(CS_0024_003C_003E8__locals0._0024VB_0024Local_tree, CS_0024_003C_003E8__locals0._0024VB_0024Local_filterSpanWithinTree))
				{
					if (typeOrNamespace.IsNamespace)
					{
						((SourceNamespaceSymbol)typeOrNamespace).GenerateDeclarationErrorsInTree(CS_0024_003C_003E8__locals0._0024VB_0024Local_tree, CS_0024_003C_003E8__locals0._0024VB_0024Local_filterSpanWithinTree, CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
					}
					else
					{
						((NamedTypeSymbol)typeOrNamespace).GenerateDeclarationErrors(CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
					}
				}
			}, concurrentStack, CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
			if (concurrentStack != null)
			{
				Task result = null;
				while (concurrentStack.TryPop(out result))
				{
					result.GetAwaiter().GetResult();
				}
			}
			IEnumerable<Diagnostic> enumerable = sourceFile.DeclarationDiagnostics.AsEnumerable();
			if (locationFilter != null)
			{
				enumerable = locationFilter(enumerable, CS_0024_003C_003E8__locals0._0024VB_0024Local_tree, CS_0024_003C_003E8__locals0._0024VB_0024Local_filterSpanWithinTree);
			}
			foreach (Diagnostic item in enumerable)
			{
				instance.Add(item);
			}
			return instance.ToImmutableAndFree();
		}

		internal void GetAllDeclarationErrors(BindingDiagnosticBag diagnostics, CancellationToken cancellationToken, ref bool hasExtensionMethods)
		{
			_Closure_0024__78_002D1 arg = default(_Closure_0024__78_002D1);
			_Closure_0024__78_002D1 CS_0024_003C_003E8__locals0 = new _Closure_0024__78_002D1(arg);
			CS_0024_003C_003E8__locals0._0024VB_0024Me = this;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken = cancellationToken;
			EnsureImportsAreBound(CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
			if (!ContainingSourceAssembly.DeclaringCompilation.Options.ConcurrentBuild)
			{
				foreach (SyntaxTree syntaxTree in SyntaxTrees)
				{
					CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken.ThrowIfCancellationRequested();
					TryGetSourceFile(syntaxTree).GenerateAllDeclarationErrors();
				}
			}
			else
			{
				_Closure_0024__78_002D0 arg2 = default(_Closure_0024__78_002D0);
				_Closure_0024__78_002D0 CS_0024_003C_003E8__locals1 = new _Closure_0024__78_002D0(arg2);
				CS_0024_003C_003E8__locals1._0024VB_0024NonLocal__0024VB_0024Closure_2 = CS_0024_003C_003E8__locals0;
				CS_0024_003C_003E8__locals1._0024VB_0024Local_trees = ArrayBuilder<SyntaxTree>.GetInstance();
				CS_0024_003C_003E8__locals1._0024VB_0024Local_trees.AddRange(SyntaxTrees);
				RoslynParallel.For(0, CS_0024_003C_003E8__locals1._0024VB_0024Local_trees.Count, UICultureUtilities.WithCurrentUICulture(delegate(int i)
				{
					CS_0024_003C_003E8__locals1._0024VB_0024NonLocal__0024VB_0024Closure_2._0024VB_0024Me.TryGetSourceFile(CS_0024_003C_003E8__locals1._0024VB_0024Local_trees[i]).GenerateAllDeclarationErrors();
				}), CS_0024_003C_003E8__locals1._0024VB_0024NonLocal__0024VB_0024Closure_2._0024VB_0024Local_cancellationToken);
				CS_0024_003C_003E8__locals1._0024VB_0024Local_trees.Free();
			}
			GetAttributes();
			ContainingAssembly.GetAttributes();
			EnsureLinkedAssembliesAreValidated(CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
			ConcurrentStack<Task> concurrentStack = (ContainingSourceAssembly.DeclaringCompilation.Options.ConcurrentBuild ? new ConcurrentStack<Task>() : null);
			VisitAllSourceTypesAndNamespaces(delegate(NamespaceOrTypeSymbol typeOrNamespace)
			{
				if (typeOrNamespace.IsNamespace)
				{
					((SourceNamespaceSymbol)typeOrNamespace).GenerateDeclarationErrors(CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
				}
				else
				{
					((NamedTypeSymbol)typeOrNamespace).GenerateDeclarationErrors(CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
				}
			}, concurrentStack, CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
			if (concurrentStack != null)
			{
				Task result = null;
				while (concurrentStack.TryPop(out result))
				{
					result.GetAwaiter().GetResult();
				}
			}
			if (_lazyContainsExtensionMethods == 0)
			{
				_lazyContainsExtensionMethods = 1;
			}
			hasExtensionMethods = _lazyContainsExtensionMethods == 2;
			diagnostics.AddRange(_diagnosticBagDeclare);
			diagnostics.AddRange(_lazyBoundImports.Diagnostics, allowMismatchInDependencyAccumulation: true);
			diagnostics.AddRange(_lazyLinkedAssemblyDiagnostics);
			foreach (SyntaxTree syntaxTree2 in SyntaxTrees)
			{
				diagnostics.AddRange(TryGetSourceFile(syntaxTree2).DeclarationDiagnostics);
			}
		}

		private void VisitAllSourceTypesAndNamespaces(Action<NamespaceOrTypeSymbol> visitor, ConcurrentStack<Task> tasks, CancellationToken cancellationToken)
		{
			VisitTypesAndNamespacesWithin(GlobalNamespace, visitor, tasks, cancellationToken);
		}

		private void VisitTypesAndNamespacesWithin(NamespaceOrTypeSymbol ns, Action<NamespaceOrTypeSymbol> visitor, ConcurrentStack<Task> tasks, CancellationToken cancellationToken)
		{
			_Closure_0024__80_002D0 arg = new _Closure_0024__80_002D0(arg);
			arg._0024VB_0024Local_visitor = visitor;
			ArrayBuilder<NamespaceOrTypeSymbol> instance = ArrayBuilder<NamespaceOrTypeSymbol>.GetInstance();
			try
			{
				instance.Push(ns);
				_Closure_0024__80_002D1 closure_0024__80_002D = default(_Closure_0024__80_002D1);
				while (instance.Count > 0)
				{
					closure_0024__80_002D = new _Closure_0024__80_002D1(closure_0024__80_002D);
					closure_0024__80_002D._0024VB_0024NonLocal__0024VB_0024Closure_2 = arg;
					cancellationToken.ThrowIfCancellationRequested();
					closure_0024__80_002D._0024VB_0024Local_symbol = instance.Pop();
					if (tasks != null)
					{
						Task item = Task.Run(UICultureUtilities.WithCurrentUICulture(closure_0024__80_002D._Lambda_0024__0), cancellationToken);
						tasks.Push(item);
					}
					else
					{
						closure_0024__80_002D._0024VB_0024NonLocal__0024VB_0024Closure_2._0024VB_0024Local_visitor(closure_0024__80_002D._0024VB_0024Local_symbol);
					}
					foreach (Symbol item2 in closure_0024__80_002D._0024VB_0024Local_symbol.IsNamespace ? ((IEnumerable<Symbol>)closure_0024__80_002D._0024VB_0024Local_symbol.GetMembers()) : closure_0024__80_002D._0024VB_0024Local_symbol.GetTypeMembers().Cast<Symbol>())
					{
						instance.Push((NamespaceOrTypeSymbol)item2);
					}
				}
			}
			finally
			{
				instance.Free();
			}
		}

		private void EnsureLinkedAssembliesAreValidated(CancellationToken cancellationToken)
		{
			if (_lazyLinkedAssemblyDiagnostics.IsDefault)
			{
				DiagnosticBag instance = DiagnosticBag.GetInstance();
				ValidateLinkedAssemblies(instance, cancellationToken);
				ImmutableInterlocked.InterlockedInitialize(ref _lazyLinkedAssemblyDiagnostics, instance.ToReadOnlyAndFree());
			}
		}

		private void ValidateLinkedAssemblies(DiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			ImmutableArray<AssemblySymbol>.Enumerator enumerator = GetReferencedAssemblySymbols().GetEnumerator();
			while (enumerator.MoveNext())
			{
				AssemblySymbol current = enumerator.Current;
				cancellationToken.ThrowIfCancellationRequested();
				if (current.IsMissing || !current.IsLinked)
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator2 = current.GetAttributes().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					VisualBasicAttributeData current2 = enumerator2.Current;
					if (current2.IsTargetAttribute(current, AttributeDescription.GuidAttribute))
					{
						if (current2.CommonConstructorArguments.Length == 1)
						{
							object objectValue = RuntimeHelpers.GetObjectValue(current2.CommonConstructorArguments[0].ValueInternal);
							if (objectValue == null || objectValue is string)
							{
								flag = true;
							}
						}
					}
					else if (current2.IsTargetAttribute(current, AttributeDescription.ImportedFromTypeLibAttribute))
					{
						if (current2.CommonConstructorArguments.Length == 1)
						{
							flag2 = true;
						}
					}
					else if (current2.IsTargetAttribute(current, AttributeDescription.PrimaryInteropAssemblyAttribute) && current2.CommonConstructorArguments.Length == 2)
					{
						flag2 = true;
					}
					if (flag && flag2)
					{
						break;
					}
				}
				if (!flag)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_PIAHasNoAssemblyGuid1, NoLocation.Singleton, current, AttributeDescription.GuidAttribute.FullName);
				}
				if (!flag2)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_PIAHasNoTypeLibAttribute1, NoLocation.Singleton, current, AttributeDescription.ImportedFromTypeLibAttribute.FullName, AttributeDescription.PrimaryInteropAssemblyAttribute.FullName);
				}
			}
		}

		private static bool HasAllLazyDiagnostics(DiagnosticBag diagBag)
		{
			foreach (Diagnostic item in diagBag.AsEnumerable())
			{
				if (!(item is DiagnosticWithInfo diagnosticWithInfo) || !diagnosticWithInfo.HasLazyInfo)
				{
					return false;
				}
			}
			return true;
		}

		internal bool AtomicStoreReferenceAndDiagnostics<T>(ref T variable, T value, BindingDiagnosticBag diagBag, T comparand = null) where T : class
		{
			if (diagBag == null || diagBag.IsEmpty)
			{
				return Interlocked.CompareExchange(ref variable, value, comparand) == comparand && comparand == null;
			}
			bool flag = false;
			lock (_diagnosticLock)
			{
				if (variable == comparand)
				{
					StoreDeclarationDiagnostics(diagBag);
					flag = Interlocked.CompareExchange(ref variable, value, comparand) == comparand;
					if (!flag && !IsEmptyIgnoringLazyDiagnostics(diagBag))
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
			}
			return flag && comparand == null;
		}

		private static bool IsEmptyIgnoringLazyDiagnostics(BindingDiagnosticBag diagBag)
		{
			if (diagBag.DependenciesBag.IsNullOrEmpty())
			{
				if (diagBag.AccumulatesDiagnostics)
				{
					return HasAllLazyDiagnostics(diagBag.DiagnosticBag);
				}
				return true;
			}
			return false;
		}

		internal void AtomicStoreIntegerAndDiagnostics(ref int variable, int value, int comparand, BindingDiagnosticBag diagBag)
		{
			if (diagBag == null || diagBag.IsEmpty)
			{
				Interlocked.CompareExchange(ref variable, value, comparand);
				return;
			}
			lock (_diagnosticLock)
			{
				if (variable == comparand)
				{
					StoreDeclarationDiagnostics(diagBag);
					if (Interlocked.CompareExchange(ref variable, value, comparand) != comparand && !IsEmptyIgnoringLazyDiagnostics(diagBag))
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
			}
		}

		internal bool AtomicSetFlagAndStoreDiagnostics(ref int variable, int mask, int comparand, BindingDiagnosticBag diagBag)
		{
			if (diagBag == null || diagBag.IsEmpty)
			{
				return ThreadSafeFlagOperations.Set(ref variable, mask);
			}
			lock (_diagnosticLock)
			{
				bool num = (variable & mask) == comparand;
				if (num)
				{
					StoreDeclarationDiagnostics(diagBag);
					if (!ThreadSafeFlagOperations.Set(ref variable, mask) && !IsEmptyIgnoringLazyDiagnostics(diagBag))
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
				return num;
			}
		}

		internal bool AtomicSetFlagAndRaiseSymbolDeclaredEvent(ref int variable, int mask, int comparand, Symbol symbol)
		{
			lock (_diagnosticLock)
			{
				bool num = (variable & mask) == comparand;
				if (num)
				{
					DeclaringCompilation.SymbolDeclaredEvent(symbol);
					if (!ThreadSafeFlagOperations.Set(ref variable, mask))
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
				return num;
			}
		}

		internal bool AtomicStoreArrayAndDiagnostics<T>(ref ImmutableArray<T> variable, ImmutableArray<T> value, BindingDiagnosticBag diagBag)
		{
			if (diagBag == null || diagBag.IsEmpty)
			{
				return ImmutableInterlocked.InterlockedInitialize(ref variable, value);
			}
			lock (_diagnosticLock)
			{
				if (variable.IsDefault)
				{
					StoreDeclarationDiagnostics(diagBag);
					bool num = ImmutableInterlocked.InterlockedInitialize(ref variable, value);
					if (!num && !IsEmptyIgnoringLazyDiagnostics(diagBag))
					{
						throw ExceptionUtilities.Unreachable;
					}
					return num;
				}
				return false;
			}
		}

		internal void AtomicStoreAttributesAndDiagnostics(CustomAttributesBag<VisualBasicAttributeData> attributesBag, ImmutableArray<VisualBasicAttributeData> attributesToStore, BindingDiagnosticBag diagBag)
		{
			RecordPresenceOfBadAttributes(attributesToStore);
			if (diagBag == null || diagBag.IsEmpty)
			{
				attributesBag.SetAttributes(attributesToStore);
				return;
			}
			lock (_diagnosticLock)
			{
				if (!attributesBag.IsSealed)
				{
					StoreDeclarationDiagnostics(diagBag);
					if (!attributesBag.SetAttributes(attributesToStore) && !IsEmptyIgnoringLazyDiagnostics(diagBag))
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
			}
		}

		private void RecordPresenceOfBadAttributes(ImmutableArray<VisualBasicAttributeData> attributes)
		{
			if (_hasBadAttributes)
			{
				return;
			}
			ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = attributes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.HasErrors)
				{
					_hasBadAttributes = true;
					break;
				}
			}
		}

		internal void AddDeclarationDiagnostics(BindingDiagnosticBag diagBag)
		{
			if (!diagBag.IsEmpty)
			{
				lock (_diagnosticLock)
				{
					StoreDeclarationDiagnostics(diagBag);
				}
			}
		}

		private void StoreDeclarationDiagnostics(BindingDiagnosticBag diagBag)
		{
			if (diagBag.IsEmpty)
			{
				return;
			}
			bool? flag = diagBag.DiagnosticBag?.IsEmptyWithoutResolution;
			if (((!flag) ?? flag).GetValueOrDefault())
			{
				foreach (Diagnostic item in diagBag.DiagnosticBag!.AsEnumerableWithoutResolution())
				{
					Location location = item.Location;
					if (location.IsInSource)
					{
						VisualBasicSyntaxTree tree = (VisualBasicSyntaxTree)location.SourceTree;
						TryGetSourceFile(tree).DeclarationDiagnostics.Add(item);
					}
					else
					{
						_diagnosticBagDeclare.Add(item);
					}
				}
			}
			if (!diagBag.DependenciesBag.IsNullOrEmpty())
			{
				DeclaringCompilation.AddUsedAssemblies(diagBag.DependenciesBag);
			}
		}

		internal void RecordPresenceOfExtensionMethods()
		{
			_lazyContainsExtensionMethods = 2;
		}

		internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			if (attribute.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
			{
				((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
			}
			if (attribute.IsTargetAttribute(this, AttributeDescription.DefaultCharSetAttribute))
			{
				CharSet constructorArgument = attribute.GetConstructorArgument<CharSet>(0, SpecialType.System_Enum);
				if (!CommonModuleWellKnownAttributeData.IsValidCharSet(constructorArgument))
				{
					((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_BadAttribute1, arguments.AttributeSyntaxOpt!.ArgumentList.Arguments[0].GetLocation(), attribute.AttributeClass);
				}
				else
				{
					arguments.GetOrCreateData<CommonModuleWellKnownAttributeData>().DefaultCharacterSet = constructorArgument;
				}
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.DebuggableAttribute))
			{
				arguments.GetOrCreateData<CommonModuleWellKnownAttributeData>().HasDebuggableAttribute = true;
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		public SynthesizedMyGroupCollectionPropertySymbol GetMyGroupCollectionPropertyWithDefaultInstanceAlias(NamedTypeSymbol classType)
		{
			if (_lazyTypesWithDefaultInstanceAlias == null)
			{
				_lazyTypesWithDefaultInstanceAlias = GetTypesWithDefaultInstanceAlias();
			}
			SynthesizedMyGroupCollectionPropertySymbol value = null;
			if (_lazyTypesWithDefaultInstanceAlias != s_noTypesWithDefaultInstanceAlias && _lazyTypesWithDefaultInstanceAlias.TryGetValue(classType, out value))
			{
				return value;
			}
			return null;
		}

		private Dictionary<NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol> GetTypesWithDefaultInstanceAlias()
		{
			Dictionary<NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol> result = null;
			if (_assemblySymbol.DeclaringCompilation.MyTemplate != null)
			{
				GetTypesWithDefaultInstanceAlias(GlobalNamespace, ref result);
			}
			if (result == null)
			{
				result = s_noTypesWithDefaultInstanceAlias;
			}
			return result;
		}

		private static void GetTypesWithDefaultInstanceAlias(NamespaceOrTypeSymbol namespaceOrType, [In][Out] ref Dictionary<NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol> result)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = namespaceOrType.GetMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				switch (current.Kind)
				{
				case SymbolKind.Property:
				{
					if (!current.IsMyGroupCollectionProperty)
					{
						break;
					}
					SynthesizedMyGroupCollectionPropertySymbol synthesizedMyGroupCollectionPropertySymbol = (SynthesizedMyGroupCollectionPropertySymbol)current;
					if (synthesizedMyGroupCollectionPropertySymbol.DefaultInstanceAlias.Length > 0)
					{
						NamedTypeSymbol key = (NamedTypeSymbol)synthesizedMyGroupCollectionPropertySymbol.Type;
						if (result == null)
						{
							result = new Dictionary<NamedTypeSymbol, SynthesizedMyGroupCollectionPropertySymbol>(ReferenceEqualityComparer.Instance);
						}
						else if (result.ContainsKey(key))
						{
							result[key] = null;
							break;
						}
						result.Add(key, synthesizedMyGroupCollectionPropertySymbol);
					}
					break;
				}
				case SymbolKind.NamedType:
				{
					if (!(current is SourceNamedTypeSymbol sourceNamedTypeSymbol))
					{
						break;
					}
					ImmutableArray<SyntaxReference>.Enumerator enumerator2 = sourceNamedTypeSymbol.SyntaxReferences.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (VisualBasicExtensions.IsMyTemplate(enumerator2.Current.SyntaxTree))
						{
							GetTypesWithDefaultInstanceAlias(sourceNamedTypeSymbol, ref result);
							break;
						}
					}
					break;
				}
				case SymbolKind.Namespace:
					GetTypesWithDefaultInstanceAlias((NamespaceSymbol)current, ref result);
					break;
				}
			}
		}

		public override ModuleMetadata GetMetadata()
		{
			return null;
		}
	}
}
