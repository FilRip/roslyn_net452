using System.Collections.Immutable;
using System.Linq;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class FunctionPointerParameterSymbol : ParameterSymbol
    {
        private readonly FunctionPointerMethodSymbol _containingSymbol;

        public override TypeWithAnnotations TypeWithAnnotations { get; }

        public override RefKind RefKind { get; }

        public override int Ordinal { get; }

        public override Symbol ContainingSymbol => _containingSymbol;

        public override ImmutableArray<CustomModifier> RefCustomModifiers { get; }

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override bool IsDiscard => false;

        public override bool IsParams => false;

        public override bool IsImplicitlyDeclared => true;

        internal override MarshalPseudoCustomAttributeData? MarshallingInformation => null;

        internal override bool IsMetadataOptional => false;

        internal override bool IsMetadataIn => RefKind == RefKind.In;

        internal override bool IsMetadataOut => RefKind == RefKind.Out;

        internal override ConstantValue? ExplicitDefaultConstantValue => null;

        internal override bool IsIDispatchConstant => false;

        internal override bool IsIUnknownConstant => false;

        internal override bool IsCallerFilePath => false;

        internal override bool IsCallerLineNumber => false;

        internal override bool IsCallerMemberName => false;

        internal override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        internal override ImmutableHashSet<string> NotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public FunctionPointerParameterSymbol(TypeWithAnnotations typeWithAnnotations, RefKind refKind, int ordinal, FunctionPointerMethodSymbol containingSymbol, ImmutableArray<CustomModifier> refCustomModifiers)
        {
            TypeWithAnnotations = typeWithAnnotations;
            RefKind = refKind;
            Ordinal = ordinal;
            _containingSymbol = containingSymbol;
            RefCustomModifiers = refCustomModifiers;
        }

        public override bool Equals(Symbol other, TypeCompareKind compareKind)
        {
            if ((object)this == other)
            {
                return true;
            }
            if (!(other is FunctionPointerParameterSymbol other2))
            {
                return false;
            }
            return Equals(other2, compareKind);
        }

        internal bool Equals(FunctionPointerParameterSymbol other, TypeCompareKind compareKind)
        {
            if (other.Ordinal == Ordinal)
            {
                return _containingSymbol.Equals(other._containingSymbol, compareKind);
            }
            return false;
        }

        internal bool MethodEqualityChecks(FunctionPointerParameterSymbol other, TypeCompareKind compareKind)
        {
            if (FunctionPointerTypeSymbol.RefKindEquals(compareKind, RefKind, other.RefKind) && ((compareKind & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) != 0 || RefCustomModifiers.SequenceEqual(other.RefCustomModifiers)))
            {
                return TypeWithAnnotations.Equals(other.TypeWithAnnotations, compareKind);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_containingSymbol.GetHashCode(), Ordinal + 1);
        }

        internal int MethodHashCode()
        {
            return Hash.Combine(TypeWithAnnotations.GetHashCode(), FunctionPointerTypeSymbol.GetRefKindForHashCode(RefKind).GetHashCode());
        }
    }
}