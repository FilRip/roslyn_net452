using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SynthesizedRecordPropertySymbol : SourcePropertySymbolBase
    {
        public SourceParameterSymbol BackingParameter { get; }

        public override IAttributeTargetSymbol AttributesOwner => (BackingParameter as IAttributeTargetSymbol) ?? this;

        protected override Location TypeLocation => ((ParameterSyntax)base.CSharpSyntaxNode).Type!.Location;

        public override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => BackingParameter.AttributeDeclarationList;

        protected override bool HasPointerTypeSyntactically => TypeWithAnnotations.DefaultType.IsPointerOrFunctionPointer();

        public SynthesizedRecordPropertySymbol(SourceMemberContainerTypeSymbol containingType, CSharpSyntaxNode syntax, ParameterSymbol backingParameter, bool isOverride, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax, hasGetAccessor: true, hasSetAccessor: true, isExplicitInterfaceImplementation: false, null, null, DeclarationModifiers.Public | (isOverride ? DeclarationModifiers.Override : DeclarationModifiers.None), hasInitializer: true, isAutoProperty: true, isExpressionBodied: false, ShouldUseInit(containingType), RefKind.None, backingParameter.Name, default(SyntaxList<AttributeListSyntax>), backingParameter.Locations[0], diagnostics)
        {
            BackingParameter = (SourceParameterSymbol)backingParameter;
        }

        public override SourcePropertyAccessorSymbol CreateGetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            return CreateAccessorSymbol(isGet: true, base.CSharpSyntaxNode, diagnostics);
        }

        public override SourcePropertyAccessorSymbol CreateSetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            return CreateAccessorSymbol(isGet: false, base.CSharpSyntaxNode, diagnostics);
        }

        private static bool ShouldUseInit(TypeSymbol container)
        {
            if (container.IsStructType())
            {
                return container.IsReadOnly;
            }
            return true;
        }

        private SourcePropertyAccessorSymbol CreateAccessorSymbol(bool isGet, CSharpSyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            bool usesInit = !isGet && ShouldUseInit(ContainingType);
            return SourcePropertyAccessorSymbol.CreateAccessorSymbol(isGet, usesInit, ContainingType, this, _modifiers, ((ParameterSyntax)syntax).Identifier.GetLocation(), syntax, diagnostics);
        }

        protected override (TypeWithAnnotations Type, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindType(BindingDiagnosticBag diagnostics)
        {
            return (BackingParameter.TypeWithAnnotations, ImmutableArray<ParameterSymbol>.Empty);
        }

        public static bool HaveCorrespondingSynthesizedRecordPropertySymbol(SourceParameterSymbol parameter)
        {
            if (parameter.ContainingSymbol is SynthesizedRecordConstructor)
            {
                return parameter.ContainingType.GetMembersUnordered().Any((Symbol s, SourceParameterSymbol parameter) => (object)(s as SynthesizedRecordPropertySymbol)?.BackingParameter == parameter, parameter);
            }
            return false;
        }
    }
}
