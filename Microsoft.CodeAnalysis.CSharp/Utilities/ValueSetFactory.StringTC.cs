// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static partial class ValueSetFactory
    {
        private struct StringTC : IEquatableValueTC<string>
        {
            string IEquatableValueTC<string>.FromConstantValue(ConstantValue constantValue)
            {
                var result = constantValue.IsBad ? string.Empty : constantValue.StringValue;
                return result;
            }

            string[] IEquatableValueTC<string>.RandomValues(int count, Random random, int scope)
            {
                string[] result = new string[count];
                int next = 0;
                for (int i = 0; i < scope; i++)
                {
                    int need = count - next;
                    int remain = scope - i;
                    if (random.NextDouble() * remain < need)
                    {
                        result[next++] = i.ToString();
                    }
                }

                return result;
            }

            ConstantValue IEquatableValueTC<string>.ToConstantValue(string value) => ConstantValue.Create(value);
        }
    }
}
