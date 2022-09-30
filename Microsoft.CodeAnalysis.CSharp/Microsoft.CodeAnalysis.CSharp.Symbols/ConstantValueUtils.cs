using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class ConstantValueUtils
    {
        private sealed class CheckConstantInterpolatedStringValidity : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
        {
            internal readonly BindingDiagnosticBag diagnostics;

            public CheckConstantInterpolatedStringValidity(BindingDiagnosticBag diagnostics)
            {
                this.diagnostics = diagnostics;
            }

            public override BoundNode VisitInterpolatedString(BoundInterpolatedString node)
            {
                Binder.CheckFeatureAvailability(node.Syntax, MessageID.IDS_FeatureConstantInterpolatedStrings, diagnostics);
                return null;
            }
        }

        public static ConstantValue EvaluateFieldConstant(SourceFieldSymbol symbol, EqualsValueClauseSyntax equalsValueNode, HashSet<SourceFieldSymbolWithSyntaxReference> dependencies, bool earlyDecodingWellKnownAttributes, BindingDiagnosticBag diagnostics)
        {
            Binder binder = symbol.DeclaringCompilation.GetBinderFactory(symbol.Locations[0].SourceTree).GetBinder(equalsValueNode);
            if (earlyDecodingWellKnownAttributes)
            {
                binder = new EarlyWellKnownAttributeBinder(binder);
            }
            BoundFieldEqualsValue boundFieldEqualsValue = BindFieldOrEnumInitializer(new ConstantFieldsInProgressBinder(new ConstantFieldsInProgress(symbol, dependencies), binder), symbol, equalsValueNode, diagnostics);
            return GetAndValidateConstantValue(initValueNodeLocation: equalsValueNode.Value.Location, boundValue: boundFieldEqualsValue.Value, thisSymbol: symbol, typeSymbol: symbol.Type, diagnostics: diagnostics);
        }

        private static BoundFieldEqualsValue BindFieldOrEnumInitializer(Binder binder, FieldSymbol fieldSymbol, EqualsValueClauseSyntax initializer, BindingDiagnosticBag diagnostics)
        {
            SourceEnumConstantSymbol sourceEnumConstantSymbol = fieldSymbol as SourceEnumConstantSymbol;
            Binder next = new LocalScopeBinder(binder);
            next = new ExecutableCodeBinder(initializer, fieldSymbol, next);
            if ((object)sourceEnumConstantSymbol != null)
            {
                return next.BindEnumConstantInitializer(sourceEnumConstantSymbol, initializer, diagnostics);
            }
            return next.BindFieldInitializer(fieldSymbol, initializer, diagnostics);
        }

        internal static string UnescapeInterpolatedStringLiteral(string s)
        {
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            StringBuilder builder = instance.Builder;
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                char c = s[i];
                builder.Append(c);
                if ((c == '{' || c == '}') && i + 1 < length && s[i + 1] == c)
                {
                    i++;
                }
            }
            return instance.ToStringAndFree();
        }

        internal static ConstantValue GetAndValidateConstantValue(BoundExpression boundValue, Symbol thisSymbol, TypeSymbol typeSymbol, Location initValueNodeLocation, BindingDiagnosticBag diagnostics)
        {
            ConstantValue result = ConstantValue.Bad;
            CheckLangVersionForConstantValue(boundValue, diagnostics);
            if (!boundValue.HasAnyErrors)
            {
                if (typeSymbol.TypeKind == TypeKind.TypeParameter)
                {
                    diagnostics.Add(ErrorCode.ERR_InvalidConstantDeclarationType, initValueNodeLocation, thisSymbol, typeSymbol);
                }
                else
                {
                    bool flag = false;
                    BoundExpression boundExpression = boundValue;
                    while (boundExpression.Kind == BoundKind.Conversion)
                    {
                        BoundConversion boundConversion = (BoundConversion)boundExpression;
                        flag = flag || boundConversion.ConversionKind.IsDynamic();
                        boundExpression = boundConversion.Operand;
                    }
                    ConstantValue constantValue = boundValue.ConstantValue;
                    ConstantValue constantValue2 = boundExpression.ConstantValue;
                    if (constantValue2 != null && !constantValue2.IsNull && typeSymbol.IsReferenceType && typeSymbol.SpecialType != SpecialType.System_String)
                    {
                        diagnostics.Add(ErrorCode.ERR_NotNullConstRefField, initValueNodeLocation, thisSymbol, typeSymbol);
                        constantValue = constantValue ?? constantValue2;
                    }
                    if (constantValue != null && !flag)
                    {
                        result = constantValue;
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_NotConstantExpression, initValueNodeLocation, thisSymbol);
                    }
                }
            }
            return result;
        }

        internal static void CheckLangVersionForConstantValue(BoundExpression expression, BindingDiagnosticBag diagnostics)
        {
            if ((object)expression.Type != null && expression.Type.IsStringType())
            {
                new CheckConstantInterpolatedStringValidity(diagnostics).Visit(expression);
            }
        }
    }
}
