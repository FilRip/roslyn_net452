using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class OverloadResolution
    {
        internal static class BinopEasyOut
        {
            private const BinaryOperatorKind ERR = BinaryOperatorKind.Error;

            private const BinaryOperatorKind OBJ = BinaryOperatorKind.Object;

            private const BinaryOperatorKind STR = BinaryOperatorKind.String;

            private const BinaryOperatorKind OSC = BinaryOperatorKind.ObjectAndString;

            private const BinaryOperatorKind SOC = BinaryOperatorKind.StringAndObject;

            private const BinaryOperatorKind INT = BinaryOperatorKind.Int;

            private const BinaryOperatorKind UIN = BinaryOperatorKind.UInt;

            private const BinaryOperatorKind LNG = BinaryOperatorKind.Long;

            private const BinaryOperatorKind ULG = BinaryOperatorKind.ULong;

            private const BinaryOperatorKind NIN = BinaryOperatorKind.NInt;

            private const BinaryOperatorKind NUI = BinaryOperatorKind.NUInt;

            private const BinaryOperatorKind FLT = BinaryOperatorKind.Float;

            private const BinaryOperatorKind DBL = BinaryOperatorKind.Double;

            private const BinaryOperatorKind DEC = BinaryOperatorKind.Decimal;

            private const BinaryOperatorKind BOL = BinaryOperatorKind.Bool;

            private const BinaryOperatorKind LIN = BinaryOperatorKind.Int | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LUN = BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LLG = BinaryOperatorKind.Long | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LUL = BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LNI = BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LNU = BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LFL = BinaryOperatorKind.Float | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LDB = BinaryOperatorKind.Double | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LDC = BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted;

            private const BinaryOperatorKind LBL = BinaryOperatorKind.Bool | BinaryOperatorKind.Lifted;

            private static readonly BinaryOperatorKind[,] s_arithmetic = new BinaryOperatorKind[32, 32]
            {
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                }
            };

            private static readonly BinaryOperatorKind[,] s_addition = new BinaryOperatorKind[32, 32]
            {
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.String,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject,
                    BinaryOperatorKind.StringAndObject
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ObjectAndString,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                }
            };

            private static readonly BinaryOperatorKind[,] s_shift = new BinaryOperatorKind[32, 32]
            {
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                }
            };

            private static readonly BinaryOperatorKind[,] s_equality = new BinaryOperatorKind[32, 32]
            {
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.String,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Bool,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Bool | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Float,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Double,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Bool | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Bool | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Float | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Double | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object
                },
                {
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Object,
                    BinaryOperatorKind.Decimal | BinaryOperatorKind.Lifted
                }
            };

            private static readonly BinaryOperatorKind[,] s_logical = new BinaryOperatorKind[32, 32]
            {
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Bool,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Bool | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.Int,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.UInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Long,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.ULong,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Bool | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Bool | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Int | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.UInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Long | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.ULong | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.NUInt | BinaryOperatorKind.Lifted,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                },
                {
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error,
                    BinaryOperatorKind.Error
                }
            };

            private static readonly BinaryOperatorKind[][,] s_opkind = new BinaryOperatorKind[16][,]
            {
                s_arithmetic, s_addition, s_arithmetic, s_arithmetic, s_arithmetic, s_shift, s_shift, s_equality, s_equality, s_arithmetic,
                s_arithmetic, s_arithmetic, s_arithmetic, s_logical, s_logical, s_logical
            };

            public static BinaryOperatorKind OpKind(BinaryOperatorKind kind, TypeSymbol left, TypeSymbol right)
            {
                int num = left.TypeToIndex();
                if (num < 0)
                {
                    return BinaryOperatorKind.Error;
                }
                int num2 = right.TypeToIndex();
                if (num2 < 0)
                {
                    return BinaryOperatorKind.Error;
                }
                BinaryOperatorKind binaryOperatorKind = BinaryOperatorKind.Error;
                if (!kind.IsLogical() || (num == 15 && num2 == 15))
                {
                    binaryOperatorKind = s_opkind[kind.OperatorIndex()][num, num2];
                }
                if (binaryOperatorKind != 0)
                {
                    return binaryOperatorKind | kind;
                }
                return binaryOperatorKind;
            }
        }

        private enum LiftingResult
        {
            NotLifted,
            LiftOperandsAndResult,
            LiftOperandsButNotResult
        }

        internal static class UnopEasyOut
        {
            private const UnaryOperatorKind ERR = UnaryOperatorKind.Error;

            private const UnaryOperatorKind BOL = UnaryOperatorKind.Bool;

            private const UnaryOperatorKind CHR = UnaryOperatorKind.Char;

            private const UnaryOperatorKind I08 = UnaryOperatorKind.SByte;

            private const UnaryOperatorKind U08 = UnaryOperatorKind.Byte;

            private const UnaryOperatorKind I16 = UnaryOperatorKind.Short;

            private const UnaryOperatorKind U16 = UnaryOperatorKind.UShort;

            private const UnaryOperatorKind I32 = UnaryOperatorKind.Int;

            private const UnaryOperatorKind U32 = UnaryOperatorKind.UInt;

            private const UnaryOperatorKind I64 = UnaryOperatorKind.Long;

            private const UnaryOperatorKind U64 = UnaryOperatorKind.ULong;

            private const UnaryOperatorKind NIN = UnaryOperatorKind.NInt;

            private const UnaryOperatorKind NUI = UnaryOperatorKind.NUInt;

            private const UnaryOperatorKind R32 = UnaryOperatorKind.Float;

            private const UnaryOperatorKind R64 = UnaryOperatorKind.Double;

            private const UnaryOperatorKind DEC = UnaryOperatorKind.Decimal;

            private const UnaryOperatorKind LBOL = UnaryOperatorKind.Bool | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LCHR = UnaryOperatorKind.Char | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LI08 = UnaryOperatorKind.SByte | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LU08 = UnaryOperatorKind.Byte | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LI16 = UnaryOperatorKind.Short | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LU16 = UnaryOperatorKind.UShort | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LI32 = UnaryOperatorKind.Int | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LU32 = UnaryOperatorKind.UInt | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LI64 = UnaryOperatorKind.Long | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LU64 = UnaryOperatorKind.ULong | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LNI = UnaryOperatorKind.NInt | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LNU = UnaryOperatorKind.NUInt | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LR32 = UnaryOperatorKind.Float | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LR64 = UnaryOperatorKind.Double | UnaryOperatorKind.Lifted;

            private const UnaryOperatorKind LDEC = UnaryOperatorKind.Decimal | UnaryOperatorKind.Lifted;

            private static readonly UnaryOperatorKind[] s_increment = new UnaryOperatorKind[32]
            {
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Char,
                UnaryOperatorKind.SByte,
                UnaryOperatorKind.Short,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Long,
                UnaryOperatorKind.Byte,
                UnaryOperatorKind.UShort,
                UnaryOperatorKind.UInt,
                UnaryOperatorKind.ULong,
                UnaryOperatorKind.NInt,
                UnaryOperatorKind.NUInt,
                UnaryOperatorKind.Float,
                UnaryOperatorKind.Double,
                UnaryOperatorKind.Decimal,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Char | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.SByte | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Short | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Long | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Byte | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.UShort | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.UInt | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.ULong | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.NInt | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.NUInt | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Float | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Double | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Decimal | UnaryOperatorKind.Lifted
            };

            private static readonly UnaryOperatorKind[] s_plus = new UnaryOperatorKind[32]
            {
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Long,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.UInt,
                UnaryOperatorKind.ULong,
                UnaryOperatorKind.NInt,
                UnaryOperatorKind.NUInt,
                UnaryOperatorKind.Float,
                UnaryOperatorKind.Double,
                UnaryOperatorKind.Decimal,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Long | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.UInt | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.ULong | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.NInt | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.NUInt | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Float | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Double | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Decimal | UnaryOperatorKind.Lifted
            };

            private static readonly UnaryOperatorKind[] s_minus = new UnaryOperatorKind[32]
            {
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Long,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Int,
                UnaryOperatorKind.Long,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.NInt,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Float,
                UnaryOperatorKind.Double,
                UnaryOperatorKind.Decimal,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Long | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Long | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.NInt | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Error,
                UnaryOperatorKind.Float | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Double | UnaryOperatorKind.Lifted,
                UnaryOperatorKind.Decimal | UnaryOperatorKind.Lifted
            };

            private static readonly UnaryOperatorKind[] s_logicalNegation;

            private static readonly UnaryOperatorKind[] s_bitwiseComplement;

            private static readonly UnaryOperatorKind[][] s_opkind;

            public static UnaryOperatorKind OpKind(UnaryOperatorKind kind, TypeSymbol operand)
            {
                int num = operand.TypeToIndex();
                if (num < 0)
                {
                    return UnaryOperatorKind.Error;
                }
                int num2 = kind.OperatorIndex();
                UnaryOperatorKind unaryOperatorKind = ((num2 < s_opkind.Length) ? s_opkind[num2][num] : UnaryOperatorKind.Error);
                if (unaryOperatorKind != 0)
                {
                    return unaryOperatorKind | kind;
                }
                return unaryOperatorKind;
            }

            static UnopEasyOut()
            {
                UnaryOperatorKind[] array = new UnaryOperatorKind[32];
                array[2] = UnaryOperatorKind.Bool;
                array[17] = UnaryOperatorKind.Bool | UnaryOperatorKind.Lifted;
                s_logicalNegation = array;
                s_bitwiseComplement = new UnaryOperatorKind[32]
                {
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Int,
                    UnaryOperatorKind.Int,
                    UnaryOperatorKind.Int,
                    UnaryOperatorKind.Int,
                    UnaryOperatorKind.Long,
                    UnaryOperatorKind.Int,
                    UnaryOperatorKind.Int,
                    UnaryOperatorKind.UInt,
                    UnaryOperatorKind.ULong,
                    UnaryOperatorKind.NInt,
                    UnaryOperatorKind.NUInt,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.Long | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.Int | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.UInt | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.ULong | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.NInt | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.NUInt | UnaryOperatorKind.Lifted,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Error,
                    UnaryOperatorKind.Error
                };
                s_opkind = new UnaryOperatorKind[8][] { s_increment, s_increment, s_increment, s_increment, s_plus, s_minus, s_logicalNegation, s_bitwiseComplement };
            }
        }

        private class ReturnStatements : BoundTreeWalker
        {
            private readonly ArrayBuilder<BoundReturnStatement> _returns;

            public ReturnStatements(ArrayBuilder<BoundReturnStatement> returns)
            {
                _returns = returns;
            }

            public override BoundNode Visit(BoundNode node)
            {
                if (!(node is BoundExpression))
                {
                    return base.Visit(node);
                }
                return null;
            }

            protected override BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
            {
                return null;
            }

            public override BoundNode VisitReturnStatement(BoundReturnStatement node)
            {
                _returns.Add(node);
                return null;
            }
        }

        private struct EffectiveParameters
        {
            internal readonly ImmutableArray<TypeWithAnnotations> ParameterTypes;

            internal readonly ImmutableArray<RefKind> ParameterRefKinds;

            internal EffectiveParameters(ImmutableArray<TypeWithAnnotations> types, ImmutableArray<RefKind> refKinds)
            {
                ParameterTypes = types;
                ParameterRefKinds = refKinds;
            }
        }

        private struct ParameterMap
        {
            private readonly int[] _parameters;

            private readonly int _length;

            public bool IsTrivial => _parameters == null;

            public int Length => _length;

            public int this[int argument]
            {
                get
                {
                    if (_parameters != null)
                    {
                        return _parameters[argument];
                    }
                    return argument;
                }
            }

            public ParameterMap(int[] parameters, int length)
            {
                _parameters = parameters;
                _length = length;
            }

            public ImmutableArray<int> ToImmutableArray()
            {
                return _parameters.AsImmutableOrNull();
            }
        }

        private readonly Binder _binder;

        private bool? _strict;

        private const int BetterConversionTargetRecursionLimit = 100;

        private CSharpCompilation Compilation => _binder.Compilation;

        private Conversions Conversions => _binder.Conversions;

        private bool Strict
        {
            get
            {
                if (_strict.HasValue)
                {
                    return _strict.Value;
                }
                bool featureStrictEnabled = _binder.Compilation.FeatureStrictEnabled;
                _strict = featureStrictEnabled;
                return featureStrictEnabled;
            }
        }

        private void BinaryOperatorEasyOut(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, BinaryOperatorOverloadResolutionResult result)
        {
            TypeSymbol type = left.Type;
            if ((object)type == null)
            {
                return;
            }
            TypeSymbol type2 = right.Type;
            if ((object)type2 != null && !PossiblyUnusualConstantOperation(left, right))
            {
                BinaryOperatorKind binaryOperatorKind = BinopEasyOut.OpKind(kind, type, type2);
                if (binaryOperatorKind != 0)
                {
                    BinaryOperatorSignature signature = Compilation.builtInOperators.GetSignature(binaryOperatorKind);
                    Conversion leftConversion = ConversionsBase.FastClassifyConversion(type, signature.LeftType);
                    Conversion rightConversion = ConversionsBase.FastClassifyConversion(type2, signature.RightType);
                    result.Results.Add(BinaryOperatorAnalysisResult.Applicable(signature, leftConversion, rightConversion));
                }
            }
        }

        private static bool PossiblyUnusualConstantOperation(BoundExpression left, BoundExpression right)
        {
            if (left.ConstantValue == null && right.ConstantValue == null)
            {
                return false;
            }
            if (left.Type!.SpecialType != right.Type!.SpecialType)
            {
                return true;
            }
            if (left.Type!.SpecialType == SpecialType.System_Int32 || left.Type!.SpecialType == SpecialType.System_Boolean || left.Type!.SpecialType == SpecialType.System_String)
            {
                return false;
            }
            return true;
        }

        public void BinaryOperatorOverloadResolution(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, BinaryOperatorOverloadResolutionResult result, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            BinaryOperatorOverloadResolution_EasyOut(kind, left, right, result);
            if (result.Results.Count <= 0)
            {
                BinaryOperatorOverloadResolution_NoEasyOut(kind, left, right, result, ref useSiteInfo);
            }
        }

        internal void BinaryOperatorOverloadResolution_EasyOut(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, BinaryOperatorOverloadResolutionResult result)
        {
            BinaryOperatorKind kind2 = kind & ~BinaryOperatorKind.Logical;
            BinaryOperatorEasyOut(kind2, left, right, result);
        }

        internal void BinaryOperatorOverloadResolution_NoEasyOut(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, BinaryOperatorOverloadResolutionResult result, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeSymbol typeSymbol = left.Type?.StrippedType();
            TypeSymbol typeSymbol2 = right.Type?.StrippedType();
            bool flag = typeSymbol?.IsInterfaceType() ?? false;
            bool flag2 = typeSymbol2?.IsInterfaceType() ?? false;
            bool flag3 = false;
            if ((object)typeSymbol != null && !flag)
            {
                flag3 = GetUserDefinedOperators(kind, typeSymbol, left, right, result.Results, ref useSiteInfo);
                if (!flag3)
                {
                    result.Results.Clear();
                }
            }
            if ((object)typeSymbol2 != null && !flag2 && !typeSymbol2.Equals(typeSymbol))
            {
                ArrayBuilder<BinaryOperatorAnalysisResult> instance = ArrayBuilder<BinaryOperatorAnalysisResult>.GetInstance();
                if (GetUserDefinedOperators(kind, typeSymbol2, left, right, instance, ref useSiteInfo))
                {
                    flag3 = true;
                    AddDistinctOperators(result.Results, instance);
                }
                instance.Free();
            }
            if (!flag3 && kind != BinaryOperatorKind.Equal && kind != BinaryOperatorKind.NotEqual)
            {
                result.Results.Clear();
                string name = OperatorFacts.BinaryOperatorNameFromOperatorKind(kind);
                PooledDictionary<TypeSymbol, bool> instance2 = PooledDictionary<TypeSymbol, bool>.GetInstance();
                flag3 = GetUserDefinedBinaryOperatorsFromInterfaces(kind, name, typeSymbol, flag, left, right, ref useSiteInfo, instance2, result.Results);
                if (!flag3)
                {
                    result.Results.Clear();
                }
                if ((object)typeSymbol2 != null && !typeSymbol2.Equals(typeSymbol))
                {
                    ArrayBuilder<BinaryOperatorAnalysisResult> instance3 = ArrayBuilder<BinaryOperatorAnalysisResult>.GetInstance();
                    if (GetUserDefinedBinaryOperatorsFromInterfaces(kind, name, typeSymbol2, flag2, left, right, ref useSiteInfo, instance2, instance3))
                    {
                        flag3 = true;
                        AddDistinctOperators(result.Results, instance3);
                    }
                    instance3.Free();
                }
                instance2.Free();
            }
            if (!flag3)
            {
                result.Results.Clear();
                GetAllBuiltInOperators(kind, left, right, result.Results, ref useSiteInfo);
            }
            BinaryOperatorOverloadResolution(left, right, result, ref useSiteInfo);
        }

        private bool GetUserDefinedBinaryOperatorsFromInterfaces(BinaryOperatorKind kind, string name, TypeSymbol operatorSourceOpt, bool sourceIsInterface, BoundExpression left, BoundExpression right, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, Dictionary<TypeSymbol, bool> lookedInInterfaces, ArrayBuilder<BinaryOperatorAnalysisResult> candidates)
        {
            if ((object)operatorSourceOpt == null)
            {
                return false;
            }
            bool flag = false;
            ImmutableArray<NamedTypeSymbol> immutableArray = default(ImmutableArray<NamedTypeSymbol>);
            if (sourceIsInterface)
            {
                if (!lookedInInterfaces.TryGetValue(operatorSourceOpt, out var _))
                {
                    ArrayBuilder<BinaryOperatorSignature> instance = ArrayBuilder<BinaryOperatorSignature>.GetInstance();
                    GetUserDefinedBinaryOperatorsFromType((NamedTypeSymbol)operatorSourceOpt, kind, name, instance);
                    flag = CandidateOperators(instance, left, right, candidates, ref useSiteInfo);
                    instance.Free();
                    lookedInInterfaces.Add(operatorSourceOpt, flag);
                    if (!flag)
                    {
                        candidates.Clear();
                        immutableArray = operatorSourceOpt.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                    }
                }
            }
            else if (operatorSourceOpt.IsTypeParameter())
            {
                immutableArray = ((TypeParameterSymbol)operatorSourceOpt).AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            if (!immutableArray.IsDefaultOrEmpty)
            {
                ArrayBuilder<BinaryOperatorSignature> instance2 = ArrayBuilder<BinaryOperatorSignature>.GetInstance();
                ArrayBuilder<BinaryOperatorAnalysisResult> instance3 = ArrayBuilder<BinaryOperatorAnalysisResult>.GetInstance();
                PooledHashSet<NamedTypeSymbol> instance4 = PooledHashSet<NamedTypeSymbol>.GetInstance();
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NamedTypeSymbol current = enumerator.Current;
                    if (!current.IsInterface || instance4.Contains(current))
                    {
                        continue;
                    }
                    if (lookedInInterfaces.TryGetValue(current, out var value2))
                    {
                        if (value2)
                        {
                            instance4.AddAll(current.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo));
                        }
                        continue;
                    }
                    instance2.Clear();
                    instance3.Clear();
                    GetUserDefinedBinaryOperatorsFromType(current, kind, name, instance2);
                    value2 = CandidateOperators(instance2, left, right, instance3, ref useSiteInfo);
                    lookedInInterfaces.Add(current, value2);
                    if (value2)
                    {
                        flag = true;
                        candidates.AddRange(instance3);
                        instance4.AddAll(current.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo));
                    }
                }
                instance2.Free();
                instance3.Free();
                instance4.Free();
            }
            return flag;
        }

        private void AddDelegateOperation(BinaryOperatorKind kind, TypeSymbol delegateType, ArrayBuilder<BinaryOperatorSignature> operators)
        {
            switch (kind)
            {
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                    operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Delegate, delegateType, delegateType, Compilation.GetSpecialType(SpecialType.System_Boolean)));
                    break;
                default:
                    operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Delegate, delegateType, delegateType, delegateType));
                    break;
            }
        }

        private void GetDelegateOperations(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorSignature> operators, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            switch (kind)
            {
                case BinaryOperatorKind.Multiplication:
                case BinaryOperatorKind.Division:
                case BinaryOperatorKind.Remainder:
                case BinaryOperatorKind.LeftShift:
                case BinaryOperatorKind.RightShift:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThanOrEqual:
                case BinaryOperatorKind.And:
                case BinaryOperatorKind.Xor:
                case BinaryOperatorKind.Or:
                case BinaryOperatorKind.LogicalAnd:
                case BinaryOperatorKind.LogicalOr:
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
                case BinaryOperatorKind.Addition:
                case BinaryOperatorKind.Subtraction:
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                    {
                        TypeSymbol type = left.Type;
                        bool flag = type?.IsDelegateType() ?? false;
                        TypeSymbol type2 = right.Type;
                        bool flag2 = type2?.IsDelegateType() ?? false;
                        if (!flag && !flag2)
                        {
                            BinaryOperatorKind binaryOperatorKind = kind.Operator();
                            if (binaryOperatorKind == BinaryOperatorKind.Equal || binaryOperatorKind == BinaryOperatorKind.NotEqual)
                            {
                                TypeSymbol specialType = _binder.Compilation.GetSpecialType(SpecialType.System_Delegate);
                                specialType.AddUseSiteInfo(ref useSiteInfo);
                                if (Conversions.ClassifyImplicitConversionFromExpression(left, specialType, ref useSiteInfo).IsValid && Conversions.ClassifyImplicitConversionFromExpression(right, specialType, ref useSiteInfo).IsValid)
                                {
                                    AddDelegateOperation(kind, specialType, operators);
                                }
                            }
                        }
                        else if (flag && flag2)
                        {
                            AddDelegateOperation(kind, type, operators);
                            if (!((kind == BinaryOperatorKind.Equal || kind == BinaryOperatorKind.NotEqual) ? ConversionsBase.HasIdentityConversion(type, type2) : type.Equals(type2)))
                            {
                                AddDelegateOperation(kind, type2, operators);
                            }
                        }
                        else
                        {
                            TypeSymbol delegateType = (flag ? type : type2);
                            BoundExpression boundExpression = (flag ? right : left);
                            if ((kind != BinaryOperatorKind.Equal && kind != BinaryOperatorKind.NotEqual) || boundExpression.Kind != BoundKind.UnboundLambda)
                            {
                                AddDelegateOperation(kind, delegateType, operators);
                            }
                        }
                        break;
                    }
            }
        }

        private void GetEnumOperation(BinaryOperatorKind kind, TypeSymbol enumType, BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorSignature> operators)
        {
            if (!enumType.IsValidEnumType())
            {
                return;
            }
            NamedTypeSymbol enumUnderlyingType = enumType.GetEnumUnderlyingType();
            NamedTypeSymbol specialType = Compilation.GetSpecialType(SpecialType.System_Nullable_T);
            NamedTypeSymbol namedTypeSymbol = specialType.Construct(enumType);
            NamedTypeSymbol namedTypeSymbol2 = specialType.Construct(enumUnderlyingType);
            switch (kind)
            {
                case BinaryOperatorKind.Addition:
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.EnumAndUnderlyingAddition, enumType, enumUnderlyingType, enumType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.UnderlyingAndEnumAddition, enumUnderlyingType, enumType, enumType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LiftedEnumAndUnderlyingAddition, namedTypeSymbol, namedTypeSymbol2, namedTypeSymbol));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LiftedUnderlyingAndEnumAddition, namedTypeSymbol2, namedTypeSymbol, namedTypeSymbol));
                    break;
                case BinaryOperatorKind.Subtraction:
                    {
                        if (Strict)
                        {
                            operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.EnumSubtraction, enumType, enumType, enumUnderlyingType));
                            operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.EnumAndUnderlyingSubtraction, enumType, enumUnderlyingType, enumType));
                            operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LiftedEnumSubtraction, namedTypeSymbol, namedTypeSymbol, namedTypeSymbol2));
                            operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LiftedEnumAndUnderlyingSubtraction, namedTypeSymbol, namedTypeSymbol2, namedTypeSymbol));
                            break;
                        }
                        bool flag = TypeSymbol.Equals(right.Type?.StrippedType(), enumUnderlyingType, TypeCompareKind.ConsiderEverything);
                        operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.EnumSubtraction, enumType, enumType, enumUnderlyingType)
                        {
                            Priority = 2
                        });
                        operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.EnumAndUnderlyingSubtraction, enumType, enumUnderlyingType, enumType)
                        {
                            Priority = (flag ? 1 : 3)
                        });
                        operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LiftedEnumSubtraction, namedTypeSymbol, namedTypeSymbol, namedTypeSymbol2)
                        {
                            Priority = 12
                        });
                        operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LiftedEnumAndUnderlyingSubtraction, namedTypeSymbol, namedTypeSymbol2, namedTypeSymbol)
                        {
                            Priority = (flag ? 11 : 13)
                        });
                        operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.UnderlyingAndEnumSubtraction, enumUnderlyingType, enumType, enumType)
                        {
                            Priority = 4
                        });
                        operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LiftedUnderlyingAndEnumSubtraction, namedTypeSymbol2, namedTypeSymbol, namedTypeSymbol)
                        {
                            Priority = 14
                        });
                        break;
                    }
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThanOrEqual:
                    {
                        NamedTypeSymbol specialType2 = Compilation.GetSpecialType(SpecialType.System_Boolean);
                        operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Enum, enumType, enumType, specialType2));
                        operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Lifted | BinaryOperatorKind.Enum, namedTypeSymbol, namedTypeSymbol, specialType2));
                        break;
                    }
                case BinaryOperatorKind.And:
                case BinaryOperatorKind.Xor:
                case BinaryOperatorKind.Or:
                    operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Enum, enumType, enumType, enumType));
                    operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Lifted | BinaryOperatorKind.Enum, namedTypeSymbol, namedTypeSymbol, namedTypeSymbol));
                    break;
            }
        }

        private void GetPointerArithmeticOperators(BinaryOperatorKind kind, PointerTypeSymbol pointerType, ArrayBuilder<BinaryOperatorSignature> operators)
        {
            switch (kind)
            {
                case BinaryOperatorKind.Addition:
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndIntAddition, pointerType, Compilation.GetSpecialType(SpecialType.System_Int32), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndUIntAddition, pointerType, Compilation.GetSpecialType(SpecialType.System_UInt32), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndLongAddition, pointerType, Compilation.GetSpecialType(SpecialType.System_Int64), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndULongAddition, pointerType, Compilation.GetSpecialType(SpecialType.System_UInt64), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.IntAndPointerAddition, Compilation.GetSpecialType(SpecialType.System_Int32), pointerType, pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.UIntAndPointerAddition, Compilation.GetSpecialType(SpecialType.System_UInt32), pointerType, pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.LongAndPointerAddition, Compilation.GetSpecialType(SpecialType.System_Int64), pointerType, pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.ULongAndPointerAddition, Compilation.GetSpecialType(SpecialType.System_UInt64), pointerType, pointerType));
                    break;
                case BinaryOperatorKind.Subtraction:
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndIntSubtraction, pointerType, Compilation.GetSpecialType(SpecialType.System_Int32), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndUIntSubtraction, pointerType, Compilation.GetSpecialType(SpecialType.System_UInt32), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndLongSubtraction, pointerType, Compilation.GetSpecialType(SpecialType.System_Int64), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerAndULongSubtraction, pointerType, Compilation.GetSpecialType(SpecialType.System_UInt64), pointerType));
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.PointerSubtraction, pointerType, pointerType, Compilation.GetSpecialType(SpecialType.System_Int64)));
                    break;
            }
        }

        private void GetPointerComparisonOperators(BinaryOperatorKind kind, ArrayBuilder<BinaryOperatorSignature> operators)
        {
            switch (kind)
            {
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThanOrEqual:
                    {
                        PointerTypeSymbol pointerTypeSymbol = new PointerTypeSymbol(TypeWithAnnotations.Create(Compilation.GetSpecialType(SpecialType.System_Void)));
                        operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Pointer, pointerTypeSymbol, pointerTypeSymbol, Compilation.GetSpecialType(SpecialType.System_Boolean)));
                        break;
                    }
            }
        }

        private void GetEnumOperations(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorSignature> results)
        {
            switch (kind)
            {
                case BinaryOperatorKind.Multiplication:
                case BinaryOperatorKind.Division:
                case BinaryOperatorKind.Remainder:
                case BinaryOperatorKind.LeftShift:
                case BinaryOperatorKind.RightShift:
                case BinaryOperatorKind.LogicalAnd:
                case BinaryOperatorKind.LogicalOr:
                    return;
            }
            TypeSymbol typeSymbol = left.Type;
            if ((object)typeSymbol != null)
            {
                typeSymbol = typeSymbol.StrippedType();
            }
            TypeSymbol typeSymbol2 = right.Type;
            if ((object)typeSymbol2 != null)
            {
                typeSymbol2 = typeSymbol2.StrippedType();
            }
            bool flag;
            switch (kind)
            {
                case BinaryOperatorKind.And:
                case BinaryOperatorKind.Xor:
                case BinaryOperatorKind.Or:
                    flag = false;
                    break;
                case BinaryOperatorKind.Addition:
                    flag = true;
                    break;
                case BinaryOperatorKind.Subtraction:
                    flag = true;
                    break;
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThanOrEqual:
                    flag = true;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
            if ((object)typeSymbol != null)
            {
                GetEnumOperation(kind, typeSymbol, left, right, results);
            }
            if ((object)typeSymbol2 != null && ((object)typeSymbol == null || !(flag ? ConversionsBase.HasIdentityConversion(typeSymbol2, typeSymbol) : typeSymbol2.Equals(typeSymbol))))
            {
                GetEnumOperation(kind, typeSymbol2, left, right, results);
            }
        }

        private void GetPointerOperators(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorSignature> results)
        {
            PointerTypeSymbol pointerTypeSymbol = left.Type as PointerTypeSymbol;
            PointerTypeSymbol pointerTypeSymbol2 = right.Type as PointerTypeSymbol;
            if ((object)pointerTypeSymbol != null)
            {
                GetPointerArithmeticOperators(kind, pointerTypeSymbol, results);
            }
            if ((object)pointerTypeSymbol2 != null && ((object)pointerTypeSymbol == null || !ConversionsBase.HasIdentityConversion(pointerTypeSymbol2, pointerTypeSymbol)))
            {
                GetPointerArithmeticOperators(kind, pointerTypeSymbol2, results);
            }
            if ((object)pointerTypeSymbol != null || (object)pointerTypeSymbol2 != null || left.Type is FunctionPointerTypeSymbol || right.Type is FunctionPointerTypeSymbol)
            {
                GetPointerComparisonOperators(kind, results);
            }
        }

        private void GetAllBuiltInOperators(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorAnalysisResult> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            kind = kind.OperatorWithLogical();
            ArrayBuilder<BinaryOperatorSignature> instance = ArrayBuilder<BinaryOperatorSignature>.GetInstance();
            if ((kind == BinaryOperatorKind.Equal || kind == BinaryOperatorKind.NotEqual) && useOnlyReferenceEquality(Conversions, left, right, ref useSiteInfo))
            {
                GetReferenceEquality(kind, instance);
            }
            else
            {
                Compilation.builtInOperators.GetSimpleBuiltInOperators(kind, instance, !left.Type.IsNativeIntegerOrNullableNativeIntegerType() && !right.Type.IsNativeIntegerOrNullableNativeIntegerType());
                GetDelegateOperations(kind, left, right, instance, ref useSiteInfo);
                GetEnumOperations(kind, left, right, instance);
                GetPointerOperators(kind, left, right, instance);
            }
            CandidateOperators(instance, left, right, results, ref useSiteInfo);
            instance.Free();
            static bool useOnlyReferenceEquality(Conversions conversions, BoundExpression left, BoundExpression right, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
            {
                if (BuiltInOperators.IsValidObjectEquality(conversions, left.Type, left.IsLiteralNull(), leftIsDefault: false, right.Type, right.IsLiteralNull(), rightIsDefault: false, ref useSiteInfo) && ((object)left.Type == null || (!left.Type.IsDelegateType() && left.Type!.SpecialType != SpecialType.System_String && left.Type!.SpecialType != SpecialType.System_Delegate)))
                {
                    if ((object)right.Type != null)
                    {
                        if (!right.Type.IsDelegateType() && right.Type!.SpecialType != SpecialType.System_String)
                        {
                            return right.Type!.SpecialType != SpecialType.System_Delegate;
                        }
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        private void GetReferenceEquality(BinaryOperatorKind kind, ArrayBuilder<BinaryOperatorSignature> operators)
        {
            NamedTypeSymbol specialType = Compilation.GetSpecialType(SpecialType.System_Object);
            operators.Add(new BinaryOperatorSignature(kind | BinaryOperatorKind.Object, specialType, specialType, Compilation.GetSpecialType(SpecialType.System_Boolean)));
        }

        private bool CandidateOperators(ArrayBuilder<BinaryOperatorSignature> operators, BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorAnalysisResult> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            bool result = false;
            ArrayBuilder<BinaryOperatorSignature>.Enumerator enumerator = operators.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BinaryOperatorSignature current = enumerator.Current;
                Conversion leftConversion = Conversions.ClassifyConversionFromExpression(left, current.LeftType, ref useSiteInfo);
                Conversion rightConversion = Conversions.ClassifyConversionFromExpression(right, current.RightType, ref useSiteInfo);
                if (leftConversion.IsImplicit && rightConversion.IsImplicit)
                {
                    results.Add(BinaryOperatorAnalysisResult.Applicable(current, leftConversion, rightConversion));
                    result = true;
                }
                else
                {
                    results.Add(BinaryOperatorAnalysisResult.Inapplicable(current, leftConversion, rightConversion));
                }
            }
            return result;
        }

        private static void AddDistinctOperators(ArrayBuilder<BinaryOperatorAnalysisResult> result, ArrayBuilder<BinaryOperatorAnalysisResult> additionalOperators)
        {
            int count = result.Count;
            ArrayBuilder<BinaryOperatorAnalysisResult>.Enumerator enumerator = additionalOperators.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BinaryOperatorAnalysisResult current = enumerator.Current;
                bool flag = false;
                for (int i = 0; i < count; i++)
                {
                    BinaryOperatorSignature signature = result[i].Signature;
                    if (current.Signature.Kind == signature.Kind && equalsIgnoringNullable(current.Signature.ReturnType, signature.ReturnType) && equalsIgnoringNullableAndDynamic(current.Signature.LeftType, signature.LeftType) && equalsIgnoringNullableAndDynamic(current.Signature.RightType, signature.RightType) && equalsIgnoringNullableAndDynamic(current.Signature.Method.ContainingType, signature.Method.ContainingType))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    result.Add(current);
                }
            }
            static bool equalsIgnoringNullable(TypeSymbol a, TypeSymbol b)
            {
                return a.Equals(b, TypeCompareKind.AllNullableIgnoreOptions);
            }
            static bool equalsIgnoringNullableAndDynamic(TypeSymbol a, TypeSymbol b)
            {
                return a.Equals(b, TypeCompareKind.AllNullableIgnoreOptions | TypeCompareKind.IgnoreDynamic);
            }
        }

        private bool GetUserDefinedOperators(BinaryOperatorKind kind, TypeSymbol type0, BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorAnalysisResult> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)type0 == null || OperatorFacts.DefinitelyHasNoUserDefinedOperators(type0))
            {
                return false;
            }
            string name = OperatorFacts.BinaryOperatorNameFromOperatorKind(kind);
            ArrayBuilder<BinaryOperatorSignature> instance = ArrayBuilder<BinaryOperatorSignature>.GetInstance();
            bool result = false;
            NamedTypeSymbol namedTypeSymbol = type0 as NamedTypeSymbol;
            if ((object)namedTypeSymbol == null)
            {
                namedTypeSymbol = type0.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            if ((object)namedTypeSymbol == null && type0.IsTypeParameter())
            {
                namedTypeSymbol = ((TypeParameterSymbol)type0).EffectiveBaseClass(ref useSiteInfo);
            }
            while ((object)namedTypeSymbol != null)
            {
                instance.Clear();
                GetUserDefinedBinaryOperatorsFromType(namedTypeSymbol, kind, name, instance);
                results.Clear();
                if (CandidateOperators(instance, left, right, results, ref useSiteInfo))
                {
                    result = true;
                    break;
                }
                namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            instance.Free();
            return result;
        }

        private void GetUserDefinedBinaryOperatorsFromType(NamedTypeSymbol type, BinaryOperatorKind kind, string name, ArrayBuilder<BinaryOperatorSignature> operators)
        {
            ImmutableArray<MethodSymbol>.Enumerator enumerator = type.GetOperators(name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                if (current.ParameterCount == 2 && !current.ReturnsVoid)
                {
                    TypeSymbol parameterType = current.GetParameterType(0);
                    TypeSymbol parameterType2 = current.GetParameterType(1);
                    TypeSymbol returnType = current.ReturnType;
                    operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.UserDefined | kind, parameterType, parameterType2, returnType, current));
                    switch (UserDefinedBinaryOperatorCanBeLifted(parameterType, parameterType2, returnType, kind))
                    {
                        case LiftingResult.LiftOperandsAndResult:
                            operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.UserDefined | BinaryOperatorKind.Lifted | kind, MakeNullable(parameterType), MakeNullable(parameterType2), MakeNullable(returnType), current));
                            break;
                        case LiftingResult.LiftOperandsButNotResult:
                            operators.Add(new BinaryOperatorSignature(BinaryOperatorKind.UserDefined | BinaryOperatorKind.Lifted | kind, MakeNullable(parameterType), MakeNullable(parameterType2), returnType, current));
                            break;
                    }
                }
            }
        }

        private static LiftingResult UserDefinedBinaryOperatorCanBeLifted(TypeSymbol left, TypeSymbol right, TypeSymbol result, BinaryOperatorKind kind)
        {
            // SPEC: For the binary operators + - * / % & | ^ << >> a lifted form of the
            // SPEC: operator exists if the operand and result types are all non-nullable
            // SPEC: value types. The lifted form is constructed by adding a single ?
            // SPEC: modifier to each operand and result type. 
            //
            // SPEC: For the equality operators == != a lifted form of the operator exists
            // SPEC: if the operand types are both non-nullable value types and if the 
            // SPEC: result type is bool. The lifted form is constructed by adding
            // SPEC: a single ? modifier to each operand type.
            //
            // SPEC: For the relational operators > < >= <= a lifted form of the 
            // SPEC: operator exists if the operand types are both non-nullable value
            // SPEC: types and if the result type is bool. The lifted form is 
            // SPEC: constructed by adding a single ? modifier to each operand type.

            if (!left.IsValueType ||
                left.IsNullableType() ||
                !right.IsValueType ||
                right.IsNullableType())
            {
                return LiftingResult.NotLifted;
            }

            switch (kind)
            {
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                    // Spec violation: can't lift unless the types match.
                    // The spec doesn't require this, but dev11 does and it reduces ambiguity in some cases.
                    if (!TypeSymbol.Equals(left, right, TypeCompareKind.ConsiderEverything2)) return LiftingResult.NotLifted;
                    goto case BinaryOperatorKind.GreaterThan;
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.LessThanOrEqual:
                    return result.SpecialType == SpecialType.System_Boolean ?
                        LiftingResult.LiftOperandsButNotResult :
                        LiftingResult.NotLifted;
                default:
                    return result.IsValueType && !result.IsNullableType() ?
                        LiftingResult.LiftOperandsAndResult :
                        LiftingResult.NotLifted;
            }
        }

        private void BinaryOperatorOverloadResolution(BoundExpression left, BoundExpression right, BinaryOperatorOverloadResolutionResult result, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (result.SingleValid())
            {
                return;
            }
            ArrayBuilder<BinaryOperatorAnalysisResult> results = result.Results;
            int theBestCandidateIndex = GetTheBestCandidateIndex(left, right, results, ref useSiteInfo);
            if (theBestCandidateIndex != -1)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].Kind != OperatorAnalysisResultKind.Inapplicable && i != theBestCandidateIndex)
                    {
                        results[i] = results[i].Worse();
                    }
                }
                return;
            }
            for (int j = 1; j < results.Count; j++)
            {
                if (results[j].Kind != OperatorAnalysisResultKind.Applicable)
                {
                    continue;
                }
                for (int k = 0; k < j; k++)
                {
                    if (results[k].Kind != OperatorAnalysisResultKind.Inapplicable)
                    {
                        switch (BetterOperator(results[j].Signature, results[k].Signature, left, right, ref useSiteInfo))
                        {
                            case BetterResult.Left:
                                results[k] = results[k].Worse();
                                break;
                            case BetterResult.Right:
                                results[j] = results[j].Worse();
                                break;
                        }
                    }
                }
            }
        }

        private int GetTheBestCandidateIndex(BoundExpression left, BoundExpression right, ArrayBuilder<BinaryOperatorAnalysisResult> candidates, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            int num = -1;
            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].Kind != OperatorAnalysisResultKind.Applicable)
                {
                    continue;
                }
                if (num == -1)
                {
                    num = i;
                    continue;
                }
                switch (BetterOperator(candidates[num].Signature, candidates[i].Signature, left, right, ref useSiteInfo))
                {
                    case BetterResult.Right:
                        num = i;
                        break;
                    default:
                        num = -1;
                        break;
                    case BetterResult.Left:
                        break;
                }
            }
            for (int j = 0; j < num; j++)
            {
                if (candidates[j].Kind != OperatorAnalysisResultKind.Inapplicable && BetterOperator(candidates[num].Signature, candidates[j].Signature, left, right, ref useSiteInfo) != 0)
                {
                    return -1;
                }
            }
            return num;
        }

        private BetterResult BetterOperator(BinaryOperatorSignature op1, BinaryOperatorSignature op2, BoundExpression left, BoundExpression right, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (op1.Priority.HasValue && op1.Priority.GetValueOrDefault() != op2.Priority.GetValueOrDefault())
            {
                if (op1.Priority.GetValueOrDefault() >= op2.Priority.GetValueOrDefault())
                {
                    return BetterResult.Right;
                }
                return BetterResult.Left;
            }
            BetterResult betterResult = BetterConversionFromExpression(left, op1.LeftType, op2.LeftType, ref useSiteInfo);
            BetterResult betterResult2 = BetterConversionFromExpression(right, op1.RightType, op2.RightType, ref useSiteInfo);
            if ((betterResult == BetterResult.Left && betterResult2 != BetterResult.Right) || (betterResult != BetterResult.Right && betterResult2 == BetterResult.Left))
            {
                return BetterResult.Left;
            }
            if ((betterResult == BetterResult.Right && betterResult2 != 0) || (betterResult != 0 && betterResult2 == BetterResult.Right))
            {
                return BetterResult.Right;
            }
            if (ConversionsBase.HasIdentityConversion(op1.LeftType, op2.LeftType) && ConversionsBase.HasIdentityConversion(op1.RightType, op2.RightType))
            {
                BetterResult betterResult3 = MoreSpecificOperator(op1, op2, ref useSiteInfo);
                if (betterResult3 == BetterResult.Left || betterResult3 == BetterResult.Right)
                {
                    return betterResult3;
                }
                bool flag = op1.Kind.IsLifted();
                bool flag2 = op2.Kind.IsLifted();
                if (flag && !flag2)
                {
                    return BetterResult.Right;
                }
                if (!flag && flag2)
                {
                    return BetterResult.Left;
                }
            }
            BetterResult betterResult4 = ((op1.LeftRefKind != 0 || op2.LeftRefKind != RefKind.In) ? ((op2.LeftRefKind == RefKind.None && op1.LeftRefKind == RefKind.In) ? BetterResult.Right : BetterResult.Neither) : BetterResult.Left);
            if (op1.RightRefKind == RefKind.None && op2.RightRefKind == RefKind.In)
            {
                if (betterResult4 == BetterResult.Right)
                {
                    return BetterResult.Neither;
                }
                betterResult4 = BetterResult.Left;
            }
            else if (op2.RightRefKind == RefKind.None && op1.RightRefKind == RefKind.In)
            {
                if (betterResult4 == BetterResult.Left)
                {
                    return BetterResult.Neither;
                }
                betterResult4 = BetterResult.Right;
            }
            return betterResult4;
        }

        private BetterResult MoreSpecificOperator(BinaryOperatorSignature op1, BinaryOperatorSignature op2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeSymbol typeSymbol;
            TypeSymbol typeSymbol2;
            if ((object)op1.Method != null)
            {
                ImmutableArray<ParameterSymbol> parameters = op1.Method.OriginalDefinition.GetParameters();
                typeSymbol = parameters[0].Type;
                typeSymbol2 = parameters[1].Type;
                if (op1.Kind.IsLifted())
                {
                    typeSymbol = MakeNullable(typeSymbol);
                    typeSymbol2 = MakeNullable(typeSymbol2);
                }
            }
            else
            {
                typeSymbol = op1.LeftType;
                typeSymbol2 = op1.RightType;
            }
            TypeSymbol typeSymbol3;
            TypeSymbol typeSymbol4;
            if ((object)op2.Method != null)
            {
                ImmutableArray<ParameterSymbol> parameters2 = op2.Method.OriginalDefinition.GetParameters();
                typeSymbol3 = parameters2[0].Type;
                typeSymbol4 = parameters2[1].Type;
                if (op2.Kind.IsLifted())
                {
                    typeSymbol3 = MakeNullable(typeSymbol3);
                    typeSymbol4 = MakeNullable(typeSymbol4);
                }
            }
            else
            {
                typeSymbol3 = op2.LeftType;
                typeSymbol4 = op2.RightType;
            }
            ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
            ArrayBuilder<TypeSymbol> instance2 = ArrayBuilder<TypeSymbol>.GetInstance();
            instance.Add(typeSymbol);
            instance.Add(typeSymbol2);
            instance2.Add(typeSymbol3);
            instance2.Add(typeSymbol4);
            BetterResult result = MoreSpecificType(instance, instance2, ref useSiteInfo);
            instance.Free();
            instance2.Free();
            return result;
        }

        [Conditional("DEBUG")]
        private static void AssertNotChecked(BinaryOperatorKind kind)
        {
        }

        private void UnaryOperatorEasyOut(UnaryOperatorKind kind, BoundExpression operand, UnaryOperatorOverloadResolutionResult result)
        {
            TypeSymbol type = operand.Type;
            if ((object)type != null)
            {
                UnaryOperatorKind unaryOperatorKind = UnopEasyOut.OpKind(kind, type);
                if (unaryOperatorKind != 0)
                {
                    UnaryOperatorSignature signature = Compilation.builtInOperators.GetSignature(unaryOperatorKind);
                    Conversion? conversion = ConversionsBase.FastClassifyConversion(type, signature.OperandType);
                    result.Results.Add(UnaryOperatorAnalysisResult.Applicable(signature, conversion.Value));
                }
            }
        }

        private NamedTypeSymbol MakeNullable(TypeSymbol type)
        {
            return Compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(type);
        }

        public void UnaryOperatorOverloadResolution(UnaryOperatorKind kind, BoundExpression operand, UnaryOperatorOverloadResolutionResult result, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            UnaryOperatorEasyOut(kind, operand, result);
            if (result.Results.Count <= 0)
            {
                if (!GetUserDefinedOperators(kind, operand, result.Results, ref useSiteInfo))
                {
                    result.Results.Clear();
                    GetAllBuiltInOperators(kind, operand, result.Results, ref useSiteInfo);
                }
                UnaryOperatorOverloadResolution(operand, result, ref useSiteInfo);
            }
        }

        private void UnaryOperatorOverloadResolution(BoundExpression operand, UnaryOperatorOverloadResolutionResult result, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (result.SingleValid())
            {
                return;
            }
            ArrayBuilder<UnaryOperatorAnalysisResult> results = result.Results;
            int theBestCandidateIndex = GetTheBestCandidateIndex(operand, results, ref useSiteInfo);
            if (theBestCandidateIndex != -1)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].Kind != OperatorAnalysisResultKind.Inapplicable && i != theBestCandidateIndex)
                    {
                        results[i] = results[i].Worse();
                    }
                }
                return;
            }
            for (int j = 1; j < results.Count; j++)
            {
                if (results[j].Kind != OperatorAnalysisResultKind.Applicable)
                {
                    continue;
                }
                for (int k = 0; k < j; k++)
                {
                    if (results[k].Kind != OperatorAnalysisResultKind.Inapplicable)
                    {
                        switch (BetterOperator(results[j].Signature, results[k].Signature, operand, ref useSiteInfo))
                        {
                            case BetterResult.Left:
                                results[k] = results[k].Worse();
                                break;
                            case BetterResult.Right:
                                results[j] = results[j].Worse();
                                break;
                        }
                    }
                }
            }
        }

        private int GetTheBestCandidateIndex(BoundExpression operand, ArrayBuilder<UnaryOperatorAnalysisResult> candidates, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            int num = -1;
            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].Kind != OperatorAnalysisResultKind.Applicable)
                {
                    continue;
                }
                if (num == -1)
                {
                    num = i;
                    continue;
                }
                switch (BetterOperator(candidates[num].Signature, candidates[i].Signature, operand, ref useSiteInfo))
                {
                    case BetterResult.Right:
                        num = i;
                        break;
                    default:
                        num = -1;
                        break;
                    case BetterResult.Left:
                        break;
                }
            }
            for (int j = 0; j < num; j++)
            {
                if (candidates[j].Kind != OperatorAnalysisResultKind.Inapplicable && BetterOperator(candidates[num].Signature, candidates[j].Signature, operand, ref useSiteInfo) != 0)
                {
                    return -1;
                }
            }
            return num;
        }

        private BetterResult BetterOperator(UnaryOperatorSignature op1, UnaryOperatorSignature op2, BoundExpression operand, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            BetterResult betterResult = BetterConversionFromExpression(operand, op1.OperandType, op2.OperandType, ref useSiteInfo);
            if (betterResult == BetterResult.Left || betterResult == BetterResult.Right)
            {
                return betterResult;
            }
            if (ConversionsBase.HasIdentityConversion(op1.OperandType, op2.OperandType))
            {
                bool flag = op1.Kind.IsLifted();
                bool flag2 = op2.Kind.IsLifted();
                if (flag && !flag2)
                {
                    return BetterResult.Right;
                }
                if (!flag && flag2)
                {
                    return BetterResult.Left;
                }
            }
            if (op1.RefKind == RefKind.None && op2.RefKind == RefKind.In)
            {
                return BetterResult.Left;
            }
            if (op2.RefKind == RefKind.None && op1.RefKind == RefKind.In)
            {
                return BetterResult.Right;
            }
            return BetterResult.Neither;
        }

        private void GetAllBuiltInOperators(UnaryOperatorKind kind, BoundExpression operand, ArrayBuilder<UnaryOperatorAnalysisResult> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<UnaryOperatorSignature> instance = ArrayBuilder<UnaryOperatorSignature>.GetInstance();
            Compilation.builtInOperators.GetSimpleBuiltInOperators(kind, instance, !operand.Type.IsNativeIntegerOrNullableNativeIntegerType());
            GetEnumOperations(kind, operand, instance);
            UnaryOperatorSignature? pointerOperation = GetPointerOperation(kind, operand);
            if (pointerOperation.HasValue)
            {
                instance.Add(pointerOperation.Value);
            }
            CandidateOperators(instance, operand, results, ref useSiteInfo);
            instance.Free();
        }

        private bool CandidateOperators(ArrayBuilder<UnaryOperatorSignature> operators, BoundExpression operand, ArrayBuilder<UnaryOperatorAnalysisResult> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            bool result = false;
            ArrayBuilder<UnaryOperatorSignature>.Enumerator enumerator = operators.GetEnumerator();
            while (enumerator.MoveNext())
            {
                UnaryOperatorSignature current = enumerator.Current;
                Conversion conversion = Conversions.ClassifyConversionFromExpression(operand, current.OperandType, ref useSiteInfo);
                if (conversion.IsImplicit)
                {
                    result = true;
                    results.Add(UnaryOperatorAnalysisResult.Applicable(current, conversion));
                }
                else
                {
                    results.Add(UnaryOperatorAnalysisResult.Inapplicable(current, conversion));
                }
            }
            return result;
        }

        private void GetEnumOperations(UnaryOperatorKind kind, BoundExpression operand, ArrayBuilder<UnaryOperatorSignature> operators)
        {
            TypeSymbol type = operand.Type;
            if ((object)type == null)
            {
                return;
            }
            type = type.StrippedType();
            if (type.IsValidEnumType())
            {
                NamedTypeSymbol namedTypeSymbol = MakeNullable(type);
                switch (kind)
                {
                    case UnaryOperatorKind.PostfixIncrement:
                    case UnaryOperatorKind.PostfixDecrement:
                    case UnaryOperatorKind.PrefixIncrement:
                    case UnaryOperatorKind.PrefixDecrement:
                    case UnaryOperatorKind.BitwiseComplement:
                        operators.Add(new UnaryOperatorSignature(kind | UnaryOperatorKind.Enum, type, type));
                        operators.Add(new UnaryOperatorSignature(kind | UnaryOperatorKind.Lifted | UnaryOperatorKind.Enum, namedTypeSymbol, namedTypeSymbol));
                        break;
                }
            }
        }

        private static UnaryOperatorSignature? GetPointerOperation(UnaryOperatorKind kind, BoundExpression operand)
        {
            if (!(operand.Type is PointerTypeSymbol pointerTypeSymbol))
            {
                return null;
            }
            UnaryOperatorSignature? result = null;
            switch (kind)
            {
                case UnaryOperatorKind.PostfixIncrement:
                case UnaryOperatorKind.PostfixDecrement:
                case UnaryOperatorKind.PrefixIncrement:
                case UnaryOperatorKind.PrefixDecrement:
                    result = new UnaryOperatorSignature(kind | UnaryOperatorKind.Pointer, pointerTypeSymbol, pointerTypeSymbol);
                    break;
            }
            return result;
        }

        private bool GetUserDefinedOperators(UnaryOperatorKind kind, BoundExpression operand, ArrayBuilder<UnaryOperatorAnalysisResult> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)operand.Type == null)
            {
                return false;
            }
            TypeSymbol typeSymbol = operand.Type.StrippedType();
            if (OperatorFacts.DefinitelyHasNoUserDefinedOperators(typeSymbol))
            {
                return false;
            }
            string name = OperatorFacts.UnaryOperatorNameFromOperatorKind(kind);
            ArrayBuilder<UnaryOperatorSignature> instance = ArrayBuilder<UnaryOperatorSignature>.GetInstance();
            bool flag = false;
            NamedTypeSymbol namedTypeSymbol = typeSymbol as NamedTypeSymbol;
            if ((object)namedTypeSymbol == null)
            {
                namedTypeSymbol = typeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            if ((object)namedTypeSymbol == null && typeSymbol.IsTypeParameter())
            {
                namedTypeSymbol = ((TypeParameterSymbol)typeSymbol).EffectiveBaseClass(ref useSiteInfo);
            }
            while ((object)namedTypeSymbol != null)
            {
                instance.Clear();
                GetUserDefinedUnaryOperatorsFromType(namedTypeSymbol, kind, name, instance);
                results.Clear();
                if (CandidateOperators(instance, operand, results, ref useSiteInfo))
                {
                    flag = true;
                    break;
                }
                namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            if (!flag)
            {
                ImmutableArray<NamedTypeSymbol> immutableArray = default(ImmutableArray<NamedTypeSymbol>);
                if (typeSymbol.IsInterfaceType())
                {
                    immutableArray = typeSymbol.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                }
                else if (typeSymbol.IsTypeParameter())
                {
                    immutableArray = ((TypeParameterSymbol)typeSymbol).AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                }
                if (!immutableArray.IsDefaultOrEmpty)
                {
                    PooledHashSet<NamedTypeSymbol> instance2 = PooledHashSet<NamedTypeSymbol>.GetInstance();
                    ArrayBuilder<UnaryOperatorAnalysisResult> instance3 = ArrayBuilder<UnaryOperatorAnalysisResult>.GetInstance();
                    results.Clear();
                    ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        NamedTypeSymbol current = enumerator.Current;
                        if (current.IsInterface && !instance2.Contains(current))
                        {
                            instance.Clear();
                            instance3.Clear();
                            GetUserDefinedUnaryOperatorsFromType(current, kind, name, instance);
                            if (CandidateOperators(instance, operand, instance3, ref useSiteInfo))
                            {
                                flag = true;
                                results.AddRange(instance3);
                                instance2.AddAll(current.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo));
                            }
                        }
                    }
                    instance2.Free();
                    instance3.Free();
                }
            }
            instance.Free();
            return flag;
        }

        private void GetUserDefinedUnaryOperatorsFromType(NamedTypeSymbol type, UnaryOperatorKind kind, string name, ArrayBuilder<UnaryOperatorSignature> operators)
        {
            ImmutableArray<MethodSymbol>.Enumerator enumerator = type.GetOperators(name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                if (current.ParameterCount != 1 || current.ReturnsVoid)
                {
                    continue;
                }
                TypeSymbol parameterType = current.GetParameterType(0);
                TypeSymbol returnType = current.ReturnType;
                operators.Add(new UnaryOperatorSignature(UnaryOperatorKind.UserDefined | kind, parameterType, returnType, current));
                switch (kind)
                {
                    case UnaryOperatorKind.PostfixIncrement:
                    case UnaryOperatorKind.PostfixDecrement:
                    case UnaryOperatorKind.PrefixIncrement:
                    case UnaryOperatorKind.PrefixDecrement:
                    case UnaryOperatorKind.UnaryPlus:
                    case UnaryOperatorKind.UnaryMinus:
                    case UnaryOperatorKind.LogicalNegation:
                    case UnaryOperatorKind.BitwiseComplement:
                        if (parameterType.IsValueType && !parameterType.IsNullableType() && returnType.IsValueType && !returnType.IsNullableType())
                        {
                            operators.Add(new UnaryOperatorSignature(UnaryOperatorKind.UserDefined | UnaryOperatorKind.Lifted | kind, MakeNullable(parameterType), MakeNullable(returnType), current));
                        }
                        break;
                }
            }
        }

        public OverloadResolution(Binder binder)
        {
            _binder = binder;
        }

        private static bool AnyValidResult<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results) where TMember : Symbol
        {
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = results.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsValid)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SingleValidResult<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results) where TMember : Symbol
        {
            bool flag = false;
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = results.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsValid)
                {
                    if (flag)
                    {
                        return false;
                    }
                    flag = true;
                }
            }
            return flag;
        }

        public void ObjectCreationOverloadResolution(ImmutableArray<MethodSymbol> constructors, AnalyzedArguments arguments, OverloadResolutionResult<MethodSymbol> result, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<MemberResolutionResult<MethodSymbol>> resultsBuilder = result.ResultsBuilder;
            PerformObjectCreationOverloadResolution(resultsBuilder, constructors, arguments, completeResults: false, ref useSiteInfo);
            if (!OverloadResolutionResultIsValid(resultsBuilder, arguments.HasDynamicArgument))
            {
                result.Clear();
                PerformObjectCreationOverloadResolution(resultsBuilder, constructors, arguments, completeResults: true, ref useSiteInfo);
            }
        }

        public void MethodInvocationOverloadResolution(ArrayBuilder<MethodSymbol> methods, ArrayBuilder<TypeWithAnnotations> typeArguments, BoundExpression receiver, AnalyzedArguments arguments, OverloadResolutionResult<MethodSymbol> result, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool isMethodGroupConversion = false, bool allowRefOmittedArguments = false, bool inferWithDynamic = false, bool allowUnexpandedForm = true, RefKind returnRefKind = RefKind.None, TypeSymbol returnType = null, bool isFunctionPointerResolution = false, in CallingConventionInfo callingConventionInfo = default(CallingConventionInfo))
        {
            MethodOrPropertyOverloadResolution(methods, typeArguments, receiver, arguments, result, isMethodGroupConversion, allowRefOmittedArguments, ref useSiteInfo, inferWithDynamic, allowUnexpandedForm, returnRefKind, returnType, isFunctionPointerResolution, in callingConventionInfo);
        }

        public void PropertyOverloadResolution(ArrayBuilder<PropertySymbol> indexers, BoundExpression receiverOpt, AnalyzedArguments arguments, OverloadResolutionResult<PropertySymbol> result, bool allowRefOmittedArguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            CallingConventionInfo callingConventionInfo = default(CallingConventionInfo);
            MethodOrPropertyOverloadResolution(indexers, instance, receiverOpt, arguments, result, isMethodGroupConversion: false, allowRefOmittedArguments, ref useSiteInfo, inferWithDynamic: false, allowUnexpandedForm: true, RefKind.None, null, isFunctionPointerResolution: false, in callingConventionInfo);
            instance.Free();
        }

        internal void MethodOrPropertyOverloadResolution<TMember>(ArrayBuilder<TMember> members, ArrayBuilder<TypeWithAnnotations> typeArguments, BoundExpression receiver, AnalyzedArguments arguments, OverloadResolutionResult<TMember> result, bool isMethodGroupConversion, bool allowRefOmittedArguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool inferWithDynamic = false, bool allowUnexpandedForm = true, RefKind returnRefKind = RefKind.None, TypeSymbol returnType = null, bool isFunctionPointerResolution = false, in CallingConventionInfo callingConventionInfo = default(CallingConventionInfo)) where TMember : Symbol
        {
            ArrayBuilder<MemberResolutionResult<TMember>> resultsBuilder = result.ResultsBuilder;
            PerformMemberOverloadResolution(resultsBuilder, members, typeArguments, receiver, arguments, completeResults: false, isMethodGroupConversion, returnRefKind, returnType, allowRefOmittedArguments, isFunctionPointerResolution, in callingConventionInfo, ref useSiteInfo, inferWithDynamic, allowUnexpandedForm);
            if (!OverloadResolutionResultIsValid(resultsBuilder, arguments.HasDynamicArgument))
            {
                result.Clear();
                PerformMemberOverloadResolution(resultsBuilder, members, typeArguments, receiver, arguments, completeResults: true, isMethodGroupConversion, returnRefKind, returnType, allowRefOmittedArguments, isFunctionPointerResolution, in callingConventionInfo, ref useSiteInfo, inferWithDynamic: false, allowUnexpandedForm);
            }
        }

        private static bool OverloadResolutionResultIsValid<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, bool hasDynamicArgument) where TMember : Symbol
        {
            if (hasDynamicArgument)
            {
                ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = results.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Result.IsApplicable)
                    {
                        return true;
                    }
                }
                return false;
            }
            return SingleValidResult(results);
        }

        private void PerformMemberOverloadResolution<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, ArrayBuilder<TMember> members, ArrayBuilder<TypeWithAnnotations> typeArguments, BoundExpression receiver, AnalyzedArguments arguments, bool completeResults, bool isMethodGroupConversion, RefKind returnRefKind, TypeSymbol returnType, bool allowRefOmittedArguments, bool isFunctionPointerResolution, in CallingConventionInfo callingConventionInfo, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool inferWithDynamic = false, bool allowUnexpandedForm = true) where TMember : Symbol
        {
            Dictionary<NamedTypeSymbol, ArrayBuilder<TMember>> containingTypeMapOpt = null;
            if (members.Count > 50)
            {
                containingTypeMapOpt = PartitionMembersByContainingType(members);
            }
            for (int i = 0; i < members.Count; i++)
            {
                AddMemberToCandidateSet(members[i], results, members, typeArguments, receiver, arguments, completeResults, isMethodGroupConversion, allowRefOmittedArguments, containingTypeMapOpt, inferWithDynamic, ref useSiteInfo, allowUnexpandedForm);
            }
            ClearContainingTypeMap(ref containingTypeMapOpt);
            RemoveInaccessibleTypeArguments(results, ref useSiteInfo);
            RemoveLessDerivedMembers(results, ref useSiteInfo);
            if (Compilation.LanguageVersion.AllowImprovedOverloadCandidates())
            {
                RemoveStaticInstanceMismatches(results, arguments, receiver);
                RemoveConstraintViolations(results, new CompoundUseSiteInfo<AssemblySymbol>(useSiteInfo));
                if (isMethodGroupConversion)
                {
                    RemoveDelegateConversionsWithWrongReturnType(results, ref useSiteInfo, returnRefKind, returnType, isFunctionPointerResolution);
                }
            }
            if (isFunctionPointerResolution)
            {
                RemoveCallingConventionMismatches(results, in callingConventionInfo);
                RemoveMethodsNotDeclaredStatic(results);
            }
            ReportUseSiteInfo(results, ref useSiteInfo);
            if (AnyValidResult(results))
            {
                RemoveWorseMembers(results, arguments, ref useSiteInfo);
            }
        }

        internal void FunctionPointerOverloadResolution(ArrayBuilder<FunctionPointerMethodSymbol> funcPtrBuilder, AnalyzedArguments analyzedArguments, OverloadResolutionResult<FunctionPointerMethodSymbol> overloadResolutionResult, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            AddMemberToCandidateSet(funcPtrBuilder[0], overloadResolutionResult.ResultsBuilder, funcPtrBuilder, instance, null, analyzedArguments, completeResults: true, isMethodGroupConversion: false, allowRefOmittedArguments: false, null, inferWithDynamic: false, ref useSiteInfo, allowUnexpandedForm: true);
            ReportUseSiteInfo(overloadResolutionResult.ResultsBuilder, ref useSiteInfo);
        }

        private void RemoveStaticInstanceMismatches<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, AnalyzedArguments arguments, BoundExpression receiverOpt) where TMember : Symbol
        {
            if (!arguments.IsExtensionMethodInvocation && !Binder.IsTypeOrValueExpression(receiverOpt))
            {
                bool flag = Binder.WasImplicitReceiver(receiverOpt);
                bool flag2 = !_binder.HasThis(!flag, out bool inStaticContext) || inStaticContext;
                if (!flag || flag2)
                {
                    bool requireStatic = (flag && flag2) || Binder.IsMemberAccessedThroughType(receiverOpt);
                    RemoveStaticInstanceMismatches(results, requireStatic);
                }
            }
        }

        private static void RemoveStaticInstanceMismatches<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, bool requireStatic) where TMember : Symbol
        {
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                TMember member = memberResolutionResult.Member;
                if (memberResolutionResult.Result.IsValid && member.RequiresInstanceReceiver() == requireStatic)
                {
                    results[i] = new MemberResolutionResult<TMember>(member, memberResolutionResult.LeastOverriddenMember, MemberAnalysisResult.StaticInstanceMismatch());
                }
            }
        }

        private static void RemoveMethodsNotDeclaredStatic<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results) where TMember : Symbol
        {
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                TMember member = memberResolutionResult.Member;
                if (memberResolutionResult.Result.IsValid && !member.IsStatic)
                {
                    results[i] = new MemberResolutionResult<TMember>(member, memberResolutionResult.LeastOverriddenMember, MemberAnalysisResult.StaticInstanceMismatch());
                }
            }
        }

        private void RemoveConstraintViolations<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, CompoundUseSiteInfo<AssemblySymbol> template) where TMember : Symbol
        {
            if (typeof(TMember) != typeof(MethodSymbol))
            {
                return;
            }
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                MethodSymbol method = (MethodSymbol)(object)memberResolutionResult.Member;
                if ((memberResolutionResult.Result.IsValid || memberResolutionResult.Result.Kind == MemberResolutionKind.ConstructedParameterFailedConstraintCheck) && FailsConstraintChecks(method, out var constraintFailureDiagnosticsOpt, template))
                {
                    results[i] = new MemberResolutionResult<TMember>(memberResolutionResult.Member, memberResolutionResult.LeastOverriddenMember, MemberAnalysisResult.ConstraintFailure(constraintFailureDiagnosticsOpt.ToImmutableAndFree()));
                }
            }
        }

#nullable enable
        private void RemoveCallingConventionMismatches<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, in CallingConventionInfo expectedConvention) where TMember : Symbol
        {
            if (typeof(TMember) != typeof(MethodSymbol))
            {
                return;
            }

            Debug.Assert(!expectedConvention.CallKind.HasUnknownCallingConventionAttributeBits());
            Debug.Assert(expectedConvention.UnmanagedCallingConventionTypes is not null);
            Debug.Assert(expectedConvention.UnmanagedCallingConventionTypes.IsEmpty || expectedConvention.CallKind == Cci.CallingConvention.Unmanaged);

            Debug.Assert(!_binder.IsEarlyAttributeBinder);
            if (_binder.InAttributeArgument || (_binder.Flags & BinderFlags.InContextualAttributeBinder) != 0)
            {
                // We're at a location where the unmanaged data might not yet been bound. This cannot be valid code
                // anyway, as attribute arguments can't be method references, so we'll just assume that the conventions
                // match, as there will be other errors that supersede these anyway
                return;
            }

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                var member = (MethodSymbol)(Symbol)result.Member;
                if (result.Result.IsValid)
                {
                    // We're not in an attribute, so cycles shouldn't be possible
                    var unmanagedCallersOnlyData = member.GetUnmanagedCallersOnlyAttributeData(forceComplete: true);

                    Debug.Assert(!ReferenceEquals(unmanagedCallersOnlyData, UnmanagedCallersOnlyAttributeData.AttributePresentDataNotBound)
                                 && !ReferenceEquals(unmanagedCallersOnlyData, UnmanagedCallersOnlyAttributeData.Uninitialized));

                    Cci.CallingConvention actualCallKind;
                    ImmutableHashSet<INamedTypeSymbolInternal> actualUnmanagedCallingConventionTypes;

                    if (unmanagedCallersOnlyData is null)
                    {
                        actualCallKind = member.CallingConvention;
                        actualUnmanagedCallingConventionTypes = ImmutableHashSet<INamedTypeSymbolInternal>.Empty;
                    }
                    else
                    {
                        // There's data from an UnmanagedCallersOnlyAttribute present, which takes precedence over the
                        // CallKind bit in the method definition. We use the following rules to decode the attribute:
                        // * If no types are specified, the CallKind is treated as Unmanaged, with no unmanaged calling convention types
                        // * If there is one type specified, and that type is named CallConvCdecl, CallConvThiscall, CallConvStdcall, or 
                        //   CallConvFastcall, the CallKind is treated as CDecl, ThisCall, Standard, or FastCall, respectively, with no
                        //   calling types.
                        // * If multiple types are specified or the single type is not named one of the specially called out types above,
                        //   the CallKind is treated as Unmanaged, with the union of the types specified treated as calling convention types.

                        var unmanagedCallingConventionTypes = unmanagedCallersOnlyData.CallingConventionTypes;
                        Debug.Assert(unmanagedCallingConventionTypes.All(u => FunctionPointerTypeSymbol.IsCallingConventionModifier((NamedTypeSymbol)u)));

                        switch (unmanagedCallingConventionTypes.Count)
                        {
                            case 0:
                                actualCallKind = Cci.CallingConvention.Unmanaged;
                                actualUnmanagedCallingConventionTypes = ImmutableHashSet<INamedTypeSymbolInternal>.Empty;
                                break;
                            case 1:
                                switch (unmanagedCallingConventionTypes.Single().Name)
                                {
                                    case "CallConvCdecl":
                                        actualCallKind = Cci.CallingConvention.CDecl;
                                        actualUnmanagedCallingConventionTypes = ImmutableHashSet<INamedTypeSymbolInternal>.Empty;
                                        break;
                                    case "CallConvStdcall":
                                        actualCallKind = Cci.CallingConvention.Standard;
                                        actualUnmanagedCallingConventionTypes = ImmutableHashSet<INamedTypeSymbolInternal>.Empty;
                                        break;
                                    case "CallConvThiscall":
                                        actualCallKind = Cci.CallingConvention.ThisCall;
                                        actualUnmanagedCallingConventionTypes = ImmutableHashSet<INamedTypeSymbolInternal>.Empty;
                                        break;
                                    case "CallConvFastcall":
                                        actualCallKind = Cci.CallingConvention.FastCall;
                                        actualUnmanagedCallingConventionTypes = ImmutableHashSet<INamedTypeSymbolInternal>.Empty;
                                        break;
                                    default:
                                        goto outerDefault;
                                }
                                break;

                            default:
                            outerDefault:
                                actualCallKind = Cci.CallingConvention.Unmanaged;
                                actualUnmanagedCallingConventionTypes = unmanagedCallingConventionTypes;
                                break;
                        }
                    }

                    // The rules for matching a calling convention are:
                    // 1. The CallKinds must match exactly
                    // 2. If the CallKind is Unmanaged, then the set of calling convention types must match exactly, ignoring order
                    //    and duplicates. We already have both sets in a HashSet, so we can just ensure they're the same length and
                    //    that everything from one set is in the other set.

                    if (actualCallKind.HasUnknownCallingConventionAttributeBits() || !actualCallKind.IsCallingConvention(expectedConvention.CallKind))
                    {
                        results[i] = makeWrongCallingConvention(result);
                        continue;
                    }

                    if (expectedConvention.CallKind.IsCallingConvention(Cci.CallingConvention.Unmanaged))
                    {
                        if (expectedConvention.UnmanagedCallingConventionTypes.Count != actualUnmanagedCallingConventionTypes.Count)
                        {
                            results[i] = makeWrongCallingConvention(result);
                            continue;
                        }

                        foreach (var expectedModifier in expectedConvention.UnmanagedCallingConventionTypes)
                        {
                            if (!actualUnmanagedCallingConventionTypes.Contains(((CSharpCustomModifier)expectedModifier).ModifierSymbol))
                            {
                                results[i] = makeWrongCallingConvention(result);
                                break;
                            }
                        }
                    }
                }
            }

            static MemberResolutionResult<TMember> makeWrongCallingConvention(MemberResolutionResult<TMember> result)
                => new MemberResolutionResult<TMember>(result.Member, result.LeastOverriddenMember, MemberAnalysisResult.WrongCallingConvention());
        }
#nullable disable

        private bool FailsConstraintChecks(MethodSymbol method, out ArrayBuilder<TypeParameterDiagnosticInfo> constraintFailureDiagnosticsOpt, CompoundUseSiteInfo<AssemblySymbol> template)
        {
            if (method.Arity == 0 || (object)method.OriginalDefinition == method)
            {
                constraintFailureDiagnosticsOpt = null;
                return false;
            }
            ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
            ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
            ConstraintsHelper.CheckConstraintsArgs args = new ConstraintsHelper.CheckConstraintsArgs(Compilation, Conversions, includeNullability: false, NoLocation.Singleton, null, template);
            if (!ConstraintsHelper.CheckMethodConstraints(method, in args, instance, null, ref useSiteDiagnosticsBuilder))
            {
                if (useSiteDiagnosticsBuilder != null)
                {
                    instance.AddRange(useSiteDiagnosticsBuilder);
                    useSiteDiagnosticsBuilder.Free();
                }
                constraintFailureDiagnosticsOpt = instance;
                return true;
            }
            instance.Free();
            useSiteDiagnosticsBuilder?.Free();
            constraintFailureDiagnosticsOpt = null;
            return false;
        }

        private void RemoveDelegateConversionsWithWrongReturnType<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, RefKind? returnRefKind, TypeSymbol returnType, bool isFunctionPointerConversion) where TMember : Symbol
        {
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                if (!memberResolutionResult.Result.IsValid)
                {
                    continue;
                }
                MethodSymbol methodSymbol = (MethodSymbol)(object)memberResolutionResult.Member;
                bool flag;
                if ((object)returnType == null || methodSymbol.ReturnType.Equals(returnType, TypeCompareKind.AllIgnoreOptions))
                {
                    flag = true;
                }
                else if (returnRefKind == RefKind.None)
                {
                    flag = Conversions.HasIdentityOrImplicitReferenceConversion(methodSymbol.ReturnType, returnType, ref useSiteInfo);
                    if (!flag && isFunctionPointerConversion)
                    {
                        flag = ConversionsBase.HasImplicitPointerToVoidConversion(methodSymbol.ReturnType, returnType) || Conversions.HasImplicitPointerConversion(methodSymbol.ReturnType, returnType, ref useSiteInfo);
                    }
                }
                else
                {
                    flag = false;
                }
                if (!flag)
                {
                    results[i] = new MemberResolutionResult<TMember>(memberResolutionResult.Member, memberResolutionResult.LeastOverriddenMember, MemberAnalysisResult.WrongReturnType());
                }
                else if (methodSymbol.RefKind != returnRefKind)
                {
                    results[i] = new MemberResolutionResult<TMember>(memberResolutionResult.Member, memberResolutionResult.LeastOverriddenMember, MemberAnalysisResult.WrongRefKind());
                }
            }
        }

        private static Dictionary<NamedTypeSymbol, ArrayBuilder<TMember>> PartitionMembersByContainingType<TMember>(ArrayBuilder<TMember> members) where TMember : Symbol
        {
            Dictionary<NamedTypeSymbol, ArrayBuilder<TMember>> dictionary = new Dictionary<NamedTypeSymbol, ArrayBuilder<TMember>>();
            for (int i = 0; i < members.Count; i++)
            {
                TMember val = members[i];
                NamedTypeSymbol containingType = val.ContainingType;
                if (!dictionary.TryGetValue(containingType, out var value))
                {
                    value = (dictionary[containingType] = ArrayBuilder<TMember>.GetInstance());
                }
                value.Add(val);
            }
            return dictionary;
        }

        private static void ClearContainingTypeMap<TMember>(ref Dictionary<NamedTypeSymbol, ArrayBuilder<TMember>> containingTypeMapOpt) where TMember : Symbol
        {
            if (containingTypeMapOpt == null)
            {
                return;
            }
            foreach (ArrayBuilder<TMember> value in containingTypeMapOpt.Values)
            {
                value.Free();
            }
            containingTypeMapOpt = null;
        }

        private void AddConstructorToCandidateSet(MethodSymbol constructor, ArrayBuilder<MemberResolutionResult<MethodSymbol>> results, AnalyzedArguments arguments, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (constructor.HasUnsupportedMetadata)
            {
                if (completeResults)
                {
                    results.Add(new MemberResolutionResult<MethodSymbol>(constructor, constructor, MemberAnalysisResult.UnsupportedMetadata()));
                }
                return;
            }
            MemberAnalysisResult memberAnalysisResult = IsConstructorApplicableInNormalForm(constructor, arguments, completeResults, ref useSiteInfo);
            MemberAnalysisResult result = memberAnalysisResult;
            if (!memberAnalysisResult.IsValid && IsValidParams(constructor))
            {
                MemberAnalysisResult memberAnalysisResult2 = IsConstructorApplicableInExpandedForm(constructor, arguments, completeResults, ref useSiteInfo);
                if (memberAnalysisResult2.IsValid || completeResults)
                {
                    result = memberAnalysisResult2;
                }
            }
            if (result.IsValid || completeResults || result.HasUseSiteDiagnosticToReportFor(constructor))
            {
                results.Add(new MemberResolutionResult<MethodSymbol>(constructor, constructor, result));
            }
        }

        private MemberAnalysisResult IsConstructorApplicableInNormalForm(MethodSymbol constructor, AnalyzedArguments arguments, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArgumentAnalysisResult argAnalysis = AnalyzeArguments(constructor, arguments, isMethodGroupConversion: false, expanded: false);
            if (!argAnalysis.IsValid)
            {
                return MemberAnalysisResult.ArgumentParameterMismatch(argAnalysis);
            }
            if (constructor.HasUseSiteError)
            {
                return MemberAnalysisResult.UseSiteError();
            }
            EffectiveParameters effectiveParametersInNormalForm = GetEffectiveParametersInNormalForm(constructor, arguments.Arguments.Count, argAnalysis.ArgsToParamsOpt, arguments.RefKinds, isMethodGroupConversion: false, allowRefOmittedArguments: false);
            return IsApplicable(constructor, effectiveParametersInNormalForm, arguments, argAnalysis.ArgsToParamsOpt, constructor.IsVararg, hasAnyRefOmittedArgument: false, ignoreOpenTypes: false, completeResults, ref useSiteInfo);
        }

        private MemberAnalysisResult IsConstructorApplicableInExpandedForm(MethodSymbol constructor, AnalyzedArguments arguments, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArgumentAnalysisResult argAnalysis = AnalyzeArguments(constructor, arguments, isMethodGroupConversion: false, expanded: true);
            if (!argAnalysis.IsValid)
            {
                return MemberAnalysisResult.ArgumentParameterMismatch(argAnalysis);
            }
            if (constructor.HasUseSiteError)
            {
                return MemberAnalysisResult.UseSiteError();
            }
            EffectiveParameters effectiveParametersInExpandedForm = GetEffectiveParametersInExpandedForm(constructor, arguments.Arguments.Count, argAnalysis.ArgsToParamsOpt, arguments.RefKinds, isMethodGroupConversion: false, allowRefOmittedArguments: false);
            MemberAnalysisResult result = IsApplicable(constructor, effectiveParametersInExpandedForm, arguments, argAnalysis.ArgsToParamsOpt, isVararg: false, hasAnyRefOmittedArgument: false, ignoreOpenTypes: false, completeResults, ref useSiteInfo);
            if (!result.IsValid)
            {
                return result;
            }
            return MemberAnalysisResult.ExpandedForm(result.ArgsToParamsOpt, result.ConversionsOpt, hasAnyRefOmittedArgument: false);
        }

        private void AddMemberToCandidateSet<TMember>(TMember member, ArrayBuilder<MemberResolutionResult<TMember>> results, ArrayBuilder<TMember> members, ArrayBuilder<TypeWithAnnotations> typeArguments, BoundExpression receiverOpt, AnalyzedArguments arguments, bool completeResults, bool isMethodGroupConversion, bool allowRefOmittedArguments, Dictionary<NamedTypeSymbol, ArrayBuilder<TMember>> containingTypeMapOpt, bool inferWithDynamic, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool allowUnexpandedForm) where TMember : Symbol
        {
            if (members.Count >= 2)
            {
                if (containingTypeMapOpt == null)
                {
                    if (MemberGroupContainsMoreDerivedOverride(members, member, checkOverrideContainingType: true, ref useSiteInfo) || MemberGroupHidesByName(members, member, ref useSiteInfo))
                    {
                        return;
                    }
                }
                else if (containingTypeMapOpt.Count != 1)
                {
                    NamedTypeSymbol containingType = member.ContainingType;
                    foreach (KeyValuePair<NamedTypeSymbol, ArrayBuilder<TMember>> item2 in containingTypeMapOpt)
                    {
                        if (item2.Key.IsDerivedFrom(containingType, TypeCompareKind.ConsiderEverything, ref useSiteInfo))
                        {
                            ArrayBuilder<TMember> value = item2.Value;
                            if (MemberGroupContainsMoreDerivedOverride(value, member, checkOverrideContainingType: false, ref useSiteInfo) || MemberGroupHidesByName(value, member, ref useSiteInfo))
                            {
                                return;
                            }
                        }
                    }
                }
            }
            TMember val = (TMember)member.GetLeastOverriddenMember(_binder.ContainingType);
            if (member.HasUnsupportedMetadata)
            {
                if (completeResults)
                {
                    results.Add(new MemberResolutionResult<TMember>(member, val, MemberAnalysisResult.UnsupportedMetadata()));
                }
                return;
            }
            MemberResolutionResult<TMember> memberResolutionResult = ((allowUnexpandedForm || !IsValidParams(val)) ? IsMemberApplicableInNormalForm(member, val, typeArguments, arguments, isMethodGroupConversion, allowRefOmittedArguments, inferWithDynamic, completeResults, ref useSiteInfo) : default(MemberResolutionResult<TMember>));
            MemberResolutionResult<TMember> item = memberResolutionResult;
            if (!memberResolutionResult.Result.IsValid && !isMethodGroupConversion && IsValidParams(val))
            {
                MemberResolutionResult<TMember> memberResolutionResult2 = IsMemberApplicableInExpandedForm(member, val, typeArguments, arguments, allowRefOmittedArguments, completeResults, ref useSiteInfo);
                if (PreferExpandedFormOverNormalForm(memberResolutionResult.Result, memberResolutionResult2.Result))
                {
                    item = memberResolutionResult2;
                }
            }
            if (item.Result.IsValid || completeResults || item.HasUseSiteDiagnosticToReport)
            {
                results.Add(item);
            }
            else
            {
                item.Member.AddUseSiteInfo(ref useSiteInfo, addDiagnostics: false);
            }
        }

        private static bool PreferExpandedFormOverNormalForm(MemberAnalysisResult normalResult, MemberAnalysisResult expandedResult)
        {
            if (expandedResult.IsValid)
            {
                return true;
            }
            MemberResolutionKind kind = normalResult.Kind;
            if (kind == MemberResolutionKind.NoCorrespondingParameter || kind == MemberResolutionKind.RequiredParameterMissing)
            {
                switch (expandedResult.Kind)
                {
                    case MemberResolutionKind.NoCorrespondingNamedParameter:
                    case MemberResolutionKind.DuplicateNamedArgument:
                    case MemberResolutionKind.NameUsedForPositional:
                    case MemberResolutionKind.BadNonTrailingNamedArgument:
                    case MemberResolutionKind.UseSiteError:
                    case MemberResolutionKind.BadArgumentConversion:
                    case MemberResolutionKind.TypeInferenceFailed:
                    case MemberResolutionKind.TypeInferenceExtensionInstanceArgument:
                    case MemberResolutionKind.ConstructedParameterFailedConstraintCheck:
                        return true;
                }
            }
            return false;
        }

        public static bool IsValidParams(Symbol member)
        {
            if (member.GetIsVararg())
            {
                return false;
            }
            if (member.GetParameterCount() == 0)
            {
                return false;
            }
            ParameterSymbol parameterSymbol = member.GetParameters().Last();
            if (parameterSymbol.IsParams)
            {
                return parameterSymbol.OriginalDefinition.Type.IsSZArray();
            }
            return false;
        }

        private static bool IsMoreDerivedOverride(Symbol member, Symbol moreDerivedOverride, bool checkOverrideContainingType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!moreDerivedOverride.IsOverride || (checkOverrideContainingType && !moreDerivedOverride.ContainingType.IsDerivedFrom(member.ContainingType, TypeCompareKind.ConsiderEverything, ref useSiteInfo)) || !MemberSignatureComparer.SloppyOverrideComparer.Equals(member, moreDerivedOverride))
            {
                return false;
            }
            return moreDerivedOverride.GetLeastOverriddenMember(null).OriginalDefinition == member.GetLeastOverriddenMember(null).OriginalDefinition;
        }

        private static bool MemberGroupContainsMoreDerivedOverride<TMember>(ArrayBuilder<TMember> members, TMember member, bool checkOverrideContainingType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            if (!member.IsVirtual && !member.IsAbstract && !member.IsOverride)
            {
                return false;
            }
            if (!member.ContainingType.IsClassType())
            {
                return false;
            }
            for (int i = 0; i < members.Count; i++)
            {
                if (IsMoreDerivedOverride(member, members[i], checkOverrideContainingType, ref useSiteInfo))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool MemberGroupHidesByName<TMember>(ArrayBuilder<TMember> members, TMember member, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            NamedTypeSymbol containingType = member.ContainingType;
            ArrayBuilder<TMember>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TMember current = enumerator.Current;
                NamedTypeSymbol containingType2 = current.ContainingType;
                if (HidesByName(current) && containingType2.IsDerivedFrom(containingType, TypeCompareKind.ConsiderEverything, ref useSiteInfo))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HidesByName(Symbol member)
        {
            return member.Kind switch
            {
                SymbolKind.Method => ((MethodSymbol)member).HidesBaseMethodsByName,
                SymbolKind.Property => ((PropertySymbol)member).HidesBasePropertiesByName,
                _ => throw ExceptionUtilities.UnexpectedValue(member.Kind),
            };
        }

        private void RemoveInaccessibleTypeArguments<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                if (memberResolutionResult.Result.IsValid && !TypeArgumentsAccessible(memberResolutionResult.Member.GetMemberTypeArgumentsNoUseSiteDiagnostics(), ref useSiteInfo))
                {
                    results[i] = new MemberResolutionResult<TMember>(memberResolutionResult.Member, memberResolutionResult.LeastOverriddenMember, MemberAnalysisResult.InaccessibleTypeArgument());
                }
            }
        }

        private bool TypeArgumentsAccessible(ImmutableArray<TypeSymbol> typeArguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ImmutableArray<TypeSymbol>.Enumerator enumerator = typeArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeSymbol current = enumerator.Current;
                if (!_binder.IsAccessible(current, ref useSiteInfo))
                {
                    return false;
                }
            }
            return true;
        }

        private static void RemoveLessDerivedMembers<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            RemoveAllInterfaceMembers(results);
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                if ((memberResolutionResult.Result.IsValid || memberResolutionResult.HasUseSiteDiagnosticToReport) && IsLessDerivedThanAny(memberResolutionResult.LeastOverriddenMember.ContainingType, results, ref useSiteInfo))
                {
                    results[i] = new MemberResolutionResult<TMember>(memberResolutionResult.Member, memberResolutionResult.LeastOverriddenMember, MemberAnalysisResult.LessDerived());
                }
            }
        }

        private static bool IsLessDerivedThanAny<TMember>(TypeSymbol type, ArrayBuilder<MemberResolutionResult<TMember>> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                if (memberResolutionResult.Result.IsValid)
                {
                    NamedTypeSymbol containingType = memberResolutionResult.LeastOverriddenMember.ContainingType;
                    if (type.SpecialType == SpecialType.System_Object && containingType.SpecialType != SpecialType.System_Object)
                    {
                        return true;
                    }
                    if (containingType.IsInterfaceType() && type.IsInterfaceType() && containingType.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).Contains((NamedTypeSymbol)type))
                    {
                        return true;
                    }
                    if (containingType.IsClassType() && type.IsClassType() && containingType.IsDerivedFrom(type, TypeCompareKind.ConsiderEverything, ref useSiteInfo))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void RemoveAllInterfaceMembers<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results) where TMember : Symbol
        {
            bool flag = false;
            for (int i = 0; i < results.Count; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult = results[i];
                if (memberResolutionResult.Result.IsValid)
                {
                    NamedTypeSymbol containingType = memberResolutionResult.LeastOverriddenMember.ContainingType;
                    if (containingType.IsClassType() && containingType.GetSpecialTypeSafe() != SpecialType.System_Object)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                return;
            }
            for (int j = 0; j < results.Count; j++)
            {
                MemberResolutionResult<TMember> memberResolutionResult2 = results[j];
                if (memberResolutionResult2.Result.IsValid)
                {
                    TMember member = memberResolutionResult2.Member;
                    if (member.ContainingType.IsInterfaceType())
                    {
                        results[j] = new MemberResolutionResult<TMember>(member, memberResolutionResult2.LeastOverriddenMember, MemberAnalysisResult.LessDerived());
                    }
                }
            }
        }

        private void PerformObjectCreationOverloadResolution(ArrayBuilder<MemberResolutionResult<MethodSymbol>> results, ImmutableArray<MethodSymbol> constructors, AnalyzedArguments arguments, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ImmutableArray<MethodSymbol>.Enumerator enumerator = constructors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                AddConstructorToCandidateSet(current, results, arguments, completeResults, ref useSiteInfo);
            }
            ReportUseSiteInfo(results, ref useSiteInfo);
            RemoveWorseMembers(results, arguments, ref useSiteInfo);
        }

        private static void ReportUseSiteInfo<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = results.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberResolutionResult<TMember> current = enumerator.Current;
                current.Member.AddUseSiteInfo(ref useSiteInfo, current.HasUseSiteDiagnosticToReport);
            }
        }

        private int GetTheBestCandidateIndex<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, AnalyzedArguments arguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            int num = -1;
            for (int i = 0; i < results.Count; i++)
            {
                if (!results[i].IsValid)
                {
                    continue;
                }
                if (num == -1)
                {
                    num = i;
                    continue;
                }
                if (results[num].Member == results[i].Member)
                {
                    num = -1;
                    continue;
                }
                switch (BetterFunctionMember(results[num], results[i], arguments.Arguments, ref useSiteInfo))
                {
                    case BetterResult.Right:
                        num = i;
                        break;
                    default:
                        num = -1;
                        break;
                    case BetterResult.Left:
                        break;
                }
            }
            for (int j = 0; j < num; j++)
            {
                if (results[j].IsValid)
                {
                    if (results[num].Member == results[j].Member)
                    {
                        return -1;
                    }
                    if (BetterFunctionMember(results[num], results[j], arguments.Arguments, ref useSiteInfo) != 0)
                    {
                        return -1;
                    }
                }
            }
            return num;
        }

        private void RemoveWorseMembers<TMember>(ArrayBuilder<MemberResolutionResult<TMember>> results, AnalyzedArguments arguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            if (SingleValidResult(results))
            {
                return;
            }
            int theBestCandidateIndex = GetTheBestCandidateIndex(results, arguments, ref useSiteInfo);
            if (theBestCandidateIndex != -1)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].IsValid && i != theBestCandidateIndex)
                    {
                        results[i] = results[i].Worse();
                    }
                }
                return;
            }
            ArrayBuilder<int> instance = ArrayBuilder<int>.GetInstance(results.Count, 0);
            int num = 0;
            int index = -1;
            for (int j = 0; j < results.Count; j++)
            {
                MemberResolutionResult<TMember> m = results[j];
                if (!m.IsValid || instance[j] == 1)
                {
                    continue;
                }
                for (int k = 0; k < results.Count; k++)
                {
                    MemberResolutionResult<TMember> m2 = results[k];
                    if (m2.IsValid && j != k && !(m.Member == m2.Member))
                    {
                        switch (BetterFunctionMember(m, m2, arguments.Arguments, ref useSiteInfo))
                        {
                            case BetterResult.Left:
                                instance[k] = 1;
                                continue;
                            case BetterResult.Right:
                                break;
                            default:
                                continue;
                        }
                        instance[j] = 1;
                        break;
                    }
                }
                if (instance[j] == 0)
                {
                    instance[j] = 2;
                    num++;
                    index = j;
                }
            }
            switch (num)
            {
                case 0:
                    {
                        for (int n = 0; n < instance.Count; n++)
                        {
                            if (instance[n] == 1)
                            {
                                results[n] = results[n].Worse();
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        for (int num2 = 0; num2 < instance.Count; num2++)
                        {
                            if (instance[num2] == 1)
                            {
                                results[num2] = ((BetterFunctionMember(results[index], results[num2], arguments.Arguments, ref useSiteInfo) == BetterResult.Left) ? results[num2].Worst() : results[num2].Worse());
                            }
                        }
                        results[index] = results[index].Worse();
                        break;
                    }
                default:
                    {
                        for (int l = 0; l < instance.Count; l++)
                        {
                            if (instance[l] == 1)
                            {
                                results[l] = results[l].Worst();
                            }
                            else if (instance[l] == 2)
                            {
                                results[l] = results[l].Worse();
                            }
                        }
                        break;
                    }
            }
            instance.Free();
        }

        private TypeSymbol GetParameterType(ParameterSymbol parameter, MemberAnalysisResult result)
        {
            TypeSymbol type = parameter.Type;
            if (result.Kind == MemberResolutionKind.ApplicableInExpandedForm && parameter.IsParams && type.IsSZArray())
            {
                return ((ArrayTypeSymbol)type).ElementType;
            }
            return type;
        }

        private static ParameterSymbol GetParameter(int argIndex, MemberAnalysisResult result, ImmutableArray<ParameterSymbol> parameters)
        {
            int index = result.ParameterFromArgument(argIndex);
            return parameters[index];
        }

        private BetterResult BetterFunctionMember<TMember>(MemberResolutionResult<TMember> m1, MemberResolutionResult<TMember> m2, ArrayBuilder<BoundExpression> arguments, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            bool hasAnyRefOmittedArgument = m1.Result.HasAnyRefOmittedArgument;
            bool hasAnyRefOmittedArgument2 = m2.Result.HasAnyRefOmittedArgument;
            if (hasAnyRefOmittedArgument != hasAnyRefOmittedArgument2)
            {
                if (!hasAnyRefOmittedArgument)
                {
                    return BetterResult.Left;
                }
                return BetterResult.Right;
            }
            return BetterFunctionMember(m1, m2, arguments, hasAnyRefOmittedArgument, ref useSiteInfo);
        }

        private BetterResult BetterFunctionMember<TMember>(MemberResolutionResult<TMember> m1, MemberResolutionResult<TMember> m2, ArrayBuilder<BoundExpression> arguments, bool considerRefKinds, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            BetterResult betterResult = BetterResult.Neither;
            bool flag = false;
            bool flag2 = false;
            ImmutableArray<ParameterSymbol> parameters = m1.LeastOverriddenMember.GetParameters();
            ImmutableArray<ParameterSymbol> parameters2 = m2.LeastOverriddenMember.GetParameters();
            bool flag3 = true;
            int i;
            for (i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].Kind == BoundKind.ArgListOperator)
                {
                    continue;
                }
                ParameterSymbol parameter = GetParameter(i, m1.Result, parameters);
                TypeSymbol parameterType = GetParameterType(parameter, m1.Result);
                ParameterSymbol parameter2 = GetParameter(i, m2.Result, parameters2);
                TypeSymbol parameterType2 = GetParameterType(parameter2, m2.Result);
                BetterResult betterResult2 = BetterConversionFromExpression(arguments[i], parameterType, m1.Result.ConversionForArg(i), parameter.RefKind, parameterType2, m2.Result.ConversionForArg(i), parameter2.RefKind, considerRefKinds, ref useSiteInfo, out bool okToDowngradeToNeither);
                TypeSymbol source = parameterType;
                TypeSymbol destination = parameterType2;
                if (!_binder.InAttributeArgument)
                {
                    source = parameterType.NormalizeTaskTypes(Compilation);
                    destination = parameterType2.NormalizeTaskTypes(Compilation);
                }
                if (betterResult2 == BetterResult.Neither)
                {
                    if (flag3 && Conversions.ClassifyImplicitConversionFromType(source, destination, ref useSiteInfo).Kind != ConversionKind.Identity)
                    {
                        flag3 = false;
                    }
                    continue;
                }
                if (Conversions.ClassifyImplicitConversionFromType(source, destination, ref useSiteInfo).Kind != ConversionKind.Identity)
                {
                    flag3 = false;
                }
                if (betterResult == BetterResult.Neither)
                {
                    if (!(flag2 && okToDowngradeToNeither))
                    {
                        betterResult = betterResult2;
                        flag = okToDowngradeToNeither;
                    }
                }
                else if (betterResult != betterResult2)
                {
                    if (flag)
                    {
                        if (okToDowngradeToNeither)
                        {
                            betterResult = BetterResult.Neither;
                            flag = false;
                            flag2 = true;
                        }
                        else
                        {
                            betterResult = betterResult2;
                            flag = false;
                        }
                    }
                    else if (!okToDowngradeToNeither)
                    {
                        betterResult = BetterResult.Neither;
                        break;
                    }
                }
                else
                {
                    flag = flag && okToDowngradeToNeither;
                }
            }
            if (betterResult != BetterResult.Neither)
            {
                return betterResult;
            }
            GetParameterCounts(m1, arguments, out var declaredParameterCount, out var parametersUsedIncludingExpansionAndOptional);
            GetParameterCounts(m2, arguments, out var declaredParameterCount2, out var parametersUsedIncludingExpansionAndOptional2);
            if (flag3 && parametersUsedIncludingExpansionAndOptional == parametersUsedIncludingExpansionAndOptional2)
            {
                for (i++; i < arguments.Count; i++)
                {
                    if (arguments[i].Kind != BoundKind.ArgListOperator)
                    {
                        ParameterSymbol parameter3 = GetParameter(i, m1.Result, parameters);
                        TypeSymbol parameterType3 = GetParameterType(parameter3, m1.Result);
                        ParameterSymbol parameter4 = GetParameter(i, m2.Result, parameters2);
                        TypeSymbol parameterType4 = GetParameterType(parameter4, m2.Result);
                        TypeSymbol source2 = parameterType3;
                        TypeSymbol destination2 = parameterType4;
                        if (!_binder.InAttributeArgument)
                        {
                            source2 = parameterType3.NormalizeTaskTypes(Compilation);
                            destination2 = parameterType4.NormalizeTaskTypes(Compilation);
                        }
                        if (Conversions.ClassifyImplicitConversionFromType(source2, destination2, ref useSiteInfo).Kind != ConversionKind.Identity)
                        {
                            flag3 = false;
                            break;
                        }
                    }
                }
            }
            if (!flag3 || parametersUsedIncludingExpansionAndOptional != parametersUsedIncludingExpansionAndOptional2)
            {
                if (parametersUsedIncludingExpansionAndOptional != parametersUsedIncludingExpansionAndOptional2)
                {
                    if (m1.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm)
                    {
                        if (m2.Result.Kind != MemberResolutionKind.ApplicableInExpandedForm)
                        {
                            return BetterResult.Right;
                        }
                    }
                    else if (m2.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm)
                    {
                        return BetterResult.Left;
                    }
                    if (parametersUsedIncludingExpansionAndOptional == arguments.Count)
                    {
                        return BetterResult.Left;
                    }
                    if (parametersUsedIncludingExpansionAndOptional2 == arguments.Count)
                    {
                        return BetterResult.Right;
                    }
                }
                return PreferValOverInParameters(arguments, m1, parameters, m2, parameters2);
            }
            if (m1.Member.GetMemberArity() == 0)
            {
                if (m2.Member.GetMemberArity() > 0)
                {
                    return BetterResult.Left;
                }
            }
            else if (m2.Member.GetMemberArity() == 0)
            {
                return BetterResult.Right;
            }
            if (m1.Result.Kind == MemberResolutionKind.ApplicableInNormalForm && m2.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm)
            {
                return BetterResult.Left;
            }
            if (m1.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm && m2.Result.Kind == MemberResolutionKind.ApplicableInNormalForm)
            {
                return BetterResult.Right;
            }
            if (m1.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm && m2.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm)
            {
                if (declaredParameterCount > declaredParameterCount2)
                {
                    return BetterResult.Left;
                }
                if (declaredParameterCount < declaredParameterCount2)
                {
                    return BetterResult.Right;
                }
            }
            bool flag4 = m1.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm || declaredParameterCount == arguments.Count;
            bool flag5 = m2.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm || declaredParameterCount2 == arguments.Count;
            if (flag4 && !flag5)
            {
                return BetterResult.Left;
            }
            if (!flag4 && flag5)
            {
                return BetterResult.Right;
            }
            ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
            ArrayBuilder<TypeSymbol> instance2 = ArrayBuilder<TypeSymbol>.GetInstance();
            ImmutableArray<ParameterSymbol> parameters3 = m1.LeastOverriddenMember.OriginalDefinition.GetParameters();
            ImmutableArray<ParameterSymbol> parameters4 = m2.LeastOverriddenMember.OriginalDefinition.GetParameters();
            for (i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].Kind != BoundKind.ArgListOperator)
                {
                    ParameterSymbol parameter5 = GetParameter(i, m1.Result, parameters3);
                    instance.Add(GetParameterType(parameter5, m1.Result));
                    ParameterSymbol parameter6 = GetParameter(i, m2.Result, parameters4);
                    instance2.Add(GetParameterType(parameter6, m2.Result));
                }
            }
            betterResult = MoreSpecificType(instance, instance2, ref useSiteInfo);
            instance.Free();
            instance2.Free();
            if (betterResult != BetterResult.Neither)
            {
                return betterResult;
            }
            if (m1.Member.ContainingType.TypeKind == TypeKind.Submission && m2.Member.ContainingType.TypeKind == TypeKind.Submission)
            {
                CSharpCompilation declaringCompilation = m1.Member.DeclaringCompilation;
                CSharpCompilation declaringCompilation2 = m2.Member.DeclaringCompilation;
                int submissionSlotIndex = declaringCompilation.GetSubmissionSlotIndex();
                int submissionSlotIndex2 = declaringCompilation2.GetSubmissionSlotIndex();
                if (submissionSlotIndex > submissionSlotIndex2)
                {
                    return BetterResult.Left;
                }
                if (submissionSlotIndex < submissionSlotIndex2)
                {
                    return BetterResult.Right;
                }
            }
            int num = m1.LeastOverriddenMember.CustomModifierCount();
            int num2 = m2.LeastOverriddenMember.CustomModifierCount();
            if (num != num2)
            {
                if (num >= num2)
                {
                    return BetterResult.Right;
                }
                return BetterResult.Left;
            }
            return PreferValOverInParameters(arguments, m1, parameters, m2, parameters2);
        }

        private static BetterResult PreferValOverInParameters<TMember>(ArrayBuilder<BoundExpression> arguments, MemberResolutionResult<TMember> m1, ImmutableArray<ParameterSymbol> parameters1, MemberResolutionResult<TMember> m2, ImmutableArray<ParameterSymbol> parameters2) where TMember : Symbol
        {
            BetterResult betterResult = BetterResult.Neither;
            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].Kind == BoundKind.ArgListOperator)
                {
                    continue;
                }
                ParameterSymbol parameter = GetParameter(i, m1.Result, parameters1);
                ParameterSymbol parameter2 = GetParameter(i, m2.Result, parameters2);
                if (parameter.RefKind == RefKind.None && parameter2.RefKind == RefKind.In)
                {
                    if (betterResult == BetterResult.Right)
                    {
                        return BetterResult.Neither;
                    }
                    betterResult = BetterResult.Left;
                }
                else if (parameter2.RefKind == RefKind.None && parameter.RefKind == RefKind.In)
                {
                    if (betterResult == BetterResult.Left)
                    {
                        return BetterResult.Neither;
                    }
                    betterResult = BetterResult.Right;
                }
            }
            return betterResult;
        }

        private static void GetParameterCounts<TMember>(MemberResolutionResult<TMember> m, ArrayBuilder<BoundExpression> arguments, out int declaredParameterCount, out int parametersUsedIncludingExpansionAndOptional) where TMember : Symbol
        {
            declaredParameterCount = m.Member.GetParameterCount();
            if (m.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm)
            {
                if (arguments.Count < declaredParameterCount)
                {
                    ImmutableArray<int> argsToParamsOpt = m.Result.ArgsToParamsOpt;
                    if (argsToParamsOpt.IsDefaultOrEmpty || !argsToParamsOpt.Contains(declaredParameterCount - 1))
                    {
                        parametersUsedIncludingExpansionAndOptional = declaredParameterCount - 1;
                    }
                    else
                    {
                        parametersUsedIncludingExpansionAndOptional = declaredParameterCount;
                    }
                }
                else
                {
                    parametersUsedIncludingExpansionAndOptional = arguments.Count;
                }
            }
            else
            {
                parametersUsedIncludingExpansionAndOptional = declaredParameterCount;
            }
        }

        private static BetterResult MoreSpecificType(ArrayBuilder<TypeSymbol> t1, ArrayBuilder<TypeSymbol> t2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            BetterResult betterResult = BetterResult.Neither;
            for (int i = 0; i < t1.Count; i++)
            {
                BetterResult betterResult2 = MoreSpecificType(t1[i], t2[i], ref useSiteInfo);
                if (betterResult2 != BetterResult.Neither)
                {
                    if (betterResult == BetterResult.Neither)
                    {
                        betterResult = betterResult2;
                    }
                    else if (betterResult != betterResult2)
                    {
                        return BetterResult.Neither;
                    }
                }
            }
            return betterResult;
        }

        private static BetterResult MoreSpecificType(TypeSymbol t1, TypeSymbol t2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            bool flag = t1.IsTypeParameter();
            bool flag2 = t2.IsTypeParameter();
            if (flag && !flag2)
            {
                return BetterResult.Right;
            }
            if (!flag && flag2)
            {
                return BetterResult.Left;
            }
            if (flag && flag2)
            {
                return BetterResult.Neither;
            }
            if (t1.IsArray())
            {
                ArrayTypeSymbol obj = (ArrayTypeSymbol)t1;
                return MoreSpecificType(t2: ((ArrayTypeSymbol)t2).ElementType, t1: obj.ElementType, useSiteInfo: ref useSiteInfo);
            }
            if (t1.TypeKind == TypeKind.Pointer)
            {
                PointerTypeSymbol obj2 = (PointerTypeSymbol)t1;
                return MoreSpecificType(t2: ((PointerTypeSymbol)t2).PointedAtType, t1: obj2.PointedAtType, useSiteInfo: ref useSiteInfo);
            }
            if (t1.IsDynamic() || t2.IsDynamic())
            {
                return BetterResult.Neither;
            }
            NamedTypeSymbol namedTypeSymbol = t1 as NamedTypeSymbol;
            NamedTypeSymbol namedTypeSymbol2 = t2 as NamedTypeSymbol;
            if ((object)namedTypeSymbol == null)
            {
                return BetterResult.Neither;
            }
            ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
            ArrayBuilder<TypeSymbol> instance2 = ArrayBuilder<TypeSymbol>.GetInstance();
            namedTypeSymbol.GetAllTypeArguments(instance, ref useSiteInfo);
            namedTypeSymbol2.GetAllTypeArguments(instance2, ref useSiteInfo);
            BetterResult result = MoreSpecificType(instance, instance2, ref useSiteInfo);
            instance.Free();
            instance2.Free();
            return result;
        }

        private BetterResult BetterConversionFromExpression(BoundExpression node, TypeSymbol t1, TypeSymbol t2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return BetterConversionFromExpression(node, t1, Conversions.ClassifyImplicitConversionFromExpression(node, t1, ref useSiteInfo), t2, Conversions.ClassifyImplicitConversionFromExpression(node, t2, ref useSiteInfo), ref useSiteInfo, out bool okToDowngradeToNeither);
        }

        private BetterResult BetterConversionFromExpression(BoundExpression node, TypeSymbol t1, Conversion conv1, RefKind refKind1, TypeSymbol t2, Conversion conv2, RefKind refKind2, bool considerRefKinds, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, out bool okToDowngradeToNeither)
        {
            okToDowngradeToNeither = false;
            if (considerRefKinds)
            {
                if (refKind1 != refKind2)
                {
                    if (refKind1 == RefKind.None)
                    {
                        if (conv1.Kind != ConversionKind.Identity)
                        {
                            return BetterResult.Neither;
                        }
                        return BetterResult.Left;
                    }
                    if (conv2.Kind != ConversionKind.Identity)
                    {
                        return BetterResult.Neither;
                    }
                    return BetterResult.Right;
                }
                if (refKind1 == RefKind.Ref)
                {
                    return BetterResult.Neither;
                }
            }
            return BetterConversionFromExpression(node, t1, conv1, t2, conv2, ref useSiteInfo, out okToDowngradeToNeither);
        }

        private BetterResult BetterConversionFromExpression(BoundExpression node, TypeSymbol t1, Conversion conv1, TypeSymbol t2, Conversion conv2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, out bool okToDowngradeToNeither)
        {
            okToDowngradeToNeither = false;
            if (ConversionsBase.HasIdentityConversion(t1, t2))
            {
                return BetterResult.Neither;
            }
            UnboundLambda unboundLambda = node as UnboundLambda;
            BoundKind kind = node.Kind;
            if (kind == BoundKind.OutVariablePendingInference || kind == BoundKind.OutDeconstructVarPendingInference || (kind == BoundKind.DiscardExpression && !node.HasExpressionType()))
            {
                okToDowngradeToNeither = false;
                return BetterResult.Neither;
            }
            bool num = ExpressionMatchExactly(node, t1, ref useSiteInfo);
            bool flag = ExpressionMatchExactly(node, t2, ref useSiteInfo);
            if (num)
            {
                if (!flag)
                {
                    okToDowngradeToNeither = unboundLambda != null && CanDowngradeConversionFromLambdaToNeither(BetterResult.Left, unboundLambda, t1, t2, ref useSiteInfo, fromTypeAnalysis: false);
                    return BetterResult.Left;
                }
            }
            else if (flag)
            {
                okToDowngradeToNeither = unboundLambda != null && CanDowngradeConversionFromLambdaToNeither(BetterResult.Right, unboundLambda, t1, t2, ref useSiteInfo, fromTypeAnalysis: false);
                return BetterResult.Right;
            }
            if (!conv1.IsConditionalExpression && conv2.IsConditionalExpression)
            {
                return BetterResult.Left;
            }
            if (!conv2.IsConditionalExpression && conv1.IsConditionalExpression)
            {
                return BetterResult.Right;
            }
            return BetterConversionTarget(node, t1, conv1, t2, conv2, ref useSiteInfo, out okToDowngradeToNeither);
        }

        private bool ExpressionMatchExactly(BoundExpression node, TypeSymbol t, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)node.Type != null && ConversionsBase.HasIdentityConversion(node.Type, t))
            {
                return true;
            }
            if (node.Kind == BoundKind.TupleLiteral)
            {
                return ExpressionMatchExactly((BoundTupleLiteral)node, t, ref useSiteInfo);
            }
            NamedTypeSymbol delegateType;
            MethodSymbol delegateInvokeMethod;
            TypeSymbol typeSymbol;
            if (node.Kind == BoundKind.UnboundLambda && (object)(delegateType = t.GetDelegateType()) != null && (object)(delegateInvokeMethod = delegateType.DelegateInvokeMethod) != null && !(typeSymbol = delegateInvokeMethod.ReturnType).IsVoidType())
            {
                BoundLambda boundLambda = ((UnboundLambda)node).BindForReturnTypeInference(delegateType);
                TypeWithAnnotations inferredReturnType = boundLambda.GetInferredReturnType(ref useSiteInfo);
                if (inferredReturnType.HasType && ConversionsBase.HasIdentityConversion(inferredReturnType.Type, typeSymbol))
                {
                    return true;
                }
                if (boundLambda.Symbol.IsAsync)
                {
                    typeSymbol = ((!typeSymbol.OriginalDefinition.IsGenericTaskType(Compilation)) ? null : ((NamedTypeSymbol)typeSymbol).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type);
                }
                if ((object)typeSymbol != null)
                {
                    int length = boundLambda.Body.Statements.Length;
                    if (length != 0)
                    {
                        if (length == 1 && boundLambda.Body.Statements[0].Kind == BoundKind.ReturnStatement)
                        {
                            BoundReturnStatement boundReturnStatement = (BoundReturnStatement)boundLambda.Body.Statements[0];
                            if (boundReturnStatement.ExpressionOpt != null && ExpressionMatchExactly(boundReturnStatement.ExpressionOpt, typeSymbol, ref useSiteInfo))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            ArrayBuilder<BoundReturnStatement> instance = ArrayBuilder<BoundReturnStatement>.GetInstance();
                            new ReturnStatements(instance).Visit(boundLambda.Body);
                            bool flag = false;
                            ArrayBuilder<BoundReturnStatement>.Enumerator enumerator = instance.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                BoundReturnStatement current = enumerator.Current;
                                if (current.ExpressionOpt == null || !ExpressionMatchExactly(current.ExpressionOpt, typeSymbol, ref useSiteInfo))
                                {
                                    flag = false;
                                    break;
                                }
                                flag = true;
                            }
                            instance.Free();
                            if (flag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool ExpressionMatchExactly(BoundTupleLiteral tupleSource, TypeSymbol targetType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (targetType.Kind != SymbolKind.NamedType)
            {
                return false;
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)targetType;
            ImmutableArray<BoundExpression> arguments = tupleSource.Arguments;
            if (!namedTypeSymbol.IsTupleTypeOfCardinality(arguments.Length))
            {
                return false;
            }
            ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = namedTypeSymbol.TupleElementTypesWithAnnotations;
            for (int i = 0; i < arguments.Length; i++)
            {
                if (!ExpressionMatchExactly(arguments[i], tupleElementTypesWithAnnotations[i].Type, ref useSiteInfo))
                {
                    return false;
                }
            }
            return true;
        }

        private BetterResult BetterConversionTarget(TypeSymbol type1, TypeSymbol type2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return BetterConversionTargetCore(null, type1, default(Conversion), type2, default(Conversion), ref useSiteInfo, out bool okToDowngradeToNeither, 100);
        }

        private BetterResult BetterConversionTargetCore(TypeSymbol type1, TypeSymbol type2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, int betterConversionTargetRecursionLimit)
        {
            if (betterConversionTargetRecursionLimit < 0)
            {
                return BetterResult.Neither;
            }
            return BetterConversionTargetCore(null, type1, default(Conversion), type2, default(Conversion), ref useSiteInfo, out bool okToDowngradeToNeither, betterConversionTargetRecursionLimit - 1);
        }

        private BetterResult BetterConversionTarget(BoundExpression node, TypeSymbol type1, Conversion conv1, TypeSymbol type2, Conversion conv2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, out bool okToDowngradeToNeither)
        {
            return BetterConversionTargetCore(node, type1, conv1, type2, conv2, ref useSiteInfo, out okToDowngradeToNeither, 100);
        }

        private BetterResult BetterConversionTargetCore(BoundExpression node, TypeSymbol type1, Conversion conv1, TypeSymbol type2, Conversion conv2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, out bool okToDowngradeToNeither, int betterConversionTargetRecursionLimit)
        {
            okToDowngradeToNeither = false;
            if (ConversionsBase.HasIdentityConversion(type1, type2))
            {
                return BetterResult.Neither;
            }
            bool isImplicit = Conversions.ClassifyImplicitConversionFromType(type1, type2, ref useSiteInfo).IsImplicit;
            bool isImplicit2 = Conversions.ClassifyImplicitConversionFromType(type2, type1, ref useSiteInfo).IsImplicit;
            UnboundLambda unboundLambda = node as UnboundLambda;
            if (isImplicit)
            {
                if (isImplicit2)
                {
                    return BetterResult.Neither;
                }
                okToDowngradeToNeither = unboundLambda != null && CanDowngradeConversionFromLambdaToNeither(BetterResult.Left, unboundLambda, type1, type2, ref useSiteInfo, fromTypeAnalysis: true);
                return BetterResult.Left;
            }
            if (isImplicit2)
            {
                okToDowngradeToNeither = unboundLambda != null && CanDowngradeConversionFromLambdaToNeither(BetterResult.Right, unboundLambda, type1, type2, ref useSiteInfo, fromTypeAnalysis: true);
                return BetterResult.Right;
            }
            bool num = type1.OriginalDefinition.IsGenericTaskType(Compilation);
            bool flag = type2.OriginalDefinition.IsGenericTaskType(Compilation);
            if (num)
            {
                if (flag)
                {
                    return BetterConversionTargetCore(((NamedTypeSymbol)type1).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type, ((NamedTypeSymbol)type2).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type, ref useSiteInfo, betterConversionTargetRecursionLimit);
                }
                return BetterResult.Neither;
            }
            if (flag)
            {
                return BetterResult.Neither;
            }
            NamedTypeSymbol delegateType;
            if ((object)(delegateType = type1.GetDelegateType()) != null)
            {
                NamedTypeSymbol delegateType2;
                if ((object)(delegateType2 = type2.GetDelegateType()) != null)
                {
                    MethodSymbol delegateInvokeMethod = delegateType.DelegateInvokeMethod;
                    MethodSymbol delegateInvokeMethod2 = delegateType2.DelegateInvokeMethod;
                    if ((object)delegateInvokeMethod != null && (object)delegateInvokeMethod2 != null)
                    {
                        TypeSymbol returnType = delegateInvokeMethod.ReturnType;
                        TypeSymbol returnType2 = delegateInvokeMethod2.ReturnType;
                        BetterResult betterResult = BetterResult.Neither;
                        if (!returnType.IsVoidType())
                        {
                            if (returnType2.IsVoidType())
                            {
                                betterResult = BetterResult.Left;
                            }
                        }
                        else if (!returnType2.IsVoidType())
                        {
                            betterResult = BetterResult.Right;
                        }
                        if (betterResult == BetterResult.Neither)
                        {
                            betterResult = BetterConversionTargetCore(returnType, returnType2, ref useSiteInfo, betterConversionTargetRecursionLimit);
                        }
                        if (node != null && node.Kind == BoundKind.MethodGroup)
                        {
                            BoundMethodGroup node2 = (BoundMethodGroup)node;
                            switch (betterResult)
                            {
                                case BetterResult.Left:
                                    if (IsMethodGroupConversionIncompatibleWithDelegate(node2, delegateType, conv1))
                                    {
                                        return BetterResult.Neither;
                                    }
                                    break;
                                case BetterResult.Right:
                                    if (IsMethodGroupConversionIncompatibleWithDelegate(node2, delegateType2, conv2))
                                    {
                                        return BetterResult.Neither;
                                    }
                                    break;
                            }
                        }
                        return betterResult;
                    }
                }
                return BetterResult.Neither;
            }
            if ((object)type2.GetDelegateType() != null)
            {
                return BetterResult.Neither;
            }
            if (IsSignedIntegralType(type1))
            {
                if (IsUnsignedIntegralType(type2))
                {
                    return BetterResult.Left;
                }
            }
            else if (IsUnsignedIntegralType(type1) && IsSignedIntegralType(type2))
            {
                return BetterResult.Right;
            }
            return BetterResult.Neither;
        }

        private bool IsMethodGroupConversionIncompatibleWithDelegate(BoundMethodGroup node, NamedTypeSymbol delegateType, Conversion conv)
        {
            if (conv.IsMethodGroup)
            {
                return !_binder.MethodIsCompatibleWithDelegateOrFunctionPointer(node.ReceiverOpt, conv.IsExtensionMethod, conv.Method, delegateType, Location.None, BindingDiagnosticBag.Discarded);
            }
            return false;
        }

        private bool CanDowngradeConversionFromLambdaToNeither(BetterResult currentResult, UnboundLambda lambda, TypeSymbol type1, TypeSymbol type2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool fromTypeAnalysis)
        {
            NamedTypeSymbol delegateType;
            NamedTypeSymbol delegateType2;
            if ((object)(delegateType = type1.GetDelegateType()) != null && (object)(delegateType2 = type2.GetDelegateType()) != null)
            {
                MethodSymbol delegateInvokeMethod = delegateType.DelegateInvokeMethod;
                MethodSymbol delegateInvokeMethod2 = delegateType2.DelegateInvokeMethod;
                if ((object)delegateInvokeMethod != null && (object)delegateInvokeMethod2 != null)
                {
                    if (!IdenticalParameters(delegateInvokeMethod.Parameters, delegateInvokeMethod2.Parameters))
                    {
                        return true;
                    }
                    TypeSymbol returnType = delegateInvokeMethod.ReturnType;
                    TypeSymbol returnType2 = delegateInvokeMethod2.ReturnType;
                    if (returnType.IsVoidType())
                    {
                        if (returnType2.IsVoidType())
                        {
                            return true;
                        }
                        return false;
                    }
                    if (returnType2.IsVoidType())
                    {
                        return false;
                    }
                    if (ConversionsBase.HasIdentityConversion(returnType, returnType2))
                    {
                        return true;
                    }
                    if (!lambda.InferReturnType(Conversions, delegateType, ref useSiteInfo).HasType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IdenticalParameters(ImmutableArray<ParameterSymbol> p1, ImmutableArray<ParameterSymbol> p2)
        {
            if (p1.IsDefault || p2.IsDefault)
            {
                return false;
            }
            if (p1.Length != p2.Length)
            {
                return false;
            }
            for (int i = 0; i < p1.Length; i++)
            {
                ParameterSymbol parameterSymbol = p1[i];
                ParameterSymbol parameterSymbol2 = p2[i];
                if (parameterSymbol.RefKind != parameterSymbol2.RefKind)
                {
                    return false;
                }
                if (!ConversionsBase.HasIdentityConversion(parameterSymbol.Type, parameterSymbol2.Type))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsSignedIntegralType(TypeSymbol type)
        {
            if ((object)type != null && type.IsNullableType())
            {
                type = type.GetNullableUnderlyingType();
            }
            switch (type.GetSpecialTypeSafe())
            {
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_IntPtr:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsUnsignedIntegralType(TypeSymbol type)
        {
            if ((object)type != null && type.IsNullableType())
            {
                type = type.GetNullableUnderlyingType();
            }
            switch (type.GetSpecialTypeSafe())
            {
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_UIntPtr:
                    return true;
                default:
                    return false;
            }
        }

        internal static void GetEffectiveParameterTypes(MethodSymbol method, int argumentCount, ImmutableArray<int> argToParamMap, ArrayBuilder<RefKind> argumentRefKinds, bool isMethodGroupConversion, bool allowRefOmittedArguments, Binder binder, bool expanded, out ImmutableArray<TypeWithAnnotations> parameterTypes, out ImmutableArray<RefKind> parameterRefKinds)
        {
            EffectiveParameters effectiveParameters = (expanded ? GetEffectiveParametersInExpandedForm(method, argumentCount, argToParamMap, argumentRefKinds, isMethodGroupConversion, allowRefOmittedArguments, binder, out bool hasAnyRefOmittedArgument) : GetEffectiveParametersInNormalForm(method, argumentCount, argToParamMap, argumentRefKinds, isMethodGroupConversion, allowRefOmittedArguments, binder, out hasAnyRefOmittedArgument));
            parameterTypes = effectiveParameters.ParameterTypes;
            parameterRefKinds = effectiveParameters.ParameterRefKinds;
        }

        private EffectiveParameters GetEffectiveParametersInNormalForm<TMember>(TMember member, int argumentCount, ImmutableArray<int> argToParamMap, ArrayBuilder<RefKind> argumentRefKinds, bool isMethodGroupConversion, bool allowRefOmittedArguments) where TMember : Symbol
        {
            return GetEffectiveParametersInNormalForm(member, argumentCount, argToParamMap, argumentRefKinds, isMethodGroupConversion, allowRefOmittedArguments, _binder, out bool hasAnyRefOmittedArgument);
        }

        private static EffectiveParameters GetEffectiveParametersInNormalForm<TMember>(TMember member, int argumentCount, ImmutableArray<int> argToParamMap, ArrayBuilder<RefKind> argumentRefKinds, bool isMethodGroupConversion, bool allowRefOmittedArguments, Binder binder, out bool hasAnyRefOmittedArgument) where TMember : Symbol
        {
            hasAnyRefOmittedArgument = false;
            ImmutableArray<ParameterSymbol> parameters = member.GetParameters();
            int num = member.GetParameterCount() + (member.GetIsVararg() ? 1 : 0);
            if (argumentCount == num && argToParamMap.IsDefaultOrEmpty)
            {
                ImmutableArray<RefKind> parameterRefKinds = member.GetParameterRefKinds();
                if (parameterRefKinds.IsDefaultOrEmpty)
                {
                    return new EffectiveParameters(member.GetParameterTypes(), parameterRefKinds);
                }
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<RefKind> arrayBuilder = null;
            bool flag = argumentRefKinds.Any();
            for (int i = 0; i < argumentCount; i++)
            {
                int num2 = (argToParamMap.IsDefault ? i : argToParamMap[i]);
                if (num2 >= parameters.Length)
                {
                    continue;
                }
                ParameterSymbol parameterSymbol = parameters[num2];
                instance.Add(parameterSymbol.TypeWithAnnotations);
                RefKind argRefKind = (flag ? argumentRefKinds[i] : RefKind.None);
                RefKind effectiveParameterRefKind = GetEffectiveParameterRefKind(parameterSymbol, argRefKind, isMethodGroupConversion, allowRefOmittedArguments, binder, ref hasAnyRefOmittedArgument);
                if (arrayBuilder == null)
                {
                    if (effectiveParameterRefKind != 0)
                    {
                        arrayBuilder = ArrayBuilder<RefKind>.GetInstance(i, RefKind.None);
                        arrayBuilder.Add(effectiveParameterRefKind);
                    }
                }
                else
                {
                    arrayBuilder.Add(effectiveParameterRefKind);
                }
            }
            ImmutableArray<RefKind> refKinds = arrayBuilder?.ToImmutableAndFree() ?? default(ImmutableArray<RefKind>);
            return new EffectiveParameters(instance.ToImmutableAndFree(), refKinds);
        }

        private static RefKind GetEffectiveParameterRefKind(ParameterSymbol parameter, RefKind argRefKind, bool isMethodGroupConversion, bool allowRefOmittedArguments, Binder binder, ref bool hasAnyRefOmittedArgument)
        {
            RefKind refKind = parameter.RefKind;
            if (!isMethodGroupConversion && argRefKind == RefKind.None && refKind == RefKind.In)
            {
                return RefKind.None;
            }
            if (allowRefOmittedArguments && refKind == RefKind.Ref && argRefKind == RefKind.None && !binder.InAttributeArgument)
            {
                hasAnyRefOmittedArgument = true;
                return RefKind.None;
            }
            return refKind;
        }

        private EffectiveParameters GetEffectiveParametersInExpandedForm<TMember>(TMember member, int argumentCount, ImmutableArray<int> argToParamMap, ArrayBuilder<RefKind> argumentRefKinds, bool isMethodGroupConversion, bool allowRefOmittedArguments) where TMember : Symbol
        {
            return GetEffectiveParametersInExpandedForm(member, argumentCount, argToParamMap, argumentRefKinds, isMethodGroupConversion, allowRefOmittedArguments, _binder, out bool hasAnyRefOmittedArgument);
        }

        private static EffectiveParameters GetEffectiveParametersInExpandedForm<TMember>(TMember member, int argumentCount, ImmutableArray<int> argToParamMap, ArrayBuilder<RefKind> argumentRefKinds, bool isMethodGroupConversion, bool allowRefOmittedArguments, Binder binder, out bool hasAnyRefOmittedArgument) where TMember : Symbol
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<RefKind> instance2 = ArrayBuilder<RefKind>.GetInstance();
            bool flag = false;
            ImmutableArray<ParameterSymbol> parameters = member.GetParameters();
            bool flag2 = argumentRefKinds.Any();
            hasAnyRefOmittedArgument = false;
            for (int i = 0; i < argumentCount; i++)
            {
                int num = (argToParamMap.IsDefault ? i : argToParamMap[i]);
                ParameterSymbol parameterSymbol = parameters[num];
                TypeWithAnnotations typeWithAnnotations = parameterSymbol.TypeWithAnnotations;
                instance.Add((num == parameters.Length - 1) ? ((ArrayTypeSymbol)typeWithAnnotations.Type).ElementTypeWithAnnotations : typeWithAnnotations);
                RefKind argRefKind = (flag2 ? argumentRefKinds[i] : RefKind.None);
                RefKind effectiveParameterRefKind = GetEffectiveParameterRefKind(parameterSymbol, argRefKind, isMethodGroupConversion, allowRefOmittedArguments, binder, ref hasAnyRefOmittedArgument);
                instance2.Add(effectiveParameterRefKind);
                if (effectiveParameterRefKind != 0)
                {
                    flag = true;
                }
            }
            ImmutableArray<RefKind> refKinds = (flag ? instance2.ToImmutable() : default(ImmutableArray<RefKind>));
            instance2.Free();
            return new EffectiveParameters(instance.ToImmutableAndFree(), refKinds);
        }

        private MemberResolutionResult<TMember> IsMemberApplicableInNormalForm<TMember>(TMember member, TMember leastOverriddenMember, ArrayBuilder<TypeWithAnnotations> typeArguments, AnalyzedArguments arguments, bool isMethodGroupConversion, bool allowRefOmittedArguments, bool inferWithDynamic, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            ArgumentAnalysisResult argAnalysis = AnalyzeArguments(member, arguments, isMethodGroupConversion, expanded: false);
            if (!argAnalysis.IsValid)
            {
                ArgumentAnalysisResultKind kind = argAnalysis.Kind;
                if ((kind != ArgumentAnalysisResultKind.NoCorrespondingParameter && kind - 4 > ArgumentAnalysisResultKind.Expanded) || !completeResults)
                {
                    return new MemberResolutionResult<TMember>(member, leastOverriddenMember, MemberAnalysisResult.ArgumentParameterMismatch(argAnalysis));
                }
            }
            if (member.HasUseSiteError)
            {
                return new MemberResolutionResult<TMember>(member, leastOverriddenMember, MemberAnalysisResult.UseSiteError());
            }
            EffectiveParameters effectiveParametersInNormalForm = GetEffectiveParametersInNormalForm(GetConstructedFrom(leastOverriddenMember), arguments.Arguments.Count, argAnalysis.ArgsToParamsOpt, arguments.RefKinds, isMethodGroupConversion, allowRefOmittedArguments, _binder, out bool hasAnyRefOmittedArgument);
            EffectiveParameters effectiveParametersInNormalForm2 = GetEffectiveParametersInNormalForm(leastOverriddenMember, arguments.Arguments.Count, argAnalysis.ArgsToParamsOpt, arguments.RefKinds, isMethodGroupConversion, allowRefOmittedArguments);
            MemberResolutionResult<TMember> result = IsApplicable(member, leastOverriddenMember, typeArguments, arguments, effectiveParametersInNormalForm, effectiveParametersInNormalForm2, argAnalysis.ArgsToParamsOpt, hasAnyRefOmittedArgument, inferWithDynamic, completeResults, ref useSiteInfo);
            if (completeResults && !argAnalysis.IsValid)
            {
                return new MemberResolutionResult<TMember>(member, leastOverriddenMember, MemberAnalysisResult.ArgumentParameterMismatch(argAnalysis));
            }
            return result;
        }

        private MemberResolutionResult<TMember> IsMemberApplicableInExpandedForm<TMember>(TMember member, TMember leastOverriddenMember, ArrayBuilder<TypeWithAnnotations> typeArguments, AnalyzedArguments arguments, bool allowRefOmittedArguments, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            ArgumentAnalysisResult argAnalysis = AnalyzeArguments(member, arguments, isMethodGroupConversion: false, expanded: true);
            if (!argAnalysis.IsValid)
            {
                return new MemberResolutionResult<TMember>(member, leastOverriddenMember, MemberAnalysisResult.ArgumentParameterMismatch(argAnalysis));
            }
            if (member.HasUseSiteError)
            {
                return new MemberResolutionResult<TMember>(member, leastOverriddenMember, MemberAnalysisResult.UseSiteError());
            }
            EffectiveParameters effectiveParametersInExpandedForm = GetEffectiveParametersInExpandedForm(GetConstructedFrom(leastOverriddenMember), arguments.Arguments.Count, argAnalysis.ArgsToParamsOpt, arguments.RefKinds, isMethodGroupConversion: false, allowRefOmittedArguments, _binder, out bool hasAnyRefOmittedArgument);
            EffectiveParameters effectiveParametersInExpandedForm2 = GetEffectiveParametersInExpandedForm(leastOverriddenMember, arguments.Arguments.Count, argAnalysis.ArgsToParamsOpt, arguments.RefKinds, isMethodGroupConversion: false, allowRefOmittedArguments);
            MemberResolutionResult<TMember> result = IsApplicable(member, leastOverriddenMember, typeArguments, arguments, effectiveParametersInExpandedForm, effectiveParametersInExpandedForm2, argAnalysis.ArgsToParamsOpt, hasAnyRefOmittedArgument, inferWithDynamic: false, completeResults, ref useSiteInfo);
            if (!result.Result.IsValid)
            {
                return result;
            }
            return new MemberResolutionResult<TMember>(result.Member, result.LeastOverriddenMember, MemberAnalysisResult.ExpandedForm(result.Result.ArgsToParamsOpt, result.Result.ConversionsOpt, hasAnyRefOmittedArgument));
        }

        private MemberResolutionResult<TMember> IsApplicable<TMember>(TMember member, TMember leastOverriddenMember, ArrayBuilder<TypeWithAnnotations> typeArgumentsBuilder, AnalyzedArguments arguments, EffectiveParameters originalEffectiveParameters, EffectiveParameters constructedEffectiveParameters, ImmutableArray<int> argsToParamsMap, bool hasAnyRefOmittedArgument, bool inferWithDynamic, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo) where TMember : Symbol
        {
            MethodSymbol methodSymbol;
            bool ignoreOpenTypes;
            EffectiveParameters parameters;
            if (member.Kind == SymbolKind.Method && (methodSymbol = (MethodSymbol)(object)member).Arity > 0)
            {
                if (typeArgumentsBuilder.Count == 0 && arguments.HasDynamicArgument && !inferWithDynamic)
                {
                    ignoreOpenTypes = true;
                    parameters = constructedEffectiveParameters;
                }
                else
                {
                    MethodSymbol methodSymbol2 = (MethodSymbol)(object)leastOverriddenMember;
                    ImmutableArray<TypeWithAnnotations> immutableArray;
                    if (typeArgumentsBuilder.Count > 0)
                    {
                        immutableArray = typeArgumentsBuilder.ToImmutable();
                    }
                    else
                    {
                        immutableArray = InferMethodTypeArguments(methodSymbol, methodSymbol2.ConstructedFrom.TypeParameters, arguments, originalEffectiveParameters, out var error, ref useSiteInfo);
                        if (immutableArray.IsDefault)
                        {
                            return new MemberResolutionResult<TMember>(member, leastOverriddenMember, error);
                        }
                    }
                    member = (TMember)(Symbol)methodSymbol.Construct(immutableArray);
                    leastOverriddenMember = (TMember)(Symbol)methodSymbol2.ConstructedFrom.Construct(immutableArray);
                    ImmutableArray<TypeWithAnnotations> parameterTypes = leastOverriddenMember.GetParameterTypes();
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        if (!parameterTypes[i].Type.CheckAllConstraints(Compilation, Conversions))
                        {
                            return new MemberResolutionResult<TMember>(member, leastOverriddenMember, MemberAnalysisResult.ConstructedParameterFailedConstraintsCheck(i));
                        }
                    }
                    TypeMap typeMap = new TypeMap(methodSymbol.TypeParameters, immutableArray, allowAlpha: true);
                    parameters = new EffectiveParameters(typeMap.SubstituteTypes(constructedEffectiveParameters.ParameterTypes), constructedEffectiveParameters.ParameterRefKinds);
                    ignoreOpenTypes = false;
                }
            }
            else
            {
                parameters = constructedEffectiveParameters;
                ignoreOpenTypes = false;
            }
            MemberAnalysisResult result = IsApplicable(member, parameters, arguments, argsToParamsMap, member.GetIsVararg(), hasAnyRefOmittedArgument, ignoreOpenTypes, completeResults, ref useSiteInfo);
            return new MemberResolutionResult<TMember>(member, leastOverriddenMember, result);
        }

        private ImmutableArray<TypeWithAnnotations> InferMethodTypeArguments(MethodSymbol method, ImmutableArray<TypeParameterSymbol> originalTypeParameters, AnalyzedArguments arguments, EffectiveParameters originalEffectiveParameters, out MemberAnalysisResult error, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ImmutableArray<BoundExpression> arguments2 = arguments.Arguments.ToImmutable();
            MethodTypeInferenceResult methodTypeInferenceResult = MethodTypeInferrer.Infer(_binder, _binder.Conversions, originalTypeParameters, method.ContainingType, originalEffectiveParameters.ParameterTypes, originalEffectiveParameters.ParameterRefKinds, arguments2, ref useSiteInfo);
            if (methodTypeInferenceResult.Success)
            {
                error = default(MemberAnalysisResult);
                return methodTypeInferenceResult.InferredTypeArguments;
            }
            if (arguments.IsExtensionMethodInvocation && MethodTypeInferrer.InferTypeArgumentsFromFirstArgument(_binder.Conversions, method, arguments2, ref useSiteInfo).IsDefault)
            {
                error = MemberAnalysisResult.TypeInferenceExtensionInstanceArgumentFailed();
                return default(ImmutableArray<TypeWithAnnotations>);
            }
            error = MemberAnalysisResult.TypeInferenceFailed();
            return default(ImmutableArray<TypeWithAnnotations>);
        }

        private MemberAnalysisResult IsApplicable(Symbol candidate, EffectiveParameters parameters, AnalyzedArguments arguments, ImmutableArray<int> argsToParameters, bool isVararg, bool hasAnyRefOmittedArgument, bool ignoreOpenTypes, bool completeResults, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            int num = parameters.ParameterTypes.Length + (isVararg ? 1 : 0);
            if (arguments.Arguments.Count < num)
            {
                num = arguments.Arguments.Count;
            }
            ArrayBuilder<Conversion> arrayBuilder = null;
            ArrayBuilder<int> arrayBuilder2 = null;
            for (int i = 0; i < num; i++)
            {
                BoundExpression boundExpression = arguments.Argument(i);
                Conversion conversion;
                if (isVararg && i == num - 1)
                {
                    if (boundExpression.Kind == BoundKind.ArgListOperator)
                    {
                        conversion = Conversion.Identity;
                    }
                    else
                    {
                        arrayBuilder2 = arrayBuilder2 ?? ArrayBuilder<int>.GetInstance();
                        arrayBuilder2.Add(i);
                        conversion = Conversion.NoConversion;
                    }
                }
                else
                {
                    RefKind argRefKind = arguments.RefKind(i);
                    RefKind refKind = ((!parameters.ParameterRefKinds.IsDefault) ? parameters.ParameterRefKinds[i] : RefKind.None);
                    bool flag = arguments.IsExtensionMethodThisArgument(i);
                    if (flag && refKind == RefKind.Ref)
                    {
                        argRefKind = refKind;
                    }
                    conversion = CheckArgumentForApplicability(candidate, boundExpression, argRefKind, parameters.ParameterTypes[i].Type, refKind, ignoreOpenTypes, ref useSiteInfo, flag);
                    if (flag && !ConversionsBase.IsValidExtensionMethodThisArgConversion(conversion))
                    {
                        return MemberAnalysisResult.BadArgumentConversions(argsToParameters, ImmutableArray.Create(i), ImmutableArray.Create(conversion));
                    }
                    if (!conversion.Exists)
                    {
                        arrayBuilder2 = arrayBuilder2 ?? ArrayBuilder<int>.GetInstance();
                        arrayBuilder2.Add(i);
                    }
                }
                if (arrayBuilder != null)
                {
                    arrayBuilder.Add(conversion);
                }
                else if (!conversion.IsIdentity)
                {
                    arrayBuilder = ArrayBuilder<Conversion>.GetInstance(num);
                    arrayBuilder.AddMany(Conversion.Identity, i);
                    arrayBuilder.Add(conversion);
                }
                if (arrayBuilder2 != null && !completeResults)
                {
                    break;
                }
            }
            ImmutableArray<Conversion> conversions = arrayBuilder?.ToImmutableAndFree() ?? default(ImmutableArray<Conversion>);
            if (arrayBuilder2 != null)
            {
                return MemberAnalysisResult.BadArgumentConversions(argsToParameters, arrayBuilder2.ToImmutableAndFree(), conversions);
            }
            return MemberAnalysisResult.NormalForm(argsToParameters, conversions, hasAnyRefOmittedArgument);
        }

        private Conversion CheckArgumentForApplicability(Symbol candidate, BoundExpression argument, RefKind argRefKind, TypeSymbol parameterType, RefKind parRefKind, bool ignoreOpenTypes, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forExtensionMethodThisArg)
        {
            if (argRefKind != parRefKind && (argRefKind != 0 || !argument.HasDynamicType()))
            {
                return Conversion.NoConversion;
            }
            if (ignoreOpenTypes && parameterType.ContainsTypeParameter((MethodSymbol)candidate))
            {
                return Conversion.ImplicitDynamic;
            }
            TypeSymbol type = argument.Type;
            if (argument.Kind == BoundKind.OutVariablePendingInference || argument.Kind == BoundKind.OutDeconstructVarPendingInference || (argument.Kind == BoundKind.DiscardExpression && (object)type == null))
            {
                return Conversion.Identity;
            }
            if (argRefKind == RefKind.None)
            {
                if (!forExtensionMethodThisArg)
                {
                    return Conversions.ClassifyImplicitConversionFromExpression(argument, parameterType, ref useSiteInfo);
                }
                return Conversions.ClassifyImplicitExtensionMethodThisArgConversion(argument, argument.Type, parameterType, ref useSiteInfo);
            }
            if ((object)type != null && ConversionsBase.HasIdentityConversion(type, parameterType))
            {
                return Conversion.Identity;
            }
            return Conversion.NoConversion;
        }

        private static TMember GetConstructedFrom<TMember>(TMember member) where TMember : Symbol
        {
            return member.Kind switch
            {
                SymbolKind.Property => member,
                SymbolKind.Method => (TMember)(Symbol)(member as MethodSymbol).ConstructedFrom,
                _ => throw ExceptionUtilities.UnexpectedValue(member.Kind),
            };
        }

        private static ArgumentAnalysisResult AnalyzeArguments(Symbol symbol, AnalyzedArguments arguments, bool isMethodGroupConversion, bool expanded)
        {
            ImmutableArray<ParameterSymbol> parameters = symbol.GetParameters();
            bool isVararg = symbol.GetIsVararg();
            if (!expanded && arguments.Names.Count == 0)
            {
                return AnalyzeArgumentsForNormalFormNoNamedArguments(parameters, arguments, isMethodGroupConversion, isVararg);
            }
            int count = arguments.Arguments.Count;
            int[] array = null;
            int? num = null;
            bool? flag = null;
            bool seenNamedParams = false;
            bool seenOutOfPositionNamedArgument = false;
            bool isValidParams = IsValidParams(symbol);
            for (int i = 0; i < count; i++)
            {
                int num2 = CorrespondsToAnyParameter(parameters, expanded, arguments, i, isValidParams, isVararg, out bool isNamedArgument, ref seenNamedParams, ref seenOutOfPositionNamedArgument) ?? (-1);
                if (num2 == -1 && !num.HasValue)
                {
                    num = i;
                    flag = isNamedArgument;
                }
                if (num2 != i && array == null)
                {
                    array = new int[count];
                    for (int j = 0; j < i; j++)
                    {
                        array[j] = j;
                    }
                }
                if (array != null)
                {
                    array[i] = num2;
                }
            }
            ParameterMap argsToParameters = new ParameterMap(array, count);
            int? num3 = CheckForBadNonTrailingNamedArgument(arguments, argsToParameters, parameters);
            if (num3.HasValue)
            {
                return ArgumentAnalysisResult.BadNonTrailingNamedArgument(num3.Value);
            }
            if (num.HasValue)
            {
                if (flag.Value)
                {
                    return ArgumentAnalysisResult.NoCorrespondingNamedParameter(num.Value);
                }
                return ArgumentAnalysisResult.NoCorrespondingParameter(num.Value);
            }
            int? num4 = NameUsedForPositional(arguments, argsToParameters);
            if (num4.HasValue)
            {
                return ArgumentAnalysisResult.NameUsedForPositional(num4.Value);
            }
            int? num5 = CheckForMissingRequiredParameter(argsToParameters, parameters, isMethodGroupConversion, expanded);
            if (num5.HasValue)
            {
                return ArgumentAnalysisResult.RequiredParameterMissing(num5.Value);
            }
            if (arguments.Names.Any() && arguments.Names.Last() != null && isVararg)
            {
                return ArgumentAnalysisResult.RequiredParameterMissing(parameters.Length);
            }
            int? num6 = CheckForDuplicateNamedArgument(arguments);
            if (num6.HasValue)
            {
                return ArgumentAnalysisResult.DuplicateNamedArgument(num6.Value);
            }
            if (!expanded)
            {
                return ArgumentAnalysisResult.NormalForm(argsToParameters.ToImmutableArray());
            }
            return ArgumentAnalysisResult.ExpandedForm(argsToParameters.ToImmutableArray());
        }

        private static int? CheckForBadNonTrailingNamedArgument(AnalyzedArguments arguments, ParameterMap argsToParameters, ImmutableArray<ParameterSymbol> parameters)
        {
            if (argsToParameters.IsTrivial)
            {
                return null;
            }
            int num = -1;
            int count = arguments.Arguments.Count;
            for (int i = 0; i < count; i++)
            {
                int num2 = argsToParameters[i];
                if (num2 != -1 && num2 != i && arguments.Name(i) != null)
                {
                    num = i;
                    break;
                }
            }
            if (num != -1)
            {
                for (int j = num + 1; j < count; j++)
                {
                    if (arguments.Name(j) == null)
                    {
                        return num;
                    }
                }
            }
            return null;
        }

        private static int? CorrespondsToAnyParameter(ImmutableArray<ParameterSymbol> memberParameters, bool expanded, AnalyzedArguments arguments, int argumentPosition, bool isValidParams, bool isVararg, out bool isNamedArgument, ref bool seenNamedParams, ref bool seenOutOfPositionNamedArgument)
        {
            isNamedArgument = arguments.Names.Count > argumentPosition && arguments.Names[argumentPosition] != null;
            if (!isNamedArgument)
            {
                if (seenNamedParams)
                {
                    return null;
                }
                if (seenOutOfPositionNamedArgument)
                {
                    return null;
                }
                int num = memberParameters.Length + (isVararg ? 1 : 0);
                if (argumentPosition >= num)
                {
                    if (!expanded)
                    {
                        return null;
                    }
                    return num - 1;
                }
                return argumentPosition;
            }
            IdentifierNameSyntax identifierNameSyntax = arguments.Names[argumentPosition];
            for (int i = 0; i < memberParameters.Length; i++)
            {
                if (memberParameters[i].Name == identifierNameSyntax.Identifier.ValueText)
                {
                    if (isValidParams && i == memberParameters.Length - 1)
                    {
                        seenNamedParams = true;
                    }
                    if (i != argumentPosition)
                    {
                        seenOutOfPositionNamedArgument = true;
                    }
                    return i;
                }
            }
            return null;
        }

        private static ArgumentAnalysisResult AnalyzeArgumentsForNormalFormNoNamedArguments(ImmutableArray<ParameterSymbol> parameters, AnalyzedArguments arguments, bool isMethodGroupConversion, bool isVararg)
        {
            int num = parameters.Length + (isVararg ? 1 : 0);
            int count = arguments.Arguments.Count;
            if (count < num)
            {
                for (int i = count; i < num; i++)
                {
                    if (parameters.Length == i || !CanBeOptional(parameters[i], isMethodGroupConversion))
                    {
                        return ArgumentAnalysisResult.RequiredParameterMissing(i);
                    }
                }
            }
            else if (num < count)
            {
                return ArgumentAnalysisResult.NoCorrespondingParameter(num);
            }
            return ArgumentAnalysisResult.NormalForm(default(ImmutableArray<int>));
        }

        private static bool CanBeOptional(ParameterSymbol parameter, bool isMethodGroupConversion)
        {
            if (!isMethodGroupConversion)
            {
                return parameter.IsOptional;
            }
            return false;
        }

        private static int? NameUsedForPositional(AnalyzedArguments arguments, ParameterMap argsToParameters)
        {
            if (argsToParameters.IsTrivial)
            {
                return null;
            }
            for (int i = 0; i < argsToParameters.Length; i++)
            {
                if (arguments.Name(i) == null)
                {
                    continue;
                }
                for (int j = 0; j < i; j++)
                {
                    if (arguments.Name(j) == null && argsToParameters[i] == argsToParameters[j])
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        private static int? CheckForMissingRequiredParameter(ParameterMap argsToParameters, ImmutableArray<ParameterSymbol> parameters, bool isMethodGroupConversion, bool expanded)
        {
            int num = (expanded ? (parameters.Length - 1) : parameters.Length);
            if (argsToParameters.IsTrivial && num <= argsToParameters.Length)
            {
                return null;
            }
            for (int i = 0; i < num; i++)
            {
                if (CanBeOptional(parameters[i], isMethodGroupConversion))
                {
                    continue;
                }
                bool flag = false;
                for (int j = 0; j < argsToParameters.Length; j++)
                {
                    flag = argsToParameters[j] == i;
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    return i;
                }
            }
            return null;
        }

        private static int? CheckForDuplicateNamedArgument(AnalyzedArguments arguments)
        {
            if (arguments.Names.IsEmpty())
            {
                return null;
            }
            PooledHashSet<string> instance = PooledHashSet<string>.GetInstance();
            for (int i = 0; i < arguments.Names.Count; i++)
            {
                string text = arguments.Name(i);
                if (text != null && !instance.Add(text))
                {
                    instance.Free();
                    return i;
                }
            }
            instance.Free();
            return null;
        }
    }
}
