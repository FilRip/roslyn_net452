using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceDestructorSymbol : SourceMemberMethodSymbol
    {
        private TypeWithAnnotations _lazyReturnType;

        private readonly bool _isExpressionBodied;

        public override bool IsVararg => false;

        internal override int ParameterCount => 0;

        public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override RefKind RefKind => RefKind.None;

        public override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        public override string Name => "Finalize";

        internal override bool IsExpressionBodied => _isExpressionBodied;

        internal override bool IsMetadataFinal => false;

        internal override bool GenerateDebugInfo => true;

        internal SourceDestructorSymbol(SourceMemberContainerTypeSymbol containingType, DestructorDeclarationSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax.GetReference(), syntax.Identifier.GetLocation(), SyntaxFacts.HasYieldOperations(syntax.Body))
        {
            Location location = Locations[0];
            DeclarationModifiers declarationModifiers = MakeModifiers(syntax.Modifiers, location, diagnostics, out bool modifierErrors);
            MakeFlags(MethodKind.Destructor, declarationModifiers, returnsVoid: true, isExtensionMethod: false, isNullableAnalysisEnabled);
            if (syntax.Identifier.ValueText != containingType.Name)
            {
                diagnostics.Add(ErrorCode.ERR_BadDestructorName, syntax.Identifier.GetLocation());
            }
            bool flag = syntax.Body != null;
            _isExpressionBodied = !flag && syntax.ExpressionBody != null;
            if ((flag || _isExpressionBodied) && IsExtern)
            {
                diagnostics.Add(ErrorCode.ERR_ExternHasBody, location, this);
            }
            if (!modifierErrors && !flag && !_isExpressionBodied && !IsExtern)
            {
                diagnostics.Add(ErrorCode.ERR_ConcreteMissingBody, location, this);
            }
            if (containingType.IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_DestructorInStaticClass, location, this);
            }
            else if (!containingType.IsReferenceType)
            {
                diagnostics.Add(ErrorCode.ERR_OnlyClassesCanContainDestructors, location, this);
            }
            Symbol.CheckForBlockAndExpressionBody(syntax.Body, syntax.ExpressionBody, syntax, diagnostics);
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            DestructorDeclarationSyntax syntax = GetSyntax();
            Binder binder = DeclaringCompilation.GetBinderFactory(syntaxReferenceOpt.SyntaxTree).GetBinder(syntax, syntax, this);
            _lazyReturnType = TypeWithAnnotations.Create(binder.GetSpecialType(SpecialType.System_Void, diagnostics, syntax));
        }

        internal DestructorDeclarationSyntax GetSyntax()
        {
            return (DestructorDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
        }

        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        private DeclarationModifiers MakeModifiers(SyntaxTokenList modifiers, Location location, BindingDiagnosticBag diagnostics, out bool modifierErrors)
        {
            DeclarationModifiers declarationModifiers = ModifierUtils.MakeAndCheckNontypeMemberModifiers(modifiers, DeclarationModifiers.None, DeclarationModifiers.Extern | DeclarationModifiers.Unsafe, location, diagnostics, out modifierErrors);
            this.CheckUnsafeModifier(declarationModifiers, diagnostics);
            return (declarationModifiers & ~DeclarationModifiers.AccessibilityMask) | DeclarationModifiers.Protected;
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(GetSyntax().AttributeLists);
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
        {
            return OneOrMany.Create(default(SyntaxList<AttributeListSyntax>));
        }

        internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return true;
        }

        internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return (object)ContainingType.BaseTypeNoUseSiteDiagnostics == null;
        }
    }
}
