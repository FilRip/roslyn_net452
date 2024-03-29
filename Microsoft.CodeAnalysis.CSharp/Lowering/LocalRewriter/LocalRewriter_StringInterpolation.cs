// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed partial class LocalRewriter
    {
        private BoundExpression RewriteInterpolatedStringConversion(BoundConversion conversion)
        {
            MakeInterpolatedStringFormat((BoundInterpolatedString)conversion.Operand, out BoundExpression format, out ArrayBuilder<BoundExpression> expressions);
            expressions.Insert(0, format);
            var stringFactory = _factory.WellKnownType(WellKnownType.System_Runtime_CompilerServices_FormattableStringFactory);

            // The normal pattern for lowering is to lower subtrees before the enclosing tree. However we cannot lower
            // the arguments first in this situation because we do not know what conversions will be
            // produced for the arguments until after we've done overload resolution. So we produce the invocation
            // and then lower it along with its arguments.
            var result = _factory.StaticCall(stringFactory, "Create", expressions.ToImmutableAndFree(),
                allowUnexpandedForm: false // if an interpolation expression is the null literal, it should not match a params parameter.
                );
            if (!result.HasAnyErrors)
            {
                result = VisitExpression(result); // lower the arguments AND handle expanded form, argument conversions, etc.
                result = MakeImplicitConversion(result, conversion.Type);
            }

            return result;
        }

        private bool CanLowerToStringConcatenation(BoundInterpolatedString node)
        {
            foreach (var part in node.Parts)
            {
                if (part is BoundStringInsert fillin)
                {
                    // this is one of the expression holes
                    if (_inExpressionLambda ||
                        fillin.HasErrors ||
                        fillin.Value.Type?.SpecialType != SpecialType.System_String ||
                        fillin.Alignment != null ||
                        fillin.Format != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void MakeInterpolatedStringFormat(BoundInterpolatedString node, out BoundExpression format, out ArrayBuilder<BoundExpression> expressions)
        {
            _factory.Syntax = node.Syntax;
            int n = node.Parts.Length - 1;
            var formatString = PooledStringBuilder.GetInstance();
            var stringBuilder = formatString.Builder;
            expressions = ArrayBuilder<BoundExpression>.GetInstance(n + 1);
            int nextFormatPosition = 0;
            for (int i = 0; i <= n; i++)
            {
                var part = node.Parts[i];
                if (part is not BoundStringInsert fillin)
                {
                    // this is one of the literal parts
#nullable restore
                    stringBuilder.Append(part.ConstantValue.StringValue);
                }
                else
                {
                    // this is one of the expression holes
                    stringBuilder.Append('{').Append(nextFormatPosition++);
                    if (fillin.Alignment != null && !fillin.Alignment.HasErrors)
                    {
                        stringBuilder.Append(',').Append(fillin.Alignment.ConstantValue.Int64Value);
                    }
                    if (fillin.Format != null && !fillin.Format.HasErrors)
                    {
                        stringBuilder.Append(':').Append(fillin.Format.ConstantValue.StringValue);
                    }
                    stringBuilder.Append('}');
                    var value = fillin.Value;
                    if (value.Type?.TypeKind == TypeKind.Dynamic)
                    {
                        value = MakeConversionNode(value, _compilation.ObjectType, @checked: false);
                    }

                    expressions.Add(value); // NOTE: must still be lowered
                }
            }

            format = _factory.StringLiteral(formatString.ToStringAndFree());
        }

        public override BoundNode VisitInterpolatedString(BoundInterpolatedString node)
        {
#nullable enable
            BoundExpression? result;
#nullable restore

            if (CanLowerToStringConcatenation(node))
            {
                // All fill-ins, if any, are strings, and none of them have alignment or format specifiers.
                // We can lower to a more efficient string concatenation
                // The normal pattern for lowering is to lower subtrees before the enclosing tree. However in this case
                // we want to lower the entire concatenation so we get the optimizations done by that lowering (e.g. constant folding).

                int length = node.Parts.Length;
                if (length == 0)
                {
                    // $"" -> ""
                    return _factory.StringLiteral("");
                }

                result = null;
                for (int i = 0; i < length; i++)
                {
                    var part = node.Parts[i];
                    if (part is BoundStringInsert fillin)
                    {
                        // this is one of the filled-in expressions
                        part = fillin.Value;
                    }
                    else
                    {
                        // this is one of the literal parts
                        part = _factory.StringLiteral(ConstantValueUtils.UnescapeInterpolatedStringLiteral(part.ConstantValue.StringValue));
                    }

                    result = result == null ?
                        part :
                        _factory.Binary(BinaryOperatorKind.StringConcatenation, node.Type, result, part);
                }

                if (length == 1)
                {
                    result = _factory.Coalesce(result!, _factory.StringLiteral(""));
                }
            }
            else
            {
                //
                // We lower an interpolated string into an invocation of string.Format.  For example, we translate the expression
                //
                //     $"Jenny don\'t change your number { 8675309 }"
                //
                // into
                //
                //     string.Format("Jenny don\'t change your number {0}", new object[] { 8675309 })
                //

                MakeInterpolatedStringFormat(node, out BoundExpression format, out ArrayBuilder<BoundExpression> expressions);

                // The normal pattern for lowering is to lower subtrees before the enclosing tree. However we cannot lower
                // the arguments first in this situation because we do not know what conversions will be
                // produced for the arguments until after we've done overload resolution. So we produce the invocation
                // and then lower it along with its arguments.
                expressions.Insert(0, format);
                var stringType = node.Type;
                result = _factory.StaticCall(stringType, "Format", expressions.ToImmutableAndFree(),
                    allowUnexpandedForm: false // if an interpolation expression is the null literal, it should not match a params parameter.
                    );
            }

            if (!result.HasAnyErrors)
            {
                result = VisitExpression(result); // lower the arguments AND handle expanded form, argument conversions, etc.
                result = MakeImplicitConversion(result, node.Type);
            }
            return result;
        }
    }
}
