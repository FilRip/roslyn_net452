// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class OperatorFacts
    {
        public static bool DefinitelyHasNoUserDefinedOperators(TypeSymbol type)
        {
            // We can take an early out and not look for user-defined operators.

            switch (type.TypeKind)
            {
                case TypeKind.Struct:
                case TypeKind.Class:
                case TypeKind.TypeParameter:
                case TypeKind.Interface:
                    break;
                default:
                    return true;
            }

            // System.Decimal does have user-defined operators but it is treated as 
            // though it were a built-in type.
            switch (type.SpecialType)
            {
                case SpecialType.System_Array:
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_Decimal:
                case SpecialType.System_Delegate:
                case SpecialType.System_Double:
                case SpecialType.System_Enum:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_IntPtr when type.IsNativeIntegerType:
                case SpecialType.System_UIntPtr when type.IsNativeIntegerType:
                case SpecialType.System_MulticastDelegate:
                case SpecialType.System_Object:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                case SpecialType.System_String:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_ValueType:
                case SpecialType.System_Void:
                    return true;
            }

            return false;
        }

        public static string BinaryOperatorNameFromSyntaxKind(SyntaxKind kind)
        {
            return BinaryOperatorNameFromSyntaxKindIfAny(kind) ??
                WellKnownMemberNames.AdditionOperatorName; // This can occur in the presence of syntax errors.
        }

        internal static string BinaryOperatorNameFromSyntaxKindIfAny(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.PlusToken => WellKnownMemberNames.AdditionOperatorName,
                SyntaxKind.MinusToken => WellKnownMemberNames.SubtractionOperatorName,
                SyntaxKind.AsteriskToken => WellKnownMemberNames.MultiplyOperatorName,
                SyntaxKind.SlashToken => WellKnownMemberNames.DivisionOperatorName,
                SyntaxKind.PercentToken => WellKnownMemberNames.ModulusOperatorName,
                SyntaxKind.CaretToken => WellKnownMemberNames.ExclusiveOrOperatorName,
                SyntaxKind.AmpersandToken => WellKnownMemberNames.BitwiseAndOperatorName,
                SyntaxKind.BarToken => WellKnownMemberNames.BitwiseOrOperatorName,
                SyntaxKind.EqualsEqualsToken => WellKnownMemberNames.EqualityOperatorName,
                SyntaxKind.LessThanToken => WellKnownMemberNames.LessThanOperatorName,
                SyntaxKind.LessThanEqualsToken => WellKnownMemberNames.LessThanOrEqualOperatorName,
                SyntaxKind.LessThanLessThanToken => WellKnownMemberNames.LeftShiftOperatorName,
                SyntaxKind.GreaterThanToken => WellKnownMemberNames.GreaterThanOperatorName,
                SyntaxKind.GreaterThanEqualsToken => WellKnownMemberNames.GreaterThanOrEqualOperatorName,
                SyntaxKind.GreaterThanGreaterThanToken => WellKnownMemberNames.RightShiftOperatorName,
                SyntaxKind.ExclamationEqualsToken => WellKnownMemberNames.InequalityOperatorName,
                _ => null,
            };
        }

        public static string UnaryOperatorNameFromSyntaxKind(SyntaxKind kind)
        {
            return UnaryOperatorNameFromSyntaxKindIfAny(kind) ??
                WellKnownMemberNames.UnaryPlusOperatorName; // This can occur in the presence of syntax errors.
        }

        internal static string UnaryOperatorNameFromSyntaxKindIfAny(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.PlusToken => WellKnownMemberNames.UnaryPlusOperatorName,
                SyntaxKind.MinusToken => WellKnownMemberNames.UnaryNegationOperatorName,
                SyntaxKind.TildeToken => WellKnownMemberNames.OnesComplementOperatorName,
                SyntaxKind.ExclamationToken => WellKnownMemberNames.LogicalNotOperatorName,
                SyntaxKind.PlusPlusToken => WellKnownMemberNames.IncrementOperatorName,
                SyntaxKind.MinusMinusToken => WellKnownMemberNames.DecrementOperatorName,
                SyntaxKind.TrueKeyword => WellKnownMemberNames.TrueOperatorName,
                SyntaxKind.FalseKeyword => WellKnownMemberNames.FalseOperatorName,
                _ => null,
            };
        }

        public static string OperatorNameFromDeclaration(OperatorDeclarationSyntax declaration)
        {
            return OperatorNameFromDeclaration((Syntax.InternalSyntax.OperatorDeclarationSyntax)(declaration.Green));
        }

        public static string OperatorNameFromDeclaration(Syntax.InternalSyntax.OperatorDeclarationSyntax declaration)
        {
            var opTokenKind = declaration.OperatorToken.Kind;

            if (SyntaxFacts.IsBinaryExpressionOperatorToken(opTokenKind))
            {
                // Some tokens may be either unary or binary operators (e.g. +, -).
                if (SyntaxFacts.IsPrefixUnaryExpressionOperatorToken(opTokenKind) &&
                    declaration.ParameterList.Parameters.Count == 1)
                {
                    return OperatorFacts.UnaryOperatorNameFromSyntaxKind(opTokenKind);
                }

                return OperatorFacts.BinaryOperatorNameFromSyntaxKind(opTokenKind);
            }
            else if (SyntaxFacts.IsUnaryOperatorDeclarationToken(opTokenKind))
            {
                return OperatorFacts.UnaryOperatorNameFromSyntaxKind(opTokenKind);
            }
            else
            {
                // fallback for error recovery
                return WellKnownMemberNames.UnaryPlusOperatorName;
            }
        }

        public static string UnaryOperatorNameFromOperatorKind(UnaryOperatorKind kind)
        {
            return (kind & UnaryOperatorKind.OpMask) switch
            {
                UnaryOperatorKind.UnaryPlus => WellKnownMemberNames.UnaryPlusOperatorName,
                UnaryOperatorKind.UnaryMinus => WellKnownMemberNames.UnaryNegationOperatorName,
                UnaryOperatorKind.BitwiseComplement => WellKnownMemberNames.OnesComplementOperatorName,
                UnaryOperatorKind.LogicalNegation => WellKnownMemberNames.LogicalNotOperatorName,
                UnaryOperatorKind.PostfixIncrement or UnaryOperatorKind.PrefixIncrement => WellKnownMemberNames.IncrementOperatorName,
                UnaryOperatorKind.PostfixDecrement or UnaryOperatorKind.PrefixDecrement => WellKnownMemberNames.DecrementOperatorName,
                UnaryOperatorKind.True => WellKnownMemberNames.TrueOperatorName,
                UnaryOperatorKind.False => WellKnownMemberNames.FalseOperatorName,
                _ => throw ExceptionUtilities.UnexpectedValue(kind & UnaryOperatorKind.OpMask),
            };
        }

        public static string BinaryOperatorNameFromOperatorKind(BinaryOperatorKind kind)
        {
            return (kind & BinaryOperatorKind.OpMask) switch
            {
                BinaryOperatorKind.Addition => WellKnownMemberNames.AdditionOperatorName,
                BinaryOperatorKind.And => WellKnownMemberNames.BitwiseAndOperatorName,
                BinaryOperatorKind.Division => WellKnownMemberNames.DivisionOperatorName,
                BinaryOperatorKind.Equal => WellKnownMemberNames.EqualityOperatorName,
                BinaryOperatorKind.GreaterThan => WellKnownMemberNames.GreaterThanOperatorName,
                BinaryOperatorKind.GreaterThanOrEqual => WellKnownMemberNames.GreaterThanOrEqualOperatorName,
                BinaryOperatorKind.LeftShift => WellKnownMemberNames.LeftShiftOperatorName,
                BinaryOperatorKind.LessThan => WellKnownMemberNames.LessThanOperatorName,
                BinaryOperatorKind.LessThanOrEqual => WellKnownMemberNames.LessThanOrEqualOperatorName,
                BinaryOperatorKind.Multiplication => WellKnownMemberNames.MultiplyOperatorName,
                BinaryOperatorKind.Or => WellKnownMemberNames.BitwiseOrOperatorName,
                BinaryOperatorKind.NotEqual => WellKnownMemberNames.InequalityOperatorName,
                BinaryOperatorKind.Remainder => WellKnownMemberNames.ModulusOperatorName,
                BinaryOperatorKind.RightShift => WellKnownMemberNames.RightShiftOperatorName,
                BinaryOperatorKind.Subtraction => WellKnownMemberNames.SubtractionOperatorName,
                BinaryOperatorKind.Xor => WellKnownMemberNames.ExclusiveOrOperatorName,
                _ => throw ExceptionUtilities.UnexpectedValue(kind & BinaryOperatorKind.OpMask),
            };
        }
    }
}
