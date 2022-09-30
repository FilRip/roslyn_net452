using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class DeclarationTable
	{
		private class Cache
		{
			internal readonly Lazy<MergedNamespaceDeclaration> MergedRoot;

			internal readonly Lazy<ICollection<string>> TypeNames;

			internal readonly Lazy<ICollection<string>> NamespaceNames;

			internal readonly Lazy<ImmutableArray<ReferenceDirective>> ReferenceDirectives;

			public Cache(DeclarationTable table)
			{
				MergedRoot = new Lazy<MergedNamespaceDeclaration>(table.MergeOlderNamespaces);
				TypeNames = new Lazy<ICollection<string>>(() => GetTypeNames(MergedRoot.Value));
				NamespaceNames = new Lazy<ICollection<string>>(() => GetNamespaceNames(MergedRoot.Value));
				ReferenceDirectives = new Lazy<ImmutableArray<ReferenceDirective>>(() => table.SelectManyFromOlderDeclarationsNoEmbedded((RootSingleNamespaceDeclaration r) => r.ReferenceDirectives));
			}
		}

		private sealed class RootNamespaceLocationComparer : IComparer<SingleNamespaceDeclaration>
		{
			private readonly VisualBasicCompilation _compilation;

			internal RootNamespaceLocationComparer(VisualBasicCompilation compilation)
			{
				_compilation = compilation;
			}

			public int Compare(SingleNamespaceDeclaration x, SingleNamespaceDeclaration y)
			{
				return _compilation.CompareSourceLocations(x.SyntaxReference, y.SyntaxReference);
			}

			int IComparer<SingleNamespaceDeclaration>.Compare(SingleNamespaceDeclaration x, SingleNamespaceDeclaration y)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Compare
				return this.Compare(x, y);
			}
		}

		public static readonly DeclarationTable Empty = new DeclarationTable(ImmutableSetWithInsertionOrder<DeclarationTableEntry>.Empty, null, null);

		private readonly ImmutableSetWithInsertionOrder<DeclarationTableEntry> _allOlderRootDeclarations;

		private readonly DeclarationTableEntry _latestLazyRootDeclaration;

		private readonly Cache _cache;

		private MergedNamespaceDeclaration _mergedRoot;

		private readonly Lazy<ICollection<string>> _typeNames;

		private readonly Lazy<ICollection<string>> _namespaceNames;

		private readonly Lazy<ICollection<ReferenceDirective>> _referenceDirectives;

		private ImmutableArray<RootSingleNamespaceDeclaration> _lazyAllRootDeclarations;

		private static readonly Predicate<Declaration> s_isNamespacePredicate = (Declaration d) => d.Kind == DeclarationKind.Namespace;

		private static readonly Predicate<Declaration> s_isTypePredicate = (Declaration d) => d.Kind != DeclarationKind.Namespace;

		public ICollection<string> TypeNames => _typeNames.Value;

		public ICollection<string> NamespaceNames => _namespaceNames.Value;

		public ICollection<ReferenceDirective> ReferenceDirectives => _referenceDirectives.Value;

		private DeclarationTable(ImmutableSetWithInsertionOrder<DeclarationTableEntry> allOlderRootDeclarations, DeclarationTableEntry latestLazyRootDeclaration, Cache cache)
		{
			_allOlderRootDeclarations = allOlderRootDeclarations;
			_latestLazyRootDeclaration = latestLazyRootDeclaration;
			_cache = cache ?? new Cache(this);
			_typeNames = new Lazy<ICollection<string>>(GetMergedTypeNames);
			_namespaceNames = new Lazy<ICollection<string>>(GetMergedNamespaceNames);
			_referenceDirectives = new Lazy<ICollection<ReferenceDirective>>(GetMergedReferenceDirectives);
		}

		public DeclarationTable AddRootDeclaration(DeclarationTableEntry lazyRootDeclaration)
		{
			if (_latestLazyRootDeclaration == null)
			{
				return new DeclarationTable(_allOlderRootDeclarations, lazyRootDeclaration, _cache);
			}
			return new DeclarationTable(_allOlderRootDeclarations.Add(_latestLazyRootDeclaration), lazyRootDeclaration, null);
		}

		public bool Contains(DeclarationTableEntry rootDeclaration)
		{
			if (rootDeclaration != null)
			{
				if (!_allOlderRootDeclarations.Contains(rootDeclaration))
				{
					return _latestLazyRootDeclaration == rootDeclaration;
				}
				return true;
			}
			return false;
		}

		public DeclarationTable RemoveRootDeclaration(DeclarationTableEntry lazyRootDeclaration)
		{
			if (_latestLazyRootDeclaration == lazyRootDeclaration)
			{
				return new DeclarationTable(_allOlderRootDeclarations, null, _cache);
			}
			return new DeclarationTable(_allOlderRootDeclarations.Remove(lazyRootDeclaration), _latestLazyRootDeclaration, null);
		}

		public ImmutableArray<RootSingleNamespaceDeclaration> AllRootNamespaces()
		{
			if (_lazyAllRootDeclarations.IsDefault)
			{
				ArrayBuilder<RootSingleNamespaceDeclaration> instance = ArrayBuilder<RootSingleNamespaceDeclaration>.GetInstance();
				GetOlderNamespaces(instance);
				RootSingleNamespaceDeclaration latestRootDeclarationIfAny = GetLatestRootDeclarationIfAny(includeEmbedded: true);
				if (latestRootDeclarationIfAny != null)
				{
					instance.Add(latestRootDeclarationIfAny);
				}
				ImmutableInterlocked.InterlockedInitialize(ref _lazyAllRootDeclarations, instance.ToImmutableAndFree());
			}
			return _lazyAllRootDeclarations;
		}

		private void GetOlderNamespaces(ArrayBuilder<RootSingleNamespaceDeclaration> builder)
		{
			foreach (DeclarationTableEntry item in _allOlderRootDeclarations.InInsertionOrder)
			{
				RootSingleNamespaceDeclaration value = item.Root.Value;
				if (value != null)
				{
					builder.Add(value);
				}
			}
		}

		private MergedNamespaceDeclaration MergeOlderNamespaces()
		{
			ArrayBuilder<RootSingleNamespaceDeclaration> instance = ArrayBuilder<RootSingleNamespaceDeclaration>.GetInstance();
			GetOlderNamespaces(instance);
			MergedNamespaceDeclaration result = MergedNamespaceDeclaration.Create(instance);
			instance.Free();
			return result;
		}

		private ImmutableArray<T> SelectManyFromOlderDeclarationsNoEmbedded<T>(Func<RootSingleNamespaceDeclaration, ImmutableArray<T>> selector)
		{
			return _allOlderRootDeclarations.InInsertionOrder.Where((DeclarationTableEntry d) => !d.IsEmbedded && d.Root.Value != null).SelectMany((DeclarationTableEntry d) => selector(d.Root.Value)).AsImmutable();
		}

		public MergedNamespaceDeclaration GetMergedRoot(VisualBasicCompilation compilation)
		{
			if (_mergedRoot == null)
			{
				Interlocked.CompareExchange(ref _mergedRoot, CalculateMergedRoot(compilation), null);
			}
			return _mergedRoot;
		}

		internal MergedNamespaceDeclaration CalculateMergedRoot(VisualBasicCompilation compilation)
		{
			MergedNamespaceDeclaration value = _cache.MergedRoot.Value;
			RootSingleNamespaceDeclaration latestRootDeclarationIfAny = GetLatestRootDeclarationIfAny(includeEmbedded: true);
			if (latestRootDeclarationIfAny == null)
			{
				return value;
			}
			if (value == null)
			{
				return MergedNamespaceDeclaration.Create(latestRootDeclarationIfAny);
			}
			ImmutableArray<SingleNamespaceDeclaration> declarations = value.Declarations;
			ArrayBuilder<SingleNamespaceDeclaration> instance = ArrayBuilder<SingleNamespaceDeclaration>.GetInstance(declarations.Length + 1);
			instance.AddRange(declarations);
			instance.Add(_latestLazyRootDeclaration.Root.Value);
			if (compilation != null)
			{
				instance.Sort(new RootNamespaceLocationComparer(compilation));
			}
			return MergedNamespaceDeclaration.Create(instance.ToImmutableAndFree());
		}

		private ICollection<string> GetMergedTypeNames()
		{
			ICollection<string> value = _cache.TypeNames.Value;
			RootSingleNamespaceDeclaration latestRootDeclarationIfAny = GetLatestRootDeclarationIfAny(includeEmbedded: true);
			if (latestRootDeclarationIfAny == null)
			{
				return value;
			}
			return UnionCollection<string>.Create(value, GetTypeNames(latestRootDeclarationIfAny));
		}

		private ICollection<string> GetMergedNamespaceNames()
		{
			ICollection<string> value = _cache.NamespaceNames.Value;
			RootSingleNamespaceDeclaration latestRootDeclarationIfAny = GetLatestRootDeclarationIfAny(includeEmbedded: true);
			if (latestRootDeclarationIfAny == null)
			{
				return value;
			}
			return UnionCollection<string>.Create(value, GetNamespaceNames(latestRootDeclarationIfAny));
		}

		private ICollection<ReferenceDirective> GetMergedReferenceDirectives()
		{
			ImmutableArray<ReferenceDirective> value = _cache.ReferenceDirectives.Value;
			RootSingleNamespaceDeclaration latestRootDeclarationIfAny = GetLatestRootDeclarationIfAny(includeEmbedded: false);
			if (latestRootDeclarationIfAny == null)
			{
				return value;
			}
			return UnionCollection<ReferenceDirective>.Create(value, latestRootDeclarationIfAny.ReferenceDirectives);
		}

		private RootSingleNamespaceDeclaration GetLatestRootDeclarationIfAny(bool includeEmbedded)
		{
			if (_latestLazyRootDeclaration == null || (!includeEmbedded && _latestLazyRootDeclaration.IsEmbedded))
			{
				return null;
			}
			return _latestLazyRootDeclaration.Root.Value;
		}

		private static ICollection<string> GetTypeNames(Declaration declaration)
		{
			return GetNames(declaration, s_isTypePredicate);
		}

		private static ICollection<string> GetNamespaceNames(Declaration declaration)
		{
			return GetNames(declaration, s_isNamespacePredicate);
		}

		private static ICollection<string> GetNames(Declaration declaration, Predicate<Declaration> predicate)
		{
			IdentifierCollection identifierCollection = new IdentifierCollection();
			Stack<Declaration> stack = new Stack<Declaration>();
			stack.Push(declaration);
			while (stack.Count > 0)
			{
				Declaration declaration2 = stack.Pop();
				if (declaration2 != null)
				{
					if (predicate(declaration2))
					{
						identifierCollection.AddIdentifier(declaration2.Name);
					}
					ImmutableArray<Declaration>.Enumerator enumerator = declaration2.Children.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Declaration current = enumerator.Current;
						stack.Push(current);
					}
				}
			}
			return identifierCollection.AsCaseInsensitiveCollection();
		}

		public static bool ContainsName(MergedNamespaceDeclaration mergedRoot, string name, SymbolFilter filter, CancellationToken cancellationToken)
		{
			return ContainsNameHelper(mergedRoot, (string n) => CaseInsensitiveComparison.Equals(n, name), filter, (SingleTypeDeclaration t) => t.MemberNames.Contains(name), cancellationToken);
		}

		public static bool ContainsName(MergedNamespaceDeclaration mergedRoot, Func<string, bool> predicate, SymbolFilter filter, CancellationToken cancellationToken)
		{
			_Closure_0024__38_002D0 arg = default(_Closure_0024__38_002D0);
			_Closure_0024__38_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__38_002D0(arg);
			CS_0024_003C_003E8__locals0._0024VB_0024Local_predicate = predicate;
			return ContainsNameHelper(mergedRoot, CS_0024_003C_003E8__locals0._0024VB_0024Local_predicate, filter, delegate(SingleTypeDeclaration t)
			{
				foreach (string memberName in t.MemberNames)
				{
					if (CS_0024_003C_003E8__locals0._0024VB_0024Local_predicate(memberName))
					{
						return true;
					}
				}
				return false;
			}, cancellationToken);
		}

		public static bool ContainsNameHelper(MergedNamespaceDeclaration mergedRoot, Func<string, bool> predicate, SymbolFilter filter, Func<SingleTypeDeclaration, bool> typePredicate, CancellationToken cancellationToken)
		{
			bool flag = (filter & SymbolFilter.Namespace) == SymbolFilter.Namespace;
			bool flag2 = (filter & SymbolFilter.Type) == SymbolFilter.Type;
			bool flag3 = (filter & SymbolFilter.Member) == SymbolFilter.Member;
			Stack<MergedNamespaceOrTypeDeclaration> stack = new Stack<MergedNamespaceOrTypeDeclaration>();
			stack.Push(mergedRoot);
			while (stack.Count > 0)
			{
				cancellationToken.ThrowIfCancellationRequested();
				MergedNamespaceOrTypeDeclaration mergedNamespaceOrTypeDeclaration = stack.Pop();
				if (mergedNamespaceOrTypeDeclaration == null)
				{
					continue;
				}
				if (mergedNamespaceOrTypeDeclaration.Kind == DeclarationKind.Namespace)
				{
					if (flag && predicate(mergedNamespaceOrTypeDeclaration.Name))
					{
						return true;
					}
				}
				else
				{
					if (flag2 && predicate(mergedNamespaceOrTypeDeclaration.Name))
					{
						return true;
					}
					if (flag3)
					{
						ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = ((MergedTypeDeclaration)mergedNamespaceOrTypeDeclaration).Declarations.GetEnumerator();
						while (enumerator.MoveNext())
						{
							SingleTypeDeclaration current = enumerator.Current;
							if (typePredicate(current))
							{
								return true;
							}
						}
					}
				}
				ImmutableArray<Declaration>.Enumerator enumerator2 = mergedNamespaceOrTypeDeclaration.Children.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					MergedNamespaceOrTypeDeclaration mergedNamespaceOrTypeDeclaration2 = (MergedNamespaceOrTypeDeclaration)enumerator2.Current;
					if (flag3 || flag2 || mergedNamespaceOrTypeDeclaration2.Kind == DeclarationKind.Namespace)
					{
						stack.Push(mergedNamespaceOrTypeDeclaration2);
					}
				}
			}
			return false;
		}
	}
}
