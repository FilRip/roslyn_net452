using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class MergedNamespaceSymbol : PEOrSourceOrMergedNamespaceSymbol
	{
		private sealed class AssemblyMergedNamespaceSymbol : MergedNamespaceSymbol
		{
			private readonly AssemblySymbol _assembly;

			internal override NamespaceExtent Extent => new NamespaceExtent(_assembly);

			internal override ThreeState RawContainsAccessibleTypes
			{
				get
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			internal override ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods
			{
				get
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			public AssemblyMergedNamespaceSymbol(AssemblySymbol assembly, AssemblyMergedNamespaceSymbol containingNamespace, ImmutableArray<NamespaceSymbol> namespacesToMerge)
				: base(containingNamespace, namespacesToMerge)
			{
				_assembly = assembly;
			}

			protected override NamespaceSymbol CreateChildMergedNamespaceSymbol(ImmutableArray<NamespaceSymbol> nsSymbols)
			{
				return Create(_assembly, this, nsSymbols);
			}

			internal override void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class CompilationMergedNamespaceSymbol : MergedNamespaceSymbol
		{
			private readonly VisualBasicCompilation _compilation;

			private ThreeState _containsAccessibleTypes;

			private ThreeState _isDeclaredInSourceModule;

			internal override ThreeState RawContainsAccessibleTypes => _containsAccessibleTypes;

			internal override NamespaceExtent Extent => new NamespaceExtent(_compilation);

			internal override ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods
			{
				get
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			public CompilationMergedNamespaceSymbol(VisualBasicCompilation compilation, CompilationMergedNamespaceSymbol containingNamespace, ImmutableArray<NamespaceSymbol> namespacesToMerge)
				: base(containingNamespace, namespacesToMerge)
			{
				_containsAccessibleTypes = ThreeState.Unknown;
				_isDeclaredInSourceModule = ThreeState.Unknown;
				_compilation = compilation;
			}

			protected override NamespaceSymbol CreateChildMergedNamespaceSymbol(ImmutableArray<NamespaceSymbol> nsSymbols)
			{
				return Create(_compilation, this, nsSymbols);
			}

			internal override bool ContainsTypesAccessibleFrom(AssemblySymbol fromAssembly)
			{
				if (_containsAccessibleTypes.HasValue())
				{
					return _containsAccessibleTypes == ThreeState.True;
				}
				if (base.RawLazyDeclaredAccessibilityOfMostAccessibleDescendantType == Accessibility.Public)
				{
					return true;
				}
				bool flag = false;
				ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ContainsTypesAccessibleFrom(fromAssembly))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					_containsAccessibleTypes = ThreeState.True;
					CompilationMergedNamespaceSymbol compilationMergedNamespaceSymbol = ContainingSymbol as CompilationMergedNamespaceSymbol;
					while ((object)compilationMergedNamespaceSymbol != null && compilationMergedNamespaceSymbol._containsAccessibleTypes == ThreeState.Unknown)
					{
						compilationMergedNamespaceSymbol._containsAccessibleTypes = ThreeState.True;
						compilationMergedNamespaceSymbol = compilationMergedNamespaceSymbol.ContainingSymbol as CompilationMergedNamespaceSymbol;
					}
				}
				else
				{
					_containsAccessibleTypes = ThreeState.False;
				}
				return flag;
			}

			internal override bool IsDeclaredInSourceModule(ModuleSymbol module)
			{
				if (_isDeclaredInSourceModule.HasValue())
				{
					return _isDeclaredInSourceModule == ThreeState.True;
				}
				bool flag = base.IsDeclaredInSourceModule(module);
				if (flag)
				{
					_isDeclaredInSourceModule = ThreeState.True;
					CompilationMergedNamespaceSymbol compilationMergedNamespaceSymbol = ContainingSymbol as CompilationMergedNamespaceSymbol;
					while ((object)compilationMergedNamespaceSymbol != null && compilationMergedNamespaceSymbol._isDeclaredInSourceModule == ThreeState.Unknown)
					{
						compilationMergedNamespaceSymbol._isDeclaredInSourceModule = ThreeState.True;
						compilationMergedNamespaceSymbol = compilationMergedNamespaceSymbol.ContainingSymbol as CompilationMergedNamespaceSymbol;
					}
				}
				else
				{
					_isDeclaredInSourceModule = ThreeState.False;
				}
				return flag;
			}

			internal override void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map)
			{
				ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.BuildExtensionMethodsMap(map);
				}
			}

			internal override void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, string name)
			{
				ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.GetExtensionMethods(methods, name);
				}
			}
		}

		private sealed class NamespaceGroupSymbol : MergedNamespaceSymbol
		{
			public static readonly NamespaceGroupSymbol GlobalNamespace = new NamespaceGroupSymbol(null, ImmutableArray<NamespaceSymbol>.Empty);

			public override string Name
			{
				get
				{
					if (_namespacesToMerge.Length <= 0)
					{
						return "";
					}
					return _namespacesToMerge[0].Name;
				}
			}

			internal override NamespaceExtent Extent => default(NamespaceExtent);

			internal override ThreeState RawContainsAccessibleTypes
			{
				get
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			internal override ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods
			{
				get
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			public NamespaceGroupSymbol(NamespaceGroupSymbol containingNamespace, ImmutableArray<NamespaceSymbol> namespacesToMerge)
				: base(containingNamespace, namespacesToMerge)
			{
			}

			internal override void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map)
			{
				throw ExceptionUtilities.Unreachable;
			}

			protected override NamespaceSymbol CreateChildMergedNamespaceSymbol(ImmutableArray<NamespaceSymbol> nsSymbols)
			{
				return Create(this, nsSymbols);
			}

			public override NamespaceSymbol Shrink(IEnumerable<NamespaceSymbol> namespacesToMerge)
			{
				ArrayBuilder<NamespaceSymbol> instance = ArrayBuilder<NamespaceSymbol>.GetInstance();
				instance.AddRange(namespacesToMerge);
				if (instance.Count == 0)
				{
					instance.Free();
					return this;
				}
				if (instance.Count == 1)
				{
					NamespaceSymbol namespaceSymbol = instance[0];
					instance.Free();
					if (_namespacesToMerge.Contains(namespaceSymbol))
					{
						return namespaceSymbol;
					}
					throw ExceptionUtilities.Unreachable;
				}
				SmallDictionary<NamespaceSymbol, bool> smallDictionary = new SmallDictionary<NamespaceSymbol, bool>();
				ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamespaceSymbol current = enumerator.Current;
					smallDictionary[current] = false;
				}
				ArrayBuilder<NamespaceSymbol>.Enumerator enumerator2 = instance.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					NamespaceSymbol current2 = enumerator2.Current;
					bool value = false;
					if (!smallDictionary.TryGetValue(current2, out value))
					{
						instance.Free();
						return this;
					}
				}
				return Shrink(instance.ToImmutableAndFree());
			}

			private NamespaceGroupSymbol Shrink(ImmutableArray<NamespaceSymbol> namespaceArray)
			{
				if (namespaceArray.Length >= _namespacesToMerge.Length)
				{
					return this;
				}
				if ((object)_containingNamespace == GlobalNamespace)
				{
					return new NamespaceGroupSymbol(GlobalNamespace, namespaceArray);
				}
				ArrayBuilder<NamespaceSymbol> instance = ArrayBuilder<NamespaceSymbol>.GetInstance(namespaceArray.Length);
				ImmutableArray<NamespaceSymbol>.Enumerator enumerator = namespaceArray.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamespaceSymbol current = enumerator.Current;
					instance.Add(current.ContainingNamespace);
				}
				return new NamespaceGroupSymbol(((NamespaceGroupSymbol)_containingNamespace).Shrink(instance.ToImmutableAndFree()), namespaceArray);
			}
		}

		protected readonly ImmutableArray<NamespaceSymbol> _namespacesToMerge;

		protected readonly MergedNamespaceSymbol _containingNamespace;

		private readonly CachingDictionary<string, Symbol> _cachedLookup;

		private ImmutableArray<NamedTypeSymbol> _lazyModuleMembers;

		private ImmutableArray<Symbol> _lazyMembers;

		private int _lazyEmbeddedKind;

		public override ImmutableArray<NamespaceSymbol> ConstituentNamespaces => _namespacesToMerge;

		public override string Name => _namespacesToMerge[0].Name;

		internal override EmbeddedSymbolKind EmbeddedSymbolKind
		{
			get
			{
				if (_lazyEmbeddedKind == 1)
				{
					int num = 0;
					ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
					while (enumerator.MoveNext())
					{
						NamespaceSymbol current = enumerator.Current;
						num |= (int)current.EmbeddedSymbolKind;
					}
					Interlocked.CompareExchange(ref _lazyEmbeddedKind, num, 1);
				}
				return (EmbeddedSymbolKind)_lazyEmbeddedKind;
			}
		}

		public override Symbol ContainingSymbol => _containingNamespace;

		public override AssemblySymbol ContainingAssembly
		{
			get
			{
				if (Extent.Kind == NamespaceKind.Module)
				{
					return Extent.Module.ContainingAssembly;
				}
				if (Extent.Kind == NamespaceKind.Assembly)
				{
					return Extent.Assembly;
				}
				return null;
			}
		}

		public override ImmutableArray<Location> Locations => ImmutableArray.CreateRange(from ns in _namespacesToMerge
			from loc in ns.Locations
			select loc);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.CreateRange(from ns in _namespacesToMerge
			from reference in ns.DeclaringSyntaxReferences
			select reference);

		internal abstract ThreeState RawContainsAccessibleTypes { get; }

		public static NamespaceSymbol CreateGlobalNamespace(AssemblySymbol extent)
		{
			return Create(extent, null, ConstituentGlobalNamespaces(extent).AsImmutable());
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_7_ConstituentGlobalNamespaces))]
		private static IEnumerable<NamespaceSymbol> ConstituentGlobalNamespaces(AssemblySymbol extent)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_7_ConstituentGlobalNamespaces(-2)
			{
				_0024P_extent = extent
			};
		}

		private static NamespaceSymbol Create(AssemblySymbol extent, AssemblyMergedNamespaceSymbol containingNamespace, ImmutableArray<NamespaceSymbol> namespacesToMerge)
		{
			if (namespacesToMerge.Length == 1)
			{
				return namespacesToMerge[0];
			}
			return new AssemblyMergedNamespaceSymbol(extent, containingNamespace, namespacesToMerge);
		}

		internal static NamespaceSymbol CreateForTestPurposes(AssemblySymbol extent, IEnumerable<NamespaceSymbol> namespacesToMerge)
		{
			return Create(extent, null, namespacesToMerge.AsImmutable());
		}

		public static NamespaceSymbol CreateGlobalNamespace(VisualBasicCompilation extent)
		{
			return Create(extent, null, ConstituentGlobalNamespaces(extent));
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_11_ConstituentGlobalNamespaces))]
		private static IEnumerable<NamespaceSymbol> ConstituentGlobalNamespaces(VisualBasicCompilation extent)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_11_ConstituentGlobalNamespaces(-2)
			{
				_0024P_extent = extent
			};
		}

		private static NamespaceSymbol Create(VisualBasicCompilation extent, CompilationMergedNamespaceSymbol containingNamespace, IEnumerable<NamespaceSymbol> namespacesToMerge)
		{
			ArrayBuilder<NamespaceSymbol> instance = ArrayBuilder<NamespaceSymbol>.GetInstance();
			instance.AddRange(namespacesToMerge);
			if (instance.Count == 1)
			{
				NamespaceSymbol result = instance[0];
				instance.Free();
				return result;
			}
			return new CompilationMergedNamespaceSymbol(extent, containingNamespace, instance.ToImmutableAndFree());
		}

		public static NamespaceSymbol CreateNamespaceGroup(IEnumerable<NamespaceSymbol> namespacesToMerge)
		{
			return Create(NamespaceGroupSymbol.GlobalNamespace, namespacesToMerge);
		}

		public virtual NamespaceSymbol Shrink(IEnumerable<NamespaceSymbol> namespacesToMerge)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private static NamespaceSymbol Create(NamespaceGroupSymbol containingNamespace, IEnumerable<NamespaceSymbol> namespacesToMerge)
		{
			ArrayBuilder<NamespaceSymbol> instance = ArrayBuilder<NamespaceSymbol>.GetInstance();
			instance.AddRange(namespacesToMerge);
			if (instance.Count == 1)
			{
				NamespaceSymbol result = instance[0];
				instance.Free();
				return result;
			}
			return new NamespaceGroupSymbol(containingNamespace, instance.ToImmutableAndFree());
		}

		private MergedNamespaceSymbol(MergedNamespaceSymbol containingNamespace, ImmutableArray<NamespaceSymbol> namespacesToMerge)
		{
			_lazyEmbeddedKind = 1;
			_namespacesToMerge = namespacesToMerge;
			_containingNamespace = containingNamespace;
			_cachedLookup = new CachingDictionary<string, Symbol>(SlowGetChildrenOfName, SlowGetChildNames, CaseInsensitiveComparison.Comparer);
		}

		internal NamespaceSymbol GetConstituentForCompilation(VisualBasicCompilation compilation)
		{
			ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamespaceSymbol current = enumerator.Current;
				if (current.IsFromCompilation(compilation))
				{
					return current;
				}
			}
			return null;
		}

		private ImmutableArray<Symbol> SlowGetChildrenOfName(string name)
		{
			ArrayBuilder<NamespaceSymbol> arrayBuilder = null;
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<Symbol>.Enumerator enumerator2 = enumerator.Current.GetMembers(name).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current = enumerator2.Current;
					if (current.Kind == SymbolKind.Namespace)
					{
						arrayBuilder = arrayBuilder ?? ArrayBuilder<NamespaceSymbol>.GetInstance();
						arrayBuilder.Add((NamespaceSymbol)current);
					}
					else
					{
						instance.Add(current);
					}
				}
			}
			if (arrayBuilder != null)
			{
				instance.Add(CreateChildMergedNamespaceSymbol(arrayBuilder.ToImmutableAndFree()));
			}
			return instance.ToImmutableAndFree();
		}

		protected abstract NamespaceSymbol CreateChildMergedNamespaceSymbol(ImmutableArray<NamespaceSymbol> nsSymbols);

		private HashSet<string> SlowGetChildNames(IEqualityComparer<string> comparer)
		{
			HashSet<string> hashSet = new HashSet<string>(comparer);
			ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<Symbol>.Enumerator enumerator2 = enumerator.Current.GetMembersUnordered().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					NamespaceOrTypeSymbol namespaceOrTypeSymbol = (NamespaceOrTypeSymbol)enumerator2.Current;
					hashSet.Add(namespaceOrTypeSymbol.Name);
				}
			}
			return hashSet;
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers()
		{
			if (_lazyModuleMembers.IsDefault)
			{
				ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
				ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamespaceSymbol current = enumerator.Current;
					instance.AddRange(current.GetModuleMembers());
				}
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyModuleMembers, instance.ToImmutableAndFree(), default(ImmutableArray<NamedTypeSymbol>));
			}
			return _lazyModuleMembers;
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			if (_lazyMembers.IsDefault)
			{
				ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
				_cachedLookup.AddValues(instance);
				_lazyMembers = instance.ToImmutableAndFree();
			}
			return _lazyMembers;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return _cachedLookup[name];
		}

		internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			return ImmutableArray.CreateRange(GetMembersUnordered().OfType<NamedTypeSymbol>());
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return ImmutableArray.CreateRange(GetMembers().OfType<NamedTypeSymbol>());
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return ImmutableArray.CreateRange(GetMembers(name).OfType<NamedTypeSymbol>());
		}

		protected sealed override Accessibility GetDeclaredAccessibilityOfMostAccessibleDescendantType()
		{
			Accessibility accessibility = Accessibility.NotApplicable;
			ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Accessibility declaredAccessibilityOfMostAccessibleDescendantType = enumerator.Current.DeclaredAccessibilityOfMostAccessibleDescendantType;
				if (declaredAccessibilityOfMostAccessibleDescendantType > accessibility)
				{
					if (declaredAccessibilityOfMostAccessibleDescendantType == Accessibility.Public)
					{
						return Accessibility.Public;
					}
					accessibility = declaredAccessibilityOfMostAccessibleDescendantType;
				}
			}
			return accessibility;
		}

		internal override bool IsDeclaredInSourceModule(ModuleSymbol module)
		{
			ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsDeclaredInSourceModule(module))
				{
					return true;
				}
			}
			return false;
		}
	}
}
