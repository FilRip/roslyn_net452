using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public readonly struct TypeWithAnnotations : IFormattable
    {
        internal sealed class Boxed
        {
            internal static readonly Boxed Sentinel = new Boxed(default(TypeWithAnnotations));

            internal readonly TypeWithAnnotations Value;

            internal Boxed(TypeWithAnnotations value)
            {
                Value = value;
            }

            internal string GetDebuggerDisplay()
            {
                return Value.GetDebuggerDisplay();
            }
        }

        internal sealed class EqualsComparer : EqualityComparer<TypeWithAnnotations>
        {
            internal static readonly EqualsComparer ConsiderEverythingComparer = new EqualsComparer(TypeCompareKind.ConsiderEverything);

            internal static readonly EqualsComparer IgnoreNullableModifiersForReferenceTypesComparer = new EqualsComparer(TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);

            private readonly TypeCompareKind _compareKind;

            public EqualsComparer(TypeCompareKind compareKind)
            {
                _compareKind = compareKind;
            }

            public override int GetHashCode(TypeWithAnnotations obj)
            {
                if (!obj.HasType)
                {
                    return 0;
                }
                return obj.Type.GetHashCode();
            }

            public override bool Equals(TypeWithAnnotations x, TypeWithAnnotations y)
            {
                if (!x.HasType)
                {
                    return !y.HasType;
                }
                return x.Equals(y, _compareKind);
            }
        }

        private abstract class Extensions
        {
            internal static readonly Extensions Default = new NonLazyType(ImmutableArray<CustomModifier>.Empty);

            internal abstract bool IsResolved { get; }

            internal abstract ImmutableArray<CustomModifier> CustomModifiers { get; }

            internal static Extensions Create(ImmutableArray<CustomModifier> customModifiers)
            {
                if (customModifiers.IsEmpty)
                {
                    return Default;
                }
                return new NonLazyType(customModifiers);
            }

            internal static Extensions CreateLazy(CSharpCompilation compilation, TypeWithAnnotations underlying)
            {
                return new LazyNullableTypeParameter(compilation, underlying);
            }

            internal abstract TypeSymbol GetResolvedType(TypeSymbol defaultType);

            internal abstract TypeWithAnnotations AsNullableReferenceType(TypeWithAnnotations type);

            internal abstract TypeWithAnnotations AsNotNullableReferenceType(TypeWithAnnotations type);

            internal abstract TypeWithAnnotations WithModifiers(TypeWithAnnotations type, ImmutableArray<CustomModifier> customModifiers);

            internal abstract TypeSymbol GetNullableUnderlyingTypeOrSelf(TypeSymbol typeSymbol);

            internal abstract TypeSymbol AsTypeSymbolOnly(TypeSymbol typeSymbol);

            internal abstract SpecialType GetSpecialType(TypeSymbol typeSymbol);

            internal abstract bool IsRestrictedType(TypeSymbol typeSymbol, bool ignoreSpanLikeTypes);

            internal abstract bool IsStatic(TypeSymbol typeSymbol);

            internal abstract bool IsVoid(TypeSymbol typeSymbol);

            internal abstract bool IsSZArray(TypeSymbol typeSymbol);

            internal abstract TypeWithAnnotations WithTypeAndModifiers(TypeWithAnnotations type, TypeSymbol typeSymbol, ImmutableArray<CustomModifier> customModifiers);

            internal abstract bool TypeSymbolEquals(TypeWithAnnotations type, TypeWithAnnotations other, TypeCompareKind comparison);

            internal abstract TypeWithAnnotations SubstituteType(TypeWithAnnotations type, AbstractTypeMap typeMap);

            internal abstract void ReportDiagnosticsIfObsolete(TypeWithAnnotations type, Binder binder, SyntaxNode syntax, BindingDiagnosticBag diagnostics);

            internal abstract void TryForceResolve(bool asValueType);
        }

        private sealed class NonLazyType : Extensions
        {
            private readonly ImmutableArray<CustomModifier> _customModifiers;

            internal override bool IsResolved => true;

            internal override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

            public NonLazyType(ImmutableArray<CustomModifier> customModifiers)
            {
                _customModifiers = customModifiers;
            }

            internal override TypeSymbol GetResolvedType(TypeSymbol defaultType)
            {
                return defaultType;
            }

            internal override SpecialType GetSpecialType(TypeSymbol typeSymbol)
            {
                return typeSymbol.SpecialType;
            }

            internal override bool IsRestrictedType(TypeSymbol typeSymbol, bool ignoreSpanLikeTypes)
            {
                return typeSymbol.IsRestrictedType(ignoreSpanLikeTypes);
            }

            internal override bool IsStatic(TypeSymbol typeSymbol)
            {
                return typeSymbol.IsStatic;
            }

            internal override bool IsVoid(TypeSymbol typeSymbol)
            {
                return typeSymbol.IsVoidType();
            }

            internal override bool IsSZArray(TypeSymbol typeSymbol)
            {
                return typeSymbol.IsSZArray();
            }

            internal override TypeSymbol GetNullableUnderlyingTypeOrSelf(TypeSymbol typeSymbol)
            {
                return typeSymbol.StrippedType();
            }

            internal override TypeWithAnnotations WithModifiers(TypeWithAnnotations type, ImmutableArray<CustomModifier> customModifiers)
            {
                return CreateNonLazyType(type.DefaultType, type.NullableAnnotation, customModifiers);
            }

            internal override TypeSymbol AsTypeSymbolOnly(TypeSymbol typeSymbol)
            {
                return typeSymbol;
            }

            internal override TypeWithAnnotations WithTypeAndModifiers(TypeWithAnnotations type, TypeSymbol typeSymbol, ImmutableArray<CustomModifier> customModifiers)
            {
                return CreateNonLazyType(typeSymbol, type.NullableAnnotation, customModifiers);
            }

            internal override TypeWithAnnotations AsNullableReferenceType(TypeWithAnnotations type)
            {
                return CreateNonLazyType(type.DefaultType, NullableAnnotation.Annotated, _customModifiers);
            }

            internal override TypeWithAnnotations AsNotNullableReferenceType(TypeWithAnnotations type)
            {
                TypeSymbol defaultType = type.DefaultType;
                return CreateNonLazyType(defaultType, defaultType.IsNullableType() ? type.NullableAnnotation : NullableAnnotation.NotAnnotated, _customModifiers);
            }

            internal override bool TypeSymbolEquals(TypeWithAnnotations type, TypeWithAnnotations other, TypeCompareKind comparison)
            {
                return type.TypeSymbolEqualsCore(other, comparison);
            }

            internal override TypeWithAnnotations SubstituteType(TypeWithAnnotations type, AbstractTypeMap typeMap)
            {
                return type.SubstituteTypeCore(typeMap);
            }

            internal override void ReportDiagnosticsIfObsolete(TypeWithAnnotations type, Binder binder, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
            {
                type.ReportDiagnosticsIfObsoleteCore(binder, syntax, diagnostics);
            }

            internal override void TryForceResolve(bool asValueType)
            {
            }
        }

        private sealed class LazyNullableTypeParameter : Extensions
        {
            private readonly CSharpCompilation _compilation;

            private readonly TypeWithAnnotations _underlying;

            private TypeSymbol _resolved;

            internal override bool IsResolved => (object)_resolved != null;

            internal override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

            public LazyNullableTypeParameter(CSharpCompilation compilation, TypeWithAnnotations underlying)
            {
                _compilation = compilation;
                _underlying = underlying;
            }

            internal override bool IsVoid(TypeSymbol typeSymbol)
            {
                return false;
            }

            internal override bool IsSZArray(TypeSymbol typeSymbol)
            {
                return false;
            }

            internal override bool IsStatic(TypeSymbol typeSymbol)
            {
                return false;
            }

            private TypeSymbol GetResolvedType()
            {
                if ((object)_resolved == null)
                {
                    TryForceResolve(_underlying.Type.IsValueType);
                }
                return _resolved;
            }

            internal override TypeSymbol GetNullableUnderlyingTypeOrSelf(TypeSymbol typeSymbol)
            {
                return _underlying.Type;
            }

            internal override SpecialType GetSpecialType(TypeSymbol typeSymbol)
            {
                SpecialType specialType = _underlying.SpecialType;
                if (!specialType.IsValueType())
                {
                    return specialType;
                }
                return SpecialType.None;
            }

            internal override bool IsRestrictedType(TypeSymbol typeSymbol, bool ignoreSpanLikeTypes)
            {
                return _underlying.IsRestrictedType(ignoreSpanLikeTypes);
            }

            internal override TypeSymbol AsTypeSymbolOnly(TypeSymbol typeSymbol)
            {
                return GetResolvedType();
            }

            internal override TypeSymbol GetResolvedType(TypeSymbol defaultType)
            {
                return GetResolvedType();
            }

            internal override TypeWithAnnotations WithModifiers(TypeWithAnnotations type, ImmutableArray<CustomModifier> customModifiers)
            {
                if (customModifiers.IsEmpty)
                {
                    return type;
                }
                TypeSymbol resolvedType = GetResolvedType();
                if (resolvedType.IsNullableType())
                {
                    return TypeWithAnnotations.Create(resolvedType, type.NullableAnnotation, customModifiers);
                }
                return CreateNonLazyType(resolvedType, type.NullableAnnotation, customModifiers);
            }

            internal override TypeWithAnnotations WithTypeAndModifiers(TypeWithAnnotations type, TypeSymbol typeSymbol, ImmutableArray<CustomModifier> customModifiers)
            {
                if (typeSymbol.IsNullableType())
                {
                    return TypeWithAnnotations.Create(typeSymbol, type.NullableAnnotation, customModifiers);
                }
                return CreateNonLazyType(typeSymbol, type.NullableAnnotation, customModifiers);
            }

            internal override TypeWithAnnotations AsNullableReferenceType(TypeWithAnnotations type)
            {
                return type;
            }

            internal override TypeWithAnnotations AsNotNullableReferenceType(TypeWithAnnotations type)
            {
                if (!_underlying.Type.IsValueType)
                {
                    return _underlying;
                }
                return type;
            }

            internal override TypeWithAnnotations SubstituteType(TypeWithAnnotations type, AbstractTypeMap typeMap)
            {
                if ((object)_resolved != null)
                {
                    return type.SubstituteTypeCore(typeMap);
                }
                TypeWithAnnotations underlying = _underlying.SubstituteTypeCore(typeMap);
                if (!underlying.IsSameAs(_underlying))
                {
                    if (underlying.Type.Equals(_underlying.Type, TypeCompareKind.ConsiderEverything) && underlying.CustomModifiers.IsEmpty)
                    {
                        return CreateLazyNullableType(_compilation, underlying);
                    }
                    return type.SubstituteTypeCore(typeMap);
                }
                return type;
            }

            internal override void ReportDiagnosticsIfObsolete(TypeWithAnnotations type, Binder binder, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
            {
                if ((object)_resolved != null)
                {
                    type.ReportDiagnosticsIfObsoleteCore(binder, syntax, diagnostics);
                }
                else
                {
                    diagnostics.Add(new LazyObsoleteDiagnosticInfo(type, binder.ContainingMemberOrLambda, binder.Flags), syntax.GetLocation());
                }
            }

            internal override bool TypeSymbolEquals(TypeWithAnnotations type, TypeWithAnnotations other, TypeCompareKind comparison)
            {
                if (other._extensions is LazyNullableTypeParameter lazyNullableTypeParameter)
                {
                    return _underlying.TypeSymbolEquals(lazyNullableTypeParameter._underlying, comparison);
                }
                return type.TypeSymbolEqualsCore(other, comparison);
            }

            internal override void TryForceResolve(bool asValueType)
            {
                TypeSymbol value = (asValueType ? _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(ImmutableArray.Create(_underlying)) : _underlying.Type);
                Interlocked.CompareExchange(ref _resolved, value, null);
            }
        }

        internal readonly TypeSymbol DefaultType;

        private readonly Extensions _extensions;

        public readonly NullableAnnotation NullableAnnotation;

        private static readonly SymbolDisplayFormat DebuggerDisplayFormat = new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters, SymbolDisplayMemberOptions.None, SymbolDisplayDelegateStyle.NameOnly, SymbolDisplayExtensionMethodStyle.Default, SymbolDisplayParameterOptions.None, SymbolDisplayPropertyStyle.NameOnly, SymbolDisplayLocalOptions.None, SymbolDisplayKindOptions.None, SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        internal static readonly SymbolDisplayFormat TestDisplayFormat = new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters, SymbolDisplayMemberOptions.None, SymbolDisplayDelegateStyle.NameOnly, SymbolDisplayExtensionMethodStyle.Default, SymbolDisplayParameterOptions.None, SymbolDisplayPropertyStyle.NameOnly, SymbolDisplayLocalOptions.None, SymbolDisplayKindOptions.None, SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier);

        internal bool CanBeAssignedNull
        {
            get
            {
                switch (NullableAnnotation)
                {
                    case NullableAnnotation.Oblivious:
                    case NullableAnnotation.Annotated:
                        return true;
                    case NullableAnnotation.NotAnnotated:
                        return Type.IsNullableTypeOrTypeParameter();
                    default:
                        throw ExceptionUtilities.UnexpectedValue(NullableAnnotation);
                }
            }
        }

        internal bool IsDefault
        {
            get
            {
                if ((object)DefaultType == null && NullableAnnotation == NullableAnnotation.NotAnnotated)
                {
                    if (_extensions != null)
                    {
                        return _extensions == Extensions.Default;
                    }
                    return true;
                }
                return false;
            }
        }

        internal bool HasType => (object)DefaultType != null;

        public bool IsResolved => _extensions?.IsResolved ?? true;

        public TypeSymbol Type => _extensions?.GetResolvedType(DefaultType);

        public TypeSymbol NullableUnderlyingTypeOrSelf => _extensions.GetNullableUnderlyingTypeOrSelf(DefaultType);

        public ImmutableArray<CustomModifier> CustomModifiers => _extensions.CustomModifiers;

        public TypeKind TypeKind => Type.TypeKind;

        public SpecialType SpecialType => _extensions.GetSpecialType(DefaultType);

        public PrimitiveTypeCode PrimitiveTypeCode => Type.PrimitiveTypeCode;

        public bool IsStatic => _extensions.IsStatic(DefaultType);

        private TypeWithAnnotations(TypeSymbol defaultType, NullableAnnotation nullableAnnotation, Extensions extensions)
        {
            DefaultType = defaultType;
            NullableAnnotation = nullableAnnotation;
            _extensions = extensions;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        internal static TypeWithAnnotations Create(bool isNullableEnabled, TypeSymbol typeSymbol, bool isAnnotated = false)
        {
            if ((object)typeSymbol == null)
            {
                return default(TypeWithAnnotations);
            }
            return Create(typeSymbol, isAnnotated ? NullableAnnotation.Annotated : ((!isNullableEnabled) ? NullableAnnotation.Oblivious : NullableAnnotation.NotAnnotated));
        }

        internal static TypeWithAnnotations Create(TypeSymbol typeSymbol, NullableAnnotation nullableAnnotation = NullableAnnotation.Oblivious, ImmutableArray<CustomModifier> customModifiers = default(ImmutableArray<CustomModifier>))
        {
            if ((object)typeSymbol == null && nullableAnnotation == NullableAnnotation.NotAnnotated)
            {
                return default(TypeWithAnnotations);
            }
            if (nullableAnnotation <= NullableAnnotation.Oblivious && (object)typeSymbol != null && typeSymbol.IsNullableType())
            {
                nullableAnnotation = NullableAnnotation.Annotated;
            }
            return CreateNonLazyType(typeSymbol, nullableAnnotation, customModifiers.NullToEmpty());
        }

        internal TypeWithAnnotations AsAnnotated()
        {
            if (NullableAnnotation.IsAnnotated() || (Type.IsValueType && Type.IsNullableType()))
            {
                return this;
            }
            return Create(Type, NullableAnnotation.Annotated, CustomModifiers);
        }

        internal TypeWithAnnotations AsNotAnnotated()
        {
            if (NullableAnnotation.IsNotAnnotated() || (Type.IsValueType && !Type.IsNullableType()))
            {
                return this;
            }
            return Create(Type, NullableAnnotation.NotAnnotated, CustomModifiers);
        }

        internal NullableAnnotation GetValueNullableAnnotation()
        {
            if (NullableAnnotation.IsAnnotated())
            {
                return NullableAnnotation;
            }
            TypeSymbol type = Type;
            if ((object)type != null && type.IsPossiblyNullableReferenceTypeTypeParameter())
            {
                return NullableAnnotation.Annotated;
            }
            if (Type.IsNullableTypeOrTypeParameter())
            {
                return NullableAnnotation.Annotated;
            }
            return NullableAnnotation;
        }

        private static TypeWithAnnotations CreateNonLazyType(TypeSymbol typeSymbol, NullableAnnotation nullableAnnotation, ImmutableArray<CustomModifier> customModifiers)
        {
            return new TypeWithAnnotations(typeSymbol, nullableAnnotation, Extensions.Create(customModifiers));
        }

        private static TypeWithAnnotations CreateLazyNullableType(CSharpCompilation compilation, TypeWithAnnotations underlying)
        {
            return new TypeWithAnnotations(underlying.DefaultType, NullableAnnotation.Annotated, Extensions.CreateLazy(compilation, underlying));
        }

        public TypeWithAnnotations SetIsAnnotated(CSharpCompilation compilation)
        {
            CSharpCompilation compilation2 = compilation;
            TypeSymbol typeSymbol = Type;
            if (typeSymbol.TypeKind != TypeKind.TypeParameter)
            {
                if (!typeSymbol.IsValueType && !typeSymbol.IsErrorType())
                {
                    return CreateNonLazyType(typeSymbol, NullableAnnotation.Annotated, CustomModifiers);
                }
                return makeNullableT();
            }
            if (((TypeParameterSymbol)typeSymbol).TypeParameterKind == TypeParameterKind.Cref)
            {
                return makeNullableT();
            }
            return CreateLazyNullableType(compilation2, this);
            TypeWithAnnotations makeNullableT()
            {
                return Create(compilation2.GetSpecialType(SpecialType.System_Nullable_T).Construct(ImmutableArray.Create(typeSymbol)));
            }
        }

        public void TryForceResolve(bool asValueType)
        {
            _extensions.TryForceResolve(asValueType);
        }

        private TypeWithAnnotations AsNullableReferenceType()
        {
            return _extensions.AsNullableReferenceType(this);
        }

        public TypeWithAnnotations AsNotNullableReferenceType()
        {
            return _extensions.AsNotNullableReferenceType(this);
        }

        internal TypeWithAnnotations MergeEquivalentTypes(TypeWithAnnotations other, VarianceKind variance)
        {
            TypeSymbol type = other.Type;
            NullableAnnotation nullableAnnotation = NullableAnnotation.MergeNullableAnnotation(other.NullableAnnotation, variance);
            return Create(Type.MergeEquivalentTypes(type, variance), nullableAnnotation, CustomModifiers);
        }

        public TypeWithAnnotations WithModifiers(ImmutableArray<CustomModifier> customModifiers)
        {
            return _extensions.WithModifiers(this, customModifiers);
        }

        public bool IsNullableType()
        {
            return Type.IsNullableType();
        }

        public bool IsVoidType()
        {
            return _extensions.IsVoid(DefaultType);
        }

        public bool IsSZArray()
        {
            return _extensions.IsSZArray(DefaultType);
        }

        public bool IsRestrictedType(bool ignoreSpanLikeTypes = false)
        {
            return _extensions.IsRestrictedType(DefaultType, ignoreSpanLikeTypes);
        }

        public string ToDisplayString(SymbolDisplayFormat format = null)
        {
            if (!IsResolved && !IsSafeToResolve())
            {
                if (NullableAnnotation.IsAnnotated() && format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier))
                {
                    return DefaultType.ToDisplayString(format) + "?";
                }
                return DefaultType.ToDisplayString(format);
            }
            string text = ((!HasType) ? "<null>" : Type.ToDisplayString(format));
            if (format != null)
            {
                if (NullableAnnotation.IsAnnotated() && format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier) && (!HasType || (!IsNullableType() && !Type.IsValueType)))
                {
                    return text + "?";
                }
                if (NullableAnnotation.IsNotAnnotated() && format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier) && (!HasType || (!Type.IsValueType && !Type.IsTypeParameterDisallowingAnnotationInCSharp8())))
                {
                    return text + "!";
                }
            }
            return text;
        }

        private bool IsSafeToResolve()
        {
            if ((DefaultType as TypeParameterSymbol)?.DeclaringMethod is SourceOrdinaryMethodSymbol sourceOrdinaryMethodSymbol && !sourceOrdinaryMethodSymbol.HasComplete(CompletionPart.StartMemberChecks))
            {
                if (!sourceOrdinaryMethodSymbol.IsOverride)
                {
                    return !sourceOrdinaryMethodSymbol.IsExplicitInterfaceImplementation;
                }
                return false;
            }
            return true;
        }

        internal string GetDebuggerDisplay()
        {
            if (HasType)
            {
                return ToDisplayString(DebuggerDisplayFormat);
            }
            return "<null>";
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        }

        public bool Equals(TypeWithAnnotations other, TypeCompareKind comparison)
        {
            if (IsSameAs(other))
            {
                return true;
            }
            if (!HasType)
            {
                if (other.HasType)
                {
                    return false;
                }
            }
            else if (!other.HasType || !TypeSymbolEquals(other, comparison))
            {
                return false;
            }
            if ((comparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0 && !CustomModifiers.SequenceEqual(other.CustomModifiers))
            {
                return false;
            }
            NullableAnnotation nullableAnnotation = NullableAnnotation;
            NullableAnnotation nullableAnnotation2 = other.NullableAnnotation;
            if ((comparison & TypeCompareKind.IgnoreNullableModifiersForReferenceTypes) == 0 && nullableAnnotation2 != nullableAnnotation && ((comparison & TypeCompareKind.ObliviousNullableModifierMatchesAny) == 0 || (!nullableAnnotation.IsOblivious() && !nullableAnnotation2.IsOblivious())))
            {
                if (!HasType)
                {
                    return false;
                }
                TypeSymbol type = Type;
                if (!type.IsValueType || type.IsNullableType())
                {
                    return false;
                }
            }
            return true;
        }

        internal bool TypeSymbolEquals(TypeWithAnnotations other, TypeCompareKind comparison)
        {
            return _extensions.TypeSymbolEquals(this, other, comparison);
        }

        public bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            if (!Type.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes))
            {
                return Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result, CustomModifiers, owner, ref checkedTypes);
            }
            return true;
        }

        public bool IsAtLeastAsVisibleAs(Symbol sym, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return NullableUnderlyingTypeOrSelf.IsAtLeastAsVisibleAs(sym, ref useSiteInfo);
        }

        public TypeWithAnnotations SubstituteType(AbstractTypeMap typeMap)
        {
            return _extensions.SubstituteType(this, typeMap);
        }

        internal TypeWithAnnotations SubstituteTypeCore(AbstractTypeMap typeMap)
        {
            ImmutableArray<CustomModifier> immutableArray = typeMap.SubstituteCustomModifiers(CustomModifiers);
            TypeSymbol type = Type;
            TypeWithAnnotations result = typeMap.SubstituteType(type);
            if (!type.IsTypeParameter())
            {
                if (type.Equals(result.Type, TypeCompareKind.ConsiderEverything) && immutableArray == CustomModifiers)
                {
                    return this;
                }
                if ((NullableAnnotation.IsOblivious() || (type.IsNullableType() && NullableAnnotation.IsAnnotated())) && immutableArray.IsEmpty)
                {
                    return result;
                }
                return Create(result.Type, NullableAnnotation, immutableArray);
            }
            if (result.Is((TypeParameterSymbol)type) && immutableArray == CustomModifiers)
            {
                return this;
            }
            if (Is((TypeParameterSymbol)type) && result.NullableAnnotation != NullableAnnotation.Ignored)
            {
                return result;
            }
            if (result.Type is PlaceholderTypeArgumentSymbol)
            {
                return result;
            }
            return CreateNonLazyType(nullableAnnotation: (NullableAnnotation.IsAnnotated() || result.NullableAnnotation.IsAnnotated()) ? NullableAnnotation.Annotated : ((result.NullableAnnotation == NullableAnnotation.Ignored) ? NullableAnnotation : ((NullableAnnotation != NullableAnnotation.Oblivious) ? (type.IsTypeParameterDisallowingAnnotationInCSharp8() ? result.NullableAnnotation : NullableAnnotation) : ((result.NullableAnnotation == NullableAnnotation.Oblivious) ? NullableAnnotation : result.NullableAnnotation))), typeSymbol: result.Type, customModifiers: immutableArray.Concat(result.CustomModifiers));
        }

        public void ReportDiagnosticsIfObsolete(Binder binder, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            _extensions.ReportDiagnosticsIfObsolete(this, binder, syntax, diagnostics);
        }

        private bool TypeSymbolEqualsCore(TypeWithAnnotations other, TypeCompareKind comparison)
        {
            return Type.Equals(other.Type, comparison);
        }

        private void ReportDiagnosticsIfObsoleteCore(Binder binder, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            binder.ReportDiagnosticsIfObsolete(diagnostics, Type, syntax, hasBaseReceiver: false);
        }

        public TypeSymbol AsTypeSymbolOnly()
        {
            return _extensions.AsTypeSymbolOnly(DefaultType);
        }

        public bool Is(TypeParameterSymbol other)
        {
            if (NullableAnnotation.IsOblivious() && (object)DefaultType == other)
            {
                return CustomModifiers.IsEmpty;
            }
            return false;
        }

        public TypeWithAnnotations WithTypeAndModifiers(TypeSymbol typeSymbol, ImmutableArray<CustomModifier> customModifiers)
        {
            return _extensions.WithTypeAndModifiers(this, typeSymbol, customModifiers);
        }

        public TypeWithAnnotations WithType(TypeSymbol typeSymbol)
        {
            return _extensions.WithTypeAndModifiers(this, typeSymbol, CustomModifiers);
        }

        public bool NeedsNullableAttribute()
        {
            return NeedsNullableAttribute(this, null);
        }

        public static bool NeedsNullableAttribute(TypeWithAnnotations typeWithAnnotationsOpt, TypeSymbol typeOpt)
        {
            return (object)typeWithAnnotationsOpt.VisitType(typeOpt, (TypeWithAnnotations t, object a, bool b) => t.NullableAnnotation != NullableAnnotation.Oblivious && !t.Type.IsErrorType() && !t.Type.IsValueType, null, null) != null;
        }

        private static bool IsNonGenericValueType(TypeSymbol type)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            if (namedTypeSymbol.IsGenericType)
            {
                return type.IsNullableType();
            }
            return type.IsValueType;
        }

        public void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            AddNullableTransforms(this, transforms);
        }

        private static void AddNullableTransforms(TypeWithAnnotations typeWithAnnotations, ArrayBuilder<byte> transforms)
        {
            TypeSymbol type;
            while (true)
            {
                type = typeWithAnnotations.Type;
                if (!IsNonGenericValueType(type))
                {
                    NullableAnnotation nullableAnnotation = typeWithAnnotations.NullableAnnotation;
                    byte item = (byte)((!nullableAnnotation.IsOblivious() && !type.IsValueType) ? ((!nullableAnnotation.IsAnnotated()) ? 1 : 2) : 0);
                    transforms.Add(item);
                }
                if (type.TypeKind != TypeKind.Array)
                {
                    break;
                }
                typeWithAnnotations = ((ArrayTypeSymbol)type).ElementTypeWithAnnotations;
            }
            type.AddNullableTransforms(transforms);
        }

        public bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeWithAnnotations result)
        {
            result = this;
            TypeSymbol type = Type;
            byte b;
            if (IsNonGenericValueType(type))
            {
                b = 0;
            }
            else if (transforms.IsDefault)
            {
                b = defaultTransformFlag;
            }
            else
            {
                if (position >= transforms.Length)
                {
                    return false;
                }
                b = transforms[position++];
            }
            if (!type.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position, out var result2))
            {
                return false;
            }
            if ((object)type != result2)
            {
                result = result.WithTypeAndModifiers(result2, result.CustomModifiers);
            }
            switch (b)
            {
                case 2:
                    result = result.AsNullableReferenceType();
                    break;
                case 1:
                    result = result.AsNotNullableReferenceType();
                    break;
                case 0:
                    if (result.NullableAnnotation != NullableAnnotation.Oblivious && (!result.NullableAnnotation.IsAnnotated() || !type.IsNullableType()))
                    {
                        result = CreateNonLazyType(result2, NullableAnnotation.Oblivious, result.CustomModifiers);
                    }
                    break;
                default:
                    result = this;
                    return false;
            }
            return true;
        }

        public TypeWithAnnotations WithTopLevelNonNullability()
        {
            TypeSymbol type = Type;
            if (NullableAnnotation.IsNotAnnotated() || (type.IsValueType && !type.IsNullableType()))
            {
                return this;
            }
            return CreateNonLazyType(type, NullableAnnotation.NotAnnotated, CustomModifiers);
        }

        public TypeWithAnnotations SetUnknownNullabilityForReferenceTypes()
        {
            TypeSymbol type = Type;
            TypeSymbol typeSymbol = type.SetUnknownNullabilityForReferenceTypes();
            if (NullableAnnotation != NullableAnnotation.Oblivious && !type.IsValueType)
            {
                return CreateNonLazyType(typeSymbol, NullableAnnotation.Oblivious, CustomModifiers);
            }
            if ((object)typeSymbol != type)
            {
                return WithTypeAndModifiers(typeSymbol, CustomModifiers);
            }
            return this;
        }

        [Obsolete("Unsupported", true)]
        public override bool Equals(object other)
        {
            if (other is TypeWithAnnotations other2)
            {
                return Equals(other2, TypeCompareKind.ConsiderEverything);
            }
            return false;
        }

        [Obsolete("Unsupported", true)]
        public override int GetHashCode()
        {
            if (!HasType)
            {
                return 0;
            }
            return Type.GetHashCode();
        }

        public static bool operator ==(TypeWithAnnotations? x, TypeWithAnnotations? y)
        {
            if (x.HasValue == y.HasValue)
            {
                return x?.IsSameAs(y.GetValueOrDefault()) ?? true;
            }
            return false;
        }

        public static bool operator !=(TypeWithAnnotations? x, TypeWithAnnotations? y)
        {
            return !(x == y);
        }

        internal bool IsSameAs(TypeWithAnnotations other)
        {
            if ((object)DefaultType == other.DefaultType && NullableAnnotation == other.NullableAnnotation)
            {
                return _extensions == other._extensions;
            }
            return false;
        }

        internal TypeWithState ToTypeWithState()
        {
            return TypeWithState.Create(Type, getFlowState(Type, NullableAnnotation));
            static NullableFlowState getFlowState(TypeSymbol type, NullableAnnotation annotation)
            {
                if ((object)type == null)
                {
                    if (!annotation.IsAnnotated())
                    {
                        return NullableFlowState.NotNull;
                    }
                    return NullableFlowState.MaybeDefault;
                }
                if (type.IsPossiblyNullableReferenceTypeTypeParameter())
                {
                    return annotation switch
                    {
                        NullableAnnotation.Annotated => NullableFlowState.MaybeDefault,
                        NullableAnnotation.NotAnnotated => NullableFlowState.MaybeNull,
                        _ => NullableFlowState.NotNull,
                    };
                }
                if (type.IsTypeParameterDisallowingAnnotationInCSharp8())
                {
                    if (annotation == NullableAnnotation.Annotated)
                    {
                        return NullableFlowState.MaybeDefault;
                    }
                    return NullableFlowState.NotNull;
                }
                if (type.IsNullableTypeOrTypeParameter())
                {
                    return NullableFlowState.MaybeNull;
                }
                if (annotation == NullableAnnotation.Annotated)
                {
                    return NullableFlowState.MaybeNull;
                }
                return NullableFlowState.NotNull;
            }
        }
    }
}
