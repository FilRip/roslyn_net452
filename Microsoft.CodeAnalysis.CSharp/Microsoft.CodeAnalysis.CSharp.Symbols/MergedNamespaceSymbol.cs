using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class MergedNamespaceSymbol : NamespaceSymbol
    {
        private readonly NamespaceExtent _extent;

        private readonly ImmutableArray<NamespaceSymbol> _namespacesToMerge;

        private readonly NamespaceSymbol _containingNamespace;

        private readonly string _nameOpt;

        private readonly CachingDictionary<string, Symbol> _cachedLookup;

        private ImmutableArray<Symbol> _allMembers;

        public override string Name => _nameOpt ?? _namespacesToMerge[0].Name;

        internal override NamespaceExtent Extent => _extent;

        public override ImmutableArray<NamespaceSymbol> ConstituentNamespaces => _namespacesToMerge;

        public override Symbol ContainingSymbol => _containingNamespace;

        public override AssemblySymbol ContainingAssembly
        {
            get
            {
                if (_extent.Kind == NamespaceKind.Module)
                {
                    return _extent.Module.ContainingAssembly;
                }
                if (_extent.Kind == NamespaceKind.Assembly)
                {
                    return _extent.Assembly;
                }
                return null;
            }
        }

        public override ImmutableArray<Location> Locations => _namespacesToMerge.SelectMany((NamespaceSymbol namespaceSymbol) => namespaceSymbol.Locations).AsImmutable();

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _namespacesToMerge.SelectMany((NamespaceSymbol namespaceSymbol) => namespaceSymbol.DeclaringSyntaxReferences).AsImmutable();

        internal static NamespaceSymbol Create(NamespaceExtent extent, NamespaceSymbol containingNamespace, ImmutableArray<NamespaceSymbol> namespacesToMerge, string nameOpt = null)
        {
            if (namespacesToMerge.Length != 1 || nameOpt != null)
            {
                return new MergedNamespaceSymbol(extent, containingNamespace, namespacesToMerge, nameOpt);
            }
            return namespacesToMerge[0];
        }

        private MergedNamespaceSymbol(NamespaceExtent extent, NamespaceSymbol containingNamespace, ImmutableArray<NamespaceSymbol> namespacesToMerge, string nameOpt)
        {
            _extent = extent;
            _namespacesToMerge = namespacesToMerge;
            _containingNamespace = containingNamespace;
            _cachedLookup = new CachingDictionary<string, Symbol>(SlowGetChildrenOfName, SlowGetChildNames, EqualityComparer<string>.Default);
            _nameOpt = nameOpt;
        }

        internal NamespaceSymbol GetConstituentForCompilation(CSharpCompilation compilation)
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

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamespaceSymbol current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                current.ForceComplete(locationOpt, cancellationToken);
            }
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
                instance.Add(Create(_extent, this, arrayBuilder.ToImmutableAndFree()));
            }
            return instance.ToImmutableAndFree();
        }

        private HashSet<string> SlowGetChildNames(IEqualityComparer<string> comparer)
        {
            HashSet<string> hashSet = new HashSet<string>(comparer);
            ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<Symbol>.Enumerator enumerator2 = enumerator.Current.GetMembersUnordered().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current = enumerator2.Current;
                    hashSet.Add(current.Name);
                }
            }
            return hashSet;
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            if (_allMembers.IsDefault)
            {
                ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
                _cachedLookup.AddValues(instance);
                _allMembers = instance.ToImmutableAndFree();
            }
            return _allMembers;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return _cachedLookup[name];
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
        {
            return ImmutableArray.CreateRange(GetMembersUnordered().OfType<NamedTypeSymbol>());
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray.CreateRange(GetMembers().OfType<NamedTypeSymbol>());
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray.CreateRange(_cachedLookup[name].OfType<NamedTypeSymbol>());
        }

        internal override void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, string name, int arity, LookupOptions options)
        {
            ImmutableArray<NamespaceSymbol>.Enumerator enumerator = _namespacesToMerge.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.GetExtensionMethods(methods, name, arity, options);
            }
        }
    }
}
