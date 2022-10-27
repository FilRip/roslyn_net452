// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax
{
    internal partial class SyntaxList
    {
        internal class WithTwoChildren : SyntaxList
        {
            private SyntaxNode? _child0;
            private SyntaxNode? _child1;

            internal WithTwoChildren(InternalSyntax.SyntaxList green, SyntaxNode? parent, int position)
                : base(green, parent, position)
            {
            }

            public override SyntaxNode? GetNodeSlot(int index)
            {
                return index switch
                {
                    0 => this.GetRedElement(ref _child0, 0),
                    1 => this.GetRedElementIfNotToken(ref _child1),
                    _ => null,
                };
            }

            public override SyntaxNode? GetCachedSlot(int index)
            {
                return index switch
                {
                    0 => _child0,
                    1 => _child1,
                    _ => null,
                };
            }

        }
    }
}
