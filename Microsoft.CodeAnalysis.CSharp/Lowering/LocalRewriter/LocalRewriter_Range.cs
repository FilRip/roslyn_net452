// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitRangeExpression(BoundRangeExpression node)
        {

            bool needLifting = false;
            var F = _factory;

            var left = node.LeftOperandOpt;
            if (left != null)
            {
                left = tryOptimizeOperand(left);
            }

            var right = node.RightOperandOpt;
            if (right != null)
            {
                right = tryOptimizeOperand(right);
            }

            if (needLifting)
            {
                return LiftRangeExpression(node, left, right);
            }
            else
            {
#nullable restore
                BoundExpression rangeCreation = MakeRangeExpression(node.MethodOpt, left, right);

                if (node.Type.IsNullableType())
                {
                    if (!TryGetNullableMethod(node.Syntax, node.Type, SpecialMember.System_Nullable_T__ctor, out MethodSymbol nullableCtor))
                    {
                        return BadExpression(node.Syntax, node.Type, node);
                    }

                    return new BoundObjectCreationExpression(node.Syntax, nullableCtor, rangeCreation);
                }

                return rangeCreation;
            }

            BoundExpression tryOptimizeOperand(BoundExpression operand)
            {
                operand = VisitExpression(operand);

                if (NullableNeverHasValue(operand))
                {
                    operand = new BoundDefaultExpression(operand.Syntax, operand.Type.GetNullableUnderlyingType());
                }
                else
                {
                    operand = NullableAlwaysHasValue(operand) ?? operand;

                    if (operand.Type.IsNullableType())
                    {
                        needLifting = true;
                    }
                }

                return operand;
            }
        }

#nullable enable

        private BoundExpression LiftRangeExpression(BoundRangeExpression node, BoundExpression? left, BoundExpression? right)
        {

            var sideeffects = ArrayBuilder<BoundExpression>.GetInstance();
            var locals = ArrayBuilder<LocalSymbol>.GetInstance();

            // makeRange(left.GetValueOrDefault(), right.GetValueOrDefault())
            BoundExpression? condition = null;
            left = getIndexFromPossibleNullable(left);
            right = getIndexFromPossibleNullable(right);
#nullable restore
            var rangeExpr = MakeRangeExpression(node.MethodOpt, left, right);

            if (!TryGetNullableMethod(node.Syntax, node.Type, SpecialMember.System_Nullable_T__ctor, out MethodSymbol nullableCtor))
            {
                return BadExpression(node.Syntax, node.Type, node);
            }

            // new Nullable(makeRange(left.GetValueOrDefault(), right.GetValueOrDefault()))
            BoundExpression consequence = new BoundObjectCreationExpression(node.Syntax, nullableCtor, rangeExpr);

            // default
            BoundExpression alternative = new BoundDefaultExpression(node.Syntax, node.Type);

            // left.HasValue && right.HasValue
            //     ? new Nullable(makeRange(left.GetValueOrDefault(), right.GetValueOrDefault()))
            //     : default
            BoundExpression conditionalExpression = RewriteConditionalOperator(
                syntax: node.Syntax,
                rewrittenCondition: condition,
                rewrittenConsequence: consequence,
                rewrittenAlternative: alternative,
                constantValueOpt: null,
                rewrittenType: node.Type,
                isRef: false);

            return new BoundSequence(
                syntax: node.Syntax,
                locals: locals.ToImmutableAndFree(),
                sideEffects: sideeffects.ToImmutableAndFree(),
                value: conditionalExpression,
                type: node.Type);

#nullable enable

            BoundExpression? getIndexFromPossibleNullable(BoundExpression? arg)
            {
                if (arg is null)
                    return null;

                BoundExpression tempOperand = CaptureExpressionInTempIfNeeded(arg, sideeffects, locals);

#nullable restore
                if (tempOperand.Type.IsNullableType())
                {
                    BoundExpression operandHasValue = MakeOptimizedHasValue(tempOperand.Syntax, tempOperand);

                    if (condition is null)
                    {
                        condition = operandHasValue;
                    }
                    else
                    {
                        TypeSymbol boolType = _compilation.GetSpecialType(SpecialType.System_Boolean);
                        condition = MakeBinaryOperator(node.Syntax, BinaryOperatorKind.BoolAnd, condition, operandHasValue, boolType, method: null);
                    }

                    return MakeOptimizedGetValueOrDefault(tempOperand.Syntax, tempOperand);
                }
                else
                {
                    return tempOperand;
                }
            }
        }

#nullable enable

        private BoundExpression MakeRangeExpression(
            MethodSymbol constructionMethod,
            BoundExpression? left,
            BoundExpression? right)
        {
            var F = _factory;
            // The construction method may vary based on what well-known
            // members were available during binding. Depending on which member
            // is chosen we need to change our adjust our calling node.
            switch (constructionMethod.MethodKind)
            {
                case MethodKind.Constructor:
                    // Represents Range..ctor(Index left, Index right)
                    // The constructor can always be used to construct a range,
                    // but if any of the arguments are missing then we need to
                    // construct replacement Indexes
                    left ??= newIndexZero(fromEnd: false);
                    right ??= newIndexZero(fromEnd: true);

                    return F.New(constructionMethod, ImmutableArray.Create(left, right));

                case MethodKind.Ordinary:
                    // Represents either Range.StartAt or Range.EndAt, which
                    // means that the `..` expression is missing an argument on
                    // either the left or the right (i.e., `x..` or `..x`)
                    var arg = left ?? right;
#nullable restore
                    return F.StaticCall(constructionMethod, ImmutableArray.Create(arg));

                case MethodKind.PropertyGet:
                    // The only property is Range.All, so the expression must
                    // be `..` with both arguments missing
                    return F.StaticCall(constructionMethod, ImmutableArray<BoundExpression>.Empty);

                default:
                    throw ExceptionUtilities.UnexpectedValue(constructionMethod.MethodKind);
            }

            BoundExpression newIndexZero(bool fromEnd) =>
                // new Index(0, fromEnd: fromEnd)
                F.New(
                    WellKnownMember.System_Index__ctor,
                    ImmutableArray.Create<BoundExpression>(F.Literal(0), F.Literal(fromEnd)));
        }
    }
}
