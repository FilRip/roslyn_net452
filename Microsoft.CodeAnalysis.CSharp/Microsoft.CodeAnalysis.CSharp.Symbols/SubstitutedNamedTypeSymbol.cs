using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SubstitutedNamedTypeSymbol : WrappedNamedTypeSymbol
    {
        private static readonly Func<Symbol, NamedTypeSymbol, Symbol> s_symbolAsMemberFunc = SymbolExtensions.SymbolAsMember;

        private readonly bool _unbound;

        private readonly TypeMap _inputMap;

        private readonly Symbol _newContainer;

        private TypeMap _lazyMap;

        private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

        private int _hashCode;

        private ConcurrentCache<string, ImmutableArray<Symbol>> _lazyMembersByNameCache;

        public sealed override bool IsUnboundGenericType => _unbound;

        private TypeMap Map
        {
            get
            {
                EnsureMapAndTypeParameters();
                return _lazyMap;
            }
        }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get
            {
                EnsureMapAndTypeParameters();
                return _lazyTypeParameters;
            }
        }

        public sealed override Symbol ContainingSymbol => _newContainer;

        public override NamedTypeSymbol ContainingType => _newContainer as NamedTypeSymbol;

        public sealed override SymbolKind Kind => OriginalDefinition.Kind;

        public sealed override NamedTypeSymbol OriginalDefinition => _underlyingType;

        internal sealed override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
        {
            get
            {
                if (!_unbound)
                {
                    return Map.SubstituteNamedType(OriginalDefinition.BaseTypeNoUseSiteDiagnostics);
                }
                return null;
            }
        }

        public sealed override IEnumerable<string> MemberNames
        {
            get
            {
                if (_unbound)
                {
                    return new List<string>((from s in GetTypeMembersUnordered()
                                             select s.Name).Distinct());
                }
                if (IsTupleType)
                {
                    return (from s in GetMembers()
                            select s.Name).Distinct();
                }
                return OriginalDefinition.MemberNames;
            }
        }

        public sealed override NamedTypeSymbol EnumUnderlyingType => OriginalDefinition.EnumUnderlyingType;

        internal sealed override TypeMap TypeSubstitution => Map;

        internal sealed override bool IsComImport => OriginalDefinition.IsComImport;

        internal sealed override NamedTypeSymbol ComImportCoClass => OriginalDefinition.ComImportCoClass;

        internal sealed override NamedTypeSymbol NativeIntegerUnderlyingType => null;

        internal sealed override bool IsRecord => _underlyingType.IsRecord;

        internal sealed override bool IsRecordStruct => _underlyingType.IsRecordStruct;

        protected SubstitutedNamedTypeSymbol(Symbol newContainer, TypeMap map, NamedTypeSymbol originalDefinition, NamedTypeSymbol constructedFrom = null, bool unbound = false, TupleExtraData tupleData = null)
            : base(originalDefinition, tupleData)
        {
            _newContainer = newContainer;
            _inputMap = map;
            _unbound = unbound;
            if ((object)constructedFrom != null)
            {
                _lazyTypeParameters = constructedFrom.TypeParameters;
                _lazyMap = map;
            }
        }

        private void EnsureMapAndTypeParameters()
        {
            if (_lazyTypeParameters.IsDefault)
            {
                TypeMap value = _inputMap.WithAlphaRename(OriginalDefinition, this, out ImmutableArray<TypeParameterSymbol> newTypeParameters);
                TypeMap typeMap = Interlocked.CompareExchange(ref _lazyMap, value, null);
                if (typeMap != null)
                {
                    newTypeParameters = typeMap.SubstituteTypeParameters(OriginalDefinition.TypeParameters);
                }
                ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeParameters, newTypeParameters, default(ImmutableArray<TypeParameterSymbol>));
            }
        }

        internal sealed override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
        {
            if (!_unbound)
            {
                return Map.SubstituteNamedType(OriginalDefinition.GetDeclaredBaseType(basesBeingResolved));
            }
            return null;
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
        {
            if (!_unbound)
            {
                return Map.SubstituteNamedTypes(OriginalDefinition.GetDeclaredInterfaces(basesBeingResolved));
            }
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
        {
            if (!_unbound)
            {
                return Map.SubstituteNamedTypes(OriginalDefinition.InterfacesNoUseSiteDiagnostics(basesBeingResolved));
            }
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal abstract override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes);

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return OriginalDefinition.GetAttributes();
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
        {
            return OriginalDefinition.GetTypeMembersUnordered().SelectAsArray((NamedTypeSymbol t, SubstitutedNamedTypeSymbol self) => t.AsMember(self), this);
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return OriginalDefinition.GetTypeMembers().SelectAsArray((NamedTypeSymbol t, SubstitutedNamedTypeSymbol self) => t.AsMember(self), this);
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return OriginalDefinition.GetTypeMembers(name).SelectAsArray((NamedTypeSymbol t, SubstitutedNamedTypeSymbol self) => t.AsMember(self), this);
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return OriginalDefinition.GetTypeMembers(name, arity).SelectAsArray((NamedTypeSymbol t, SubstitutedNamedTypeSymbol self) => t.AsMember(self), this);
        }

        public sealed override ImmutableArray<Symbol> GetMembers()
        {
            ArrayBuilder<Symbol> arrayBuilder = ArrayBuilder<Symbol>.GetInstance();
            if (_unbound)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = OriginalDefinition.GetMembers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind == SymbolKind.NamedType)
                    {
                        arrayBuilder.Add(((NamedTypeSymbol)current).AsMember(this));
                    }
                }
            }
            else
            {
                ImmutableArray<Symbol>.Enumerator enumerator = OriginalDefinition.GetMembers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current2 = enumerator.Current;
                    arrayBuilder.Add(current2.SymbolAsMember(this));
                }
            }
            if (IsTupleType)
            {
                arrayBuilder = AddOrWrapTupleMembers(arrayBuilder.ToImmutableAndFree());
            }
            return arrayBuilder.ToImmutableAndFree();
        }

        internal sealed override ImmutableArray<Symbol> GetMembersUnordered()
        {
            ArrayBuilder<Symbol> arrayBuilder = ArrayBuilder<Symbol>.GetInstance();
            if (_unbound)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = OriginalDefinition.GetMembersUnordered().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind == SymbolKind.NamedType)
                    {
                        arrayBuilder.Add(((NamedTypeSymbol)current).AsMember(this));
                    }
                }
            }
            else
            {
                ImmutableArray<Symbol>.Enumerator enumerator = OriginalDefinition.GetMembersUnordered().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current2 = enumerator.Current;
                    arrayBuilder.Add(current2.SymbolAsMember(this));
                }
            }
            if (IsTupleType)
            {
                arrayBuilder = AddOrWrapTupleMembers(arrayBuilder.ToImmutableAndFree());
            }
            return arrayBuilder.ToImmutableAndFree();
        }

        public sealed override ImmutableArray<Symbol> GetMembers(string name)
        {
            if (_unbound)
            {
                return StaticCast<Symbol>.From(GetTypeMembers(name));
            }
            ConcurrentCache<string, ImmutableArray<Symbol>> lazyMembersByNameCache = _lazyMembersByNameCache;
            if (lazyMembersByNameCache != null && lazyMembersByNameCache.TryGetValue(name, out var value))
            {
                return value;
            }
            return GetMembersWorker(name);
        }

        private ImmutableArray<Symbol> GetMembersWorker(string name)
        {
            if (IsTupleType)
            {
                ImmutableArray<Symbol> result2 = GetMembers().WhereAsArray((Symbol m, string name) => m.Name == name, name);
                cacheResult(result2);
                return result2;
            }
            ImmutableArray<Symbol> members = OriginalDefinition.GetMembers(name);
            if (members.IsDefaultOrEmpty)
            {
                return members;
            }
            ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(members.Length);
            ImmutableArray<Symbol>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                instance.Add(current.SymbolAsMember(this));
            }
            ImmutableArray<Symbol> result3 = instance.ToImmutableAndFree();
            cacheResult(result3);
            return result3;
            void cacheResult(ImmutableArray<Symbol> result)
            {
                (_lazyMembersByNameCache ?? (_lazyMembersByNameCache = new ConcurrentCache<string, ImmutableArray<Symbol>>(8))).TryAdd(name, result);
            }
        }

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
        {
            if (!_unbound)
            {
                return OriginalDefinition.GetEarlyAttributeDecodingMembers().SelectAsArray(s_symbolAsMemberFunc, this);
            }
            return GetMembers();
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
        {
            if (_unbound)
            {
                return GetMembers(name);
            }
            ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
            ImmutableArray<Symbol>.Enumerator enumerator = OriginalDefinition.GetEarlyAttributeDecodingMembers(name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                instance.Add(current.SymbolAsMember(this));
            }
            return instance.ToImmutableAndFree();
        }

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                _hashCode = this.ComputeHashCode();
            }
            return _hashCode;
        }

        internal override IEnumerable<MethodSymbol> GetMethodsToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override IEnumerable<EventSymbol> GetEventsToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override IEnumerable<PropertySymbol> GetPropertiesToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override NamedTypeSymbol AsNativeInteger()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override bool HasPossibleWellKnownCloneMethod()
        {
            return _underlyingType.HasPossibleWellKnownCloneMethod();
        }
    }
}
