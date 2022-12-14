// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            BoundExpression rewrittenLeft = VisitExpression(node.LeftOperand);
            BoundExpression rewrittenRight = VisitExpression(node.RightOperand);
            TypeSymbol? rewrittenResultType = VisitType(node.Type);

            return MakeNullCoalescingOperator(node.Syntax, rewrittenLeft, rewrittenRight, node.LeftConversion, node.OperatorResultKind, rewrittenResultType);
        }

        private BoundExpression MakeNullCoalescingOperator(
            SyntaxNode syntax,
            BoundExpression rewrittenLeft,
            BoundExpression rewrittenRight,
            Conversion leftConversion,
            BoundNullCoalescingOperatorResultKind resultKind,
            TypeSymbol? rewrittenResultType)
        {

            if (_inExpressionLambda)
            {
                // Because of Error CS0845 (An expression tree lambda may not contain a coalescing operator with a null or default literal left-hand side)
                // we know that the left-hand-side has a type.
#nullable restore
                TypeSymbol strippedLeftType = rewrittenLeft.Type.StrippedType();
                Conversion rewrittenConversion = TryMakeConversion(syntax, leftConversion, strippedLeftType, rewrittenResultType);
                if (!rewrittenConversion.Exists)
                {
                    return BadExpression(syntax, rewrittenResultType, rewrittenLeft, rewrittenRight);
                }

                return new BoundNullCoalescingOperator(syntax, rewrittenLeft, rewrittenRight, rewrittenConversion, resultKind, rewrittenResultType);
            }

            var isUnconstrainedTypeParameter = rewrittenLeft.Type is { IsReferenceType: false, IsValueType: false };

            // first we can make a small optimization:
            // If left is a constant then we already know whether it is null or not. If it is null then we
            // can simply generate "right". If it is not null then we can simply generate
            // MakeConversion(left). This does not hold when the left is an unconstrained type parameter: at runtime,
            // it can be either left or right depending on the runtime type of T
            if (!isUnconstrainedTypeParameter)
            {
                if (rewrittenLeft.IsDefaultValue())
                {
                    return rewrittenRight;
                }

                if (rewrittenLeft.ConstantValue != null)
                {

                    return GetConvertedLeftForNullCoalescingOperator(rewrittenLeft, leftConversion, rewrittenResultType);
                }
            }

            // string concatenation is never null.
            // interpolated string lowering may introduce redundant null coalescing, which we have to remove.
            if (IsStringConcat(rewrittenLeft))
            {
                return GetConvertedLeftForNullCoalescingOperator(rewrittenLeft, leftConversion, rewrittenResultType);
            }

            // if left conversion is intrinsic implicit (always succeeds) and results in a reference type
            // we can apply conversion before doing the null check that allows for a more efficient IL emit.
            if (rewrittenLeft.Type.IsReferenceType &&
                leftConversion.IsImplicit &&
                !leftConversion.IsUserDefined)
            {
                if (!leftConversion.IsIdentity)
                {
                    rewrittenLeft = MakeConversionNode(rewrittenLeft.Syntax, rewrittenLeft, leftConversion, rewrittenResultType, @checked: false);
                }
                return new BoundNullCoalescingOperator(syntax, rewrittenLeft, rewrittenRight, Conversion.Identity, resultKind, rewrittenResultType);
            }

            if (leftConversion.IsIdentity || leftConversion.Kind == ConversionKind.ExplicitNullable)
            {
                if (rewrittenLeft is BoundLoweredConditionalAccess conditionalAccess &&
                    (conditionalAccess.WhenNullOpt == null || NullableNeverHasValue(conditionalAccess.WhenNullOpt)))
                {
                    var notNullAccess = NullableAlwaysHasValue(conditionalAccess.WhenNotNull);
                    if (notNullAccess != null)
                    {
#nullable enable
                        BoundExpression? whenNullOpt = rewrittenRight;

#nullable restore
                        if (whenNullOpt.Type.IsNullableType())
                        {
                            notNullAccess = conditionalAccess.WhenNotNull;
                        }

                        if (whenNullOpt.IsDefaultValue() && whenNullOpt.Type.SpecialType != SpecialType.System_Decimal)
                        {
                            whenNullOpt = null;
                        }

                        return conditionalAccess.Update(
                            conditionalAccess.Receiver,
                            conditionalAccess.HasValueMethodOpt,
                            whenNotNull: notNullAccess,
                            whenNullOpt: whenNullOpt,
                            id: conditionalAccess.Id,
                            type: rewrittenResultType
                        );
                    }
                }
            }

            // Optimize left ?? right to left.GetValueOrDefault() when left is T? and right is the default value of T
            if (rewrittenLeft.Type.IsNullableType()
                && RemoveIdentityConversions(rewrittenRight).IsDefaultValue()
                && rewrittenRight.Type.Equals(rewrittenLeft.Type.GetNullableUnderlyingType(), TypeCompareKind.AllIgnoreOptions)
                && TryGetNullableMethod(rewrittenLeft.Syntax, rewrittenLeft.Type, SpecialMember.System_Nullable_T_GetValueOrDefault, out MethodSymbol getValueOrDefault))
            {
                return BoundCall.Synthesized(rewrittenLeft.Syntax, rewrittenLeft, getValueOrDefault);
            }

            // We lower left ?? right to
            //
            // var temp = left;
            // (temp != null) ? MakeConversion(temp) : right
            //

            BoundLocal boundTemp = _factory.StoreToTemp(rewrittenLeft, out BoundAssignmentOperator tempAssignment);

            // temp != null
            BoundExpression nullCheck = MakeNullCheck(syntax, boundTemp, BinaryOperatorKind.NotEqual);

            // MakeConversion(temp, rewrittenResultType)
            BoundExpression convertedLeft = GetConvertedLeftForNullCoalescingOperator(boundTemp, leftConversion, rewrittenResultType);

            // (temp != null) ? MakeConversion(temp, LeftConversion) : RightOperand
            BoundExpression conditionalExpression = RewriteConditionalOperator(
                syntax: syntax,
                rewrittenCondition: nullCheck,
                rewrittenConsequence: convertedLeft,
                rewrittenAlternative: rewrittenRight,
                constantValueOpt: null,
                rewrittenType: rewrittenResultType,
                isRef: false);


            return new BoundSequence(
                syntax: syntax,
                locals: ImmutableArray.Create(boundTemp.LocalSymbol),
                sideEffects: ImmutableArray.Create<BoundExpression>(tempAssignment),
                value: conditionalExpression,
                type: rewrittenResultType);
        }

        private bool IsStringConcat(BoundExpression expression)
        {
            if (expression.Kind != BoundKind.Call)
            {
                return false;
            }

            var boundCall = (BoundCall)expression;

            var method = boundCall.Method;

            if (method.IsStatic && method.ContainingType.SpecialType == SpecialType.System_String)
            {
                if (method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringString) ||
                    method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringString) ||
                    method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringStringString) ||
                    method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObject) ||
                    method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObjectObject) ||
                    method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObjectObjectObject) ||
                    method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringArray) ||
                    method == (object)_compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatObjectArray))
                {
                    return true;
                }
            }

            return false;
        }

        private static BoundExpression RemoveIdentityConversions(BoundExpression expression)
        {
            while (expression.Kind == BoundKind.Conversion)
            {
                var boundConversion = (BoundConversion)expression;
                if (boundConversion.ConversionKind != ConversionKind.Identity)
                {
                    return expression;
                }

                expression = boundConversion.Operand;
            }

            return expression;
        }

        private BoundExpression GetConvertedLeftForNullCoalescingOperator(BoundExpression rewrittenLeft, Conversion leftConversion, TypeSymbol rewrittenResultType)
        {

            TypeSymbol rewrittenLeftType = rewrittenLeft.Type;

            // Native compiler violates the specification for the case where result type is right operand type and left operand is nullable.
            // For this case, we need to insert an extra explicit nullable conversion from the left operand to its underlying nullable type
            // before performing the leftConversion.
            // See comments in Binder.BindNullCoalescingOperator referring to GetConvertedLeftForNullCoalescingOperator for more details.

            if (!TypeSymbol.Equals(rewrittenLeftType, rewrittenResultType, TypeCompareKind.ConsiderEverything2) && rewrittenLeftType.IsNullableType())
            {
                TypeSymbol strippedLeftType = rewrittenLeftType.GetNullableUnderlyingType();
                MethodSymbol getValueOrDefault = UnsafeGetNullableMethod(rewrittenLeft.Syntax, rewrittenLeftType, SpecialMember.System_Nullable_T_GetValueOrDefault);
                rewrittenLeft = BoundCall.Synthesized(rewrittenLeft.Syntax, rewrittenLeft, getValueOrDefault);
                if (TypeSymbol.Equals(strippedLeftType, rewrittenResultType, TypeCompareKind.ConsiderEverything2))
                {
                    return rewrittenLeft;
                }
            }

            return MakeConversionNode(rewrittenLeft.Syntax, rewrittenLeft, leftConversion, rewrittenResultType, @checked: false);
        }
    }
}
