// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection.Metadata;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CodeGen
{
    public static partial class ILOpCodeExtensions
    {
        public static int Size(this ILOpCode opcode)
        {
            int code = (int)opcode;
            if (code <= 0xff)
            {
                Debug.Assert(code < 0xf0);
                return 1;
            }
            else
            {
                Debug.Assert((code & 0xff00) == 0xfe00);
                return 2;
            }
        }

        public static ILOpCode GetLeaveOpcode(this ILOpCode opcode)
        {
            return opcode switch
            {
                ILOpCode.Br => ILOpCode.Leave,
                ILOpCode.Br_s => ILOpCode.Leave_s,
                _ => throw ExceptionUtilities.UnexpectedValue(opcode),
            };
        }

        public static bool HasVariableStackBehavior(this ILOpCode opcode)
        {
            return opcode switch
            {
                ILOpCode.Call or ILOpCode.Calli or ILOpCode.Callvirt or ILOpCode.Newobj or ILOpCode.Ret => true,
                _ => false,
            };
        }

        /// <summary>
        /// These opcodes represent control transfer.
        /// They should not appear inside basic blocks.
        /// </summary>
        public static bool IsControlTransfer(this ILOpCode opcode)
        {
            if (opcode.IsBranch())
            {
                return true;
            }

            return opcode switch
            {
                ILOpCode.Ret or ILOpCode.Throw or ILOpCode.Rethrow or ILOpCode.Endfilter or ILOpCode.Endfinally or ILOpCode.Switch or ILOpCode.Jmp => true,
                _ => false,
            };
        }

        public static bool IsConditionalBranch(this ILOpCode opcode)
        {
            return opcode switch
            {
                ILOpCode.Brtrue or ILOpCode.Brtrue_s or ILOpCode.Brfalse or ILOpCode.Brfalse_s or ILOpCode.Beq or ILOpCode.Beq_s or ILOpCode.Bne_un or ILOpCode.Bne_un_s or ILOpCode.Bge or ILOpCode.Bge_s or ILOpCode.Bge_un or ILOpCode.Bge_un_s or ILOpCode.Bgt or ILOpCode.Bgt_s or ILOpCode.Bgt_un or ILOpCode.Bgt_un_s or ILOpCode.Ble or ILOpCode.Ble_s or ILOpCode.Ble_un or ILOpCode.Ble_un_s or ILOpCode.Blt or ILOpCode.Blt_s or ILOpCode.Blt_un or ILOpCode.Blt_un_s => true,
                _ => false,
            };
        }

        public static bool IsRelationalBranch(this ILOpCode opcode)
        {
            return opcode switch
            {
                ILOpCode.Beq or ILOpCode.Beq_s or ILOpCode.Bne_un or ILOpCode.Bne_un_s or ILOpCode.Bge or ILOpCode.Bge_s or ILOpCode.Bge_un or ILOpCode.Bge_un_s or ILOpCode.Bgt or ILOpCode.Bgt_s or ILOpCode.Bgt_un or ILOpCode.Bgt_un_s or ILOpCode.Ble or ILOpCode.Ble_s or ILOpCode.Ble_un or ILOpCode.Ble_un_s or ILOpCode.Blt or ILOpCode.Blt_s or ILOpCode.Blt_un or ILOpCode.Blt_un_s => true,
                _ => false,
            };
        }

        /// <summary>
        /// Opcodes that represents a branch to a label.
        /// </summary>
        public static bool CanFallThrough(this ILOpCode opcode)
        {
            //12.4.2.8.1  Most instructions can allow control to fall through after their execution—only unconditional branches,
            //ret, jmp, leave(.s), endfinally, endfault, endfilter, throw, and rethrow do not.
            return opcode switch
            {
                ILOpCode.Br or ILOpCode.Br_s or ILOpCode.Ret or ILOpCode.Jmp or ILOpCode.Throw or ILOpCode.Endfinally or ILOpCode.Leave or ILOpCode.Leave_s or ILOpCode.Rethrow => false,
                _ => true,
            };
        }

        public static int NetStackBehavior(this ILOpCode opcode)
        {
            Debug.Assert(!opcode.HasVariableStackBehavior());
            return opcode.StackPushCount() - opcode.StackPopCount();
        }

        public static int StackPopCount(this ILOpCode opcode)
        {
            return opcode switch
            {
                ILOpCode.Nop or ILOpCode.Break or ILOpCode.Ldarg_0 or ILOpCode.Ldarg_1 or ILOpCode.Ldarg_2 or ILOpCode.Ldarg_3 or ILOpCode.Ldloc_0 or ILOpCode.Ldloc_1 or ILOpCode.Ldloc_2 or ILOpCode.Ldloc_3 => 0,
                ILOpCode.Stloc_0 or ILOpCode.Stloc_1 or ILOpCode.Stloc_2 or ILOpCode.Stloc_3 => 1,
                ILOpCode.Ldarg_s or ILOpCode.Ldarga_s => 0,
                ILOpCode.Starg_s => 1,
                ILOpCode.Ldloc_s or ILOpCode.Ldloca_s => 0,
                ILOpCode.Stloc_s => 1,
                ILOpCode.Ldnull or ILOpCode.Ldc_i4_m1 or ILOpCode.Ldc_i4_0 or ILOpCode.Ldc_i4_1 or ILOpCode.Ldc_i4_2 or ILOpCode.Ldc_i4_3 or ILOpCode.Ldc_i4_4 or ILOpCode.Ldc_i4_5 or ILOpCode.Ldc_i4_6 or ILOpCode.Ldc_i4_7 or ILOpCode.Ldc_i4_8 or ILOpCode.Ldc_i4_s or ILOpCode.Ldc_i4 or ILOpCode.Ldc_i8 or ILOpCode.Ldc_r4 or ILOpCode.Ldc_r8 => 0,
                ILOpCode.Dup or ILOpCode.Pop => 1,
                ILOpCode.Jmp => 0,
                ILOpCode.Call or ILOpCode.Calli or ILOpCode.Ret => -1,// Variable
                ILOpCode.Br_s => 0,
                ILOpCode.Brfalse_s or ILOpCode.Brtrue_s => 1,
                ILOpCode.Beq_s or ILOpCode.Bge_s or ILOpCode.Bgt_s or ILOpCode.Ble_s or ILOpCode.Blt_s or ILOpCode.Bne_un_s or ILOpCode.Bge_un_s or ILOpCode.Bgt_un_s or ILOpCode.Ble_un_s or ILOpCode.Blt_un_s => 2,
                ILOpCode.Br => 0,
                ILOpCode.Brfalse or ILOpCode.Brtrue => 1,
                ILOpCode.Beq or ILOpCode.Bge or ILOpCode.Bgt or ILOpCode.Ble or ILOpCode.Blt or ILOpCode.Bne_un or ILOpCode.Bge_un or ILOpCode.Bgt_un or ILOpCode.Ble_un or ILOpCode.Blt_un => 2,
                ILOpCode.Switch or ILOpCode.Ldind_i1 or ILOpCode.Ldind_u1 or ILOpCode.Ldind_i2 or ILOpCode.Ldind_u2 or ILOpCode.Ldind_i4 or ILOpCode.Ldind_u4 or ILOpCode.Ldind_i8 or ILOpCode.Ldind_i or ILOpCode.Ldind_r4 or ILOpCode.Ldind_r8 or ILOpCode.Ldind_ref => 1,
                ILOpCode.Stind_ref or ILOpCode.Stind_i1 or ILOpCode.Stind_i2 or ILOpCode.Stind_i4 or ILOpCode.Stind_i8 or ILOpCode.Stind_r4 or ILOpCode.Stind_r8 or ILOpCode.Add or ILOpCode.Sub or ILOpCode.Mul or ILOpCode.Div or ILOpCode.Div_un or ILOpCode.Rem or ILOpCode.Rem_un or ILOpCode.And or ILOpCode.Or or ILOpCode.Xor or ILOpCode.Shl or ILOpCode.Shr or ILOpCode.Shr_un => 2,
                ILOpCode.Neg or ILOpCode.Not or ILOpCode.Conv_i1 or ILOpCode.Conv_i2 or ILOpCode.Conv_i4 or ILOpCode.Conv_i8 or ILOpCode.Conv_r4 or ILOpCode.Conv_r8 or ILOpCode.Conv_u4 or ILOpCode.Conv_u8 => 1,
                ILOpCode.Callvirt => -1,// Variable
                ILOpCode.Cpobj => 2,
                ILOpCode.Ldobj => 1,
                ILOpCode.Ldstr => 0,
                ILOpCode.Newobj => -1,// Variable
                ILOpCode.Castclass or ILOpCode.Isinst or ILOpCode.Conv_r_un or ILOpCode.Unbox or ILOpCode.Throw or ILOpCode.Ldfld or ILOpCode.Ldflda => 1,
                ILOpCode.Stfld => 2,
                ILOpCode.Ldsfld or ILOpCode.Ldsflda => 0,
                ILOpCode.Stsfld => 1,
                ILOpCode.Stobj => 2,
                ILOpCode.Conv_ovf_i1_un or ILOpCode.Conv_ovf_i2_un or ILOpCode.Conv_ovf_i4_un or ILOpCode.Conv_ovf_i8_un or ILOpCode.Conv_ovf_u1_un or ILOpCode.Conv_ovf_u2_un or ILOpCode.Conv_ovf_u4_un or ILOpCode.Conv_ovf_u8_un or ILOpCode.Conv_ovf_i_un or ILOpCode.Conv_ovf_u_un or ILOpCode.Box or ILOpCode.Newarr or ILOpCode.Ldlen => 1,
                ILOpCode.Ldelema or ILOpCode.Ldelem_i1 or ILOpCode.Ldelem_u1 or ILOpCode.Ldelem_i2 or ILOpCode.Ldelem_u2 or ILOpCode.Ldelem_i4 or ILOpCode.Ldelem_u4 or ILOpCode.Ldelem_i8 or ILOpCode.Ldelem_i or ILOpCode.Ldelem_r4 or ILOpCode.Ldelem_r8 or ILOpCode.Ldelem_ref => 2,
                ILOpCode.Stelem_i or ILOpCode.Stelem_i1 or ILOpCode.Stelem_i2 or ILOpCode.Stelem_i4 or ILOpCode.Stelem_i8 or ILOpCode.Stelem_r4 or ILOpCode.Stelem_r8 or ILOpCode.Stelem_ref => 3,
                ILOpCode.Ldelem => 2,
                ILOpCode.Stelem => 3,
                ILOpCode.Unbox_any or ILOpCode.Conv_ovf_i1 or ILOpCode.Conv_ovf_u1 or ILOpCode.Conv_ovf_i2 or ILOpCode.Conv_ovf_u2 or ILOpCode.Conv_ovf_i4 or ILOpCode.Conv_ovf_u4 or ILOpCode.Conv_ovf_i8 or ILOpCode.Conv_ovf_u8 or ILOpCode.Refanyval or ILOpCode.Ckfinite or ILOpCode.Mkrefany => 1,
                ILOpCode.Ldtoken => 0,
                ILOpCode.Conv_u2 or ILOpCode.Conv_u1 or ILOpCode.Conv_i or ILOpCode.Conv_ovf_i or ILOpCode.Conv_ovf_u => 1,
                ILOpCode.Add_ovf or ILOpCode.Add_ovf_un or ILOpCode.Mul_ovf or ILOpCode.Mul_ovf_un or ILOpCode.Sub_ovf or ILOpCode.Sub_ovf_un => 2,
                ILOpCode.Endfinally or ILOpCode.Leave or ILOpCode.Leave_s => 0,
                ILOpCode.Stind_i => 2,
                ILOpCode.Conv_u => 1,
                ILOpCode.Arglist => 0,
                ILOpCode.Ceq or ILOpCode.Cgt or ILOpCode.Cgt_un or ILOpCode.Clt or ILOpCode.Clt_un => 2,
                ILOpCode.Ldftn => 0,
                ILOpCode.Ldvirtftn => 1,
                ILOpCode.Ldarg or ILOpCode.Ldarga => 0,
                ILOpCode.Starg => 1,
                ILOpCode.Ldloc or ILOpCode.Ldloca => 0,
                ILOpCode.Stloc or ILOpCode.Localloc or ILOpCode.Endfilter => 1,
                ILOpCode.Unaligned or ILOpCode.Volatile or ILOpCode.Tail => 0,
                ILOpCode.Initobj => 1,
                ILOpCode.Constrained => 0,
                ILOpCode.Cpblk or ILOpCode.Initblk => 3,
                ILOpCode.Rethrow or ILOpCode.Sizeof => 0,
                ILOpCode.Refanytype => 1,
                ILOpCode.Readonly => 0,
                _ => throw ExceptionUtilities.UnexpectedValue(opcode),
            };
        }

        public static int StackPushCount(this ILOpCode opcode)
        {
            return opcode switch
            {
                ILOpCode.Nop or ILOpCode.Break => 0,
                ILOpCode.Ldarg_0 or ILOpCode.Ldarg_1 or ILOpCode.Ldarg_2 or ILOpCode.Ldarg_3 or ILOpCode.Ldloc_0 or ILOpCode.Ldloc_1 or ILOpCode.Ldloc_2 or ILOpCode.Ldloc_3 => 1,
                ILOpCode.Stloc_0 or ILOpCode.Stloc_1 or ILOpCode.Stloc_2 or ILOpCode.Stloc_3 => 0,
                ILOpCode.Ldarg_s or ILOpCode.Ldarga_s => 1,
                ILOpCode.Starg_s => 0,
                ILOpCode.Ldloc_s or ILOpCode.Ldloca_s => 1,
                ILOpCode.Stloc_s => 0,
                ILOpCode.Ldnull or ILOpCode.Ldc_i4_m1 or ILOpCode.Ldc_i4_0 or ILOpCode.Ldc_i4_1 or ILOpCode.Ldc_i4_2 or ILOpCode.Ldc_i4_3 or ILOpCode.Ldc_i4_4 or ILOpCode.Ldc_i4_5 or ILOpCode.Ldc_i4_6 or ILOpCode.Ldc_i4_7 or ILOpCode.Ldc_i4_8 or ILOpCode.Ldc_i4_s or ILOpCode.Ldc_i4 or ILOpCode.Ldc_i8 or ILOpCode.Ldc_r4 or ILOpCode.Ldc_r8 => 1,
                ILOpCode.Dup => 2,
                ILOpCode.Pop or ILOpCode.Jmp => 0,
                ILOpCode.Call or ILOpCode.Calli => -1,// Variable
                ILOpCode.Ret or ILOpCode.Br_s or ILOpCode.Brfalse_s or ILOpCode.Brtrue_s or ILOpCode.Beq_s or ILOpCode.Bge_s or ILOpCode.Bgt_s or ILOpCode.Ble_s or ILOpCode.Blt_s or ILOpCode.Bne_un_s or ILOpCode.Bge_un_s or ILOpCode.Bgt_un_s or ILOpCode.Ble_un_s or ILOpCode.Blt_un_s or ILOpCode.Br or ILOpCode.Brfalse or ILOpCode.Brtrue or ILOpCode.Beq or ILOpCode.Bge or ILOpCode.Bgt or ILOpCode.Ble or ILOpCode.Blt or ILOpCode.Bne_un or ILOpCode.Bge_un or ILOpCode.Bgt_un or ILOpCode.Ble_un or ILOpCode.Blt_un or ILOpCode.Switch => 0,
                ILOpCode.Ldind_i1 or ILOpCode.Ldind_u1 or ILOpCode.Ldind_i2 or ILOpCode.Ldind_u2 or ILOpCode.Ldind_i4 or ILOpCode.Ldind_u4 or ILOpCode.Ldind_i8 or ILOpCode.Ldind_i or ILOpCode.Ldind_r4 or ILOpCode.Ldind_r8 or ILOpCode.Ldind_ref => 1,
                ILOpCode.Stind_ref or ILOpCode.Stind_i1 or ILOpCode.Stind_i2 or ILOpCode.Stind_i4 or ILOpCode.Stind_i8 or ILOpCode.Stind_r4 or ILOpCode.Stind_r8 => 0,
                ILOpCode.Add or ILOpCode.Sub or ILOpCode.Mul or ILOpCode.Div or ILOpCode.Div_un or ILOpCode.Rem or ILOpCode.Rem_un or ILOpCode.And or ILOpCode.Or or ILOpCode.Xor or ILOpCode.Shl or ILOpCode.Shr or ILOpCode.Shr_un or ILOpCode.Neg or ILOpCode.Not or ILOpCode.Conv_i1 or ILOpCode.Conv_i2 or ILOpCode.Conv_i4 or ILOpCode.Conv_i8 or ILOpCode.Conv_r4 or ILOpCode.Conv_r8 or ILOpCode.Conv_u4 or ILOpCode.Conv_u8 => 1,
                ILOpCode.Callvirt => -1,// Variable
                ILOpCode.Cpobj => 0,
                ILOpCode.Ldobj or ILOpCode.Ldstr or ILOpCode.Newobj or ILOpCode.Castclass or ILOpCode.Isinst or ILOpCode.Conv_r_un or ILOpCode.Unbox => 1,
                ILOpCode.Throw => 0,
                ILOpCode.Ldfld or ILOpCode.Ldflda => 1,
                ILOpCode.Stfld => 0,
                ILOpCode.Ldsfld or ILOpCode.Ldsflda => 1,
                ILOpCode.Stsfld or ILOpCode.Stobj => 0,
                ILOpCode.Conv_ovf_i1_un or ILOpCode.Conv_ovf_i2_un or ILOpCode.Conv_ovf_i4_un or ILOpCode.Conv_ovf_i8_un or ILOpCode.Conv_ovf_u1_un or ILOpCode.Conv_ovf_u2_un or ILOpCode.Conv_ovf_u4_un or ILOpCode.Conv_ovf_u8_un or ILOpCode.Conv_ovf_i_un or ILOpCode.Conv_ovf_u_un or ILOpCode.Box or ILOpCode.Newarr or ILOpCode.Ldlen or ILOpCode.Ldelema or ILOpCode.Ldelem_i1 or ILOpCode.Ldelem_u1 or ILOpCode.Ldelem_i2 or ILOpCode.Ldelem_u2 or ILOpCode.Ldelem_i4 or ILOpCode.Ldelem_u4 or ILOpCode.Ldelem_i8 or ILOpCode.Ldelem_i or ILOpCode.Ldelem_r4 or ILOpCode.Ldelem_r8 or ILOpCode.Ldelem_ref => 1,
                ILOpCode.Stelem_i or ILOpCode.Stelem_i1 or ILOpCode.Stelem_i2 or ILOpCode.Stelem_i4 or ILOpCode.Stelem_i8 or ILOpCode.Stelem_r4 or ILOpCode.Stelem_r8 or ILOpCode.Stelem_ref => 0,
                ILOpCode.Ldelem => 1,
                ILOpCode.Stelem => 0,
                ILOpCode.Unbox_any or ILOpCode.Conv_ovf_i1 or ILOpCode.Conv_ovf_u1 or ILOpCode.Conv_ovf_i2 or ILOpCode.Conv_ovf_u2 or ILOpCode.Conv_ovf_i4 or ILOpCode.Conv_ovf_u4 or ILOpCode.Conv_ovf_i8 or ILOpCode.Conv_ovf_u8 or ILOpCode.Refanyval or ILOpCode.Ckfinite or ILOpCode.Mkrefany or ILOpCode.Ldtoken or ILOpCode.Conv_u2 or ILOpCode.Conv_u1 or ILOpCode.Conv_i or ILOpCode.Conv_ovf_i or ILOpCode.Conv_ovf_u or ILOpCode.Add_ovf or ILOpCode.Add_ovf_un or ILOpCode.Mul_ovf or ILOpCode.Mul_ovf_un or ILOpCode.Sub_ovf or ILOpCode.Sub_ovf_un => 1,
                ILOpCode.Endfinally or ILOpCode.Leave or ILOpCode.Leave_s or ILOpCode.Stind_i => 0,
                ILOpCode.Conv_u or ILOpCode.Arglist or ILOpCode.Ceq or ILOpCode.Cgt or ILOpCode.Cgt_un or ILOpCode.Clt or ILOpCode.Clt_un or ILOpCode.Ldftn or ILOpCode.Ldvirtftn or ILOpCode.Ldarg or ILOpCode.Ldarga => 1,
                ILOpCode.Starg => 0,
                ILOpCode.Ldloc or ILOpCode.Ldloca => 1,
                ILOpCode.Stloc => 0,
                ILOpCode.Localloc => 1,
                ILOpCode.Endfilter or ILOpCode.Unaligned or ILOpCode.Volatile or ILOpCode.Tail or ILOpCode.Initobj or ILOpCode.Constrained or ILOpCode.Cpblk or ILOpCode.Initblk or ILOpCode.Rethrow => 0,
                ILOpCode.Sizeof or ILOpCode.Refanytype => 1,
                ILOpCode.Readonly => 0,
                _ => throw ExceptionUtilities.UnexpectedValue(opcode),
            };
        }
    }
}
