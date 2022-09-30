using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal abstract class NamedTypeSymbol : TypeSymbol, INamedTypeSymbol, ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private ImmutableArray<ITypeSymbol> _lazyTypeArguments;

        internal abstract Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol UnderlyingNamedTypeSymbol { get; }

        int INamedTypeSymbol.Arity => UnderlyingNamedTypeSymbol.Arity;

        ImmutableArray<IMethodSymbol> INamedTypeSymbol.InstanceConstructors => UnderlyingNamedTypeSymbol.InstanceConstructors.GetPublicSymbols();

        ImmutableArray<IMethodSymbol> INamedTypeSymbol.StaticConstructors => UnderlyingNamedTypeSymbol.StaticConstructors.GetPublicSymbols();

        ImmutableArray<IMethodSymbol> INamedTypeSymbol.Constructors => UnderlyingNamedTypeSymbol.Constructors.GetPublicSymbols();

        IEnumerable<string> INamedTypeSymbol.MemberNames => UnderlyingNamedTypeSymbol.MemberNames;

        ImmutableArray<ITypeParameterSymbol> INamedTypeSymbol.TypeParameters => UnderlyingNamedTypeSymbol.TypeParameters.GetPublicSymbols();

        ImmutableArray<ITypeSymbol> INamedTypeSymbol.TypeArguments
        {
            get
            {
                if (_lazyTypeArguments.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeArguments, UnderlyingNamedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetPublicSymbols(), default(ImmutableArray<ITypeSymbol>));
                }
                return _lazyTypeArguments;
            }
        }

        ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> INamedTypeSymbol.TypeArgumentNullableAnnotations => UnderlyingNamedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.ToPublicAnnotations();

        INamedTypeSymbol INamedTypeSymbol.OriginalDefinition => UnderlyingNamedTypeSymbol.OriginalDefinition.GetPublicSymbol();

        IMethodSymbol INamedTypeSymbol.DelegateInvokeMethod => UnderlyingNamedTypeSymbol.DelegateInvokeMethod.GetPublicSymbol();

        INamedTypeSymbol INamedTypeSymbol.EnumUnderlyingType => UnderlyingNamedTypeSymbol.EnumUnderlyingType.GetPublicSymbol();

        INamedTypeSymbol INamedTypeSymbol.ConstructedFrom => UnderlyingNamedTypeSymbol.ConstructedFrom.GetPublicSymbol();

        ISymbol INamedTypeSymbol.AssociatedSymbol => null;

        ImmutableArray<IFieldSymbol> INamedTypeSymbol.TupleElements => UnderlyingNamedTypeSymbol.TupleElements.GetPublicSymbols();

        INamedTypeSymbol INamedTypeSymbol.TupleUnderlyingType
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol underlyingNamedTypeSymbol = UnderlyingNamedTypeSymbol;
                Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol tupleUnderlyingType = underlyingNamedTypeSymbol.TupleUnderlyingType;
                if (!underlyingNamedTypeSymbol.Equals(tupleUnderlyingType, TypeCompareKind.ConsiderEverything))
                {
                    return tupleUnderlyingType.GetPublicSymbol();
                }
                return null;
            }
        }

        bool INamedTypeSymbol.IsComImport => UnderlyingNamedTypeSymbol.IsComImport;

        bool INamedTypeSymbol.IsGenericType => UnderlyingNamedTypeSymbol.IsGenericType;

        bool INamedTypeSymbol.IsUnboundGenericType => UnderlyingNamedTypeSymbol.IsUnboundGenericType;

        bool INamedTypeSymbol.IsScriptClass => UnderlyingNamedTypeSymbol.IsScriptClass;

        bool INamedTypeSymbol.IsImplicitClass => UnderlyingNamedTypeSymbol.IsImplicitClass;

        bool INamedTypeSymbol.MightContainExtensionMethods => UnderlyingNamedTypeSymbol.MightContainExtensionMethods;

        bool INamedTypeSymbol.IsSerializable => UnderlyingNamedTypeSymbol.IsSerializable;

        INamedTypeSymbol INamedTypeSymbol.NativeIntegerUnderlyingType => UnderlyingNamedTypeSymbol.NativeIntegerUnderlyingType.GetPublicSymbol();

        public NamedTypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation = Microsoft.CodeAnalysis.NullableAnnotation.None)
            : base(nullableAnnotation)
        {
        }

        ImmutableArray<CustomModifier> INamedTypeSymbol.GetTypeArgumentCustomModifiers(int ordinal)
        {
            return UnderlyingNamedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[ordinal].CustomModifiers;
        }

        INamedTypeSymbol INamedTypeSymbol.Construct(params ITypeSymbol[] typeArguments)
        {
            return UnderlyingNamedTypeSymbol.Construct(Symbol.ConstructTypeArguments(typeArguments), unbound: false).GetPublicSymbol();
        }

        INamedTypeSymbol INamedTypeSymbol.Construct(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> typeArgumentNullableAnnotations)
        {
            return UnderlyingNamedTypeSymbol.Construct(Symbol.ConstructTypeArguments(typeArguments, typeArgumentNullableAnnotations), unbound: false).GetPublicSymbol();
        }

        INamedTypeSymbol INamedTypeSymbol.ConstructUnboundGenericType()
        {
            return UnderlyingNamedTypeSymbol.ConstructUnboundGenericType().GetPublicSymbol();
        }

        protected sealed override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitNamedType(this);
        }

        protected sealed override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitNamedType(this);
        }
    }
}
