// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    using static BinaryOperatorKind;

    internal static partial class ValueSetFactory
    {
        private struct ULongTC : INumericTC<ulong>
        {
            ulong INumericTC<ulong>.MinValue => ulong.MinValue;

            ulong INumericTC<ulong>.MaxValue => ulong.MaxValue;

            ulong INumericTC<ulong>.Zero => 0;

            bool INumericTC<ulong>.Related(BinaryOperatorKind relation, ulong left, ulong right)
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

            ulong INumericTC<ulong>.Next(ulong value)
            {
                return value + 1;
            }

            ulong INumericTC<ulong>.Prev(ulong value)
            {
                return value - 1;
            }

            ulong INumericTC<ulong>.FromConstantValue(ConstantValue constantValue) => constantValue.IsBad ? 0UL : constantValue.UInt64Value;

            ConstantValue INumericTC<ulong>.ToConstantValue(ulong value) => ConstantValue.Create(value);

            string INumericTC<ulong>.ToString(ulong value) => value.ToString();

            ulong INumericTC<ulong>.Random(Random random)
            {
                return ((ulong)random.Next() << 35) ^ ((ulong)random.Next() << 10) ^ (ulong)random.Next();
            }
        }
    }
}
