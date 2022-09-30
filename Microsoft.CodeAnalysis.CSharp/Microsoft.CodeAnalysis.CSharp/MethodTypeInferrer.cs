using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class MethodTypeInferrer
    {
        internal abstract class Extensions
        {
            private sealed class DefaultExtensions : Extensions
            {
                internal override TypeWithAnnotations GetTypeWithAnnotations(BoundExpression expr)
                {
                    return TypeWithAnnotations.Create(expr.Type);
                }

                internal override TypeWithAnnotations GetMethodGroupResultType(BoundMethodGroup group, MethodSymbol method)
                {
                    return method.ReturnTypeWithAnnotations;
                }
            }

            internal static readonly Extensions Default = new DefaultExtensions();

            internal abstract TypeWithAnnotations GetTypeWithAnnotations(BoundExpression expr);

            internal abstract TypeWithAnnotations GetMethodGroupResultType(BoundMethodGroup group, MethodSymbol method);
        }

        private enum InferenceResult
        {
            InferenceFailed,
            MadeProgress,
            NoProgress,
            Success
        }

        private enum Dependency
        {
            Unknown = 0,
            NotDependent = 1,
            DependsMask = 16,
            Direct = 17,
            Indirect = 18
        }

        private enum ExactOrBoundsKind
        {
            Exact,
            LowerBound,
            UpperBound
        }

        private sealed class EqualsIgnoringDynamicTupleNamesAndNullabilityComparer : EqualityComparer<TypeWithAnnotations>
        {
            internal static readonly EqualsIgnoringDynamicTupleNamesAndNullabilityComparer Instance = new EqualsIgnoringDynamicTupleNamesAndNullabilityComparer();

            public override int GetHashCode(TypeWithAnnotations obj)
            {
                return obj.Type.GetHashCode();
            }

            public override bool Equals(TypeWithAnnotations x, TypeWithAnnotations y)
            {
                if (x.Type.IsDynamic() ^ y.Type.IsDynamic())
                {
                    return false;
                }
                return x.Equals(y, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);
            }
        }

        private readonly ConversionsBase _conversions;

        private readonly ImmutableArray<TypeParameterSymbol> _methodTypeParameters;

        private readonly NamedTypeSymbol _constructedContainingTypeOfMethod;

        private readonly ImmutableArray<TypeWithAnnotations> _formalParameterTypes;

        private readonly ImmutableArray<RefKind> _formalParameterRefKinds;

        private readonly ImmutableArray<BoundExpression> _arguments;

        private readonly Extensions _extensions;

        private readonly TypeWithAnnotations[] _fixedResults;

        private readonly HashSet<TypeWithAnnotations>[] _exactBounds;

        private readonly HashSet<TypeWithAnnotations>[] _upperBounds;

        private readonly HashSet<TypeWithAnnotations>[] _lowerBounds;

        private Dependency[,] _dependencies;

        private bool _dependenciesDirty;

        private int NumberArgumentsToProcess => Math.Min(_arguments.Length, _formalParameterTypes.Length);

        public static MethodTypeInferenceResult Infer(Binder binder, ConversionsBase conversions, ImmutableArray<TypeParameterSymbol> methodTypeParameters, NamedTypeSymbol constructedContainingTypeOfMethod, ImmutableArray<TypeWithAnnotations> formalParameterTypes, ImmutableArray<RefKind> formalParameterRefKinds, ImmutableArray<BoundExpression> arguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, Extensions extensions = null)
        {
            if (formalParameterTypes.Length == 0)
            {
                return new MethodTypeInferenceResult(success: false, default(ImmutableArray<TypeWithAnnotations>));
            }
            return new MethodTypeInferrer(conversions, methodTypeParameters, constructedContainingTypeOfMethod, formalParameterTypes, formalParameterRefKinds, arguments, extensions).InferTypeArgs(binder, ref useSiteInfo);
        }

        private MethodTypeInferrer(ConversionsBase conversions, ImmutableArray<TypeParameterSymbol> methodTypeParameters, NamedTypeSymbol constructedContainingTypeOfMethod, ImmutableArray<TypeWithAnnotations> formalParameterTypes, ImmutableArray<RefKind> formalParameterRefKinds, ImmutableArray<BoundExpression> arguments, Extensions extensions)
        {
            _conversions = conversions;
            _methodTypeParameters = methodTypeParameters;
            _constructedContainingTypeOfMethod = constructedContainingTypeOfMethod;
            _formalParameterTypes = formalParameterTypes;
            _formalParameterRefKinds = formalParameterRefKinds;
            _arguments = arguments;
            _extensions = extensions ?? Extensions.Default;
            _fixedResults = new TypeWithAnnotations[methodTypeParameters.Length];
            _exactBounds = new HashSet<TypeWithAnnotations>[methodTypeParameters.Length];
            _upperBounds = new HashSet<TypeWithAnnotations>[methodTypeParameters.Length];
            _lowerBounds = new HashSet<TypeWithAnnotations>[methodTypeParameters.Length];
            _dependencies = null;
            _dependenciesDirty = false;
        }

        private RefKind GetRefKind(int index)
        {
            if (!_formalParameterRefKinds.IsDefault)
            {
                return _formalParameterRefKinds[index];
            }
            return RefKind.None;
        }

        private ImmutableArray<TypeWithAnnotations> GetResults()
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (!_fixedResults[i].HasType || (_fixedResults[i].Type.IsErrorType() && _fixedResults[i].Type.Name == null))
                {
                    _fixedResults[i] = TypeWithAnnotations.Create(new ExtendedErrorTypeSymbol(_constructedContainingTypeOfMethod, _methodTypeParameters[i].Name, 0, null));
                }
            }
            return _fixedResults.AsImmutable();
        }

        private bool ValidIndex(int index)
        {
            if (0 <= index)
            {
                return index < _methodTypeParameters.Length;
            }
            return false;
        }

        private bool IsUnfixed(int methodTypeParameterIndex)
        {
            return !_fixedResults[methodTypeParameterIndex].HasType;
        }

        private bool IsUnfixedTypeParameter(TypeWithAnnotations type)
        {
            if (type.TypeKind != TypeKind.TypeParameter)
            {
                return false;
            }
            TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type.Type;
            int ordinal = typeParameterSymbol.Ordinal;
            if (ValidIndex(ordinal) && TypeSymbol.Equals(typeParameterSymbol, _methodTypeParameters[ordinal], TypeCompareKind.ConsiderEverything))
            {
                return IsUnfixed(ordinal);
            }
            return false;
        }

        private bool AllFixed()
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (IsUnfixed(i))
                {
                    return false;
                }
            }
            return true;
        }

        private void AddBound(TypeWithAnnotations addedBound, HashSet<TypeWithAnnotations>[] collectedBounds, TypeWithAnnotations methodTypeParameterWithAnnotations)
        {
            int ordinal = ((TypeParameterSymbol)methodTypeParameterWithAnnotations.Type).Ordinal;
            if (collectedBounds[ordinal] == null)
            {
                collectedBounds[ordinal] = new HashSet<TypeWithAnnotations>(TypeWithAnnotations.EqualsComparer.ConsiderEverythingComparer);
            }
            collectedBounds[ordinal].Add(addedBound);
        }

        private bool HasBound(int methodTypeParameterIndex)
        {
            if (_lowerBounds[methodTypeParameterIndex] == null && _upperBounds[methodTypeParameterIndex] == null)
            {
                return _exactBounds[methodTypeParameterIndex] != null;
            }
            return true;
        }

        private TypeSymbol GetFixedDelegateOrFunctionPointer(TypeSymbol delegateOrFunctionPointerType)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(_methodTypeParameters.Length);
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                instance.Add(IsUnfixed(i) ? TypeWithAnnotations.Create(_methodTypeParameters[i]) : _fixedResults[i]);
            }
            return new TypeMap(_constructedContainingTypeOfMethod, _methodTypeParameters, instance.ToImmutableAndFree()).SubstituteType(delegateOrFunctionPointerType).Type;
        }

        private MethodTypeInferenceResult InferTypeArgs(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            InferTypeArgsFirstPhase(ref useSiteInfo);
            return new MethodTypeInferenceResult(InferTypeArgsSecondPhase(binder, ref useSiteInfo), GetResults());
        }

        private void InferTypeArgsFirstPhase(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            int i = 0;
            for (int numberArgumentsToProcess = NumberArgumentsToProcess; i < numberArgumentsToProcess; i++)
            {
                BoundExpression argument = _arguments[i];
                TypeWithAnnotations target = _formalParameterTypes[i];
                ExactOrBoundsKind kind = ((!GetRefKind(i).IsManagedReference() && !target.Type.IsPointerType()) ? ExactOrBoundsKind.LowerBound : ExactOrBoundsKind.Exact);
                MakeExplicitParameterTypeInferences(argument, target, kind, ref useSiteInfo);
            }
        }

        private void MakeExplicitParameterTypeInferences(BoundExpression argument, TypeWithAnnotations target, ExactOrBoundsKind kind, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (argument.Kind == BoundKind.UnboundLambda)
            {
                ExplicitParameterTypeInference(argument, target, ref useSiteInfo);
            }
            else if ((argument.Kind != BoundKind.TupleLiteral || !MakeExplicitParameterTypeInferences((BoundTupleLiteral)argument, target, kind, ref useSiteInfo)) && IsReallyAType(argument.Type))
            {
                ExactOrBoundsInference(kind, _extensions.GetTypeWithAnnotations(argument), target, ref useSiteInfo);
            }
        }

        private bool MakeExplicitParameterTypeInferences(BoundTupleLiteral argument, TypeWithAnnotations target, ExactOrBoundsKind kind, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (target.Type.Kind != SymbolKind.NamedType)
            {
                return false;
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)target.Type;
            ImmutableArray<BoundExpression> arguments = argument.Arguments;
            if (!namedTypeSymbol.IsTupleTypeOfCardinality(arguments.Length))
            {
                return false;
            }
            ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = namedTypeSymbol.TupleElementTypesWithAnnotations;
            for (int i = 0; i < arguments.Length; i++)
            {
                BoundExpression argument2 = arguments[i];
                TypeWithAnnotations target2 = tupleElementTypesWithAnnotations[i];
                MakeExplicitParameterTypeInferences(argument2, target2, kind, ref useSiteInfo);
            }
            return true;
        }

        private bool InferTypeArgsSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            InitializeDependencies();
            while (true)
            {
                switch (DoSecondPhase(binder, ref useSiteInfo))
                {
                    case InferenceResult.InferenceFailed:
                        return false;
                    case InferenceResult.Success:
                        return true;
                }
            }
        }

        private InferenceResult DoSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (AllFixed())
            {
                return InferenceResult.Success;
            }
            MakeOutputTypeInferences(binder, ref useSiteInfo);
            InferenceResult inferenceResult = FixNondependentParameters(ref useSiteInfo);
            if (inferenceResult != InferenceResult.NoProgress)
            {
                return inferenceResult;
            }
            inferenceResult = FixDependentParameters(ref useSiteInfo);
            if (inferenceResult != InferenceResult.NoProgress)
            {
                return inferenceResult;
            }
            return InferenceResult.InferenceFailed;
        }

        private void MakeOutputTypeInferences(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            int i = 0;
            for (int numberArgumentsToProcess = NumberArgumentsToProcess; i < numberArgumentsToProcess; i++)
            {
                TypeWithAnnotations formalType = _formalParameterTypes[i];
                BoundExpression argument = _arguments[i];
                MakeOutputTypeInferences(binder, argument, formalType, ref useSiteInfo);
            }
        }

        private void MakeOutputTypeInferences(Binder binder, BoundExpression argument, TypeWithAnnotations formalType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (argument.Kind == BoundKind.TupleLiteral && (object)argument.Type == null)
            {
                MakeOutputTypeInferences(binder, (BoundTupleLiteral)argument, formalType, ref useSiteInfo);
            }
            else if (HasUnfixedParamInOutputType(argument, formalType.Type) && !HasUnfixedParamInInputType(argument, formalType.Type))
            {
                OutputTypeInference(binder, argument, formalType, ref useSiteInfo);
            }
        }

        private void MakeOutputTypeInferences(Binder binder, BoundTupleLiteral argument, TypeWithAnnotations formalType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (formalType.Type.Kind != SymbolKind.NamedType)
            {
                return;
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)formalType.Type;
            ImmutableArray<BoundExpression> arguments = argument.Arguments;
            if (namedTypeSymbol.IsTupleTypeOfCardinality(arguments.Length))
            {
                ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = namedTypeSymbol.TupleElementTypesWithAnnotations;
                for (int i = 0; i < arguments.Length; i++)
                {
                    BoundExpression argument2 = arguments[i];
                    TypeWithAnnotations formalType2 = tupleElementTypesWithAnnotations[i];
                    MakeOutputTypeInferences(binder, argument2, formalType2, ref useSiteInfo);
                }
            }
        }

        private InferenceResult FixNondependentParameters(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return FixParameters((MethodTypeInferrer inferrer, int index) => !inferrer.DependsOnAny(index), ref useSiteInfo);
        }

        private InferenceResult FixDependentParameters(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return FixParameters((MethodTypeInferrer inferrer, int index) => inferrer.AnyDependsOn(index), ref useSiteInfo);
        }

        private InferenceResult FixParameters(Func<MethodTypeInferrer, int, bool> predicate, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            bool[] array = new bool[_methodTypeParameters.Length];
            InferenceResult result = InferenceResult.NoProgress;
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (IsUnfixed(i) && HasBound(i) && predicate(this, i))
                {
                    array[i] = true;
                    result = InferenceResult.MadeProgress;
                }
            }
            for (int j = 0; j < _methodTypeParameters.Length; j++)
            {
                if (array[j] && !Fix(j, ref useSiteInfo))
                {
                    result = InferenceResult.InferenceFailed;
                }
            }
            return result;
        }

        private static bool DoesInputTypeContain(BoundExpression argument, TypeSymbol formalParameterType, TypeParameterSymbol typeParameter)
        {
            TypeSymbol delegateOrFunctionPointerType = formalParameterType.GetDelegateOrFunctionPointerType();
            if ((object)delegateOrFunctionPointerType == null)
            {
                return false;
            }
            bool flag = delegateOrFunctionPointerType.IsFunctionPointer();
            if (!flag || argument.Kind == BoundKind.UnconvertedAddressOfOperator)
            {
                if (!flag)
                {
                    BoundKind kind = argument.Kind;
                    if (kind != BoundKind.UnboundLambda && kind != BoundKind.MethodGroup)
                    {
                        goto IL_003a;
                    }
                }
                ImmutableArray<ParameterSymbol> immutableArray = delegateOrFunctionPointerType.DelegateOrFunctionPointerParameters();
                if (immutableArray.IsDefaultOrEmpty)
                {
                    return false;
                }
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Type.ContainsTypeParameter(typeParameter))
                    {
                        return true;
                    }
                }
                return false;
            }
            goto IL_003a;
        IL_003a:
            return false;
        }

        private bool HasUnfixedParamInInputType(BoundExpression pSource, TypeSymbol pDest)
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (IsUnfixed(i) && DoesInputTypeContain(pSource, pDest, _methodTypeParameters[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool DoesOutputTypeContain(BoundExpression argument, TypeSymbol formalParameterType, TypeParameterSymbol typeParameter)
        {
            TypeSymbol delegateOrFunctionPointerType = formalParameterType.GetDelegateOrFunctionPointerType();
            if ((object)delegateOrFunctionPointerType == null)
            {
                return false;
            }
            bool flag = delegateOrFunctionPointerType.IsFunctionPointer();
            if (!flag || argument.Kind == BoundKind.UnconvertedAddressOfOperator)
            {
                if (!flag)
                {
                    BoundKind kind = argument.Kind;
                    if (kind != BoundKind.UnboundLambda && kind != BoundKind.MethodGroup)
                    {
                        goto IL_003d;
                    }
                }
                MethodSymbol methodSymbol;
                if (!(delegateOrFunctionPointerType is NamedTypeSymbol namedTypeSymbol))
                {
                    if (!(delegateOrFunctionPointerType is FunctionPointerTypeSymbol functionPointerTypeSymbol))
                    {
                        throw ExceptionUtilities.UnexpectedValue(delegateOrFunctionPointerType);
                    }
                    methodSymbol = functionPointerTypeSymbol.Signature;
                }
                else
                {
                    methodSymbol = namedTypeSymbol.DelegateInvokeMethod;
                }
                MethodSymbol methodSymbol2 = methodSymbol;
                if ((object)methodSymbol2 == null || methodSymbol2.HasUseSiteError)
                {
                    return false;
                }
                return methodSymbol2.ReturnType?.ContainsTypeParameter(typeParameter) ?? false;
            }
            goto IL_003d;
        IL_003d:
            return false;
        }

        private bool HasUnfixedParamInOutputType(BoundExpression argument, TypeSymbol formalParameterType)
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (IsUnfixed(i) && DoesOutputTypeContain(argument, formalParameterType, _methodTypeParameters[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool DependsDirectlyOn(int iParam, int jParam)
        {
            int i = 0;
            for (int numberArgumentsToProcess = NumberArgumentsToProcess; i < numberArgumentsToProcess; i++)
            {
                TypeSymbol type = _formalParameterTypes[i].Type;
                BoundExpression argument = _arguments[i];
                if (DoesInputTypeContain(argument, type, _methodTypeParameters[jParam]) && DoesOutputTypeContain(argument, type, _methodTypeParameters[iParam]))
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeDependencies()
        {
            _dependencies = new Dependency[_methodTypeParameters.Length, _methodTypeParameters.Length];
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                for (int j = 0; j < _methodTypeParameters.Length; j++)
                {
                    if (DependsDirectlyOn(i, j))
                    {
                        _dependencies[i, j] = Dependency.Direct;
                    }
                }
            }
            DeduceAllDependencies();
        }

        private bool DependsOn(int iParam, int jParam)
        {
            if (_dependenciesDirty)
            {
                SetIndirectsToUnknown();
                DeduceAllDependencies();
            }
            return (_dependencies[iParam, jParam] & Dependency.DependsMask) != 0;
        }

        private bool DependsTransitivelyOn(int iParam, int jParam)
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if ((_dependencies[iParam, i] & Dependency.DependsMask) != 0 && (_dependencies[i, jParam] & Dependency.DependsMask) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void DeduceAllDependencies()
        {
            while (DeduceDependencies())
            {
            }
            SetUnknownsToNotDependent();
            _dependenciesDirty = false;
        }

        private bool DeduceDependencies()
        {
            bool result = false;
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                for (int j = 0; j < _methodTypeParameters.Length; j++)
                {
                    if (_dependencies[i, j] == Dependency.Unknown && DependsTransitivelyOn(i, j))
                    {
                        _dependencies[i, j] = Dependency.Indirect;
                        result = true;
                    }
                }
            }
            return result;
        }

        private void SetUnknownsToNotDependent()
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                for (int j = 0; j < _methodTypeParameters.Length; j++)
                {
                    if (_dependencies[i, j] == Dependency.Unknown)
                    {
                        _dependencies[i, j] = Dependency.NotDependent;
                    }
                }
            }
        }

        private void SetIndirectsToUnknown()
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                for (int j = 0; j < _methodTypeParameters.Length; j++)
                {
                    if (_dependencies[i, j] == Dependency.Indirect)
                    {
                        _dependencies[i, j] = Dependency.Unknown;
                    }
                }
            }
        }

        private void UpdateDependenciesAfterFix(int iParam)
        {
            if (_dependencies != null)
            {
                for (int i = 0; i < _methodTypeParameters.Length; i++)
                {
                    _dependencies[iParam, i] = Dependency.NotDependent;
                    _dependencies[i, iParam] = Dependency.NotDependent;
                }
                _dependenciesDirty = true;
            }
        }

        private bool DependsOnAny(int iParam)
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (DependsOn(iParam, i))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AnyDependsOn(int iParam)
        {
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (DependsOn(i, iParam))
                {
                    return true;
                }
            }
            return false;
        }

        private void OutputTypeInference(Binder binder, BoundExpression expression, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!InferredReturnTypeInference(expression, target, ref useSiteInfo) && !MethodGroupReturnTypeInference(binder, expression, target.Type, ref useSiteInfo))
            {
                TypeWithAnnotations typeWithAnnotations = _extensions.GetTypeWithAnnotations(expression);
                if (typeWithAnnotations.HasType)
                {
                    LowerBoundInference(typeWithAnnotations, target, ref useSiteInfo);
                }
            }
        }

        private bool InferredReturnTypeInference(BoundExpression source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            NamedTypeSymbol delegateType = target.Type.GetDelegateType();
            if ((object)delegateType == null)
            {
                return false;
            }
            TypeWithAnnotations returnTypeWithAnnotations = delegateType.DelegateInvokeMethod.ReturnTypeWithAnnotations;
            if (!returnTypeWithAnnotations.HasType || returnTypeWithAnnotations.SpecialType == SpecialType.System_Void)
            {
                return false;
            }
            TypeWithAnnotations source2 = InferReturnType(source, delegateType, ref useSiteInfo);
            if (!source2.HasType)
            {
                return false;
            }
            LowerBoundInference(source2, returnTypeWithAnnotations, ref useSiteInfo);
            return true;
        }

        private bool MethodGroupReturnTypeInference(Binder binder, BoundExpression source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            BoundKind kind = source.Kind;
            if (kind != BoundKind.MethodGroup && kind != BoundKind.UnconvertedAddressOfOperator)
            {
                return false;
            }
            TypeSymbol delegateOrFunctionPointerType = target.GetDelegateOrFunctionPointerType();
            if ((object)delegateOrFunctionPointerType == null)
            {
                return false;
            }
            if (delegateOrFunctionPointerType.IsFunctionPointer() != (source.Kind == BoundKind.UnconvertedAddressOfOperator))
            {
                return false;
            }
            (MethodSymbol, bool) tuple;
            if (!(delegateOrFunctionPointerType is NamedTypeSymbol namedTypeSymbol))
            {
                if (!(delegateOrFunctionPointerType is FunctionPointerTypeSymbol functionPointerTypeSymbol))
                {
                    throw ExceptionUtilities.UnexpectedValue(delegateOrFunctionPointerType);
                }
                tuple = (functionPointerTypeSymbol.Signature, true);
            }
            else
            {
                tuple = (namedTypeSymbol.DelegateInvokeMethod, false);
            }
            (MethodSymbol, bool) tuple2 = tuple;
            MethodSymbol item = tuple2.Item1;
            bool item2 = tuple2.Item2;
            TypeWithAnnotations returnTypeWithAnnotations = item.ReturnTypeWithAnnotations;
            if (!returnTypeWithAnnotations.HasType || returnTypeWithAnnotations.SpecialType == SpecialType.System_Void)
            {
                return false;
            }
            ImmutableArray<ParameterSymbol> delegateParameters = GetFixedDelegateOrFunctionPointer(delegateOrFunctionPointerType).DelegateOrFunctionPointerParameters();
            if (delegateParameters.IsDefault)
            {
                return false;
            }
            CallingConventionInfo callingConventionInfo = (item2 ? new CallingConventionInfo(item.CallingConvention, ((FunctionPointerMethodSymbol)item).GetCallingConventionModifiers()) : default(CallingConventionInfo));
            BoundMethodGroup source2 = (source as BoundMethodGroup) ?? ((BoundUnconvertedAddressOfOperator)source).Operand;
            TypeWithAnnotations source3 = MethodGroupReturnType(binder, source2, delegateParameters, item.RefKind, item2, ref useSiteInfo, in callingConventionInfo);
            if (source3.IsDefault || source3.IsVoidType())
            {
                return false;
            }
            LowerBoundInference(source3, returnTypeWithAnnotations, ref useSiteInfo);
            return true;
        }

        private TypeWithAnnotations MethodGroupReturnType(Binder binder, BoundMethodGroup source, ImmutableArray<ParameterSymbol> delegateParameters, RefKind delegateRefKind, bool isFunctionPointerResolution, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, in CallingConventionInfo callingConventionInfo)
        {
            AnalyzedArguments instance = AnalyzedArguments.GetInstance();
            Conversions.GetDelegateOrFunctionPointerArguments(source.Syntax, instance, delegateParameters, binder.Compilation);
            MethodGroupResolution methodGroupResolution = binder.ResolveMethodGroup(source, instance, isMethodGroupConversion: true, ref useSiteInfo, inferWithDynamic: false, delegateRefKind, null, isFunctionPointerResolution, in callingConventionInfo);
            TypeWithAnnotations result = default(TypeWithAnnotations);
            if (!methodGroupResolution.IsEmpty)
            {
                OverloadResolutionResult<MethodSymbol> overloadResolutionResult = methodGroupResolution.OverloadResolutionResult;
                if (overloadResolutionResult.Succeeded)
                {
                    result = _extensions.GetMethodGroupResultType(source, overloadResolutionResult.BestResult.Member);
                }
            }
            instance.Free();
            methodGroupResolution.Free();
            return result;
        }

        private void ExplicitParameterTypeInference(BoundExpression source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.Kind != BoundKind.UnboundLambda)
            {
                return;
            }
            UnboundLambda unboundLambda = (UnboundLambda)source;
            if (!unboundLambda.HasExplicitlyTypedParameterList)
            {
                return;
            }
            NamedTypeSymbol delegateType = target.Type.GetDelegateType();
            if ((object)delegateType == null)
            {
                return;
            }
            ImmutableArray<ParameterSymbol> immutableArray = delegateType.DelegateParameters();
            if (!immutableArray.IsDefault)
            {
                int num = immutableArray.Length;
                if (unboundLambda.ParameterCount < num)
                {
                    num = unboundLambda.ParameterCount;
                }
                for (int i = 0; i < num; i++)
                {
                    ExactInference(unboundLambda.ParameterTypeWithAnnotations(i), immutableArray[i].TypeWithAnnotations, ref useSiteInfo);
                }
            }
        }

        private void ExactInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!ExactNullableInference(source, target, ref useSiteInfo) && !ExactTypeParameterInference(source, target) && !ExactArrayInference(source, target, ref useSiteInfo) && !ExactConstructedInference(source, target, ref useSiteInfo))
            {
                ExactPointerInference(source, target, ref useSiteInfo);
            }
        }

        private bool ExactTypeParameterInference(TypeWithAnnotations source, TypeWithAnnotations target)
        {
            if (IsUnfixedTypeParameter(target))
            {
                AddBound(source, _exactBounds, target);
                return true;
            }
            return false;
        }

        private bool ExactArrayInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!source.Type.IsArray() || !target.Type.IsArray())
            {
                return false;
            }
            ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)source.Type;
            ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)target.Type;
            if (!arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol2))
            {
                return false;
            }
            ExactInference(arrayTypeSymbol.ElementTypeWithAnnotations, arrayTypeSymbol2.ElementTypeWithAnnotations, ref useSiteInfo);
            return true;
        }

        private void ExactOrBoundsInference(ExactOrBoundsKind kind, TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            switch (kind)
            {
                case ExactOrBoundsKind.Exact:
                    ExactInference(source, target, ref useSiteInfo);
                    break;
                case ExactOrBoundsKind.LowerBound:
                    LowerBoundInference(source, target, ref useSiteInfo);
                    break;
                case ExactOrBoundsKind.UpperBound:
                    UpperBoundInference(source, target, ref useSiteInfo);
                    break;
            }
        }

        private bool ExactOrBoundsNullableInference(ExactOrBoundsKind kind, TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.IsNullableType() && target.IsNullableType())
            {
                ExactOrBoundsInference(kind, ((NamedTypeSymbol)source.Type).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0], ((NamedTypeSymbol)target.Type).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0], ref useSiteInfo);
                return true;
            }
            if (isNullableOnly(source) && isNullableOnly(target))
            {
                ExactOrBoundsInference(kind, source.AsNotNullableReferenceType(), target.AsNotNullableReferenceType(), ref useSiteInfo);
                return true;
            }
            return false;
            static bool isNullableOnly(TypeWithAnnotations type)
            {
                return type.NullableAnnotation.IsAnnotated();
            }
        }

        private bool ExactNullableInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ExactOrBoundsNullableInference(ExactOrBoundsKind.Exact, source, target, ref useSiteInfo);
        }

        private bool LowerBoundTupleInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!source.Type.TryGetElementTypesWithAnnotationsIfTupleType(out var elementTypes) || !target.Type.TryGetElementTypesWithAnnotationsIfTupleType(out var elementTypes2) || elementTypes.Length != elementTypes2.Length)
            {
                return false;
            }
            for (int i = 0; i < elementTypes.Length; i++)
            {
                LowerBoundInference(elementTypes[i], elementTypes2[i], ref useSiteInfo);
            }
            return true;
        }

        private bool ExactConstructedInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!(source.Type is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            if (!(target.Type is NamedTypeSymbol namedTypeSymbol2))
            {
                return false;
            }
            if (!TypeSymbol.Equals(namedTypeSymbol.OriginalDefinition, namedTypeSymbol2.OriginalDefinition, TypeCompareKind.ConsiderEverything))
            {
                return false;
            }
            ExactTypeArgumentInference(namedTypeSymbol, namedTypeSymbol2, ref useSiteInfo);
            return true;
        }

        private bool ExactPointerInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.TypeKind == TypeKind.Pointer && target.TypeKind == TypeKind.Pointer)
            {
                ExactInference(((PointerTypeSymbol)source.Type).PointedAtTypeWithAnnotations, ((PointerTypeSymbol)target.Type).PointedAtTypeWithAnnotations, ref useSiteInfo);
                return true;
            }
            if (source.Type is FunctionPointerTypeSymbol functionPointerTypeSymbol)
            {
                FunctionPointerMethodSymbol signature = functionPointerTypeSymbol.Signature;
                if ((object)signature != null)
                {
                    int parameterCount = signature.ParameterCount;
                    if (target.Type is FunctionPointerTypeSymbol functionPointerTypeSymbol2)
                    {
                        FunctionPointerMethodSymbol signature2 = functionPointerTypeSymbol2.Signature;
                        if ((object)signature2 != null)
                        {
                            int parameterCount2 = signature2.ParameterCount;
                            if (parameterCount == parameterCount2)
                            {
                                if (!FunctionPointerRefKindsEqual(signature, signature2) || !FunctionPointerCallingConventionsEqual(signature, signature2))
                                {
                                    return false;
                                }
                                for (int i = 0; i < parameterCount; i++)
                                {
                                    ExactInference(signature.ParameterTypesWithAnnotations[i], signature2.ParameterTypesWithAnnotations[i], ref useSiteInfo);
                                }
                                ExactInference(signature.ReturnTypeWithAnnotations, signature2.ReturnTypeWithAnnotations, ref useSiteInfo);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool FunctionPointerCallingConventionsEqual(FunctionPointerMethodSymbol sourceSignature, FunctionPointerMethodSymbol targetSignature)
        {
            if (sourceSignature.CallingConvention != targetSignature.CallingConvention)
            {
                return false;
            }
            ImmutableHashSet<CustomModifier> callingConventionModifiers = sourceSignature.GetCallingConventionModifiers();
            ImmutableHashSet<CustomModifier> callingConventionModifiers2 = targetSignature.GetCallingConventionModifiers();
            if (callingConventionModifiers == null)
            {
                if (callingConventionModifiers2 == null)
                {
                    return true;
                }
            }
            else if (callingConventionModifiers2 != null && callingConventionModifiers.SetEquals(callingConventionModifiers2))
            {
                return true;
            }
            return false;
        }

        private static bool FunctionPointerRefKindsEqual(FunctionPointerMethodSymbol sourceSignature, FunctionPointerMethodSymbol targetSignature)
        {
            bool flag = sourceSignature.RefKind == targetSignature.RefKind;
            bool flag2;
            if (flag)
            {
                bool isDefault = sourceSignature.ParameterRefKinds.IsDefault;
                bool isDefault2 = targetSignature.ParameterRefKinds.IsDefault;
                if (isDefault)
                {
                    if (!isDefault2)
                    {
                        goto IL_0039;
                    }
                    flag2 = true;
                }
                else
                {
                    if (isDefault2)
                    {
                        goto IL_0039;
                    }
                    flag2 = sourceSignature.ParameterRefKinds.SequenceEqual(targetSignature.ParameterRefKinds);
                }
                goto IL_0054;
            }
            goto IL_0056;
        IL_0056:
            return flag;
        IL_0039:
            flag2 = false;
            goto IL_0054;
        IL_0054:
            flag = flag2;
            goto IL_0056;
        }

        private void ExactTypeArgumentInference(NamedTypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            source.GetAllTypeArguments(instance, ref useSiteInfo);
            target.GetAllTypeArguments(instance2, ref useSiteInfo);
            for (int i = 0; i < instance.Count; i++)
            {
                ExactInference(instance[i], instance2[i], ref useSiteInfo);
            }
            instance.Free();
            instance2.Free();
        }

        private void LowerBoundInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!LowerBoundNullableInference(source, target, ref useSiteInfo) && !LowerBoundTypeParameterInference(source, target) && !LowerBoundArrayInference(source.Type, target.Type, ref useSiteInfo) && !LowerBoundTupleInference(source, target, ref useSiteInfo) && !LowerBoundConstructedInference(source.Type, target.Type, ref useSiteInfo))
            {
                LowerBoundFunctionPointerTypeInference(source.Type, target.Type, ref useSiteInfo);
            }
        }

        private bool LowerBoundTypeParameterInference(TypeWithAnnotations source, TypeWithAnnotations target)
        {
            if (IsUnfixedTypeParameter(target))
            {
                AddBound(source, _lowerBounds, target);
                return true;
            }
            return false;
        }

        private static TypeWithAnnotations GetMatchingElementType(ArrayTypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (target.IsArray())
            {
                ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)target;
                if (!arrayTypeSymbol.HasSameShapeAs(source))
                {
                    return default(TypeWithAnnotations);
                }
                return arrayTypeSymbol.ElementTypeWithAnnotations;
            }
            if (!source.IsSZArray)
            {
                return default(TypeWithAnnotations);
            }
            if (!target.IsPossibleArrayGenericInterface())
            {
                return default(TypeWithAnnotations);
            }
            return ((NamedTypeSymbol)target).TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref useSiteInfo);
        }

        private bool LowerBoundArrayInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!source.IsArray())
            {
                return false;
            }
            ArrayTypeSymbol obj = (ArrayTypeSymbol)source;
            TypeWithAnnotations elementTypeWithAnnotations = obj.ElementTypeWithAnnotations;
            TypeWithAnnotations matchingElementType = GetMatchingElementType(obj, target, ref useSiteInfo);
            if (!matchingElementType.HasType)
            {
                return false;
            }
            if (elementTypeWithAnnotations.Type.IsReferenceType)
            {
                LowerBoundInference(elementTypeWithAnnotations, matchingElementType, ref useSiteInfo);
            }
            else
            {
                ExactInference(elementTypeWithAnnotations, matchingElementType, ref useSiteInfo);
            }
            return true;
        }

        private bool LowerBoundNullableInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ExactOrBoundsNullableInference(ExactOrBoundsKind.LowerBound, source, target, ref useSiteInfo);
        }

        private bool LowerBoundConstructedInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!(target is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            if (namedTypeSymbol.AllTypeArgumentCount() == 0)
            {
                return false;
            }
            if (source is NamedTypeSymbol namedTypeSymbol2 && TypeSymbol.Equals(namedTypeSymbol2.OriginalDefinition, namedTypeSymbol.OriginalDefinition, TypeCompareKind.ConsiderEverything))
            {
                if (namedTypeSymbol2.IsInterface || namedTypeSymbol2.IsDelegateType())
                {
                    LowerBoundTypeArgumentInference(namedTypeSymbol2, namedTypeSymbol, ref useSiteInfo);
                }
                else
                {
                    ExactTypeArgumentInference(namedTypeSymbol2, namedTypeSymbol, ref useSiteInfo);
                }
                return true;
            }
            if (LowerBoundClassInference(source, namedTypeSymbol, ref useSiteInfo))
            {
                return true;
            }
            if (LowerBoundInterfaceInference(source, namedTypeSymbol, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        private bool LowerBoundClassInference(TypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (target.TypeKind != TypeKind.Class)
            {
                return false;
            }
            NamedTypeSymbol namedTypeSymbol = null;
            if (source.TypeKind == TypeKind.Class)
            {
                namedTypeSymbol = source.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            else if (source.TypeKind == TypeKind.TypeParameter)
            {
                namedTypeSymbol = ((TypeParameterSymbol)source).EffectiveBaseClass(ref useSiteInfo);
            }
            while ((object)namedTypeSymbol != null)
            {
                if (TypeSymbol.Equals(namedTypeSymbol.OriginalDefinition, target.OriginalDefinition, TypeCompareKind.ConsiderEverything))
                {
                    ExactTypeArgumentInference(namedTypeSymbol, target, ref useSiteInfo);
                    return true;
                }
                namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            return false;
        }

        private bool LowerBoundInterfaceInference(TypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!target.IsInterface)
            {
                return false;
            }
            ImmutableArray<NamedTypeSymbol> interfaces;
            switch (source.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Interface:
                case TypeKind.Struct:
                    interfaces = source.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                    break;
                case TypeKind.TypeParameter:
                    {
                        TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)source;
                        interfaces = typeParameterSymbol.EffectiveBaseClass(ref useSiteInfo).AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).Concat(typeParameterSymbol.AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo));
                        break;
                    }
                default:
                    return false;
            }
            interfaces = ModuloReferenceTypeNullabilityDifferences(interfaces, VarianceKind.In);
            NamedTypeSymbol interfaceInferenceBound = GetInterfaceInferenceBound(interfaces, target);
            if ((object)interfaceInferenceBound == null)
            {
                return false;
            }
            LowerBoundTypeArgumentInference(interfaceInferenceBound, target, ref useSiteInfo);
            return true;
        }

        internal static ImmutableArray<NamedTypeSymbol> ModuloReferenceTypeNullabilityDifferences(ImmutableArray<NamedTypeSymbol> interfaces, VarianceKind variance)
        {
            PooledDictionary<NamedTypeSymbol, NamedTypeSymbol> instance = PooledDictionaryIgnoringNullableModifiersForReferenceTypes.GetInstance();
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = interfaces.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (instance.TryGetValue(current, out var value))
                {
                    NamedTypeSymbol namedTypeSymbol2 = (instance[current] = (NamedTypeSymbol)value.MergeEquivalentTypes(current, variance));
                }
                else
                {
                    instance.Add(current, current);
                }
            }
            ImmutableArray<NamedTypeSymbol> result = ((instance.Count != interfaces.Length) ? instance.Values.ToImmutableArray() : interfaces);
            instance.Free();
            return result;
        }

        private void LowerBoundTypeArgumentInference(NamedTypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance3 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            source.OriginalDefinition.GetAllTypeParameters(instance);
            source.GetAllTypeArguments(instance2, ref useSiteInfo);
            target.GetAllTypeArguments(instance3, ref useSiteInfo);
            for (int i = 0; i < instance2.Count; i++)
            {
                TypeParameterSymbol typeParameterSymbol = instance[i];
                TypeWithAnnotations source2 = instance2[i];
                TypeWithAnnotations target2 = instance3[i];
                if (source2.Type.IsReferenceType && typeParameterSymbol.Variance == VarianceKind.Out)
                {
                    LowerBoundInference(source2, target2, ref useSiteInfo);
                }
                else if (source2.Type.IsReferenceType && typeParameterSymbol.Variance == VarianceKind.In)
                {
                    UpperBoundInference(source2, target2, ref useSiteInfo);
                }
                else
                {
                    ExactInference(source2, target2, ref useSiteInfo);
                }
            }
            instance.Free();
            instance2.Free();
            instance3.Free();
        }

        private bool LowerBoundFunctionPointerTypeInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source is FunctionPointerTypeSymbol functionPointerTypeSymbol)
            {
                FunctionPointerMethodSymbol signature = functionPointerTypeSymbol.Signature;
                if ((object)signature != null && target is FunctionPointerTypeSymbol functionPointerTypeSymbol2)
                {
                    FunctionPointerMethodSymbol signature2 = functionPointerTypeSymbol2.Signature;
                    if ((object)signature2 != null)
                    {
                        if (signature.ParameterCount != signature2.ParameterCount)
                        {
                            return false;
                        }
                        if (!FunctionPointerRefKindsEqual(signature, signature2) || !FunctionPointerCallingConventionsEqual(signature, signature2))
                        {
                            return false;
                        }
                        for (int i = 0; i < signature.ParameterCount; i++)
                        {
                            ParameterSymbol parameterSymbol = signature.Parameters[i];
                            ParameterSymbol parameterSymbol2 = signature2.Parameters[i];
                            if ((parameterSymbol.Type.IsReferenceType || parameterSymbol.Type.IsFunctionPointer()) && parameterSymbol.RefKind == RefKind.None)
                            {
                                UpperBoundInference(parameterSymbol.TypeWithAnnotations, parameterSymbol2.TypeWithAnnotations, ref useSiteInfo);
                            }
                            else
                            {
                                ExactInference(parameterSymbol.TypeWithAnnotations, parameterSymbol2.TypeWithAnnotations, ref useSiteInfo);
                            }
                        }
                        if ((signature.ReturnType.IsReferenceType || signature.ReturnType.IsFunctionPointer()) && signature.RefKind == RefKind.None)
                        {
                            LowerBoundInference(signature.ReturnTypeWithAnnotations, signature2.ReturnTypeWithAnnotations, ref useSiteInfo);
                        }
                        else
                        {
                            ExactInference(signature.ReturnTypeWithAnnotations, signature2.ReturnTypeWithAnnotations, ref useSiteInfo);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private void UpperBoundInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!UpperBoundNullableInference(source, target, ref useSiteInfo) && !UpperBoundTypeParameterInference(source, target) && !UpperBoundArrayInference(source, target, ref useSiteInfo) && !UpperBoundConstructedInference(source, target, ref useSiteInfo))
            {
                UpperBoundFunctionPointerTypeInference(source.Type, target.Type, ref useSiteInfo);
            }
        }

        private bool UpperBoundTypeParameterInference(TypeWithAnnotations source, TypeWithAnnotations target)
        {
            if (IsUnfixedTypeParameter(target))
            {
                AddBound(source, _upperBounds, target);
                return true;
            }
            return false;
        }

        private bool UpperBoundArrayInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!target.Type.IsArray())
            {
                return false;
            }
            ArrayTypeSymbol obj = (ArrayTypeSymbol)target.Type;
            TypeWithAnnotations elementTypeWithAnnotations = obj.ElementTypeWithAnnotations;
            TypeWithAnnotations matchingElementType = GetMatchingElementType(obj, source.Type, ref useSiteInfo);
            if (!matchingElementType.HasType)
            {
                return false;
            }
            if (matchingElementType.Type.IsReferenceType)
            {
                UpperBoundInference(matchingElementType, elementTypeWithAnnotations, ref useSiteInfo);
            }
            else
            {
                ExactInference(matchingElementType, elementTypeWithAnnotations, ref useSiteInfo);
            }
            return true;
        }

        private bool UpperBoundNullableInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ExactOrBoundsNullableInference(ExactOrBoundsKind.UpperBound, source, target, ref useSiteInfo);
        }

        private bool UpperBoundConstructedInference(TypeWithAnnotations sourceWithAnnotations, TypeWithAnnotations targetWithAnnotations, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeSymbol type = sourceWithAnnotations.Type;
            TypeSymbol type2 = targetWithAnnotations.Type;
            if (!(type is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            if (namedTypeSymbol.AllTypeArgumentCount() == 0)
            {
                return false;
            }
            if (type2 is NamedTypeSymbol namedTypeSymbol2 && TypeSymbol.Equals(namedTypeSymbol.OriginalDefinition, type2.OriginalDefinition, TypeCompareKind.ConsiderEverything))
            {
                if (namedTypeSymbol2.IsInterface || namedTypeSymbol2.IsDelegateType())
                {
                    UpperBoundTypeArgumentInference(namedTypeSymbol, namedTypeSymbol2, ref useSiteInfo);
                }
                else
                {
                    ExactTypeArgumentInference(namedTypeSymbol, namedTypeSymbol2, ref useSiteInfo);
                }
                return true;
            }
            if (UpperBoundClassInference(namedTypeSymbol, type2, ref useSiteInfo))
            {
                return true;
            }
            if (UpperBoundInterfaceInference(namedTypeSymbol, type2, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        private bool UpperBoundClassInference(NamedTypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.TypeKind != TypeKind.Class || target.TypeKind != TypeKind.Class)
            {
                return false;
            }
            NamedTypeSymbol namedTypeSymbol = target.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            while ((object)namedTypeSymbol != null)
            {
                if (TypeSymbol.Equals(namedTypeSymbol.OriginalDefinition, source.OriginalDefinition, TypeCompareKind.ConsiderEverything))
                {
                    ExactTypeArgumentInference(source, namedTypeSymbol, ref useSiteInfo);
                    return true;
                }
                namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            return false;
        }

        private bool UpperBoundInterfaceInference(NamedTypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!source.IsInterface)
            {
                return false;
            }
            TypeKind typeKind = target.TypeKind;
            if (typeKind != TypeKind.Class && typeKind != TypeKind.Interface && typeKind != TypeKind.Struct)
            {
                return false;
            }
            NamedTypeSymbol interfaceInferenceBound = GetInterfaceInferenceBound(ModuloReferenceTypeNullabilityDifferences(target.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo), VarianceKind.Out), source);
            if ((object)interfaceInferenceBound == null)
            {
                return false;
            }
            UpperBoundTypeArgumentInference(source, interfaceInferenceBound, ref useSiteInfo);
            return true;
        }

        private void UpperBoundTypeArgumentInference(NamedTypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance3 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            source.OriginalDefinition.GetAllTypeParameters(instance);
            source.GetAllTypeArguments(instance2, ref useSiteInfo);
            target.GetAllTypeArguments(instance3, ref useSiteInfo);
            for (int i = 0; i < instance2.Count; i++)
            {
                TypeParameterSymbol typeParameterSymbol = instance[i];
                TypeWithAnnotations source2 = instance2[i];
                TypeWithAnnotations target2 = instance3[i];
                if (source2.Type.IsReferenceType && typeParameterSymbol.Variance == VarianceKind.Out)
                {
                    UpperBoundInference(source2, target2, ref useSiteInfo);
                }
                else if (source2.Type.IsReferenceType && typeParameterSymbol.Variance == VarianceKind.In)
                {
                    LowerBoundInference(source2, target2, ref useSiteInfo);
                }
                else
                {
                    ExactInference(source2, target2, ref useSiteInfo);
                }
            }
            instance.Free();
            instance2.Free();
            instance3.Free();
        }

        private bool UpperBoundFunctionPointerTypeInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source is FunctionPointerTypeSymbol functionPointerTypeSymbol)
            {
                FunctionPointerMethodSymbol signature = functionPointerTypeSymbol.Signature;
                if ((object)signature != null && target is FunctionPointerTypeSymbol functionPointerTypeSymbol2)
                {
                    FunctionPointerMethodSymbol signature2 = functionPointerTypeSymbol2.Signature;
                    if ((object)signature2 != null)
                    {
                        if (signature.ParameterCount != signature2.ParameterCount)
                        {
                            return false;
                        }
                        if (!FunctionPointerRefKindsEqual(signature, signature2) || !FunctionPointerCallingConventionsEqual(signature, signature2))
                        {
                            return false;
                        }
                        for (int i = 0; i < signature.ParameterCount; i++)
                        {
                            ParameterSymbol parameterSymbol = signature.Parameters[i];
                            ParameterSymbol parameterSymbol2 = signature2.Parameters[i];
                            if ((parameterSymbol.Type.IsReferenceType || parameterSymbol.Type.IsFunctionPointer()) && parameterSymbol.RefKind == RefKind.None)
                            {
                                LowerBoundInference(parameterSymbol.TypeWithAnnotations, parameterSymbol2.TypeWithAnnotations, ref useSiteInfo);
                            }
                            else
                            {
                                ExactInference(parameterSymbol.TypeWithAnnotations, parameterSymbol2.TypeWithAnnotations, ref useSiteInfo);
                            }
                        }
                        if ((signature.ReturnType.IsReferenceType || signature.ReturnType.IsFunctionPointer()) && signature.RefKind == RefKind.None)
                        {
                            UpperBoundInference(signature.ReturnTypeWithAnnotations, signature2.ReturnTypeWithAnnotations, ref useSiteInfo);
                        }
                        else
                        {
                            ExactInference(signature.ReturnTypeWithAnnotations, signature2.ReturnTypeWithAnnotations, ref useSiteInfo);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Fix(int iParam, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            HashSet<TypeWithAnnotations> exact = _exactBounds[iParam];
            HashSet<TypeWithAnnotations> lower = _lowerBounds[iParam];
            HashSet<TypeWithAnnotations> upper = _upperBounds[iParam];
            TypeWithAnnotations typeWithAnnotations = Fix(exact, lower, upper, ref useSiteInfo, _conversions);
            if (!typeWithAnnotations.HasType)
            {
                return false;
            }
            _fixedResults[iParam] = typeWithAnnotations;
            UpdateDependenciesAfterFix(iParam);
            return true;
        }

        private static TypeWithAnnotations Fix(HashSet<TypeWithAnnotations> exact, HashSet<TypeWithAnnotations> lower, HashSet<TypeWithAnnotations> upper, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConversionsBase conversions)
        {
            Dictionary<TypeWithAnnotations, TypeWithAnnotations> dictionary = new Dictionary<TypeWithAnnotations, TypeWithAnnotations>(EqualsIgnoringDynamicTupleNamesAndNullabilityComparer.Instance);
            if (exact == null)
            {
                if (lower != null)
                {
                    AddAllCandidates(dictionary, lower, VarianceKind.Out, conversions);
                }
                if (upper != null)
                {
                    AddAllCandidates(dictionary, upper, VarianceKind.In, conversions);
                }
            }
            else
            {
                AddAllCandidates(dictionary, exact, VarianceKind.None, conversions);
                if (dictionary.Count >= 2)
                {
                    return default(TypeWithAnnotations);
                }
            }
            if (dictionary.Count == 0)
            {
                return default(TypeWithAnnotations);
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            GetAllCandidates(dictionary, instance);
            if (lower != null)
            {
                MergeOrRemoveCandidates(dictionary, lower, instance, conversions, VarianceKind.Out, ref useSiteInfo);
            }
            if (upper != null)
            {
                MergeOrRemoveCandidates(dictionary, upper, instance, conversions, VarianceKind.In, ref useSiteInfo);
            }
            instance.Clear();
            GetAllCandidates(dictionary, instance);
            TypeWithAnnotations result = default(TypeWithAnnotations);
            ArrayBuilder<TypeWithAnnotations>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeWithAnnotations current = enumerator.Current;
                ArrayBuilder<TypeWithAnnotations>.Enumerator enumerator2 = instance.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeWithAnnotations current2 = enumerator2.Current;
                    if (current.Equals(current2, TypeCompareKind.ConsiderEverything) || ImplicitConversionExists(current2, current, ref useSiteInfo, conversions.WithNullability(includeNullability: false)))
                    {
                        continue;
                    }
                    goto IL_0100;
                }
                if (!result.HasType)
                {
                    result = current;
                    continue;
                }
                result = default(TypeWithAnnotations);
                break;
            IL_0100:;
            }
            instance.Free();
            return result;
        }

        private static bool ImplicitConversionExists(TypeWithAnnotations sourceWithAnnotations, TypeWithAnnotations destinationWithAnnotations, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConversionsBase conversions)
        {
            TypeSymbol type = sourceWithAnnotations.Type;
            TypeSymbol type2 = destinationWithAnnotations.Type;
            if (type.IsDynamic() && !type2.IsDynamic())
            {
                return false;
            }
            if (!conversions.HasTopLevelNullabilityImplicitConversion(sourceWithAnnotations, destinationWithAnnotations))
            {
                return false;
            }
            return conversions.ClassifyImplicitConversionFromType(type, type2, ref useSiteInfo).Exists;
        }

        private TypeWithAnnotations InferReturnType(BoundExpression source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.Kind != BoundKind.UnboundLambda)
            {
                return default(TypeWithAnnotations);
            }
            UnboundLambda unboundLambda = (UnboundLambda)source;
            if (unboundLambda.HasSignature)
            {
                ImmutableArray<ParameterSymbol> immutableArray = target.DelegateParameters();
                if (immutableArray.IsDefault)
                {
                    return default(TypeWithAnnotations);
                }
                if (immutableArray.Length != unboundLambda.ParameterCount)
                {
                    return default(TypeWithAnnotations);
                }
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)GetFixedDelegateOrFunctionPointer(target);
            ImmutableArray<ParameterSymbol> immutableArray2 = namedTypeSymbol.DelegateParameters();
            if (unboundLambda.HasExplicitlyTypedParameterList)
            {
                for (int i = 0; i < unboundLambda.ParameterCount; i++)
                {
                    if (!unboundLambda.ParameterType(i).Equals(immutableArray2[i].Type, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        return default(TypeWithAnnotations);
                    }
                }
            }
            return unboundLambda.InferReturnType(_conversions, namedTypeSymbol, ref useSiteInfo);
        }

        private static NamedTypeSymbol GetInterfaceInferenceBound(ImmutableArray<NamedTypeSymbol> interfaces, NamedTypeSymbol target)
        {
            NamedTypeSymbol namedTypeSymbol = null;
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = interfaces.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (TypeSymbol.Equals(current.OriginalDefinition, target.OriginalDefinition, TypeCompareKind.ConsiderEverything))
                {
                    if ((object)namedTypeSymbol == null)
                    {
                        namedTypeSymbol = current;
                    }
                    else if (!TypeSymbol.Equals(namedTypeSymbol, current, TypeCompareKind.ConsiderEverything))
                    {
                        return null;
                    }
                }
            }
            return namedTypeSymbol;
        }

        public static ImmutableArray<TypeWithAnnotations> InferTypeArgumentsFromFirstArgument(ConversionsBase conversions, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (method.ParameterCount < 1 || arguments.Length < 1)
            {
                return default(ImmutableArray<TypeWithAnnotations>);
            }
            MethodSymbol constructedFrom = method.ConstructedFrom;
            MethodTypeInferrer methodTypeInferrer = new MethodTypeInferrer(conversions, constructedFrom.TypeParameters, constructedFrom.ContainingType, constructedFrom.GetParameterTypes(), constructedFrom.ParameterRefKinds, arguments, null);
            if (!methodTypeInferrer.InferTypeArgumentsFromFirstArgument(ref useSiteInfo))
            {
                return default(ImmutableArray<TypeWithAnnotations>);
            }
            return methodTypeInferrer.GetInferredTypeArguments();
        }

        private bool InferTypeArgumentsFromFirstArgument(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeWithAnnotations target = _formalParameterTypes[0];
            BoundExpression boundExpression = _arguments[0];
            if (!IsReallyAType(boundExpression.Type))
            {
                return false;
            }
            LowerBoundInference(_extensions.GetTypeWithAnnotations(boundExpression), target, ref useSiteInfo);
            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                TypeParameterSymbol parameter = _methodTypeParameters[i];
                if (target.Type.ContainsTypeParameter(parameter) && (!HasBound(i) || !Fix(i, ref useSiteInfo)))
                {
                    return false;
                }
            }
            return true;
        }

        private ImmutableArray<TypeWithAnnotations> GetInferredTypeArguments()
        {
            return _fixedResults.AsImmutable();
        }

        private static bool IsReallyAType(TypeSymbol type)
        {
            if ((object)type != null && !type.IsErrorType())
            {
                return !type.IsVoidType();
            }
            return false;
        }

        private static void GetAllCandidates(Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates, ArrayBuilder<TypeWithAnnotations> builder)
        {
            builder.AddRange(candidates.Values);
        }

        private static void AddAllCandidates(Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates, HashSet<TypeWithAnnotations> bounds, VarianceKind variance, ConversionsBase conversions)
        {
            foreach (TypeWithAnnotations bound in bounds)
            {
                TypeWithAnnotations newCandidate = bound;
                if (!conversions.IncludeNullability)
                {
                    newCandidate = newCandidate.SetUnknownNullabilityForReferenceTypes();
                }
                AddOrMergeCandidate(candidates, newCandidate, variance, conversions);
            }
        }

        private static void AddOrMergeCandidate(Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates, TypeWithAnnotations newCandidate, VarianceKind variance, ConversionsBase conversions)
        {
            if (candidates.TryGetValue(newCandidate, out var value))
            {
                MergeAndReplaceIfStillCandidate(candidates, value, newCandidate, variance, conversions);
            }
            else
            {
                candidates.Add(newCandidate, newCandidate);
            }
        }

        private static void MergeOrRemoveCandidates(Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates, HashSet<TypeWithAnnotations> bounds, ArrayBuilder<TypeWithAnnotations> initialCandidates, ConversionsBase conversions, VarianceKind variance, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeCompareKind comparison = ((!conversions.IncludeNullability) ? TypeCompareKind.IgnoreNullableModifiersForReferenceTypes : TypeCompareKind.ConsiderEverything);
            foreach (TypeWithAnnotations bound in bounds)
            {
                ArrayBuilder<TypeWithAnnotations>.Enumerator enumerator2 = initialCandidates.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeWithAnnotations current2 = enumerator2.Current;
                    if (bound.Equals(current2, comparison))
                    {
                        continue;
                    }
                    TypeWithAnnotations sourceWithAnnotations;
                    TypeWithAnnotations destinationWithAnnotations;
                    if (variance == VarianceKind.Out)
                    {
                        sourceWithAnnotations = bound;
                        destinationWithAnnotations = current2;
                    }
                    else
                    {
                        sourceWithAnnotations = current2;
                        destinationWithAnnotations = bound;
                    }
                    if (!ImplicitConversionExists(sourceWithAnnotations, destinationWithAnnotations, ref useSiteInfo, conversions.WithNullability(includeNullability: false)))
                    {
                        candidates.Remove(current2);
                        if (conversions.IncludeNullability && candidates.TryGetValue(bound, out var value))
                        {
                            NullableAnnotation nullableAnnotation = value.NullableAnnotation;
                            NullableAnnotation nullableAnnotation2 = nullableAnnotation.MergeNullableAnnotation(current2.NullableAnnotation, variance);
                            if (nullableAnnotation != nullableAnnotation2)
                            {
                                TypeSymbol type = value.Type;
                                TypeWithAnnotations typeWithAnnotations2 = (candidates[bound] = TypeWithAnnotations.Create(type, nullableAnnotation2));
                            }
                        }
                    }
                    else if (bound.Equals(current2, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        MergeAndReplaceIfStillCandidate(candidates, current2, bound, variance, conversions);
                    }
                }
            }
        }

        private static void MergeAndReplaceIfStillCandidate(Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates, TypeWithAnnotations oldCandidate, TypeWithAnnotations newCandidate, VarianceKind variance, ConversionsBase conversions)
        {
            if (!newCandidate.Type.IsDynamic() && candidates.TryGetValue(oldCandidate, out var value))
            {
                TypeWithAnnotations typeWithAnnotations2 = (candidates[oldCandidate] = value.MergeEquivalentTypes(newCandidate, variance));
            }
        }
    }
}
