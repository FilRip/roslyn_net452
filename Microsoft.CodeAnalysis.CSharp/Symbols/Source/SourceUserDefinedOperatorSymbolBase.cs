// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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

        protected SourceUserDefinedOperatorSymbolBase(
            MethodKind methodKind,
            string name,
            SourceMemberContainerTypeSymbol containingType,
            Location location,
            CSharpSyntaxNode syntax,
            DeclarationModifiers declarationModifiers,
            bool hasBody,
            bool isExpressionBodied,
            bool isIterator,
            bool isNullableAnalysisEnabled,
            BindingDiagnosticBag diagnostics) :
            base(containingType, syntax.GetReference(), location, isIterator: isIterator)
        {
            _name = name;
            _isExpressionBodied = isExpressionBodied;

            this.CheckUnsafeModifier(declarationModifiers, diagnostics);

            // We will bind the formal parameters and the return type lazily. For now,
            // assume that the return type is non-void; when we do the lazy initialization
            // of the parameters and return type we will update the flag if necessary.

            this.MakeFlags(methodKind, declarationModifiers, returnsVoid: false, isExtensionMethod: false, isNullableAnalysisEnabled: isNullableAnalysisEnabled);

            if (this.ContainingType.IsInterface &&
                (methodKind == MethodKind.Conversion || name == WellKnownMemberNames.EqualityOperatorName || name == WellKnownMemberNames.InequalityOperatorName))
            {
                // If we have a conversion or equality/inequality operator in an interface, we already have reported that fact as 
                // an error. No need to cascade the error further.
                return;
            }

            if (this.ContainingType.IsStatic)
            {
                // Similarly if we're in a static class, though we have not reported it yet.

                // CS0715: '{0}': static classes cannot contain user-defined operators
                diagnostics.Add(ErrorCode.ERR_OperatorInStaticClass, location, this);
                return;
            }

            // SPEC: An operator declaration must include both a public and a
            // SPEC: static modifier
            if (this.DeclaredAccessibility != Accessibility.Public || !this.IsStatic)
            {
                // CS0558: User-defined operator '...' must be declared static and public
                diagnostics.Add(ErrorCode.ERR_OperatorsMustBeStatic, this.Locations[0], this);
            }

            // SPEC: Because an external operator provides no actual implementation, 
            // SPEC: its operator body consists of a semicolon. For expression-bodied
            // SPEC: operators, the body is an expression. For all other operators,
            // SPEC: the operator body consists of a block...
            if (hasBody && IsExtern)
            {
                diagnostics.Add(ErrorCode.ERR_ExternHasBody, location, this);
            }
            else if (!hasBody && !IsExtern && !IsAbstract && !IsPartial)
            {
                // Do not report that the body is missing if the operator is marked as
                // partial or abstract; we will already have given an error for that so
                // there's no need to "cascade" the error.
                diagnostics.Add(ErrorCode.ERR_ConcreteMissingBody, location, this);
            }

            // SPEC: It is an error for the same modifier to appear multiple times in an
            // SPEC: operator declaration.
            var info = ModifierUtils.CheckAccessibility(this.DeclarationModifiers, this, isExplicitInterfaceImplementation: false);
            if (info != null)
            {
                diagnostics.Add(info, location);
            }
        }

        protected static DeclarationModifiers MakeDeclarationModifiers(BaseMethodDeclarationSyntax syntax, Location location, BindingDiagnosticBag diagnostics)
        {
            var defaultAccess = DeclarationModifiers.Private;
            var allowedModifiers =
                DeclarationModifiers.AccessibilityMask |
                DeclarationModifiers.Static |
                DeclarationModifiers.Extern |
                DeclarationModifiers.Unsafe;

            return ModifierUtils.MakeAndCheckNontypeMemberModifiers(
                syntax.Modifiers, defaultAccess, allowedModifiers, location, diagnostics, modifierErrors: out _);
        }

        protected abstract Location ReturnTypeLocation { get; }

        protected (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindReturnType(BaseMethodDeclarationSyntax declarationSyntax, TypeSyntax returnTypeSyntax, BindingDiagnosticBag diagnostics)
        {
            TypeWithAnnotations returnType;
            ImmutableArray<ParameterSymbol> parameters;

            var binder = this.DeclaringCompilation.
                GetBinderFactory(declarationSyntax.SyntaxTree).GetBinder(returnTypeSyntax, declarationSyntax, this);


            var signatureBinder = binder.WithAdditionalFlags(EBinder.SuppressConstraintChecks);

            parameters = ParameterHelpers.MakeParameters(
                signatureBinder,
                this,
                declarationSyntax.ParameterList,
                out SyntaxToken arglistToken,
                allowRefOrOut: true,
                allowThis: false,
                addRefReadOnlyModifier: false,
                diagnostics: diagnostics);

            if (arglistToken.Kind() == SyntaxKind.ArgListKeyword)
            {
                // This is a parse-time error in the native compiler; it is a semantic analysis error in Roslyn.

                // error CS1669: __arglist is not valid in this context
                diagnostics.Add(ErrorCode.ERR_IllegalVarArgs, new SourceLocation(arglistToken));

                // Regardless of whether __arglist appears in the source code, we do not mark
                // the operator method as being a varargs method.
            }

            returnType = signatureBinder.BindType(returnTypeSyntax, diagnostics);

            // restricted types cannot be returned. 
            // NOTE: Span-like types can be returned (if expression is returnable).
            if (returnType.IsRestrictedType(ignoreSpanLikeTypes: true))
            {
                // The return type of a method, delegate, or function pointer cannot be '{0}'
                diagnostics.Add(ErrorCode.ERR_MethodReturnCantBeRefAny, returnTypeSyntax.Location, returnType.Type);
            }

            if (returnType.Type.IsStatic)
            {
                // Operators in interfaces was introduced in C# 8, so there's no need to be specially concerned about
                // maintaining backcompat with the native compiler bug around interfaces.
                // '{0}': static types cannot be used as return types
                diagnostics.Add(ErrorFacts.GetStaticClassReturnCode(useWarning: false), returnTypeSyntax.Location, returnType.Type);
            }

            return (returnType, parameters);
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            (_lazyReturnType, _lazyParameters) = MakeParametersAndBindReturnType(diagnostics);

            this.SetReturnsVoid(_lazyReturnType.IsVoidType());

            // If we have a conversion/equality/inequality operator in an interface or static class then we already 
            // have reported that fact as an error. No need to cascade the error further.
            if ((this.ContainingType.IsInterfaceType() &&
                (MethodKind == MethodKind.Conversion || Name == WellKnownMemberNames.EqualityOperatorName || Name == WellKnownMemberNames.InequalityOperatorName)) ||
                this.ContainingType.IsStatic)
            {
                return;
            }

            // SPEC: All types referenced in an operator declaration must be at least as accessible 
            // SPEC: as the operator itself.

            CheckEffectiveAccessibility(_lazyReturnType, _lazyParameters, diagnostics);
            CheckValueParameters(diagnostics);
            CheckOperatorSignatures(diagnostics);
        }

        protected abstract (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics);

        private void CheckValueParameters(BindingDiagnosticBag diagnostics)
        {
            // SPEC: The parameters of an operator must be value parameters.
            foreach (var p in this.Parameters)
            {
                if (p.RefKind != RefKind.None && p.RefKind != RefKind.In)
                {
                    diagnostics.Add(ErrorCode.ERR_IllegalRefParam, this.Locations[0]);
                    break;
                }
            }
        }

        private void CheckOperatorSignatures(BindingDiagnosticBag diagnostics)
        {
            // Have we even got the right formal parameter arity? If not then 
            // we are in an error recovery scenario and we should just bail 
            // out immediately.
            if (!DoesOperatorHaveCorrectArity(this.Name, this.ParameterCount))
            {
                return;
            }

            switch (this.Name)
            {
                case WellKnownMemberNames.ImplicitConversionName:
                case WellKnownMemberNames.ExplicitConversionName:
                    CheckUserDefinedConversionSignature(diagnostics);
                    break;

                case WellKnownMemberNames.UnaryNegationOperatorName:
                case WellKnownMemberNames.UnaryPlusOperatorName:
                case WellKnownMemberNames.LogicalNotOperatorName:
                case WellKnownMemberNames.OnesComplementOperatorName:
                    CheckUnarySignature(diagnostics);
                    break;

                case WellKnownMemberNames.TrueOperatorName:
                case WellKnownMemberNames.FalseOperatorName:
                    CheckTrueFalseSignature(diagnostics);
                    break;

                case WellKnownMemberNames.IncrementOperatorName:
                case WellKnownMemberNames.DecrementOperatorName:
                    CheckIncrementDecrementSignature(diagnostics);
                    break;

                case WellKnownMemberNames.LeftShiftOperatorName:
                case WellKnownMemberNames.RightShiftOperatorName:
                    CheckShiftSignature(diagnostics);
                    break;

                default:
                    CheckBinarySignature(diagnostics);
                    break;
            }
        }

        private static bool DoesOperatorHaveCorrectArity(string name, int parameterCount)
        {
            return name switch
            {
                WellKnownMemberNames.IncrementOperatorName or WellKnownMemberNames.DecrementOperatorName or WellKnownMemberNames.UnaryNegationOperatorName or WellKnownMemberNames.UnaryPlusOperatorName or WellKnownMemberNames.LogicalNotOperatorName or WellKnownMemberNames.OnesComplementOperatorName or WellKnownMemberNames.TrueOperatorName or WellKnownMemberNames.FalseOperatorName or WellKnownMemberNames.ImplicitConversionName or WellKnownMemberNames.ExplicitConversionName => parameterCount == 1,
                _ => parameterCount == 2,
            };
        }

        private void CheckUserDefinedConversionSignature(BindingDiagnosticBag diagnostics)
        {
            if (this.ReturnsVoid)
            {
                // CS0590: User-defined operators cannot return void
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, this.Locations[0]);
            }

            // SPEC: For a given source type S and target type T, if S or T are
            // SPEC: nullable types let S0 and T0 refer to their underlying types,
            // SPEC: otherwise, S0 and T0 are equal to S and T, respectively.

            var source = this.GetParameterType(0);
            var target = this.ReturnType;
            var source0 = source.StrippedType();
            var target0 = target.StrippedType();

            // SPEC: A class or struct is permitted to declare a conversion from S to T
            // SPEC: only if all the following are true:

            // SPEC: Neither S0 nor T0 is an interface type.

            if (source0.IsInterfaceType() || target0.IsInterfaceType())
            {
                // CS0552: '{0}': user-defined conversions to or from an interface are not allowed
                diagnostics.Add(ErrorCode.ERR_ConversionWithInterface, this.Locations[0], this);
                return;
            }

            // SPEC: Either S0 or T0 is the class or struct type in which the operator
            // SPEC: declaration takes place.

            if (!MatchesContainingType(source0) &&
                !MatchesContainingType(target0) &&
                // allow conversion between T and Nullable<T> in declaration of Nullable<T>
                !MatchesContainingType(source) &&
                !MatchesContainingType(target))
            {
                // CS0556: User-defined conversion must convert to or from the enclosing type
                diagnostics.Add(ErrorCode.ERR_ConversionNotInvolvingContainedType, this.Locations[0]);
                return;
            }

            // SPEC: * S0 and T0 are different types:

            if ((ContainingType.SpecialType == SpecialType.System_Nullable_T)
                    ? source.Equals(target, ComparisonForUserDefinedOperators)
                    : source0.Equals(target0, ComparisonForUserDefinedOperators))
            {
                // CS0555: User-defined operator cannot take an object of the enclosing type 
                // and convert to an object of the enclosing type
                diagnostics.Add(ErrorCode.ERR_IdentityConversion, this.Locations[0]);
                return;
            }

            // Those are the easy ones. Now we come to:

            // SPEC: 
            // Excluding user-defined conversions, a conversion does not exist from 
            // S to T or T to S. For the purposes of these rules, any type parameters
            // associated with S or T are considered to be unique types that have
            // no inheritance relationship with other types, and any constraints on
            // those type parameters are ignored.

            // A counter-intuitive consequence of this rule is that:
            //
            // class X<U> where U : X<U>
            // {
            //     public implicit operator X<U>(U u) { return u; }
            // }
            //
            // is *legal*, even though there is *already* an implicit conversion
            // from U to X<U> because U is constrained to have such a conversion.
            //
            // In discussing the implications of this rule, let's call the 
            // containing type (which may be a class or struct) "C". S and T
            // are the source and target types.  
            //
            // If we have made it this far in the error analysis we already know that
            // exactly one of S and T is C or C? -- if two or zero were, then we'd
            // have already reported ERR_ConversionNotInvolvingContainedType or 
            // ERR_IdentityConversion and returned.
            //
            // WOLOG for the purposes of this discussion let's assume that S is 
            // the one that is C or C?, and that T is the one that is neither C nor C?.
            //
            // So the question is: under what circumstances could T-to-S or S-to-T,
            // be a valid conversion, by the definition of valid above?
            //
            // Let's consider what kinds of types T could be. T cannot be an interface
            // because we've already reported an error and returned if it is. If T is
            // a delegate, array, enum, pointer, struct or nullable type then there 
            // is no built-in conversion from T to the user-declared class/struct 
            // C, or to C?. If T is a type parameter, then by assumption the type
            // parameter has no constraints, and therefore is not convertible to
            // C or C?. 
            //
            // That leaves T to be a class. We already know that T is not C, (or C?, 
            // since T is a class) and therefore there is no identity conversion from T to S.
            //
            // Suppose S is C and C is a class. Then the only way that there can be a 
            // conversion between T and S is if T is a base class of S or S is a base class of T.
            //
            // Suppose S is C and C is a struct. Then the only way that there can be a
            // conversion between T and S is if T is a base class of S. (And T would
            // have to be System.Object or System.ValueType.)
            //
            // Suppose S is C? and C is a struct. Then the only way that there can be a 
            // conversion between T and S is again, if T is a base class of S.
            //
            // Summing up:
            //
            // WOLOG, we assume that T is not C or C?, and S is C or C?. The conversion is
            // illegal only if T is a class, and either T is a base class of S, or S is a 
            // base class of T.

            if (source.IsDynamic() || target.IsDynamic())
            {
                // '{0}': user-defined conversions to or from the dynamic type are not allowed
                diagnostics.Add(ErrorCode.ERR_BadDynamicConversion, this.Locations[0], this);
                return;
            }

            TypeSymbol same;
            TypeSymbol different;

            if (MatchesContainingType(source0))
            {
                same = source;
                different = target;
            }
            else
            {
                same = target;
                different = source;
            }

            if (different.IsClassType())
            {
                // different is a class type:

                // "same" is the containing class, so it can't be a type parameter

                var useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);

                if (same.IsDerivedFrom(different, ComparisonForUserDefinedOperators, useSiteInfo: ref useSiteInfo)) // tomat: ignoreDynamic should be true, but we don't want to introduce breaking change. See bug 605326.
                {
                    // '{0}': user-defined conversions to or from a base type are not allowed
                    diagnostics.Add(ErrorCode.ERR_ConversionWithBase, this.Locations[0], this);
                }
                else if (different.IsDerivedFrom(same, ComparisonForUserDefinedOperators, useSiteInfo: ref useSiteInfo)) // tomat: ignoreDynamic should be true, but we don't want to introduce breaking change. See bug 605326.
                {
                    // '{0}': user-defined conversions to or from a derived type are not allowed
                    diagnostics.Add(ErrorCode.ERR_ConversionWithDerived, this.Locations[0], this);
                }

                diagnostics.Add(this.Locations[0], useSiteInfo);
            }
        }

        private void CheckUnarySignature(BindingDiagnosticBag diagnostics)
        {
            // SPEC: A unary + - ! ~ operator must take a single parameter of type
            // SPEC: T or T? and can return any type.

            if (!MatchesContainingType(this.GetParameterType(0).StrippedType()))
            {
                // The parameter of a unary operator must be the containing type
                diagnostics.Add(ErrorCode.ERR_BadUnaryOperatorSignature, this.Locations[0]);
            }

            if (this.ReturnsVoid)
            {
                // The Roslyn parser does not detect this error.
                // CS0590: User-defined operators cannot return void
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, this.Locations[0]);
            }
        }

        private void CheckTrueFalseSignature(BindingDiagnosticBag diagnostics)
        {
            // SPEC: A unary true or false operator must take a single parameter of type
            // SPEC: T or T? and must return type bool.

            if (this.ReturnType.SpecialType != SpecialType.System_Boolean)
            {
                // The return type of operator True or False must be bool
                diagnostics.Add(ErrorCode.ERR_OpTFRetType, this.Locations[0]);
            }

            if (!MatchesContainingType(this.GetParameterType(0).StrippedType()))
            {
                // The parameter of a unary operator must be the containing type
                diagnostics.Add(ErrorCode.ERR_BadUnaryOperatorSignature, this.Locations[0]);
            }
        }

        private void CheckIncrementDecrementSignature(BindingDiagnosticBag diagnostics)
        {
            // SPEC: A unary ++ or -- operator must take a single parameter of type T or T?
            // SPEC: and it must return that same type or a type derived from it.

            // The native compiler error reporting behavior is not very good in some cases
            // here, both because it reports the wrong errors, and because the wording
            // of the error messages is misleading. The native compiler reports two errors:

            // CS0448: The return type for ++ or -- operator must be the 
            //         containing type or derived from the containing type
            //
            // CS0559: The parameter type for ++ or -- operator must be the containing type
            //
            // Neither error message mentions nullable types. But worse, there is a 
            // situation in which the native compiler reports a misleading error:
            //
            // struct S { public static S operator ++(S? s) { ... } }
            //
            // This reports CS0559, but that is not the error; the *parameter* is perfectly
            // legal. The error is that the return type does not match the parameter type.
            // 
            // I have changed the error message to reflect the true error, and we now 
            // report 0448, not 0559, in the given scenario. The error is now:
            //
            // CS0448: The return type for ++ or -- operator must match the parameter type
            //         or be derived from the parameter type
            //
            // However, this now means that we must make *another* change from native compiler
            // behavior. The native compiler would report both 0448 and 0559 when given:
            //
            // struct S { public static int operator ++(int s) { ... } }
            //
            // The previous wording of error 0448 was *correct* in this scenario, but not
            // it is wrong because it *does* match the formal parameter type.
            //
            // The solution is: First see if 0559 must be reported. Only if the formal
            // parameter type is *good* do we then go on to try to report an error against
            // the return type.

            var parameterType = this.GetParameterType(0);
            var useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);

            if (!MatchesContainingType(parameterType.StrippedType()))
            {
                // CS0559: The parameter type for ++ or -- operator must be the containing type
                diagnostics.Add(ErrorCode.ERR_BadIncDecSignature, this.Locations[0]);
            }
            else if (!this.ReturnType.EffectiveTypeNoUseSiteDiagnostics.IsEqualToOrDerivedFrom(parameterType, ComparisonForUserDefinedOperators, useSiteInfo: ref useSiteInfo))
            {
                // CS0448: The return type for ++ or -- operator must match the parameter type
                //         or be derived from the parameter type
                diagnostics.Add(ErrorCode.ERR_BadIncDecRetType, this.Locations[0]);
            }

            diagnostics.Add(this.Locations[0], useSiteInfo);
        }

        private bool MatchesContainingType(TypeSymbol type)
        {
            return type.Equals(this.ContainingType, ComparisonForUserDefinedOperators);
        }

        private void CheckShiftSignature(BindingDiagnosticBag diagnostics)
        {
            // SPEC: A binary << or >> operator must take two parameters, the first
            // SPEC: of which must have type T or T? and the second of which must
            // SPEC: have type int or int?, and can return any type.

            if (!MatchesContainingType(this.GetParameterType(0).StrippedType()) ||
                this.GetParameterType(1).StrippedType().SpecialType != SpecialType.System_Int32)
            {
                // CS0546: The first operand of an overloaded shift operator must have the 
                //         same type as the containing type, and the type of the second 
                //         operand must be int
                diagnostics.Add(ErrorCode.ERR_BadShiftOperatorSignature, this.Locations[0]);
            }

            if (this.ReturnsVoid)
            {
                // The Roslyn parser does not detect this error.
                // CS0590: User-defined operators cannot return void
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, this.Locations[0]);
            }
        }

        private void CheckBinarySignature(BindingDiagnosticBag diagnostics)
        {
            // SPEC: A binary nonshift operator must take two parameters, at least
            // SPEC: one of which must have the type T or T?, and can return any type.
            if (!MatchesContainingType(this.GetParameterType(0).StrippedType()) &&
                !MatchesContainingType(this.GetParameterType(1).StrippedType()))
            {
                // CS0563: One of the parameters of a binary operator must be the containing type
                diagnostics.Add(ErrorCode.ERR_BadBinaryOperatorSignature, this.Locations[0]);
            }

            if (this.ReturnsVoid)
            {
                // The parser does not detect this error.
                // CS0590: User-defined operators cannot return void
                diagnostics.Add(ErrorCode.ERR_OperatorCantReturnVoid, this.Locations[0]);
            }
        }

        public sealed override string Name
        {
            get
            {
                return _name;
            }
        }

        public sealed override bool ReturnsVoid
        {
            get
            {
                LazyMethodChecks();
                return base.ReturnsVoid;
            }
        }

        public sealed override bool IsVararg
        {
            get
            {
                return false;
            }
        }

        public sealed override bool IsExtensionMethod
        {
            get
            {
                return false;
            }
        }

        public sealed override ImmutableArray<Location> Locations
        {
            get
            {
                return this.locations;
            }
        }

        internal sealed override int ParameterCount
        {
            get
            {
                if (!_lazyParameters.IsDefault)
                {
                    int result = _lazyParameters.Length;
                    return result;
                }

                return GetParameterCountFromSyntax();
            }
        }

        protected abstract int GetParameterCountFromSyntax();

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                LazyMethodChecks();
                return _lazyParameters;
            }
        }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get { return ImmutableArray<TypeParameterSymbol>.Empty; }
        }

        public sealed override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
            => ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;

        public sealed override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
            => ImmutableArray<TypeParameterConstraintKind>.Empty;

        public sealed override RefKind RefKind
        {
            get { return RefKind.None; }
        }

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        internal sealed override bool IsExpressionBodied
        {
            get { return _isExpressionBodied; }
        }

        internal sealed override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            // Check constraints on return type and parameters. Note: Dev10 uses the
            // method name location for any such errors. We'll do the same for return
            // type errors but for parameter errors, we'll use the parameter location.

            var compilation = DeclaringCompilation;

            this.ReturnType.CheckAllConstraints(compilation, conversions, this.Locations[0], diagnostics);

            foreach (var parameter in this.Parameters)
            {
                parameter.Type.CheckAllConstraints(compilation, conversions, parameter.Locations[0], diagnostics);
            }

            ParameterHelpers.EnsureIsReadOnlyAttributeExists(compilation, Parameters, diagnostics, modifyCompilation: true);

            if (ReturnType.ContainsNativeInteger())
            {
                compilation.EnsureNativeIntegerAttributeExists(diagnostics, ReturnTypeLocation, modifyCompilation: true);
            }

            ParameterHelpers.EnsureNativeIntegerAttributeExists(compilation, Parameters, diagnostics, modifyCompilation: true);

            if (compilation.ShouldEmitNullableAttributes(this) &&
                ReturnTypeWithAnnotations.NeedsNullableAttribute())
            {
                compilation.EnsureNullableAttributeExists(diagnostics, ReturnTypeLocation, modifyCompilation: true);
            }

            ParameterHelpers.EnsureNullableAttributeExists(compilation, this, Parameters, diagnostics, modifyCompilation: true);
        }
    }
}
