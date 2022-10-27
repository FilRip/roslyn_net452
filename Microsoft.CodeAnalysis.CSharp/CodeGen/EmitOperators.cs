// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Reflection.Metadata;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.CodeGen
{
    internal partial class CodeGenerator
    {
        private void EmitUnaryOperatorExpression(BoundUnaryOperator expression, bool used)
        {
            var operatorKind = expression.OperatorKind;

            if (operatorKind.IsChecked())
            {
                EmitUnaryCheckedOperatorExpression(expression, used);
                return;
            }

            if (!used)
            {
                EmitExpression(expression.Operand, used: false);
                return;
            }

            if (operatorKind == UnaryOperatorKind.BoolLogicalNegation)
            {
                EmitCondExpr(expression.Operand, sense: false);
                return;
            }

            EmitExpression(expression.Operand, used: true);
            switch (operatorKind.Operator())
            {
                case UnaryOperatorKind.UnaryMinus:
                    _builder.EmitOpCode(ILOpCode.Neg);
                    break;

                case UnaryOperatorKind.BitwiseComplement:
                    _builder.EmitOpCode(ILOpCode.Not);
                    break;

                case UnaryOperatorKind.UnaryPlus:
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(operatorKind.Operator());
            }
        }

        private void EmitBinaryOperatorExpression(BoundBinaryOperator expression, bool used)
        {
            var operatorKind = expression.OperatorKind;

            if (operatorKind.EmitsAsCheckedInstruction())
            {
                EmitBinaryOperator(expression);
            }
            else
            {
                // if operator does not have side-effects itself and is not short-circuiting
                // we can simply emit side-effects from the first operand and then from the second one
                if (!used && !operatorKind.IsLogical() && !OperatorHasSideEffects(operatorKind))
                {
                    EmitExpression(expression.Left, false);
                    EmitExpression(expression.Right, false);
                    return;
                }

                if (IsConditional(operatorKind))
                {
                    EmitBinaryCondOperator(expression, true);
                }
                else
                {
                    EmitBinaryOperator(expression);
                }
            }

            EmitPopIfUnused(used);
        }

        private void EmitBinaryOperator(BoundBinaryOperator expression)
        {
            BoundExpression child = expression.Left;

            if (child.Kind != BoundKind.BinaryOperator || child.ConstantValue != null)
            {
                EmitBinaryOperatorSimple(expression);
                return;
            }

            BoundBinaryOperator binary = (BoundBinaryOperator)child;
            var operatorKind = binary.OperatorKind;

            if (!operatorKind.EmitsAsCheckedInstruction() && IsConditional(operatorKind))
            {
                EmitBinaryOperatorSimple(expression);
                return;
            }

            // Do not blow the stack due to a deep recursion on the left.
            var stack = ArrayBuilder<BoundBinaryOperator>.GetInstance();
            stack.Push(expression);

            while (true)
            {
                stack.Push(binary);
                child = binary.Left;

                if (child.Kind != BoundKind.BinaryOperator || child.ConstantValue != null)
                {
                    break;
                }

                binary = (BoundBinaryOperator)child;
                operatorKind = binary.OperatorKind;

                if (!operatorKind.EmitsAsCheckedInstruction() && IsConditional(operatorKind))
                {
                    break;
                }
            }

            EmitExpression(child, true);

            do
            {
                binary = stack.Pop();

                EmitExpression(binary.Right, true);
                bool isChecked = binary.OperatorKind.EmitsAsCheckedInstruction();
                if (isChecked)
                {
                    EmitBinaryCheckedOperatorInstruction(binary);
                }
                else
                {
                    EmitBinaryOperatorInstruction(binary);
                }

                EmitConversionToEnumUnderlyingType(binary, @checked: isChecked);
            }
            while (stack.Count > 0);

            stack.Free();
        }

        private void EmitBinaryOperatorSimple(BoundBinaryOperator expression)
        {
            EmitExpression(expression.Left, true);
            EmitExpression(expression.Right, true);

            bool isChecked = expression.OperatorKind.EmitsAsCheckedInstruction();
            if (isChecked)
            {
                EmitBinaryCheckedOperatorInstruction(expression);
            }
            else
            {
                EmitBinaryOperatorInstruction(expression);
            }

            EmitConversionToEnumUnderlyingType(expression, @checked: isChecked);
        }

        private void EmitBinaryOperatorInstruction(BoundBinaryOperator expression)
        {
            switch (expression.OperatorKind.Operator())
            {
                case BinaryOperatorKind.Multiplication:
                    _builder.EmitOpCode(ILOpCode.Mul);
                    break;

                case BinaryOperatorKind.Addition:
                    _builder.EmitOpCode(ILOpCode.Add);
                    break;

                case BinaryOperatorKind.Subtraction:
                    _builder.EmitOpCode(ILOpCode.Sub);
                    break;

                case BinaryOperatorKind.Division:
                    if (IsUnsignedBinaryOperator(expression))
                    {
                        _builder.EmitOpCode(ILOpCode.Div_un);
                    }
                    else
                    {
                        _builder.EmitOpCode(ILOpCode.Div);
                    }
                    break;

                case BinaryOperatorKind.Remainder:
                    if (IsUnsignedBinaryOperator(expression))
                    {
                        _builder.EmitOpCode(ILOpCode.Rem_un);
                    }
                    else
                    {
                        _builder.EmitOpCode(ILOpCode.Rem);
                    }
                    break;

                case BinaryOperatorKind.LeftShift:
                    _builder.EmitOpCode(ILOpCode.Shl);
                    break;

                case BinaryOperatorKind.RightShift:
                    if (IsUnsignedBinaryOperator(expression))
                    {
                        _builder.EmitOpCode(ILOpCode.Shr_un);
                    }
                    else
                    {
                        _builder.EmitOpCode(ILOpCode.Shr);
                    }
                    break;

                case BinaryOperatorKind.And:
                    _builder.EmitOpCode(ILOpCode.And);
                    break;

                case BinaryOperatorKind.Xor:
                    _builder.EmitOpCode(ILOpCode.Xor);
                    break;

                case BinaryOperatorKind.Or:
                    _builder.EmitOpCode(ILOpCode.Or);
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(expression.OperatorKind.Operator());
            }
        }

        private void EmitShortCircuitingOperator(BoundBinaryOperator condition, bool sense, bool stopSense, bool stopValue)
        {
            // we generate:
            //
            // gotoif (a == stopSense) fallThrough
            // b == sense
            // goto labEnd
            // fallThrough:
            // stopValue
            // labEnd:
            //                 AND       OR
            //            +-  ------    -----
            // stopSense  |   !sense    sense
            // stopValue  |     0         1

            object lazyFallThrough = null;

            EmitCondBranch(condition.Left, ref lazyFallThrough, stopSense);
            EmitCondExpr(condition.Right, sense);

            // if fall-through was not initialized, no-one is going to take that branch
            // and we are done with Right on stack
            if (lazyFallThrough == null)
            {
                return;
            }

            var labEnd = new object();
            _builder.EmitBranch(ILOpCode.Br, labEnd);

            // if we get to fallThrough, we should not have Right on stack. Adjust for that.
            _builder.AdjustStack(-1);

            _builder.MarkLabel(lazyFallThrough);
            _builder.EmitBoolConstant(stopValue);
            _builder.MarkLabel(labEnd);
        }

        //NOTE: odd positions assume inverted sense
        private static readonly ILOpCode[] s_compOpCodes = new ILOpCode[]
        {
            //  <            <=               >                >=
            ILOpCode.Clt,    ILOpCode.Cgt,    ILOpCode.Cgt,    ILOpCode.Clt,     // Signed
            ILOpCode.Clt_un, ILOpCode.Cgt_un, ILOpCode.Cgt_un, ILOpCode.Clt_un,  // Unsigned
            ILOpCode.Clt,    ILOpCode.Cgt_un, ILOpCode.Cgt,    ILOpCode.Clt_un,  // Float
        };

        //NOTE: The result of this should be a boolean on the stack.
        private void EmitBinaryCondOperator(BoundBinaryOperator binOp, bool sense)
        {
            bool andOrSense = sense;
            int opIdx;

            switch (binOp.OperatorKind.OperatorWithLogical())
            {
                case BinaryOperatorKind.LogicalOr:

                    // Rewrite (a || b) as ~(~a && ~b)
                    andOrSense = !andOrSense;
                    // Fall through
                    goto case BinaryOperatorKind.LogicalAnd;

                case BinaryOperatorKind.LogicalAnd:

                    // ~(a && b) is equivalent to (~a || ~b)
                    if (!andOrSense)
                    {
                        // generate (~a || ~b)
                        EmitShortCircuitingOperator(binOp, sense, sense, true);
                    }
                    else
                    {
                        // generate (a && b)
                        EmitShortCircuitingOperator(binOp, sense, !sense, false);
                    }
                    return;

                case BinaryOperatorKind.And:
                    EmitBinaryCondOperatorHelper(ILOpCode.And, binOp.Left, binOp.Right, sense);
                    return;

                case BinaryOperatorKind.Or:
                    EmitBinaryCondOperatorHelper(ILOpCode.Or, binOp.Left, binOp.Right, sense);
                    return;

                case BinaryOperatorKind.Xor:

                    // Xor is equivalent to not equal.
                    if (sense)
                        EmitBinaryCondOperatorHelper(ILOpCode.Xor, binOp.Left, binOp.Right, true);
                    else
                        EmitBinaryCondOperatorHelper(ILOpCode.Ceq, binOp.Left, binOp.Right, true);
                    return;

                case BinaryOperatorKind.NotEqual:
                    // neq  is emitted as  !eq
                    sense = !sense;
                    goto case BinaryOperatorKind.Equal;

                case BinaryOperatorKind.Equal:

                    var constant = binOp.Left.ConstantValue;
                    var comparand = binOp.Right;

                    if (constant == null)
                    {
                        constant = comparand.ConstantValue;
                        comparand = binOp.Left;
                    }

                    if (constant != null)
                    {
                        if (constant.IsDefaultValue)
                        {
                            if (!constant.IsFloating)
                            {
                                if (sense)
                                {
                                    EmitIsNullOrZero(comparand, constant);
                                }
                                else
                                {
                                    //  obj != null/0   for pointers and integral numerics is emitted as cgt.un
                                    EmitIsNotNullOrZero(comparand, constant);
                                }
                                return;
                            }
                        }
                        else if (constant.IsBoolean)
                        {
                            // treat  "x = True" ==> "x"
                            EmitExpression(comparand, true);
                            EmitIsSense(sense);
                            return;
                        }
                    }

                    EmitBinaryCondOperatorHelper(ILOpCode.Ceq, binOp.Left, binOp.Right, sense);
                    return;

                case BinaryOperatorKind.LessThan:
                    opIdx = 0;
                    break;

                case BinaryOperatorKind.LessThanOrEqual:
                    opIdx = 1;
                    sense = !sense; // lte is emitted as !gt 
                    break;

                case BinaryOperatorKind.GreaterThan:
                    opIdx = 2;
                    break;

                case BinaryOperatorKind.GreaterThanOrEqual:
                    opIdx = 3;
                    sense = !sense; // gte is emitted as !lt 
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(binOp.OperatorKind.OperatorWithLogical());
            }

            if (IsUnsignedBinaryOperator(binOp))
            {
                opIdx += 4;
            }
            else if (IsFloat(binOp.OperatorKind))
            {
                opIdx += 8;
            }

            EmitBinaryCondOperatorHelper(s_compOpCodes[opIdx], binOp.Left, binOp.Right, sense);
            return;
        }

        private void EmitIsNotNullOrZero(BoundExpression comparand, ConstantValue nullOrZero)
        {
            EmitExpression(comparand, true);

            var comparandType = comparand.Type;
            if (comparandType.IsReferenceType && !comparandType.IsVerifierReference())
            {
                EmitBox(comparandType, comparand.Syntax);
            }

            _builder.EmitConstantValue(nullOrZero);
            _builder.EmitOpCode(ILOpCode.Cgt_un);
        }

        private void EmitIsNullOrZero(BoundExpression comparand, ConstantValue nullOrZero)
        {
            EmitExpression(comparand, true);

            var comparandType = comparand.Type;
            if (comparandType.IsReferenceType && !comparandType.IsVerifierReference())
            {
                EmitBox(comparandType, comparand.Syntax);
            }

            _builder.EmitConstantValue(nullOrZero);
            _builder.EmitOpCode(ILOpCode.Ceq);
        }

        private void EmitBinaryCondOperatorHelper(ILOpCode opCode, BoundExpression left, BoundExpression right, bool sense)
        {
            EmitExpression(left, true);
            EmitExpression(right, true);
            _builder.EmitOpCode(opCode);
            EmitIsSense(sense);
        }

        // generate a conditional (ie, boolean) expression...
        // this will leave a value on the stack which conforms to sense, ie:(condition == sense)
        private void EmitCondExpr(BoundExpression condition, bool sense)
        {
            while (condition.Kind == BoundKind.UnaryOperator)
            {
                var unOp = (BoundUnaryOperator)condition;
                condition = unOp.Operand;
                sense = !sense;
            }


            var constantValue = condition.ConstantValue;
            if (constantValue != null)
            {
                var constant = constantValue.BooleanValue;
                _builder.EmitBoolConstant(constant == sense);
                return;
            }

            if (condition.Kind == BoundKind.BinaryOperator)
            {
                var binOp = (BoundBinaryOperator)condition;
                if (IsConditional(binOp.OperatorKind))
                {
                    EmitBinaryCondOperator(binOp, sense);
                    return;
                }
            }

            EmitExpression(condition, true);
            EmitIsSense(sense);

            return;
        }

        private void EmitUnaryCheckedOperatorExpression(BoundUnaryOperator expression, bool used)
        {
            var type = expression.OperatorKind.OperandTypes();

            // Spec 7.6.2
            // Implementation of unary minus has two overloads:
            //   int operator –(int x)
            //   long operator –(long x)
            // 
            // The result is computed by subtracting x from zero. 
            // If the value of x is the smallest representable value of the operand type (−2^31 for int or −2^63 for long),
            // then the mathematical negation of x is not representable within the operand type. If this occurs within a checked context, 
            // a System.OverflowException is thrown; if it occurs within an unchecked context, 
            // the result is the value of the operand and the overflow is not reported.

            // ldc.i4.0
            // conv.i8  (when the operand is 64bit)
            // <expr>
            // sub.ovf

            _builder.EmitOpCode(ILOpCode.Ldc_i4_0);

            if (type == UnaryOperatorKind.Long)
            {
                _builder.EmitOpCode(ILOpCode.Conv_i8);
            }

            EmitExpression(expression.Operand, used: true);
            _builder.EmitOpCode(ILOpCode.Sub_ovf);

            EmitPopIfUnused(used);
        }

        private void EmitConversionToEnumUnderlyingType(BoundBinaryOperator expression, bool @checked)
        {
            TypeSymbol enumType = (expression.OperatorKind.Operator() | expression.OperatorKind.OperandTypes()) switch
            {
                BinaryOperatorKind.EnumAndUnderlyingAddition or BinaryOperatorKind.EnumSubtraction or BinaryOperatorKind.EnumAndUnderlyingSubtraction => expression.Left.Type,
                BinaryOperatorKind.EnumAnd or BinaryOperatorKind.EnumOr or BinaryOperatorKind.EnumXor => null,
                BinaryOperatorKind.UnderlyingAndEnumSubtraction or BinaryOperatorKind.UnderlyingAndEnumAddition => expression.Right.Type,
                _ => null,
            };
            if (enumType is null)
            {
                return;
            }


            SpecialType type = enumType.GetEnumUnderlyingType().SpecialType;
            switch (type)
            {
                case SpecialType.System_Byte:
                    _builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.Int32, Microsoft.Cci.PrimitiveTypeCode.UInt8, @checked);
                    break;
                case SpecialType.System_SByte:
                    _builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.Int32, Microsoft.Cci.PrimitiveTypeCode.Int8, @checked);
                    break;
                case SpecialType.System_Int16:
                    _builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.Int32, Microsoft.Cci.PrimitiveTypeCode.Int16, @checked);
                    break;
                case SpecialType.System_UInt16:
                    _builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.Int32, Microsoft.Cci.PrimitiveTypeCode.UInt16, @checked);
                    break;
            }
        }

        private void EmitBinaryCheckedOperatorInstruction(BoundBinaryOperator expression)
        {
            var unsigned = IsUnsignedBinaryOperator(expression);

            switch (expression.OperatorKind.Operator())
            {
                case BinaryOperatorKind.Multiplication:
                    if (unsigned)
                    {
                        _builder.EmitOpCode(ILOpCode.Mul_ovf_un);
                    }
                    else
                    {
                        _builder.EmitOpCode(ILOpCode.Mul_ovf);
                    }
                    break;

                case BinaryOperatorKind.Addition:
                    if (unsigned)
                    {
                        _builder.EmitOpCode(ILOpCode.Add_ovf_un);
                    }
                    else
                    {
                        _builder.EmitOpCode(ILOpCode.Add_ovf);
                    }
                    break;

                case BinaryOperatorKind.Subtraction:
                    if (unsigned)
                    {
                        _builder.EmitOpCode(ILOpCode.Sub_ovf_un);
                    }
                    else
                    {
                        _builder.EmitOpCode(ILOpCode.Sub_ovf);
                    }
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(expression.OperatorKind.Operator());
            }
        }

        private static bool OperatorHasSideEffects(BinaryOperatorKind kind)
        {
            return kind.Operator() switch
            {
                BinaryOperatorKind.Division or BinaryOperatorKind.Remainder => true,
                _ => kind.IsChecked(),
            };
        }

        // emits IsTrue/IsFalse according to the sense
        // IsTrue actually does nothing
        private void EmitIsSense(bool sense)
        {
            if (!sense)
            {
                _builder.EmitOpCode(ILOpCode.Ldc_i4_0);
                _builder.EmitOpCode(ILOpCode.Ceq);
            }
        }

        private static bool IsUnsigned(SpecialType type)
        {
            return type switch
            {
                SpecialType.System_Byte or SpecialType.System_UInt16 or SpecialType.System_UInt32 or SpecialType.System_UInt64 => true,
                _ => false,
            };
        }

        private static bool IsUnsignedBinaryOperator(BoundBinaryOperator op)
        {
            BinaryOperatorKind opKind = op.OperatorKind;
            BinaryOperatorKind type = opKind.OperandTypes();
            return type switch
            {
                BinaryOperatorKind.Enum or BinaryOperatorKind.EnumAndUnderlying => IsUnsigned(Binder.GetEnumPromotedType(op.Left.Type.GetEnumUnderlyingType().SpecialType)),
                BinaryOperatorKind.UnderlyingAndEnum => IsUnsigned(Binder.GetEnumPromotedType(op.Right.Type.GetEnumUnderlyingType().SpecialType)),
                BinaryOperatorKind.UInt or BinaryOperatorKind.NUInt or BinaryOperatorKind.ULong or BinaryOperatorKind.ULongAndPointer or BinaryOperatorKind.PointerAndInt or BinaryOperatorKind.PointerAndUInt or BinaryOperatorKind.PointerAndLong or BinaryOperatorKind.PointerAndULong or BinaryOperatorKind.Pointer => true,
                // Dev10 bases signedness on the first operand (see ILGENREC::genOperatorExpr).
                _ => false,
            };
        }

        private static bool IsConditional(BinaryOperatorKind opKind)
        {
            return opKind.OperatorWithLogical() switch
            {
                BinaryOperatorKind.LogicalAnd or BinaryOperatorKind.LogicalOr or BinaryOperatorKind.Equal or BinaryOperatorKind.NotEqual or BinaryOperatorKind.LessThan or BinaryOperatorKind.LessThanOrEqual or BinaryOperatorKind.GreaterThan or BinaryOperatorKind.GreaterThanOrEqual => true,
                BinaryOperatorKind.And or BinaryOperatorKind.Or or BinaryOperatorKind.Xor => opKind.OperandTypes() == BinaryOperatorKind.Bool,
                _ => false,
            };
        }

        private static bool IsFloat(BinaryOperatorKind opKind)
        {
            var type = opKind.OperandTypes();
            return type switch
            {
                BinaryOperatorKind.Float or BinaryOperatorKind.Double => true,
                _ => false,
            };
        }
    }
}
