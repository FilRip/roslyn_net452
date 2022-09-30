using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class ParameterHelpers
    {
        public static ImmutableArray<ParameterSymbol> MakeParameters(Binder binder, Symbol owner, BaseParameterListSyntax syntax, out SyntaxToken arglistToken, BindingDiagnosticBag diagnostics, bool allowRefOrOut, bool allowThis, bool addRefReadOnlyModifier)
        {
            return MakeParameters<ParameterSyntax, ParameterSymbol, Symbol>(binder, owner, syntax.Parameters, out arglistToken, diagnostics, allowRefOrOut, allowThis, addRefReadOnlyModifier, suppressUseSiteDiagnostics: false, syntax.Parameters.Count - 1, (Binder context, Symbol owner, TypeWithAnnotations parameterType, ParameterSyntax syntax, RefKind refKind, int ordinal, SyntaxToken paramsKeyword, SyntaxToken thisKeyword, bool addRefReadOnlyModifier, BindingDiagnosticBag declarationDiagnostics) => SourceParameterSymbol.Create(context, owner, parameterType, syntax, refKind, syntax.Identifier, ordinal, paramsKeyword.Kind() != SyntaxKind.None, ordinal == 0 && thisKeyword.Kind() != SyntaxKind.None, addRefReadOnlyModifier, declarationDiagnostics));
        }

        public static ImmutableArray<FunctionPointerParameterSymbol> MakeFunctionPointerParameters(Binder binder, FunctionPointerMethodSymbol owner, SeparatedSyntaxList<FunctionPointerParameterSyntax> parametersList, BindingDiagnosticBag diagnostics, bool suppressUseSiteDiagnostics)
        {
            return MakeParameters(binder, owner, parametersList, out SyntaxToken arglistToken, diagnostics, allowRefOrOut: true, allowThis: false, addRefReadOnlyModifier: true, suppressUseSiteDiagnostics, parametersList.Count - 2, delegate (Binder binder, FunctionPointerMethodSymbol owner, TypeWithAnnotations parameterType, FunctionPointerParameterSyntax syntax, RefKind refKind, int ordinal, SyntaxToken paramsKeyword, SyntaxToken thisKeyword, bool addRefReadOnlyModifier, BindingDiagnosticBag diagnostics)
            {
                ImmutableArray<CustomModifier> refCustomModifiers = refKind switch
                {
                    RefKind.In => CreateInModifiers(binder, diagnostics, syntax),
                    RefKind.Out => CreateOutModifiers(binder, diagnostics, syntax),
                    _ => ImmutableArray<CustomModifier>.Empty,
                };
                if (parameterType.IsVoidType())
                {
                    diagnostics.Add(ErrorCode.ERR_NoVoidParameter, syntax.Type.Location);
                }
                return new FunctionPointerParameterSymbol(parameterType, refKind, ordinal, owner, refCustomModifiers);
            }, parsingFunctionPointer: true);
        }

        private static ImmutableArray<TParameterSymbol> MakeParameters<TParameterSyntax, TParameterSymbol, TOwningSymbol>(Binder binder, TOwningSymbol owner, SeparatedSyntaxList<TParameterSyntax> parametersList, out SyntaxToken arglistToken, BindingDiagnosticBag diagnostics, bool allowRefOrOut, bool allowThis, bool addRefReadOnlyModifier, bool suppressUseSiteDiagnostics, int lastIndex, Func<Binder, TOwningSymbol, TypeWithAnnotations, TParameterSyntax, RefKind, int, SyntaxToken, SyntaxToken, bool, BindingDiagnosticBag, TParameterSymbol> parameterCreationFunc, bool parsingFunctionPointer = false) where TParameterSyntax : BaseParameterSyntax where TParameterSymbol : ParameterSymbol where TOwningSymbol : Symbol
        {
            arglistToken = default(SyntaxToken);
            int num = 0;
            int num2 = -1;
            ArrayBuilder<TParameterSymbol> instance = ArrayBuilder<TParameterSymbol>.GetInstance();
            ParameterSyntax parameterSyntax = null;
            SeparatedSyntaxList<TParameterSyntax>.Enumerator enumerator = parametersList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TParameterSyntax current = enumerator.Current;
                if (num > lastIndex)
                {
                    break;
                }
                CheckParameterModifiers(current, diagnostics, parsingFunctionPointer);
                RefKind modifiers = GetModifiers(current.Modifiers, out SyntaxToken refnessKeyword, out SyntaxToken paramsKeyword, out SyntaxToken thisKeyword);
                if (thisKeyword.Kind() != 0 && !allowThis)
                {
                    diagnostics.Add(ErrorCode.ERR_ThisInBadContext, thisKeyword.GetLocation());
                }
                if (current is ParameterSyntax parameterSyntax2)
                {
                    if (parameterSyntax == null && (parameterSyntax2.Modifiers.Any(SyntaxKind.ParamsKeyword) || parameterSyntax2.Identifier.Kind() == SyntaxKind.ArgListKeyword))
                    {
                        parameterSyntax = parameterSyntax2;
                    }
                    if (parameterSyntax2.IsArgList)
                    {
                        arglistToken = parameterSyntax2.Identifier;
                        if (paramsKeyword.Kind() != 0 || refnessKeyword.Kind() != 0 || thisKeyword.Kind() != 0)
                        {
                            diagnostics.Add(ErrorCode.ERR_IllegalVarArgs, arglistToken.GetLocation());
                        }
                        continue;
                    }
                    if (parameterSyntax2.Default != null && num2 == -1)
                    {
                        num2 = num;
                    }
                }
                TypeWithAnnotations arg = binder.BindType(current.Type, diagnostics, null, suppressUseSiteDiagnostics);
                if (!allowRefOrOut && (modifiers == RefKind.Ref || modifiers == RefKind.Out))
                {
                    diagnostics.Add(ErrorCode.ERR_IllegalRefParam, refnessKeyword.GetLocation());
                }
                TParameterSymbol val = parameterCreationFunc(binder, owner, arg, current, modifiers, num, paramsKeyword, thisKeyword, addRefReadOnlyModifier, diagnostics);
                ReportParameterErrors(owner, current, val, thisKeyword, paramsKeyword, num2, diagnostics);
                instance.Add(val);
                num++;
            }
            if (parameterSyntax != null && parameterSyntax != parametersList[lastIndex])
            {
                diagnostics.Add((parameterSyntax.Identifier.Kind() == SyntaxKind.ArgListKeyword) ? ErrorCode.ERR_VarargsLast : ErrorCode.ERR_ParamsLast, parameterSyntax.GetLocation());
            }
            ImmutableArray<TParameterSymbol> immutableArray = instance.ToImmutableAndFree();
            if (!parsingFunctionPointer)
            {
                MethodSymbol methodSymbol = owner as MethodSymbol;
                ImmutableArray<TypeParameterSymbol> typeParameters = methodSymbol?.TypeParameters ?? default(ImmutableArray<TypeParameterSymbol>);
                bool allowShadowingNames = binder.Compilation.IsFeatureEnabled(MessageID.IDS_FeatureNameShadowingInNestedFunctions) && (object)methodSymbol != null && methodSymbol.MethodKind == MethodKind.LocalFunction;
                binder.ValidateParameterNameConflicts(typeParameters, immutableArray.Cast<TParameterSymbol, ParameterSymbol>(), allowShadowingNames, diagnostics);
            }
            return immutableArray;
        }

        internal static void EnsureIsReadOnlyAttributeExists(CSharpCompilation compilation, ImmutableArray<ParameterSymbol> parameters, BindingDiagnosticBag diagnostics, bool modifyCompilation)
        {
            if (compilation == null)
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (current.RefKind == RefKind.In)
                {
                    compilation.EnsureIsReadOnlyAttributeExists(diagnostics, GetParameterLocation(current), modifyCompilation);
                }
            }
        }

        internal static void EnsureNativeIntegerAttributeExists(CSharpCompilation compilation, ImmutableArray<ParameterSymbol> parameters, BindingDiagnosticBag diagnostics, bool modifyCompilation)
        {
            if (compilation == null)
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (current.TypeWithAnnotations.ContainsNativeInteger())
                {
                    compilation.EnsureNativeIntegerAttributeExists(diagnostics, GetParameterLocation(current), modifyCompilation);
                }
            }
        }

        internal static void EnsureNullableAttributeExists(CSharpCompilation compilation, Symbol container, ImmutableArray<ParameterSymbol> parameters, BindingDiagnosticBag diagnostics, bool modifyCompilation)
        {
            if (compilation == null || parameters.Length <= 0 || !compilation.ShouldEmitNullableAttributes(container))
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (current.TypeWithAnnotations.NeedsNullableAttribute())
                {
                    compilation.EnsureNullableAttributeExists(diagnostics, GetParameterLocation(current), modifyCompilation);
                }
            }
        }

        private static Location GetParameterLocation(ParameterSymbol parameter)
        {
            return parameter.GetNonNullSyntaxNode().Location;
        }

        private static void CheckParameterModifiers(BaseParameterSyntax parameter, BindingDiagnosticBag diagnostics, bool parsingFunctionPointerParams)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            SyntaxTokenList.Enumerator enumerator = parameter.Modifiers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxToken current = enumerator.Current;
                switch (current.Kind())
                {
                    case SyntaxKind.ThisKeyword:
                        if (flag)
                        {
                            diagnostics.Add(ErrorCode.ERR_DupParamMod, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.ThisKeyword));
                        }
                        else if (flag3)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.ThisKeyword), SyntaxFacts.GetText(SyntaxKind.OutKeyword));
                        }
                        else if (flag4)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParamModThis, current.GetLocation());
                        }
                        else
                        {
                            flag = true;
                        }
                        continue;
                    case SyntaxKind.RefKeyword:
                        if (flag2)
                        {
                            diagnostics.Add(ErrorCode.ERR_DupParamMod, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.RefKeyword));
                        }
                        else if (flag4)
                        {
                            diagnostics.Add(ErrorCode.ERR_ParamsCantBeWithModifier, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.RefKeyword));
                        }
                        else if (flag3)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.RefKeyword), SyntaxFacts.GetText(SyntaxKind.OutKeyword));
                        }
                        else if (flag5)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.RefKeyword), SyntaxFacts.GetText(SyntaxKind.InKeyword));
                        }
                        else
                        {
                            flag2 = true;
                        }
                        continue;
                    case SyntaxKind.OutKeyword:
                        if (flag3)
                        {
                            diagnostics.Add(ErrorCode.ERR_DupParamMod, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.OutKeyword));
                        }
                        else if (flag)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.OutKeyword), SyntaxFacts.GetText(SyntaxKind.ThisKeyword));
                        }
                        else if (flag4)
                        {
                            diagnostics.Add(ErrorCode.ERR_ParamsCantBeWithModifier, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.OutKeyword));
                        }
                        else if (flag2)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.OutKeyword), SyntaxFacts.GetText(SyntaxKind.RefKeyword));
                        }
                        else if (flag5)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.OutKeyword), SyntaxFacts.GetText(SyntaxKind.InKeyword));
                        }
                        else
                        {
                            flag3 = true;
                        }
                        continue;
                    case SyntaxKind.ParamsKeyword:
                        if (!parsingFunctionPointerParams)
                        {
                            if (flag4)
                            {
                                diagnostics.Add(ErrorCode.ERR_DupParamMod, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.ParamsKeyword));
                            }
                            else if (flag)
                            {
                                diagnostics.Add(ErrorCode.ERR_BadParamModThis, current.GetLocation());
                            }
                            else if (flag2)
                            {
                                diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.ParamsKeyword), SyntaxFacts.GetText(SyntaxKind.RefKeyword));
                            }
                            else if (flag5)
                            {
                                diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.ParamsKeyword), SyntaxFacts.GetText(SyntaxKind.InKeyword));
                            }
                            else if (flag3)
                            {
                                diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.ParamsKeyword), SyntaxFacts.GetText(SyntaxKind.OutKeyword));
                            }
                            else
                            {
                                flag4 = true;
                            }
                            continue;
                        }
                        if (!parsingFunctionPointerParams)
                        {
                            break;
                        }
                        goto IL_04cc;
                    case SyntaxKind.InKeyword:
                        if (flag5)
                        {
                            diagnostics.Add(ErrorCode.ERR_DupParamMod, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.InKeyword));
                        }
                        else if (flag3)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.InKeyword), SyntaxFacts.GetText(SyntaxKind.OutKeyword));
                        }
                        else if (flag2)
                        {
                            diagnostics.Add(ErrorCode.ERR_BadParameterModifiers, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.InKeyword), SyntaxFacts.GetText(SyntaxKind.RefKeyword));
                        }
                        else if (flag4)
                        {
                            diagnostics.Add(ErrorCode.ERR_ParamsCantBeWithModifier, current.GetLocation(), SyntaxFacts.GetText(SyntaxKind.InKeyword));
                        }
                        else
                        {
                            flag5 = true;
                        }
                        continue;
                    case SyntaxKind.ReadOnlyKeyword:
                        {
                            if (!parsingFunctionPointerParams)
                            {
                                break;
                            }
                            goto IL_04cc;
                        }
                    IL_04cc:
                        diagnostics.Add(ErrorCode.ERR_BadFuncPointerParamModifier, current.GetLocation(), SyntaxFacts.GetText(current.Kind()));
                        continue;
                }
                throw ExceptionUtilities.UnexpectedValue(current.Kind());
            }
        }

        private static void ReportParameterErrors(Symbol owner, BaseParameterSyntax parameterSyntax, ParameterSymbol parameter, SyntaxToken thisKeyword, SyntaxToken paramsKeyword, int firstDefault, BindingDiagnosticBag diagnostics)
        {
            int ordinal = parameter.Ordinal;
            bool flag = parameterSyntax is ParameterSyntax parameterSyntax2 && parameterSyntax2.Default != null;
            if (thisKeyword.Kind() == SyntaxKind.ThisKeyword && ordinal != 0)
            {
                diagnostics.Add(ErrorCode.ERR_BadThisParam, thisKeyword.GetLocation(), owner.Name);
            }
            else if (parameter.IsParams && owner.IsOperator())
            {
                diagnostics.Add(ErrorCode.ERR_IllegalParams, paramsKeyword.GetLocation());
            }
            else if (parameter.IsParams && !parameter.TypeWithAnnotations.IsSZArray())
            {
                diagnostics.Add(ErrorCode.ERR_ParamsMustBeArray, paramsKeyword.GetLocation());
            }
            else if (parameter.TypeWithAnnotations.IsStatic)
            {
                diagnostics.Add(ErrorFacts.GetStaticClassParameterCode(parameter.ContainingSymbol.ContainingType?.IsInterfaceType() ?? false), owner.Locations.IsEmpty ? parameterSyntax.GetLocation() : owner.Locations[0], parameter.Type);
            }
            else if (firstDefault != -1 && ordinal > firstDefault && !flag && !parameter.IsParams)
            {
                Location location = ((ParameterSyntax)parameterSyntax).Identifier.GetNextToken(includeZeroWidth: true).GetLocation();
                diagnostics.Add(ErrorCode.ERR_DefaultValueBeforeRequiredValue, location);
            }
            else if (parameter.RefKind != 0 && parameter.TypeWithAnnotations.IsRestrictedType(ignoreSpanLikeTypes: true))
            {
                diagnostics.Add(ErrorCode.ERR_MethodArgCantBeRefAny, parameterSyntax.Location, parameter.Type);
            }
        }

        internal static bool ReportDefaultParameterErrors(Binder binder, Symbol owner, ParameterSyntax parameterSyntax, SourceParameterSymbol parameter, BoundExpression defaultExpression, BoundExpression convertedExpression, BindingDiagnosticBag diagnostics)
        {
            bool result = false;
            TypeSymbol type = parameter.Type;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = binder.GetNewCompoundUseSiteInfo(diagnostics);
            Conversion conversion = binder.Conversions.ClassifyImplicitConversionFromExpression(defaultExpression, type, ref useSiteInfo);
            diagnostics.Add(defaultExpression.Syntax, useSiteInfo);
            RefKind modifiers = GetModifiers(parameterSyntax.Modifiers, out SyntaxToken refnessKeyword, out SyntaxToken paramsKeyword, out SyntaxToken thisKeyword);
            if (modifiers == RefKind.Ref || modifiers == RefKind.Out)
            {
                diagnostics.Add(ErrorCode.ERR_RefOutDefaultValue, refnessKeyword.GetLocation());
                result = true;
            }
            else if (paramsKeyword.Kind() == SyntaxKind.ParamsKeyword)
            {
                diagnostics.Add(ErrorCode.ERR_DefaultValueForParamsParameter, paramsKeyword.GetLocation());
                result = true;
            }
            else if (thisKeyword.Kind() == SyntaxKind.ThisKeyword)
            {
                if (parameter.Ordinal == 0)
                {
                    diagnostics.Add(ErrorCode.ERR_DefaultValueForExtensionParameter, thisKeyword.GetLocation());
                    result = true;
                }
            }
            else if (!defaultExpression.HasAnyErrors && !IsValidDefaultValue(defaultExpression.IsImplicitObjectCreation() ? convertedExpression : defaultExpression))
            {
                diagnostics.Add(ErrorCode.ERR_DefaultValueMustBeConstant, parameterSyntax.Default!.Value.Location, parameterSyntax.Identifier.ValueText);
                result = true;
            }
            else if (!conversion.Exists || conversion.IsUserDefined || (conversion.IsIdentity && type.SpecialType == SpecialType.System_Object && defaultExpression.Type.IsDynamic()))
            {
                diagnostics.Add(ErrorCode.ERR_NoConversionForDefaultParam, parameterSyntax.Identifier.GetLocation(), defaultExpression.Display, type);
                result = true;
            }
            else if ((conversion.IsReference && (type.SpecialType == SpecialType.System_Object || type.Kind == SymbolKind.DynamicType) && (object)defaultExpression.Type != null && defaultExpression.Type!.SpecialType == SpecialType.System_String) || conversion.IsBoxing)
            {
                diagnostics.Add(ErrorCode.ERR_NotNullRefDefaultParameter, parameterSyntax.Identifier.GetLocation(), parameterSyntax.Identifier.ValueText, type);
                result = true;
            }
            else if (conversion.IsNullable && !defaultExpression.Type.IsNullableType() && !type.GetNullableUnderlyingType().IsEnumType() && !type.GetNullableUnderlyingType().IsIntrinsicType())
            {
                diagnostics.Add(ErrorCode.ERR_NoConversionForNubDefaultParam, parameterSyntax.Identifier.GetLocation(), defaultExpression.Type, parameterSyntax.Identifier.ValueText);
                result = true;
            }
            ConstantValueUtils.CheckLangVersionForConstantValue(convertedExpression, diagnostics);
            if (owner.IsExplicitInterfaceImplementation() || owner.IsPartialImplementation() || owner.IsOperator())
            {
                diagnostics.Add(ErrorCode.WRN_DefaultValueForUnconsumedLocation, parameterSyntax.Identifier.GetLocation(), parameterSyntax.Identifier.ValueText);
            }
            return result;
        }

        private static bool IsValidDefaultValue(BoundExpression expression)
        {
            if (expression.ConstantValue != null)
            {
                return true;
            }
            switch (expression.Kind)
            {
                case BoundKind.DefaultLiteral:
                case BoundKind.DefaultExpression:
                    return true;
                case BoundKind.ObjectCreationExpression:
                    return IsValidDefaultValue((BoundObjectCreationExpression)expression);
                default:
                    return false;
            }
        }

        private static bool IsValidDefaultValue(BoundObjectCreationExpression expression)
        {
            if (expression.Constructor.IsDefaultValueTypeConstructor())
            {
                return expression.InitializerExpressionOpt == null;
            }
            return false;
        }

        internal static MethodSymbol FindContainingGenericMethod(Symbol symbol)
        {
            Symbol symbol2 = symbol;
            while ((object)symbol2 != null)
            {
                if (symbol2.Kind == SymbolKind.Method)
                {
                    MethodSymbol methodSymbol = (MethodSymbol)symbol2;
                    if (methodSymbol.MethodKind != 0)
                    {
                        if (!methodSymbol.IsGenericMethod)
                        {
                            return null;
                        }
                        return methodSymbol;
                    }
                }
                symbol2 = symbol2.ContainingSymbol;
            }
            return null;
        }

        private static RefKind GetModifiers(SyntaxTokenList modifiers, out SyntaxToken refnessKeyword, out SyntaxToken paramsKeyword, out SyntaxToken thisKeyword)
        {
            RefKind refKind = RefKind.None;
            refnessKeyword = default(SyntaxToken);
            paramsKeyword = default(SyntaxToken);
            thisKeyword = default(SyntaxToken);
            SyntaxTokenList.Enumerator enumerator = modifiers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxToken current = enumerator.Current;
                switch (current.Kind())
                {
                    case SyntaxKind.OutKeyword:
                        if (refKind == RefKind.None)
                        {
                            refnessKeyword = current;
                            refKind = RefKind.Out;
                        }
                        break;
                    case SyntaxKind.RefKeyword:
                        if (refKind == RefKind.None)
                        {
                            refnessKeyword = current;
                            refKind = RefKind.Ref;
                        }
                        break;
                    case SyntaxKind.InKeyword:
                        if (refKind == RefKind.None)
                        {
                            refnessKeyword = current;
                            refKind = RefKind.In;
                        }
                        break;
                    case SyntaxKind.ParamsKeyword:
                        paramsKeyword = current;
                        break;
                    case SyntaxKind.ThisKeyword:
                        thisKeyword = current;
                        break;
                }
            }
            return refKind;
        }

        internal static ImmutableArray<CustomModifier> ConditionallyCreateInModifiers(RefKind refKind, bool addRefReadOnlyModifier, Binder binder, BindingDiagnosticBag diagnostics, SyntaxNode syntax)
        {
            if (addRefReadOnlyModifier && refKind == RefKind.In)
            {
                return CreateInModifiers(binder, diagnostics, syntax);
            }
            return ImmutableArray<CustomModifier>.Empty;
        }

        internal static ImmutableArray<CustomModifier> CreateInModifiers(Binder binder, BindingDiagnosticBag diagnostics, SyntaxNode syntax)
        {
            return CreateModifiers(WellKnownType.System_Runtime_InteropServices_InAttribute, binder, diagnostics, syntax);
        }

        internal static ImmutableArray<CustomModifier> CreateOutModifiers(Binder binder, BindingDiagnosticBag diagnostics, SyntaxNode syntax)
        {
            return CreateModifiers(WellKnownType.System_Runtime_InteropServices_OutAttribute, binder, diagnostics, syntax);
        }

        private static ImmutableArray<CustomModifier> CreateModifiers(WellKnownType modifier, Binder binder, BindingDiagnosticBag diagnostics, SyntaxNode syntax)
        {
            return ImmutableArray.Create(CSharpCustomModifier.CreateRequired(binder.GetWellKnownType(modifier, diagnostics, syntax)));
        }
    }
}
