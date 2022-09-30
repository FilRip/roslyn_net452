using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourceParameterSymbol : SourceParameterSymbolBase
    {
        protected SymbolCompletionState state;

        protected readonly TypeWithAnnotations parameterType;

        private readonly string _name;

        private readonly ImmutableArray<Location> _locations;

        private readonly RefKind _refKind;

        internal sealed override bool RequiresCompletion => true;

        internal abstract bool HasOptionalAttribute { get; }

        internal abstract bool HasDefaultArgumentSyntax { get; }

        internal abstract SyntaxList<AttributeListSyntax> AttributeDeclarationList { get; }

        internal abstract SyntaxReference SyntaxReference { get; }

        internal abstract bool IsExtensionMethodThis { get; }

        public sealed override RefKind RefKind => _refKind;

        public sealed override string Name => _name;

        public sealed override ImmutableArray<Location> Locations => _locations;

        public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                if (!IsImplicitlyDeclared)
                {
                    return Symbol.GetDeclaringSyntaxReferenceHelper<ParameterSyntax>(_locations);
                }
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public sealed override TypeWithAnnotations TypeWithAnnotations => parameterType;

        public override bool IsImplicitlyDeclared
        {
            get
            {
                if (ContainingSymbol is MethodSymbol methodSymbol)
                {
                    return methodSymbol.IsAccessor();
                }
                return false;
            }
        }

        internal override bool IsMetadataIn => RefKind == RefKind.In;

        internal override bool IsMetadataOut => RefKind == RefKind.Out;

        public static SourceParameterSymbol Create(Binder context, Symbol owner, TypeWithAnnotations parameterType, ParameterSyntax syntax, RefKind refKind, SyntaxToken identifier, int ordinal, bool isParams, bool isExtensionMethodThis, bool addRefReadOnlyModifier, BindingDiagnosticBag declarationDiagnostics)
        {
            string valueText = identifier.ValueText;
            ImmutableArray<Location> locations = ImmutableArray.Create((Location)new SourceLocation(in identifier));
            if (isParams)
            {
                Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(context.Compilation, WellKnownMember.System_ParamArrayAttribute__ctor, declarationDiagnostics, identifier.Parent!.GetLocation());
            }
            ImmutableArray<CustomModifier> refCustomModifiers = ParameterHelpers.ConditionallyCreateInModifiers(refKind, addRefReadOnlyModifier, context, declarationDiagnostics, syntax);
            if (!refCustomModifiers.IsDefaultOrEmpty)
            {
                return new SourceComplexParameterSymbolWithCustomModifiersPrecedingByRef(owner, ordinal, parameterType, refKind, refCustomModifiers, valueText, locations, syntax.GetReference(), isParams, isExtensionMethodThis);
            }
            if (!isParams && !isExtensionMethodThis && syntax.Default == null && syntax.AttributeLists.Count == 0 && !owner.IsPartialMethod())
            {
                return new SourceSimpleParameterSymbol(owner, parameterType, ordinal, refKind, valueText, locations);
            }
            return new SourceComplexParameterSymbol(owner, ordinal, parameterType, refKind, valueText, locations, syntax.GetReference(), isParams, isExtensionMethodThis);
        }

        protected SourceParameterSymbol(Symbol owner, TypeWithAnnotations parameterType, int ordinal, RefKind refKind, string name, ImmutableArray<Location> locations)
            : base(owner, ordinal)
        {
            this.parameterType = parameterType;
            _refKind = refKind;
            _name = name;
            _locations = locations;
        }

        internal override ParameterSymbol WithCustomModifiersAndParams(TypeSymbol newType, ImmutableArray<CustomModifier> newCustomModifiers, ImmutableArray<CustomModifier> newRefCustomModifiers, bool newIsParams)
        {
            return WithCustomModifiersAndParamsCore(newType, newCustomModifiers, newRefCustomModifiers, newIsParams);
        }

        internal SourceParameterSymbol WithCustomModifiersAndParamsCore(TypeSymbol newType, ImmutableArray<CustomModifier> newCustomModifiers, ImmutableArray<CustomModifier> newRefCustomModifiers, bool newIsParams)
        {
            newType = CustomModifierUtils.CopyTypeCustomModifiers(newType, base.Type, ContainingAssembly);
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations.WithTypeAndModifiers(newType, newCustomModifiers);
            if (newRefCustomModifiers.IsEmpty)
            {
                return new SourceComplexParameterSymbol(ContainingSymbol, Ordinal, typeWithAnnotations, _refKind, _name, _locations, SyntaxReference, newIsParams, IsExtensionMethodThis);
            }
            return new SourceComplexParameterSymbolWithCustomModifiersPrecedingByRef(ContainingSymbol, Ordinal, typeWithAnnotations, _refKind, newRefCustomModifiers, _name, _locations, SyntaxReference, newIsParams, IsExtensionMethodThis);
        }

        internal sealed override bool HasComplete(CompletionPart part)
        {
            return state.HasComplete(part);
        }

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            state.DefaultForceComplete(this, cancellationToken);
        }

        internal abstract CustomAttributesBag<CSharpAttributeData> GetAttributesBag();

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return GetAttributesBag().Attributes;
        }

        internal override void AddDeclarationDiagnostics(BindingDiagnosticBag diagnostics)
        {
            ContainingSymbol.AddDeclarationDiagnostics(diagnostics);
        }
    }
}
