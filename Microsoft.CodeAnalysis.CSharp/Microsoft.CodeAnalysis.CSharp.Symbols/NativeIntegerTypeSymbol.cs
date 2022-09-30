using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class NativeIntegerTypeSymbol : WrappedNamedTypeSymbol, IReference
    {
        private sealed class NativeIntegerTypeMap : AbstractTypeMap
        {
            private readonly NativeIntegerTypeSymbol _type;

            private readonly SpecialType _specialType;

            internal NativeIntegerTypeMap(NativeIntegerTypeSymbol type)
            {
                _type = type;
                _specialType = _type.UnderlyingNamedType.SpecialType;
            }

            internal override NamedTypeSymbol SubstituteTypeDeclaration(NamedTypeSymbol previous)
            {
                if (previous.SpecialType != _specialType)
                {
                    return base.SubstituteTypeDeclaration(previous);
                }
                return _type;
            }

            internal override ImmutableArray<CustomModifier> SubstituteCustomModifiers(ImmutableArray<CustomModifier> customModifiers)
            {
                return customModifiers;
            }
        }

        private ImmutableArray<NamedTypeSymbol> _lazyInterfaces;

        private ImmutableArray<Symbol> _lazyMembers;

        private NativeIntegerTypeMap? _lazyTypeMap;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override NamedTypeSymbol ConstructedFrom => this;

        public override Symbol ContainingSymbol => _underlyingType.ContainingSymbol;

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => ImmutableArray<TypeWithAnnotations>.Empty;

        internal override bool IsComImport => _underlyingType.IsComImport;

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => _underlyingType.BaseTypeNoUseSiteDiagnostics;

        public override SpecialType SpecialType => _underlyingType.SpecialType;

        public override IEnumerable<string> MemberNames => from m in GetMembers()
                                                           select m.Name;

        public override bool AreLocalsZeroed
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsNativeIntegerType => true;

        internal sealed override NamedTypeSymbol NativeIntegerUnderlyingType => _underlyingType;

        internal sealed override bool IsRecord => false;

        internal sealed override bool IsRecordStruct => false;

        internal NativeIntegerTypeSymbol(NamedTypeSymbol underlyingType)
            : base(underlyingType, null)
        {
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            if (_lazyMembers.IsDefault)
            {
                ImmutableInterlocked.InterlockedInitialize(ref _lazyMembers, makeMembers(_underlyingType.GetMembers()));
            }
            return _lazyMembers;
            ImmutableArray<Symbol> makeMembers(ImmutableArray<Symbol> underlyingMembers)
            {
                ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
                instance.Add(new SynthesizedInstanceConstructor(this));
                ImmutableArray<Symbol>.Enumerator enumerator = underlyingMembers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.DeclaredAccessibility == Accessibility.Public)
                    {
                        if (!(current is MethodSymbol methodSymbol))
                        {
                            if (current is PropertySymbol propertySymbol && propertySymbol.ParameterCount == 0 && propertySymbol.Name != "Size")
                            {
                                NativeIntegerPropertySymbol nativeIntegerPropertySymbol = new NativeIntegerPropertySymbol(this, propertySymbol, (NativeIntegerTypeSymbol container, NativeIntegerPropertySymbol property, MethodSymbol? underlyingAccessor) => ((object)underlyingAccessor != null) ? new NativeIntegerMethodSymbol(container, underlyingAccessor, property) : null);
                                instance.Add(nativeIntegerPropertySymbol);
                                instance.AddIfNotNull(nativeIntegerPropertySymbol.GetMethod);
                                instance.AddIfNotNull(nativeIntegerPropertySymbol.SetMethod);
                            }
                        }
                        else if (!methodSymbol.IsGenericMethod && !methodSymbol.IsAccessor() && methodSymbol.MethodKind == MethodKind.Ordinary)
                        {
                            switch (methodSymbol.Name)
                            {
                                default:
                                    instance.Add(new NativeIntegerMethodSymbol(this, methodSymbol, null));
                                    break;
                                case "Add":
                                case "Subtract":
                                case "ToInt32":
                                case "ToInt64":
                                case "ToUInt32":
                                case "ToUInt64":
                                case "ToPointer":
                                    break;
                            }
                        }
                    }
                }
                return instance.ToImmutableAndFree();
            }
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return GetMembers().WhereAsArray((Symbol member, string name) => member.Name == name, name);
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
        {
            return _underlyingType.GetDeclaredBaseType(basesBeingResolved);
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
        {
            return GetInterfaces(basesBeingResolved);
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol>? basesBeingResolved = null)
        {
            return GetInterfaces(basesBeingResolved);
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            return _underlyingType.GetUseSiteInfo();
        }

        internal sealed override NamedTypeSymbol AsNativeInteger()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override bool HasPossibleWellKnownCloneMethod()
        {
            return false;
        }

        internal override bool Equals(TypeSymbol? other, TypeCompareKind comparison)
        {
            if ((object)other == null)
            {
                return false;
            }
            if ((object)this == other)
            {
                return true;
            }
            if (!_underlyingType.Equals(other, comparison))
            {
                return false;
            }
            if ((comparison & TypeCompareKind.IgnoreNativeIntegers) == 0)
            {
                return other!.IsNativeIntegerType;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return _underlyingType.GetHashCode();
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeSymbol>? basesBeingResolved)
        {
            if (_lazyInterfaces.IsDefault)
            {
                ImmutableArray<NamedTypeSymbol> value = _underlyingType.InterfacesNoUseSiteDiagnostics(basesBeingResolved).SelectAsArray((NamedTypeSymbol type, NativeIntegerTypeMap map) => map.SubstituteNamedType(type), GetTypeMap());
                ImmutableInterlocked.InterlockedInitialize(ref _lazyInterfaces, value);
            }
            return _lazyInterfaces;
        }

        private NativeIntegerTypeMap GetTypeMap()
        {
            if (_lazyTypeMap == null)
            {
                Interlocked.CompareExchange(ref _lazyTypeMap, new NativeIntegerTypeMap(this), null);
            }
            return _lazyTypeMap;
        }

        internal TypeWithAnnotations SubstituteUnderlyingType(TypeWithAnnotations type)
        {
            return type.SubstituteType(GetTypeMap());
        }

        internal NamedTypeSymbol SubstituteUnderlyingType(NamedTypeSymbol type)
        {
            return GetTypeMap().SubstituteNamedType(type);
        }

        internal static bool EqualsHelper<TSymbol>(TSymbol symbol, Symbol? other, TypeCompareKind comparison, Func<TSymbol, Symbol> getUnderlyingSymbol) where TSymbol : Symbol
        {
            if ((object)other == null)
            {
                return false;
            }
            if ((object)symbol == other)
            {
                return true;
            }
            if (!getUnderlyingSymbol(symbol).Equals(other, comparison))
            {
                return false;
            }
            if ((comparison & TypeCompareKind.IgnoreNativeIntegers) == 0)
            {
                return other is TSymbol;
            }
            return true;
        }

        [Conditional("DEBUG")]
        internal static void VerifyEquality(Symbol symbolA, Symbol symbolB)
        {
        }
    }
}
