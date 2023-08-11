// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

/*
SPEC:

Type inference occurs as part of the compile-time processing of a method invocation
and takes place before the overload resolution step of the invocation. When a
particular method group is specified in a method invocation, and no type arguments
are specified as part of the method invocation, type inference is applied to each
generic method in the method group. If type inference succeeds, then the inferred
type arguments are used to determine the types of formal parameters for subsequent 
overload resolution. If overload resolution chooses a generic method as the one to
invoke then the inferred type arguments are used as the actual type arguments for the
invocation. If type inference for a particular method fails, that method does not
participate in overload resolution. The failure of type inference, in and of itself,
does not cause a compile-time error. However, it often leads to a compile-time error
when overload resolution then fails to find any applicable methods.

If the supplied number of arguments is different than the number of parameters in
the method, then inference immediately fails. Otherwise, assume that the generic
method has the following signature:

Tr M<X1...Xn>(T1 x1 ... Tm xm)

With a method call of the form M(E1...Em) the task of type inference is to find
unique type arguments S1...Sn for each of the type parameters X1...Xn so that the
call M<S1...Sn>(E1...Em)becomes valid.

During the process of inference each type parameter Xi is either fixed to a particular
type Si or unfixed with an associated set of bounds. Each of the bounds is some type T.
Each bound is classified as an upper bound, lower bound or exact bound.
Initially each type variable Xi is unfixed with an empty set of bounds.

*/

// This file contains the implementation for method type inference on calls (with
// arguments, and method type inference on conversion of method groups to delegate
// types (which will not have arguments.)

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class PooledDictionaryIgnoringNullableModifiersForReferenceTypes
    {
        private static readonly ObjectPool<PooledDictionary<NamedTypeSymbol, NamedTypeSymbol>> s_poolInstance
            = PooledDictionary<NamedTypeSymbol, NamedTypeSymbol>.CreatePool(Symbols.SymbolEqualityComparer.IgnoringNullable);

        internal static PooledDictionary<NamedTypeSymbol, NamedTypeSymbol> GetInstance()
        {
            var instance = s_poolInstance.Allocate();
            return instance;
        }
    }

    // Method type inference can fail, but we still might have some best guesses. 
    internal struct MethodTypeInferenceResult
    {
        public readonly ImmutableArray<TypeWithAnnotations> InferredTypeArguments;
        public readonly bool Success;

        public MethodTypeInferenceResult(
            bool success,
            ImmutableArray<TypeWithAnnotations> inferredTypeArguments)
        {
            this.Success = success;
            this.InferredTypeArguments = inferredTypeArguments;
        }
    }

    internal sealed class MethodTypeInferrer
    {
        internal abstract class Extensions
        {
            internal static readonly Extensions Default = new DefaultExtensions();

            internal abstract TypeWithAnnotations GetTypeWithAnnotations(BoundExpression expr);

            internal abstract TypeWithAnnotations GetMethodGroupResultType(BoundMethodGroup group, MethodSymbol method);

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
            Unknown = 0x00,
            NotDependent = 0x01,
            DependsMask = 0x10,
            Direct = 0x11,
            Indirect = 0x12
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
        private Dependency[,] _dependencies; // Initialized lazily
        private bool _dependenciesDirty;

        /// <summary>
        /// For error recovery, we allow a mismatch between the number of arguments and parameters
        /// during type inference. This sometimes enables inferring the type for a lambda parameter.
        /// </summary>
        private int NumberArgumentsToProcess => System.Math.Min(_arguments.Length, _formalParameterTypes.Length);

        public static MethodTypeInferenceResult Infer(
            Binder binder,
            ConversionsBase conversions,
            ImmutableArray<TypeParameterSymbol> methodTypeParameters,
            // We are attempting to build a map from method type parameters 
            // to inferred type arguments.

            NamedTypeSymbol constructedContainingTypeOfMethod,
            ImmutableArray<TypeWithAnnotations> formalParameterTypes,

            // We have some unusual requirements for the types that flow into the inference engine.
            // Consider the following inference problems:
            // 
            // Scenario one: 
            //
#pragma warning disable S125 // Sections of code should not be commented out
            // class C<T> 
            // {
            //   delegate Y FT<X, Y>(T t, X x);
            //   static void M<U, V>(U u, FT<U, V> f);
            //   ...
            //   C<double>.M(123, (t,x)=>t+x);
            //
            // From the first argument we infer that U is int. How now must we make an inference on
            // the second argument? The *declared* type of the formal is C<T>.FT<U,V>. The
            // actual type at the time of inference is known to be C<double>.FT<int, something>
            // where "something" is to be determined by inferring the return type of the 
            // lambda by determine the type of "double + int". 
            // 
            // Therefore when we do type inference, if a formal parameter type is a generic delegate
            // then *its* formal parameter types must be the formal parameter types of the 
            // *constructed* generic delegate C<double>.FT<...>, not C<T>.FT<...>. 
            //
            // One would therefore suppose that we'd expect the formal parameter types to here
            // be passed in with the types constructed with the information known from the
            // call site, not the declared types.
            //
            // Contrast that with this scenario:
            //
            // Scenario Two:
            //
            // interface I<T> 
            // { 
            //    void M<U>(T t, U u); 
            // }
            // ...
            // static void Goo<V>(V v, I<V> iv) 
            // {
            //   iv.M(v, "");
            // }
            //
            // Obviously inference should succeed here; it should infer that U is string. 
            //
            // But consider what happens during the inference process on the first argument.
            // The first thing we will do is say "what's the type of the argument? V. What's
            // the type of the corresponding formal parameter? The first formal parameter of
            // I<V>.M<whatever> is of type V. The inference engine will then say "V is a 
            // method type parameter, and therefore we have an inference from V to V". 
            // But *V* is not one of the method type parameters being inferred; the only 
            // method type parameter being inferred here is *U*.
            //
            // This is perhaps some evidence that the formal parameters passed in should be
            // the formal parameters of the *declared* method; in this case, (T, U), not
            // the formal parameters of the *constructed* method, (V, U). 
            //
            // However, one might make the argument that no, we could just add a check
            // to ensure that if we see a method type parameter as a formal parameter type,
            // then we only perform the inference if the method type parameter is a type 
            // parameter of the method for which inference is being performed.
            //
            // Unfortunately, that does not work either:
            //
            // Scenario three:
            //
            // class C<T>
            // {
            //   static void M<U>(T t, U u)
            //   {
            //     ...
            //     C<U>.M(u, 123);
            //     ...
            //   }
            // }
            //
            // The *original* formal parameter types are (T, U); the *constructed* formal parameter types
            // are (U, U), but *those are logically two different U's*. The first U is from the outer caller;
            // the second U is the U of the recursive call.
            //
            // That is, suppose someone called C<string>.M<double>(string, double).  The recursive call should be to
            // C<double>.M<int>(double, int). We should absolutely not make an inference on the first argument
            // from U to U just because C<U>.M<something>'s first formal parameter is of type U.  If we did then
            // inference would fail, because we'd end up with two bounds on 'U' -- 'U' and 'int'. We only want
            // the latter bound.
            //
            // What these three scenarios show is that for a "normal" inference we need to have the
            // formal parameters of the *original* method definition, but when making an inference from a lambda
            // to a delegate, we need to have the *constructed* method signature in order that the formal
            // parameters *of the delegate* be correct.
            //
            // How to solve this problem?
            //
            // We solve it by passing in the formal parameters in their *original* form, but also getting
            // the *fully constructed* type that the method call is on. When constructing the fixed
            // delegate type for inference from a lambda, we do the appropriate type substitution on
            // the delegate.
#pragma warning restore S125 // Sections of code should not be commented out

            ImmutableArray<RefKind> formalParameterRefKinds, // Optional; assume all value if missing.
            ImmutableArray<BoundExpression> arguments,// Required; in scenarios like method group conversions where there are
                                                      // no arguments per se we cons up some fake arguments.
            ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo,
            Extensions extensions = null)
        {

            // Early out: if the method has no formal parameters then we know that inference will fail.
            if (formalParameterTypes.Length == 0)
            {
                return new MethodTypeInferenceResult(false, default);

                // UNDONE: OPTIMIZATION: We could check to see whether there is a type
                // UNDONE: parameter which is never used in any formal parameter; if
                // UNDONE: so then we know ahead of time that inference will fail.
            }

            var inferrer = new MethodTypeInferrer(
                conversions,
                methodTypeParameters,
                constructedContainingTypeOfMethod,
                formalParameterTypes,
                formalParameterRefKinds,
                arguments,
                extensions);
            return inferrer.InferTypeArgs(binder, ref useSiteInfo);
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Fixed, unfixed and bounded type parameters
        //
        // SPEC: During the process of inference each type parameter is either fixed to
        // SPEC: a particular type or unfixed with an associated set of bounds. Each of
        // SPEC: the bounds is of some type T. Initially each type parameter is unfixed
        // SPEC: with an empty set of bounds.

        private MethodTypeInferrer(
            ConversionsBase conversions,
            ImmutableArray<TypeParameterSymbol> methodTypeParameters,
            NamedTypeSymbol constructedContainingTypeOfMethod,
            ImmutableArray<TypeWithAnnotations> formalParameterTypes,
            ImmutableArray<RefKind> formalParameterRefKinds,
            ImmutableArray<BoundExpression> arguments,
            Extensions extensions)
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

#if DEBUG

        internal string Dump()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Method type inference internal state");
            sb.AppendFormat("Inferring method type parameters <{0}>\n", string.Join(", ", _methodTypeParameters));
            sb.Append("Formal parameter types (");
            for (int i = 0; i < _formalParameterTypes.Length; ++i)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }

                sb.Append(GetRefKind(i).ToParameterPrefix());
                sb.Append(_formalParameterTypes[i]);
            }

            sb.Append("\n");

            sb.AppendFormat("Argument types ({0})\n", string.Join(", ", from a in _arguments select a.Type));

            if (_dependencies == null)
            {
                sb.AppendLine("Dependencies are not yet calculated");
            }
            else
            {
                sb.AppendFormat("Dependencies are {0}\n", _dependenciesDirty ? "out of date" : "up to date");
                sb.AppendLine("dependency matrix (Not dependent / Direct / Indirect / Unknown):");
                for (int i = 0; i < _methodTypeParameters.Length; ++i)
                {
                    for (int j = 0; j < _methodTypeParameters.Length; ++j)
                    {
                        switch (_dependencies[i, j])
                        {
                            case Dependency.NotDependent:
                                sb.Append("N");
                                break;
                            case Dependency.Direct:
                                sb.Append("D");
                                break;
                            case Dependency.Indirect:
                                sb.Append("I");
                                break;
                            case Dependency.Unknown:
                                sb.Append("U");
                                break;
                        }
                    }
                    sb.AppendLine();
                }
            }

            for (int i = 0; i < _methodTypeParameters.Length; ++i)
            {
                sb.AppendFormat("Method type parameter {0}: ", _methodTypeParameters[i].Name);

                var fixedType = _fixedResults[i];

                if (!fixedType.HasType)
                {
                    sb.Append("UNFIXED ");
                }
                else
                {
                    sb.AppendFormat("FIXED to {0} ", fixedType);
                }

                sb.AppendFormat("upper bounds: ({0}) ", (_upperBounds[i] == null) ? "" : string.Join(", ", _upperBounds[i]));
                sb.AppendFormat("lower bounds: ({0}) ", (_lowerBounds[i] == null) ? "" : string.Join(", ", _lowerBounds[i]));
                sb.AppendFormat("exact bounds: ({0}) ", (_exactBounds[i] == null) ? "" : string.Join(", ", _exactBounds[i]));
                sb.AppendLine();
            }

            return sb.ToString();
        }

#endif

        private RefKind GetRefKind(int index)
        {
            return _formalParameterRefKinds.IsDefault ? RefKind.None : _formalParameterRefKinds[index];
        }

        private ImmutableArray<TypeWithAnnotations> GetResults()
        {
            // Anything we didn't infer a type for, give the error type.
            // Note: the error type will have the same name as the name
            // of the type parameter we were trying to infer.  This will give a
            // nice user experience where by we will show something like
            // the following:
            //
            // user types: customers.Select(
            // we show   : IE<TResult> IE<Customer>.Select<Customer,TResult>(Func<Customer,TResult> selector)
            //
            // Initially we thought we'd just show ?.  i.e.:
            //
            //  IE<?> IE<Customer>.Select<Customer,?>(Func<Customer,?> selector)
            //
            // This is nice and concise.  However, it falls down if there are multiple
            // type params that we have left.

            for (int i = 0; i < _methodTypeParameters.Length; i++)
            {
                if (_fixedResults[i].HasType)
                {
                    if (!_fixedResults[i].Type.IsErrorType())
                    {
                        continue;
                    }

                    var errorTypeName = _fixedResults[i].Type.Name;
                    if (errorTypeName != null)
                    {
                        continue;
                    }
                }
                _fixedResults[i] = TypeWithAnnotations.Create(new ExtendedErrorTypeSymbol(_constructedContainingTypeOfMethod, _methodTypeParameters[i].Name, 0, null, false));
            }

            return _fixedResults.AsImmutable();
        }

        private bool ValidIndex(int index)
        {
            return 0 <= index && index < _methodTypeParameters.Length;
        }

        private bool IsUnfixed(int methodTypeParameterIndex)
        {
            return !_fixedResults[methodTypeParameterIndex].HasType;
        }

        private bool IsUnfixedTypeParameter(TypeWithAnnotations type)
        {

            if (type.TypeKind != TypeKind.TypeParameter) return false;

            TypeParameterSymbol typeParameter = (TypeParameterSymbol)type.Type;
            int ordinal = typeParameter.Ordinal;
            return ValidIndex(ordinal) &&
                TypeSymbol.Equals(typeParameter, _methodTypeParameters[ordinal], TypeCompareKind.ConsiderEverything2) &&
                IsUnfixed(ordinal);
        }

        private bool AllFixed()
        {
            for (int methodTypeParameterIndex = 0; methodTypeParameterIndex < _methodTypeParameters.Length; ++methodTypeParameterIndex)
            {
                if (IsUnfixed(methodTypeParameterIndex))
                {
                    return false;
                }
            }
            return true;
        }

        private void AddBound(TypeWithAnnotations addedBound, HashSet<TypeWithAnnotations>[] collectedBounds, TypeWithAnnotations methodTypeParameterWithAnnotations)
        {

            var methodTypeParameter = (TypeParameterSymbol)methodTypeParameterWithAnnotations.Type;
            int methodTypeParameterIndex = methodTypeParameter.Ordinal;

            if (collectedBounds[methodTypeParameterIndex] == null)
            {
                collectedBounds[methodTypeParameterIndex] = new HashSet<TypeWithAnnotations>(TypeWithAnnotations.EqualsComparer.ConsiderEverythingComparer);
            }

            collectedBounds[methodTypeParameterIndex].Add(addedBound);
        }

        private bool HasBound(int methodTypeParameterIndex)
        {
            return _lowerBounds[methodTypeParameterIndex] != null ||
                _upperBounds[methodTypeParameterIndex] != null ||
                _exactBounds[methodTypeParameterIndex] != null;
        }

        private TypeSymbol GetFixedDelegateOrFunctionPointer(TypeSymbol delegateOrFunctionPointerType)
        {

            // We have a delegate where the input types use no unfixed parameters.  Create
            // a substitution context; we can substitute unfixed parameters for themselves
            // since they don't actually occur in the inputs.  (They may occur in the outputs,
            // or there may be input parameters fixed to _unfixed_ method type variables.
            // Both of those scenarios are legal.)

            var fixedArguments = ArrayBuilder<TypeWithAnnotations>.GetInstance(_methodTypeParameters.Length);
            for (int iParam = 0; iParam < _methodTypeParameters.Length; iParam++)
            {
                fixedArguments.Add(IsUnfixed(iParam) ? TypeWithAnnotations.Create(_methodTypeParameters[iParam]) : _fixedResults[iParam]);
            }

            TypeMap typeMap = new(_constructedContainingTypeOfMethod, _methodTypeParameters, fixedArguments.ToImmutableAndFree());
            return typeMap.SubstituteType(delegateOrFunctionPointerType).Type;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Phases
        //

        private MethodTypeInferenceResult InferTypeArgs(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: Type inference takes place in phases. Each phase will try to infer type 
            // SPEC: arguments for more type parameters based on the findings of the previous
            // SPEC: phase. The first phase makes some initial inferences of bounds, whereas
            // SPEC: the second phase fixes type parameters to specific types and infers further
            // SPEC: bounds. The second phase may have to be repeated a number of times.
            InferTypeArgsFirstPhase(ref useSiteInfo);
            bool success = InferTypeArgsSecondPhase(binder, ref useSiteInfo);
            return new MethodTypeInferenceResult(success, GetResults());
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // The first phase
        //

        private void InferTypeArgsFirstPhase(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // We expect that we have been handed a list of arguments and a list of the 
            // formal parameter types they correspond to; all the details about named and 
            // optional parameters have already been dealt with.

            // SPEC: For each of the method arguments Ei:
            for (int arg = 0, length = this.NumberArgumentsToProcess; arg < length; arg++)
            {
                BoundExpression argument = _arguments[arg];
                TypeWithAnnotations target = _formalParameterTypes[arg];
                ExactOrBoundsKind kind = GetRefKind(arg).IsManagedReference() || target.Type.IsPointerType() ? ExactOrBoundsKind.Exact : ExactOrBoundsKind.LowerBound;

                MakeExplicitParameterTypeInferences(argument, target, kind, ref useSiteInfo);
            }
        }

        private void MakeExplicitParameterTypeInferences(BoundExpression argument, TypeWithAnnotations target, ExactOrBoundsKind kind, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: * If Ei is an anonymous function, an explicit type parameter
            // SPEC:   inference is made from Ei to Ti.

            // (We cannot make an output type inference from a method group
            // at this time because we have no fixed types yet to use for
            // overload resolution.)

            // SPEC: * Otherwise, if Ei has a type U then a lower-bound inference 
            // SPEC:   or exact inference is made from U to Ti.

            // SPEC: * Otherwise, no inference is made for this argument

            if (argument.Kind == BoundKind.UnboundLambda)
            {
                ExplicitParameterTypeInference(argument, target, ref useSiteInfo);
            }
            else if ((argument.Kind != BoundKind.TupleLiteral ||
                !MakeExplicitParameterTypeInferences((BoundTupleLiteral)argument, target, kind, ref useSiteInfo)) &&
                    IsReallyAType(argument.Type))
            {
                // Either the argument is not a tuple literal, or we were unable to do the inference from its elements, let's try to infer from argument type
                ExactOrBoundsInference(kind, _extensions.GetTypeWithAnnotations(argument), target, ref useSiteInfo);
            }
        }

        private bool MakeExplicitParameterTypeInferences(BoundTupleLiteral argument, TypeWithAnnotations target, ExactOrBoundsKind kind, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // try match up element-wise to the destination tuple (or underlying type)
            // Example:
            //      if   "(a: 1, b: "qq")" is passed as   (T, U) arg
            //      then T becomes int and U becomes string
            if (target.Type.Kind != SymbolKind.NamedType)
            {
                // tuples can only match to tuples or tuple underlying types.
                return false;
            }

            var destination = (NamedTypeSymbol)target.Type;
            var sourceArguments = argument.Arguments;

            // check if the type is actually compatible type for a tuple of given cardinality
            if (!destination.IsTupleTypeOfCardinality(sourceArguments.Length))
            {
                // target is not a tuple of appropriate shape
                return false;
            }

            var destTypes = destination.TupleElementTypesWithAnnotations;

            // NOTE: we are losing tuple element names when recursing into argument expressions.
            //       that is ok, because we are inferring type parameters used in the matching elements, 
            //       This is not the situation where entire tuple literal is used to infer a single type param

            for (int i = 0; i < sourceArguments.Length; i++)
            {
                var sourceArgument = sourceArguments[i];
                var destType = destTypes[i];
                MakeExplicitParameterTypeInferences(sourceArgument, destType, kind, ref useSiteInfo);
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // The second phase
        //

        private bool InferTypeArgsSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: The second phase proceeds as follows:
            // SPEC: * If no unfixed type parameters exist then type inference succeeds.
            // SPEC: * Otherwise, if there exists one or more arguments Ei with corresponding
            // SPEC:   parameter type Ti such that:
            // SPEC:     o the output type of Ei with type Ti contains at least one unfixed
            // SPEC:       type parameter Xj, and
            // SPEC:     o none of the input types of Ei with type Ti contains any unfixed
            // SPEC:       type parameter Xj, 
            // SPEC:   then an output type inference is made from all such Ei to Ti. 
            // SPEC: * Whether or not the previous step actually made an inference, we must
            // SPEC:   now fix at least one type parameter, as follows:
            // SPEC: * If there exists one or more type parameters Xi such that 
            // SPEC:     o Xi is unfixed, and
            // SPEC:     o Xi has a non-empty set of bounds, and
            // SPEC:     o Xi does not depend on any Xj 
            // SPEC:   then each such Xi is fixed. If any fixing operation fails then type
            // SPEC:   inference fails.
            // SPEC: * Otherwise, if there exists one or more type parameters Xi such that
            // SPEC:     o Xi is unfixed, and
            // SPEC:     o Xi has a non-empty set of bounds, and
            // SPEC:     o there is at least one type parameter Xj that depends on Xi
            // SPEC:   then each such Xi is fixed. If any fixing operation fails then
            // SPEC:   type inference fails.
            // SPEC: * Otherwise, we are unable to make progress and there are unfixed parameters.
            // SPEC:   Type inference fails. 
            // SPEC: * If type inference neither succeeds nor fails then the second phase is
            // SPEC:   repeated until type inference succeeds or fails. (Since each repetition of
            // SPEC:   the second phase either succeeds, fails or fixes an unfixed type parameter,
            // SPEC:   the algorithm must terminate with no more repetitions than the number
            // SPEC:   of type parameters.

            InitializeDependencies();
            while (true)
            {
                var res = DoSecondPhase(binder, ref useSiteInfo);
                if (res == InferenceResult.InferenceFailed)
                {
                    return false;
                }
                if (res == InferenceResult.Success)
                {
                    return true;
                }
                // Otherwise, we made some progress last time; do it again.
            }
        }

        private InferenceResult DoSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: * If no unfixed type parameters exist then type inference succeeds.
            if (AllFixed())
            {
                return InferenceResult.Success;
            }
            // SPEC: * Otherwise, if there exists one or more arguments Ei with
            // SPEC:   corresponding parameter type Ti such that:
            // SPEC:     o the output type of Ei with type Ti contains at least one unfixed
            // SPEC:       type parameter Xj, and
            // SPEC:     o none of the input types of Ei with type Ti contains any unfixed
            // SPEC:       type parameter Xj,
            // SPEC:   then an output type inference is made from all such Ei to Ti.

            MakeOutputTypeInferences(binder, ref useSiteInfo);

            // SPEC: * Whether or not the previous step actually made an inference, we
            // SPEC:   must now fix at least one type parameter, as follows:
            // SPEC: * If there exists one or more type parameters Xi such that
            // SPEC:     o Xi is unfixed, and
            // SPEC:     o Xi has a non-empty set of bounds, and
            // SPEC:     o Xi does not depend on any Xj
            // SPEC:   then each such Xi is fixed. If any fixing operation fails then
            // SPEC:   type inference fails.

            InferenceResult res;
            res = FixNondependentParameters(ref useSiteInfo);
            if (res != InferenceResult.NoProgress)
            {
                return res;
            }
            // SPEC: * Otherwise, if there exists one or more type parameters Xi such that
            // SPEC:     o Xi is unfixed, and
            // SPEC:     o Xi has a non-empty set of bounds, and
            // SPEC:     o there is at least one type parameter Xj that depends on Xi
            // SPEC:   then each such Xi is fixed. If any fixing operation fails then
            // SPEC:   type inference fails.
            res = FixDependentParameters(ref useSiteInfo);
            if (res != InferenceResult.NoProgress)
            {
                return res;
            }
            // SPEC: * Otherwise, we are unable to make progress and there are
            // SPEC:   unfixed parameters. Type inference fails.
            return InferenceResult.InferenceFailed;
        }

        private void MakeOutputTypeInferences(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: Otherwise, for all arguments Ei with corresponding parameter type Ti
            // SPEC: where the output types contain unfixed type parameters but the input
            // SPEC: types do not, an output type inference is made from Ei to Ti.

            for (int arg = 0, length = this.NumberArgumentsToProcess; arg < length; arg++)
            {
                var formalType = _formalParameterTypes[arg];
                var argument = _arguments[arg];
                MakeOutputTypeInferences(binder, argument, formalType, ref useSiteInfo);
            }
        }

        private void MakeOutputTypeInferences(Binder binder, BoundExpression argument, TypeWithAnnotations formalType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (argument.Kind == BoundKind.TupleLiteral && argument.Type is null)
            {
                MakeOutputTypeInferences(binder, (BoundTupleLiteral)argument, formalType, ref useSiteInfo);
            }
            else
            {
                if (HasUnfixedParamInOutputType(argument, formalType.Type) && !HasUnfixedParamInInputType(argument, formalType.Type))
                {
#pragma warning disable S125 // Sections of code should not be commented out
                    //UNDONE: if (argument->isTYPEORNAMESPACEERROR() && argumentType->IsErrorType())
                    //UNDONE: {
                    //UNDONE:     argumentType = GetTypeManager().GetErrorSym();
                    //UNDONE: }
#pragma warning restore S125 // Sections of code should not be commented out
                    OutputTypeInference(binder, argument, formalType, ref useSiteInfo);
                }
            }
        }

        private void MakeOutputTypeInferences(Binder binder, BoundTupleLiteral argument, TypeWithAnnotations formalType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (formalType.Type.Kind != SymbolKind.NamedType)
            {
                // tuples can only match to tuples or tuple underlying types.
                return;
            }

            var destination = (NamedTypeSymbol)formalType.Type;

            var sourceArguments = argument.Arguments;

            // check if the type is actually compatible type for a tuple of given cardinality
            if (!destination.IsTupleTypeOfCardinality(sourceArguments.Length))
            {
                return;
            }

            var destTypes = destination.TupleElementTypesWithAnnotations;

            for (int i = 0; i < sourceArguments.Length; i++)
            {
                var sourceArgument = sourceArguments[i];
                var destType = destTypes[i];
                MakeOutputTypeInferences(binder, sourceArgument, destType, ref useSiteInfo);
            }
        }

        private InferenceResult FixNondependentParameters(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: * Otherwise, if there exists one or more type parameters Xi such that
            // SPEC:     o Xi is unfixed, and
            // SPEC:     o Xi has a non-empty set of bounds, and
            // SPEC:     o Xi does not depend on any Xj
            // SPEC:   then each such Xi is fixed.
            return FixParameters((inferrer, index) => !inferrer.DependsOnAny(index), ref useSiteInfo);
        }

        private InferenceResult FixDependentParameters(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: * All unfixed type parameters Xi are fixed for which all of the following hold:
            // SPEC:   * There is at least one type parameter Xj that depends on Xi.
            // SPEC:   * Xi has a non-empty set of bounds.
            return FixParameters((inferrer, index) => inferrer.AnyDependsOn(index), ref useSiteInfo);
        }

        private InferenceResult FixParameters(
            Func<MethodTypeInferrer, int, bool> predicate,
            ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // Dependency is only defined for unfixed parameters. Therefore, fixing
            // a parameter may cause all of its dependencies to become no longer
            // dependent on anything. We need to first determine which parameters need to be 
            // fixed, and then fix them all at once.

            var needsFixing = new bool[_methodTypeParameters.Length];
            var result = InferenceResult.NoProgress;
            for (int param = 0; param < _methodTypeParameters.Length; param++)
            {
                if (IsUnfixed(param) && HasBound(param) && predicate(this, param))
                {
                    needsFixing[param] = true;
                    result = InferenceResult.MadeProgress;
                }
            }

            for (int param = 0; param < _methodTypeParameters.Length; param++)
            {
                // Fix as much as you can, even if there are errors.  That will
                // help with intellisense.
                if (needsFixing[param] && !Fix(param, ref useSiteInfo))
                {
                    result = InferenceResult.InferenceFailed;
                }
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Input types
        //
        private static bool DoesInputTypeContain(BoundExpression argument, TypeSymbol formalParameterType, TypeParameterSymbol typeParameter)
        {
            // SPEC: If E is a method group or an anonymous function and T is a delegate
            // SPEC: type or expression tree type then all the parameter types of T are
            // SPEC: input types of E with type T.

            var delegateOrFunctionPointerType = formalParameterType.GetDelegateOrFunctionPointerType();
            if (delegateOrFunctionPointerType is null)
            {
                return false; // No input types.
            }

            var isFunctionPointer = delegateOrFunctionPointerType.IsFunctionPointer();
            if ((isFunctionPointer && argument.Kind != BoundKind.UnconvertedAddressOfOperator) ||
                (!isFunctionPointer && argument.Kind is not (BoundKind.UnboundLambda or BoundKind.MethodGroup)))
            {
                return false; // No input types.
            }

            var parameters = delegateOrFunctionPointerType.DelegateOrFunctionPointerParameters();
            if (parameters.IsDefaultOrEmpty)
            {
                return false;
            }

            return parameters.Any(parameter => parameter.Type.ContainsTypeParameter(typeParameter));
        }

        private bool HasUnfixedParamInInputType(BoundExpression pSource, TypeSymbol pDest)
        {
            for (int iParam = 0; iParam < _methodTypeParameters.Length; iParam++)
            {
                if (IsUnfixed(iParam) && DoesInputTypeContain(pSource, pDest, _methodTypeParameters[iParam]))
                {
                    return true;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Output types
        //
        private static bool DoesOutputTypeContain(BoundExpression argument, TypeSymbol formalParameterType,
            TypeParameterSymbol typeParameter)
        {
            // SPEC: If E is a method group or an anonymous function and T is a delegate
            // SPEC: type or expression tree type then the return type of T is an output type
            // SPEC: of E with type T.

            var delegateOrFunctionPointerType = formalParameterType.GetDelegateOrFunctionPointerType();
            if (delegateOrFunctionPointerType is null)
            {
                return false;
            }

            var isFunctionPointer = delegateOrFunctionPointerType.IsFunctionPointer();
            if ((isFunctionPointer && argument.Kind != BoundKind.UnconvertedAddressOfOperator) ||
                (!isFunctionPointer && argument.Kind is not (BoundKind.UnboundLambda or BoundKind.MethodGroup)))
            {
                return false;
            }

            MethodSymbol method = delegateOrFunctionPointerType switch
            {
                NamedTypeSymbol n => n.DelegateInvokeMethod,
                FunctionPointerTypeSymbol f => f.Signature,
                _ => throw ExceptionUtilities.UnexpectedValue(delegateOrFunctionPointerType)
            };

            if (method is null || method.HasUseSiteError)
            {
                return false;
            }

            var returnType = method.ReturnType;
            if (returnType is null)
            {
                return false;
            }

            return returnType.ContainsTypeParameter(typeParameter);
        }

        private bool HasUnfixedParamInOutputType(BoundExpression argument, TypeSymbol formalParameterType)
        {
            for (int iParam = 0; iParam < _methodTypeParameters.Length; iParam++)
            {
                if (IsUnfixed(iParam) && DoesOutputTypeContain(argument, formalParameterType, _methodTypeParameters[iParam]))
                {
                    return true;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Dependence
        //

        private bool DependsDirectlyOn(int iParam, int jParam)
        {

            // SPEC: An unfixed type parameter Xi depends directly on an unfixed type
            // SPEC: parameter Xj if for some argument Ek with type Tk, Xj occurs
            // SPEC: in an input type of Ek and Xi occurs in an output type of Ek
            // SPEC: with type Tk.

            // We compute and record the Depends Directly On relationship once, in
            // InitializeDependencies, below.

            // At this point, everything should be unfixed.


            for (int iArg = 0, length = this.NumberArgumentsToProcess; iArg < length; iArg++)
            {
                var formalParameterType = _formalParameterTypes[iArg].Type;
                var argument = _arguments[iArg];
                if (DoesInputTypeContain(argument, formalParameterType, _methodTypeParameters[jParam]) &&
                    DoesOutputTypeContain(argument, formalParameterType, _methodTypeParameters[iParam]))
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeDependencies()
        {
            // We track dependencies by a two-d square array that gives the known
            // relationship between every pair of type parameters. The relationship
            // is one of:
            //
            // * Unknown relationship
            // * known to be not dependent
            // * known to depend directly
            // * known to depend indirectly
            //
            // Since dependency is only defined on unfixed type parameters, fixing a type
            // parameter causes all dependencies involving that parameter to go to
            // the "known to be not dependent" state. Since dependency is a transitive property,
            // this means that doing so may require recalculating the indirect dependencies
            // from the now possibly smaller set of dependencies.
            //
            // Therefore, when we detect that the dependency state has possibly changed
            // due to fixing, we change all "depends indirectly" back into "unknown" and
            // recalculate from the remaining "depends directly".
            //
            // This algorithm thereby yields an extremely bad (but extremely unlikely) worst
            // case for asymptotic performance. Suppose there are n type parameters.
            // "DependsTransitivelyOn" below costs O(n) because it must potentially check
            // all n type parameters to see if there is any k such that Xj => Xk => Xi.
            // "DeduceDependencies" calls "DependsTransitivelyOn" for each "Unknown"
            // pair, and there could be O(n^2) such pairs, so DependsTransitivelyOn is
            // worst-case O(n^3).  And we could have to recalculate the dependency graph
            // after each type parameter is fixed in turn, so that would be O(n) calls to
            // DependsTransitivelyOn, giving this algorithm a worst case of O(n^4).
            //
            // Of course, in reality, n is going to almost always be on the order of
            // "smaller than 5", and there will not be O(n^2) dependency relationships 
            // between type parameters; it is far more likely that the transitivity chains
            // will be very short and not branch or loop at all. This is much more likely to
            // be an O(n^2) algorithm in practice.

            _dependencies = new Dependency[_methodTypeParameters.Length, _methodTypeParameters.Length];
            int iParam;
            int jParam;
            for (iParam = 0; iParam < _methodTypeParameters.Length; ++iParam)
            {
                for (jParam = 0; jParam < _methodTypeParameters.Length; ++jParam)
                {
                    if (DependsDirectlyOn(iParam, jParam))
                    {
                        _dependencies[iParam, jParam] = Dependency.Direct;
                    }
                }
            }

            DeduceAllDependencies();
        }

        private bool DependsOn(int iParam, int jParam)
        {

            // SPEC: Xj depends on Xi if Xj depends directly on Xi, or if Xi depends
            // SPEC: directly on Xk and Xk depends on Xj. Thus "depends on" is the
            // SPEC: transitive but not reflexive closure of "depends directly on".


            if (_dependenciesDirty)
            {
                SetIndirectsToUnknown();
                DeduceAllDependencies();
            }
#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
            return ((_dependencies[iParam, jParam]) & Dependency.DependsMask) != 0;
#pragma warning restore S3265 // Non-flags enums should not be used in bitwise operations
        }

        private bool DependsTransitivelyOn(int iParam, int jParam)
        {

            // Can we find Xk such that Xi depends on Xk and Xk depends on Xj?
            // If so, then Xi depends indirectly on Xj.  (Note that there is
            // a minor optimization here -- the spec comment above notes that
            // we want Xi to depend DIRECTLY on Xk, and Xk to depend directly
            // or indirectly on Xj. But if we already know that Xi depends
            // directly OR indirectly on Xk and Xk depends on Xj, then that's
            // good enough.)

            for (int kParam = 0; kParam < _methodTypeParameters.Length; ++kParam)
            {
#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
                if (((_dependencies[iParam, kParam]) & Dependency.DependsMask) != 0 &&
                    ((_dependencies[kParam, jParam]) & Dependency.DependsMask) != 0)
                {
                    return true;
                }
#pragma warning restore S3265 // Non-flags enums should not be used in bitwise operations
            }
            return false;
        }

        private void DeduceAllDependencies()
        {
            bool madeProgress;
            do
            {
                madeProgress = DeduceDependencies();
            } while (madeProgress);
            SetUnknownsToNotDependent();
            _dependenciesDirty = false;
        }

        private bool DeduceDependencies()
        {
            bool madeProgress = false;
            for (int iParam = 0; iParam < _methodTypeParameters.Length; ++iParam)
            {
                for (int jParam = 0; jParam < _methodTypeParameters.Length; ++jParam)
                {
                    if (_dependencies[iParam, jParam] == Dependency.Unknown && DependsTransitivelyOn(iParam, jParam))
                    {
                        _dependencies[iParam, jParam] = Dependency.Indirect;
                        madeProgress = true;
                    }
                }
            }
            return madeProgress;
        }

        private void SetUnknownsToNotDependent()
        {
            for (int iParam = 0; iParam < _methodTypeParameters.Length; ++iParam)
            {
                for (int jParam = 0; jParam < _methodTypeParameters.Length; ++jParam)
                {
                    if (_dependencies[iParam, jParam] == Dependency.Unknown)
                    {
                        _dependencies[iParam, jParam] = Dependency.NotDependent;
                    }
                }
            }
        }

        private void SetIndirectsToUnknown()
        {
            for (int iParam = 0; iParam < _methodTypeParameters.Length; ++iParam)
            {
                for (int jParam = 0; jParam < _methodTypeParameters.Length; ++jParam)
                {
                    if (_dependencies[iParam, jParam] == Dependency.Indirect)
                    {
                        _dependencies[iParam, jParam] = Dependency.Unknown;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // A fixed parameter never depends on anything, nor is depended upon by anything.

        private void UpdateDependenciesAfterFix(int iParam)
        {
            if (_dependencies == null)
            {
                return;
            }
            for (int jParam = 0; jParam < _methodTypeParameters.Length; ++jParam)
            {
                _dependencies[iParam, jParam] = Dependency.NotDependent;
                _dependencies[jParam, iParam] = Dependency.NotDependent;
            }
            _dependenciesDirty = true;
        }

        private bool DependsOnAny(int iParam)
        {
            for (int jParam = 0; jParam < _methodTypeParameters.Length; ++jParam)
            {
                if (DependsOn(iParam, jParam))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AnyDependsOn(int iParam)
        {
            for (int jParam = 0; jParam < _methodTypeParameters.Length; ++jParam)
            {
#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
                if (DependsOn(jParam, iParam))
                {
                    return true;
                }
#pragma warning restore S2234 // Arguments should be passed in the same order as the method parameters
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Output type inferences
        //
        ////////////////////////////////////////////////////////////////////////////////

        private void OutputTypeInference(Binder binder, BoundExpression expression, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: An output type inference is made from an expression E to a type T
            // SPEC: in the following way:

            // SPEC: * If E is an anonymous function with inferred return type U and
            // SPEC:   T is a delegate type or expression tree with return type Tb
            // SPEC:   then a lower bound inference is made from U to Tb.
            if (InferredReturnTypeInference(expression, target, ref useSiteInfo))
            {
                return;
            }
            // SPEC: * Otherwise, if E is a method group and T is a delegate type or
            // SPEC:   expression tree type with parameter types T1...Tk and return
            // SPEC:   type Tb and overload resolution of E with the types T1...Tk
            // SPEC:   yields a single method with return type U then a lower-bound
            // SPEC:   inference is made from U to Tb.
            if (MethodGroupReturnTypeInference(binder, expression, target.Type, ref useSiteInfo))
            {
                return;
            }
            // SPEC: * Otherwise, if E is an expression with type U then a lower-bound
            // SPEC:   inference is made from U to T.
            var sourceType = _extensions.GetTypeWithAnnotations(expression);
            if (sourceType.HasType)
            {
                LowerBoundInference(sourceType, target, ref useSiteInfo);
            }
            // SPEC: * Otherwise, no inferences are made.
        }

        private bool InferredReturnTypeInference(BoundExpression source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: * If E is an anonymous function with inferred return type U and
            // SPEC:   T is a delegate type or expression tree with return type Tb
            // SPEC:   then a lower bound inference is made from U to Tb.

            var delegateType = target.Type.GetDelegateType();
            if (delegateType is null)
            {
                return false;
            }

            // cannot be hit, because an invalid delegate does not have an unfixed return type
            // this will be checked earlier.
            var returnType = delegateType.DelegateInvokeMethod.ReturnTypeWithAnnotations;
            if (!returnType.HasType || returnType.SpecialType == SpecialType.System_Void)
            {
                return false;
            }

            var inferredReturnType = InferReturnType(source, delegateType, ref useSiteInfo);
            if (!inferredReturnType.HasType)
            {
                return false;
            }

            LowerBoundInference(inferredReturnType, returnType, ref useSiteInfo);
            return true;
        }

        private bool MethodGroupReturnTypeInference(Binder binder, BoundExpression source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: * Otherwise, if E is a method group and T is a delegate type or
            // SPEC:   expression tree type with parameter types T1...Tk and return
            // SPEC:   type Tb and overload resolution of E with the types T1...Tk
            // SPEC:   yields a single method with return type U then a lower-bound
            // SPEC:   inference is made from U to Tb.

            if (source.Kind is not (BoundKind.MethodGroup or BoundKind.UnconvertedAddressOfOperator))
            {
                return false;
            }

            var delegateOrFunctionPointerType = target.GetDelegateOrFunctionPointerType();
            if (delegateOrFunctionPointerType is null)
            {
                return false;
            }


            if (delegateOrFunctionPointerType.IsFunctionPointer() != (source.Kind == BoundKind.UnconvertedAddressOfOperator))
            {
                return false;
            }

            // this part of the code is only called if the targetType has an unfixed type argument in the output 
            // type, which is not the case for invalid delegate invoke methods.
            var (method, isFunctionPointerResolution) = delegateOrFunctionPointerType switch
            {
                NamedTypeSymbol n => (n.DelegateInvokeMethod, false),
                FunctionPointerTypeSymbol f => (f.Signature, true),
                _ => throw ExceptionUtilities.UnexpectedValue(delegateOrFunctionPointerType),
            };

            TypeWithAnnotations sourceReturnType = method.ReturnTypeWithAnnotations;
            if (!sourceReturnType.HasType || sourceReturnType.SpecialType == SpecialType.System_Void)
            {
                return false;
            }

            // At this point we are in the second phase; we know that all the input types are fixed.

            var fixedParameters = GetFixedDelegateOrFunctionPointer(delegateOrFunctionPointerType).DelegateOrFunctionPointerParameters();
            if (fixedParameters.IsDefault)
            {
                return false;
            }

            CallingConventionInfo callingConventionInfo = isFunctionPointerResolution
                ? new CallingConventionInfo(method.CallingConvention, ((FunctionPointerMethodSymbol)method).GetCallingConventionModifiers())
                : default;
            BoundMethodGroup originalMethodGroup = source as BoundMethodGroup ?? ((BoundUnconvertedAddressOfOperator)source).Operand;

            var returnType = MethodGroupReturnType(binder, originalMethodGroup, fixedParameters, method.RefKind, isFunctionPointerResolution, ref useSiteInfo, in callingConventionInfo);
            if (returnType.IsDefault || returnType.IsVoidType())
            {
                return false;
            }

            LowerBoundInference(returnType, sourceReturnType, ref useSiteInfo);
            return true;
        }

        private TypeWithAnnotations MethodGroupReturnType(
            Binder binder, BoundMethodGroup source,
            ImmutableArray<ParameterSymbol> delegateParameters,
            RefKind delegateRefKind,
            bool isFunctionPointerResolution,
            ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo,
            in CallingConventionInfo callingConventionInfo)
        {
            var analyzedArguments = AnalyzedArguments.GetInstance();
            Conversions.GetDelegateOrFunctionPointerArguments(source.Syntax, analyzedArguments, delegateParameters, binder.Compilation);

            var resolution = binder.ResolveMethodGroup(source, analyzedArguments, useSiteInfo: ref useSiteInfo,
                isMethodGroupConversion: true, returnRefKind: delegateRefKind,
                // Since we are trying to infer the return type, it is not an input to resolving the method group
                returnType: null,
                isFunctionPointerResolution: isFunctionPointerResolution, callingConventionInfo: in callingConventionInfo);

            TypeWithAnnotations type = default;

            // The resolution could be empty (e.g. if there are no methods in the BoundMethodGroup).
            if (!resolution.IsEmpty)
            {
                var result = resolution.OverloadResolutionResult;
                if (result.Succeeded)
                {
                    type = _extensions.GetMethodGroupResultType(source, result.BestResult.Member);
                }
            }

            analyzedArguments.Free();
            resolution.Free();
            return type;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Explicit parameter type inferences
        //
        private void ExplicitParameterTypeInference(BoundExpression source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // SPEC: An explicit type parameter type inference is made from an expression
            // SPEC: E to a type T in the following way.
            // SPEC: If E is an explicitly typed anonymous function with parameter types
            // SPEC: U1...Uk and T is a delegate type or expression tree type with
            // SPEC: parameter types V1...Vk then for each Ui an exact inference is made
            // SPEC: from Ui to the corresponding Vi.

            if (source.Kind != BoundKind.UnboundLambda)
            {
                return;
            }

            UnboundLambda anonymousFunction = (UnboundLambda)source;

            if (!anonymousFunction.HasExplicitlyTypedParameterList)
            {
                return;
            }

            var delegateType = target.Type.GetDelegateType();
            if (delegateType is null)
            {
                return;
            }

            var delegateParameters = delegateType.DelegateParameters();
            if (delegateParameters.IsDefault)
            {
                return;
            }

            int size = delegateParameters.Length;
            if (anonymousFunction.ParameterCount < size)
            {
                size = anonymousFunction.ParameterCount;
            }

            // SPEC ISSUE: What should we do if there is an out/ref mismatch between an
            // SPEC ISSUE: anonymous function parameter and a delegate parameter?
            // SPEC ISSUE: The result will not be applicable no matter what, but should
            // SPEC ISSUE: we make any inferences?  This is going to be an error
            // SPEC ISSUE: ultimately, but it might make a difference for intellisense or
            // SPEC ISSUE: other analysis.

            for (int i = 0; i < size; ++i)
            {
                ExactInference(anonymousFunction.ParameterTypeWithAnnotations(i), delegateParameters[i].TypeWithAnnotations, ref useSiteInfo);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Exact inferences
        //
        private void ExactInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // SPEC: An exact inference from a type U to a type V is made as follows:

            // SPEC: * Otherwise, if U is the type U1? and V is the type V1? then an
            // SPEC:   exact inference is made from U to V.

            if (ExactNullableInference(source, target, ref useSiteInfo))
            {
                return;
            }

            // SPEC: * If V is one of the unfixed Xi then U is added to the set of
            // SPEC:   exact bounds for Xi.
            if (ExactTypeParameterInference(source, target))
            {
                return;
            }

            // SPEC: * Otherwise, if U is an array type UE[...] and V is an array type VE[...]
            // SPEC:   of the same rank then an exact inference from UE to VE is made.
            if (ExactArrayInference(source, target, ref useSiteInfo))
            {
                return;
            }

            // SPEC: * Otherwise, if V is a constructed type C<V1...Vk> and U is a constructed
            // SPEC:   type C<U1...Uk> then an exact inference is made
            // SPEC:    from each Ui to the corresponding Vi.

            if (ExactConstructedInference(source, target, ref useSiteInfo))
            {
                return;
            }

            // This can be valid via (where T : unmanaged) constraints
            if (ExactPointerInference(source, target, ref useSiteInfo))
            {
#pragma warning disable S3626 // Jump statements should not be redundant
                return;
#pragma warning restore S3626 // Jump statements should not be redundant
            }

            // SPEC: * Otherwise no inferences are made.
        }

        private bool ExactTypeParameterInference(TypeWithAnnotations source, TypeWithAnnotations target)
        {

            // SPEC: * If V is one of the unfixed Xi then U is added to the set of bounds
            // SPEC:   for Xi.
            if (IsUnfixedTypeParameter(target))
            {
                AddBound(source, _exactBounds, target);
                return true;
            }
            return false;
        }

        private bool ExactArrayInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // SPEC: * Otherwise, if U is an array type UE[...] and V is an array type VE[...]
            // SPEC:   of the same rank then an exact inference from UE to VE is made.
            if (!source.Type.IsArray() || !target.Type.IsArray())
            {
                return false;
            }

            var arraySource = (ArrayTypeSymbol)source.Type;
            var arrayTarget = (ArrayTypeSymbol)target.Type;
            if (!arraySource.HasSameShapeAs(arrayTarget))
            {
                return false;
            }

            ExactInference(arraySource.ElementTypeWithAnnotations, arrayTarget.ElementTypeWithAnnotations, ref useSiteInfo);
            return true;
        }

        private enum ExactOrBoundsKind
        {
            Exact,
            LowerBound,
            UpperBound,
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

            // True if the type is nullable.
            static bool isNullableOnly(TypeWithAnnotations type)
                => type.NullableAnnotation.IsAnnotated();
        }

        private bool ExactNullableInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ExactOrBoundsNullableInference(ExactOrBoundsKind.Exact, source, target, ref useSiteInfo);
        }

        private bool LowerBoundTupleInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // NOTE: we are losing tuple element names when unwrapping tuple types to underlying types.
            //       that is ok, because we are inferring type parameters used in the matching elements, 
            //       This is not the situation where entire tuple type used to infer a single type param


            if (!source.Type.TryGetElementTypesWithAnnotationsIfTupleType(out ImmutableArray<TypeWithAnnotations> sourceTypes) ||
                !target.Type.TryGetElementTypesWithAnnotationsIfTupleType(out ImmutableArray<TypeWithAnnotations> targetTypes) ||
                sourceTypes.Length != targetTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < sourceTypes.Length; i++)
            {
                LowerBoundInference(sourceTypes[i], targetTypes[i], ref useSiteInfo);
            }

            return true;
        }

        private bool ExactConstructedInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: * Otherwise, if V is a constructed type C<V1...Vk> and U is a constructed
            // SPEC:   type C<U1...Uk> then an exact inference 
            // SPEC:   is made from each Ui to the corresponding Vi.

            if (source.Type is not NamedTypeSymbol namedSource)
            {
                return false;
            }

            if (target.Type is not NamedTypeSymbol namedTarget)
            {
                return false;
            }

            if (!TypeSymbol.Equals(namedSource.OriginalDefinition, namedTarget.OriginalDefinition, TypeCompareKind.ConsiderEverything2))
            {
                return false;
            }

            ExactTypeArgumentInference(namedSource, namedTarget, ref useSiteInfo);
            return true;
        }

        private bool ExactPointerInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.TypeKind == TypeKind.Pointer && target.TypeKind == TypeKind.Pointer)
            {
                ExactInference(((PointerTypeSymbol)source.Type).PointedAtTypeWithAnnotations, ((PointerTypeSymbol)target.Type).PointedAtTypeWithAnnotations, ref useSiteInfo);
                return true;
            }
            else if (source.Type is FunctionPointerTypeSymbol { Signature: { ParameterCount: int sourceParameterCount } sourceSignature } &&
                     target.Type is FunctionPointerTypeSymbol { Signature: { ParameterCount: int targetParameterCount } targetSignature } &&
                     sourceParameterCount == targetParameterCount)
            {
                if (!FunctionPointerRefKindsEqual(sourceSignature, targetSignature) || !FunctionPointerCallingConventionsEqual(sourceSignature, targetSignature))
                {
                    return false;
                }

                for (int i = 0; i < sourceParameterCount; i++)
                {
                    ExactInference(sourceSignature.ParameterTypesWithAnnotations[i], targetSignature.ParameterTypesWithAnnotations[i], ref useSiteInfo);
                }

                ExactInference(sourceSignature.ReturnTypeWithAnnotations, targetSignature.ReturnTypeWithAnnotations, ref useSiteInfo);
                return true;
            }

            return false;
        }

        private static bool FunctionPointerCallingConventionsEqual(FunctionPointerMethodSymbol sourceSignature, FunctionPointerMethodSymbol targetSignature)
        {
            if (sourceSignature.CallingConvention != targetSignature.CallingConvention)
            {
                return false;
            }

            return (sourceSignature.GetCallingConventionModifiers(), targetSignature.GetCallingConventionModifiers()) switch
            {
                (null, null) => true,
                ({ } sourceModifiers, { } targetModifiers) when sourceModifiers.SetEquals(targetModifiers) => true,
                _ => false
            };
        }

        private static bool FunctionPointerRefKindsEqual(FunctionPointerMethodSymbol sourceSignature, FunctionPointerMethodSymbol targetSignature)
        {
            return sourceSignature.RefKind == targetSignature.RefKind
                   && (sourceSignature.ParameterRefKinds.IsDefault, targetSignature.ParameterRefKinds.IsDefault) switch
                   {
                       (true, false) or (false, true) => false,
                       (true, true) => true,
                       _ => sourceSignature.ParameterRefKinds.SequenceEqual(targetSignature.ParameterRefKinds)
                   };
        }

        private void ExactTypeArgumentInference(NamedTypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            var sourceTypeArguments = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            var targetTypeArguments = ArrayBuilder<TypeWithAnnotations>.GetInstance();

            source.GetAllTypeArguments(sourceTypeArguments, ref useSiteInfo);
            target.GetAllTypeArguments(targetTypeArguments, ref useSiteInfo);


            for (int arg = 0; arg < sourceTypeArguments.Count; ++arg)
            {
                ExactInference(sourceTypeArguments[arg], targetTypeArguments[arg], ref useSiteInfo);
            }

            sourceTypeArguments.Free();
            targetTypeArguments.Free();
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Lower-bound inferences
        //
        private void LowerBoundInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // SPEC: A lower-bound inference from a type U to a type V is made as follows:

            // SPEC: * Otherwise, if V is nullable type V1? and U is nullable type U1?
            // SPEC:   then an exact inference is made from U1 to V1.

            // SPEC ERROR: The spec should say "lower" here; we can safely make a lower-bound
            // SPEC ERROR: inference to nullable even though it is a generic struct.  That is,
            // SPEC ERROR: if we have M<T>(T?, T) called with (char?, int) then we can infer
            // SPEC ERROR: lower bounds of char and int, and choose int. If we make an exact
            // SPEC ERROR: inference of char then type inference fails.

            if (LowerBoundNullableInference(source, target, ref useSiteInfo))
            {
                return;
            }

            // SPEC: * If V is one of the unfixed Xi then U is added to the set of 
            // SPEC:   lower bounds for Xi.

            if (LowerBoundTypeParameterInference(source, target))
            {
                return;
            }

            // SPEC: * Otherwise, if U is an array type Ue[...] and V is either an array
            // SPEC:   type Ve[...] of the same rank, or if U is a one-dimensional array
            // SPEC:   type Ue[] and V is one of IEnumerable<Ve>, ICollection<Ve> or
            // SPEC:   IList<Ve> then
            // SPEC:   * if Ue is known to be a reference type then a lower-bound inference
            // SPEC:     from Ue to Ve is made.
            // SPEC:   * otherwise an exact inference from Ue to Ve is made.

            if (LowerBoundArrayInference(source.Type, target.Type, ref useSiteInfo))
            {
                return;
            }

            // UNDONE: At this point we could also do an inference from non-nullable U
            // UNDONE: to nullable V.
            // UNDONE: 
            // UNDONE: We tried implementing lower bound nullable inference as follows:
            // UNDONE:
            // UNDONE: * Otherwise, if V is nullable type V1? and U is a non-nullable 
            // UNDONE:   struct type then an exact inference is made from U to V1.
            // UNDONE:
            // UNDONE: However, this causes an unfortunate interaction with what
            // UNDONE: looks like a bug in our implementation of section 15.2 of
            // UNDONE: the specification. Namely, it appears that the code which
            // UNDONE: checks whether a given method is compatible with
            // UNDONE: a delegate type assumes that if method type inference succeeds,
            // UNDONE: then the inferred types are compatible with the delegate types.
            // UNDONE: This is not necessarily so; the inferred types could be compatible
            // UNDONE: via a conversion other than reference or identity.
            // UNDONE:
            // UNDONE: We should take an action item to investigate this problem.
            // UNDONE: Until then, we will turn off the proposed lower bound nullable
            // UNDONE: inference.

            if (LowerBoundTupleInference(source, target, ref useSiteInfo))
            {
                return;
            }

            // SPEC: Otherwise... many cases for constructed generic types.
            if (LowerBoundConstructedInference(source.Type, target.Type, ref useSiteInfo))
            {
                return;
            }

            if (LowerBoundFunctionPointerTypeInference(source.Type, target.Type, ref useSiteInfo))
            {
#pragma warning disable S3626 // Jump statements should not be redundant
                return;
#pragma warning restore S3626 // Jump statements should not be redundant
            }

            // SPEC: * Otherwise, no inferences are made.
        }

        private bool LowerBoundTypeParameterInference(TypeWithAnnotations source, TypeWithAnnotations target)
        {

            // SPEC: * If V is one of the unfixed Xi then U is added to the set of bounds
            // SPEC:   for Xi.
            if (IsUnfixedTypeParameter(target))
            {
                AddBound(source, _lowerBounds, target);
                return true;
            }
            return false;
        }

        private static TypeWithAnnotations GetMatchingElementType(ArrayTypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // It might be an array of same rank.
            if (target.IsArray())
            {
                var arrayTarget = (ArrayTypeSymbol)target;
                if (!arrayTarget.HasSameShapeAs(source))
                {
                    return default;
                }
                return arrayTarget.ElementTypeWithAnnotations;
            }

            // Or it might be IEnum<T> and source is rank one.

            if (!source.IsSZArray)
            {
                return default;
            }

            // Arrays are specified as being convertible to IEnumerable<T>, ICollection<T> and
            // IList<T>; we also honor their convertibility to IReadOnlyCollection<T> and
            // IReadOnlyList<T>, and make inferences accordingly.

            if (!target.IsPossibleArrayGenericInterface())
            {
                return default;
            }

            return ((NamedTypeSymbol)target).TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref useSiteInfo);
        }

        private bool LowerBoundArrayInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // SPEC: * Otherwise, if U is an array type Ue[...] and V is either an array
            // SPEC:   type Ve[...] of the same rank, or if U is a one-dimensional array
            // SPEC:   type Ue[] and V is one of IEnumerable<Ve>, ICollection<Ve> or
            // SPEC:   IList<Ve> then
            // SPEC:   * if Ue is known to be a reference type then a lower-bound inference
            // SPEC:     from Ue to Ve is made.
            // SPEC:   * otherwise an exact inference from Ue to Ve is made.

            if (!source.IsArray())
            {
                return false;
            }

            var arraySource = (ArrayTypeSymbol)source;
            var elementSource = arraySource.ElementTypeWithAnnotations;
            var elementTarget = GetMatchingElementType(arraySource, target, ref useSiteInfo);
            if (!elementTarget.HasType)
            {
                return false;
            }

            if (elementSource.Type.IsReferenceType)
            {
                LowerBoundInference(elementSource, elementTarget, ref useSiteInfo);
            }
            else
            {
                ExactInference(elementSource, elementTarget, ref useSiteInfo);
            }

            return true;
        }

        private bool LowerBoundNullableInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ExactOrBoundsNullableInference(ExactOrBoundsKind.LowerBound, source, target, ref useSiteInfo);
        }

        private bool LowerBoundConstructedInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (target is not NamedTypeSymbol constructedTarget)
            {
                return false;
            }

            if (constructedTarget.AllTypeArgumentCount() == 0)
            {
                return false;
            }

            // SPEC: * Otherwise, if V is a constructed class or struct type C<V1...Vk> 
            // SPEC:   and U is C<U1...Uk> then an exact inference
            // SPEC:   is made from each Ui to the corresponding Vi.

            // SPEC: * Otherwise, if V is a constructed interface or delegate type C<V1...Vk> 
            // SPEC:   and U is C<U1...Uk> then an exact inference,
            // SPEC:   lower bound inference or upper bound inference
            // SPEC:   is made from each Ui to the corresponding Vi.

            if (source is NamedTypeSymbol constructedSource &&
                TypeSymbol.Equals(constructedSource.OriginalDefinition, constructedTarget.OriginalDefinition, TypeCompareKind.ConsiderEverything2))
            {
                if (constructedSource.IsInterface || constructedSource.IsDelegateType())
                {
                    LowerBoundTypeArgumentInference(constructedSource, constructedTarget, ref useSiteInfo);
                }
                else
                {
                    ExactTypeArgumentInference(constructedSource, constructedTarget, ref useSiteInfo);
                }
                return true;
            }

            // SPEC: * Otherwise, if V is a class type C<V1...Vk> and U is a class type which
            // SPEC:   inherits directly or indirectly from C<U1...Uk> then an exact ...
            // SPEC: * ... and U is a type parameter with effective base class ...
            // SPEC: * ... and U is a type parameter with an effective base class which inherits ...

            if (LowerBoundClassInference(source, constructedTarget, ref useSiteInfo))
            {
                return true;
            }

            // SPEC: * Otherwise, if V is an interface type C<V1...Vk> and U is a class type
            // SPEC:   or struct type and there is a unique set U1...Uk such that U directly 
            // SPEC:   or indirectly implements C<U1...Uk> then an exact ...
            // SPEC: * ... and U is an interface type ...
            // SPEC: * ... and U is a type parameter ...

            if (LowerBoundInterfaceInference(source, constructedTarget, ref useSiteInfo))
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

            // Spec: 7.5.2.9 Lower-bound interfaces 
            // SPEC: * Otherwise, if V is a class type C<V1...Vk> and U is a class type which
            // SPEC:   inherits directly or indirectly from C<U1...Uk> 
            // SPEC:   then an exact inference is made from each Ui to the corresponding Vi.
            // SPEC: * Otherwise, if V is a class type C<V1...Vk> and U is a type parameter
            // SPEC:   with effective base class C<U1...Uk> 
            // SPEC:   then an exact inference is made from each Ui to the corresponding Vi.
            // SPEC: * Otherwise, if V is a class type C<V1...Vk> and U is a type parameter
            // SPEC:   with an effective base class which inherits directly or indirectly from
            // SPEC:   C<U1...Uk> then an exact inference is made
            // SPEC:   from each Ui to the corresponding Vi.

            NamedTypeSymbol sourceBase = null;

            if (source.TypeKind == TypeKind.Class)
            {
                sourceBase = source.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            else if (source.TypeKind == TypeKind.TypeParameter)
            {
                sourceBase = ((TypeParameterSymbol)source).EffectiveBaseClass(ref useSiteInfo);
            }

            while (sourceBase is not null)
            {
                if (TypeSymbol.Equals(sourceBase.OriginalDefinition, target.OriginalDefinition, TypeCompareKind.ConsiderEverything2))
                {
                    ExactTypeArgumentInference(sourceBase, target, ref useSiteInfo);
                    return true;
                }
                sourceBase = sourceBase.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            return false;
        }

        private bool LowerBoundInterfaceInference(TypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            if (!target.IsInterface)
            {
                return false;
            }

            // Spec 7.5.2.9 Lower-bound interfaces
            // SPEC: * Otherwise, if V [target] is an interface type C<V1...Vk> and U [source] is a class type
            // SPEC:   or struct type and there is a unique set U1...Uk such that U directly 
            // SPEC:   or indirectly implements C<U1...Uk> then an
            // SPEC:   exact, upper-bound, or lower-bound inference ...
            // SPEC: * ... and U is an interface type ...
            // SPEC: * ... and U is a type parameter ...

            ImmutableArray<NamedTypeSymbol> allInterfaces;
            switch (source.TypeKind)
            {
                case TypeKind.Struct:
                case TypeKind.Class:
                case TypeKind.Interface:
                    allInterfaces = source.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                    break;

                case TypeKind.TypeParameter:
                    var typeParameter = (TypeParameterSymbol)source;
                    allInterfaces = typeParameter.EffectiveBaseClass(ref useSiteInfo).
                                        AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).
                                        Concat(typeParameter.AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo));
                    break;

                default:
                    return false;
            }

            // duplicates with only nullability differences can be merged (to avoid an error in type inference)
            allInterfaces = ModuloReferenceTypeNullabilityDifferences(allInterfaces, VarianceKind.In);

            NamedTypeSymbol matchingInterface = GetInterfaceInferenceBound(allInterfaces, target);
            if (matchingInterface is null)
            {
                return false;
            }
            LowerBoundTypeArgumentInference(matchingInterface, target, ref useSiteInfo);
            return true;
        }

        internal static ImmutableArray<NamedTypeSymbol> ModuloReferenceTypeNullabilityDifferences(ImmutableArray<NamedTypeSymbol> interfaces, VarianceKind variance)
        {
            var dictionary = PooledDictionaryIgnoringNullableModifiersForReferenceTypes.GetInstance();

            foreach (var @interface in interfaces)
            {
                if (dictionary.TryGetValue(@interface, out var found))
                {
                    var merged = (NamedTypeSymbol)found.MergeEquivalentTypes(@interface, variance);
                    dictionary[@interface] = merged;
                }
                else
                {
                    dictionary.Add(@interface, @interface);
                }
            }

            var result = dictionary.Count != interfaces.Length ? dictionary.Values.ToImmutableArray() : interfaces;
            dictionary.Free();
            return result;
        }

        private void LowerBoundTypeArgumentInference(NamedTypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: The choice of inference for the i-th type argument is made
            // SPEC: based on the declaration of the i-th type parameter of C, as
            // SPEC: follows:
            // SPEC: * if Ui is known to be of reference type and the i-th type parameter
            // SPEC:   was declared as covariant then a lower bound inference is made.
            // SPEC: * if Ui is known to be of reference type and the i-th type parameter
            // SPEC:   was declared as contravariant then an upper bound inference is made.
            // SPEC: * otherwise, an exact inference is made.


            var typeParameters = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            var sourceTypeArguments = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            var targetTypeArguments = ArrayBuilder<TypeWithAnnotations>.GetInstance();

            source.OriginalDefinition.GetAllTypeParameters(typeParameters);
            source.GetAllTypeArguments(sourceTypeArguments, ref useSiteInfo);
            target.GetAllTypeArguments(targetTypeArguments, ref useSiteInfo);


            for (int arg = 0; arg < sourceTypeArguments.Count; ++arg)
            {
                var typeParameter = typeParameters[arg];
                var sourceTypeArgument = sourceTypeArguments[arg];
                var targetTypeArgument = targetTypeArguments[arg];

                if (sourceTypeArgument.Type.IsReferenceType && typeParameter.Variance == VarianceKind.Out)
                {
                    LowerBoundInference(sourceTypeArgument, targetTypeArgument, ref useSiteInfo);
                }
                else if (sourceTypeArgument.Type.IsReferenceType && typeParameter.Variance == VarianceKind.In)
                {
                    UpperBoundInference(sourceTypeArgument, targetTypeArgument, ref useSiteInfo);
                }
                else
                {
                    ExactInference(sourceTypeArgument, targetTypeArgument, ref useSiteInfo);
                }
            }

            typeParameters.Free();
            sourceTypeArguments.Free();
            targetTypeArguments.Free();
        }

#nullable enable
        private bool LowerBoundFunctionPointerTypeInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source is not FunctionPointerTypeSymbol { Signature: { } sourceSignature } || target is not FunctionPointerTypeSymbol { Signature: { } targetSignature })
            {
                return false;
            }

            if (sourceSignature.ParameterCount != targetSignature.ParameterCount)
            {
                return false;
            }

            if (!FunctionPointerRefKindsEqual(sourceSignature, targetSignature) || !FunctionPointerCallingConventionsEqual(sourceSignature, targetSignature))
            {
                return false;
            }

            // Reference parameters are treated as "input" variance by default, and reference return types are treated as out variance by default.
            // If they have a ref kind or are not reference types, then they are treated as invariant.
            for (int i = 0; i < sourceSignature.ParameterCount; i++)
            {
                var sourceParam = sourceSignature.Parameters[i];
                var targetParam = targetSignature.Parameters[i];

                if ((sourceParam.Type.IsReferenceType || sourceParam.Type.IsFunctionPointer()) && sourceParam.RefKind == RefKind.None)
                {
                    UpperBoundInference(sourceParam.TypeWithAnnotations, targetParam.TypeWithAnnotations, ref useSiteInfo);
                }
                else
                {
                    ExactInference(sourceParam.TypeWithAnnotations, targetParam.TypeWithAnnotations, ref useSiteInfo);
                }
            }

            if ((sourceSignature.ReturnType.IsReferenceType || sourceSignature.ReturnType.IsFunctionPointer()) && sourceSignature.RefKind == RefKind.None)
            {
                LowerBoundInference(sourceSignature.ReturnTypeWithAnnotations, targetSignature.ReturnTypeWithAnnotations, ref useSiteInfo);
            }
            else
            {
                ExactInference(sourceSignature.ReturnTypeWithAnnotations, targetSignature.ReturnTypeWithAnnotations, ref useSiteInfo);
            }

            return true;
        }
#nullable disable

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Upper-bound inferences
        //
        private void UpperBoundInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // SPEC: An upper-bound inference from a type U to a type V is made as follows:

            // SPEC: * Otherwise, if V is nullable type V1? and U is nullable type U1?
            // SPEC:   then an exact inference is made from U1 to V1.

            if (UpperBoundNullableInference(source, target, ref useSiteInfo))
            {
                return;
            }

            // SPEC: * If V is one of the unfixed Xi then U is added to the set of 
            // SPEC:   upper bounds for Xi.

            if (UpperBoundTypeParameterInference(source, target))
            {
                return;
            }

            // SPEC: * Otherwise, if V is an array type Ve[...] and U is an array
            // SPEC:   type Ue[...] of the same rank, or if V is a one-dimensional array
            // SPEC:   type Ve[] and U is one of IEnumerable<Ue>, ICollection<Ue> or
            // SPEC:   IList<Ue> then
            // SPEC:   * if Ue is known to be a reference type then an upper-bound inference
            // SPEC:     from Ue to Ve is made.
            // SPEC:   * otherwise an exact inference from Ue to Ve is made.

            if (UpperBoundArrayInference(source, target, ref useSiteInfo))
            {
                return;
            }


            // NOTE: spec would ask us to do the following checks, but since the value types
            //       are trivially handled as exact inference in the callers, we do not have to.

#pragma warning disable S125 // Sections of code should not be commented out
            //if (ExactTupleInference(source, target, ref useSiteInfo))
            //{
            //    return;
            //}
#pragma warning restore S125 // Sections of code should not be commented out

            // SPEC: * Otherwise... cases for constructed types

            if (UpperBoundConstructedInference(source, target, ref useSiteInfo))
            {
                return;
            }

            if (UpperBoundFunctionPointerTypeInference(source.Type, target.Type, ref useSiteInfo))
            {
#pragma warning disable S3626 // Jump statements should not be redundant
                return;
#pragma warning restore S3626 // Jump statements should not be redundant
            }

            // SPEC: * Otherwise, no inferences are made.
        }

        private bool UpperBoundTypeParameterInference(TypeWithAnnotations source, TypeWithAnnotations target)
        {
            // SPEC: * If V is one of the unfixed Xi then U is added to the set of upper bounds
            // SPEC:   for Xi.
            if (IsUnfixedTypeParameter(target))
            {
                AddBound(source, _upperBounds, target);
                return true;
            }
            return false;
        }

        private bool UpperBoundArrayInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // SPEC: * Otherwise, if V is an array type Ve[...] and U is an array
            // SPEC:   type Ue[...] of the same rank, or if V is a one-dimensional array
            // SPEC:   type Ve[] and U is one of IEnumerable<Ue>, ICollection<Ue> or
            // SPEC:   IList<Ue> then
            // SPEC:   * if Ue is known to be a reference type then an upper-bound inference
            // SPEC:     from Ue to Ve is made.
            // SPEC:   * otherwise an exact inference from Ue to Ve is made.

            if (!target.Type.IsArray())
            {
                return false;
            }
            var arrayTarget = (ArrayTypeSymbol)target.Type;
            var elementTarget = arrayTarget.ElementTypeWithAnnotations;
            var elementSource = GetMatchingElementType(arrayTarget, source.Type, ref useSiteInfo);
            if (!elementSource.HasType)
            {
                return false;
            }

            if (elementSource.Type.IsReferenceType)
            {
                UpperBoundInference(elementSource, elementTarget, ref useSiteInfo);
            }
            else
            {
                ExactInference(elementSource, elementTarget, ref useSiteInfo);
            }

            return true;
        }

        private bool UpperBoundNullableInference(TypeWithAnnotations source, TypeWithAnnotations target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ExactOrBoundsNullableInference(ExactOrBoundsKind.UpperBound, source, target, ref useSiteInfo);
        }

        private bool UpperBoundConstructedInference(TypeWithAnnotations sourceWithAnnotations, TypeWithAnnotations targetWithAnnotations, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            var source = sourceWithAnnotations.Type;
            var target = targetWithAnnotations.Type;

            if (source is not NamedTypeSymbol constructedSource)
            {
                return false;
            }

            if (constructedSource.AllTypeArgumentCount() == 0)
            {
                return false;
            }

            // SPEC: * Otherwise, if V is a constructed type C<V1...Vk> and U is
            // SPEC:   C<U1...Uk> then an exact inference,
            // SPEC:   lower bound inference or upper bound inference
            // SPEC:   is made from each Ui to the corresponding Vi.


            if (target is NamedTypeSymbol constructedTarget &&
                TypeSymbol.Equals(constructedSource.OriginalDefinition, target.OriginalDefinition, TypeCompareKind.ConsiderEverything2))
            {
                if (constructedTarget.IsInterface || constructedTarget.IsDelegateType())
                {
                    UpperBoundTypeArgumentInference(constructedSource, constructedTarget, ref useSiteInfo);
                }
                else
                {
                    ExactTypeArgumentInference(constructedSource, constructedTarget, ref useSiteInfo);
                }
                return true;
            }

            // SPEC: * Otherwise, if U is a class type C<U1...Uk> and V is a class type which
            // SPEC:   inherits directly or indirectly from C<V1...Vk> then an exact ...

            if (UpperBoundClassInference(constructedSource, target, ref useSiteInfo))
            {
                return true;
            }

            // SPEC: * Otherwise, if U is an interface type C<U1...Uk> and V is a class type
            // SPEC:   or struct type and there is a unique set V1...Vk such that V directly 
            // SPEC:   or indirectly implements C<V1...Vk> then an exact ...
            // SPEC: * ... and U is an interface type ...

            if (UpperBoundInterfaceInference(constructedSource, target, ref useSiteInfo))
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

            // SPEC: * Otherwise, if U is a class type C<U1...Uk> and V is a class type which
            // SPEC:   inherits directly or indirectly from C<V1...Vk> then an exact 
            // SPEC:   inference is made from each Ui to the corresponding Vi.

            var targetBase = target.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            while (targetBase is not null)
            {
                if (TypeSymbol.Equals(targetBase.OriginalDefinition, source.OriginalDefinition, TypeCompareKind.ConsiderEverything2))
                {
                    ExactTypeArgumentInference(source, targetBase, ref useSiteInfo);
                    return true;
                }

                targetBase = targetBase.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }

            return false;
        }

        private bool UpperBoundInterfaceInference(NamedTypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            if (!source.IsInterface)
            {
                return false;
            }

            // SPEC: * Otherwise, if U [source] is an interface type C<U1...Uk> and V [target] is a class type
            // SPEC:   or struct type and there is a unique set V1...Vk such that V directly 
            // SPEC:   or indirectly implements C<V1...Vk> then an exact ...
            // SPEC: * ... and U is an interface type ...

            switch (target.TypeKind)
            {
                case TypeKind.Struct:
                case TypeKind.Class:
                case TypeKind.Interface:
                    break;

                default:
                    return false;
            }

            ImmutableArray<NamedTypeSymbol> allInterfaces = target.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);

            // duplicates with only nullability differences can be merged (to avoid an error in type inference)
            allInterfaces = ModuloReferenceTypeNullabilityDifferences(allInterfaces, VarianceKind.Out);

            NamedTypeSymbol bestInterface = GetInterfaceInferenceBound(allInterfaces, source);
            if (bestInterface is null)
            {
                return false;
            }

            UpperBoundTypeArgumentInference(source, bestInterface, ref useSiteInfo);
            return true;
        }

        private void UpperBoundTypeArgumentInference(NamedTypeSymbol source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC: The choice of inference for the i-th type argument is made
            // SPEC: based on the declaration of the i-th type parameter of C, as
            // SPEC: follows:
            // SPEC: * if Ui is known to be of reference type and the i-th type parameter
            // SPEC:   was declared as covariant then an upper-bound inference is made.
            // SPEC: * if Ui is known to be of reference type and the i-th type parameter
            // SPEC:   was declared as contravariant then a lower-bound inference is made.
            // SPEC: * otherwise, an exact inference is made.


            var typeParameters = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            var sourceTypeArguments = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            var targetTypeArguments = ArrayBuilder<TypeWithAnnotations>.GetInstance();

            source.OriginalDefinition.GetAllTypeParameters(typeParameters);
            source.GetAllTypeArguments(sourceTypeArguments, ref useSiteInfo);
            target.GetAllTypeArguments(targetTypeArguments, ref useSiteInfo);


            for (int arg = 0; arg < sourceTypeArguments.Count; ++arg)
            {
                var typeParameter = typeParameters[arg];
                var sourceTypeArgument = sourceTypeArguments[arg];
                var targetTypeArgument = targetTypeArguments[arg];

                if (sourceTypeArgument.Type.IsReferenceType && typeParameter.Variance == VarianceKind.Out)
                {
                    UpperBoundInference(sourceTypeArgument, targetTypeArgument, ref useSiteInfo);
                }
                else if (sourceTypeArgument.Type.IsReferenceType && typeParameter.Variance == VarianceKind.In)
                {
                    LowerBoundInference(sourceTypeArgument, targetTypeArgument, ref useSiteInfo);
                }
                else
                {
                    ExactInference(sourceTypeArgument, targetTypeArgument, ref useSiteInfo);
                }
            }

            typeParameters.Free();
            sourceTypeArguments.Free();
            targetTypeArguments.Free();
        }

#nullable enable
        private bool UpperBoundFunctionPointerTypeInference(TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source is not FunctionPointerTypeSymbol { Signature: { } sourceSignature } || target is not FunctionPointerTypeSymbol { Signature: { } targetSignature })
            {
                return false;
            }

            if (sourceSignature.ParameterCount != targetSignature.ParameterCount)
            {
                return false;
            }

            if (!FunctionPointerRefKindsEqual(sourceSignature, targetSignature) || !FunctionPointerCallingConventionsEqual(sourceSignature, targetSignature))
            {
                return false;
            }

            // Reference parameters are treated as "input" variance by default, and reference return types are treated as out variance by default.
            // If they have a ref kind or are not reference types, then they are treated as invariant.
            for (int i = 0; i < sourceSignature.ParameterCount; i++)
            {
                var sourceParam = sourceSignature.Parameters[i];
                var targetParam = targetSignature.Parameters[i];

                if ((sourceParam.Type.IsReferenceType || sourceParam.Type.IsFunctionPointer()) && sourceParam.RefKind == RefKind.None)
                {
                    LowerBoundInference(sourceParam.TypeWithAnnotations, targetParam.TypeWithAnnotations, ref useSiteInfo);
                }
                else
                {
                    ExactInference(sourceParam.TypeWithAnnotations, targetParam.TypeWithAnnotations, ref useSiteInfo);
                }
            }

            if ((sourceSignature.ReturnType.IsReferenceType || sourceSignature.ReturnType.IsFunctionPointer()) && sourceSignature.RefKind == RefKind.None)
            {
                UpperBoundInference(sourceSignature.ReturnTypeWithAnnotations, targetSignature.ReturnTypeWithAnnotations, ref useSiteInfo);
            }
            else
            {
                ExactInference(sourceSignature.ReturnTypeWithAnnotations, targetSignature.ReturnTypeWithAnnotations, ref useSiteInfo);
            }

            return true;
        }
#nullable disable

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Fixing
        //
        private bool Fix(int iParam, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            var exact = _exactBounds[iParam];
            var lower = _lowerBounds[iParam];
            var upper = _upperBounds[iParam];

            var best = Fix(exact, lower, upper, ref useSiteInfo, _conversions);
            if (!best.HasType)
            {
                return false;
            }

#if DEBUG
            if (_conversions.IncludeNullability)
            {
                // If the first attempt succeeded, the result should be the same as
                // the second attempt, although perhaps with different nullability.
                var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                Fix(exact, lower, upper, ref discardedUseSiteInfo, _conversions.WithNullability(false));
                // https://github.com/dotnet/roslyn/issues/27961 Results may differ by tuple names or dynamic.
                // See NullableReferenceTypesTests.TypeInference_TupleNameDifferences_01 for example.
            }
#endif

            _fixedResults[iParam] = best;
            UpdateDependenciesAfterFix(iParam);
            return true;
        }

        private static TypeWithAnnotations Fix(
            HashSet<TypeWithAnnotations> exact,
            HashSet<TypeWithAnnotations> lower,
            HashSet<TypeWithAnnotations> upper,
            ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo,
            ConversionsBase conversions)
        {
            // UNDONE: This method makes a lot of garbage.

            // SPEC: An unfixed type parameter with a set of bounds is fixed as follows:

            // SPEC: * The set of candidate types starts out as the set of all types in
            // SPEC:   the bounds.

            // SPEC: * We then examine each bound in turn. For each exact bound U of Xi,
            // SPEC:   all types which are not identical to U are removed from the candidate set.

            // Optimization: if we have two or more exact bounds, fixing is impossible.

            var candidates = new Dictionary<TypeWithAnnotations, TypeWithAnnotations>(EqualsIgnoringDynamicTupleNamesAndNullabilityComparer.Instance);

            // Optimization: if we have one exact bound then we need not add any
            // inexact bounds; we're just going to remove them anyway.

            if (exact == null)
            {
                if (lower != null)
                {
                    // Lower bounds represent co-variance.
                    AddAllCandidates(candidates, lower, VarianceKind.Out, conversions);
                }
                if (upper != null)
                {
                    // Lower bounds represent contra-variance.
                    AddAllCandidates(candidates, upper, VarianceKind.In, conversions);
                }
            }
            else
            {
                // Exact bounds represent invariance.
                AddAllCandidates(candidates, exact, VarianceKind.None, conversions);
                if (candidates.Count >= 2)
                {
                    return default;
                }
            }

            if (candidates.Count == 0)
            {
                return default;
            }

            // Don't mutate the collection as we're iterating it.
            var initialCandidates = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            GetAllCandidates(candidates, initialCandidates);

            // SPEC:   For each lower bound U of Xi all types to which there is not an
            // SPEC:   implicit conversion from U are removed from the candidate set.

            if (lower != null)
            {
                MergeOrRemoveCandidates(candidates, lower, initialCandidates, conversions, VarianceKind.Out, ref useSiteInfo);
            }

            // SPEC:   For each upper bound U of Xi all types from which there is not an
            // SPEC:   implicit conversion to U are removed from the candidate set.

            if (upper != null)
            {
                MergeOrRemoveCandidates(candidates, upper, initialCandidates, conversions, VarianceKind.In, ref useSiteInfo);
            }

            initialCandidates.Clear();
            GetAllCandidates(candidates, initialCandidates);

            // SPEC: * If among the remaining candidate types there is a unique type V to
            // SPEC:   which there is an implicit conversion from all the other candidate
            // SPEC:   types, then the parameter is fixed to V.
            TypeWithAnnotations best = default;
            foreach (var candidate in initialCandidates)
            {
                foreach (var candidate2 in initialCandidates)
                {
                    if (!candidate.Equals(candidate2, TypeCompareKind.ConsiderEverything) &&
                        !ImplicitConversionExists(candidate2, candidate, ref useSiteInfo, conversions.WithNullability(false)))
                    {
                        // Nothing to do
                    }
                    else
                    {
                        if (!best.HasType)
                        {
                            best = candidate;
                        }
                        else
                        {
                            // best candidate is not unique
                            best = default;
                            break;
                        }
                    }
                }
            }

            initialCandidates.Free();

            return best;
        }

        private static bool ImplicitConversionExists(TypeWithAnnotations sourceWithAnnotations, TypeWithAnnotations destinationWithAnnotations, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConversionsBase conversions)
        {
            var source = sourceWithAnnotations.Type;
            var destination = destinationWithAnnotations.Type;

            // SPEC VIOLATION: For the purpose of algorithm in Fix method, dynamic type is not considered convertible to any other type, including object.
            if (source.IsDynamic() && !destination.IsDynamic())
            {
                return false;
            }

            if (!conversions.HasTopLevelNullabilityImplicitConversion(sourceWithAnnotations, destinationWithAnnotations))
            {
                return false;
            }

            return conversions.ClassifyImplicitConversionFromType(source, destination, ref useSiteInfo).Exists;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Inferred return type
        //
        private TypeWithAnnotations InferReturnType(BoundExpression source, NamedTypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // We should not be computing the inferred return type unless we are converting
            // to a delegate type where all the input types are fixed.

            // Spec 7.5.2.12: Inferred return type:
            // The inferred return type of an anonymous function F is used during
            // type inference and overload resolution. The inferred return type
            // can only be determined for an anonymous function where all parameter
            // types are known, either because they are explicitly given, provided
            // through an anonymous function conversion, or inferred during type
            // inference on an enclosing generic method invocation.
            // The inferred return type is determined as follows:
            // * If the body of F is an expression (that has a type) then the 
            //   inferred return type of F is the type of that expression.
            // * If the body of F is a block and the set of expressions in the
            //   blocks return statements has a best common type T then the
            //   inferred return type of F is T.
            // * Otherwise, a return type cannot be inferred for F.

            if (source.Kind != BoundKind.UnboundLambda)
            {
                return default;
            }

            var anonymousFunction = (UnboundLambda)source;
            if (anonymousFunction.HasSignature)
            {
                // Optimization: 
                // We know that the anonymous function has a parameter list. If it does not
                // have the same arity as the delegate, then it cannot possibly be applicable.
                // Rather than have type inference fail, we will simply not make a return
                // type inference and have type inference continue on.  Either inference
                // will fail, or we will infer a nonapplicable method. Either way, there
                // is no change to the semantics of overload resolution.

                var originalDelegateParameters = target.DelegateParameters();
                if (originalDelegateParameters.IsDefault)
                {
                    return default;
                }

                if (originalDelegateParameters.Length != anonymousFunction.ParameterCount)
                {
                    return default;
                }
            }

            var fixedDelegate = (NamedTypeSymbol)GetFixedDelegateOrFunctionPointer(target);
            var fixedDelegateParameters = fixedDelegate.DelegateParameters();
            // Optimization:
            // Similarly, if we have an entirely fixed delegate and an explicitly typed
            // anonymous function, then the parameter types had better be identical.
            // If not, applicability will eventually fail, so there is no semantic
            // difference caused by failing to make a return type inference.
            if (anonymousFunction.HasExplicitlyTypedParameterList)
            {
                for (int p = 0; p < anonymousFunction.ParameterCount; ++p)
                {
                    if (!anonymousFunction.ParameterType(p).Equals(fixedDelegateParameters[p].Type, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        return default;
                    }
                }
            }

            // Future optimization: We could return default if the delegate has out or ref parameters
            // and the anonymous function is an implicitly typed lambda. It will not be applicable.

            // We have an entirely fixed delegate parameter list, which is of the same arity as
            // the anonymous function parameter list, and possibly exactly the same types if
            // the anonymous function is explicitly typed.  Make an inference from the
            // delegate parameters to the return type.

            return anonymousFunction.InferReturnType(_conversions, fixedDelegate, ref useSiteInfo);
        }

        /// <summary>
        /// Return the interface with an original definition matches
        /// the original definition of the target. If the are no matches,
        /// or multiple matches, the return value is null.
        /// </summary>
        private static NamedTypeSymbol GetInterfaceInferenceBound(ImmutableArray<NamedTypeSymbol> interfaces, NamedTypeSymbol target)
        {
            NamedTypeSymbol matchingInterface = null;
            foreach (var currentInterface in interfaces.Where(ci => TypeSymbol.Equals(ci.OriginalDefinition, target.OriginalDefinition, TypeCompareKind.ConsiderEverything)))
            {
                if (matchingInterface is null)
                {
                    matchingInterface = currentInterface;
                }
                else if (!TypeSymbol.Equals(matchingInterface, currentInterface, TypeCompareKind.ConsiderEverything))
                {
                    // Not unique. Bail out.
                    return null;
                }
            }
            return matchingInterface;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //
        // Helper methods
        //


        ////////////////////////////////////////////////////////////////////////////////
        //
        // In error recovery and reporting scenarios we sometimes end up in a situation
        // like this:
        //
        // x.Goo( y=>
        //
        // and the question is, "is Goo a valid extension method of x?"  If Goo is
        // generic, then Goo will be something like:
        //
        // static Blah Goo<T>(this Bar<T> bar, Func<T, T> f){ ... }
        //
        // What we would like to know is: given _only_ the expression x, can we infer
        // what T is in Bar<T> ?  If we can, then for error recovery and reporting
        // we can provisionally consider Goo to be an extension method of x. If we 
        // cannot deduce this just from x then we should consider Goo to not be an
        // extension method of x, at least until we have more information.
        //
        // Clearly it is pointless to run multiple phases
        public static ImmutableArray<TypeWithAnnotations> InferTypeArgumentsFromFirstArgument(
            ConversionsBase conversions,
            MethodSymbol method,
            ImmutableArray<BoundExpression> arguments,
            ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {

            // We need at least one formal parameter type and at least one argument.
            if ((method.ParameterCount < 1) || (arguments.Length < 1))
            {
                return default;
            }


            var constructedFromMethod = method.ConstructedFrom;

            var inferrer = new MethodTypeInferrer(
                conversions,
                constructedFromMethod.TypeParameters,
                constructedFromMethod.ContainingType,
                constructedFromMethod.GetParameterTypes(),
                constructedFromMethod.ParameterRefKinds,
                arguments,
                extensions: null);

            if (!inferrer.InferTypeArgumentsFromFirstArgument(ref useSiteInfo))
            {
                return default;
            }

            return inferrer.GetInferredTypeArguments();
        }

        ////////////////////////////////////////////////////////////////////////////////

        private bool InferTypeArgumentsFromFirstArgument(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            var dest = _formalParameterTypes[0];
            var argument = _arguments[0];
            TypeSymbol source = argument.Type;
            // Rule out lambdas, nulls, and so on.
            if (!IsReallyAType(source))
            {
                return false;
            }
            LowerBoundInference(_extensions.GetTypeWithAnnotations(argument), dest, ref useSiteInfo);
            // Now check to see that every type parameter used by the first
            // formal parameter type was successfully inferred.
            for (int iParam = 0; iParam < _methodTypeParameters.Length; ++iParam)
            {
                TypeParameterSymbol pParam = _methodTypeParameters[iParam];
                if (!dest.Type.ContainsTypeParameter(pParam))
                {
                    continue;
                }
                if (!HasBound(iParam) || !Fix(iParam, ref useSiteInfo))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Return the inferred type arguments using null
        /// for any type arguments that were not inferred.
        /// </summary>
        private ImmutableArray<TypeWithAnnotations> GetInferredTypeArguments()
        {
            return _fixedResults.AsImmutable();
        }

        private static bool IsReallyAType(TypeSymbol type)
        {
            return type is not null &&
                !type.IsErrorType() &&
                !type.IsVoidType();
        }

        private static void GetAllCandidates(Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates, ArrayBuilder<TypeWithAnnotations> builder)
        {
            builder.AddRange(candidates.Values);
        }

        private static void AddAllCandidates(
            Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates,
            HashSet<TypeWithAnnotations> bounds,
            VarianceKind variance,
            ConversionsBase conversions)
        {
            foreach (var candidate in bounds)
            {
                var type = candidate;
                if (!conversions.IncludeNullability)
                {
                    // https://github.com/dotnet/roslyn/issues/30534: Should preserve
                    // distinct "not computed" state from initial binding.
                    type = type.SetUnknownNullabilityForReferenceTypes();
                }

                AddOrMergeCandidate(candidates, type, variance/*, conversions*/);
            }
        }

        private static void AddOrMergeCandidate(
            Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates,
            TypeWithAnnotations newCandidate,
            VarianceKind variance/*,
            ConversionsBase conversions*/)
        {
            if (candidates.TryGetValue(newCandidate, out TypeWithAnnotations oldCandidate))
            {
                MergeAndReplaceIfStillCandidate(candidates, oldCandidate, newCandidate, variance/*, conversions*/);
            }
            else
            {
                candidates.Add(newCandidate, newCandidate);
            }
        }

        private static void MergeOrRemoveCandidates(
            Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates,
            HashSet<TypeWithAnnotations> bounds,
            ArrayBuilder<TypeWithAnnotations> initialCandidates,
            ConversionsBase conversions,
            VarianceKind variance,
            ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // SPEC:   For each lower (upper) bound U of Xi all types to which there is not an
            // SPEC:   implicit conversion from (to) U are removed from the candidate set.
            var comparison = conversions.IncludeNullability ? TypeCompareKind.ConsiderEverything : TypeCompareKind.IgnoreNullableModifiersForReferenceTypes;
            foreach (var bound in bounds)
            {
                foreach (var candidate in initialCandidates)
                {
                    if (bound.Equals(candidate, comparison))
                    {
                        continue;
                    }
                    TypeWithAnnotations source;
                    TypeWithAnnotations destination;
                    if (variance == VarianceKind.Out)
                    {
                        source = bound;
                        destination = candidate;
                    }
                    else
                    {
                        source = candidate;
                        destination = bound;
                    }
                    if (!ImplicitConversionExists(source, destination, ref useSiteInfo, conversions.WithNullability(false)))
                    {
                        candidates.Remove(candidate);
                        if (conversions.IncludeNullability && candidates.TryGetValue(bound, out var oldBound))
                        {
                            // merge the nullability from candidate into bound
                            var oldAnnotation = oldBound.NullableAnnotation;
                            var newAnnotation = oldAnnotation.MergeNullableAnnotation(candidate.NullableAnnotation, variance);
                            if (oldAnnotation != newAnnotation)
                            {
                                var newBound = TypeWithAnnotations.Create(oldBound.Type, newAnnotation);
                                candidates[bound] = newBound;
                            }
                        }
                    }
                    else if (bound.Equals(candidate, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        // SPEC: 4.7 The Dynamic Type
                        //       Type inference (7.5.2) will prefer dynamic over object if both are candidates.
                        //
                        // This rule doesn't have to be implemented explicitly due to special handling of 
                        // conversions from dynamic in ImplicitConversionExists helper.
                        // 
                        MergeAndReplaceIfStillCandidate(candidates, candidate, bound, variance/*, conversions*/);
                    }
                }
            }
        }

        private static void MergeAndReplaceIfStillCandidate(
            Dictionary<TypeWithAnnotations, TypeWithAnnotations> candidates,
            TypeWithAnnotations oldCandidate,
            TypeWithAnnotations newCandidate,
            VarianceKind variance/*,
            ConversionsBase conversions*/)
        {
            // We make an exception when new candidate is dynamic, for backwards compatibility 
            if (newCandidate.Type.IsDynamic())
            {
                return;
            }

            if (candidates.TryGetValue(oldCandidate, out TypeWithAnnotations latest))
            {
                // Note: we're ignoring the variance used merging previous candidates into `latest`.
                // If that variance is different than `variance`, we might infer the wrong nullability, but in that case,
                // we assume we'll report a warning when converting the arguments to the inferred parameter types.
                // (For instance, with F<T>(T x, T y, IIn<T> z) and interface IIn<in T> and interface IIOut<out T>, the
                // call F(IOut<object?>, IOut<object!>, IIn<IOut<object!>>) should find a nullability mismatch. Instead,
                // we'll merge the lower bounds IOut<object?> with IOut<object!> (using VarianceKind.Out) to produce
                // IOut<object?>, then merge that result with upper bound IOut<object!> (using VarianceKind.In)
                // to produce IOut<object?>. But then conversion of argument IIn<IOut<object!>> to parameter
                // IIn<IOut<object?>> will generate a warning at that point.)
                TypeWithAnnotations merged = latest.MergeEquivalentTypes(newCandidate, variance);
                candidates[oldCandidate] = merged;
            }
        }

        /// <summary>
        /// This is a comparer that ignores differences in dynamic-ness and tuple names.
        /// But it has a special case for top-level object vs. dynamic for purpose of method type inference.
        /// </summary>
        private sealed class EqualsIgnoringDynamicTupleNamesAndNullabilityComparer : EqualityComparer<TypeWithAnnotations>
        {
            internal static readonly EqualsIgnoringDynamicTupleNamesAndNullabilityComparer Instance = new();

            public override int GetHashCode(TypeWithAnnotations obj)
            {
                return obj.Type.GetHashCode();
            }

            public override bool Equals(TypeWithAnnotations x, TypeWithAnnotations y)
            {
                // We do a equality test ignoring dynamic and tuple names differences,
                // but dynamic and object are not considered equal for backwards compatibility.
                if (x.Type.IsDynamic() ^ y.Type.IsDynamic()) { return false; }

                return x.Equals(y, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);
            }
        }
    }
}

