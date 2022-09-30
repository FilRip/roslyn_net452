using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class DynamicTypeSymbol : TypeSymbol
    {
        internal static readonly DynamicTypeSymbol Instance = new DynamicTypeSymbol();

        public override string Name => "dynamic";

        public override bool IsAbstract => false;

        public override bool IsReferenceType => true;

        public override bool IsSealed => false;

        public override SymbolKind Kind => SymbolKind.DynamicType;

        public override TypeKind TypeKind => TypeKind.Dynamic;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        internal override NamedTypeSymbol? BaseTypeNoUseSiteDiagnostics => null;

        public override bool IsStatic => false;

        public override bool IsValueType => false;

        public sealed override bool IsRefLikeType => false;

        public sealed override bool IsReadOnly => false;

        internal sealed override ObsoleteAttributeData? ObsoleteAttributeData => null;

        public override Symbol? ContainingSymbol => null;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        private DynamicTypeSymbol()
        {
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol>? basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal sealed override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ManagedKind.Managed;
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitDynamicType(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitDynamicType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitDynamicType(this);
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        internal override bool Equals(TypeSymbol? t2, TypeCompareKind comparison)
        {
            if ((object)t2 == null)
            {
                return false;
            }
            if ((object)this == t2 || t2!.TypeKind == TypeKind.Dynamic)
            {
                return true;
            }
            if ((comparison & TypeCompareKind.IgnoreDynamic) != 0)
            {
                if (t2 is NamedTypeSymbol namedTypeSymbol)
                {
                    return namedTypeSymbol.SpecialType == SpecialType.System_Object;
                }
                return false;
            }
            return false;
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            result = this;
            return true;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            return this;
        }

        internal override TypeSymbol MergeEquivalentTypes(TypeSymbol other, VarianceKind variance)
        {
            return this;
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.DynamicTypeSymbol(this, base.DefaultNullableAnnotation);
        }

        protected sealed override ITypeSymbol CreateITypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.DynamicTypeSymbol(this, nullableAnnotation);
        }
    }
}
