using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceConstructorSymbol : SourceConstructorSymbolBase
    {
        private readonly bool _isExpressionBodied;

        private readonly bool _hasThisInitializer;

        internal override bool IsExpressionBodied => _isExpressionBodied;

        protected override bool AllowRefOrOut => true;

        public static SourceConstructorSymbol CreateConstructorSymbol(SourceMemberContainerTypeSymbol containingType, ConstructorDeclarationSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
        {
            MethodKind methodKind = ((!syntax.Modifiers.Any(SyntaxKind.StaticKeyword)) ? MethodKind.Constructor : MethodKind.StaticConstructor);
            return new SourceConstructorSymbol(containingType, syntax.Identifier.GetLocation(), syntax, methodKind, isNullableAnalysisEnabled, diagnostics);
        }

        private SourceConstructorSymbol(SourceMemberContainerTypeSymbol containingType, Location location, ConstructorDeclarationSyntax syntax, MethodKind methodKind, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(containingType, location, syntax, SyntaxFacts.HasYieldOperations(syntax))
        {
            bool flag = syntax.Body != null;
            _isExpressionBodied = !flag && syntax.ExpressionBody != null;
            bool flag2 = flag || _isExpressionBodied;
            ConstructorInitializerSyntax? initializer = syntax.Initializer;
            _hasThisInitializer = initializer != null && initializer!.Kind() == SyntaxKind.ThisConstructorInitializer;
            DeclarationModifiers declarationModifiers = MakeModifiers(syntax.Modifiers, methodKind, flag2, location, diagnostics, out bool modifierErrors);
            MakeFlags(methodKind, declarationModifiers, returnsVoid: true, isExtensionMethod: false, isNullableAnalysisEnabled);
            if (syntax.Identifier.ValueText != containingType.Name)
            {
                diagnostics.Add(ErrorCode.ERR_MemberNeedsType, location);
            }
            if (IsExtern)
            {
                if (methodKind == MethodKind.Constructor && syntax.Initializer != null)
                {
                    diagnostics.Add(ErrorCode.ERR_ExternHasConstructorInitializer, location, this);
                }
                if (flag2)
                {
                    diagnostics.Add(ErrorCode.ERR_ExternHasBody, location, this);
                }
            }
            if (methodKind == MethodKind.StaticConstructor)
            {
                CheckFeatureAvailabilityAndRuntimeSupport(syntax, location, flag2, diagnostics);
            }
            CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(DeclarationModifiers, this, isExplicitInterfaceImplementation: false);
            if (cSDiagnosticInfo != null)
            {
                diagnostics.Add(cSDiagnosticInfo, location);
            }
            if (!modifierErrors)
            {
                CheckModifiers(methodKind, flag2, location, diagnostics);
            }
            Symbol.CheckForBlockAndExpressionBody(syntax.Body, syntax.ExpressionBody, syntax, diagnostics);
        }

        internal ConstructorDeclarationSyntax GetSyntax()
        {
            return (ConstructorDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
        }

        protected override ParameterListSyntax GetParameterList()
        {
            return GetSyntax().ParameterList;
        }

        protected override CSharpSyntaxNode GetInitializer()
        {
            return GetSyntax().Initializer;
        }

        private DeclarationModifiers MakeModifiers(SyntaxTokenList modifiers, MethodKind methodKind, bool hasBody, Location location, BindingDiagnosticBag diagnostics, out bool modifierErrors)
        {
            DeclarationModifiers defaultAccess = ((methodKind != MethodKind.StaticConstructor) ? DeclarationModifiers.Private : DeclarationModifiers.None);
            DeclarationModifiers declarationModifiers = ModifierUtils.MakeAndCheckNontypeMemberModifiers(modifiers, defaultAccess, DeclarationModifiers.AccessibilityMask | DeclarationModifiers.Static | DeclarationModifiers.Extern | DeclarationModifiers.Unsafe, location, diagnostics, out modifierErrors);
            this.CheckUnsafeModifier(declarationModifiers, diagnostics);
            if (methodKind == MethodKind.StaticConstructor)
            {
                if ((declarationModifiers & DeclarationModifiers.AccessibilityMask) != 0 && ContainingType.Name == ((ConstructorDeclarationSyntax)SyntaxNode).Identifier.ValueText)
                {
                    diagnostics.Add(ErrorCode.ERR_StaticConstructorWithAccessModifiers, location, this);
                    declarationModifiers &= ~DeclarationModifiers.AccessibilityMask;
                    modifierErrors = true;
                }
                declarationModifiers |= DeclarationModifiers.Private;
                if (ContainingType.IsInterface)
                {
                    ModifierUtils.ReportDefaultInterfaceImplementationModifiers(hasBody, declarationModifiers, DeclarationModifiers.Extern, location, diagnostics);
                }
            }
            return declarationModifiers;
        }

        private void CheckModifiers(MethodKind methodKind, bool hasBody, Location location, BindingDiagnosticBag diagnostics)
        {
            if (!hasBody && !IsExtern)
            {
                diagnostics.Add(ErrorCode.ERR_ConcreteMissingBody, location, this);
            }
            else if (ContainingType.IsSealed && DeclaredAccessibility.HasProtected() && !IsOverride)
            {
                diagnostics.Add(AccessCheck.GetProtectedMemberInSealedTypeError(ContainingType), location, this);
            }
            else if (ContainingType.IsStatic && methodKind == MethodKind.Constructor)
            {
                diagnostics.Add(ErrorCode.ERR_ConstructorInStaticClass, location);
            }
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(((ConstructorDeclarationSyntax)SyntaxNode).AttributeLists);
        }

        internal override bool IsNullableAnalysisEnabled()
        {
            if (!_hasThisInitializer)
            {
                return ((SourceMemberContainerTypeSymbol)ContainingType).IsNullableEnabledForConstructorsAndInitializers(IsStatic);
            }
            return flags.IsNullableAnalysisEnabled;
        }

        protected override bool IsWithinExpressionOrBlockBody(int position, out int offset)
        {
            ConstructorDeclarationSyntax syntax = GetSyntax();
            BlockSyntax? body = syntax.Body;
            if (body != null && body!.Span.Contains(position))
            {
                offset = position - syntax.Body!.Span.Start;
                return true;
            }
            ArrowExpressionClauseSyntax? expressionBody = syntax.ExpressionBody;
            if (expressionBody != null && expressionBody!.Span.Contains(position))
            {
                offset = position - syntax.ExpressionBody!.Span.Start;
                return true;
            }
            offset = -1;
            return false;
        }
    }
}
