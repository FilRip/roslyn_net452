using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SynthesizedRecordEqualityContractProperty : SourcePropertySymbolBase
    {
        internal sealed class GetAccessorSymbol : SourcePropertyAccessorSymbol
        {
            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

            internal override bool SynthesizesLoweredBoundBody => true;

            internal GetAccessorSymbol(NamedTypeSymbol containingType, SourcePropertySymbolBase property, DeclarationModifiers propertyModifiers, Location location, CSharpSyntaxNode syntax, BindingDiagnosticBag diagnostics)
                : base(containingType, property, propertyModifiers, location, syntax, hasBody: false, hasExpressionBody: false, isIterator: false, default(SyntaxTokenList), MethodKind.PropertyGet, usesInit: false, isAutoPropertyAccessor: true, isNullableAnalysisEnabled: false, diagnostics)
            {
            }

            internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, this.GetNonNullSyntaxNode(), compilationState, diagnostics);
                try
                {
                    syntheticBoundNodeFactory.CurrentFunction = this;
                    syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(syntheticBoundNodeFactory.Typeof(ContainingType))));
                }
                catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
                {
                    diagnostics.Add(missingPredefinedMember.Diagnostic);
                    syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                }
            }
        }

        internal const string PropertyName = "EqualityContract";

        public override bool IsImplicitlyDeclared => true;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => default(SyntaxList<AttributeListSyntax>);

        public override IAttributeTargetSymbol AttributesOwner => this;

        protected override Location TypeLocation => ContainingType.Locations[0];

        protected override bool HasPointerTypeSyntactically => false;

        public SynthesizedRecordEqualityContractProperty(SourceMemberContainerTypeSymbol containingType, BindingDiagnosticBag diagnostics)
            : base(
                containingType,
                syntax: (CSharpSyntaxNode)containingType.SyntaxReferences[0].GetSyntax(),
                hasGetAccessor: true,
                hasSetAccessor: false,
                isExplicitInterfaceImplementation: false,
                explicitInterfaceType: null,
                aliasQualifierOpt: null,
                modifiers: (containingType.IsSealed, containingType.BaseTypeNoUseSiteDiagnostics.IsObjectType()) switch
                {
                    (true, true) => DeclarationModifiers.Private,
                    (false, true) => DeclarationModifiers.Protected | DeclarationModifiers.Virtual,
                    (_, false) => DeclarationModifiers.Protected | DeclarationModifiers.Override
                },
                hasInitializer: false,
                isAutoProperty: false,
                isExpressionBodied: false,
                isInitOnly: false,
                RefKind.None,
                PropertyName,
                indexerNameAttributeLists: new SyntaxList<AttributeListSyntax>(),
                containingType.Locations[0],
                diagnostics)
        {
        }

        public override SourcePropertyAccessorSymbol CreateGetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            return SourcePropertyAccessorSymbol.CreateAccessorSymbol(ContainingType, this, _modifiers, ContainingType.Locations[0], (CSharpSyntaxNode)((SourceMemberContainerTypeSymbol)ContainingType).SyntaxReferences[0].GetSyntax(), diagnostics);
        }

        public override SourcePropertyAccessorSymbol CreateSetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            throw ExceptionUtilities.Unreachable;
        }

        protected override (TypeWithAnnotations Type, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindType(BindingDiagnosticBag diagnostics)
        {
            return (TypeWithAnnotations.Create(Binder.GetWellKnownType(DeclaringCompilation, WellKnownType.System_Type, diagnostics, base.Location), NullableAnnotation.NotAnnotated), ImmutableArray<ParameterSymbol>.Empty);
        }

        protected override void ValidatePropertyType(BindingDiagnosticBag diagnostics)
        {
            base.ValidatePropertyType(diagnostics);
            VerifyOverridesEqualityContractFromBase(this, diagnostics);
        }

        internal static void VerifyOverridesEqualityContractFromBase(PropertySymbol overriding, BindingDiagnosticBag diagnostics)
        {
            if (overriding.ContainingType.BaseTypeNoUseSiteDiagnostics.IsObjectType())
            {
                return;
            }
            bool flag = false;
            if (!overriding.IsOverride)
            {
                flag = true;
            }
            else
            {
                PropertySymbol overriddenProperty = overriding.OverriddenProperty;
                if ((object)overriddenProperty != null && !overriddenProperty.ContainingType.Equals(overriding.ContainingType.BaseTypeNoUseSiteDiagnostics, TypeCompareKind.AllIgnoreOptions))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                diagnostics.Add(ErrorCode.ERR_DoesNotOverrideBaseEqualityContract, overriding.Locations[0], overriding, overriding.ContainingType.BaseTypeNoUseSiteDiagnostics);
            }
        }
    }
}
