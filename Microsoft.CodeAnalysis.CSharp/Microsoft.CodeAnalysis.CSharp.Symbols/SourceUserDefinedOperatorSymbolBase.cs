using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SourceUserDefinedOperatorSymbolBase : SourceMemberMethodSymbol
    {
        private const TypeCompareKind ComparisonForUserDefinedOperators = TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes;

        private readonly string _name;

        private readonly bool _isExpressionBodied;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private TypeWithAnnotations _lazyReturnType;

        protected abstract Location ReturnTypeLocation { get; }

        public sealed override string Name => _name;

        public sealed override bool ReturnsVoid
        {
            get
            {
                LazyMethodChecks();
                return base.ReturnsVoid;
            }
        }

        public sealed override bool IsVararg => false;

        public sealed override bool IsExtensionMethod => false;

        public sealed override ImmutableArray<Location> Locations => locations;

        internal sealed override int ParameterCount
        {
            get
            {
                if (!_lazyParameters.IsDefault)
                {
                    return _lazyParameters.Length;
                }
                return GetParameterCountFromSyntax();
            }
        }

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                LazyMethodChecks();
                return _lazyParameters;
            }
        }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public sealed override RefKind RefKind => RefKind.None;

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        internal sealed override bool IsExpressionBodied => _isExpressionBodied;

        protected SourceUserDefinedOperatorSymbolBase(MethodKind methodKind, string name, SourceMemberContainerTypeSymbol containingType, Location location, CSharpSyntaxNode syntax, DeclarationModifiers declarationModifiers, bool hasBody, bool isExpressionBodied, bool isIterator, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax.GetReference(), location, isIterator)
        {
            _name = name;
            _isExpressionBodied = isExpressionBodied;
            this.CheckUnsafeModifier(declarationModifiers, diagnostics);
            MakeFlags(methodKind, declarationModifiers, returnsVoid: false, isExtensionMethod: false, isNullableAnalysisEnabled);
            if (ContainingType.IsInterface && (methodKind == MethodKind.Conversion || name == "op_Equality" || name == "op_Inequality"))
            {
                return;
            }
            if (ContainingType.IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_OperatorInStaticClass, location, this);
                return;
            }
            if (DeclaredAccessibility != Accessibility.Public || !IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_OperatorsMustBeStatic, Locations[0], this);
            }
            if (hasBody && IsExtern)
            {
                diagnostics.Add(ErrorCode.ERR_ExternHasBody, location, this);
            }
            else if (!hasBody && !IsExtern && !IsAbstract && !base.IsPartial)
            {
                diagnostics.Add(ErrorCode.ERR_ConcreteMissingBody, location, this);
            }
            CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(DeclarationModifiers, this, isExplicitInterfaceImplementation: false);
            if (cSDiagnosticInfo != null)
            {
                diagnostics.Add(cSDiagnosticInfo, location);
            }
        }

        protected static DeclarationModifiers MakeDeclarationModifiers(BaseMethodDeclarationSyntax syntax, Location location, BindingDiagnosticBag diagnostics)
        {
            DeclarationModifiers defaultAccess = DeclarationModifiers.Private;
            DeclarationModifiers allowedModifiers = DeclarationModifiers.AccessibilityMask | DeclarationModifiers.Static | DeclarationModifiers.Extern | DeclarationModifiers.Unsafe;
            return ModifierUtils.MakeAndCheckNontypeMemberModifiers(syntax.Modifiers, defaultAccess, allowedModifiers, location, diagnostics, out bool modifierErrors);
        }

        protected (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindReturnType(BaseMethodDeclarationSyntax declarationSyntax, TypeSyntax returnTypeSyntax, BindingDiagnosticBag diagnostics)
        {
            Binder binder = DeclaringCompilation.GetBinderFactory(declarationSyntax.SyntaxTree).GetBinder(returnTypeSyntax, declarationSyntax, this).WithAdditionalFlags(BinderFlags.SuppressConstraintChecks);
            ImmutableArray<ParameterSymbol> item = ParameterHelpers.MakeParameters(binder, this, declarationSyntax.ParameterList, out SyntaxToken arglistToken, diagnostics, allowRefOrOut: true, allowThis: false, addRefReadOnlyModifier: false);
            if (arglistToken.Kind() == SyntaxKind.ArgListKeyword)
            {
                diagnostics.Add(ErrorCode.ERR_IllegalVarArgs, new SourceLocation(in arglistToken));
            }
            TypeWithAnnotations item2 = binder.BindType(returnTypeSyntax, diagnostics);
            if (item2.IsRestrictedType(ignoreSpanLikeTypes: true))
            {
                diagnostics.Add(ErrorCode.ERR_MethodReturnCantBeRefAny, returnTypeSyntax.Location, item2.Type);
            }
            if (item2.Type.IsStatic)
            {
                diagnostics.Add(ErrorFacts.GetStaticClassReturnCode(useWarning: false), returnTypeSyntax.Location, item2.Type);
            }
            return (item2, item);
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            (_lazyReturnType, _lazyParameters) = MakeParametersAndBindReturnType(diagnostics);
            SetReturnsVoid(_lazyReturnType.IsVoidType());
            if ((!ContainingType.IsInterfaceType() || (MethodKind != MethodKind.Conversion && !(Name == "op_Equality") && !(Name == "op_Inequality"))) && !ContainingType.IsStatic)
            {
                CheckEffectiveAccessibility(_lazyReturnType, _lazyParameters, diagnostics);
                CheckValueParameters(diagnostics);
                CheckOperatorSignatures(diagnostics);
            }
        }

        protected abstract (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics);

        private void CheckValueParameters(BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (current.RefKind != 0 && current.RefKind != RefKind.In)
                {
                    diagnostics.Add(ErrorCode.ERR_IllegalRefParam, Locations[0]);
                    break;
                }
            }
        }

        private void CheckOperatorSignatures(BindingDiagnosticBag diagnostics)
        {
            if (DoesOperatorHaveCorrectArity(Name, ParameterCount))
            {
                switch (Name)
                {
                    case "op_Implicit":
                    case "op_Explicit":
                        CheckUserDefinedConversionSignature(diagnostics);
                        break;
                    case "op_UnaryNegation":
                    case "op_UnaryPlus":
                    case "op_LogicalNot":
                    case "op_OnesComplement":
                        CheckUnarySignature(diagnostics);
                        break;
                    case "op_True":
                    case "op_False":
                        CheckTrueFalseSignature(diagnostics);
                        break;
                    case "op_Increment":
                    case "op_Decrement":
                        CheckIncrementDecrementSignature(diagnostics);
                        break;
                    case "op_LeftShift":
                    case "op_RightShift":
                        CheckShiftSignature(diagnostics);
                        break;
                    default:
                        CheckBinarySignature(diagnostics);
                        break;
                }
            }
        }

        private static bool DoesOperatorHaveCorrectArity(string name, int parameterCount)
        {
            switch (name)
            {
                case "op_Increment":
                case "op_Decrement":
                case "op_UnaryNegation":
                case "op_UnaryPlus":
                case "op_LogicalNot":
                case "op_OnesComplement":
                case "op_True":
                case "op_False":
                case "op_Implicit":
                case "op_Explicit":
                    return parameterCount == 1;
                default:
                    return parameterCount == 2;
            }
        }

        private void CheckUserDefinedConversionSignature(BindingDiagnosticBag diagnostics)
        {
            if (ReturnsVoid)
            {
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, Locations[0]);
            }
            TypeSymbol parameterType = GetParameterType(0);
            TypeSymbol returnType = base.ReturnType;
            TypeSymbol typeSymbol = parameterType.StrippedType();
            TypeSymbol typeSymbol2 = returnType.StrippedType();
            if (typeSymbol.IsInterfaceType() || typeSymbol2.IsInterfaceType())
            {
                diagnostics.Add(ErrorCode.ERR_ConversionWithInterface, Locations[0], this);
                return;
            }
            if (!MatchesContainingType(typeSymbol) && !MatchesContainingType(typeSymbol2) && !MatchesContainingType(parameterType) && !MatchesContainingType(returnType))
            {
                diagnostics.Add(ErrorCode.ERR_ConversionNotInvolvingContainedType, Locations[0]);
                return;
            }
            if ((ContainingType.SpecialType == SpecialType.System_Nullable_T) ? parameterType.Equals(returnType, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes) : typeSymbol.Equals(typeSymbol2, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                diagnostics.Add(ErrorCode.ERR_IdentityConversion, Locations[0]);
                return;
            }
            if (parameterType.IsDynamic() || returnType.IsDynamic())
            {
                diagnostics.Add(ErrorCode.ERR_BadDynamicConversion, Locations[0], this);
                return;
            }
            TypeSymbol typeSymbol3;
            TypeSymbol typeSymbol4;
            if (MatchesContainingType(typeSymbol))
            {
                typeSymbol3 = parameterType;
                typeSymbol4 = returnType;
            }
            else
            {
                typeSymbol3 = returnType;
                typeSymbol4 = parameterType;
            }
            if (typeSymbol4.IsClassType())
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
                if (typeSymbol3.IsDerivedFrom(typeSymbol4, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.ERR_ConversionWithBase, Locations[0], this);
                }
                else if (typeSymbol4.IsDerivedFrom(typeSymbol3, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.ERR_ConversionWithDerived, Locations[0], this);
                }
                diagnostics.Add(Locations[0], useSiteInfo);
            }
        }

        private void CheckUnarySignature(BindingDiagnosticBag diagnostics)
        {
            if (!MatchesContainingType(GetParameterType(0).StrippedType()))
            {
                diagnostics.Add(ErrorCode.ERR_BadUnaryOperatorSignature, Locations[0]);
            }
            if (ReturnsVoid)
            {
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, Locations[0]);
            }
        }

        private void CheckTrueFalseSignature(BindingDiagnosticBag diagnostics)
        {
            if (base.ReturnType.SpecialType != SpecialType.System_Boolean)
            {
                diagnostics.Add(ErrorCode.ERR_OpTFRetType, Locations[0]);
            }
            if (!MatchesContainingType(GetParameterType(0).StrippedType()))
            {
                diagnostics.Add(ErrorCode.ERR_BadUnaryOperatorSignature, Locations[0]);
            }
        }

        private void CheckIncrementDecrementSignature(BindingDiagnosticBag diagnostics)
        {
            TypeSymbol parameterType = GetParameterType(0);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
            if (!MatchesContainingType(parameterType.StrippedType()))
            {
                diagnostics.Add(ErrorCode.ERR_BadIncDecSignature, Locations[0]);
            }
            else if (!base.ReturnType.EffectiveTypeNoUseSiteDiagnostics.IsEqualToOrDerivedFrom(parameterType, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes, ref useSiteInfo))
            {
                diagnostics.Add(ErrorCode.ERR_BadIncDecRetType, Locations[0]);
            }
            diagnostics.Add(Locations[0], useSiteInfo);
        }

        private bool MatchesContainingType(TypeSymbol type)
        {
            return type.Equals(ContainingType, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);
        }

        private void CheckShiftSignature(BindingDiagnosticBag diagnostics)
        {
            if (!MatchesContainingType(GetParameterType(0).StrippedType()) || GetParameterType(1).StrippedType().SpecialType != SpecialType.System_Int32)
            {
                diagnostics.Add(ErrorCode.ERR_BadShiftOperatorSignature, Locations[0]);
            }
            if (ReturnsVoid)
            {
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, Locations[0]);
            }
        }

        private void CheckBinarySignature(BindingDiagnosticBag diagnostics)
        {
            if (!MatchesContainingType(GetParameterType(0).StrippedType()) && !MatchesContainingType(GetParameterType(1).StrippedType()))
            {
                diagnostics.Add(ErrorCode.ERR_BadBinaryOperatorSignature, Locations[0]);
            }
            if (ReturnsVoid)
            {
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, Locations[0]);
            }
        }

        protected abstract int GetParameterCountFromSyntax();

        public sealed override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public sealed override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        internal sealed override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            base.ReturnType.CheckAllConstraints(declaringCompilation, conversions, Locations[0], diagnostics);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                current.Type.CheckAllConstraints(declaringCompilation, conversions, current.Locations[0], diagnostics);
            }
            ParameterHelpers.EnsureIsReadOnlyAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            if (base.ReturnType.ContainsNativeInteger())
            {
                declaringCompilation.EnsureNativeIntegerAttributeExists(diagnostics, ReturnTypeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureNativeIntegerAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            if (declaringCompilation.ShouldEmitNullableAttributes(this) && ReturnTypeWithAnnotations.NeedsNullableAttribute())
            {
                declaringCompilation.EnsureNullableAttributeExists(diagnostics, ReturnTypeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureNullableAttributeExists(declaringCompilation, this, Parameters, diagnostics, modifyCompilation: true);
        }
    }
}
