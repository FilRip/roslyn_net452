using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SourcePropertySymbol : SourcePropertySymbolBase
    {
        protected override Location TypeLocation => GetTypeSyntax(base.CSharpSyntaxNode).Location;

        public override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => ((BasePropertyDeclarationSyntax)base.CSharpSyntaxNode).AttributeLists;

        public override IAttributeTargetSymbol AttributesOwner => this;

        protected override bool HasPointerTypeSyntactically
        {
            get
            {
                return GetTypeSyntax(base.CSharpSyntaxNode).SkipRef(out RefKind refKind).Kind() switch
                {
                    SyntaxKind.PointerType => true,
                    SyntaxKind.FunctionPointerType => true,
                    _ => false,
                };
            }
        }

        internal static SourcePropertySymbol Create(SourceMemberContainerTypeSymbol containingType, Binder bodyBinder, PropertyDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            SyntaxToken identifier = syntax.Identifier;
            Location location = identifier.GetLocation();
            return Create(containingType, bodyBinder, syntax, identifier.ValueText, location, diagnostics);
        }

        internal static SourcePropertySymbol Create(SourceMemberContainerTypeSymbol containingType, Binder bodyBinder, IndexerDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            Location location = syntax.ThisKeyword.GetLocation();
            return Create(containingType, bodyBinder, syntax, "Item", location, diagnostics);
        }

        private static SourcePropertySymbol Create(SourceMemberContainerTypeSymbol containingType, Binder binder, BasePropertyDeclarationSyntax syntax, string name, Location location, BindingDiagnosticBag diagnostics)
        {
            GetAccessorDeclarations(syntax, diagnostics, out var isAutoProperty, out var hasAccessorList, out var accessorsHaveImplementation, out var isInitOnly, out var getSyntax, out var setSyntax);
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = SourcePropertySymbolBase.GetExplicitInterfaceSpecifier(syntax);
            SyntaxTokenList modifierTokensSyntax = GetModifierTokensSyntax(syntax);
            bool isExplicitInterfaceImplementation = explicitInterfaceSpecifier != null;
            DeclarationModifiers modifiers = MakeModifiers(containingType, modifierTokensSyntax, isExplicitInterfaceImplementation, syntax.Kind() == SyntaxKind.IndexerDeclaration, accessorsHaveImplementation, location, diagnostics, out bool modifierErrors);
            bool flag = !hasAccessorList && GetArrowExpression(syntax) != null;
            binder = binder.WithUnsafeRegionIfNecessary(modifierTokensSyntax);
            string memberNameAndInterfaceSymbol = ExplicitInterfaceHelpers.GetMemberNameAndInterfaceSymbol(binder, explicitInterfaceSpecifier, name, diagnostics, out TypeSymbol explicitInterfaceTypeOpt, out string aliasQualifierOpt);
            return new SourcePropertySymbol(containingType, syntax, getSyntax != null || flag, setSyntax != null, isExplicitInterfaceImplementation, explicitInterfaceTypeOpt, aliasQualifierOpt, modifiers, isAutoProperty, flag, isInitOnly, memberNameAndInterfaceSymbol, location, diagnostics);
        }

        private SourcePropertySymbol(SourceMemberContainerTypeSymbol containingType, BasePropertyDeclarationSyntax syntax, bool hasGetAccessor, bool hasSetAccessor, bool isExplicitInterfaceImplementation, TypeSymbol? explicitInterfaceType, string? aliasQualifierOpt, DeclarationModifiers modifiers, bool isAutoProperty, bool isExpressionBodied, bool isInitOnly, string memberName, Location location, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax, hasGetAccessor, hasSetAccessor, isExplicitInterfaceImplementation, explicitInterfaceType, aliasQualifierOpt, modifiers, HasInitializer(syntax), isAutoProperty, isExpressionBodied, isInitOnly, syntax.Type.GetRefKind(), memberName, syntax.AttributeLists, location, diagnostics)
        {
            if (base.IsAutoProperty)
            {
                Binder.CheckFeatureAvailability(syntax, (hasGetAccessor && !hasSetAccessor) ? MessageID.IDS_FeatureReadonlyAutoImplementedProperties : MessageID.IDS_FeatureAutoImplementedProperties, diagnostics, location);
            }
            Symbol.CheckForBlockAndExpressionBody(syntax.AccessorList, syntax.GetExpressionBodySyntax(), syntax, diagnostics);
        }

        private TypeSyntax GetTypeSyntax(SyntaxNode syntax)
        {
            return ((BasePropertyDeclarationSyntax)syntax).Type;
        }

        private static SyntaxTokenList GetModifierTokensSyntax(SyntaxNode syntax)
        {
            return ((BasePropertyDeclarationSyntax)syntax).Modifiers;
        }

        private static ArrowExpressionClauseSyntax? GetArrowExpression(SyntaxNode syntax)
        {
            if (!(syntax is PropertyDeclarationSyntax propertyDeclarationSyntax))
            {
                if (syntax is IndexerDeclarationSyntax indexerDeclarationSyntax)
                {
                    return indexerDeclarationSyntax.ExpressionBody;
                }
                throw ExceptionUtilities.UnexpectedValue(syntax.Kind());
            }
            return propertyDeclarationSyntax.ExpressionBody;
        }

        private static bool HasInitializer(SyntaxNode syntax)
        {
            if (syntax is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                return propertyDeclarationSyntax.Initializer != null;
            }
            return false;
        }

        private static void GetAccessorDeclarations(CSharpSyntaxNode syntaxNode, BindingDiagnosticBag diagnostics, out bool isAutoProperty, out bool hasAccessorList, out bool accessorsHaveImplementation, out bool isInitOnly, out CSharpSyntaxNode? getSyntax, out CSharpSyntaxNode? setSyntax)
        {
            BasePropertyDeclarationSyntax basePropertyDeclarationSyntax = (BasePropertyDeclarationSyntax)syntaxNode;
            isAutoProperty = true;
            hasAccessorList = basePropertyDeclarationSyntax.AccessorList != null;
            getSyntax = null;
            setSyntax = null;
            isInitOnly = false;
            if (hasAccessorList)
            {
                accessorsHaveImplementation = false;
                SyntaxList<AccessorDeclarationSyntax>.Enumerator enumerator = basePropertyDeclarationSyntax.AccessorList!.Accessors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AccessorDeclarationSyntax current = enumerator.Current;
                    switch (current.Kind())
                    {
                        case SyntaxKind.GetAccessorDeclaration:
                            if (getSyntax == null)
                            {
                                getSyntax = current;
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_DuplicateAccessor, current.Keyword.GetLocation());
                            }
                            break;
                        case SyntaxKind.SetAccessorDeclaration:
                        case SyntaxKind.InitAccessorDeclaration:
                            if (setSyntax == null)
                            {
                                setSyntax = current;
                                if (current.Keyword.IsKind(SyntaxKind.InitKeyword))
                                {
                                    isInitOnly = true;
                                }
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_DuplicateAccessor, current.Keyword.GetLocation());
                            }
                            break;
                        case SyntaxKind.AddAccessorDeclaration:
                        case SyntaxKind.RemoveAccessorDeclaration:
                            diagnostics.Add(ErrorCode.ERR_GetOrSetExpected, current.Keyword.GetLocation());
                            continue;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(current.Kind());
                        case SyntaxKind.UnknownAccessorDeclaration:
                            continue;
                    }
                    if (current.Body != null || current.ExpressionBody != null)
                    {
                        isAutoProperty = false;
                        accessorsHaveImplementation = true;
                    }
                }
            }
            else
            {
                isAutoProperty = false;
                accessorsHaveImplementation = GetArrowExpression(basePropertyDeclarationSyntax) != null;
            }
        }

        private static AccessorDeclarationSyntax GetGetAccessorDeclaration(BasePropertyDeclarationSyntax syntax)
        {
            SyntaxList<AccessorDeclarationSyntax>.Enumerator enumerator = syntax.AccessorList!.Accessors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccessorDeclarationSyntax current = enumerator.Current;
                if (current.Kind() == SyntaxKind.GetAccessorDeclaration)
                {
                    return current;
                }
            }
            throw ExceptionUtilities.Unreachable;
        }

        private static AccessorDeclarationSyntax GetSetAccessorDeclaration(BasePropertyDeclarationSyntax syntax)
        {
            SyntaxList<AccessorDeclarationSyntax>.Enumerator enumerator = syntax.AccessorList!.Accessors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccessorDeclarationSyntax current = enumerator.Current;
                SyntaxKind syntaxKind = current.Kind();
                if (syntaxKind == SyntaxKind.SetAccessorDeclaration || syntaxKind == SyntaxKind.InitAccessorDeclaration)
                {
                    return current;
                }
            }
            throw ExceptionUtilities.Unreachable;
        }

        private static DeclarationModifiers MakeModifiers(NamedTypeSymbol containingType, SyntaxTokenList modifiers, bool isExplicitInterfaceImplementation, bool isIndexer, bool accessorsHaveImplementation, Location location, BindingDiagnosticBag diagnostics, out bool modifierErrors)
        {
            bool isInterface = containingType.IsInterface;
            DeclarationModifiers defaultAccess = ((isInterface && !isExplicitInterfaceImplementation) ? DeclarationModifiers.Public : DeclarationModifiers.Private);
            DeclarationModifiers declarationModifiers = DeclarationModifiers.Unsafe;
            DeclarationModifiers declarationModifiers2 = DeclarationModifiers.None;
            if (!isExplicitInterfaceImplementation)
            {
                declarationModifiers |= DeclarationModifiers.AccessibilityMask | DeclarationModifiers.Abstract | DeclarationModifiers.Sealed | DeclarationModifiers.New | DeclarationModifiers.Virtual;
                if (!isIndexer)
                {
                    declarationModifiers |= DeclarationModifiers.Static;
                }
                if (!isInterface)
                {
                    declarationModifiers |= DeclarationModifiers.Override;
                }
                else
                {
                    defaultAccess = DeclarationModifiers.None;
                    declarationModifiers2 |= DeclarationModifiers.Abstract | DeclarationModifiers.Sealed | ((!isIndexer) ? DeclarationModifiers.Static : DeclarationModifiers.None) | DeclarationModifiers.Virtual | DeclarationModifiers.Extern | DeclarationModifiers.AccessibilityMask;
                }
            }
            else if (isInterface)
            {
                declarationModifiers |= DeclarationModifiers.Abstract;
            }
            if (containingType.IsStructType())
            {
                declarationModifiers |= DeclarationModifiers.ReadOnly;
            }
            declarationModifiers |= DeclarationModifiers.Extern;
            DeclarationModifiers declarationModifiers3 = ModifierUtils.MakeAndCheckNontypeMemberModifiers(modifiers, defaultAccess, declarationModifiers, location, diagnostics, out modifierErrors);
            containingType.CheckUnsafeModifier(declarationModifiers3, location, diagnostics);
            ModifierUtils.ReportDefaultInterfaceImplementationModifiers(accessorsHaveImplementation, declarationModifiers3, declarationModifiers2, location, diagnostics);
            if (isInterface)
            {
                declarationModifiers3 = ModifierUtils.AdjustModifiersForAnInterfaceMember(declarationModifiers3, accessorsHaveImplementation, isExplicitInterfaceImplementation);
            }
            if (isIndexer)
            {
                declarationModifiers3 |= DeclarationModifiers.Indexer;
            }
            return declarationModifiers3;
        }

        public override SourcePropertyAccessorSymbol CreateGetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            BasePropertyDeclarationSyntax basePropertyDeclarationSyntax = (BasePropertyDeclarationSyntax)base.CSharpSyntaxNode;
            ArrowExpressionClauseSyntax arrowExpression = GetArrowExpression(basePropertyDeclarationSyntax);
            if (basePropertyDeclarationSyntax.AccessorList == null && arrowExpression != null)
            {
                return CreateExpressionBodiedAccessor(arrowExpression, diagnostics);
            }
            return CreateAccessorSymbol(GetGetAccessorDeclaration(basePropertyDeclarationSyntax), isAutoPropertyAccessor, diagnostics);
        }

        public override SourcePropertyAccessorSymbol CreateSetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            BasePropertyDeclarationSyntax syntax = (BasePropertyDeclarationSyntax)base.CSharpSyntaxNode;
            return CreateAccessorSymbol(GetSetAccessorDeclaration(syntax), isAutoPropertyAccessor, diagnostics);
        }

        private SourcePropertyAccessorSymbol CreateAccessorSymbol(AccessorDeclarationSyntax syntax, bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            return SourcePropertyAccessorSymbol.CreateAccessorSymbol(ContainingType, this, _modifiers, syntax, isAutoPropertyAccessor, diagnostics);
        }

        private SourcePropertyAccessorSymbol CreateExpressionBodiedAccessor(ArrowExpressionClauseSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            return SourcePropertyAccessorSymbol.CreateAccessorSymbol(ContainingType, this, _modifiers, syntax, diagnostics);
        }

        private Binder CreateBinderForTypeAndParameters()
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            SyntaxTree syntaxTree = base.SyntaxTree;
            CSharpSyntaxNode cSharpSyntaxNode = base.CSharpSyntaxNode;
            Binder binder = declaringCompilation.GetBinderFactory(syntaxTree).GetBinder(cSharpSyntaxNode, cSharpSyntaxNode, this);
            SyntaxTokenList modifierTokensSyntax = GetModifierTokensSyntax(cSharpSyntaxNode);
            return binder.WithUnsafeRegionIfNecessary(modifierTokensSyntax).WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks, this);
        }

        protected override (TypeWithAnnotations Type, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindType(BindingDiagnosticBag diagnostics)
        {
            Binder binder = CreateBinderForTypeAndParameters();
            CSharpSyntaxNode cSharpSyntaxNode = base.CSharpSyntaxNode;
            return (ComputeType(binder, cSharpSyntaxNode, diagnostics), ComputeParameters(binder, cSharpSyntaxNode, diagnostics));
        }

        private TypeWithAnnotations ComputeType(Binder binder, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            TypeSyntax syntax2 = GetTypeSyntax(syntax).SkipRef(out RefKind refKind);
            TypeWithAnnotations typeWithAnnotations = binder.BindType(syntax2, diagnostics);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = binder.GetNewCompoundUseSiteInfo(diagnostics);
            if (GetExplicitInterfaceSpecifier() == null && !this.IsNoMoreVisibleThan(typeWithAnnotations, ref useSiteInfo))
            {
                diagnostics.Add(IsIndexer ? ErrorCode.ERR_BadVisIndexerReturn : ErrorCode.ERR_BadVisPropertyType, base.Location, this, typeWithAnnotations.Type);
            }
            diagnostics.Add(base.Location, useSiteInfo);
            if (typeWithAnnotations.IsVoidType())
            {
                ErrorCode code = (IsIndexer ? ErrorCode.ERR_IndexerCantHaveVoidType : ErrorCode.ERR_PropertyCantHaveVoidType);
                diagnostics.Add(code, base.Location, this);
            }
            return typeWithAnnotations;
        }

        private static ImmutableArray<ParameterSymbol> MakeParameters(Binder binder, SourcePropertySymbolBase owner, BaseParameterListSyntax? parameterSyntaxOpt, BindingDiagnosticBag diagnostics, bool addRefReadOnlyModifier)
        {
            if (parameterSyntaxOpt == null)
            {
                return ImmutableArray<ParameterSymbol>.Empty;
            }
            if (parameterSyntaxOpt!.Parameters.Count < 1)
            {
                diagnostics.Add(ErrorCode.ERR_IndexerNeedsParam, parameterSyntaxOpt!.GetLastToken().GetLocation());
            }
            bool addRefReadOnlyModifier2 = addRefReadOnlyModifier;
            ImmutableArray<ParameterSymbol> result = ParameterHelpers.MakeParameters(binder, owner, parameterSyntaxOpt, out SyntaxToken arglistToken, diagnostics, allowRefOrOut: false, allowThis: false, addRefReadOnlyModifier2);
            if (arglistToken.Kind() != 0)
            {
                diagnostics.Add(ErrorCode.ERR_IllegalVarArgs, arglistToken.GetLocation());
            }
            if (result.Length == 1 && !owner.IsExplicitInterfaceImplementation)
            {
                ParameterSyntax parameterSyntax = parameterSyntaxOpt!.Parameters[0];
                if (parameterSyntax.Default != null)
                {
                    SyntaxToken identifier = parameterSyntax.Identifier;
                    diagnostics.Add(ErrorCode.WRN_DefaultValueForUnconsumedLocation, identifier.GetLocation(), identifier.ValueText);
                }
            }
            return result;
        }

        private ImmutableArray<ParameterSymbol> ComputeParameters(Binder binder, CSharpSyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            BaseParameterListSyntax parameterListSyntax = GetParameterListSyntax(syntax);
            return MakeParameters(binder, this, parameterListSyntax, diagnostics, IsVirtual || IsAbstract);
        }

        internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            base.AfterAddingTypeMembersChecks(conversions, diagnostics);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (!IsExplicitInterfaceImplementation && !this.IsNoMoreVisibleThan(current.Type, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.ERR_BadVisIndexerParam, base.Location, this, current.Type);
                }
                else if ((object)SetMethod != null && current.Name == "value")
                {
                    diagnostics.Add(ErrorCode.ERR_DuplicateGeneratedName, current.Locations.FirstOrDefault() ?? base.Location, current.Name);
                }
            }
            diagnostics.Add(base.Location, useSiteInfo);
        }

        private static BaseParameterListSyntax? GetParameterListSyntax(CSharpSyntaxNode syntax)
        {
            return (syntax as IndexerDeclarationSyntax)?.ParameterList;
        }
    }
}
