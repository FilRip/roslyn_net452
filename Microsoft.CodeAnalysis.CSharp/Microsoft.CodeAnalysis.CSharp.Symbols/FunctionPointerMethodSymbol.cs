using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class FunctionPointerMethodSymbol : MethodSymbol
    {
        private readonly ImmutableArray<FunctionPointerParameterSymbol> _parameters;

        private ImmutableHashSet<CustomModifier>? _lazyCallingConventionModifiers;

        internal override ImmutableArray<NamedTypeSymbol> UnmanagedCallingConventionTypes
        {
            get
            {
                if (!CallingConvention.IsCallingConvention(CallingConvention.Unmanaged))
                {
                    return ImmutableArray<NamedTypeSymbol>.Empty;
                }
                ImmutableArray<CustomModifier> immutableArray = ((RefKind != 0) ? RefCustomModifiers : ReturnTypeWithAnnotations.CustomModifiers);
                if (immutableArray.IsEmpty)
                {
                    return ImmutableArray<NamedTypeSymbol>.Empty;
                }
                ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance(immutableArray.Length);
                ImmutableArray<CustomModifier>.Enumerator enumerator = immutableArray.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    CSharpCustomModifier cSharpCustomModifier = (CSharpCustomModifier)enumerator.Current;
                    if (FunctionPointerTypeSymbol.IsCallingConventionModifier(cSharpCustomModifier.ModifierSymbol))
                    {
                        instance.Add(cSharpCustomModifier.ModifierSymbol);
                    }
                }
                return instance.ToImmutableAndFree();
            }
        }

        internal override CallingConvention CallingConvention { get; }

        public override bool ReturnsVoid => ReturnTypeWithAnnotations.IsVoidType();

        public override RefKind RefKind { get; }

        public override TypeWithAnnotations ReturnTypeWithAnnotations { get; }

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters.Cast<FunctionPointerParameterSymbol, ParameterSymbol>();

        public override ImmutableArray<CustomModifier> RefCustomModifiers { get; }

        public override MethodKind MethodKind => MethodKind.FunctionPointerSignature;

        public override bool IsVararg => CallingConvention.IsCallingConvention(CallingConvention.ExtraArguments);

        public override Symbol? ContainingSymbol => null;

        public override int Arity => 0;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override bool IsExtensionMethod => false;

        public override bool HidesBaseMethodsByName => false;

        public override bool IsAsync => false;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        public override Symbol? AssociatedSymbol => null;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override bool IsStatic => false;

        public override bool IsVirtual => false;

        public override bool IsOverride => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsExtern => false;

        public override bool IsImplicitlyDeclared => true;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

        internal override bool HasSpecialName => false;

        internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

        internal override bool HasDeclarativeSecurity => false;

        internal override MarshalPseudoCustomAttributeData? ReturnValueMarshallingInformation => null;

        internal override bool RequiresSecurityObject => false;

        internal override bool IsDeclaredReadOnly => false;

        internal override bool IsInitOnly => false;

        public override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        internal override bool GenerateDebugInfo
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override ObsoleteAttributeData? ObsoleteAttributeData
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override bool AreLocalsZeroed
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public static FunctionPointerMethodSymbol CreateFromSource(FunctionPointerTypeSyntax syntax, Binder typeBinder, BindingDiagnosticBag diagnostics, ConsList<TypeSymbol> basesBeingResolved, bool suppressUseSiteDiagnostics)
        {
            ArrayBuilder<CustomModifier> instance = ArrayBuilder<CustomModifier>.GetInstance();
            CallingConvention callingConvention = getCallingConvention(typeBinder.Compilation, syntax.CallingConvention, instance, diagnostics);
            RefKind refKind = RefKind.None;
            TypeWithAnnotations typeWithAnnotations;
            if (syntax.ParameterList.Parameters.Count == 0)
            {
                typeWithAnnotations = TypeWithAnnotations.Create(typeBinder.CreateErrorType());
            }
            else
            {
                FunctionPointerParameterSyntax functionPointerParameterSyntax = syntax.ParameterList.Parameters[^1];
                SyntaxTokenList modifiers = functionPointerParameterSyntax.Modifiers;
                for (int i = 0; i < modifiers.Count; i++)
                {
                    SyntaxToken token = modifiers[i];
                    if (token.Kind() == SyntaxKind.RefKeyword)
                    {
                        if (refKind == RefKind.None)
                        {
                            if (modifiers.Count > i + 1 && modifiers[i + 1].Kind() == SyntaxKind.ReadOnlyKeyword)
                            {
                                i++;
                                refKind = RefKind.In;
                                instance.AddRange(ParameterHelpers.CreateInModifiers(typeBinder, diagnostics, functionPointerParameterSyntax));
                            }
                            else
                            {
                                refKind = RefKind.Ref;
                            }
                        }
                        else
                        {
                            diagnostics.Add(ErrorCode.ERR_DupReturnTypeMod, token.GetLocation(), token.Text);
                        }
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_InvalidFuncPointerReturnTypeModifier, token.GetLocation(), token.Text);
                    }
                }
                typeWithAnnotations = typeBinder.BindType(functionPointerParameterSyntax.Type, diagnostics, basesBeingResolved, suppressUseSiteDiagnostics);
                if (typeWithAnnotations.IsVoidType() && refKind != 0)
                {
                    diagnostics.Add(ErrorCode.ERR_NoVoidHere, functionPointerParameterSyntax.Location);
                }
                else if (typeWithAnnotations.IsStatic)
                {
                    diagnostics.Add(ErrorFacts.GetStaticClassReturnCode(useWarning: false), functionPointerParameterSyntax.Location, typeWithAnnotations);
                }
                else if (typeWithAnnotations.IsRestrictedType(ignoreSpanLikeTypes: true))
                {
                    diagnostics.Add(ErrorCode.ERR_MethodReturnCantBeRefAny, functionPointerParameterSyntax.Location, typeWithAnnotations);
                }
            }
            ImmutableArray<CustomModifier> refCustomModifiers = ImmutableArray<CustomModifier>.Empty;
            if (refKind != 0)
            {
                refCustomModifiers = instance.ToImmutableAndFree();
            }
            else
            {
                typeWithAnnotations = typeWithAnnotations.WithModifiers(instance.ToImmutableAndFree());
            }
            return new FunctionPointerMethodSymbol(callingConvention, refKind, typeWithAnnotations, refCustomModifiers, syntax, typeBinder, diagnostics, suppressUseSiteDiagnostics);
            static void checkUnmanagedSupport(CSharpCompilation compilation, Location errorLocation, BindingDiagnosticBag diagnostics)
            {
                if (!compilation.Assembly.RuntimeSupportsUnmanagedSignatureCallingConvention)
                {
                    diagnostics.Add(ErrorCode.ERR_RuntimeDoesNotSupportUnmanagedDefaultCallConv, errorLocation);
                }
            }
            static CallingConvention getCallingConvention(CSharpCompilation compilation, FunctionPointerCallingConventionSyntax? callingConventionSyntax, ArrayBuilder<CustomModifier> customModifiers, BindingDiagnosticBag diagnostics)
            {
                SyntaxKind? syntaxKind = callingConventionSyntax?.ManagedOrUnmanagedKeyword.Kind();
                switch (syntaxKind)
                {
                    case null:
                        return CallingConvention.Default;
                    case SyntaxKind.ManagedKeyword:
                        if (callingConventionSyntax!.UnmanagedCallingConventionList != null && !callingConventionSyntax!.ContainsDiagnostics)
                        {
                            diagnostics.Add(ErrorCode.ERR_CannotSpecifyManagedWithUnmanagedSpecifiers, callingConventionSyntax!.UnmanagedCallingConventionList!.GetLocation());
                        }
                        return CallingConvention.Default;
                    case SyntaxKind.UnmanagedKeyword:
                        {
                            FunctionPointerUnmanagedCallingConventionListSyntax unmanagedCallingConventionList = callingConventionSyntax!.UnmanagedCallingConventionList;
                            if (unmanagedCallingConventionList != null)
                            {
                                SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions = unmanagedCallingConventionList.CallingConventions;
                                switch (callingConventions.Count)
                                {
                                    case 1:
                                        return callingConventions[0].Name.ValueText switch
                                        {
                                            "Cdecl" => CallingConvention.CDecl,
                                            "Stdcall" => CallingConvention.Standard,
                                            "Thiscall" => CallingConvention.ThisCall,
                                            "Fastcall" => CallingConvention.FastCall,
                                            _ => handleSingleConvention(callingConventions[0], compilation, customModifiers, diagnostics),
                                        };
                                    case 0:
                                        if (!unmanagedCallingConventionList.ContainsDiagnostics)
                                        {
                                            diagnostics.Add(ErrorCode.ERR_InvalidFunctionPointerCallingConvention, unmanagedCallingConventionList.OpenBracketToken.GetLocation(), "");
                                        }
                                        return CallingConvention.Default;
                                    default:
                                        {
                                            SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> separatedSyntaxList = callingConventions;
                                            checkUnmanagedSupport(compilation, callingConventionSyntax!.ManagedOrUnmanagedKeyword.GetLocation(), diagnostics);
                                            SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax>.Enumerator enumerator = separatedSyntaxList.GetEnumerator();
                                            while (enumerator.MoveNext())
                                            {
                                                CustomModifier customModifier2 = handleIndividualUnrecognizedSpecifier(enumerator.Current, compilation, diagnostics);
                                                if (customModifier2 != null)
                                                {
                                                    customModifiers.Add(customModifier2);
                                                }
                                            }
                                            return CallingConvention.Unmanaged;
                                        }
                                }
                            }
                            checkUnmanagedSupport(compilation, callingConventionSyntax!.ManagedOrUnmanagedKeyword.GetLocation(), diagnostics);
                            return CallingConvention.Unmanaged;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(syntaxKind);
                }
            }
            static CustomModifier? handleIndividualUnrecognizedSpecifier(FunctionPointerUnmanagedCallingConventionSyntax specifier, CSharpCompilation compilation, BindingDiagnosticBag diagnostics)
            {
                string valueText = specifier.Name.ValueText;
                if (string.IsNullOrEmpty(valueText))
                {
                    return null;
                }
                string text = "CallConv" + valueText;
                MetadataTypeName emittedName = MetadataTypeName.FromNamespaceAndTypeName("System.Runtime.CompilerServices", text, useCLSCompliantNameArityEncoding: true, 0);
                NamedTypeSymbol namedTypeSymbol = compilation.Assembly.CorLibrary.LookupTopLevelMetadataType(ref emittedName, digThroughForwardedTypes: false);
                if (namedTypeSymbol is MissingMetadataTypeSymbol)
                {
                    namedTypeSymbol = new MissingMetadataTypeSymbol.TopLevel(namedTypeSymbol.ContainingModule, ref emittedName, new CSDiagnosticInfo(ErrorCode.ERR_TypeNotFound, text));
                }
                else if (namedTypeSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    diagnostics.Add(ErrorCode.ERR_TypeMustBePublic, specifier.GetLocation(), namedTypeSymbol);
                }
                diagnostics.Add(namedTypeSymbol.GetUseSiteInfo(), specifier.GetLocation());
                return CSharpCustomModifier.CreateOptional(namedTypeSymbol);
            }
            static CallingConvention handleSingleConvention(FunctionPointerUnmanagedCallingConventionSyntax specifier, CSharpCompilation compilation, ArrayBuilder<CustomModifier> customModifiers, BindingDiagnosticBag diagnostics)
            {
                checkUnmanagedSupport(compilation, specifier.GetLocation(), diagnostics);
                CustomModifier customModifier = handleIndividualUnrecognizedSpecifier(specifier, compilation, diagnostics);
                if (customModifier != null)
                {
                    customModifiers.Add(customModifier);
                }
                return CallingConvention.Unmanaged;
            }
        }

        internal static FunctionPointerMethodSymbol CreateFromPartsForTest(CallingConvention callingConvention, TypeWithAnnotations returnType, ImmutableArray<CustomModifier> refCustomModifiers, RefKind returnRefKind, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<ImmutableArray<CustomModifier>> parameterRefCustomModifiers, ImmutableArray<RefKind> parameterRefKinds, CSharpCompilation compilation)
        {
            return new FunctionPointerMethodSymbol(callingConvention, returnRefKind, returnType, refCustomModifiers, parameterTypes, parameterRefCustomModifiers, parameterRefKinds, compilation);
        }

        internal static FunctionPointerMethodSymbol CreateFromParts(CallingConvention callingConvention, ImmutableArray<CustomModifier> callingConventionModifiers, TypeWithAnnotations returnTypeWithAnnotations, RefKind returnRefKind, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, CSharpCompilation compilation)
        {
            ArrayBuilder<CustomModifier> instance = ArrayBuilder<CustomModifier>.GetInstance();
            if (!callingConventionModifiers.IsDefaultOrEmpty)
            {
                instance.AddRange(callingConventionModifiers);
            }
            ImmutableArray<CustomModifier> refCustomModifiers;
            if (returnRefKind == RefKind.None)
            {
                refCustomModifiers = ImmutableArray<CustomModifier>.Empty;
                returnTypeWithAnnotations = returnTypeWithAnnotations.WithModifiers(instance.ToImmutableAndFree());
            }
            else
            {
                CustomModifier customModifierForRefKind = GetCustomModifierForRefKind(returnRefKind, compilation);
                if (customModifierForRefKind != null)
                {
                    instance.Add(customModifierForRefKind);
                }
                refCustomModifiers = instance.ToImmutableAndFree();
            }
            return new FunctionPointerMethodSymbol(callingConvention, returnRefKind, returnTypeWithAnnotations, refCustomModifiers, parameterTypes, default(ImmutableArray<ImmutableArray<CustomModifier>>), parameterRefKinds, compilation);
        }

        private static CustomModifier? GetCustomModifierForRefKind(RefKind refKind, CSharpCompilation compilation)
        {
            NamedTypeSymbol namedTypeSymbol = refKind switch
            {
                RefKind.In => compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_InAttribute),
                RefKind.Out => compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_OutAttribute),
                _ => null,
            };
            if ((object)namedTypeSymbol == null)
            {
                return null;
            }
            return CSharpCustomModifier.CreateRequired(namedTypeSymbol);
        }

        public static FunctionPointerMethodSymbol CreateFromMetadata(CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> retAndParamTypes)
        {
            return new FunctionPointerMethodSymbol(callingConvention, retAndParamTypes);
        }

        public FunctionPointerMethodSymbol SubstituteParameterSymbols(TypeWithAnnotations substitutedReturnType, ImmutableArray<TypeWithAnnotations> substitutedParameterTypes, ImmutableArray<CustomModifier> refCustomModifiers = default(ImmutableArray<CustomModifier>), ImmutableArray<ImmutableArray<CustomModifier>> paramRefCustomModifiers = default(ImmutableArray<ImmutableArray<CustomModifier>>))
        {
            return new FunctionPointerMethodSymbol(CallingConvention, RefKind, substitutedReturnType, refCustomModifiers.IsDefault ? RefCustomModifiers : refCustomModifiers, Parameters, substitutedParameterTypes, paramRefCustomModifiers);
        }

        internal FunctionPointerMethodSymbol MergeEquivalentTypes(FunctionPointerMethodSymbol signature, VarianceKind variance)
        {
            VarianceKind variance2 = ((RefKind == RefKind.None) ? variance : VarianceKind.None);
            TypeWithAnnotations substitutedReturnType = ReturnTypeWithAnnotations.MergeEquivalentTypes(signature.ReturnTypeWithAnnotations, variance2);
            ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
            bool flag = false;
            if (_parameters.Length > 0)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(_parameters.Length);
                for (int num = 0; num < _parameters.Length; num++)
                {
                    FunctionPointerParameterSymbol functionPointerParameterSymbol = _parameters[num];
                    FunctionPointerParameterSymbol functionPointerParameterSymbol2 = signature._parameters[num];
                    RefKind refKind = functionPointerParameterSymbol.RefKind;
                    VarianceKind varianceKind;
                    if (variance != VarianceKind.Out)
                    {
                        if (variance != VarianceKind.In || refKind != 0)
                        {
                            goto IL_00ae;
                        }
                        varianceKind = VarianceKind.Out;
                    }
                    else
                    {
                        if (refKind != 0)
                        {
                            goto IL_00ae;
                        }
                        varianceKind = VarianceKind.In;
                    }
                    goto IL_00b1;
                IL_00ae:
                    varianceKind = VarianceKind.None;
                    goto IL_00b1;
                IL_00b1:
                    VarianceKind variance3 = varianceKind;
                    TypeWithAnnotations item = functionPointerParameterSymbol.TypeWithAnnotations.MergeEquivalentTypes(functionPointerParameterSymbol2.TypeWithAnnotations, variance3);
                    instance.Add(item);
                    if (!item.IsSameAs(functionPointerParameterSymbol.TypeWithAnnotations))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    substitutedParameterTypes = instance.ToImmutableAndFree();
                }
                else
                {
                    instance.Free();
                    substitutedParameterTypes = base.ParameterTypesWithAnnotations;
                }
            }
            if (flag || !substitutedReturnType.IsSameAs(ReturnTypeWithAnnotations))
            {
                return SubstituteParameterSymbols(substitutedReturnType, substitutedParameterTypes);
            }
            return this;
        }

        public FunctionPointerMethodSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            TypeWithAnnotations substitutedReturnType = transform(ReturnTypeWithAnnotations);
            ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
            bool flag = false;
            if (_parameters.Length > 0)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(_parameters.Length);
                ImmutableArray<FunctionPointerParameterSymbol>.Enumerator enumerator = _parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    FunctionPointerParameterSymbol current = enumerator.Current;
                    TypeWithAnnotations item = transform(current.TypeWithAnnotations);
                    instance.Add(item);
                    if (!item.IsSameAs(current.TypeWithAnnotations))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    substitutedParameterTypes = instance.ToImmutableAndFree();
                }
                else
                {
                    instance.Free();
                    substitutedParameterTypes = base.ParameterTypesWithAnnotations;
                }
            }
            if (flag || !substitutedReturnType.IsSameAs(ReturnTypeWithAnnotations))
            {
                return SubstituteParameterSymbols(substitutedReturnType, substitutedParameterTypes);
            }
            return this;
        }

        private FunctionPointerMethodSymbol(CallingConvention callingConvention, RefKind refKind, TypeWithAnnotations returnType, ImmutableArray<CustomModifier> refCustomModifiers, ImmutableArray<ParameterSymbol> originalParameters, ImmutableArray<TypeWithAnnotations> substitutedParameterTypes, ImmutableArray<ImmutableArray<CustomModifier>> substitutedRefCustomModifiers)
        {
            RefCustomModifiers = refCustomModifiers;
            CallingConvention = callingConvention;
            RefKind = refKind;
            ReturnTypeWithAnnotations = returnType;
            if (originalParameters.Length > 0)
            {
                ArrayBuilder<FunctionPointerParameterSymbol> instance = ArrayBuilder<FunctionPointerParameterSymbol>.GetInstance(originalParameters.Length);
                for (int i = 0; i < originalParameters.Length; i++)
                {
                    ParameterSymbol parameterSymbol = originalParameters[i];
                    TypeWithAnnotations typeWithAnnotations = substitutedParameterTypes[i];
                    ImmutableArray<CustomModifier> refCustomModifiers2 = (substitutedRefCustomModifiers.IsDefault ? parameterSymbol.RefCustomModifiers : substitutedRefCustomModifiers[i]);
                    instance.Add(new FunctionPointerParameterSymbol(typeWithAnnotations, parameterSymbol.RefKind, parameterSymbol.Ordinal, this, refCustomModifiers2));
                }
                _parameters = instance.ToImmutableAndFree();
            }
            else
            {
                _parameters = ImmutableArray<FunctionPointerParameterSymbol>.Empty;
            }
        }

        private FunctionPointerMethodSymbol(CallingConvention callingConvention, RefKind refKind, TypeWithAnnotations returnTypeWithAnnotations, ImmutableArray<CustomModifier> refCustomModifiers, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<ImmutableArray<CustomModifier>> parameterRefCustomModifiers, ImmutableArray<RefKind> parameterRefKinds, CSharpCompilation compilation)
        {
            RefCustomModifiers = (refCustomModifiers.IsDefault ? getCustomModifierArrayForRefKind(refKind, compilation) : refCustomModifiers);
            RefKind = refKind;
            CallingConvention = callingConvention;
            ReturnTypeWithAnnotations = returnTypeWithAnnotations;
            _parameters = parameterTypes.ZipAsArray(parameterRefKinds, (this, compilation, parameterRefCustomModifiers), delegate (TypeWithAnnotations type, RefKind refKind, int i, (FunctionPointerMethodSymbol Method, CSharpCompilation Comp, ImmutableArray<ImmutableArray<CustomModifier>> ParamRefCustomModifiers) arg)
            {
                ImmutableArray<CustomModifier> refCustomModifiers2 = (arg.ParamRefCustomModifiers.IsDefault ? getCustomModifierArrayForRefKind(refKind, arg.Comp) : arg.ParamRefCustomModifiers[i]);
                return new FunctionPointerParameterSymbol(type, refKind, i, arg.Method, refCustomModifiers2);
            });
            static ImmutableArray<CustomModifier> getCustomModifierArrayForRefKind(RefKind refKind, CSharpCompilation compilation)
            {
                CustomModifier customModifierForRefKind = GetCustomModifierForRefKind(refKind, compilation);
                if (customModifierForRefKind == null)
                {
                    return ImmutableArray<CustomModifier>.Empty;
                }
                return ImmutableArray.Create(customModifierForRefKind);
            }
        }

        private FunctionPointerMethodSymbol(CallingConvention callingConvention, RefKind refKind, TypeWithAnnotations returnType, ImmutableArray<CustomModifier> refCustomModifiers, FunctionPointerTypeSyntax syntax, Binder typeBinder, BindingDiagnosticBag diagnostics, bool suppressUseSiteDiagnostics)
        {
            RefCustomModifiers = refCustomModifiers;
            CallingConvention = callingConvention;
            RefKind = refKind;
            ReturnTypeWithAnnotations = returnType;
            _parameters = ((syntax.ParameterList.Parameters.Count > 1) ? ParameterHelpers.MakeFunctionPointerParameters(typeBinder, this, syntax.ParameterList.Parameters, diagnostics, suppressUseSiteDiagnostics) : ImmutableArray<FunctionPointerParameterSymbol>.Empty);
        }

        private FunctionPointerMethodSymbol(CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> retAndParamTypes)
        {
            ParamInfo<TypeSymbol> param2 = retAndParamTypes[0];
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations.Create(param2.Type, NullableAnnotation.Oblivious, CSharpCustomModifier.Convert(param2.CustomModifiers));
            RefCustomModifiers = CSharpCustomModifier.Convert(param2.RefCustomModifiers);
            CallingConvention = callingConvention;
            ReturnTypeWithAnnotations = typeWithAnnotations;
            RefKind = getRefKind(param2, RefCustomModifiers, RefKind.In, RefKind.Ref);
            _parameters = makeParametersFromMetadata(retAndParamTypes.RemoveAt(0).AsSpan(), this);
            static RefKind getRefKind(ParamInfo<TypeSymbol> param, ImmutableArray<CustomModifier> paramRefCustomMods, RefKind hasInRefKind, RefKind hasOutRefKind)
            {
                if (!param.IsByRef)
                {
                    return RefKind.None;
                }
                if (paramRefCustomMods.HasInAttributeModifier())
                {
                    return hasInRefKind;
                }
                if (paramRefCustomMods.HasOutAttributeModifier())
                {
                    return hasOutRefKind;
                }
                return RefKind.Ref;
            }
            static ImmutableArray<FunctionPointerParameterSymbol> makeParametersFromMetadata(ReadOnlySpan<ParamInfo<TypeSymbol>> parameterTypes, FunctionPointerMethodSymbol parent)
            {
                if (parameterTypes.Length > 0)
                {
                    ArrayBuilder<FunctionPointerParameterSymbol> instance = ArrayBuilder<FunctionPointerParameterSymbol>.GetInstance(parameterTypes.Length);
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        ParamInfo<TypeSymbol> param3 = parameterTypes[i];
                        ImmutableArray<CustomModifier> immutableArray = CSharpCustomModifier.Convert(param3.RefCustomModifiers);
                        TypeWithAnnotations typeWithAnnotations2 = TypeWithAnnotations.Create(param3.Type, NullableAnnotation.Oblivious, CSharpCustomModifier.Convert(param3.CustomModifiers));
                        RefKind refKind = getRefKind(param3, immutableArray, RefKind.In, RefKind.Out);
                        instance.Add(new FunctionPointerParameterSymbol(typeWithAnnotations2, refKind, i, parent, immutableArray));
                    }
                    return instance.ToImmutableAndFree();
                }
                return ImmutableArray<FunctionPointerParameterSymbol>.Empty;
            }
        }

        internal void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            ReturnTypeWithAnnotations.AddNullableTransforms(transforms);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.TypeWithAnnotations.AddNullableTransforms(transforms);
            }
        }

        internal FunctionPointerMethodSymbol ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position)
        {
            bool flag = ReturnTypeWithAnnotations.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position, out TypeWithAnnotations result);
            ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
            if (!Parameters.IsEmpty)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(Parameters.Length);
                bool flag2 = false;
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    flag2 |= current.TypeWithAnnotations.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position, out var result2);
                    instance.Add(result2);
                }
                if (flag2)
                {
                    substitutedParameterTypes = instance.ToImmutableAndFree();
                    flag = true;
                }
                else
                {
                    instance.Free();
                    substitutedParameterTypes = base.ParameterTypesWithAnnotations;
                }
            }
            if (flag)
            {
                return SubstituteParameterSymbols(result, substitutedParameterTypes);
            }
            return this;
        }

        public ImmutableHashSet<CustomModifier> GetCallingConventionModifiers()
        {
            if (_lazyCallingConventionModifiers == null)
            {
                ImmutableArray<CustomModifier> immutableArray = ((RefKind != 0) ? RefCustomModifiers : ReturnTypeWithAnnotations.CustomModifiers);
                if (immutableArray.IsEmpty || CallingConvention != CallingConvention.Unmanaged)
                {
                    _lazyCallingConventionModifiers = ImmutableHashSet<CustomModifier>.Empty;
                }
                else
                {
                    PooledHashSet<CustomModifier> instance = PooledHashSet<CustomModifier>.GetInstance();
                    ImmutableArray<CustomModifier>.Enumerator enumerator = immutableArray.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        CustomModifier current = enumerator.Current;
                        if (FunctionPointerTypeSymbol.IsCallingConventionModifier(((CSharpCustomModifier)current).ModifierSymbol))
                        {
                            instance.Add(current);
                        }
                    }
                    if (instance.Count == 0)
                    {
                        _lazyCallingConventionModifiers = ImmutableHashSet<CustomModifier>.Empty;
                    }
                    else
                    {
                        _lazyCallingConventionModifiers = instance.ToImmutableHashSet();
                    }
                    instance.Free();
                }
            }
            return _lazyCallingConventionModifiers;
        }

        public override bool Equals(Symbol other, TypeCompareKind compareKind)
        {
            if (!(other is FunctionPointerMethodSymbol other2))
            {
                return false;
            }
            return Equals(other2, compareKind);
        }

        internal bool Equals(FunctionPointerMethodSymbol other, TypeCompareKind compareKind)
        {
            if ((object)this != other)
            {
                if (EqualsNoParameters(other, compareKind))
                {
                    return _parameters.SequenceEqual(other._parameters, compareKind, (FunctionPointerParameterSymbol param1, FunctionPointerParameterSymbol param2, TypeCompareKind compareKind) => param1.MethodEqualityChecks(param2, compareKind));
                }
                return false;
            }
            return true;
        }

        private bool EqualsNoParameters(FunctionPointerMethodSymbol other, TypeCompareKind compareKind)
        {
            if (CallingConvention != other.CallingConvention || !FunctionPointerTypeSymbol.RefKindEquals(compareKind, RefKind, other.RefKind) || !ReturnTypeWithAnnotations.Equals(other.ReturnTypeWithAnnotations, compareKind))
            {
                return false;
            }
            if ((compareKind & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) != 0)
            {
                if (CallingConvention.IsCallingConvention(CallingConvention.Unmanaged) && !GetCallingConventionModifiers().SetEquals(other.GetCallingConventionModifiers()))
                {
                    return false;
                }
            }
            else if (!RefCustomModifiers.SequenceEqual(other.RefCustomModifiers))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int num = GetHashCodeNoParameters();
            ImmutableArray<FunctionPointerParameterSymbol>.Enumerator enumerator = _parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                num = Hash.Combine(enumerator.Current.MethodHashCode(), num);
            }
            return num;
        }

        internal int GetHashCodeNoParameters()
        {
            return Hash.Combine(base.ReturnType, Hash.Combine(CallingConvention.GetHashCode(), FunctionPointerTypeSymbol.GetRefKindForHashCode(RefKind).GetHashCode()));
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            UseSiteInfo<AssemblySymbol> result = default(UseSiteInfo<AssemblySymbol>);
            CalculateUseSiteDiagnostic(ref result);
            if (CallingConvention.IsCallingConvention(CallingConvention.ExtraArguments))
            {
                MergeUseSiteInfo(ref result, new UseSiteInfo<AssemblySymbol>(new CSDiagnosticInfo(ErrorCode.ERR_UnsupportedCallingConvention, this)));
            }
            return result;
        }

        internal bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo? result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            if (!base.ReturnType.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes) && !Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result, RefCustomModifiers, owner, ref checkedTypes))
            {
                return Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result, Parameters, owner, ref checkedTypes);
            }
            return true;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal sealed override UnmanagedCallersOnlyAttributeData? GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            return null;
        }

        public override DllImportData GetDllImportData()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override bool IsNullableAnalysisEnabled()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
