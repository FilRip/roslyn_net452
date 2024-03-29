// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax
{
    internal partial class SyntaxList
    {
        internal class SeparatedWithManyChildren : SyntaxList
        {
            private readonly ArrayElement<SyntaxNode?>[] _children;

            internal SeparatedWithManyChildren(InternalSyntax.SyntaxList green, SyntaxNode? parent, int position)
                : base(green, parent, position)
            {
                _children = new ArrayElement<SyntaxNode?>[(green.SlotCount + 1) >> 1];
            }

            public override SyntaxNode? GetNodeSlot(int slot)
            {
                if ((slot & 1) != 0)
                {
                    //separator
                    return null;
                }

                return this.GetRedElement(ref _children[slot >> 1].Value, slot);
            }

            public override SyntaxNode? GetCachedSlot(int index)
            {
                if ((index & 1) != 0)
                {
                    //separator
                    return null;
                }

                return _children[index >> 1].Value;
            }
        }
    }
}
