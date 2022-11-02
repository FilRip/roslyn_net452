// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class Binder
    {
        internal BoundExpression CreateConversion(
            BoundExpression source,
            TypeSymbol destination,
            BindingDiagnosticBag diagnostics)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            var conversion = Conversions.ClassifyConversionFromExpression(source, destination, ref useSiteInfo);

            diagnostics.Add(source.Syntax, useSiteInfo);
            return CreateConversion(source.Syntax, source, conversion, isCast: false, conversionGroupOpt: null, destination: destination, diagnostics: diagnostics);
        }

        internal BoundExpression CreateConversion(
            BoundExpression source,
            Conversion conversion,
            TypeSymbol destination,
            BindingDiagnosticBag diagnostics)
        {
            return CreateConversion(source.Syntax, source, conversion, isCast: false, conversionGroupOpt: null, destination: destination, diagnostics: diagnostics);
        }

        internal BoundExpression CreateConversion(
            SyntaxNode syntax,
            BoundExpression source,
            Conversion conversion,
            bool isCast,
            ConversionGroup? conversionGroupOpt,
            TypeSymbol destination,
            BindingDiagnosticBag diagnostics)
        {
            return CreateConversion(syntax, source, conversion, isCast: isCast, conversionGroupOpt, source.WasCompilerGenerated, destination, diagnostics);
        }

        protected BoundExpression CreateConversion(
            SyntaxNode syntax,
            BoundExpression source,
            Conversion conversion,
            bool isCast,
            ConversionGroup? conversionGroupOpt,
            bool wasCompilerGenerated,
            TypeSymbol destination,
            BindingDiagnosticBag diagnostics,
            bool hasErrors = false)
        {
            if (conversion.IsIdentity)
            {
                if (source is BoundTupleLiteral sourceTuple)
                {
                    NamedTypeSymbol.ReportTupleNamesMismatchesIfAny(destination, sourceTuple, diagnostics);
                }

                // identity tuple and switch conversions result in a converted expression
                // to indicate that such conversions are no longer applicable.
                source = BindToNaturalType(source, diagnostics);

                // We need to preserve any conversion that changes the type (even identity conversions, like object->dynamic),
                // or that was explicitly written in code (so that GetSemanticInfo can find the syntax in the bound tree).
#nullable restore
                if (!isCast && source.Type.Equals(destination, TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
#nullable enable
                {
                    return source;
                }
            }

            if (conversion.IsMethodGroup)
            {
                return CreateMethodGroupConversion(syntax, source, conversion, isCast: isCast, conversionGroupOpt, destination, diagnostics);
            }

            // Obsolete diagnostics for method group are reported as part of creating the method group conversion.
            ReportDiagnosticsIfObsolete(diagnostics, conversion, syntax, hasBaseReceiver: false);

            if (conversion.IsAnonymousFunction && source.Kind == BoundKind.UnboundLambda)
            {
                return CreateAnonymousFunctionConversion(syntax, source, conversion, isCast: isCast, conversionGroupOpt, destination, diagnostics);
            }

            if (conversion.IsStackAlloc)
            {
                return CreateStackAllocConversion(syntax, source, conversion, isCast, conversionGroupOpt, destination, diagnostics);
            }

            if (conversion.IsTupleLiteralConversion ||
                (conversion.IsNullable && conversion.UnderlyingConversions[0].IsTupleLiteralConversion))
            {
                return CreateTupleLiteralConversion(syntax, (BoundTupleLiteral)source, conversion, isCast: isCast, conversionGroupOpt, destination, diagnostics);
            }

            if (conversion.Kind == ConversionKind.SwitchExpression)
            {
                var convertedSwitch = ConvertSwitchExpression((BoundUnconvertedSwitchExpression)source, destination, conversionIfTargetTyped: conversion, diagnostics);
                return new BoundConversion(
                    syntax,
                    convertedSwitch,
                    conversion,
                    CheckOverflowAtRuntime,
                    explicitCastInCode: isCast && !wasCompilerGenerated,
                    conversionGroupOpt,
                    convertedSwitch.ConstantValue,
                    destination,
                    hasErrors);
            }

            if (conversion.Kind == ConversionKind.ConditionalExpression)
            {
                var convertedConditional = ConvertConditionalExpression((BoundUnconvertedConditionalOperator)source, destination, conversionIfTargetTyped: conversion, diagnostics);
                return new BoundConversion(
                    syntax,
                    convertedConditional,
                    conversion,
                    CheckOverflowAtRuntime,
                    explicitCastInCode: isCast && !wasCompilerGenerated,
                    conversionGroupOpt,
                    convertedConditional.ConstantValue,
                    destination,
                    hasErrors);
            }

            if (conversion.Kind == ConversionKind.InterpolatedString)
            {
                var unconvertedSource = (BoundUnconvertedInterpolatedString)source;
                source = new BoundInterpolatedString(
                    unconvertedSource.Syntax,
                    unconvertedSource.Parts,
                    unconvertedSource.ConstantValue,
                    unconvertedSource.Type,
                    unconvertedSource.HasErrors);
            }

            if (source.Kind == BoundKind.UnconvertedSwitchExpression)
            {
                TypeSymbol? type = source.Type;
                if (type is null)
                {
                    type = CreateErrorType();
                    hasErrors = true;
                }

                source = ConvertSwitchExpression((BoundUnconvertedSwitchExpression)source, type, conversionIfTargetTyped: null, diagnostics, hasErrors);
                if (destination.Equals(type, TypeCompareKind.ConsiderEverything) && wasCompilerGenerated)
                {
                    return source;
                }
            }

            if (conversion.IsObjectCreation)
            {
                return ConvertObjectCreationExpression(syntax, (BoundUnconvertedObjectCreationExpression)source, isCast, destination, diagnostics);
            }

            if (source.Kind == BoundKind.UnconvertedConditionalOperator)
            {
                TypeSymbol? type = source.Type;
                if (type is null)
                {
                    type = CreateErrorType();
                    hasErrors = true;
                }

                source = ConvertConditionalExpression((BoundUnconvertedConditionalOperator)source, type, conversionIfTargetTyped: null, diagnostics, hasErrors);
                if (destination.Equals(type, TypeCompareKind.ConsiderEverything) && wasCompilerGenerated)
                {
                    return source;
                }
            }

            if (conversion.IsUserDefined)
            {
                // User-defined conversions are likely to be represented as multiple
                // BoundConversion instances so a ConversionGroup is necessary.
                return CreateUserDefinedConversion(syntax, source, conversion, isCast: isCast, conversionGroupOpt ?? new ConversionGroup(conversion), destination, diagnostics, hasErrors);
            }

            ConstantValue? constantValue = this.FoldConstantConversion(syntax, source, conversion, destination, diagnostics);
            if (conversion.Kind == ConversionKind.DefaultLiteral)
            {
                source = new BoundDefaultExpression(source.Syntax, targetType: null, constantValue, type: destination)
                    .WithSuppression(source.IsSuppressed);
            }

            return new BoundConversion(
                syntax,
                BindToNaturalType(source, diagnostics),
                conversion,
                @checked: CheckOverflowAtRuntime,
                explicitCastInCode: isCast && !wasCompilerGenerated,
                conversionGroupOpt,
                constantValueOpt: constantValue,
                type: destination,
                hasErrors: hasErrors)
            { WasCompilerGenerated = wasCompilerGenerated };
        }

        private BoundExpression ConvertObjectCreationExpression(SyntaxNode syntax, BoundUnconvertedObjectCreationExpression node, bool isCast, TypeSymbol destination, BindingDiagnosticBag diagnostics)
        {
            var arguments = AnalyzedArguments.GetInstance(node.Arguments, node.ArgumentRefKindsOpt, node.ArgumentNamesOpt);
            BoundExpression expr = BindObjectCreationExpression(node, destination.StrippedType(), arguments, diagnostics);
            if (destination.IsNullableType())
            {
                // We manually create an ImplicitNullable conversion
                // if the destination is nullable, in which case we
                // target the underlying type e.g. `S? x = new();`
                // is actually identical to `S? x = new S();`.
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                var conversion = Conversions.ClassifyStandardConversion(null, expr.Type, destination, ref useSiteInfo);
                expr = new BoundConversion(
                    node.Syntax,
                    operand: expr,
                    conversion: conversion,
                    @checked: false,
                    explicitCastInCode: isCast,
                    conversionGroupOpt: new ConversionGroup(conversion),
                    constantValueOpt: expr.ConstantValue,
                    type: destination);

                diagnostics.Add(syntax, useSiteInfo);
            }
            arguments.Free();
            return expr;
        }

        private BoundExpression BindObjectCreationExpression(BoundUnconvertedObjectCreationExpression node, TypeSymbol type, AnalyzedArguments arguments, BindingDiagnosticBag diagnostics)
        {
            var syntax = node.Syntax;
            switch (type.TypeKind)
            {
                case TypeKind.Enum:
                case TypeKind.Struct:
                case TypeKind.Class when !type.IsAnonymousType: // We don't want to enable object creation with unspeakable types
                    return BindClassCreationExpression(syntax, type.Name, typeNode: syntax, (NamedTypeSymbol)type, arguments, diagnostics, node.InitializerOpt, wasTargetTyped: true);
                case TypeKind.TypeParameter:
                    return BindTypeParameterCreationExpression(syntax, (TypeParameterSymbol)type, arguments, node.InitializerOpt, typeSyntax: syntax, diagnostics);
                case TypeKind.Delegate:
                    return BindDelegateCreationExpression(syntax, (NamedTypeSymbol)type, arguments, node.InitializerOpt, diagnostics);
                case TypeKind.Interface:
                    return BindInterfaceCreationExpression(syntax, (NamedTypeSymbol)type, diagnostics, typeNode: syntax, arguments, node.InitializerOpt, wasTargetTyped: true);
                case TypeKind.Array:
                case TypeKind.Class:
                case TypeKind.Dynamic:
                    Error(diagnostics, ErrorCode.ERR_ImplicitObjectCreationIllegalTargetType, syntax, type);
                    goto case TypeKind.Error;
                case TypeKind.Pointer:
                case TypeKind.FunctionPointer:
                    Error(diagnostics, ErrorCode.ERR_UnsafeTypeInObjectCreation, syntax, type);
                    goto case TypeKind.Error;
                case TypeKind.Error:
                    return MakeBadExpressionForObjectCreation(syntax, type, arguments, node.InitializerOpt, typeSyntax: syntax, diagnostics);
                case var v:
                    throw ExceptionUtilities.UnexpectedValue(v);
            }
        }

        /// <summary>
        /// Rewrite the subexpressions in a conditional expression to convert the whole thing to the destination type.
        /// </summary>
        private BoundExpression ConvertConditionalExpression(
            BoundUnconvertedConditionalOperator source,
            TypeSymbol destination,
            Conversion? conversionIfTargetTyped,
            BindingDiagnosticBag diagnostics,
            bool hasErrors = false)
        {
            bool targetTyped = conversionIfTargetTyped is { };
            ImmutableArray<Conversion> underlyingConversions = conversionIfTargetTyped.GetValueOrDefault().UnderlyingConversions;
            var condition = source.Condition;
            hasErrors |= source.HasErrors || destination.IsErrorType();

            var trueExpr =
                targetTyped
                ? CreateConversion(source.Consequence.Syntax, source.Consequence, underlyingConversions[0], isCast: false, conversionGroupOpt: null, destination, diagnostics)
                : GenerateConversionForAssignment(destination, source.Consequence, diagnostics);
            var falseExpr =
                targetTyped
                ? CreateConversion(source.Alternative.Syntax, source.Alternative, underlyingConversions[1], isCast: false, conversionGroupOpt: null, destination, diagnostics)
                : GenerateConversionForAssignment(destination, source.Alternative, diagnostics);
            var constantValue = FoldConditionalOperator(condition, trueExpr, falseExpr);
            hasErrors |= constantValue?.IsBad == true;
            if (targetTyped && !destination.IsErrorType())
                MessageID.IDS_FeatureTargetTypedConditional.CheckFeatureAvailability(diagnostics, source.Syntax);

            return new BoundConditionalOperator(source.Syntax, isRef: false, condition, trueExpr, falseExpr, constantValue, source.Type, wasTargetTyped: targetTyped, destination, hasErrors)
                .WithSuppression(source.IsSuppressed);
        }

        /// <summary>
        /// Rewrite the expressions in the switch expression arms to add a conversion to the destination type.
        /// </summary>
        private BoundExpression ConvertSwitchExpression(BoundUnconvertedSwitchExpression source, TypeSymbol destination, Conversion? conversionIfTargetTyped, BindingDiagnosticBag diagnostics, bool hasErrors = false)
        {
            bool targetTyped = conversionIfTargetTyped is { };
            Conversion conversion = conversionIfTargetTyped ?? Conversion.Identity;
            ImmutableArray<Conversion> underlyingConversions = conversion.UnderlyingConversions;
            var builder = ArrayBuilder<BoundSwitchExpressionArm>.GetInstance(source.SwitchArms.Length);
            for (int i = 0, n = source.SwitchArms.Length; i < n; i++)
            {
                var oldCase = source.SwitchArms[i];
                var oldValue = oldCase.Value;
                var newValue =
                    targetTyped
                    ? CreateConversion(oldValue.Syntax, oldValue, underlyingConversions[i], isCast: false, conversionGroupOpt: null, destination, diagnostics)
                    : GenerateConversionForAssignment(destination, oldValue, diagnostics);
                var newCase = (oldValue == newValue) ? oldCase :
                    new BoundSwitchExpressionArm(oldCase.Syntax, oldCase.Locals, oldCase.Pattern, oldCase.WhenClause, newValue, oldCase.Label, oldCase.HasErrors);
                builder.Add(newCase);
            }

            var newSwitchArms = builder.ToImmutableAndFree();
            return new BoundConvertedSwitchExpression(
                source.Syntax, source.Type, targetTyped, conversion, source.Expression, newSwitchArms, source.DecisionDag,
                source.DefaultLabel, source.ReportedNotExhaustive, destination, hasErrors || source.HasErrors);
        }

        private BoundExpression CreateUserDefinedConversion(
            SyntaxNode syntax,
            BoundExpression source,
            Conversion conversion,
            bool isCast,
            ConversionGroup conversionGroup,
            TypeSymbol destination,
            BindingDiagnosticBag diagnostics,
            bool hasErrors)
        {

            if (!conversion.IsValid)
            {
                if (!hasErrors)
                    GenerateImplicitConversionError(diagnostics, syntax, conversion, source, destination);

                return new BoundConversion(
                    syntax,
                    source,
                    conversion,
                    CheckOverflowAtRuntime,
                    explicitCastInCode: isCast,
                    conversionGroup,
                    constantValueOpt: ConstantValue.NotAvailable,
                    type: destination,
                    hasErrors: true)
                { WasCompilerGenerated = source.WasCompilerGenerated };
            }

            // Due to an oddity in the way we create a non-lifted user-defined conversion from A to D? 
            // (required backwards compatibility with the native compiler) we can end up in a situation 
            // where we have:
            // a standard conversion from A to B?
            // then a standard conversion from B? to B
            // then a user-defined  conversion from B to C
            // then a standard conversion from C to C? 
            // then a standard conversion from C? to D?
            //
            // In that scenario, the "from type" of the conversion will be B? and the "from conversion" will be 
            // from A to B?. Similarly the "to type" of the conversion will be C? and the "to conversion"
            // of the conversion will be from C? to D?.
            //
            // Therefore, we might need to introduce an extra conversion on the source side, from B? to B.
            // Now, you might think we should also introduce an extra conversion on the destination side,
            // from C to C?. But that then gives us the following bad situation: If we in fact bind this as
            //
            // (D?)(C?)(C)(B)(B?)(A)x 
            //
            // then what we are in effect doing is saying "convert C? to D? by checking for null, unwrapping,
            // converting C to D, and then wrapping". But we know that the C? will never be null. In this case
            // we should actually generate
            //
            // (D?)(C)(B)(B?)(A)x
            //
            // And thereby skip the unnecessary nullable conversion.


            // Original expression --> conversion's "from" type
            BoundExpression convertedOperand = CreateConversion(
                syntax: source.Syntax,
                source: source,
                conversion: conversion.UserDefinedFromConversion,
                isCast: false,
                conversionGroupOpt: conversionGroup,
                wasCompilerGenerated: false,
#nullable restore
                destination: conversion.BestUserDefinedConversionAnalysis.FromType,
#nullable enable
                diagnostics: diagnostics);

            TypeSymbol conversionParameterType = conversion.BestUserDefinedConversionAnalysis.Operator.GetParameterType(0);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);

            if (conversion.BestUserDefinedConversionAnalysis.Kind == UserDefinedConversionAnalysisKind.ApplicableInNormalForm &&
                !TypeSymbol.Equals(conversion.BestUserDefinedConversionAnalysis.FromType, conversionParameterType, TypeCompareKind.ConsiderEverything2))
            {
                // Conversion's "from" type --> conversion method's parameter type.
                convertedOperand = CreateConversion(
                    syntax: syntax,
                    source: convertedOperand,
                    conversion: Conversions.ClassifyStandardConversion(null, convertedOperand.Type, conversionParameterType, ref useSiteInfo),
                    isCast: false,
                    conversionGroupOpt: conversionGroup,
                    wasCompilerGenerated: true,
                    destination: conversionParameterType,
                    diagnostics: diagnostics);
            }

            BoundExpression userDefinedConversion;

            TypeSymbol conversionReturnType = conversion.BestUserDefinedConversionAnalysis.Operator.ReturnType;
            TypeSymbol conversionToType = conversion.BestUserDefinedConversionAnalysis.ToType;
            Conversion toConversion = conversion.UserDefinedToConversion;

            if (conversion.BestUserDefinedConversionAnalysis.Kind == UserDefinedConversionAnalysisKind.ApplicableInNormalForm &&
                !TypeSymbol.Equals(conversionToType, conversionReturnType, TypeCompareKind.ConsiderEverything2))
            {
                // Conversion method's parameter type --> conversion method's return type
                // NB: not calling CreateConversion here because this is the recursive base case.
                userDefinedConversion = new BoundConversion(
                    syntax,
                    convertedOperand,
                    conversion,
                    @checked: false, // There are no checked user-defined conversions, but the conversions on either side might be checked.
                    explicitCastInCode: isCast,
                    conversionGroup,
                    constantValueOpt: ConstantValue.NotAvailable,
                    type: conversionReturnType)
                { WasCompilerGenerated = true };

                if (conversionToType.IsNullableType() && TypeSymbol.Equals(conversionToType.GetNullableUnderlyingType(), conversionReturnType, TypeCompareKind.ConsiderEverything2))
                {
                    // Skip introducing the conversion from C to C?.  The "to" conversion is now wrong though,
                    // because it will still assume converting C? to D?. 

                    toConversion = Conversions.ClassifyConversionFromType(conversionReturnType, destination, ref useSiteInfo);
                }
                else
                {
                    // Conversion method's return type --> conversion's "to" type
                    userDefinedConversion = CreateConversion(
                        syntax: syntax,
                        source: userDefinedConversion,
                        conversion: Conversions.ClassifyStandardConversion(null, conversionReturnType, conversionToType, ref useSiteInfo),
                        isCast: false,
                        conversionGroupOpt: conversionGroup,
                        wasCompilerGenerated: true,
                        destination: conversionToType,
                        diagnostics: diagnostics);
                }
            }
            else
            {
                // Conversion method's parameter type --> conversion method's "to" type
                // NB: not calling CreateConversion here because this is the recursive base case.
                userDefinedConversion = new BoundConversion(
                    syntax,
                    convertedOperand,
                    conversion,
                    @checked: false,
                    explicitCastInCode: isCast,
                    conversionGroup,
                    constantValueOpt: ConstantValue.NotAvailable,
                    type: conversionToType)
                { WasCompilerGenerated = true };
            }

            diagnostics.Add(syntax, useSiteInfo);

            // Conversion's "to" type --> final type
            BoundExpression finalConversion = CreateConversion(
                syntax: syntax,
                source: userDefinedConversion,
                conversion: toConversion,
                isCast: false,
                conversionGroupOpt: conversionGroup,
                wasCompilerGenerated: true, // NOTE: doesn't necessarily set flag on resulting bound expression.
                destination: destination,
                diagnostics: diagnostics);

            finalConversion.ResetCompilerGenerated(source.WasCompilerGenerated);

            return finalConversion;
        }

        private BoundExpression CreateAnonymousFunctionConversion(SyntaxNode syntax, BoundExpression source, Conversion conversion, bool isCast, ConversionGroup? conversionGroup, TypeSymbol destination, BindingDiagnosticBag diagnostics)
        {
            // We have a successful anonymous function conversion; rather than producing a node
            // which is a conversion on top of an unbound lambda, replace it with the bound
            // lambda.

            // UNDONE: Figure out what to do about the error case, where a lambda
            // UNDONE: is converted to a delegate that does not match. What to surface then?

            var unboundLambda = (UnboundLambda)source;
            if ((destination.SpecialType == SpecialType.System_Delegate || destination.IsNonGenericExpressionType()) &&
                syntax.IsFeatureEnabled(MessageID.IDS_FeatureInferredDelegateType))
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                var delegateType = unboundLambda.InferDelegateType(ref useSiteInfo);
                BoundLambda boundLambda;
                if (delegateType is { })
                {
                    if (destination.IsNonGenericExpressionType())
                    {
                        delegateType = Compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T).Construct(delegateType);
                        delegateType.AddUseSiteInfo(ref useSiteInfo);
                    }
                    boundLambda = unboundLambda.Bind(delegateType);
                }
                else
                {
                    diagnostics.Add(ErrorCode.ERR_CannotInferDelegateType, syntax.GetLocation());
                    delegateType = CreateErrorType();
                    boundLambda = unboundLambda.BindForErrorRecovery();
                }
                diagnostics.AddRange(boundLambda.Diagnostics);
                var expr = createAnonymousFunctionConversion(syntax, source, boundLambda, conversion, isCast, conversionGroup, delegateType);
                conversion = Conversions.ClassifyConversionFromExpression(expr, destination, ref useSiteInfo);
                diagnostics.Add(syntax, useSiteInfo);
                return CreateConversion(syntax, expr, conversion, isCast, conversionGroup, destination, diagnostics);
            }
            else
            {
#if DEBUG
                // Test inferring a delegate type for all callers.
                var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                _ = unboundLambda.InferDelegateType(ref discardedUseSiteInfo);
#endif
                var boundLambda = unboundLambda.Bind((NamedTypeSymbol)destination);
                diagnostics.AddRange(boundLambda.Diagnostics);
                return createAnonymousFunctionConversion(syntax, source, boundLambda, conversion, isCast, conversionGroup, destination);
            }

            static BoundConversion createAnonymousFunctionConversion(SyntaxNode syntax, BoundExpression source, BoundLambda boundLambda, Conversion conversion, bool isCast, ConversionGroup? conversionGroup, TypeSymbol destination)
            {
                return new BoundConversion(
                    syntax,
                    boundLambda,
                    conversion,
                    @checked: false,
                    explicitCastInCode: isCast,
                    conversionGroup,
                    constantValueOpt: ConstantValue.NotAvailable,
                    type: destination)
                { WasCompilerGenerated = source.WasCompilerGenerated };
            }
        }

        private BoundExpression CreateMethodGroupConversion(SyntaxNode syntax, BoundExpression source, Conversion conversion, bool isCast, ConversionGroup? conversionGroup, TypeSymbol destination, BindingDiagnosticBag diagnostics)
        {
            var (originalGroup, isAddressOf) = source switch
            {
                BoundMethodGroup m => (m, false),
                BoundUnconvertedAddressOfOperator { Operand: { } m } => (m, true),
                _ => throw ExceptionUtilities.UnexpectedValue(source),
            };
            BoundMethodGroup group = FixMethodGroupWithTypeOrValue(originalGroup, conversion, diagnostics);
            bool hasErrors = false;

            if (MethodGroupConversionHasErrors(syntax, conversion, group.ReceiverOpt, conversion.IsExtensionMethod, isAddressOf, destination, diagnostics))
            {
                hasErrors = true;
            }

            if (destination.SpecialType == SpecialType.System_Delegate &&
                syntax.IsFeatureEnabled(MessageID.IDS_FeatureInferredDelegateType))
            {
                // https://github.com/dotnet/roslyn/issues/52869: Avoid calculating the delegate type multiple times during conversion.
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                var delegateType = GetMethodGroupDelegateType(group, ref useSiteInfo);
                var expr = createMethodGroupConversion(syntax, group, conversion, isCast, conversionGroup, delegateType!, hasErrors);
                conversion = Conversions.ClassifyConversionFromExpression(expr, destination, ref useSiteInfo);
                diagnostics.Add(syntax, useSiteInfo);
                return CreateConversion(syntax, expr, conversion, isCast, conversionGroup, destination, diagnostics);
            }

#if DEBUG
            // Test inferring a delegate type for all callers.
            var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            _ = GetMethodGroupDelegateType(group, ref discardedUseSiteInfo);
#endif
            return createMethodGroupConversion(syntax, group, conversion, isCast, conversionGroup, destination, hasErrors);

            static BoundConversion createMethodGroupConversion(SyntaxNode syntax, BoundMethodGroup group, Conversion conversion, bool isCast, ConversionGroup? conversionGroup, TypeSymbol destination, bool hasErrors)
            {
                return new BoundConversion(syntax, group, conversion, @checked: false, explicitCastInCode: isCast, conversionGroup, constantValueOpt: ConstantValue.NotAvailable, type: destination, hasErrors: hasErrors) { WasCompilerGenerated = group.WasCompilerGenerated };
            }
        }

        private BoundExpression CreateStackAllocConversion(SyntaxNode syntax, BoundExpression source, Conversion conversion, bool isCast, ConversionGroup? conversionGroup, TypeSymbol destination, BindingDiagnosticBag diagnostics)
        {

            var boundStackAlloc = (BoundStackAllocArrayCreation)source;
            var elementType = boundStackAlloc.ElementType;
            TypeSymbol stackAllocType;

            switch (conversion.Kind)
            {
                case ConversionKind.StackAllocToPointerType:
                    ReportUnsafeIfNotAllowed(syntax.Location, diagnostics);
                    stackAllocType = new PointerTypeSymbol(TypeWithAnnotations.Create(elementType));
                    break;
                case ConversionKind.StackAllocToSpanType:
                    CheckFeatureAvailability(syntax, MessageID.IDS_FeatureRefStructs, diagnostics);
                    stackAllocType = Compilation.GetWellKnownType(WellKnownType.System_Span_T).Construct(elementType);
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(conversion.Kind);
            }

            var convertedNode = new BoundConvertedStackAllocExpression(syntax, elementType, boundStackAlloc.Count, boundStackAlloc.InitializerOpt, stackAllocType, boundStackAlloc.HasErrors);

            var underlyingConversion = conversion.UnderlyingConversions.Single();
            return CreateConversion(syntax, convertedNode, underlyingConversion, isCast: isCast, conversionGroup, destination, diagnostics);
        }

        private BoundExpression CreateTupleLiteralConversion(SyntaxNode syntax, BoundTupleLiteral sourceTuple, Conversion conversion, bool isCast, ConversionGroup? conversionGroup, TypeSymbol destination, BindingDiagnosticBag diagnostics)
        {
            // We have a successful tuple conversion; rather than producing a separate conversion node 
            // which is a conversion on top of a tuple literal, tuple conversion is an element-wise conversion of arguments.

            var destinationWithoutNullable = destination;
            var conversionWithoutNullable = conversion;

            if (conversion.IsNullable)
            {
                destinationWithoutNullable = destination.GetNullableUnderlyingType();
                conversionWithoutNullable = conversion.UnderlyingConversions[0];
            }


            NamedTypeSymbol targetType = (NamedTypeSymbol)destinationWithoutNullable;
            if (targetType.IsTupleType)
            {
                NamedTypeSymbol.ReportTupleNamesMismatchesIfAny(targetType, sourceTuple, diagnostics);

                // do not lose the original element names and locations in the literal if different from names in the target
                //
                // the tuple has changed the type of elements due to target-typing, 
                // but element names has not changed and locations of their declarations 
                // should not be confused with element locations on the target type.

                if (sourceTuple.Type is NamedTypeSymbol { IsTupleType: true } sourceType)
                {
                    targetType = targetType.WithTupleDataFrom(sourceType);
                }
                else
                {
                    var tupleSyntax = (TupleExpressionSyntax)sourceTuple.Syntax;
                    var locationBuilder = ArrayBuilder<Location?>.GetInstance();

                    foreach (var argument in tupleSyntax.Arguments)
                    {
                        locationBuilder.Add(argument.NameColon?.Name.Location);
                    }

                    targetType = targetType.WithElementNames(sourceTuple.ArgumentNamesOpt!,
                        locationBuilder.ToImmutableAndFree(),
                        errorPositions: default,
                        ImmutableArray.Create(tupleSyntax.Location));
                }
            }

            var arguments = sourceTuple.Arguments;
            var convertedArguments = ArrayBuilder<BoundExpression>.GetInstance(arguments.Length);

            var targetElementTypes = targetType.TupleElementTypesWithAnnotations;
            var underlyingConversions = conversionWithoutNullable.UnderlyingConversions;

            for (int i = 0; i < arguments.Length; i++)
            {
                var argument = arguments[i];
                var destType = targetElementTypes[i];
                var elementConversion = underlyingConversions[i];
                var elementConversionGroup = isCast ? new ConversionGroup(elementConversion, destType) : null;
                convertedArguments.Add(CreateConversion(argument.Syntax, argument, elementConversion, isCast: isCast, elementConversionGroup, destType.Type, diagnostics));
            }

            BoundExpression result = new BoundConvertedTupleLiteral(
                sourceTuple.Syntax,
                sourceTuple,
                wasTargetTyped: true,
                convertedArguments.ToImmutableAndFree(),
                sourceTuple.ArgumentNamesOpt,
                sourceTuple.InferredNamesOpt,
                targetType).WithSuppression(sourceTuple.IsSuppressed);

            if (!TypeSymbol.Equals(sourceTuple.Type, destination, TypeCompareKind.ConsiderEverything2))
            {
                // literal cast is applied to the literal 
                result = new BoundConversion(
                    sourceTuple.Syntax,
                    result,
                    conversion,
                    @checked: false,
                    explicitCastInCode: isCast,
                    conversionGroup,
                    constantValueOpt: ConstantValue.NotAvailable,
                    type: destination);
            }

            // If we had a cast in the code, keep conversion in the tree.
            // even though the literal is already converted to the target type.
            if (isCast)
            {
                result = new BoundConversion(
                    syntax,
                    result,
                    Conversion.Identity,
                    @checked: false,
                    explicitCastInCode: isCast,
                    conversionGroup,
                    constantValueOpt: ConstantValue.NotAvailable,
                    type: destination);
            }

            return result;
        }

        private static bool IsMethodGroupWithTypeOrValueReceiver(BoundNode node)
        {
            if (node.Kind != BoundKind.MethodGroup)
            {
                return false;
            }

            return Binder.IsTypeOrValueExpression(((BoundMethodGroup)node).ReceiverOpt);
        }

        private BoundMethodGroup FixMethodGroupWithTypeOrValue(BoundMethodGroup group, Conversion conversion, BindingDiagnosticBag diagnostics)
        {
            if (!IsMethodGroupWithTypeOrValueReceiver(group))
            {
                return group;
            }

            BoundExpression? receiverOpt = group.ReceiverOpt;

            receiverOpt = ReplaceTypeOrValueReceiver(receiverOpt, useType: conversion.Method?.RequiresInstanceReceiver == false && !conversion.IsExtensionMethod, diagnostics);
            return group.Update(
                group.TypeArgumentsOpt,
                group.Name,
                group.Methods,
                group.LookupSymbolOpt,
                group.LookupError,
                group.Flags,
                receiverOpt, //only change
                group.ResultKind);
        }

        /// <summary>
        /// This method implements the algorithm in spec section 7.6.5.1.
        /// 
        /// For method group conversions, there are situations in which the conversion is
        /// considered to exist ("Otherwise the algorithm produces a single best method M having
        /// the same number of parameters as D and the conversion is considered to exist"), but
        /// application of the conversion fails.  These are the "final validation" steps of
        /// overload resolution.
        /// </summary>
        /// <returns>
        /// True if there is any error, except lack of runtime support errors.
        /// </returns>
        private bool MemberGroupFinalValidation(BoundExpression? receiverOpt, MethodSymbol methodSymbol, SyntaxNode node, BindingDiagnosticBag diagnostics, bool invokedAsExtensionMethod)
        {
            if (!IsBadBaseAccess(node, receiverOpt, methodSymbol, diagnostics))
            {
                CheckRuntimeSupportForSymbolAccess(node, /*receiverOpt, */methodSymbol, diagnostics);
            }

            if (MemberGroupFinalValidationAccessibilityChecks(receiverOpt, methodSymbol, node, diagnostics, invokedAsExtensionMethod))
            {
                return true;
            }

            // SPEC: If the best method is a generic method, the type arguments (supplied or inferred) are checked against the constraints 
            // SPEC: declared on the generic method. If any type argument does not satisfy the corresponding constraint(s) on
            // SPEC: the type parameter, a binding-time error occurs.

            // The portion of the overload resolution spec quoted above is subtle and somewhat
            // controversial. The upshot of this is that overload resolution does not consider
            // constraints to be a part of the signature. Overload resolution matches arguments to
            // parameter lists; it does not consider things which are outside of the parameter list.
            // If the best match from the arguments to the formal parameters is not viable then we
            // give an error rather than falling back to a worse match. 
            //
            // Consider the following:
            //
            // void M<T>(T t) where T : Reptile {}
            // void M(object x) {}
            // ...
            // M(new Giraffe());
            //
            // The correct analysis is to determine that the applicable candidates are
            // M<Giraffe>(Giraffe) and M(object). Overload resolution then chooses the former
            // because it is an exact match, over the latter which is an inexact match. Only after
            // the best method is determined do we check the constraints and discover that the
            // constraint on T has been violated.
            // 
            // Note that this is different from the rule that says that during type inference, if an
            // inference violates a constraint then inference fails. For example:
            // 
            // class C<T> where T : struct {}
            // ...
            // void M<U>(U u, C<U> c){}
            // void M(object x, object y) {}
            // ...
            // M("hello", null);
            //
            // Type inference determines that U is string, but since C<string> is not a valid type
            // because of the constraint, type inference fails. M<string> is never added to the
            // applicable candidate set, so the applicable candidate set consists solely of
            // M(object, object) and is therefore the best match.

            return !methodSymbol.CheckConstraints(new ConstraintsHelper.CheckConstraintsArgs(this.Compilation, this.Conversions, includeNullability: false, node.Location, diagnostics));
        }

        /// <summary>
        /// Performs the following checks:
        /// 
        /// Spec 7.6.5: Invocation expressions (definition of Final Validation) 
        ///   The method is validated in the context of the method group: If the best method is a static method, 
        ///   the method group must have resulted from a simple-name or a member-access through a type. If the best 
        ///   method is an instance method, the method group must have resulted from a simple-name, a member-access
        ///   through a variable or value, or a base-access. If neither of these requirements is true, a binding-time
        ///   error occurs.
        ///   (Note that the spec omits to mention, in the case of an instance method invoked through a simple name, that
        ///   the invocation must appear within the body of an instance method)
        ///
        /// Spec 7.5.4: Compile-time checking of dynamic overload resolution 
        ///   If F is a static method, the method group must have resulted from a simple-name, a member-access through a type, 
        ///   or a member-access whose receiver can't be classified as a type or value until after overload resolution (see §7.6.4.1). 
        ///   If F is an instance method, the method group must have resulted from a simple-name, a member-access through a variable or value,
        ///   or a member-access whose receiver can't be classified as a type or value until after overload resolution (see §7.6.4.1).
        /// </summary>
        /// <returns>
        /// True if there is any error.
        /// </returns>
        private bool MemberGroupFinalValidationAccessibilityChecks(BoundExpression? receiverOpt, Symbol memberSymbol, SyntaxNode node, BindingDiagnosticBag diagnostics, bool invokedAsExtensionMethod)
        {
            // Perform final validation of the method to be invoked.

            //note that the same assert does not hold for all properties. Some properties and (all indexers) are not referenceable by name, yet
            //their binding brings them through here, perhaps needlessly.

            if (IsTypeOrValueExpression(receiverOpt))
            {
                // TypeOrValue expression isn't replaced only if the invocation is late bound, in which case it can't be extension method.
                // None of the checks below apply if the receiver can't be classified as a type or value. 
            }
            else if (!memberSymbol.RequiresInstanceReceiver())
            {

                if (invokedAsExtensionMethod)
                {
                    if (IsMemberAccessedThroughType(receiverOpt))
                    {
                        if (receiverOpt.Kind == BoundKind.QueryClause)
                        {
                            // Could not find an implementation of the query pattern for source type '{0}'.  '{1}' not found.
#nullable restore
                            diagnostics.Add(ErrorCode.ERR_QueryNoProvider, node.Location, receiverOpt.Type, memberSymbol.Name);
                        }
                        else
                        {
                            // An object reference is required for the non-static field, method, or property '{0}'
                            diagnostics.Add(ErrorCode.ERR_ObjectRequired, node.Location, memberSymbol);
                        }
                        return true;
                    }
                }
                else if (!WasImplicitReceiver(receiverOpt) && IsMemberAccessedThroughVariableOrValue(receiverOpt))
                {
                    if (this.Flags.Includes(BinderFlags.CollectionInitializerAddMethod))
                    {
                        diagnostics.Add(ErrorCode.ERR_InitializerAddHasWrongSignature, node.Location, memberSymbol);
                    }
                    else if (node.Kind() == SyntaxKind.AwaitExpression && memberSymbol.Name == WellKnownMemberNames.GetAwaiter)
                    {
                        diagnostics.Add(ErrorCode.ERR_BadAwaitArg, node.Location, receiverOpt.Type);
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_ObjectProhibited, node.Location, memberSymbol);
                    }
                    return true;
                }
            }
            else if (IsMemberAccessedThroughType(receiverOpt))
            {
                diagnostics.Add(ErrorCode.ERR_ObjectRequired, node.Location, memberSymbol);
                return true;
            }
            else if (WasImplicitReceiver(receiverOpt))
            {
                if (InFieldInitializer && !ContainingType!.IsScriptClass || InConstructorInitializer || InAttributeArgument)
                {
                    SyntaxNode errorNode = node;
                    if (node.Parent != null && node.Parent.Kind() == SyntaxKind.InvocationExpression)
                    {
                        errorNode = node.Parent;
                    }

                    ErrorCode code = InFieldInitializer ? ErrorCode.ERR_FieldInitRefNonstatic : ErrorCode.ERR_ObjectRequired;
                    diagnostics.Add(code, errorNode.Location, memberSymbol);
                    return true;
                }

                // If we could access the member through implicit "this" the receiver would be a BoundThisReference.
                // If it is null it means that the instance member is inaccessible.
                if (receiverOpt == null || ContainingMember().IsStatic)
                {
                    Error(diagnostics, ErrorCode.ERR_ObjectRequired, node, memberSymbol);
                    return true;
                }
            }

            var containingType = this.ContainingType;
            if (containingType is object)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                bool isAccessible = this.IsSymbolAccessibleConditional(memberSymbol.GetTypeOrReturnType().Type, containingType, ref useSiteInfo);
                diagnostics.Add(node, useSiteInfo);

                if (!isAccessible)
                {
                    // In the presence of non-transitive [InternalsVisibleTo] in source, or obnoxious symbols from metadata, it is possible
                    // to select a method through overload resolution in which the type is not accessible.  In this case a method cannot
                    // be called through normal IL, so we give an error.  Neither [InternalsVisibleTo] nor the need for this diagnostic is
                    // described by the language specification.
                    //
                    // Dev11 perform different access checks. See bug #530360 and tests AccessCheckTests.InaccessibleReturnType.
                    Error(diagnostics, ErrorCode.ERR_BadAccess, node, memberSymbol);
                    return true;
                }
            }

            return false;
        }

#nullable enable
        private static bool IsMemberAccessedThroughVariableOrValue(BoundExpression? receiverOpt)
        {
            if (receiverOpt == null)
            {
                return false;
            }

            return !IsMemberAccessedThroughType(receiverOpt);
        }

        internal static bool IsMemberAccessedThroughType([NotNullWhen(true)] BoundExpression? receiverOpt)
        {
            if (receiverOpt == null)
            {
                return false;
            }

            while (receiverOpt.Kind == BoundKind.QueryClause)
            {
                receiverOpt = ((BoundQueryClause)receiverOpt).Value;
            }

            return receiverOpt.Kind == BoundKind.TypeExpression;
        }

        /// <summary>
        /// Was the receiver expression compiler-generated?
        /// </summary>
        internal static bool WasImplicitReceiver([NotNullWhen(false)] BoundExpression? receiverOpt)
        {
            if (receiverOpt == null) return true;
            if (!receiverOpt.WasCompilerGenerated) return false;
            return receiverOpt.Kind switch
            {
                BoundKind.ThisReference or BoundKind.HostObjectMemberReference or BoundKind.PreviousSubmissionReference => true,
                _ => false,
            };
        }

        /// <summary>
        /// This method implements the checks in spec section 15.2.
        /// </summary>
        internal bool MethodIsCompatibleWithDelegateOrFunctionPointer(/*BoundExpression? receiverOpt, */bool isExtensionMethod, MethodSymbol method, TypeSymbol delegateType, Location errorLocation, BindingDiagnosticBag diagnostics)
        {
            MethodSymbol delegateOrFuncPtrMethod = delegateType switch
            {
                NamedTypeSymbol { DelegateInvokeMethod: { } invokeMethod } => invokeMethod,
                FunctionPointerTypeSymbol { Signature: { } signature } => signature,
                _ => throw ExceptionUtilities.UnexpectedValue(delegateType),
            };


            // - Argument types "match", and
            var delegateOrFuncPtrParameters = delegateOrFuncPtrMethod.Parameters;
            var methodParameters = method.Parameters;
            int numParams = delegateOrFuncPtrParameters.Length;

            if (methodParameters.Length != numParams + (isExtensionMethod ? 1 : 0))
            {
                // This can happen if "method" has optional parameters.
                Error(diagnostics, getMethodMismatchErrorCode(delegateType.TypeKind), errorLocation, method, delegateType);
                return false;
            }

            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);

            // If this is an extension method delegate, the caller should have verified the
            // receiver is compatible with the "this" parameter of the extension method.

            useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(useSiteInfo);

            for (int i = 0; i < numParams; i++)
            {
                var delegateParameter = delegateOrFuncPtrParameters[i];
                var methodParameter = methodParameters[isExtensionMethod ? i + 1 : i];

                // The delegate compatibility checks are stricter than the checks on applicable functions: it's possible
                // to get here with a method that, while all the parameters are applicable, is not actually delegate
                // compatible. This is because the Applicable function member spec requires that:
                //  * Every value parameter (non-ref or similar) from the delegate type has an implicit conversion to the corresponding
                //    target parameter
                //  * Every ref or similar parameter has an identity conversion to the corresponding target parameter
                // However, the delegate compatibility requirements are stricter:
                //  * Every value parameter (non-ref or similar) from the delegate type has an implicit _reference_ conversion to the
                //    corresponding target parameter.
                //  * Every ref or similar parameter has an identity conversion to the corresponding target parameter
                // Note the addition of the reference requirement: it means that for delegate type void D(int i), void M(long l) is
                // _applicable_, but not _compatible_.
                if (!hasConversion(delegateType.TypeKind, Conversions, delegateParameter.Type, methodParameter.Type, delegateParameter.RefKind, methodParameter.RefKind, ref useSiteInfo))
                {
                    // No overload for '{0}' matches delegate '{1}'
                    Error(diagnostics, getMethodMismatchErrorCode(delegateType.TypeKind), errorLocation, method, delegateType);
                    diagnostics.Add(errorLocation, useSiteInfo);
                    return false;
                }
            }

            if (delegateOrFuncPtrMethod.RefKind != method.RefKind)
            {
                Error(diagnostics, getRefMismatchErrorCode(delegateType.TypeKind), errorLocation, method, delegateType);
                diagnostics.Add(errorLocation, useSiteInfo);
                return false;
            }

            var methodReturnType = method.ReturnType;
            var delegateReturnType = delegateOrFuncPtrMethod.ReturnType;
            bool returnsMatch = delegateOrFuncPtrMethod switch
            {
                { RefKind: RefKind.None, ReturnsVoid: true } => method.ReturnsVoid,
                { RefKind: var destinationRefKind } => hasConversion(delegateType.TypeKind, Conversions, methodReturnType, delegateReturnType, method.RefKind, destinationRefKind, ref useSiteInfo),
            };

            if (!returnsMatch)
            {
                Error(diagnostics, ErrorCode.ERR_BadRetType, errorLocation, method, method.ReturnType);
                diagnostics.Add(errorLocation, useSiteInfo);
                return false;
            }

            if (delegateType.IsFunctionPointer())
            {
                if (isExtensionMethod)
                {
                    Error(diagnostics, ErrorCode.ERR_CannotUseReducedExtensionMethodInAddressOf, errorLocation);
                    diagnostics.Add(errorLocation, useSiteInfo);
                    return false;
                }

                if (!method.IsStatic)
                {
                    // This check is here purely for completeness of implementing the spec. It should
                    // never be hit, as static methods should be eliminated as candidates in overload
                    // resolution and should never make it to this point.
                    Debug.Fail("This method should have been eliminated in overload resolution!");
                    Error(diagnostics, ErrorCode.ERR_FuncPtrMethMustBeStatic, errorLocation, method);
                    diagnostics.Add(errorLocation, useSiteInfo);
                    return false;
                }
            }

            diagnostics.Add(errorLocation, useSiteInfo);
            return true;

            static bool hasConversion(TypeKind targetKind, Conversions conversions, TypeSymbol source, TypeSymbol destination,
                RefKind sourceRefKind, RefKind destinationRefKind, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
            {
                if (sourceRefKind != destinationRefKind)
                {
                    return false;
                }

                if (sourceRefKind != RefKind.None)
                {
                    return ConversionsBase.HasIdentityConversion(source, destination);
                }

                if (conversions.HasIdentityOrImplicitReferenceConversion(source, destination, ref useSiteInfo))
                {
                    return true;
                }

                return targetKind == TypeKind.FunctionPointer
                       && (ConversionsBase.HasImplicitPointerToVoidConversion(source, destination)
                           || conversions.HasImplicitPointerConversion(source, destination, ref useSiteInfo));
            }

            static ErrorCode getMethodMismatchErrorCode(TypeKind type)
                => type switch
                {
                    TypeKind.Delegate => ErrorCode.ERR_MethDelegateMismatch,
                    TypeKind.FunctionPointer => ErrorCode.ERR_MethFuncPtrMismatch,
                    _ => throw ExceptionUtilities.UnexpectedValue(type)
                };

            static ErrorCode getRefMismatchErrorCode(TypeKind type)
                => type switch
                {
                    TypeKind.Delegate => ErrorCode.ERR_DelegateRefMismatch,
                    TypeKind.FunctionPointer => ErrorCode.ERR_FuncPtrRefMismatch,
                    _ => throw ExceptionUtilities.UnexpectedValue(type)
                };
        }

        /// <summary>
        /// This method combines final validation (section 7.6.5.1) and delegate compatibility (section 15.2).
        /// </summary>
        /// <param name="syntax">CSharpSyntaxNode of the expression requiring method group conversion.</param>
        /// <param name="conversion">Conversion to be performed.</param>
        /// <param name="receiverOpt">Optional receiver.</param>
        /// <param name="isExtensionMethod">Method invoked as extension method.</param>
        /// <param name="delegateOrFuncPtrType">Target delegate type.</param>
        /// <param name="diagnostics">Where diagnostics should be added.</param>
        /// <returns>True if a diagnostic has been added.</returns>
        private bool MethodGroupConversionHasErrors(
            SyntaxNode syntax,
            Conversion conversion,
            BoundExpression? receiverOpt,
            bool isExtensionMethod,
            bool isAddressOf,
            TypeSymbol delegateOrFuncPtrType,
            BindingDiagnosticBag diagnostics)
        {
#nullable restore
            MethodSymbol selectedMethod = conversion.Method;

            var location = syntax.Location;
            if (delegateOrFuncPtrType.SpecialType != SpecialType.System_Delegate)
            {
                if (!MethodIsCompatibleWithDelegateOrFunctionPointer(/*receiverOpt, */isExtensionMethod, selectedMethod, delegateOrFuncPtrType, location, diagnostics) ||
                    MemberGroupFinalValidation(receiverOpt, selectedMethod, syntax, diagnostics, isExtensionMethod))
                {
                    return true;
                }
            }

            if (selectedMethod.IsConditional)
            {
                // CS1618: Cannot create delegate with '{0}' because it has a Conditional attribute
                Error(diagnostics, ErrorCode.ERR_DelegateOnConditional, location, selectedMethod);
                return true;
            }

            if (selectedMethod is SourceOrdinaryMethodSymbol sourceMethod && sourceMethod.IsPartialWithoutImplementation)
            {
                // CS0762: Cannot create delegate from method '{0}' because it is a partial method without an implementing declaration
                Error(diagnostics, ErrorCode.ERR_PartialMethodToDelegate, location, selectedMethod);
                return true;
            }

            if ((selectedMethod.HasUnsafeParameter() || selectedMethod.ReturnType.IsUnsafe()) && ReportUnsafeIfNotAllowed(syntax, diagnostics))
            {
                return true;
            }
            if (!isAddressOf)
            {
                ReportDiagnosticsIfUnmanagedCallersOnly(diagnostics, selectedMethod, location, isDelegateConversion: true);
            }
            ReportDiagnosticsIfObsolete(diagnostics, selectedMethod, syntax, hasBaseReceiver: false);

            // No use site errors, but there could be use site warnings.
            // If there are use site warnings, they were reported during the overload resolution process
            // that chose selectedMethod.

            return false;
        }

        /// <summary>
        /// This method is a wrapper around MethodGroupConversionHasErrors.  As a preliminary step,
        /// it checks whether a conversion exists.
        /// </summary>
        private bool MethodGroupConversionDoesNotExistOrHasErrors(
            BoundMethodGroup boundMethodGroup,
            NamedTypeSymbol delegateType,
            Location delegateMismatchLocation,
            BindingDiagnosticBag diagnostics,
            out Conversion conversion)
        {
            if (ReportDelegateInvokeUseSiteDiagnostic(diagnostics, delegateType, delegateMismatchLocation))
            {
                conversion = Conversion.NoConversion;
                return true;
            }

            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            conversion = Conversions.GetMethodGroupDelegateConversion(boundMethodGroup, delegateType, ref useSiteInfo);
            diagnostics.Add(delegateMismatchLocation, useSiteInfo);
            if (!conversion.Exists)
            {
                if (!Conversions.ReportDelegateOrFunctionPointerMethodGroupDiagnostics(this, boundMethodGroup, delegateType, diagnostics))
                {
                    // No overload for '{0}' matches delegate '{1}'
                    diagnostics.Add(ErrorCode.ERR_MethDelegateMismatch, delegateMismatchLocation, boundMethodGroup.Name, delegateType);
                }

                return true;
            }
            else
            {
                // Only cares about nullness and type of receiver, so no need to worry about BoundTypeOrValueExpression.
                return this.MethodGroupConversionHasErrors(boundMethodGroup.Syntax, conversion, boundMethodGroup.ReceiverOpt, conversion.IsExtensionMethod, isAddressOf: false, delegateType, diagnostics);
            }
        }

#nullable enable
        public ConstantValue? FoldConstantConversion(
            SyntaxNode syntax,
            BoundExpression source,
            Conversion conversion,
            TypeSymbol destination,
            BindingDiagnosticBag diagnostics)
        {
            // The diagnostics bag can be null in cases where we know ahead of time that the
            // conversion will succeed without error or warning. (For example, if we have a valid
            // implicit numeric conversion on a constant of numeric type.)

            // SPEC: A constant expression must be the null literal or a value with one of 
            // SPEC: the following types: sbyte, byte, short, ushort, int, uint, long, 
            // SPEC: ulong, char, float, double, decimal, bool, string, or any enumeration type.

            // SPEC: The following conversions are permitted in constant expressions:
            // SPEC: Identity conversions
            // SPEC: Numeric conversions
            // SPEC: Enumeration conversions
            // SPEC: Constant expression conversions
            // SPEC: Implicit and explicit reference conversions, provided that the source of the conversions 
            // SPEC: is a constant expression that evaluates to the null value.

            // SPEC VIOLATION: C# has always allowed the following, even though this does violate the rule that
            // SPEC VIOLATION: a constant expression must be either the null literal, or an expression of one 
            // SPEC VIOLATION: of the given types. 

            // SPEC VIOLATION: const C c = (C)null;

            // TODO: Some conversions can produce errors or warnings depending on checked/unchecked.
            // TODO: Fold conversions on enums and strings too.

            var sourceConstantValue = source.ConstantValue;
            if (sourceConstantValue == null)
            {
                if (conversion.Kind == ConversionKind.DefaultLiteral)
                {
                    return destination.GetDefaultValue();
                }
                else
                {
                    return sourceConstantValue;
                }
            }
            else if (sourceConstantValue.IsBad)
            {
                return sourceConstantValue;
            }

            if (source.HasAnyErrors)
            {
                return null;
            }

            switch (conversion.Kind)
            {
                case ConversionKind.Identity:
                    // An identity conversion to a floating-point type (for example from a cast in
                    // source code) changes the internal representation of the constant value
                    // to precisely the required precision.
                    return destination.SpecialType switch
                    {
                        SpecialType.System_Single => ConstantValue.Create(sourceConstantValue.SingleValue),
                        SpecialType.System_Double => ConstantValue.Create(sourceConstantValue.DoubleValue),
                        _ => sourceConstantValue,
                    };
                case ConversionKind.NullLiteral:
                    return sourceConstantValue;

                case ConversionKind.ImplicitConstant:
                    return FoldConstantNumericConversion(syntax, sourceConstantValue, destination, diagnostics);

                case ConversionKind.ExplicitNumeric:
                case ConversionKind.ImplicitNumeric:
                case ConversionKind.ExplicitEnumeration:
                case ConversionKind.ImplicitEnumeration:
                    // The C# specification categorizes conversion from literal zero to nullable enum as 
                    // an Implicit Enumeration Conversion. Such a thing should not be constant folded
                    // because nullable enums are never constants.

                    if (destination.IsNullableType())
                    {
                        return null;
                    }

                    return FoldConstantNumericConversion(syntax, sourceConstantValue, destination, diagnostics);

                case ConversionKind.ExplicitReference:
                case ConversionKind.ImplicitReference:
                    return sourceConstantValue.IsNull ? sourceConstantValue : null;
            }

            return null;
        }

        private ConstantValue? FoldConstantNumericConversion(
            SyntaxNode syntax,
            ConstantValue sourceValue,
            TypeSymbol destination,
            BindingDiagnosticBag diagnostics)
        {
            SpecialType destinationType;
            if (destination is object && destination.IsEnumType())
            {
                var underlyingType = ((NamedTypeSymbol)destination).EnumUnderlyingType;
                destinationType = underlyingType.SpecialType;
            }
            else
            {
                destinationType = destination.GetSpecialTypeSafe();
            }

            // In an unchecked context we ignore overflowing conversions on conversions from any
            // integral type, float and double to any integral type. "unchecked" actually does not
            // affect conversions from decimal to any integral type; if those are out of bounds then
            // we always give an error regardless.

            if (sourceValue.IsDecimal)
            {
                if (!CheckConstantBounds(destinationType, sourceValue, out _))
                {
                    // NOTE: Dev10 puts a suffix, "M", on the constant value.
                    Error(diagnostics, ErrorCode.ERR_ConstOutOfRange, syntax, sourceValue.Value + "M", destination!);
                    return ConstantValue.Bad;
                }
            }
            else if (destinationType == SpecialType.System_Decimal)
            {
                if (!CheckConstantBounds(destinationType, sourceValue, out _))
                {
                    Error(diagnostics, ErrorCode.ERR_ConstOutOfRange, syntax, sourceValue.Value!, destination!);
                    return ConstantValue.Bad;
                }
            }
            else if (CheckOverflowAtCompileTime)
            {
                if (!CheckConstantBounds(destinationType, sourceValue, out bool maySucceedAtRuntime))
                {
                    if (maySucceedAtRuntime)
                    {
                        // Can be calculated at runtime, but is not a compile-time constant.
                        Error(diagnostics, ErrorCode.WRN_ConstOutOfRangeChecked, syntax, sourceValue.Value!, destination!);
                        return null;
                    }
                    else
                    {
                        Error(diagnostics, ErrorCode.ERR_ConstOutOfRangeChecked, syntax, sourceValue.Value!, destination!);
                        return ConstantValue.Bad;
                    }
                }
            }
            else if (destinationType == SpecialType.System_IntPtr || destinationType == SpecialType.System_UIntPtr)
            {
                if (!CheckConstantBounds(destinationType, sourceValue, out _))
                {
                    // Can be calculated at runtime, but is not a compile-time constant.
                    return null;
                }
            }

            return ConstantValue.Create(DoUncheckedConversion(destinationType, sourceValue), destinationType);
        }

        private static object DoUncheckedConversion(SpecialType destinationType, ConstantValue value)
        {
            // Note that we keep "single" floats as doubles internally to maintain higher precision. However,
            // we do not do so in an entirely "lossless" manner. When *converting* to a float, we do lose 
            // the precision lost due to the conversion. But when doing arithmetic, we do the arithmetic on
            // the double values.
            //
            // An example will help. Suppose we have:
            //
            // const float cf1 = 1.0f;
            // const float cf2 = 1.0e-15f;
            // const double cd3 = cf1 - cf2;
            //
            // We first take the double-precision values for 1.0 and 1.0e-15 and round them to floats,
            // and then turn them back into doubles. Then when we do the subtraction, we do the subtraction
            // in doubles, not in floats. Had we done the subtraction in floats, we'd get 1.0; but instead we
            // do it in doubles and get 0.99999999999999.
            //
            // Similarly, if we have
            //
            // const int i4 = int.MaxValue; // 2147483647
            // const float cf5 = int.MaxValue; //  2147483648.0
            // const double cd6 = cf5; // 2147483648.0
            //
            // The int is converted to float and stored internally as the double 214783648, even though the
            // fully precise int would fit into a double.

            unchecked
            {
                switch (value.Discriminator)
                {
                    case ConstantValueTypeDiscriminator.Byte:
                        byte byteValue = value.ByteValue;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => byteValue,
                            SpecialType.System_Char => (char)byteValue,
                            SpecialType.System_UInt16 => (ushort)byteValue,
                            SpecialType.System_UInt32 => (uint)byteValue,
                            SpecialType.System_UInt64 => (ulong)byteValue,
                            SpecialType.System_SByte => (sbyte)byteValue,
                            SpecialType.System_Int16 => (short)byteValue,
                            SpecialType.System_Int32 => (int)byteValue,
                            SpecialType.System_Int64 => (long)byteValue,
                            SpecialType.System_IntPtr => (int)byteValue,
                            SpecialType.System_UIntPtr => (uint)byteValue,
                            SpecialType.System_Single or SpecialType.System_Double => (double)byteValue,
                            SpecialType.System_Decimal => (decimal)byteValue,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.Char:
                        char charValue = value.CharValue;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)charValue,
                            SpecialType.System_Char => charValue,
                            SpecialType.System_UInt16 => (ushort)charValue,
                            SpecialType.System_UInt32 => (uint)charValue,
                            SpecialType.System_UInt64 => (ulong)charValue,
                            SpecialType.System_SByte => (sbyte)charValue,
                            SpecialType.System_Int16 => (short)charValue,
                            SpecialType.System_Int32 => (int)charValue,
                            SpecialType.System_Int64 => (long)charValue,
                            SpecialType.System_IntPtr => (int)charValue,
                            SpecialType.System_UIntPtr => (uint)charValue,
                            SpecialType.System_Single or SpecialType.System_Double => (double)charValue,
                            SpecialType.System_Decimal => (decimal)charValue,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.UInt16:
                        ushort uint16Value = value.UInt16Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)uint16Value,
                            SpecialType.System_Char => (char)uint16Value,
                            SpecialType.System_UInt16 => uint16Value,
                            SpecialType.System_UInt32 => (uint)uint16Value,
                            SpecialType.System_UInt64 => (ulong)uint16Value,
                            SpecialType.System_SByte => (sbyte)uint16Value,
                            SpecialType.System_Int16 => (short)uint16Value,
                            SpecialType.System_Int32 => (int)uint16Value,
                            SpecialType.System_Int64 => (long)uint16Value,
                            SpecialType.System_IntPtr => (int)uint16Value,
                            SpecialType.System_UIntPtr => (uint)uint16Value,
                            SpecialType.System_Single or SpecialType.System_Double => (double)uint16Value,
                            SpecialType.System_Decimal => (decimal)uint16Value,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.UInt32:
                        uint uint32Value = value.UInt32Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)uint32Value,
                            SpecialType.System_Char => (char)uint32Value,
                            SpecialType.System_UInt16 => (ushort)uint32Value,
                            SpecialType.System_UInt32 => uint32Value,
                            SpecialType.System_UInt64 => (ulong)uint32Value,
                            SpecialType.System_SByte => (sbyte)uint32Value,
                            SpecialType.System_Int16 => (short)uint32Value,
                            SpecialType.System_Int32 => (int)uint32Value,
                            SpecialType.System_Int64 => (long)uint32Value,
                            SpecialType.System_IntPtr => (int)uint32Value,
                            SpecialType.System_UIntPtr => uint32Value,
                            SpecialType.System_Single => (double)(float)uint32Value,
                            SpecialType.System_Double => (double)uint32Value,
                            SpecialType.System_Decimal => (decimal)uint32Value,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.UInt64:
                        ulong uint64Value = value.UInt64Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)uint64Value,
                            SpecialType.System_Char => (char)uint64Value,
                            SpecialType.System_UInt16 => (ushort)uint64Value,
                            SpecialType.System_UInt32 => (uint)uint64Value,
                            SpecialType.System_UInt64 => uint64Value,
                            SpecialType.System_SByte => (sbyte)uint64Value,
                            SpecialType.System_Int16 => (short)uint64Value,
                            SpecialType.System_Int32 => (int)uint64Value,
                            SpecialType.System_Int64 => (long)uint64Value,
                            SpecialType.System_IntPtr => (int)uint64Value,
                            SpecialType.System_UIntPtr => (uint)uint64Value,
                            SpecialType.System_Single => (double)(float)uint64Value,
                            SpecialType.System_Double => (double)uint64Value,
                            SpecialType.System_Decimal => (decimal)uint64Value,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.NUInt:
                        uint nuintValue = value.UInt32Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)nuintValue,
                            SpecialType.System_Char => (char)nuintValue,
                            SpecialType.System_UInt16 => (ushort)nuintValue,
                            SpecialType.System_UInt32 => nuintValue,
                            SpecialType.System_UInt64 => (ulong)nuintValue,
                            SpecialType.System_SByte => (sbyte)nuintValue,
                            SpecialType.System_Int16 => (short)nuintValue,
                            SpecialType.System_Int32 => (int)nuintValue,
                            SpecialType.System_Int64 => (long)nuintValue,
                            SpecialType.System_IntPtr => (int)nuintValue,
                            SpecialType.System_Single => (double)(float)nuintValue,
                            SpecialType.System_Double => (double)nuintValue,
                            SpecialType.System_Decimal => (decimal)nuintValue,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.SByte:
                        sbyte sbyteValue = value.SByteValue;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)sbyteValue,
                            SpecialType.System_Char => (char)sbyteValue,
                            SpecialType.System_UInt16 => (ushort)sbyteValue,
                            SpecialType.System_UInt32 => (uint)sbyteValue,
                            SpecialType.System_UInt64 => (ulong)sbyteValue,
                            SpecialType.System_SByte => sbyteValue,
                            SpecialType.System_Int16 => (short)sbyteValue,
                            SpecialType.System_Int32 => (int)sbyteValue,
                            SpecialType.System_Int64 => (long)sbyteValue,
                            SpecialType.System_IntPtr => (int)sbyteValue,
                            SpecialType.System_UIntPtr => (uint)sbyteValue,
                            SpecialType.System_Single or SpecialType.System_Double => (double)sbyteValue,
                            SpecialType.System_Decimal => (decimal)sbyteValue,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.Int16:
                        short int16Value = value.Int16Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)int16Value,
                            SpecialType.System_Char => (char)int16Value,
                            SpecialType.System_UInt16 => (ushort)int16Value,
                            SpecialType.System_UInt32 => (uint)int16Value,
                            SpecialType.System_UInt64 => (ulong)int16Value,
                            SpecialType.System_SByte => (sbyte)int16Value,
                            SpecialType.System_Int16 => int16Value,
                            SpecialType.System_Int32 => (int)int16Value,
                            SpecialType.System_Int64 => (long)int16Value,
                            SpecialType.System_IntPtr => (int)int16Value,
                            SpecialType.System_UIntPtr => (uint)int16Value,
                            SpecialType.System_Single or SpecialType.System_Double => (double)int16Value,
                            SpecialType.System_Decimal => (decimal)int16Value,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.Int32:
                        int int32Value = value.Int32Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)int32Value,
                            SpecialType.System_Char => (char)int32Value,
                            SpecialType.System_UInt16 => (ushort)int32Value,
                            SpecialType.System_UInt32 => (uint)int32Value,
                            SpecialType.System_UInt64 => (ulong)int32Value,
                            SpecialType.System_SByte => (sbyte)int32Value,
                            SpecialType.System_Int16 => (short)int32Value,
                            SpecialType.System_Int32 => int32Value,
                            SpecialType.System_Int64 => (long)int32Value,
                            SpecialType.System_IntPtr => int32Value,
                            SpecialType.System_UIntPtr => (uint)int32Value,
                            SpecialType.System_Single => (double)(float)int32Value,
                            SpecialType.System_Double => (double)int32Value,
                            SpecialType.System_Decimal => (decimal)int32Value,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.Int64:
                        long int64Value = value.Int64Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)int64Value,
                            SpecialType.System_Char => (char)int64Value,
                            SpecialType.System_UInt16 => (ushort)int64Value,
                            SpecialType.System_UInt32 => (uint)int64Value,
                            SpecialType.System_UInt64 => (ulong)int64Value,
                            SpecialType.System_SByte => (sbyte)int64Value,
                            SpecialType.System_Int16 => (short)int64Value,
                            SpecialType.System_Int32 => (int)int64Value,
                            SpecialType.System_Int64 => int64Value,
                            SpecialType.System_IntPtr => (int)int64Value,
                            SpecialType.System_UIntPtr => (uint)int64Value,
                            SpecialType.System_Single => (double)(float)int64Value,
                            SpecialType.System_Double => (double)int64Value,
                            SpecialType.System_Decimal => (decimal)int64Value,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.NInt:
                        int nintValue = value.Int32Value;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)nintValue,
                            SpecialType.System_Char => (char)nintValue,
                            SpecialType.System_UInt16 => (ushort)nintValue,
                            SpecialType.System_UInt32 => (uint)nintValue,
                            SpecialType.System_UInt64 => (ulong)nintValue,
                            SpecialType.System_SByte => (sbyte)nintValue,
                            SpecialType.System_Int16 => (short)nintValue,
                            SpecialType.System_Int32 => nintValue,
                            SpecialType.System_Int64 => (long)nintValue,
                            SpecialType.System_IntPtr => nintValue,
                            SpecialType.System_UIntPtr => (uint)nintValue,
                            SpecialType.System_Single => (double)(float)nintValue,
                            SpecialType.System_Double => (double)nintValue,
                            SpecialType.System_Decimal => (decimal)nintValue,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.Single:
                    case ConstantValueTypeDiscriminator.Double:
                        // When converting from a floating-point type to an integral type, if the checked conversion would
                        // throw an overflow exception, then the unchecked conversion is undefined.  So that we have
                        // identical behavior on every host platform, we yield a result of zero in that case.
                        double doubleValue = CheckConstantBounds(destinationType, value.DoubleValue, out _) ? value.DoubleValue : 0D;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)doubleValue,
                            SpecialType.System_Char => (char)doubleValue,
                            SpecialType.System_UInt16 => (ushort)doubleValue,
                            SpecialType.System_UInt32 => (uint)doubleValue,
                            SpecialType.System_UInt64 => (ulong)doubleValue,
                            SpecialType.System_SByte => (sbyte)doubleValue,
                            SpecialType.System_Int16 => (short)doubleValue,
                            SpecialType.System_Int32 => (int)doubleValue,
                            SpecialType.System_Int64 => (long)doubleValue,
                            SpecialType.System_IntPtr => (int)doubleValue,
                            SpecialType.System_UIntPtr => (uint)doubleValue,
                            SpecialType.System_Single => (double)(float)doubleValue,
                            SpecialType.System_Double => (double)doubleValue,
                            SpecialType.System_Decimal => (value.Discriminator == ConstantValueTypeDiscriminator.Single) ? (decimal)(float)doubleValue : (decimal)doubleValue,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    case ConstantValueTypeDiscriminator.Decimal:
                        decimal decimalValue = CheckConstantBounds(destinationType, value.DecimalValue, out _) ? value.DecimalValue : 0m;
                        return destinationType switch
                        {
                            SpecialType.System_Byte => (byte)decimalValue,
                            SpecialType.System_Char => (char)decimalValue,
                            SpecialType.System_UInt16 => (ushort)decimalValue,
                            SpecialType.System_UInt32 => (uint)decimalValue,
                            SpecialType.System_UInt64 => (ulong)decimalValue,
                            SpecialType.System_SByte => (sbyte)decimalValue,
                            SpecialType.System_Int16 => (short)decimalValue,
                            SpecialType.System_Int32 => (int)decimalValue,
                            SpecialType.System_Int64 => (long)decimalValue,
                            SpecialType.System_IntPtr => (int)decimalValue,
                            SpecialType.System_UIntPtr => (uint)decimalValue,
                            SpecialType.System_Single => (double)(float)decimalValue,
                            SpecialType.System_Double => (double)decimalValue,
                            SpecialType.System_Decimal => decimalValue,
                            _ => throw ExceptionUtilities.UnexpectedValue(destinationType),
                        };
                    default:
                        throw ExceptionUtilities.UnexpectedValue(value.Discriminator);
                }
            }

            // all cases should have been handled in the switch above.
            // return value.Value;
        }

        public static bool CheckConstantBounds(SpecialType destinationType, ConstantValue value, out bool maySucceedAtRuntime)
        {
            if (value.IsBad)
            {
                //assume that the constant was intended to be in bounds
                maySucceedAtRuntime = false;
                return true;
            }

            // Compute whether the value fits into the bounds of the given destination type without
            // error. We know that the constant will fit into either a double or a decimal, so
            // convert it to one of those and then check the bounds on that.
            var canonicalValue = CanonicalizeConstant(value);
            return canonicalValue is decimal @decimal ?
                CheckConstantBounds(destinationType, @decimal, out maySucceedAtRuntime) :
                CheckConstantBounds(destinationType, (double)canonicalValue, out maySucceedAtRuntime);
        }

        private static bool CheckConstantBounds(SpecialType destinationType, double value, out bool maySucceedAtRuntime)
        {
            maySucceedAtRuntime = false;

            // Dev10 checks (minValue - 1) < value < (maxValue + 1).
            // See ExpressionBinder::isConstantInRange.
            switch (destinationType)
            {
                case SpecialType.System_Byte: return (byte.MinValue - 1D) < value && value < (byte.MaxValue + 1D);
                case SpecialType.System_Char: return (char.MinValue - 1D) < value && value < (char.MaxValue + 1D);
                case SpecialType.System_UInt16: return (ushort.MinValue - 1D) < value && value < (ushort.MaxValue + 1D);
                case SpecialType.System_UInt32: return (uint.MinValue - 1D) < value && value < (uint.MaxValue + 1D);
                case SpecialType.System_UInt64: return (ulong.MinValue - 1D) < value && value < (ulong.MaxValue + 1D);
                case SpecialType.System_SByte: return (sbyte.MinValue - 1D) < value && value < (sbyte.MaxValue + 1D);
                case SpecialType.System_Int16: return (short.MinValue - 1D) < value && value < (short.MaxValue + 1D);
                case SpecialType.System_Int32: return (int.MinValue - 1D) < value && value < (int.MaxValue + 1D);
                // Note: Using <= to compare the min value matches the native compiler.
                case SpecialType.System_Int64: return (long.MinValue - 1D) <= value && value < (long.MaxValue + 1D);
                case SpecialType.System_Decimal: return ((double)decimal.MinValue - 1D) < value && value < ((double)decimal.MaxValue + 1D);
                case SpecialType.System_IntPtr:
                    maySucceedAtRuntime = (long.MinValue - 1D) < value && value < (long.MaxValue + 1D);
                    return (int.MinValue - 1D) < value && value < (int.MaxValue + 1D);
                case SpecialType.System_UIntPtr:
                    maySucceedAtRuntime = (ulong.MinValue - 1D) < value && value < (ulong.MaxValue + 1D);
                    return (uint.MinValue - 1D) < value && value < (uint.MaxValue + 1D);
            }

            return true;
        }

        private static bool CheckConstantBounds(SpecialType destinationType, decimal value, out bool maySucceedAtRuntime)
        {
            maySucceedAtRuntime = false;

            // Dev10 checks (minValue - 1) < value < (maxValue + 1).
            // See ExpressionBinder::isConstantInRange.
            switch (destinationType)
            {
                case SpecialType.System_Byte: return (byte.MinValue - 1M) < value && value < (byte.MaxValue + 1M);
                case SpecialType.System_Char: return (char.MinValue - 1M) < value && value < (char.MaxValue + 1M);
                case SpecialType.System_UInt16: return (ushort.MinValue - 1M) < value && value < (ushort.MaxValue + 1M);
                case SpecialType.System_UInt32: return (uint.MinValue - 1M) < value && value < (uint.MaxValue + 1M);
                case SpecialType.System_UInt64: return (ulong.MinValue - 1M) < value && value < (ulong.MaxValue + 1M);
                case SpecialType.System_SByte: return (sbyte.MinValue - 1M) < value && value < (sbyte.MaxValue + 1M);
                case SpecialType.System_Int16: return (short.MinValue - 1M) < value && value < (short.MaxValue + 1M);
                case SpecialType.System_Int32: return (int.MinValue - 1M) < value && value < (int.MaxValue + 1M);
                case SpecialType.System_Int64: return (long.MinValue - 1M) < value && value < (long.MaxValue + 1M);
                case SpecialType.System_IntPtr:
                    maySucceedAtRuntime = (long.MinValue - 1M) < value && value < (long.MaxValue + 1M);
                    return (int.MinValue - 1M) < value && value < (int.MaxValue + 1M);
                case SpecialType.System_UIntPtr:
                    maySucceedAtRuntime = (ulong.MinValue - 1M) < value && value < (ulong.MaxValue + 1M);
                    return (uint.MinValue - 1M) < value && value < (uint.MaxValue + 1M);
            }

            return true;
        }

        // Takes in a constant of any kind and returns the constant as either a double or decimal
        private static object CanonicalizeConstant(ConstantValue value)
        {
            return value.Discriminator switch
            {
                ConstantValueTypeDiscriminator.SByte => (decimal)value.SByteValue,
                ConstantValueTypeDiscriminator.Int16 => (decimal)value.Int16Value,
                ConstantValueTypeDiscriminator.Int32 => (decimal)value.Int32Value,
                ConstantValueTypeDiscriminator.Int64 => (decimal)value.Int64Value,
                ConstantValueTypeDiscriminator.NInt => (decimal)value.Int32Value,
                ConstantValueTypeDiscriminator.Byte => (decimal)value.ByteValue,
                ConstantValueTypeDiscriminator.Char => (decimal)value.CharValue,
                ConstantValueTypeDiscriminator.UInt16 => (decimal)value.UInt16Value,
                ConstantValueTypeDiscriminator.UInt32 => (decimal)value.UInt32Value,
                ConstantValueTypeDiscriminator.UInt64 => (decimal)value.UInt64Value,
                ConstantValueTypeDiscriminator.NUInt => (decimal)value.UInt32Value,
                ConstantValueTypeDiscriminator.Single or ConstantValueTypeDiscriminator.Double => value.DoubleValue,
                ConstantValueTypeDiscriminator.Decimal => value.DecimalValue,
                _ => throw ExceptionUtilities.UnexpectedValue(value.Discriminator),
            };

            // all cases handled in the switch, above.
        }
    }
}
