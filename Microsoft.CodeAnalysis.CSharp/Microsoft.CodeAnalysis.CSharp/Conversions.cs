using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class Conversions : ConversionsBase
    {
        private readonly Binder _binder;

        private CSharpCompilation Compilation => _binder.Compilation;

        public Conversions(Binder binder)
            : this(binder, 0, includeNullability: false, null)
        {
        }

        private Conversions(Binder binder, int currentRecursionDepth, bool includeNullability, Conversions otherNullabilityOpt)
            : base(binder.Compilation.Assembly.CorLibrary, currentRecursionDepth, includeNullability, otherNullabilityOpt)
        {
            _binder = binder;
        }

        protected override ConversionsBase CreateInstance(int currentRecursionDepth)
        {
            return new Conversions(_binder, currentRecursionDepth, IncludeNullability, null);
        }

        protected override ConversionsBase WithNullabilityCore(bool includeNullability)
        {
            return new Conversions(_binder, currentRecursionDepth, includeNullability, this);
        }

        public override Conversion GetMethodGroupDelegateConversion(BoundMethodGroup source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!destination.IsDelegateType() && (destination.SpecialType != SpecialType.System_Delegate || !ConversionsBase.IsFeatureInferredDelegateTypeEnabled(source)))
            {
                return Conversion.NoConversion;
            }
            var (methodSymbol, isFunctionPointer, callingConventionInfo) = GetDelegateInvokeOrFunctionPointerMethodIfAvailable(source, destination, ref useSiteInfo);
            if ((object)methodSymbol == null)
            {
                return Conversion.NoConversion;
            }
            MethodGroupResolution methodGroupResolution = ResolveDelegateOrFunctionPointerMethodGroup(_binder, source, methodSymbol, isFunctionPointer, in callingConventionInfo, ref useSiteInfo);
            Conversion result = ((methodGroupResolution.IsEmpty || methodGroupResolution.HasAnyErrors) ? Conversion.NoConversion : ToConversion(methodGroupResolution.OverloadResolutionResult, methodGroupResolution.MethodGroup, methodSymbol.ParameterCount));
            methodGroupResolution.Free();
            return result;
        }

        public override Conversion GetMethodGroupFunctionPointerConversion(BoundMethodGroup source, FunctionPointerTypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Binder binder = _binder;
            FunctionPointerMethodSymbol signature = destination.Signature;
            CallingConventionInfo callingConventionInfo = new CallingConventionInfo(destination.Signature.CallingConvention, destination.Signature.GetCallingConventionModifiers());
            MethodGroupResolution methodGroupResolution = ResolveDelegateOrFunctionPointerMethodGroup(binder, source, signature, isFunctionPointer: true, in callingConventionInfo, ref useSiteInfo);
            Conversion result = ((methodGroupResolution.IsEmpty || methodGroupResolution.HasAnyErrors) ? Conversion.NoConversion : ToConversion(methodGroupResolution.OverloadResolutionResult, methodGroupResolution.MethodGroup, destination.Signature.ParameterCount));
            methodGroupResolution.Free();
            return result;
        }

        protected override Conversion GetInterpolatedStringConversion(BoundUnconvertedInterpolatedString source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!TypeSymbol.Equals(destination, Compilation.GetWellKnownType(WellKnownType.System_IFormattable), TypeCompareKind.ConsiderEverything) && !TypeSymbol.Equals(destination, Compilation.GetWellKnownType(WellKnownType.System_FormattableString), TypeCompareKind.ConsiderEverything))
            {
                return Conversion.NoConversion;
            }
            return Conversion.InterpolatedString;
        }

        private static MethodGroupResolution ResolveDelegateOrFunctionPointerMethodGroup(Binder binder, BoundMethodGroup source, MethodSymbol delegateInvokeMethodOpt, bool isFunctionPointer, in CallingConventionInfo callingConventionInfo, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)delegateInvokeMethodOpt != null)
            {
                AnalyzedArguments instance = AnalyzedArguments.GetInstance();
                GetDelegateOrFunctionPointerArguments(source.Syntax, instance, delegateInvokeMethodOpt.Parameters, binder.Compilation);
                MethodGroupResolution result = binder.ResolveMethodGroup(source, instance, isMethodGroupConversion: true, ref useSiteInfo, inferWithDynamic: true, delegateInvokeMethodOpt.RefKind, delegateInvokeMethodOpt.ReturnType, isFunctionPointer, in callingConventionInfo);
                instance.Free();
                return result;
            }
            CallingConventionInfo callingConventionInfo2 = default(CallingConventionInfo);
            return binder.ResolveMethodGroup(source, null, isMethodGroupConversion: true, ref useSiteInfo, inferWithDynamic: false, RefKind.None, null, isFunctionPointerResolution: false, in callingConventionInfo2);
        }

        private (MethodSymbol, bool isFunctionPointer, CallingConventionInfo callingConventionInfo) GetDelegateInvokeOrFunctionPointerMethodIfAvailable(BoundMethodGroup methodGroup, TypeSymbol type, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (type is FunctionPointerTypeSymbol functionPointerTypeSymbol)
            {
                FunctionPointerMethodSymbol signature = functionPointerTypeSymbol.Signature;
                if ((object)signature != null)
                {
                    return (signature, true, new CallingConventionInfo(signature.CallingConvention, signature.GetCallingConventionModifiers()));
                }
            }
            NamedTypeSymbol namedTypeSymbol = ((type.SpecialType == SpecialType.System_Delegate && methodGroup.Syntax.IsFeatureEnabled(MessageID.IDS_FeatureNullableReferenceTypes)) ? _binder.GetMethodGroupDelegateType(methodGroup, ref useSiteInfo) : type.GetDelegateType());
            if ((object)namedTypeSymbol == null)
            {
                return (null, false, default(CallingConventionInfo));
            }
            MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
            if ((object)delegateInvokeMethod == null || delegateInvokeMethod.HasUseSiteError)
            {
                return (null, false, default(CallingConventionInfo));
            }
            return (delegateInvokeMethod, false, default(CallingConventionInfo));
        }

        public bool ReportDelegateOrFunctionPointerMethodGroupDiagnostics(Binder binder, BoundMethodGroup expr, TypeSymbol targetType, BindingDiagnosticBag diagnostics)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = binder.GetNewCompoundUseSiteInfo(diagnostics);
            (MethodSymbol, bool isFunctionPointer, CallingConventionInfo callingConventionInfo) delegateInvokeOrFunctionPointerMethodIfAvailable = GetDelegateInvokeOrFunctionPointerMethodIfAvailable(expr, targetType, ref useSiteInfo);
            MethodSymbol item = delegateInvokeOrFunctionPointerMethodIfAvailable.Item1;
            bool item2 = delegateInvokeOrFunctionPointerMethodIfAvailable.isFunctionPointer;
            CallingConventionInfo callingConventionInfo = delegateInvokeOrFunctionPointerMethodIfAvailable.callingConventionInfo;
            MethodGroupResolution methodGroupResolution = ResolveDelegateOrFunctionPointerMethodGroup(binder, expr, item, item2, in callingConventionInfo, ref useSiteInfo);
            diagnostics.Add(expr.Syntax, useSiteInfo);
            bool flag = methodGroupResolution.HasAnyErrors;
            diagnostics.AddRange(methodGroupResolution.Diagnostics);
            if (methodGroupResolution.MethodGroup != null)
            {
                OverloadResolutionResult<MethodSymbol> overloadResolutionResult = methodGroupResolution.OverloadResolutionResult;
                if (overloadResolutionResult != null)
                {
                    if (overloadResolutionResult.Succeeded)
                    {
                        MethodSymbol member = overloadResolutionResult.BestResult.Member;
                        if (methodGroupResolution.MethodGroup.IsExtensionMethodGroup)
                        {
                            ParameterSymbol parameterSymbol = member.Parameters[0];
                            if (!parameterSymbol.Type.IsReferenceType)
                            {
                                diagnostics.Add(ErrorCode.ERR_ValueTypeExtDelegate, expr.Syntax.Location, member, parameterSymbol.Type);
                                flag = true;
                            }
                        }
                        else if (member.ContainingType.IsNullableType() && !member.IsOverride)
                        {
                            diagnostics.Add(ErrorCode.ERR_DelegateOnNullable, expr.Syntax.Location, member);
                            flag = true;
                        }
                    }
                    else if (!flag && !methodGroupResolution.IsEmpty && methodGroupResolution.ResultKind == LookupResultKind.Viable)
                    {
                        BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, diagnostics.AccumulatesDependencies);
                        overloadResolutionResult.ReportDiagnostics(binder, expr.Syntax.Location, expr.Syntax, instance, expr.Name, methodGroupResolution.MethodGroup.Receiver, expr.Syntax, methodGroupResolution.AnalyzedArguments, methodGroupResolution.MethodGroup.Methods.ToImmutable(), null, null, null, isMethodGroupConversion: true, item?.RefKind, targetType);
                        flag = instance.HasAnyErrors();
                        diagnostics.AddRangeAndFree(instance);
                    }
                }
            }
            methodGroupResolution.Free();
            return flag;
        }

        public Conversion MethodGroupConversion(SyntaxNode syntax, MethodGroup methodGroup, NamedTypeSymbol delegateType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            AnalyzedArguments instance = AnalyzedArguments.GetInstance();
            OverloadResolutionResult<MethodSymbol> instance2 = OverloadResolutionResult<MethodSymbol>.GetInstance();
            MethodSymbol delegateInvokeMethod = delegateType.DelegateInvokeMethod;
            GetDelegateOrFunctionPointerArguments(syntax, instance, delegateInvokeMethod.Parameters, Compilation);
            OverloadResolution overloadResolution = _binder.OverloadResolution;
            ArrayBuilder<MethodSymbol> methods = methodGroup.Methods;
            ArrayBuilder<TypeWithAnnotations> typeArguments = methodGroup.TypeArguments;
            BoundExpression receiver = methodGroup.Receiver;
            RefKind refKind = delegateInvokeMethod.RefKind;
            TypeSymbol returnType = delegateInvokeMethod.ReturnType;
            CallingConventionInfo callingConventionInfo = default(CallingConventionInfo);
            overloadResolution.MethodInvocationOverloadResolution(methods, typeArguments, receiver, instance, instance2, ref useSiteInfo, isMethodGroupConversion: true, allowRefOmittedArguments: false, inferWithDynamic: false, allowUnexpandedForm: true, refKind, returnType, isFunctionPointerResolution: false, in callingConventionInfo);
            Conversion result = ToConversion(instance2, methodGroup, delegateType.DelegateInvokeMethod.ParameterCount);
            instance.Free();
            instance2.Free();
            return result;
        }

        public static void GetDelegateOrFunctionPointerArguments(SyntaxNode syntax, AnalyzedArguments analyzedArguments, ImmutableArray<ParameterSymbol> delegateParameters, CSharpCompilation compilation)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = delegateParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol parameterSymbol = enumerator.Current;
                if (parameterSymbol.Type.IsDynamic())
                {
                    parameterSymbol = new SignatureOnlyParameterSymbol(TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_Object), NullableAnnotation.Oblivious, parameterSymbol.TypeWithAnnotations.CustomModifiers), parameterSymbol.RefCustomModifiers, parameterSymbol.IsParams, parameterSymbol.RefKind);
                }
                analyzedArguments.Arguments.Add(new BoundParameter(syntax, parameterSymbol)
                {
                    WasCompilerGenerated = true
                });
                analyzedArguments.RefKinds.Add(parameterSymbol.RefKind);
            }
        }

        private static Conversion ToConversion(OverloadResolutionResult<MethodSymbol> result, MethodGroup methodGroup, int parameterCount)
        {
            if (!result.Succeeded)
            {
                return Conversion.NoConversion;
            }
            MethodSymbol member = result.BestResult.Member;
            if (methodGroup.IsExtensionMethodGroup && !member.Parameters[0].Type.IsReferenceType)
            {
                return Conversion.NoConversion;
            }
            if (member.RequiresInstanceReceiver)
            {
                BoundExpression receiver = methodGroup.Receiver;
                if (receiver != null && receiver.Type?.IsRestrictedType() == true)
                {
                    return Conversion.NoConversion;
                }
            }
            if (member.ContainingType.IsNullableType() && !member.IsOverride)
            {
                return Conversion.NoConversion;
            }
            return new Conversion(ConversionKind.MethodGroup, member, methodGroup.IsExtensionMethodGroup);
        }

        public override Conversion GetStackAllocConversion(BoundStackAllocArrayCreation sourceExpression, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (sourceExpression.NeedsToBeConverted())
            {
                PointerTypeSymbol source = new PointerTypeSymbol(TypeWithAnnotations.Create(sourceExpression.ElementType));
                Conversion underlyingConversion = ClassifyImplicitConversionFromType(source, destination, ref useSiteInfo);
                if (underlyingConversion.IsValid)
                {
                    return Conversion.MakeStackAllocToPointerType(underlyingConversion);
                }
                NamedTypeSymbol wellKnownType = _binder.GetWellKnownType(WellKnownType.System_Span_T, ref useSiteInfo);
                if (wellKnownType.TypeKind == TypeKind.Struct && wellKnownType.IsRefLikeType)
                {
                    NamedTypeSymbol source2 = wellKnownType.Construct(sourceExpression.ElementType);
                    Conversion underlyingConversion2 = ClassifyImplicitConversionFromType(source2, destination, ref useSiteInfo);
                    if (underlyingConversion2.Exists)
                    {
                        return Conversion.MakeStackAllocToSpanType(underlyingConversion2);
                    }
                }
            }
            return Conversion.NoConversion;
        }
    }
}
