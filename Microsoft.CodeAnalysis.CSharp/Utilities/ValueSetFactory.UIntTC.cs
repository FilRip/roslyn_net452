// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    using static BinaryOperatorKind;

    internal static partial class ValueSetFactory
    {
        private struct UIntTC : INumericTC<uint>
        {
            uint INumericTC<uint>.MinValue => uint.MinValue;

            uint INumericTC<uint>.MaxValue => uint.MaxValue;

            uint INumericTC<uint>.Zero => 0;

            public bool Related(BinaryOperatorKind relation, uint left, uint right)
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

            uint INumericTC<uint>.Next(uint value)
            {
                return value + 1;
            }

            public uint FromConstantValue(ConstantValue constantValue) => constantValue.IsBad ? 0 : constantValue.UInt32Value;

            public ConstantValue ToConstantValue(uint value) => ConstantValue.Create(value);

            string INumericTC<uint>.ToString(uint value) => value.ToString();

            uint INumericTC<uint>.Prev(uint value)
            {
                return value - 1;
            }

            public uint Random(Random random)
            {
                return (uint)((random.Next() << 10) ^ random.Next());
            }
        }
    }
}
