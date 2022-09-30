using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class UnboundLambdaState
    {
        private sealed class ReturnInferenceCacheKey
        {
            public readonly ImmutableArray<TypeWithAnnotations> ParameterTypes;

            public readonly ImmutableArray<RefKind> ParameterRefKinds;

            public readonly NamedTypeSymbol? TaskLikeReturnTypeOpt;

            public static readonly ReturnInferenceCacheKey Empty = new ReturnInferenceCacheKey(ImmutableArray<TypeWithAnnotations>.Empty, ImmutableArray<Microsoft.CodeAnalysis.RefKind>.Empty, null);

            private ReturnInferenceCacheKey(ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, NamedTypeSymbol? taskLikeReturnTypeOpt)
            {
                ParameterTypes = parameterTypes;
                ParameterRefKinds = parameterRefKinds;
                TaskLikeReturnTypeOpt = taskLikeReturnTypeOpt;
            }

            public override bool Equals(object? obj)
            {
                if (this == obj)
                {
                    return true;
                }
                if (!(obj is ReturnInferenceCacheKey returnInferenceCacheKey) || returnInferenceCacheKey.ParameterTypes.Length != ParameterTypes.Length || !TypeSymbol.Equals(returnInferenceCacheKey.TaskLikeReturnTypeOpt, TaskLikeReturnTypeOpt, TypeCompareKind.ConsiderEverything))
                {
                    return false;
                }
                for (int i = 0; i < ParameterTypes.Length; i++)
                {
                    if (!returnInferenceCacheKey.ParameterTypes[i].Equals(ParameterTypes[i], TypeCompareKind.ConsiderEverything) || returnInferenceCacheKey.ParameterRefKinds[i] != ParameterRefKinds[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            public override int GetHashCode()
            {
                int num = TaskLikeReturnTypeOpt?.GetHashCode() ?? 0;
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = ParameterTypes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num = Hash.Combine(enumerator.Current.Type, num);
                }
                return num;
            }

            public static ReturnInferenceCacheKey Create(NamedTypeSymbol? delegateType, bool isAsync)
            {
                ImmutableArray<TypeWithAnnotations> parameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
                ImmutableArray<RefKind> parameterRefKinds = ImmutableArray<Microsoft.CodeAnalysis.RefKind>.Empty;
                NamedTypeSymbol namedTypeSymbol = null;
                MethodSymbol methodSymbol = DelegateInvokeMethod(delegateType);
                if ((object)methodSymbol != null)
                {
                    int parameterCount = methodSymbol.ParameterCount;
                    if (parameterCount > 0)
                    {
                        ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(parameterCount);
                        ArrayBuilder<RefKind> instance2 = ArrayBuilder<Microsoft.CodeAnalysis.RefKind>.GetInstance(parameterCount);
                        ImmutableArray<ParameterSymbol>.Enumerator enumerator = methodSymbol.Parameters.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ParameterSymbol current = enumerator.Current;
                            instance2.Add(current.RefKind);
                            instance.Add(current.TypeWithAnnotations);
                        }
                        parameterTypes = instance.ToImmutableAndFree();
                        parameterRefKinds = instance2.ToImmutableAndFree();
                    }
                    if (isAsync && methodSymbol.ReturnType is NamedTypeSymbol namedTypeSymbol2 && !namedTypeSymbol2.IsVoidType() && namedTypeSymbol2.IsCustomTaskType(out var _))
                    {
                        namedTypeSymbol = namedTypeSymbol2.ConstructedFrom;
                    }
                }
                if (parameterTypes.IsEmpty && parameterRefKinds.IsEmpty && (object)namedTypeSymbol == null)
                {
                    return Empty;
                }
                return new ReturnInferenceCacheKey(parameterTypes, parameterRefKinds, namedTypeSymbol);
            }
        }

        private UnboundLambda _unboundLambda;

        internal readonly Binder Binder;

        private ImmutableDictionary<NamedTypeSymbol, BoundLambda>? _bindingCache;

        private ImmutableDictionary<ReturnInferenceCacheKey, BoundLambda>? _returnInferenceCache;

        private BoundLambda? _errorBinding;

        public UnboundLambda UnboundLambda => _unboundLambda;

        public abstract MessageID MessageID { get; }

        public abstract bool HasSignature { get; }

        public abstract bool HasExplicitlyTypedParameterList { get; }

        public abstract int ParameterCount { get; }

        public abstract bool IsAsync { get; }

        public abstract bool HasNames { get; }

        public abstract bool IsStatic { get; }

        public UnboundLambdaState(Binder binder, bool includeCache)
        {
            if (includeCache)
            {
                _bindingCache = ImmutableDictionary<NamedTypeSymbol, BoundLambda>.Empty.WithComparers(Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything);
                _returnInferenceCache = ImmutableDictionary<ReturnInferenceCacheKey, BoundLambda>.Empty;
            }
            Binder = binder;
        }

        public void SetUnboundLambda(UnboundLambda unbound)
        {
            _unboundLambda = unbound;
        }

        protected abstract UnboundLambdaState WithCachingCore(bool includeCache);

        internal UnboundLambdaState WithCaching(bool includeCache)
        {
            if (_bindingCache == null != includeCache)
            {
                return this;
            }
            return WithCachingCore(includeCache);
        }

        public abstract string ParameterName(int index);

        public abstract bool ParameterIsDiscard(int index);

        public abstract SyntaxList<AttributeListSyntax> ParameterAttributes(int index);

        public abstract Location ParameterLocation(int index);

        public abstract TypeWithAnnotations ParameterTypeWithAnnotations(int index);

        public abstract RefKind RefKind(int index);

        protected abstract BoundBlock BindLambdaBody(LambdaSymbol lambdaSymbol, Binder lambdaBodyBinder, BindingDiagnosticBag diagnostics);

        protected abstract BoundExpression? GetLambdaExpressionBody(BoundBlock body);

        protected abstract BoundBlock CreateBlockFromLambdaExpressionBody(Binder lambdaBodyBinder, BoundExpression expression, BindingDiagnosticBag diagnostics);

        public virtual void GenerateAnonymousFunctionConversionError(BindingDiagnosticBag diagnostics, TypeSymbol targetType)
        {
            Binder.GenerateAnonymousFunctionConversionError(diagnostics, _unboundLambda.Syntax, _unboundLambda, targetType);
        }

        public BoundLambda Bind(NamedTypeSymbol delegateType)
        {
            if (!_bindingCache!.TryGetValue(delegateType, out var value))
            {
                value = ReallyBind(delegateType);
                return ImmutableInterlocked.GetOrAdd(ref _bindingCache, delegateType, value);
            }
            return value;
        }

        internal IEnumerable<TypeSymbol> InferredReturnTypes()
        {
            bool any = false;
            foreach (BoundLambda value in _returnInferenceCache!.Values)
            {
                TypeWithAnnotations typeWithAnnotations = value.InferredReturnType.TypeWithAnnotations;
                if (typeWithAnnotations.HasType)
                {
                    any = true;
                    yield return typeWithAnnotations.Type;
                }
            }
            if (!any)
            {
                TypeWithAnnotations typeWithAnnotations2 = BindForErrorRecovery().InferredReturnType.TypeWithAnnotations;
                if (typeWithAnnotations2.HasType)
                {
                    yield return typeWithAnnotations2.Type;
                }
            }
        }

        private static MethodSymbol? DelegateInvokeMethod(NamedTypeSymbol? delegateType)
        {
            return delegateType.GetDelegateType()?.DelegateInvokeMethod;
        }

        private static TypeWithAnnotations DelegateReturnTypeWithAnnotations(MethodSymbol? invokeMethod, out RefKind refKind)
        {
            if ((object)invokeMethod == null)
            {
                refKind = Microsoft.CodeAnalysis.RefKind.None;
                return default(TypeWithAnnotations);
            }
            refKind = invokeMethod!.RefKind;
            return invokeMethod!.ReturnTypeWithAnnotations;
        }

        private bool DelegateNeedsReturn(MethodSymbol? invokeMethod)
        {
            if ((object)invokeMethod == null || invokeMethod!.ReturnsVoid)
            {
                return false;
            }
            if (IsAsync && invokeMethod!.ReturnType.IsNonGenericTaskType(Binder.Compilation))
            {
                return false;
            }
            return true;
        }

        internal NamedTypeSymbol? InferDelegateType(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            CSharpCompilation compilation = Binder.Compilation;
            ArrayBuilder<RefKind> instance = ArrayBuilder<Microsoft.CodeAnalysis.RefKind>.GetInstance(ParameterCount);
            ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance(ParameterCount);
            for (int i = 0; i < ParameterCount; i++)
            {
                instance.Add(HasExplicitlyTypedParameterList ? RefKind(i) : Microsoft.CodeAnalysis.RefKind.None);
                instance2.Add(HasExplicitlyTypedParameterList ? ParameterTypeWithAnnotations(i) : TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_Object)));
            }
            ImmutableArray<RefKind> parameterRefKinds = instance.ToImmutableAndFree();
            ImmutableArray<TypeWithAnnotations> parameterTypes = instance2.ToImmutableAndFree();
            LambdaSymbol lambdaSymbol = new LambdaSymbol(Binder, compilation, Binder.ContainingMemberOrLambda, _unboundLambda, parameterTypes, parameterRefKinds, Microsoft.CodeAnalysis.RefKind.None, default(TypeWithAnnotations));
            ExecutableCodeBinder executableCodeBinder = new ExecutableCodeBinder(_unboundLambda.Syntax, lambdaSymbol, ParameterBinder(lambdaSymbol, Binder));
            BoundBlock block = BindLambdaBody(lambdaSymbol, executableCodeBinder, BindingDiagnosticBag.Discarded);
            ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> instance3 = ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>.GetInstance();
            BoundLambda.BlockReturns.GetReturnTypes(instance3, block);
            InferredLambdaReturnType inferredLambdaReturnType = BoundLambda.InferReturnType(instance3, _unboundLambda, executableCodeBinder, null, IsAsync, Binder.Conversions);
            if (HasExplicitlyTypedParameterList && (inferredLambdaReturnType.TypeWithAnnotations.HasType || instance3.Count == 0))
            {
                return Binder.GetMethodGroupOrLambdaDelegateType(inferredLambdaReturnType.RefKind, inferredLambdaReturnType.TypeWithAnnotations, parameterRefKinds, parameterTypes, ref useSiteInfo);
            }
            return null;
        }

        private BoundLambda ReallyBind(NamedTypeSymbol delegateType)
        {
            MethodSymbol invokeMethod = DelegateInvokeMethod(delegateType);
            TypeWithAnnotations typeWithAnnotations = DelegateReturnTypeWithAnnotations(invokeMethod, out RefKind refKind);
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, _unboundLambda.WithDependencies);
            CSharpCompilation compilation = Binder.Compilation;
            ReturnInferenceCacheKey returnInferenceCacheKey = ReturnInferenceCacheKey.Create(delegateType, IsAsync);
            LambdaSymbol lambdaSymbol;
            Binder binder;
            BoundBlock boundBlock;
            if (refKind == Microsoft.CodeAnalysis.RefKind.None && _returnInferenceCache!.TryGetValue(returnInferenceCacheKey, out var value))
            {
                BoundExpression lambdaExpressionBody = GetLambdaExpressionBody(value.Body);
                if (lambdaExpressionBody != null && (lambdaSymbol = value.Symbol).RefKind == refKind && (object)LambdaSymbol.InferenceFailureReturnType != lambdaSymbol.ReturnType && lambdaSymbol.ReturnTypeWithAnnotations.Equals(typeWithAnnotations, TypeCompareKind.ConsiderEverything))
                {
                    binder = value.Binder;
                    boundBlock = CreateBlockFromLambdaExpressionBody(binder, lambdaExpressionBody, instance);
                    instance.AddRange(value.Diagnostics);
                    goto IL_010e;
                }
            }
            lambdaSymbol = CreateLambdaSymbol(Binder.ContainingMemberOrLambda, typeWithAnnotations, returnInferenceCacheKey.ParameterTypes, returnInferenceCacheKey.ParameterRefKinds, refKind);
            binder = new ExecutableCodeBinder(_unboundLambda.Syntax, lambdaSymbol, ParameterBinder(lambdaSymbol, Binder));
            boundBlock = BindLambdaBody(lambdaSymbol, binder, instance);
            goto IL_010e;
        IL_010e:
            lambdaSymbol.GetDeclarationDiagnostics(instance);
            if (lambdaSymbol.RefKind == Microsoft.CodeAnalysis.RefKind.In)
            {
                compilation.EnsureIsReadOnlyAttributeExists(instance, lambdaSymbol.DiagnosticLocation, modifyCompilation: false);
            }
            ImmutableArray<ParameterSymbol> parameters = lambdaSymbol.Parameters;
            ParameterHelpers.EnsureIsReadOnlyAttributeExists(compilation, parameters, instance, modifyCompilation: false);
            if (typeWithAnnotations.HasType)
            {
                if (typeWithAnnotations.Type.ContainsNativeInteger())
                {
                    compilation.EnsureNativeIntegerAttributeExists(instance, lambdaSymbol.DiagnosticLocation, modifyCompilation: false);
                }
                if (compilation.ShouldEmitNullableAttributes(lambdaSymbol) && typeWithAnnotations.NeedsNullableAttribute())
                {
                    compilation.EnsureNullableAttributeExists(instance, lambdaSymbol.DiagnosticLocation, modifyCompilation: false);
                }
            }
            ParameterHelpers.EnsureNativeIntegerAttributeExists(compilation, parameters, instance, modifyCompilation: false);
            ParameterHelpers.EnsureNullableAttributeExists(compilation, lambdaSymbol, parameters, instance, modifyCompilation: false);
            ValidateUnsafeParameters(instance, returnInferenceCacheKey.ParameterTypes);
            if (ControlFlowPass.Analyze(compilation, lambdaSymbol, boundBlock, instance.DiagnosticBag))
            {
                if (DelegateNeedsReturn(invokeMethod))
                {
                    instance.Add(ErrorCode.ERR_AnonymousReturnExpected, lambdaSymbol.DiagnosticLocation, MessageID.Localize(), delegateType);
                }
                else
                {
                    boundBlock = FlowAnalysisPass.AppendImplicitReturn(boundBlock, lambdaSymbol);
                }
            }
            if (IsAsync && !ErrorFacts.PreventsSuccessfulDelegateConversion(instance.DiagnosticBag) && typeWithAnnotations.HasType && !typeWithAnnotations.IsVoidType() && !typeWithAnnotations.Type.IsNonGenericTaskType(compilation) && !typeWithAnnotations.Type.IsGenericTaskType(compilation))
            {
                instance.Add(ErrorCode.ERR_CantConvAsyncAnonFuncReturns, lambdaSymbol.DiagnosticLocation, lambdaSymbol.MessageID.Localize(), delegateType);
            }
            if (IsAsync)
            {
                lambdaSymbol.ReportAsyncParameterErrors(instance, lambdaSymbol.DiagnosticLocation);
            }
            return new BoundLambda(_unboundLambda.Syntax, _unboundLambda, boundBlock, instance.ToReadOnlyAndFree(), binder, delegateType, default(InferredLambdaReturnType))
            {
                WasCompilerGenerated = _unboundLambda.WasCompilerGenerated
            };
        }

        internal LambdaSymbol CreateLambdaSymbol(Symbol containingSymbol, TypeWithAnnotations returnType, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, RefKind refKind)
        {
            return new LambdaSymbol(Binder, Binder.Compilation, containingSymbol, _unboundLambda, parameterTypes, parameterRefKinds, refKind, returnType);
        }

        internal LambdaSymbol CreateLambdaSymbol(NamedTypeSymbol delegateType, Symbol containingSymbol)
        {
            TypeWithAnnotations returnType = DelegateReturnTypeWithAnnotations(DelegateInvokeMethod(delegateType), out RefKind refKind);
            ReturnInferenceCacheKey returnInferenceCacheKey = ReturnInferenceCacheKey.Create(delegateType, IsAsync);
            return CreateLambdaSymbol(containingSymbol, returnType, returnInferenceCacheKey.ParameterTypes, returnInferenceCacheKey.ParameterRefKinds, refKind);
        }

        private void ValidateUnsafeParameters(BindingDiagnosticBag diagnostics, ImmutableArray<TypeWithAnnotations> targetParameterTypes)
        {
            if (!HasSignature)
            {
                return;
            }
            int num = Math.Min(targetParameterTypes.Length, ParameterCount);
            for (int i = 0; i < num; i++)
            {
                if (targetParameterTypes[i].Type.IsUnsafe())
                {
                    Binder.ReportUnsafeIfNotAllowed(ParameterLocation(i), diagnostics);
                }
            }
        }

        private BoundLambda ReallyInferReturnType(NamedTypeSymbol? delegateType, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds)
        {
            (LambdaSymbol lambdaSymbol, BoundBlock block, ExecutableCodeBinder lambdaBodyBinder, BindingDiagnosticBag diagnostics) tuple = BindWithParameterAndReturnType(parameterTypes, parameterRefKinds, default(TypeWithAnnotations), Microsoft.CodeAnalysis.RefKind.None);
            LambdaSymbol item = tuple.lambdaSymbol;
            BoundBlock item2 = tuple.block;
            ExecutableCodeBinder item3 = tuple.lambdaBodyBinder;
            BindingDiagnosticBag item4 = tuple.diagnostics;
            ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> instance = ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>.GetInstance();
            BoundLambda.BlockReturns.GetReturnTypes(instance, item2);
            InferredLambdaReturnType inferredReturnType = BoundLambda.InferReturnType(instance, _unboundLambda, item3, delegateType, item.IsAsync, item3.Conversions);
            BoundLambda result = new BoundLambda(_unboundLambda.Syntax, _unboundLambda, item2, item4.ToReadOnlyAndFree(), item3, delegateType, inferredReturnType)
            {
                WasCompilerGenerated = _unboundLambda.WasCompilerGenerated
            };
            TypeWithAnnotations inferredReturnType2 = inferredReturnType.TypeWithAnnotations;
            if (!inferredReturnType2.HasType)
            {
                inferredReturnType2 = (((object)delegateType == null && instance.Count == 0) ? TypeWithAnnotations.Create(Binder.Compilation.GetSpecialType(SpecialType.System_Void)) : TypeWithAnnotations.Create(LambdaSymbol.InferenceFailureReturnType));
            }
            item.SetInferredReturnType(inferredReturnType.RefKind, inferredReturnType2);
            instance.Free();
            return result;
        }

        private (LambdaSymbol lambdaSymbol, BoundBlock block, ExecutableCodeBinder lambdaBodyBinder, BindingDiagnosticBag diagnostics) BindWithParameterAndReturnType(ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, TypeWithAnnotations returnType, RefKind refKind)
        {
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, _unboundLambda.WithDependencies);
            LambdaSymbol lambdaSymbol = CreateLambdaSymbol(Binder.ContainingMemberOrLambda, returnType, parameterTypes, parameterRefKinds, refKind);
            ExecutableCodeBinder executableCodeBinder = new ExecutableCodeBinder(_unboundLambda.Syntax, lambdaSymbol, ParameterBinder(lambdaSymbol, Binder));
            BoundBlock item = BindLambdaBody(lambdaSymbol, executableCodeBinder, instance);
            return (lambdaSymbol, item, executableCodeBinder, instance);
        }

        public BoundLambda BindForReturnTypeInference(NamedTypeSymbol delegateType)
        {
            ReturnInferenceCacheKey returnInferenceCacheKey = ReturnInferenceCacheKey.Create(delegateType, IsAsync);
            if (!_returnInferenceCache!.TryGetValue(returnInferenceCacheKey, out var value))
            {
                value = ReallyInferReturnType(delegateType, returnInferenceCacheKey.ParameterTypes, returnInferenceCacheKey.ParameterRefKinds);
                return ImmutableInterlocked.GetOrAdd(ref _returnInferenceCache, returnInferenceCacheKey, value);
            }
            return value;
        }

        public virtual Binder ParameterBinder(LambdaSymbol lambdaSymbol, Binder binder)
        {
            return new WithLambdaParametersBinder(lambdaSymbol, binder);
        }

        public BoundLambda BindForErrorRecovery()
        {
            if (_errorBinding == null)
            {
                Interlocked.CompareExchange(ref _errorBinding, ReallyBindForErrorRecovery(), null);
            }
            return _errorBinding;
        }

        private BoundLambda ReallyBindForErrorRecovery()
        {
            return GuessBestBoundLambda(_bindingCache) ?? rebind(GuessBestBoundLambda(_returnInferenceCache)) ?? rebind(ReallyInferReturnType(null, ImmutableArray<TypeWithAnnotations>.Empty, ImmutableArray<Microsoft.CodeAnalysis.RefKind>.Empty));
            [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("lambda")]
            BoundLambda? rebind(BoundLambda? lambda)
            {
                if (lambda == null)
                {
                    return null;
                }
                NamedTypeSymbol delegateType = (NamedTypeSymbol)lambda!.Type;
                ReturnInferenceCacheKey returnInferenceCacheKey = ReturnInferenceCacheKey.Create(delegateType, IsAsync);
                return ReallyBindForErrorRecovery(delegateType, lambda!.InferredReturnType, returnInferenceCacheKey.ParameterTypes, returnInferenceCacheKey.ParameterRefKinds);
            }
        }

        private BoundLambda ReallyBindForErrorRecovery(NamedTypeSymbol? delegateType, InferredLambdaReturnType inferredReturnType, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds)
        {
            TypeWithAnnotations typeWithAnnotations = inferredReturnType.TypeWithAnnotations;
            RefKind refKind = inferredReturnType.RefKind;
            if (!typeWithAnnotations.HasType)
            {
                typeWithAnnotations = DelegateReturnTypeWithAnnotations(DelegateInvokeMethod(delegateType), out refKind);
                if (!typeWithAnnotations.HasType || typeWithAnnotations.Type.ContainsTypeParameter())
                {
                    typeWithAnnotations = TypeWithAnnotations.Create((inferredReturnType.HadExpressionlessReturn || inferredReturnType.NumExpressions == 0) ? Binder.Compilation.GetSpecialType(SpecialType.System_Void) : Binder.CreateErrorType());
                    refKind = Microsoft.CodeAnalysis.RefKind.None;
                }
            }
            (LambdaSymbol lambdaSymbol, BoundBlock block, ExecutableCodeBinder lambdaBodyBinder, BindingDiagnosticBag diagnostics) tuple = BindWithParameterAndReturnType(parameterTypes, parameterRefKinds, typeWithAnnotations, refKind);
            BoundBlock item = tuple.block;
            ExecutableCodeBinder item2 = tuple.lambdaBodyBinder;
            BindingDiagnosticBag item3 = tuple.diagnostics;
            return new BoundLambda(_unboundLambda.Syntax, _unboundLambda, item, item3.ToReadOnlyAndFree(), item2, delegateType, new InferredLambdaReturnType(inferredReturnType.NumExpressions, inferredReturnType.HadExpressionlessReturn, refKind, typeWithAnnotations, ImmutableArray<DiagnosticInfo>.Empty, ImmutableArray<AssemblySymbol>.Empty))
            {
                WasCompilerGenerated = _unboundLambda.WasCompilerGenerated
            };
        }

        private static BoundLambda? GuessBestBoundLambda<T>(ImmutableDictionary<T, BoundLambda> candidates) where T : notnull
        {
            return candidates.Count switch
            {
                0 => null,
                1 => candidates.First().Value,
                _ => (from lambda in (from lambda in candidates
                                      group lambda by lambda.Value.Diagnostics.Diagnostics.Length into @group
                                      orderby @group.Key
                                      select @group).First()
                      orderby GetLambdaSortString(lambda.Value.Symbol)
                      select lambda).FirstOrDefault().Value,
            };
        }

        private static string GetLambdaSortString(LambdaSymbol lambda)
        {
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = lambda.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                instance.Builder.Append(current.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
            }
            if (lambda.ReturnTypeWithAnnotations.HasType)
            {
                instance.Builder.Append(lambda.ReturnTypeWithAnnotations.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }
            return instance.ToStringAndFree();
        }

        public bool GenerateSummaryErrors(BindingDiagnosticBag diagnostics)
        {
            IEnumerable<ImmutableBindingDiagnostic<AssemblySymbol>> first = _bindingCache.Select<KeyValuePair<NamedTypeSymbol, BoundLambda>, ImmutableBindingDiagnostic<AssemblySymbol>>(delegate (KeyValuePair<NamedTypeSymbol, BoundLambda> boundLambda)
            {
                KeyValuePair<NamedTypeSymbol, BoundLambda> keyValuePair = boundLambda;
                return keyValuePair.Value.Diagnostics;
            });
            IEnumerable<ImmutableBindingDiagnostic<AssemblySymbol>> second = _returnInferenceCache!.Values.Select((BoundLambda boundLambda) => boundLambda.Diagnostics);
            IEnumerable<ImmutableBindingDiagnostic<AssemblySymbol>> enumerable = first.Concat(second);
            FirstAmongEqualsSet<Diagnostic> firstAmongEqualsSet = null;
            foreach (ImmutableBindingDiagnostic<AssemblySymbol> item in enumerable)
            {
                if (firstAmongEqualsSet == null)
                {
                    firstAmongEqualsSet = CreateFirstAmongEqualsSet(item.Diagnostics);
                }
                else
                {
                    firstAmongEqualsSet.IntersectWith(item.Diagnostics);
                }
            }
            if (firstAmongEqualsSet != null && PreventsSuccessfulDelegateConversion(firstAmongEqualsSet))
            {
                diagnostics.AddRange(firstAmongEqualsSet);
                return true;
            }
            FirstAmongEqualsSet<Diagnostic> firstAmongEqualsSet2 = null;
            foreach (ImmutableBindingDiagnostic<AssemblySymbol> item2 in enumerable)
            {
                if (firstAmongEqualsSet2 == null)
                {
                    firstAmongEqualsSet2 = CreateFirstAmongEqualsSet(item2.Diagnostics);
                }
                else
                {
                    firstAmongEqualsSet2.UnionWith(item2.Diagnostics);
                }
            }
            if (firstAmongEqualsSet2 != null && PreventsSuccessfulDelegateConversion(firstAmongEqualsSet2))
            {
                diagnostics.AddRange(firstAmongEqualsSet2);
                return true;
            }
            return false;
        }

        private static bool PreventsSuccessfulDelegateConversion(FirstAmongEqualsSet<Diagnostic> set)
        {
            foreach (Diagnostic item in set)
            {
                if (ErrorFacts.PreventsSuccessfulDelegateConversion((ErrorCode)item.Code))
                {
                    return true;
                }
            }
            return false;
        }

        private static FirstAmongEqualsSet<Diagnostic> CreateFirstAmongEqualsSet(ImmutableArray<Diagnostic> bag)
        {
            return new FirstAmongEqualsSet<Diagnostic>(bag, CommonDiagnosticComparer.Instance, CanonicallyCompareDiagnostics);
        }

        private static int CanonicallyCompareDiagnostics(Diagnostic x, Diagnostic y)
        {
            if (x.Code != y.Code)
            {
                return x.Code - y.Code;
            }
            int num = x.Arguments?.Count ?? 0;
            int num2 = y.Arguments?.Count ?? 0;
            int i = 0;
            for (int num3 = Math.Min(num, num2); i < num3; i++)
            {
                object? obj = x.Arguments[i];
                int num4 = string.CompareOrdinal(strB: y.Arguments[i]?.ToString(), strA: obj?.ToString());
                if (num4 != 0)
                {
                    return num4;
                }
            }
            return num - num2;
        }
    }
}
