// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    using static BinaryOperatorKind;

    internal static partial class ValueSetFactory
    {
        private struct ShortTC : INumericTC<short>
        {
            short INumericTC<short>.MinValue => short.MinValue;

            short INumericTC<short>.MaxValue => short.MaxValue;

            short INumericTC<short>.Zero => 0;

            bool INumericTC<short>.Related(BinaryOperatorKind relation, short left, short right)
            {
                return relation switch
                {
                    Equal => left == right,
                    GreaterThanOrEqual => left >= right,
                    GreaterThan => left > right,
                    LessThanOrEqual => left <= right,
                    LessThan => left < right,
                    _ => throw new ArgumentException("relation"),
                };
            }

            short INumericTC<short>.Next(short value)
            {
                return (short)(value + 1);
            }

            short INumericTC<short>.Prev(short value)
            {
                return (short)(value - 1);
            }

            short INumericTC<short>.FromConstantValue(ConstantValue constantValue) => constantValue.IsBad ? (short)0 : constantValue.Int16Value;

            ConstantValue INumericTC<short>.ToConstantValue(short value) => ConstantValue.Create(value);

            string INumericTC<short>.ToString(short value) => value.ToString();

            short INumericTC<short>.Random(Random random)
            {
                return (short)random.Next();
            }
        }
    }
}
